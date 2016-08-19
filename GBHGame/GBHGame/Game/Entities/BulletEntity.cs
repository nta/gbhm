using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace GBH
{
    class BulletEntity : Entity
    {        
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
        public PlayerEntity Shooter { get; set; }

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
            // test if we hit a wall
            var thisMove = new Vector3(Direction * Speed * Server.DeltaTime, 0.0f);

            RigidBody outBody;
            JVector outNormal;
            float fraction;

            Server.PhysicsWorld.CollisionSystem.Raycast(Position.ToJVector(), thisMove.ToJVector(), null, out outBody, out outNormal, out fraction);

            if (fraction <= 1.0f)
            {
                // yes we hit a wall
                var hitPosition = Position + (thisMove * fraction);

                //Server.SendReliableCommand(null, "print \"{0} shot a wall.\"", Shooter.Client.Name);

                // TODO: play effect on wall on client somehow (this is server so we need client events?)
                var testEvent = new TestGameEvent() { TestString = "pew" };
                EventEntity.Create(Position, testEvent);

                // destroy ourselves
                Destroy();
            }

            for (int i = 0; i < 32; i++)
            {
                var entity = Server.Entities[i] as PlayerEntity;

                if (entity != null)
                {
                    if ((entity.Position - Position).Length() < 0.3f)
                    {
                        if (entity != Shooter)
                        {
                            Server.SendReliableCommand(null, "print \"{0} shot an {1}\"", Shooter.Client.Name, entity.Client.Name);

                            entity.Spawn();
                            Destroy();
                        }
                    }
                }
            }

            Position += thisMove;
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
