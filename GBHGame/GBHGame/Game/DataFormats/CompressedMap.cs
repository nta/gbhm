using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GBH
{
    public class CompressedMap
    {
        public uint[] Base { get; set; }
        public uint ColumnWords { get; set; }
        public uint[] Column { get; set; }
        public uint NumBlocks { get; set; }
        public BlockInfo[] Block { get; set; }

        public CompressedMap()
        {
            Base = new uint[256 * 256];
        }

        public void Read(BinaryReader reader)
        {
            // read the base/columnwords fields
            for (int i = 0; i < (256 * 256); i++)
            {
                Base[i] = reader.ReadUInt32();
            }

            ColumnWords = reader.ReadUInt32();

            // read columns
            Column = new uint[ColumnWords];

            for (int i = 0; i < ColumnWords; i++)
            {
                Column[i] = reader.ReadUInt32();
            }

            // read blocks
            NumBlocks = reader.ReadUInt32();
            Block = new BlockInfo[NumBlocks];

            for (int i = 0; i < NumBlocks; i++)
            {
                Block[i] = new BlockInfo();
                Block[i].Read(reader);
            }
        }

        public static CompressedMap ReadFromStream(Stream stream)
        {
            var compressedMap = new CompressedMap();
            compressedMap.Read(new BinaryReader(stream));

            return compressedMap;
        }
    }
}
