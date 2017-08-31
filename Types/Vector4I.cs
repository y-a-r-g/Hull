using System;
using System.IO;

namespace Hull.Types {
    [Serializable]
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

        public Vector4I(BinaryReader reader) {
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            Z = reader.ReadInt32();
            W = reader.ReadInt32();
        }

        public void Serialize(BinaryWriter writer) {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            writer.Write(W);
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

        public int this[int index] {
            get {
                switch (index) {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                }

                throw new ArgumentOutOfRangeException("index", "Should be in [0..3]");
            }
        }

        public override bool Equals(object other) {
            if ((other == null) || (other.GetType() != typeof(Vector4I))) {
                return false;
            }

            return this == (Vector4I)other;
        }

        public override string ToString() {
            return string.Format("{0}, {1}, {2}, {3}", X, Y, Z, W);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}