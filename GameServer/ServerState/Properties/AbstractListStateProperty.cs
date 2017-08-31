using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    public class AbstractListStateProperty<TItem> : AbstractStatePropertyContainer, IList<TItem> {
        protected IList<TItem> Value;

        /// <summary>
        /// Creates an empty array property
        /// </summary>
        public AbstractListStateProperty() : this(null) { }

        /// <summary>
        /// Create and array property with given items.
        /// </summary>
        /// <param name="value">Initial items</param>
        /// <param name="doNotCopyReference">New array will be created and items will be coped to it until this parameter is <value>true</value></param>
        public AbstractListStateProperty(IList<TItem> value, bool doNotCopyReference = false) {
            Set(value, doNotCopyReference);
        }

        protected AbstractListStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) {
            Value = (IList<TItem>)info.GetValue("_value", typeof(IList<TItem>));
            BindItems(Value);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_value", Value, typeof(IList<TItem>));
        }

        protected virtual void BindItems(IList<TItem> items) { }

        /// <summary>
        /// Replaces all items with given ones.
        /// </summary>
        /// <param name="value">New items</param>
        /// <param name="doNotCopyReference">New array will be created and items will be coped to it until this parameter is <value>true</value></param>
        public virtual void Set(IList<TItem> value, bool doNotCopyReference = false) {
            Modify(ModificationType.Changed);
            if (value == null) {
                value = new List<TItem>();
            }
            if (doNotCopyReference) {
                Value = value;
            }
            else {
                Value = new List<TItem>(value);
            }
            BindItems(Value);
        }

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return new EmptyEnumerator<IStateProperty>();
        }

        protected override void ModifyChildren(ModificationType modificationType) { }
        protected override void SetDeserializedContainerToChildren() { }

        public IEnumerator<TItem> GetEnumerator() {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public virtual void Add(TItem item) {
            Modify(ModificationType.Changed);
            Value.Add(item);
        }

        public virtual void Clear() {
            Modify(ModificationType.Changed);
            Value.Clear();
        }

        public bool Contains(TItem item) {
            return Value.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex) {
            Value.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(TItem item) {
            Modify(ModificationType.Changed);
            return Value.Remove(item);
        }

        public int Count {
            get { return Value.Count; }
        }

        public bool IsReadOnly {
            get { return (CurrentState != null) && CurrentState.IsReadonly; }
        }

        public int IndexOf(TItem item) {
            return Value.IndexOf(item);
        }

        public virtual void Insert(int index, TItem item) {
            Modify(ModificationType.Changed);
            Value.Insert(index, item);
        }

        public virtual void RemoveAt(int index) {
            Modify(ModificationType.Changed);
            Value.RemoveAt(index);
        }

        public virtual TItem this[int index] {
            get { return Value[index]; }
            set {
                Modify(ModificationType.Changed);
                Value[index] = value;
            }
        }
    }
}