using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using System.IO;

namespace GBH
{
    public class MapLight
    {
        public Color Color { get; set; }
        public Vector3 Position { get; set; }
        public float Radius { get; set; }
        public byte Intensity { get; set; }
        public byte Shape { get; set; }
        public byte OnTime { get; set; }
        public byte OffTime { get; set; }

        public void Read(BinaryReader reader)
        {
            uint argb = reader.ReadUInt32();
            Color = new Color((int)((argb & 0xFF0000) >> 16), (int)((argb & 0xFF00) >> 8), (int)(argb & 0xFF));

            Position = new Vector3(ReadFix16(reader), ReadFix16(reader), ReadFix16(reader));
            Radius = ReadFix16(reader);

            Intensity = reader.ReadByte();
            Shape = reader.ReadByte();
            OnTime = reader.ReadByte();
            OffTime = reader.ReadByte();
        }

        private float ReadFix16(BinaryReader reader)
        {
            ushort value = reader.ReadUInt16();
            float retval = (float)((value & 0x7F80) >> 7);

            retval += (value & 0x7F) / 128.0f;

            if ((value & 0x8000) != 0)
            {
                retval = -retval;
            }

            return retval;
        }

        public static MapLight[] ReadFromStream(Stream stream, int numLights)
        {
            BinaryReader reader = new BinaryReader(stream);

            var lights = new MapLight[numLights];

            for (int i = 0; i < numLights; i++)
            {
                lights[i] = new MapLight();
                lights[i].Read(reader);
            }

            return lights;
        }
    }
}
