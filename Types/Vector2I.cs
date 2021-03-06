﻿using System;
using System.IO;

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

        public Vector2I(BinaryReader reader) {
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
        }

        public void Serialize(BinaryWriter writer) {
            writer.Write(X);
            writer.Write(Y);
        }

        public static Vector2I operator +(Vector2I a, Vector2I b) {
            return new Vector2I(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2I operator -(Vector2I a, Vector2I b) {
            return new Vector2I(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2I operator *(Vector2I v, int m) {
            return new Vector2I(v.X * m, v.Y * m);
        }

        public static Vector2I operator *(int m, Vector2I v) {
            return new Vector2I(v.X * m, v.Y * m);
        }

        public static Vector2I operator /(Vector2I v, int d) {
            return new Vector2I(v.X / d, v.Y / d);
        }

        public static bool operator ==(Vector2I a, Vector2I b) {
            return (a.X == b.X) && (a.Y == b.Y);
        }

        public static bool operator !=(Vector2I a, Vector2I b) {
            return !(a == b);
        }

        public Vector2I Clamped {
            get {
                var cx = X;
                if (cx < -1) {
                    cx = -1;
                }
                if (cx > 1) {
                    cx = 1;
                }
                var cy = Y;
                if (cy < -1) {
                    cy = -1;
                }
                if (cy > 1) {
                    cy = 1;
                }
                return new Vector2I(cx, cy);
            }
        }

        public override bool Equals(object other) {
            if ((other == null) || (other.GetType() != typeof(Vector2I))) {
                return false;
            }

            return this == (Vector2I)other;
        }

        public override string ToString() {
            return string.Format("{0}, {1}", X, Y);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}
