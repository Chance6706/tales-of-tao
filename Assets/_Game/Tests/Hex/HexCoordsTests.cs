using NUnit.Framework;
using TalesOfTao.Hex;

namespace TalesOfTao.Tests.Hex
{
    [TestFixture]
    public class HexCoordsTests
    {
        [Test]
        public void Zero_IsOrigin()
        {
            Assert.AreEqual(0, HexCoords.Zero.Q);
            Assert.AreEqual(0, HexCoords.Zero.R);
            Assert.AreEqual(0, HexCoords.Zero.S);
        }

        [Test]
        public void S_IsDerivedFromQAndR()
        {
            var hex = new HexCoords(3, -2);
            Assert.AreEqual(-1, hex.S); // -(3) - (-2) = -1
        }

        [Test]
        public void Distance_ToSelf_IsZero()
        {
            var hex = new HexCoords(3, -2);
            Assert.AreEqual(0, hex.DistanceTo(hex));
        }

        [Test]
        public void Distance_IsSymmetric()
        {
            var a = new HexCoords(0, 0);
            var b = new HexCoords(3, -2);
            Assert.AreEqual(HexCoords.Distance(a, b), HexCoords.Distance(b, a));
        }

        [Test]
        public void Distance_ToNeighbour_IsOne()
        {
            var center = new HexCoords(0, 0);
            for (int dir = 0; dir < 6; dir++)
            {
                var neighbour = center.Neighbour(dir);
                Assert.AreEqual(1, HexCoords.Distance(center, neighbour),
                    $"Distance in direction {dir} should be 1");
            }
        }

        [Test]
        public void Neighbour_WrapsDirection()
        {
            var center = new HexCoords(0, 0);
            // direction 6 should wrap to direction 0 (East)
            var expected = center.Neighbour(0);
            var actual = center.Neighbour(6);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Neighbour_NegativeDirection_WrapsCorrectly()
        {
            var center = new HexCoords(0, 0);
            // direction -1 should wrap to direction 5 (SE)
            var expected = center.Neighbour(5);
            var actual = center.Neighbour(-1);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Equality_SameCoords_AreEqual()
        {
            var a = new HexCoords(3, -2);
            var b = new HexCoords(3, -2);
            Assert.AreEqual(a, b);
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
        }

        [Test]
        public void Equality_DifferentCoords_AreNotEqual()
        {
            var a = new HexCoords(3, -2);
            var b = new HexCoords(2, -1);
            Assert.AreNotEqual(a, b);
            Assert.IsTrue(a != b);
            Assert.IsFalse(a == b);
        }

        [Test]
        public void GetHashCode_SameCoords_Match()
        {
            var a = new HexCoords(3, -2);
            var b = new HexCoords(3, -2);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Addition_Works()
        {
            var a = new HexCoords(1, 2);
            var b = new HexCoords(3, -1);
            var result = a + b;
            Assert.AreEqual(4, result.Q);
            Assert.AreEqual(1, result.R);
        }

        [Test]
        public void Subtraction_Works()
        {
            var a = new HexCoords(5, 3);
            var b = new HexCoords(2, 1);
            var result = a - b;
            Assert.AreEqual(3, result.Q);
            Assert.AreEqual(2, result.R);
        }

        [Test]
        public void ScalarMultiplication_Works()
        {
            var a = new HexCoords(2, -1);
            var result = a * 3;
            Assert.AreEqual(6, result.Q);
            Assert.AreEqual(-3, result.R);
        }

        [Test]
        public void ToString_IncludesAllCoordinates()
        {
            var hex = new HexCoords(3, -2);
            Assert.AreEqual("(3,-2,-1)", hex.ToString());
        }
    }
}
