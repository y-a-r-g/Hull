using System;
using System.Collections.Generic;

namespace Hull.Collections {
    [Serializable]
    public struct LinearMapId {
        private readonly int _value;

        private LinearMapId(int value) {
            _value = value;
        }

        public static bool operator ==(LinearMapId a, LinearMapId b) {
            return a._value == b._value;
        }

        public static bool operator !=(LinearMapId a, LinearMapId b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return (obj != null) && (_value == ((LinearMapId)obj)._value);
        }

        public override int GetHashCode() {
            return _value;
        }

        internal void RemoveFrom<T>(List<T> items, LinkedList<int> free) {
            if (!ExistsIn(items, free)) {
                throw new ArgumentOutOfRangeException();
            }
            items[_value] = default(T);
            free.AddFirst(_value);
        }

        internal bool ExistsIn<T>(List<T> items, LinkedList<int> free) {
            return (_value >= 0) && (_value < items.Count) && !free.Contains(_value);
        }

        internal T GetFrom<T>(List<T> items, LinkedList<int> free) {
            return items[_value];
        }

        internal void SetTo<T>(List<T> items, LinkedList<int> free, T value) {
            while (_value >= items.Count) {
                if (_value == items.Count) {
                    items.Add(value);
                    return;
                }
                free.AddFirst(items.Count);
                items.Add(default(T));
            }
            items[_value] = value;
            free.Remove(_value);
        }

        internal static LinearMapId NextFreeFor<T>(List<T> items, LinkedList<int> free) {
            if (free.Count > 0) {
                return new LinearMapId(free.Last.Value);
            }
            return new LinearMapId(items.Count);
        }

        internal static LinearMapId NextFor<T>(LinearMap<T>.Enumerator e) {
            return new LinearMapId(e._index);
        }
    }
}
