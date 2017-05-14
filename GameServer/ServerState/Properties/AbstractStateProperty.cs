using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public abstract class AbstractStateProperty : IStateProperty {
        private State _state;
        protected ulong UpdateId;

        protected AbstractStateProperty() { }

        protected AbstractStateProperty(SerializationInfo info, StreamingContext context) {
            UpdateId = (ulong)info.GetValue("_updateId", typeof(ulong));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("_updateId", UpdateId, typeof(ulong));
        }

        protected virtual State CurrentState {
            get {
                if (_state == null) {
                    IStateProperty prop = this;
                    while (prop.Container != null) {
                        prop = prop.Container;
                    }
                    if (prop != this) {
                        _state = (State)prop;
                    }
                }
                return _state;
            }
        }

        public virtual IStatePropertyContainer Container { get; set; }

        public virtual bool IsModified {
            get { return (CurrentState != null) && UpdateId == CurrentState.UpdateId; }
        }

        public virtual void ForceModify() {
            Modify();
        }

        protected void Modify() {
            if (CurrentState != null) {
                if (CurrentState.IsReadonly) {
                    throw new AccessViolationException();
                }

                UpdateId = CurrentState.UpdateId;
                if (Container != null) {
                    Container.Modify();
                }
            }
        }
    }
}