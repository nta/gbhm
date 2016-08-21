using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GBH
{
    public class GBH2Map : MapFileBase
    {
        struct Header
        {
            public uint magic;
            public int version;
            public SectionHeader[] sections;

            public void Read(Stream stream)
            {
                var reader = new BinaryReader(stream);
                magic = reader.ReadUInt32();
                version = reader.ReadInt32();

                var sectionCount = reader.ReadInt32();

                sections = new SectionHeader[sectionCount];

                for (int i = 0; i < sections.Length; i++)
                {
                    sections[i].Read(stream);
                }
            }

            public void Write(Stream stream)
            {
                var writer = new BinaryWriter(stream);
                writer.Write(magic);
                writer.Write(version);
                writer.Write(sections.Length);

                for (int i = 0; i < sections.Length; i++)
                {
                    sections[i].Write(stream);
                }
            }

            public SectionHeader FindSection(string key)
            {
                uint magic = KeyNameToMagic(key);
                SectionHeader section = sections.FirstOrDefault(sec => sec.magic == magic);

                if (section.magic == 0)
                {
                    throw new Exception($"No such section {key}.");
                }

                return section;
            }

            public static uint KeyNameToMagic(string key)
            {
                if (key.Length != 4)
                {
                    throw new ArgumentException("Keys should be 4 characters");
                }

                return BitConverter.ToUInt32(Encoding.ASCII.GetBytes(key).ToArray(), 0);
            }
        }

        struct SectionHeader
        {
            public uint magic;
            public int version;
            public int offset;
            public int size;

            public void Read(Stream stream)
            {
                var reader = new BinaryReader(stream);
                magic = reader.ReadUInt32();
                version = reader.ReadInt32();
                offset = reader.ReadInt32();
                size = reader.ReadInt32();
            }

            public void Write(Stream stream)
            {
                var writer = new BinaryWriter(stream);
                writer.Write(magic);
                writer.Write(version);
                writer.Write(offset);
                writer.Write(size);
            }
        }

        public GBH2Map()
            : base()
        {

        }

        public override void Load(Stream mapFile)
        {
            long startPosition = mapFile.Position;

            Header header = new Header();
            header.Read(mapFile);

            // check magic
            if (header.magic != 0x504D3247)
            {
                throw new Exception("Not a G2MP.");
            }

            // read compressed map
            SectionHeader mapSection = header.FindSection("CMAP");
            mapFile.Position = startPosition + mapSection.offset;

            Map = CompressedMap.ReadFromStream(mapFile);

            // read lights
            mapSection = header.FindSection("LGHT");
            mapFile.Position = startPosition + mapSection.offset;

            Lights = MapLight.ReadFromStream(mapFile, mapSection.size / 16);

            // read materials
            mapSection = header.FindSection("MATL");
            mapFile.Position = startPosition + mapSection.offset;

            byte[] matBuffer = new byte[128];
            Materials = new string[mapSection.size / matBuffer.Length];

            for (int i = 0; i < Materials.Length; i++)
            {
                mapFile.Read(matBuffer, 0, matBuffer.Length);
                Materials[i] = Encoding.UTF8.GetString(matBuffer).TrimEnd('\0');
            }
        }

        public override void Save(Stream mapFile)
        {
            var sections = new Dictionary<string, Action<Stream>>();
            sections["CMAP"] = stream => Map.Write(stream);
            sections["LGHT"] = stream => Lights.ToList().ForEach(light => light.Write(stream));
            sections["MATL"] = stream =>
            {
                foreach (string material in Materials)
                {
                    byte[] writeBuffer = new byte[128];
                    Encoding.UTF8.GetBytes(material, 0, material.Length, writeBuffer, 0);

                    stream.Write(writeBuffer, 0, writeBuffer.Length);
                }
            };

            WriteSectionedFile(mapFile, sections);
        }

        private static void WriteSectionedFile(Stream outStream, IDictionary<string, Action<Stream>> sections)
        {
            Header header = new Header();
            header.magic = 0x504D3247; // G2MP
            header.version = 1;
            header.sections = new SectionHeader[sections.Count];

            // get the header position in the stream as we'll have to write it twice
            long position = outStream.Position;
            header.Write(outStream);

            // write the section data
            int i = 0;

            foreach (var kvp in sections)
            {
                long startPosition = outStream.Position;
                long secPosition = outStream.Position - position;
                kvp.Value(outStream);

                long length = outStream.Position - startPosition;

                header.sections[i].version = 1;
                header.sections[i].magic = Header.KeyNameToMagic(kvp.Key);
                header.sections[i].size = (int)length;
                header.sections[i].offset = (int)secPosition;
                i++;
            }

            // rewrite the header
            long endPosition = outStream.Position;
            outStream.Position = position;

            header.Write(outStream);

            outStream.Position = endPosition;
        }
    }
}
