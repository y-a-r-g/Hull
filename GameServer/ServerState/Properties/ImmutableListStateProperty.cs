using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hull.Collections;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public class ImmutableListStateProperty<TValue> : AbstractStatePropertyContainer, IImmutableList<TValue>
        where TValue : struct {
        private ImmutableList<TValue> _list;

        public ImmutableListStateProperty() : this(new TValue[0]) { }

        public ImmutableListStateProperty(params TValue[] items) {
            _list = new ImmutableList<TValue>(items, true);
        }

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

        public int Length {
            get { return _list.Length; }
        }

        public void Set(ImmutableList<TValue> value) {
            _list = value;
        }
    }
}
