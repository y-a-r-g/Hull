using System;
using System.Runtime.Serialization;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// Simple list state property. Can hold a list of value type. Though it cannot monitor internal item value changes. It will be marked as modified only if item or whole array is replaced.
    /// </summary>
    /// <typeparam name="TValue">Type of value array</typeparam>
    [Serializable]
    public class SimpleListStateProperty<TValue> : AbstractListStateProperty<TValue> where TValue : struct {
        public SimpleListStateProperty() { }
        public SimpleListStateProperty(TValue[] value, bool doNotCopyReference = false) : base(value, doNotCopyReference) { }
        public SimpleListStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
