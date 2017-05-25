using Hull.Types;
using NUnit.Framework;

namespace Hull.Unity.Editor.Tests.Types {
    [TestFixture]
    public class Vector2ITest {
        [Test]
        public void Constructor() {
            var v = new Vector2I(42, 17);
            Assert.AreEqual(42, v.X);
            Assert.AreEqual(17, v.Y);

            var zero = Vector2I.Zero;
            Assert.AreEqual(0, zero.X);
            Assert.AreEqual(0, zero.Y);
        }

        [Test]
        public void Plus() {
            var a = new Vector2I(42, 17);
            var b = new Vector2I(6, 50);
            var sum = a + b;
            Assert.AreEqual(48, sum.X);
            Assert.AreEqual(67, sum.Y);
        }

        [Test]
        public void Minus() {
            var a = new Vector2I(42, 17);
            var b = new Vector2I(6, 50);
            var dif = a - b;
            Assert.AreEqual(36, dif.X);
            Assert.AreEqual(-33, dif.Y);
        }

        [Test]
        public void Equality() {
            var a = new Vector2I(42, 17);
            var b = new Vector2I(42, 17);
            Assert.True(a == b);
            Assert.False(a != b);
            Assert.AreEqual(a, b);
        }

        [Test]
        public void NotEqualily() {
            var a = new Vector2I(42, 17);
            var b = new Vector2I(6, 50);

            Assert.False(a == b);
            Assert.True(a != b);
            Assert.AreNotEqual(a, b);
        }
    }
}
