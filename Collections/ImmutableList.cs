using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hull.Collections {
    [Serializable]
    public struct ImmutableList<TValue> : IImmutableList<TValue>, ISerializable where TValue : struct {
        private TValue[] _set;

        public ImmutableList(params TValue[] items) {
            _set = new TValue[items.Length];
            Array.Copy(items, _set, items.Length);
        }

        public ImmutableList(TValue[] items, bool doNotCopy = false) {
            if (doNotCopy) {
                _set = items;
            }
            else {
                _set = new TValue[items.Length];
                Array.Copy(items, _set, items.Length);
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            if (_set == null) {
                return new EmptyEnumerator<TValue>();
            }
            return ((IEnumerable<TValue>)_set).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public int Length {
            get { return _set == null ? 0 : _set.Length; }
        }

        public ImmutableList(SerializationInfo info, StreamingContext context) {
            _set = (TValue[])info.GetValue("set", typeof(TValue[]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("set", _set, typeof(TValue[]));
        }
    }
}
