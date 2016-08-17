using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GBH
{
    class BulletEntity : Entity
    {        
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }

        public override int TypeCode
        {
            get
            {
                return 1;
            }
        }

        public override void Initialize()
        {

        }

        public override void InitializeFromProperties(Dictionary<string, string> properties)
        {
            // no-op; players aren't initialized from properties
        }

        public override void Deserialize(DeltaBitStream message)
        {
            Speed = message.ReadSingle();
            var x = message.ReadSingle();
            var y = message.ReadSingle();
            Direction = new Vector2(x, y);

            base.Deserialize(message);
        }

        public override void Serialize(DeltaBitStream message)
        {
            // FIXME: maybe this should not be replicated for every entity?
            message.WriteSingle(Speed);
            message.WriteSingle(Direction.X);
            message.WriteSingle(Direction.Y);

            base.Serialize(message);
        }

        public override void Spawn()
        {
        }

        public override void Think()
        {
            Position += new Vector3(Direction * Speed * Server.DeltaTime, 0.0f);
        }

        public override void ClientProcess()
        {

        }

        public override RenderEntity GetRenderEntity()
        {
            return new RenderEntity() { Position = Position, Heading = Rotation.Z, Size = new Vector2(0.1f, 0.1f), Sprite = MaterialManager.FindMaterial("default") };
        }

        public override Entity Clone()
        {
            return new BulletEntity()
            {
                Position = Position,
                Rotation = Rotation,
                SpawnKey = SpawnKey,
                Direction = Direction,
                Speed = Speed
            };
        }
    }
}
