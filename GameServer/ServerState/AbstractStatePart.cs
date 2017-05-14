using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState.Properties;

namespace Hull.GameServer.ServerState {
    [Serializable]
    public abstract class AbstractStatePart : AbstractComplexStateProperty, IStatePart {
        protected AbstractStatePart() { }
        protected AbstractStatePart(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected virtual int Priority {
            get { return 0; }
        }
    }
}