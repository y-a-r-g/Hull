using System;
using System.Runtime.Serialization;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    internal class PlaceholderStateProperty : AbstractStateProperty {
        public PlaceholderStateProperty() { }
        public PlaceholderStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
