using Hull.Types;
using NUnit.Framework;

namespace Hull.Unity.Editor.Tests.Types {
    [TestFixture]
    public class DirectionTest {
        [Test]
        public void DirectionsHaveDifferentRepresentation() {
            foreach (var a in Direction.AllAndNone) {
                foreach (var b in Direction.AllAndNone) {
                    if (a != b) {
                        Assert.AreNotEqual(a, b);
                        Assert.AreNotEqual(a.ToString(), b.ToString());
                        Assert.False(a.Delta == b.Delta);
                    }
                }
                Assert.AreNotEqual(a, Direction.Invalid);
                Assert.AreNotEqual(a.ToString(), Direction.Invalid.ToString());
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
        public void NextCW() {
            Assert.AreEqual(Direction.NE, Direction.N.NextCW);
            Assert.AreEqual(Direction.E, Direction.NE.NextCW);
            Assert.AreEqual(Direction.SE, Direction.E.NextCW);
            Assert.AreEqual(Direction.S, Direction.SE.NextCW);
            Assert.AreEqual(Direction.SW, Direction.S.NextCW);
            Assert.AreEqual(Direction.W, Direction.SW.NextCW);
            Assert.AreEqual(Direction.NW, Direction.W.NextCW);
            Assert.AreEqual(Direction.N, Direction.NW.NextCW);
        }

        [Test]
        public void NextCCW() {
            Assert.AreEqual(Direction.NW, Direction.N.NextCCW);
            Assert.AreEqual(Direction.N, Direction.NE.NextCCW);
            Assert.AreEqual(Direction.NE, Direction.E.NextCCW);
            Assert.AreEqual(Direction.E, Direction.SE.NextCCW);
            Assert.AreEqual(Direction.SE, Direction.S.NextCCW);
            Assert.AreEqual(Direction.S, Direction.SW.NextCCW);
            Assert.AreEqual(Direction.SW, Direction.W.NextCCW);
            Assert.AreEqual(Direction.W, Direction.NW.NextCCW);
        }

        [Test]
        public void Negative() {
            Assert.AreEqual(Direction.S, Direction.N.Negative);
            Assert.AreEqual(Direction.SW, Direction.NE.Negative);
            Assert.AreEqual(Direction.W, Direction.E.Negative);
            Assert.AreEqual(Direction.NW, Direction.SE.Negative);
            Assert.AreEqual(Direction.N, Direction.S.Negative);
            Assert.AreEqual(Direction.NE, Direction.SW.Negative);
            Assert.AreEqual(Direction.E, Direction.W.Negative);
            Assert.AreEqual(Direction.SE, Direction.NW.Negative);
        }
    }
}
