using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public abstract class AbstractStateProperty : IStateProperty {
        private State _state;
        private IStatePropertyContainer _container;
        protected ulong UpdateId;
        private ModificationType _modificationType;

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
                    while ((prop.Container != null) && (prop.Container != prop)) {
                        prop = prop.Container;
                    }

                    if ((prop != this) && (prop is State)) {
                        _state = (State)prop;
                    }
                }
                return _state;
            }
        }

        public virtual IStatePropertyContainer Container {
            get { return _container; }
            set {
                if (_container != value) {
                    _container = value;
                    if (_container != null) {
                        Modify(ModificationType.Added);
                    }
                    else {
                        Modify(ModificationType.Removed);
                    }
                }
            }
        }

        public virtual bool IsModified {
            get { return (CurrentState != null) && UpdateId == CurrentState.UpdateId; }
        }

        public ModificationType ModificationType {
            get { return _modificationType; }
        }

        public void Modify(ModificationType modificationType) {
            if (CurrentState != null) {
                if (CurrentState.IsReadonly) {
                    throw new AccessViolationException();
                }

                UpdateId = CurrentState.UpdateId;
                _modificationType = modificationType;

                if ((Container != null) && (Container != this)) {
                    Container.Modify(ModificationType.Changed);
                }
            }
        }
    }
}
