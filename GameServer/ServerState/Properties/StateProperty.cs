using System;
using System.Runtime.Serialization;
using Hull.GameServer.Interfaces;

namespace Hull.GameServer.ServerState.Properties {
    /// <summary>
    /// Simple state property. Can hold any type of the value. Though it cannot monitor internal value changes. It will be marked as modified only if whole value changed using <code>Set</code> method.
    /// </summary>
    /// <typeparam name="TValue">Type of property value</typeparam>
    [Serializable]
    public class StateProperty<TValue> : AbstractStateProperty {
        private TValue _value;

        /// <summary>
        /// Creates property with default value
        /// </summary>
        public StateProperty() : this(default(TValue)) { }

        /// <summary>
        /// Creates property with given value
        /// </summary>
        /// <param name="value">Initial property value</param>
        public StateProperty(TValue value) {
            _value = value;
        }

        protected StateProperty(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            _value = (TValue)info.GetValue("_value", typeof(TValue));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("_value", _value, typeof(TValue));
        }

        /// <summary>
        /// Current property value
        /// </summary>
        public TValue Value {
            get { return _value; }
        }

        /// <summary>
        /// Cast operator to the property type
        /// </summary>
        /// <param name="property"></param>
        /// <returns>Current property value</returns>
        public static implicit operator TValue(StateProperty<TValue> property) {
            return property._value;
        }

        /// <summary>
        /// Changes the property value and marks property as modified
        /// </summary>
        /// <param name="value">New property value</param>
        public void Set(TValue value) {
            Modify(ModificationType.Changed);
            _value = value;
        }

        /// <summary>
        /// Changes property value only if nullable has value
        /// </summary>
        /// <param name="value">New property value</param>
        /// <typeparam name="TStructValue"></typeparam>
        public void Set<TStructValue>(TStructValue? value) where TStructValue : struct, TValue {
            if (value.HasValue) {
                Set(value.Value);
            }
        }

        /// <summary>
        /// Calls <code>ToString</code> method of the value that property currently holds 
        /// </summary>
        /// <returns>String representation of the value</returns>
        public override string ToString() {
            if (_value != null) {
                return _value.ToString();
            }
            return "";
        }
    }
}
