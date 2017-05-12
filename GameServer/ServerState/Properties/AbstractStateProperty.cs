using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    public abstract class AbstractStateProperty : IStateProperty {
        public virtual State State {
            protected get { return _state; }
            set {
                _state = value;
                _updateId = value.UpdateId;
            }
        }

        public IStatePropertyContainer Container { protected get; set; }

        private ulong _updateId;
        private State _state;

        public bool IsModified {
            get { return _updateId == State.UpdateId; }
        }

        public AbstractStateProperty() { }

        protected void Modify() {
            if (State != null) {
                if (State.IsReadonly) {
                    throw new AccessViolationException();
                }

                _updateId = State.UpdateId;
                if (Container != null) {
                    Container.Modify();
                }
                State.Modify();
            }
        }

        public AbstractStateProperty(SerializationInfo info, StreamingContext context) {
            _updateId = (ulong)info.GetValue("_updateId", typeof(ulong));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("_updateId", _updateId, typeof(ulong));
        }
    }
}
