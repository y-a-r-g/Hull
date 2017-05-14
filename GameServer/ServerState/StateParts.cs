using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState {
    [Serializable]
    internal class StateParts : Dictionary<Type, IStatePart> {
        public StateParts() { }
        protected StateParts(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override void OnDeserialization(object sender) {
            base.OnDeserialization(sender);
            if (CurrentState != null) {
                foreach (var kvp in this) {
                    kvp.Value.Container = CurrentState;
                }
            }
        }

        private State _state;

        public State CurrentState {
            get { return _state; }
            set {
                _state = value;
                foreach (var kvp in this) {
                    kvp.Value.Container = value;
                }
            }
        }
    }
}