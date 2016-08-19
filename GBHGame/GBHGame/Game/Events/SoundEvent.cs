using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    [EventType(2)]
    class SoundEvent : GameEvent
    {
        public string SoundString { get; set; }

        public override void Deserialize(DeltaBitStream stream)
        {
            SoundString = stream.ReadString(128);
        }

        public override void Serialize(DeltaBitStream stream)
        {
            stream.WriteString(SoundString, 128);
        }

        public override void Handle()
        {
            AudioManager.PlayOneShot(SoundString, Entity.Position);
        }
    }
}
