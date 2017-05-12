using System;

namespace Hull.Collections {
    [Serializable]
    public struct LinearMapId {
        internal readonly int Value;

        public LinearMapId(int value) {
            Value = value;
        }

        public override bool Equals(object obj) {
            return (obj != null) && (Value == ((LinearMapId)obj).Value);
        }

        public override int GetHashCode() {
            return Value;
        }
    }
}