// ReSharper disable InconsistentNaming

using Hull.Extensions;

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
    public class Direction {
        private readonly int _mask;
        private readonly string _id;
        private readonly Vector2I _delta;

        private Direction(int mask, string id, Vector2I delta) {
            _mask = mask;
            _id = id;
            _delta = delta;
        }

        public static implicit operator int(Direction direction) {
            return direction._mask;
        }

        public static implicit operator string(Direction direction) {
            return direction._id;
        }

        public static explicit operator Vector2I(Direction direction) {
            return direction._delta;
        }

        public static Direction FromDelta(int x, int y) {
            return FromDelta(new Vector2I(x, y));
        }

        public static Direction FromDelta(Vector2I delta) {
            //TODO: optimize
            return AllAndNone.Find(d => d._delta == delta) ?? Invalid;
        }

        public static Direction FromMask(int mask) {
            //TODO: optimize
            return AllAndNone.Find(d => d._mask == mask) ?? Invalid;
        }

        public static Direction NextCW(Direction direction, bool fourDirections = false) {
            return FromMask(direction == 0x80 ? 0x1 : direction << (fourDirections ? 2 : 1));
        }
        
        public static Direction NextCCW(Direction direction, bool fourDirections = false) {
            return FromMask(direction == 0x01 ? 0x80 : direction >> (fourDirections ? 2 : 1));
        }

        public static Direction Negative(Direction direction) {
            var negative = direction << 4;
            if (negative > 0x80) {
                negative = direction >> 4;
            }
            return FromMask(negative);
        }

        public static int Extract(int mask) {
            return All.Find(d => (d & mask) != 0) ?? Invalid;
        }

        public static readonly Direction Invalid = new Direction(0x1000, "X", Vector2I.Zero);
        public static readonly Direction None = new Direction(0, " ", Vector2I.Zero);
        public static readonly Direction N = new Direction( 0x01, "N", new Vector2I(0, 1));
        public static readonly Direction NE = new Direction(0x02, "NE", new Vector2I(1, 1));
        public static readonly Direction E = new Direction( 0x04, "E", new Vector2I(1, 0));
        public static readonly Direction SE = new Direction(0x08, "SE", new Vector2I(1, -1));
        public static readonly Direction S = new Direction( 0x10, "S", new Vector2I(0, -1));
        public static readonly Direction SW = new Direction(0x20, "SW", new Vector2I(-1, -1));
        public static readonly Direction W = new Direction( 0x40, "W", new Vector2I(-1, 0));
        public static readonly Direction NW = new Direction(0x80, "NW", new Vector2I(-1, 1));

        public static readonly Direction[] All = {N, NE, E, SE, S, SW, W, NW};
        public static readonly Direction[] All4 = {N, E, S, W};
        public static readonly Direction[] AllAndNone = {N, NE, E, SE, S, SW, W, NW, None};
    }
}
