using System;
using System.Runtime.Serialization;

namespace Hull.GameServer.ServerState.Properties {
    [Serializable]
    public class StateProperty<TValue> : AbstractStateProperty {
        private TValue _value;

        public StateProperty(TValue value = default(TValue)) {
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

        public TValue Value {
            get { return _value; }
        }

        public static implicit operator TValue(StateProperty<TValue> property) {
            return property._value;
        }

        public void Set(TValue value) {
            Modify();
            _value = value;
        }

        public override string ToString() {
            return _value.ToString();
        }
    }
}