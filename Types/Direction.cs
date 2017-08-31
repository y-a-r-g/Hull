// ReSharper disable InconsistentNaming

using System;
using System.IO;
using System.Runtime.Serialization;

namespace Hull.Types {
    //
    //          ^ N; y++
    //          |
    //          |
    // W; x--   |
    // ---------+---------------> E; x++
    //          |
    //          |
    //          |
    //          | S; y--
    [Serializable]
    public struct Direction : ISerializable {
        internal readonly int _mask;
        private readonly string _id;
        private readonly Vector2I _delta;

        private Direction(int mask, string id, Vector2I delta) {
            _mask = mask;
            _id = id;
            _delta = delta;
        }

        private Direction(SerializationInfo info, StreamingContext context) {
            _mask = (int)info.GetValue("_mask", typeof(int));
            _id = (string)info.GetValue("_id", typeof(string));
            _delta = (Vector2I)info.GetValue("_delta", typeof(Vector2I));
        }

        public Direction(BinaryReader reader) {
            _mask = reader.ReadInt32();
            _id = reader.ReadString();
            if (_id == "") {
                _id = null;
            }
            _delta = new Vector2I(reader);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("_mask", _mask, typeof(int));
            info.AddValue("_id", _id, typeof(string));
            info.AddValue("_delta", _delta, typeof(Vector2I));
        }

        public void Serialize(BinaryWriter writer) {
            writer.Write(_mask);
            writer.Write(_id ?? "");
            _delta.Serialize(writer);
        }

        public Vector2I Delta {
            get { return _delta; }
        }

        public static Direction FromDelta(int x, int y) {
            if (x == -1) {
                if (y == -1) {
                    return SW;
                }
                if (y == 0) {
                    return W;
                }
                if (y == 1) {
                    return NW;
                }
            }
            else if (x == 0) {
                if (y == -1) {
                    return S;
                }
                if (y == 0) {
                    return None;
                }
                if (y == 1) {
                    return N;
                }
            }
            else if (x == 1) {
                if (y == -1) {
                    return SE;
                }
                if (y == 0) {
                    return E;
                }
                if (y == 1) {
                    return NE;
                }
            }

            return Invalid;
        }

        public static Direction FromDelta(Vector2I delta) {
            return FromDelta(delta.X, delta.Y);
        }

        private static Direction FromMask(int mask) {
            switch (mask) {
                case 0x100:
                    return None;
                case 0x01:
                    return N;
                case 0x02:
                    return NE;
                case 0x04:
                    return E;
                case 0x08:
                    return SE;
                case 0x10:
                    return S;
                case 0x20:
                    return SW;
                case 0x40:
                    return W;
                case 0x80:
                    return NW;
            }

            return Invalid;
        }

        public static explicit operator DirectionMask(Direction direction) {
            return new DirectionMask(direction._mask);
        }

        public Direction NextCW {
            get { return FromMask(((_mask << 1) & 0xFF) | ((_mask >> 7) & 0x01)); }
        }

        public Direction NextCW4 {
            get { return FromMask(((_mask << 2) & 0xFF) | ((_mask >> 6) & 0x03)); }
        }

        public Direction NextCCW {
            get { return FromMask((_mask >> 1) | ((_mask & 0x01) << 7)); }
        }

        public Direction NextCCW4 {
            get { return FromMask((_mask >> 2) | ((_mask & 0x03) << 6)); }
        }

        public Direction Negative {
            get {
                var negative = _mask << 4;
                if (negative > 0x80) {
                    negative = _mask >> 4;
                }
                return FromMask(negative);
            }
        }

        public override string ToString() {
            return _id;
        }

        public static bool operator ==(Direction a, Direction b) {
            return a.Equals(b);
        }

        public static bool operator !=(Direction a, Direction b) {
            return !(a == b);
        }

        public static DirectionMask operator |(Direction a, Direction b) {
            return new DirectionMask(a._mask | b._mask);
        }

        private bool Equals(Direction other) {
            return _mask == other._mask;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Direction && Equals((Direction)obj);
        }

        public override int GetHashCode() {
            return _mask;
        }

        public static readonly Direction Invalid = new Direction(0, "", Vector2I.Zero);
        public static readonly Direction None = new Direction(0x100, " ", Vector2I.Zero);
        public static readonly Direction N = new Direction(0x01, "N", new Vector2I(0, 1));
        public static readonly Direction NE = new Direction(0x02, "NE", new Vector2I(1, 1));
        public static readonly Direction E = new Direction(0x04, "E", new Vector2I(1, 0));
        public static readonly Direction SE = new Direction(0x08, "SE", new Vector2I(1, -1));
        public static readonly Direction S = new Direction(0x10, "S", new Vector2I(0, -1));
        public static readonly Direction SW = new Direction(0x20, "SW", new Vector2I(-1, -1));
        public static readonly Direction W = new Direction(0x40, "W", new Vector2I(-1, 0));
        public static readonly Direction NW = new Direction(0x80, "NW", new Vector2I(-1, 1));

        public static readonly Direction[] All = {N, NE, E, SE, S, SW, W, NW};
        public static readonly Direction[] All4 = {N, E, S, W};
        public static readonly Direction[] AllAndNone = {N, NE, E, SE, S, SW, W, NW, None};
    }
}