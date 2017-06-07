using Hull.Types;
using NUnit.Framework;

namespace Hull.Unity.Editor.Tests.Types {
    [TestFixture]
    public class DirectionMaskTest {
        [Test]
        public void NextCW() {
            Assert.AreEqual((DirectionMask)Direction.N.NextCW, ((DirectionMask)Direction.N).NextCW);
            Assert.AreEqual((Direction.NE.NextCW | Direction.SE.NextCW), (Direction.NE | Direction.SE).NextCW);
            Assert.AreEqual(
                (Direction.S.NextCW | Direction.SE.NextCW | Direction.NE.NextCW),
                (Direction.S | Direction.SE | Direction.NE).NextCW);
        }

        [Test]
        public void NextCCW() {
            Assert.AreEqual((DirectionMask)Direction.N.NextCCW, ((DirectionMask)Direction.N).NextCCW);
            Assert.AreEqual((Direction.NE.NextCCW | Direction.SE.NextCCW), (Direction.NE | Direction.SE).NextCCW);
            Assert.AreEqual(
                (Direction.S.NextCCW | Direction.SE.NextCCW | Direction.NE.NextCCW),
                (Direction.S | Direction.SE | Direction.NE).NextCCW);
        }

        [Test]
        public void NextCW4() {
            Assert.AreEqual((DirectionMask)Direction.N.NextCW4, ((DirectionMask)Direction.N).NextCW4);
            Assert.AreEqual((Direction.NE.NextCW4 | Direction.SE.NextCW4), (Direction.NE | Direction.SE).NextCW4);
            Assert.AreEqual(
                (Direction.S.NextCW4 | Direction.SE.NextCW4 | Direction.NE.NextCW4),
                (Direction.S | Direction.SE | Direction.NE).NextCW4);
        }

        [Test]
        public void NextCCW4() {
            Assert.AreEqual((DirectionMask)Direction.N.NextCCW4, ((DirectionMask)Direction.N).NextCCW4);
            Assert.AreEqual((Direction.NE.NextCCW4 | Direction.SE.NextCCW4), (Direction.NE | Direction.SE).NextCCW4);
            Assert.AreEqual(
                (Direction.S.NextCCW4 | Direction.SE.NextCCW4 | Direction.NE.NextCCW4),
                (Direction.S | Direction.SE | Direction.NE).NextCCW4);
        }
    }
}
