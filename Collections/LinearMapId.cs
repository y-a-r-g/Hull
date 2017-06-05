using System;

namespace Hull.Collections {
    [Serializable]
    public struct LinearMapId {
        internal readonly int Value;

        public LinearMapId(int value) {
            Value = value;
        }

        public static bool operator ==(LinearMapId a, LinearMapId b) {
            return a.Value == b.Value;
        }

        public static bool operator !=(LinearMapId a, LinearMapId b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            return (obj != null) && (Value == ((LinearMapId)obj).Value);
        }

        public override int GetHashCode() {
            return Value;
        }
    }
}
