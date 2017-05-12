using Hull.Types;
using NUnit.Framework;

namespace Hull.Editor.Tests.Types {
    [TestFixture]
    public class DirectionTest {
        [Test]
        public void DirectionsHaveDifferentRepresentation() {
            Assert.AreEqual(0, (int)Direction.None);

            foreach (var a in Direction.AllAndNone) {
                foreach (var b in Direction.AllAndNone) {
                    if (a != b) {
                        Assert.AreNotEqual((int)a, (int)b);
                        Assert.AreNotEqual((string)a, (string)b);
                        Assert.False((Vector2I)a == (Vector2I)b);
                    }
                }
                Assert.AreNotEqual((int)a, (int)Direction.Invalid);
                Assert.AreNotEqual((string)a, (string)Direction.Invalid);
            }
        }

        [Test]
        public void FromDelta() {
            Assert.AreEqual(Direction.N, Direction.FromDelta(0, 1));
            Assert.AreEqual(Direction.NE, Direction.FromDelta(1, 1));
            Assert.AreEqual(Direction.E, Direction.FromDelta(1, 0));
            Assert.AreEqual(Direction.SE, Direction.FromDelta(1, -1));
            Assert.AreEqual(Direction.S, Direction.FromDelta(0, -1));
            Assert.AreEqual(Direction.SW, Direction.FromDelta(-1, -1));
            Assert.AreEqual(Direction.W, Direction.FromDelta(-1, 0));
            Assert.AreEqual(Direction.NW, Direction.FromDelta(-1, 1));

            Assert.AreEqual(Direction.N, Direction.FromDelta(new Vector2I(0, 1)));
            Assert.AreEqual(Direction.NE, Direction.FromDelta(new Vector2I(1, 1)));
            Assert.AreEqual(Direction.E, Direction.FromDelta(new Vector2I(1, 0)));
            Assert.AreEqual(Direction.SE, Direction.FromDelta(new Vector2I(1, -1)));
            Assert.AreEqual(Direction.S, Direction.FromDelta(new Vector2I(0, -1)));
            Assert.AreEqual(Direction.SW, Direction.FromDelta(new Vector2I(-1, -1)));
            Assert.AreEqual(Direction.W, Direction.FromDelta(new Vector2I(-1, 0)));
            Assert.AreEqual(Direction.NW, Direction.FromDelta(new Vector2I(-1, 1)));
        }

        [Test]
        public void FromMask() {
            Assert.AreEqual(Direction.N, Direction.FromMask((int)Direction.N));
            Assert.AreEqual(Direction.NE, Direction.FromMask((int)Direction.NE));
            Assert.AreEqual(Direction.E, Direction.FromMask((int)Direction.E));
            Assert.AreEqual(Direction.SE, Direction.FromMask((int)Direction.SE));
            Assert.AreEqual(Direction.S, Direction.FromMask((int)Direction.S));
            Assert.AreEqual(Direction.SW, Direction.FromMask((int)Direction.SW));
            Assert.AreEqual(Direction.W, Direction.FromMask((int)Direction.W));
            Assert.AreEqual(Direction.NW, Direction.FromMask((int)Direction.NW));
            Assert.AreEqual(Direction.None, Direction.FromMask(0));
            Assert.AreEqual(Direction.Invalid, Direction.FromMask(Direction.N | Direction.S));
        }

        [Test]
        public void NextCW() {
            Assert.AreEqual(Direction.NE, Direction.NextCW(Direction.N));
            Assert.AreEqual(Direction.E, Direction.NextCW(Direction.NE));
            Assert.AreEqual(Direction.SE, Direction.NextCW(Direction.E));
            Assert.AreEqual(Direction.S, Direction.NextCW(Direction.SE));
            Assert.AreEqual(Direction.SW, Direction.NextCW(Direction.S));
            Assert.AreEqual(Direction.W, Direction.NextCW(Direction.SW));
            Assert.AreEqual(Direction.NW, Direction.NextCW(Direction.W));
            Assert.AreEqual(Direction.N, Direction.NextCW(Direction.NW));
        }

        [Test]
        public void NextCCW() {
            Assert.AreEqual(Direction.NW, Direction.NextCCW(Direction.N));
            Assert.AreEqual(Direction.N, Direction.NextCCW(Direction.NE));
            Assert.AreEqual(Direction.NE, Direction.NextCCW(Direction.E));
            Assert.AreEqual(Direction.E, Direction.NextCCW(Direction.SE));
            Assert.AreEqual(Direction.SE, Direction.NextCCW(Direction.S));
            Assert.AreEqual(Direction.S, Direction.NextCCW(Direction.SW));
            Assert.AreEqual(Direction.SW, Direction.NextCCW(Direction.W));
            Assert.AreEqual(Direction.W, Direction.NextCCW(Direction.NW));
        }

        [Test]
        public void Negative() {
            Assert.AreEqual(Direction.S, Direction.Negative(Direction.N));
            Assert.AreEqual(Direction.SW, Direction.Negative(Direction.NE));
            Assert.AreEqual(Direction.W, Direction.Negative(Direction.E));
            Assert.AreEqual(Direction.NW, Direction.Negative(Direction.SE));
            Assert.AreEqual(Direction.N, Direction.Negative(Direction.S));
            Assert.AreEqual(Direction.NE, Direction.Negative(Direction.SW));
            Assert.AreEqual(Direction.E, Direction.Negative(Direction.W));
            Assert.AreEqual(Direction.SE, Direction.Negative(Direction.NW));
        }

        [Test]
        public void Extract() {
            var mask = Direction.N | Direction.S;
            var extracted = Direction.Extract(mask);
            if (extracted == Direction.N) {
                Assert.AreEqual(Direction.S, mask & (~extracted));
            }
            else if (extracted == Direction.S) {
                Assert.AreEqual(Direction.N, mask & (~extracted));
            }
            else {
                Assert.Fail();
            }
        }
    }
}
