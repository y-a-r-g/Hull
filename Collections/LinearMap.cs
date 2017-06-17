using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hull.Collections {
    [Serializable]
    public class LinearMap<T> : ILinearMap<T>, ISerializable {
        protected readonly List<T> Items = new List<T>();
        protected readonly LinkedList<int> Free = new LinkedList<int>();

        internal struct Enumerator : IEnumerator<KeyValuePair<LinearMapId, T>> {
            internal int _index;
            private readonly LinearMap<T> _set;

            public Enumerator(LinearMap<T> set) {
                _index = -1;
                _set = set;
            }

            public void Dispose() { }

            public bool MoveNext() {
                while (++_index < _set.Items.Count) {
                    if (!_set.Free.Contains(_index)) {
                        return true;
                    }
                }
                return false;
            }

            public void Reset() {
                _index = -1;
            }

            public KeyValuePair<LinearMapId, T> Current {
                get { return new KeyValuePair<LinearMapId, T>(LinearMapId.NextFor(this), _set.Items[_index]); }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }

        public LinearMap() { }

        protected LinearMap(SerializationInfo info, StreamingContext context) {
            Items = (List<T>)info.GetValue("items", typeof(List<T>));
            Free = (LinkedList<int>)info.GetValue("free", typeof(LinkedList<int>));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("items", Items, typeof(List<T>));
            info.AddValue("free", Free, typeof(LinkedList<int>));
        }

        public virtual IEnumerator<KeyValuePair<LinearMapId, T>> GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual LinearMapId FreeId {
            get { return LinearMapId.NextFreeFor(Items, Free); }
        }

        public virtual void Remove(LinearMapId id) {
            id.RemoveFrom(Items, Free);
        }

        public virtual int Count {
            get { return Items.Count - Free.Count; }
        }

        public virtual int Capacity {
            get { return Items.Count; }
        }

        public virtual void Clear() {
            foreach (var kvp in this) {
                Remove(kvp.Key);
            }
        }

        public virtual bool Contains(LinearMapId id) {
            return id.ExistsIn(Items, Free);
        }

        public virtual bool TryGetValue(LinearMapId id, out T value) {
            if (Contains(id)) {
                value = id.GetFrom(Items, Free);
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
                return id.GetFrom(Items, Free);
            }
            set { id.SetTo(Items, Free, value); }
        }
    }
}
