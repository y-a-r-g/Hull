using System;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public class LinearMapStateProperty<TValue> : LinearMap<TValue>, IStatePropertyContainer
        where TValue : IStateProperty {
        private IStatePropertyContainer _container;
        private State _state;
        private ulong _updateId;

        public LinearMapStateProperty() { }

        protected LinearMapStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) {
            BindItems();
        }

        public IStatePropertyContainer Container {
            get { return _container; }
            set {
                _container = value;
                BindItems();
            }
        }

        public bool IsModified {
            get { return (CurrentState != null) && _updateId == CurrentState.UpdateId; }
        }

        public void ForceModify() {
            foreach (var item in _items) {
                if (item != null) {
                    item.ForceModify();
                }
            }
        }

        public void Modify() {
            if (CurrentState != null) {
                if (CurrentState.IsReadonly) {
                    throw new AccessViolationException();
                }

                _updateId = CurrentState.UpdateId;
                if (Container != null) {
                    Container.Modify();
                }
            }
        }

        private State CurrentState {
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

        private void BindItems() {
            for (var i = 0; i < _items.Count; i++) {
                var item = _items[i];
                if (item != null) {
                    item.Container = this;
                    _items[i] = item;
                }
            }
        }

        public override LinearMapId Add(TValue item) {
            Modify();
            if (item != null) {
                item.Container = this;
            }
            return base.Add(item);
        }

        public override void Clear() {
            Modify();
            base.Clear();
        }
    }
}
