using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    class EventEntity : Entity
    {
        public GameEvent Event { get; set; }
        public uint EventTime { get; set; }

        // FIXME: don't break this when server changes state
        private static uint _lastClientProcessedSvTime;

        public override int TypeCode
        {
            get
            {
                return 2;
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
            EventTime = message.ReadUInt32();

            byte eventType = message.ReadByte();

            Event = EventTypeManager.CreateEvent(eventType);
            Event.Entity = this;
            Event.Deserialize(message);

            base.Deserialize(message);
        }

        public override void Serialize(DeltaBitStream message)
        {
            message.WriteUInt32(EventTime);
            message.WriteByte((byte)Event.GetType().GetCustomAttribute<EventTypeAttribute>().TypeCode);
            Event.Serialize(message);

            base.Serialize(message);
        }

        public override void Spawn()
        {

        }

        public override void Think()
        {
            if ((Server.Time - EventTime) > 500)
            {
                Destroy();
            }
        }

        public override void ClientProcess()
        {
            if (EventTime > _lastClientProcessedSvTime)
            {
                Event.Handle();

                _lastClientProcessedSvTime = EventTime;
            }
        }

        public override RenderEntity GetRenderEntity()
        {
            return null;
        }

        public override Entity Clone()
        {
            return new EventEntity()
            {
                Event = Event
            };
        }

        // for server
        public static EventEntity Create(Vector3 position, GameEvent eventInstance)
        {
            var entity = Server.SpawnEntity<EventEntity>();
            entity.Event = eventInstance;
            entity.Position = position;
            entity.EventTime = Server.Time;

            return entity;
        }
    }
}
