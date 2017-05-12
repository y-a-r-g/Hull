using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public class LinearMapStateProperty<TValue>
        : AbstractStatePropertyContainer, ILinearMap<TValue>, IIndexedAccess<TValue, LinearMapId>
        where TValue : IStateProperty {
        private LinearMap<TValue> _map;

        public LinearMapStateProperty() {
            _map = new LinearMap<TValue>();
        }

        public LinearMapStateProperty(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            _map = (LinearMap<TValue>)info.GetValue("_map", typeof(LinearMap<TValue>));
            BindItems();
        }

        private void BindItems() {
            for (var i = 0; i < _map.Capacity; i++) {
                var id = new LinearMapId(i);
                if (_map.Contains(id)) {
                    var item = _map[id];
                    if (item != null) {
                        item.State = State;
                        item.Container = this;
                        _map[id] = item;
                    }
                }
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_map", _map, typeof(LinearMap<TValue>));
        }

        public IEnumerator<KeyValuePair<LinearMapId, TValue>> GetEnumerator() {
            return _map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public LinearMapId FreeId {
            get { return _map.FreeId; }
        }

        public int Capacity {
            get { return _map.Capacity; }
        }

        public LinearMapId Add(TValue item) {
            Modify();
            item.State = State;
            item.Container = this;
            return _map.Add(item);
        }

        public void Remove(LinearMapId id) {
            Modify();
            _map.Remove(id);
        }

        public int Count {
            get { return _map.Count; }
        }

        public bool Contains(LinearMapId id) {
            return _map.Contains(id);
        }

        public bool TryGetValue(LinearMapId id, out TValue value) {
            return _map.TryGetValue(id, out value);
        }

        public TValue this[LinearMapId id] {
            get { return _map[id]; }
            set {
                Modify();
                value.State = State;
                value.Container = this;
                _map[id] = value;
            }
        }
        
        public override State State {
            protected get { return base.State; }
            set {
                base.State = value;
                BindItems();
            }
        }
    }
}
