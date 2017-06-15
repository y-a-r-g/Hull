using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// This property holds an array of nested properties. Nested properties can be observed as well.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ArrayStateProperty<TValue> : AbstractStatePropertyContainer, IEnumerable<TValue>
        where TValue : IStateProperty {
        private TValue[] _value;
        
        private struct ItemsEnumerator : IEnumerator<IStateProperty> {
            private readonly IEnumerator<TValue> _enumerator;

            public ItemsEnumerator(IEnumerator<TValue> enumerator) {
                _enumerator = enumerator;
            }

            public void Dispose() {
                _enumerator.Dispose();
            }

            public bool MoveNext() {
                return _enumerator.MoveNext();
            }

            public void Reset() {
                _enumerator.Reset();
            }

            public IStateProperty Current {
                get { return _enumerator.Current; }
            }

            object IEnumerator.Current {
                get { return Current; }
            }
        }

        /// <summary>
        /// Creates an empty array property
        /// </summary>
        public ArrayStateProperty() : this(null) { }

        /// <summary>
        /// Create and array property with given items.
        /// </summary>
        /// <param name="value">Initial items</param>
        /// <param name="doNotCopyReference">New array will be created and items will be coped to it until this parameter is <value>true</value></param>
        public ArrayStateProperty(TValue[] value, bool doNotCopyReference = false) {
            Set(value, doNotCopyReference);
        }

        protected ArrayStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) {
            _value = (TValue[])info.GetValue("_value", typeof(TValue[]));
            BindItems(_value);
        }

        protected void BindItems<TItem>(TItem[] items) where TItem : IStateProperty {
            for (var i = 0; i < items.Length; i++) {
                var item = items[i];
                if (item != null) {
                    item.Container = this;
                    items[i] = item;
                }
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_value", _value, typeof(TValue[]));
        }

        /// <summary>
        /// Replaces all items with given ones.
        /// </summary>
        /// <param name="value">New items</param>
        /// <param name="doNotCopyReference">New array will be created and items will be coped to it until this parameter is <value>true</value></param>
        public void Set(TValue[] value, bool doNotCopyReference = false) {
            if (_value != null) {
                for (var i = 0; i < _value.Length; i++) {
                    var item = _value[i];
                    if (item != null) {
                        item.Container = null;
                    }
                }
            }

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
            BindItems(_value);
        }

        /// <summary>
        /// Indexed access to the items
        /// </summary>
        /// <param name="index"></param>
        public TValue this[int index] {
            get { return _value[index]; }
            set {
                Modify(ModificationType.Changed);
                if (_value[index] != null) {
                    _value[index].Container = null;
                }
                value.Container = this;
                _value[index] = value;
            }
        }

        /// <summary>
        /// Returns amount of items that stored in array
        /// </summary>
        public int Count {
            get { return _value.Length; }
        }

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
            return new ItemsEnumerator(((IEnumerable<TValue>)_value).GetEnumerator());
        }

        protected override void ModifyChildren(ModificationType modificationType) {
            if (_value != null) {
                for (var i = 0; i < _value.Length; i++) {
                    var item = _value[i];
                    ModifyChild(item, modificationType);
                    _value[i] = item;
                }
            }
        }
    }
}
