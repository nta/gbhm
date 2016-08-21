using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    public abstract class MapFileBase
    {
        public CompressedMap Map { get; set; }
        public MapLight[] Lights { get; set; }
        //public EntitySpawnData EntityData { get; protected set; }
        public string[] Materials { get; set; }
        public string Name { get; set; }

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

        public virtual string ResolveSprite(int spriteIdx)
        {
            return Materials[spriteIdx];
        }
    }
}
