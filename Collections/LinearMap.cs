using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hull.Collections {
    [Serializable]
    public class LinearMap<T> : ILinearMap<T>, ISerializable {
        protected readonly List<T> _items = new List<T>();
        protected readonly LinkedList<int> _free = new LinkedList<int>();

        protected struct Enumerator : IEnumerator<KeyValuePair<LinearMapId, T>> {
            private int _index;
            private readonly LinearMap<T> _set;

            public Enumerator(LinearMap<T> set) {
                _index = -1;
                _set = set;
            }

            public void Dispose() { }

            public bool MoveNext() {
                while (++_index < _set._items.Count) {
                    if (!_set._free.Contains(_index)) {
                        return true;
                    }
                }
                return false;
            }

            public void Reset() {
                _index = -1;
            }

            public KeyValuePair<LinearMapId, T> Current {
                get { return new KeyValuePair<LinearMapId, T>(new LinearMapId(_index), _set._items[_index]); }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }

        public LinearMap() { }

        protected LinearMap(SerializationInfo info, StreamingContext context) {
            _items = (List<T>)info.GetValue("items", typeof(List<T>));
            _free = (LinkedList<int>)info.GetValue("free", typeof(LinkedList<int>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("items", _items, typeof(List<T>));
            info.AddValue("free", _free, typeof(LinkedList<int>));
        }

        public virtual IEnumerator<KeyValuePair<LinearMapId, T>> GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual LinearMapId FreeId {
            get {
                if (_free.Count > 0) {
                    return new LinearMapId(_free.Last.Value);
                }
                return new LinearMapId(_items.Count);
            }
        }

        public virtual LinearMapId Add(T item) {
            var id = FreeId;
            this[id] = item;
            return id;
        }

        public virtual void Remove(LinearMapId id) {
            if ((id.Value < 0) || (id.Value > _items.Count) || _free.Contains(id.Value)) {
                throw new ArgumentOutOfRangeException();
            }
            _items[id.Value] = default(T);
            _free.AddFirst(id.Value);
        }

        public virtual int Count {
            get { return _items.Count - _free.Count; }
        }

        public virtual int Capacity {
            get { return _items.Count; }
        }

        public virtual void Clear() {
            foreach (var kvp in this) {
                Remove(kvp.Key);
            }
        }

        public virtual bool Contains(LinearMapId id) {
            return (id.Value >= 0) && (id.Value < _items.Count) && !_free.Contains(id.Value);
        }

        public virtual bool TryGetValue(LinearMapId id, out T value) {
            if (Contains(id)) {
                value = _items[id.Value];
                return true;
            }
            value = default(T);
            return false;
        }

        public virtual T this[LinearMapId id] {
            get {
                if (!Contains(id)) {
                    throw new ArgumentOutOfRangeException();
                }
                return _items[id.Value];
            }
            set {
                while (id.Value >= _items.Count) {
                    if (id.Value == _items.Count) {
                        _items.Add(value);
                        return;
                    }
                    _free.AddFirst(_items.Count);
                    _items.Add(default(T));
                }
                _items[id.Value] = value;
                _free.Remove(id.Value);
            }
        }
    }
}
