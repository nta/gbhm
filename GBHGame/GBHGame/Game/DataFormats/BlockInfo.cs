using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GBH
{
    public struct SlopeType
    {
        private byte _value;

        public SlopeType(byte value) { _value = value; }

        public byte Ground
        {
            get
            {
                return (byte)(_value & 0x3);
            }
        }

        public byte Slope
        {
            get
            {
                return (byte)((_value >> 2) & 0x3F);
            }
        }

        public byte Value
        {
            get
            {
                return _value;
            }
        }
    }

    public class SideTile
    {
        private ushort _value;

        public SideTile(ushort value) { _value = value; }

        public ushort Sprite
        {
            get
            {
                return (ushort)(_value & 0x3FF);
            }
            set
            {
                int temp = _value & ~0x3FF;
                temp |= value & 0x3FF;
                _value = (ushort)temp;
            }
        }

        public bool Wall
        {
            get
            {
                return ((_value >> 10) & 0x1) == 1;
            }
        }

        public bool BulletWall
        {
            get
            {
                return ((_value >> 11) & 0x1) == 1;
            }
        }

        public bool Flat
        {
            get
            {
                return ((_value >> 12) & 0x1) == 1;
            }
        }

        public bool Flip
        {
            get
            {
                return ((_value >> 13) & 0x1) == 1;
            }
        }

        public int Rotation
        {
            get
            {
                return (_value >> 14) & 0x3;
            }
        }

        public ushort Value
        {
            get
            {
                return _value;
            }
        }
    }

    public class BlockInfo
    {
        public SideTile Left { get; set; }
        public SideTile Right { get; set; }
        public SideTile Top { get; set; }
        public SideTile Bottom { get; set; }
        public SideTile Lid { get; set; }

        public byte Arrows { get; set; }

        public SlopeType SlopeType { get; set; }

        public void Read(BinaryReader reader)
        {
            Left = new SideTile(reader.ReadUInt16());
            Right = new SideTile(reader.ReadUInt16());
            Top = new SideTile(reader.ReadUInt16());
            Bottom = new SideTile(reader.ReadUInt16());
            Lid = new SideTile(reader.ReadUInt16());

            Arrows = reader.ReadByte();

            SlopeType = new SlopeType(reader.ReadByte());
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Left.Value);
            writer.Write(Right.Value);
            writer.Write(Top.Value);
            writer.Write(Bottom.Value);
            writer.Write(Lid.Value);

            writer.Write(Arrows);

            writer.Write(SlopeType.Value);
        }
    }
}
