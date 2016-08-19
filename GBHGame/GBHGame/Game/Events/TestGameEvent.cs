using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    [EventType(1)]
    class TestGameEvent : GameEvent
    {
        public string TestString { get; set; }

        public override void Deserialize(DeltaBitStream stream)
        {
            TestString = stream.ReadString(128);
        }

        public override void Serialize(DeltaBitStream stream)
        {
            stream.WriteString(TestString, 128);
        }

        public override void Handle()
        {
            Log.Write(LogLevel.Debug, $"got TestGameEvent for {TestString} at {Entity.Position}");
        }
    }
}
