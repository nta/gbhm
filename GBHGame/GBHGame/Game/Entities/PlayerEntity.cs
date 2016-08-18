using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace GBH
{
    public class PlayerEntity : Entity
    {
        public UserCommand Command { get; set; }
        public uint LastCommandTime { get; set; }
        public float VelocityZ { get; set; } // for gravity
        public ServerClient Client { get; set; }

        private bool ShootPressed { get; set; }

        public override int TypeCode
        {
            get
            {
                return 0;
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
            LastCommandTime = message.ReadUInt32();
            VelocityZ = message.ReadSingle();

            base.Deserialize(message);
        }

        public override void Serialize(DeltaBitStream message)
        {
            // FIXME: maybe this should not be replicated for every entity?
            message.WriteUInt32(LastCommandTime);
            message.WriteSingle(VelocityZ);

            base.Serialize(message);
        }

        public override void Spawn()
        {
            Position = new Vector3(72.0f, -190.0f, 2.01f);
            Rotation = new Vector3(0f, 0f, 0f);
            VelocityZ = 0f;
        }

        public override void Think()
        {
            var command = Command;

            if (command.ServerTime < (Server.Time - 1000))
            {
                command.ServerTime = Server.Time - 1000;
            }
            else if (command.ServerTime > (Server.Time + 200))
            {
                command.ServerTime = Server.Time + 200;
            }

            int msec = (int)command.ServerTime - (int)LastCommandTime;

            if (msec < 1)
            {
                return;
            }
            else if (msec > 200)
            {
                msec = 200;
            }

            // TODO: don't just use the server's physics world
            PlayerMovement.Process(this, command, Server.PhysicsWorld);

            if (command.TestButton(ClientButtons.Shoot))
            {
                if (!ShootPressed)
                {
                    var bullet = Server.SpawnEntity<BulletEntity>();
                    bullet.Position = Position;
                    bullet.Rotation = Rotation;
                    bullet.Speed = 0.5f;
                    bullet.Shooter = this;
                    bullet.Direction = new Vector2((float)Math.Cos(MathHelper.ToRadians(Rotation.Z + 90.0f)), (float)Math.Sin(MathHelper.ToRadians(Rotation.Z + 90.0f)));
                }

                ShootPressed = true;
            }
            else
            {
                ShootPressed = false;
            }
        }

        public override void ClientProcess()
        {
            
        }

        public override RenderEntity GetRenderEntity()
        {
            return new RenderEntity() { Position = Position, Heading = Rotation.Z, Size = new Vector2(0.5f, 0.5f), Sprite = MaterialManager.FindMaterial("x1") };
        }

        public override Entity Clone()
        {
            return new PlayerEntity()
            {
                Position = Position,
                Rotation = Rotation,
                SpawnKey = SpawnKey,
                LastCommandTime = LastCommandTime,
                ShootPressed = ShootPressed
            };
        }
    }
}
