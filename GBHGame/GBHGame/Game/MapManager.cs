using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace GBH
{
    public abstract class MapFileBase
    {
        public CompressedMap Map { get; protected set; }
        public MapLight[] Lights { get; protected set; }
        public string Name { get; protected set; }

        public MapFileBase()
        {
            Name = "unnamed_map";
        }

        public virtual void Load(string filename)
        {
            Name = Path.GetFileNameWithoutExtension(filename).ToLowerInvariant();

            using (Stream mapFile = FileSystem.OpenCopy(filename))
            {
                Load(mapFile);
            }
        }

        public abstract void Load(Stream mapFile);

        public virtual void Save(string filename)
        {
            using (Stream mapFile = File.OpenWrite(filename))
            {
                Save(mapFile);
            }
        }

        public virtual void Save(Stream mapFile)
        {
            throw new NotImplementedException("This map type does not implement saving.");
        }

        public BlockInfo GetBlock(int x, int y, int z)
        {
            if (y > 255 || y < 0 || x > 255 || x < 0)
            {
                return Map.Block[0];
            }

            uint column = Map.Base[(256 * y) + x];
            uint columnInfo = Map.Column[column];

            byte height = (byte)(columnInfo & 0xFF);
            byte offset = (byte)((columnInfo & 0xFF00) >> 8);

            int i = (z - offset);

            if (i < 0 || z >= height)
            {
                return Map.Block[0];
            }

            return Map.Block[Map.Column[column + (i + 1)]];
        }
    }

    public class GBHMap : MapFileBase
    {
        public GBHMap()
            : base()
        {

        }

        public override void Load(Stream mapFile)
        {
            BinaryReader reader = new BinaryReader(mapFile);

            GBMPHeader header = new GBMPHeader();
            header.Read(reader);

            if (header.Magic != 0x504D4247 || header.Version != 500)
            {
                throw new InvalidOperationException("this isn't a GBMP v.500");
            }

            while (!mapFile.EndOfStream())
            {
                GBMPChunk chunk = new GBMPChunk();
                chunk.Read(reader);

                if (chunk.Type == 0x50414D44) // DMAP
                {
                    Map = CompressedMap.ReadFromStream(mapFile);
                }
                else if (chunk.Type == 0x5448474C) // LGHT
                {
                    Lights = MapLight.ReadFromStream(mapFile, (chunk.Size / 16));
                }
                else
                {
                    mapFile.Position += chunk.Size;
                }
            }
        }
    }

    public static class MapManager
    {
        public static MapFileBase CurrentMap { get; set; }

        public static void Load(string filename)
        {
            if (Path.GetFileNameWithoutExtension(filename).ToLowerInvariant() == CurrentMap?.Name)
            {
                return;
            }

            ConVar.SetValue("mapname", Path.GetFileNameWithoutExtension(filename));

            CurrentMap = new GBHMap();
            CurrentMap.Load(filename);

            CellManager.InitializeFromMap();
        }
    }
}
