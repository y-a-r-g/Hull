using System;
using System.Runtime.Serialization;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// Simple array state property. Can hold array of value type. Though it cannot monitor internal item value changes. It will be marked as modified only if item or whole array is replaced.
    /// </summary>
    /// <typeparam name="TValue">Type of value array</typeparam>
    [Serializable]
    public class SimpleArrayStateProperty<TValue> : AbstractArrayStateProperty<TValue> where TValue : struct {
        public SimpleArrayStateProperty() { }
        public SimpleArrayStateProperty(TValue[] value, bool doNotCopyReference = false) : base(value, doNotCopyReference) { }
        public SimpleArrayStateProperty(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}