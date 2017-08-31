using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    public abstract class AbstractArrayStateProperty<TItem> : AbstractStatePropertyContainer, IEnumerable<TItem> {
        protected TItem[] Value;

        /// <summary>
        /// Creates an empty array property
        /// </summary>
        public AbstractArrayStateProperty() : this(null) { }

        /// <summary>
        /// Create and array property with given items.
        /// </summary>
        /// <param name="value">Initial items</param>
        /// <param name="doNotCopyReference">New array will be created and items will be coped to it until this parameter is <value>true</value></param>
        public AbstractArrayStateProperty(TItem[] value, bool doNotCopyReference = false) {
            Set(value, doNotCopyReference);
        }

        protected AbstractArrayStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) {
            Value = (TItem[])info.GetValue("_value", typeof(TItem[]));
            BindItems(Value);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_value", Value, typeof(TItem[]));
        }

        protected virtual void BindItems(TItem[] items) { }

        /// <summary>
        /// Replaces all items with given ones.
        /// </summary>
        /// <param name="value">New items</param>
        /// <param name="doNotCopyReference">New array will be created and items will be coped to it until this parameter is <value>true</value></param>
        public virtual void Set(TItem[] value, bool doNotCopyReference = false) {
            Modify(ModificationType.Changed);
            if (value == null) {
                value = new TItem[0];
            }
            if (doNotCopyReference) {
                Value = value;
            }
            else {
                Value = new TItem[value.Length];
                if (value.Length > 0) {
                    Array.Copy(value, Value, value.Length);
                }
            }
            BindItems(Value);
        }

        /// <summary>
        /// Indexed access to the items
        /// </summary>
        /// <param name="index"></param>
        public virtual TItem this[int index] {
            get { return Value[index]; }
            set {
                Modify(ModificationType.Changed);
                Value[index] = value;
            }
        }

        /// <summary>
        /// Returns amount of items that stored in array
        /// </summary>
        public int Count {
            get { return Value.Length; }
        }

        public void Resize(int size) {
            Modify(ModificationType.Changed);
            if (size != Value.Length) {
                var newArray = new TItem[size];
                Array.Copy(Value, newArray, size < Value.Length ? size : Value.Length);
                Value = newArray;
            }
        }

        public IEnumerator<TItem> GetEnumerator() {
            return ((IEnumerable<TItem>)Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return new EmptyEnumerator<IStateProperty>();
        }

        protected override void ModifyChildren(ModificationType modificationType) { }
        protected override void SetDeserializedContainerToChildren() { }
    }
}