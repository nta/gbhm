using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBH
{
    public abstract class GameEvent
    {
        public Entity Entity { get; set; }

        public abstract void Deserialize(DeltaBitStream stream);
        public abstract void Serialize(DeltaBitStream stream);

        // for client usage when encountered
        public abstract void Handle();
    }
}
