using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// Simple array state property. Can hold array of any type. Though it cannot monitor internal item value changes. It will be marked as modified only if item or whole array is replaced.
    /// </summary>
    /// <typeparam name="TValue">Type of value array</typeparam>
    [Serializable]
    public class SimpleArrayStateProperty<TValue> : AbstractStatePropertyContainer,
                                                    IEnumerable<TValue>
        where TValue : struct {
        private TValue[] _value;

        /// <summary>
        /// Creates property with empty array
        /// </summary>
        public SimpleArrayStateProperty() : this(null) { }

        /// <summary>
        /// Creates property from given array. Property will contain copy of given array.
        /// </summary>
        /// <param name="value">New array. Null is treated as empty array</param>
        /// <param name="doNotCopyReference">Prevent making of copy. Given array will be referenced</param>
        public SimpleArrayStateProperty(TValue[] value, bool doNotCopyReference = false) {
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

        /// <summary>
        /// Sets new array value. Property will contain copy of given array.
        /// </summary>
        /// <param name="value">New array. Null is treated as empty array</param>
        /// <param name="doNotCopyReference">Prevent making of copy. Given array will be referenced</param>
        public void Set(TValue[] value, bool doNotCopyReference = false) {
            Modify(ModificationType.Changed);
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

        /// <summary>
        /// Acces to the item by index.
        /// </summary>
        /// <param name="index"></param>
        public TValue this[int index] {
            get { return _value[index]; }
            set {
                Modify(ModificationType.Changed);
                _value[index] = value;
            }
        }

        /// <summary>
        /// Amount of items in array
        /// </summary>
        public int Count {
            get { return _value.Length; }
        }

        /// <summary>
        /// Changes array size. It will keep existing items.
        /// </summary>
        /// <param name="size">New size of the array</param>
        public void Resize(int size) {
            Modify(ModificationType.Changed);
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

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return new EmptyEnumerator<IStateProperty>();
        }

        protected override void ModifyChildren(ModificationType modificationType) { }
    }
}
