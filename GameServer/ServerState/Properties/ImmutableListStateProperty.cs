using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Collections;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// This property holds an immutable collection. It can be only change all at once. 
    /// Items can me any value type. Item changes are not observable (items cannot be changed). 
    /// <seealso cref="ImmutableList{TValue}"/> 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ImmutableListStateProperty<TValue> : AbstractStatePropertyContainer, IImmutableList<TValue>
        where TValue : struct {
        private ImmutableList<TValue> _list;

        /// <summary>
        /// Creates a property without items.
        /// </summary>
        public ImmutableListStateProperty() : this(new TValue[0]) { }

        /// <summary>
        /// Creates the property with given items.
        /// </summary>
        /// <param name="items">Initial items</param>
        public ImmutableListStateProperty(params TValue[] items) {
            _list = new ImmutableList<TValue>(items, true);
        }

        /// <summary>
        /// Creates the property with given items.
        /// </summary>
        /// <param name="items">Initial items</param>
        /// <param name="doNotCopy">Items will be copied unless the value is <value>true</value></param>
        public ImmutableListStateProperty(TValue[] items, bool doNotCopy = false) {
            _list = new ImmutableList<TValue>(items, doNotCopy);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_list", _list, typeof(ImmutableList<TValue>));
        }

        protected ImmutableListStateProperty(SerializationInfo info, StreamingContext context) {
            _list = (ImmutableList<TValue>)info.GetValue("_list", typeof(ImmutableList<TValue>));
        }

        public IEnumerator<TValue> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// Amount of items holding
        /// </summary>
        public int Length {
            get { return _list.Length; }
        }

        /// <summary>
        /// Replaces current items with given ones.
        /// </summary>
        /// <param name="value"></param>
        public void Set(ImmutableList<TValue> value) {
            Modify(ModificationType.Changed);
            _list = value;
        }

        public override IEnumerator<IStateProperty> GetChildrenEnumerator() {
            return new EmptyEnumerator<IStateProperty>();
        }

        protected override void ModifyChildren(ModificationType modificationType) { }
        protected override void SetDeserializedContainerToChildren() { }
    }
}