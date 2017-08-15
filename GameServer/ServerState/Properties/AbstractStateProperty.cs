using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;
using Hull.GameServer.ServerState.StateChangeInfos;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public abstract class AbstractStateProperty : IStateProperty {
        private static uint _lastUniqueId;
        private static long _lastTime;

        private State _state;
        private IStatePropertyContainer _container;
        protected ulong UpdateId;
        private ModificationType _modificationType;
        private readonly ulong _uniqueId;

        protected AbstractStateProperty() {
            _uniqueId = NextUniqueId;
        }

        protected AbstractStateProperty(SerializationInfo info, StreamingContext context) {
            UpdateId = (ulong)info.GetValue("_updateId", typeof(ulong));
            _uniqueId = (ulong)info.GetValue("_uniqueId", typeof(ulong));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("_updateId", UpdateId, typeof(ulong));
            info.AddValue("_uniqueId", _uniqueId, typeof(ulong));
        }

        protected static ulong NextUniqueId {
            get {
                var time = DateTime.Now.ToUniversalTime().Ticks;
                if (_lastTime != time) {
                    _lastUniqueId = 0;
                    _lastTime = time;
                }
                return (ulong)(_lastTime << 32) | ((_lastUniqueId++) & 0xFFFFFFFF);
            }
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

        /// <summary>
        /// Parent container of the property. Do not set it manually.
        /// </summary>
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

        public virtual IStatePropertyContainer DeserializedContainer {
            set { _container = value; }
        }

        /// <summary>
        /// Returns true if property was changed since last tick
        /// </summary>
        public virtual bool IsModified {
            get { return (CurrentState != null) && UpdateId == CurrentState.UpdateId; }
        }

        /// <summary>
        /// Holds last modification type
        /// </summary>
        public ModificationType ModificationType {
            get { return _modificationType; }
        }

        /// <summary>
        /// Marks property and all its parents as modified
        /// </summary>
        /// <param name="modificationType">Type of the applied modification</param>
        /// <exception cref="InvalidOperationException">Trying to modify state in readonly phase</exception>
        public void Modify(ModificationType modificationType) {
            if (CurrentState != null) {
                if (CurrentState.IsReadonly) {
                    throw new InvalidOperationException("State is readonly");
                }

                if (!IsModified || (modificationType != ModificationType.Changed)) {
                    UpdateId = CurrentState.UpdateId;
                    _modificationType = modificationType;
                }

                if (IsReplicated && (modificationType == ModificationType.Added)) {
                    CurrentState.AddChangeInfo(new ReplicatedStatePropertyAdded {PropertyId = UniqueId});
                }

                if (IsReplicated && (modificationType == ModificationType.Removed)) {
                    CurrentState.AddChangeInfo(new ReplicatedStatePropertyRemoved {PropertyId = UniqueId});
                }

                if ((Container != null) && (Container != this)) {
                    Container.Modify(ModificationType.Changed);
                }
            }
        }

        public ulong UniqueId {
            get { return _uniqueId; }
        }

        public virtual bool IsReplicated {
            get { return false; }
        }
    }
}
