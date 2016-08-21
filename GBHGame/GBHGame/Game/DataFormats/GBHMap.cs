using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
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

        public override string ResolveSprite(int spriteIdx)
        {
            return $"gbh/bil/{spriteIdx}";
        }
    }
}
