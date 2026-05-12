using NUnit.Framework;
using TalesOfTao.Hex;
using UnityEngine;

namespace TalesOfTao.Tests.Hex
{
    // EditMode — no scene, no Unity lifecycle, pure math.
    public class HexCoordsTests
    {
        // ── Cube constraint ───────────────────────────────────────────────────

        [Test]
        public void S_AlwaysEqualsNegativeQMinusR()
        {
            var coords = new HexCoords(3, -1);
            Assert.AreEqual(-coords.Q - coords.R, coords.S);
        }

        // ── Distance ─────────────────────────────────────────────────────────

        [Test]
        public void Distance_SameCoord_IsZero()
        {
            Assert.AreEqual(0, HexCoords.Distance(HexCoords.Zero, HexCoords.Zero));
        }

        [Test]
        public void Distance_AdjacentTile_IsOne()
        {
            var a = HexCoords.Zero;
            var b = new HexCoords(1, 0);
            Assert.AreEqual(1, HexCoords.Distance(a, b));
        }

        [Test]
        [TestCase(2,  0,  2)]
        [TestCase(0,  3,  3)]
        [TestCase(3, -3,  3)]
        [TestCase(-1, 2,  2)]
        public void Distance_KnownPairs_AreCorrect(int q, int r, int expected)
        {
            Assert.AreEqual(expected, HexCoords.Distance(HexCoords.Zero, new HexCoords(q, r)));
        }

        [Test]
        public void Distance_IsSymmetric()
        {
            var a = new HexCoords(2, -1);
            var b = new HexCoords(-1, 3);
            Assert.AreEqual(HexCoords.Distance(a, b), HexCoords.Distance(b, a));
        }

        // ── Neighbours ────────────────────────────────────────────────────────

        [Test]
        public void Directions_HasSixEntries()
        {
            Assert.AreEqual(6, HexCoords.Directions.Length);
        }

        [Test]
        public void AllNeighbours_AreDistanceOneFromOrigin()
        {
            foreach (var dir in HexCoords.Directions)
                Assert.AreEqual(1, HexCoords.Distance(HexCoords.Zero, dir));
        }

        [Test]
        public void Neighbour_DirectionWraps_NoException()
        {
            var coord = new HexCoords(1, 0);
            Assert.DoesNotThrow(() => coord.Neighbour(6));  // wraps to 0
            Assert.DoesNotThrow(() => coord.Neighbour(-1)); // wraps to 5
        }

        [Test]
        public void Neighbour_OppositeDirections_CancelOut()
        {
            // Moving in direction 0 then direction 3 (opposite) returns to origin.
            var start = HexCoords.Zero;
            var moved = start.Neighbour(0).Neighbour(3);
            Assert.AreEqual(start, moved);
        }

        // ── World position ────────────────────────────────────────────────────

        [Test]
        public void ToWorldPosition_Origin_IsZeroVector()
        {
            var pos = HexCoords.Zero.ToWorldPosition(1f);
            Assert.AreEqual(Vector3.zero, pos);
        }

        [Test]
        public void ToWorldPosition_Q1R0_XIs1Point5TimesSizeZIsHalfSqrt3()
        {
            // Flat-top formula: x = size * 3/2 * q, z = size * (√3/2 * q + √3 * r)
            const float size = 2f;
            var pos = new HexCoords(1, 0).ToWorldPosition(size);

            Assert.AreEqual(size * 1.5f, pos.x, 0.0001f);
            Assert.AreEqual(size * 0.866025f, pos.z, 0.0001f);
            Assert.AreEqual(0f, pos.y, 0.0001f);
        }

        [Test]
        public void ToWorldPosition_YIsAlwaysZero()
        {
            Assert.AreEqual(0f, new HexCoords(3, -2).ToWorldPosition(1f).y);
        }

        // ── Operators ─────────────────────────────────────────────────────────

        [Test]
        public void Addition_TwoCoords_SumsComponents()
        {
            var result = new HexCoords(1, 2) + new HexCoords(3, -1);
            Assert.AreEqual(new HexCoords(4, 1), result);
        }

        [Test]
        public void Equality_SameValues_AreEqual()
        {
            Assert.AreEqual(new HexCoords(2, -3), new HexCoords(2, -3));
        }

        [Test]
        public void Equality_DifferentValues_AreNotEqual()
        {
            Assert.AreNotEqual(new HexCoords(1, 0), new HexCoords(0, 1));
        }

        [Test]
        public void Multiply_ByScalar_ScalesComponents()
        {
            var result = new HexCoords(2, -1) * 3;
            Assert.AreEqual(new HexCoords(6, -3), result);
        }
    }
}
