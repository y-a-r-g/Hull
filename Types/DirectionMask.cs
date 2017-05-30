using System;
using System.Runtime.Serialization;
using Hull.Extensions;

namespace Hull.Types {
    [Serializable]
    public struct DirectionMask : ISerializable {
        private readonly int _mask;

        public DirectionMask(DirectionMask mask = default(DirectionMask)) : this(mask._mask) { }

        public DirectionMask(int mask) {
            _mask = mask;
        }

        private DirectionMask(SerializationInfo info, StreamingContext context) {
            _mask = (int)info.GetValue("_mask", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("_mask", _mask, typeof(int));
        }

        public bool IsEmpty {
            get { return _mask == 0; }
        }

        public bool Contains(Direction direction) {
            return (_mask & direction._mask) != 0;
        }

        public Direction FirstDirection {
            get {
                var mask = this;
                //TODO: optimize
                return Direction.All.Find(d => mask.Contains(d));
            }
        }

        public static DirectionMask operator |(DirectionMask a, Direction b) {
            return new DirectionMask(a._mask | b._mask);
        }

        public static DirectionMask operator |(Direction a, DirectionMask b) {
            return new DirectionMask(a._mask | b._mask);
        }

        public static DirectionMask operator |(DirectionMask a, DirectionMask b) {
            return new DirectionMask(a._mask | b._mask);
        }

        public static DirectionMask operator &(DirectionMask a, DirectionMask b) {
            return new DirectionMask(a._mask & b._mask);
        }

        public DirectionMask Without(Direction direction) {
            return new DirectionMask(_mask & (~direction._mask));
        }

        public DirectionMask Without(DirectionMask mask) {
            return new DirectionMask(_mask & (~mask._mask));
        }

        public static bool operator ==(DirectionMask a, DirectionMask b) {
            return a.Equals(b);
        }

        public static bool operator !=(DirectionMask a, DirectionMask b) {
            return !(a == b);
        }

        private bool Equals(DirectionMask other) {
            return _mask == other._mask;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DirectionMask && Equals((DirectionMask)obj);
        }

        public override int GetHashCode() {
            return _mask;
        }

        public int ToInt() {
            return _mask;
        }
    }
}
