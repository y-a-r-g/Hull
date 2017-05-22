using System;

namespace Hull.Types {
    [Serializable]
    public struct Vector2I {
        public static readonly Vector2I Zero = new Vector2I(0, 0);

        public int X;
        public int Y;

        public Vector2I(int x, int y) {
            X = x;
            Y = y;
        }

        public static Vector2I operator +(Vector2I a, Vector2I b) {
            return new Vector2I(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2I operator -(Vector2I a, Vector2I b) {
            return new Vector2I(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Vector2I a, Vector2I b) {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Vector2I a, Vector2I b) {
            return !(a == b);
        }

        public override bool Equals(object other) {
            if ((other == null) || (other.GetType() != typeof(Vector2I))) {
                return false;
            }
            return this == (Vector2I)other;
        }

        public override string ToString() {
            return string.Format("{0} : {1}", X, Y);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}