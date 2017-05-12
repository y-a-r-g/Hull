namespace Hull.Types {
    public struct Vector4I {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public Vector4I(int x, int y, int z, int w) {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static Vector4I operator +(Vector4I a, Vector4I b) {
            return new Vector4I(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Vector4I operator -(Vector4I a, Vector4I b) {
            return new Vector4I(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static bool operator ==(Vector4I a, Vector4I b) {
            return (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z) && (a.W == b.W);
        }

        public static bool operator !=(Vector4I a, Vector4I b) {
            return !(a == b);
        }

        public override bool Equals(object other) {
            if ((other == null) || (other.GetType() != typeof(Vector4I))) {
                return false;
            }
            return this == (Vector4I)other;
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
