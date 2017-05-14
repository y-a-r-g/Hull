using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public class SimpleArrayStateProperty<TValue>
        : AbstractStatePropertyContainer, IEnumerable<TValue>, IIndexedAccess<TValue, int>
        where TValue : struct {
        private TValue[] _value;

        public SimpleArrayStateProperty(TValue[] value = null, bool doNotCopyReference = false) {
            Set(value, doNotCopyReference);
        }

        protected SimpleArrayStateProperty(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            _value = (TValue[])info.GetValue("_value", typeof(TValue[]));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_value", _value, typeof(TValue[]));
        }

        public void Set(TValue[] value, bool doNotCopyReference = false) {
            Modify();
            if (value == null) {
                value = new TValue[0];
            }
            if (doNotCopyReference) {
                _value = value;
            }
            else {
                _value = new TValue[value.Length];
                if (value.Length > 0) {
                    Array.Copy(value, _value, value.Length);
                }
            }
        }

        public TValue this[int index] {
            get { return _value[index]; }
            set {
                Modify();
                _value[index] = value;
            }
        }

        public int Count {
            get { return _value.Length; }
        }

        public void Resize(int size) {
            Modify();
            if (size != _value.Length) {
                var newArray = new TValue[size];
                Array.Copy(_value, newArray, size < _value.Length ? size : _value.Length);
                _value = newArray;
            }
        }

        public IEnumerator<TValue> GetEnumerator() {
            return ((IEnumerable<TValue>)_value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public bool TryGetValue(int index, out TValue value) {
            value = this[index];
            return true;
        }
    }
}