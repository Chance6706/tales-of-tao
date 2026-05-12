using System;
using UnityEngine;

namespace TalesOfTao.Hex
{
    // Axial coordinate (q, r) for storage and serialization.
    // Cube coordinate s is derived on demand: s = -q - r.
    // All distance/pathfinding algorithms operate on the cube form.
    //
    // Flat-top orientation (first vertex at 0°, flat edge at top and bottom).
    // World layout: q grows right, r grows down-right.
    [Serializable]
    public struct HexCoords : IEquatable<HexCoords>
    {
        public int Q;
        public int R;

        // Cube third axis — always derived so serialization stays 2D.
        public int S => -Q - R;

        public static readonly HexCoords Zero = new(0, 0);

        // Six direction offsets (flat-top, E→NE→NW→W→SW→SE).
        // Note: array contents must not be modified.
        // The six cube-coordinate direction offsets (flat-top, ordered by angle).
        public static readonly HexCoords[] Directions =
        {
            new( 1,  0), // E
            new( 1, -1), // NE
            new( 0, -1), // NW
            new(-1,  0), // W
            new(-1,  1), // SW
            new( 0,  1), // SE
        };

        public HexCoords(int q, int r) { Q = q; R = r; }

        public HexCoords Neighbour(int direction) =>
            this + Directions[((direction % 6) + 6) % 6];

        // Returns the neighbour in the given direction (0–5, wraps).
        public HexCoords Neighbour(int direction) =>
            this + Directions[((direction % 6) + 6) % 6];

        // Cube-coordinate distance — exact integer result, no sqrt.
        public static int Distance(HexCoords a, HexCoords b) =>
            (Mathf.Abs(a.Q - b.Q) + Mathf.Abs(a.R - b.R) + Mathf.Abs(a.S - b.S)) / 2;

        public int DistanceTo(HexCoords other) => Distance(this, other);

        public Vector3 ToWorldPosition(float size)
        {
            float x = size * 1.5f * Q;
            float z = size * (0.866025f * Q + 1.732051f * R);
            return new Vector3(x, 0f, z);
        }

        public static HexCoords operator +(HexCoords a, HexCoords b) => new(a.Q + b.Q, a.R + b.R);
        public static HexCoords operator -(HexCoords a, HexCoords b) => new(a.Q - b.Q, a.R - b.R);
        public static HexCoords operator *(HexCoords a, int k)       => new(a.Q * k, a.R * k);
        public static HexCoords operator *(int k, HexCoords a)       => new(a.Q * k, a.R * k);
        // Converts axial coordinates to a world-space position on the XZ plane.
        // size = outer radius of the hex (centre to vertex).
        // Flat-top formula: x = size * 3/2 * q, z = size * (√3/2 * q + √3 * r).
        public Vector3 ToWorldPosition(float size)
        {
            float x = size * 1.5f * Q;
            float z = size * (0.866025f * Q + 1.732051f * R); // √3/2 and √3
            return new Vector3(x, 0f, z);
        }

        // ── Operators ─────────────────────────────────────────────────────────

        public static HexCoords operator +(HexCoords a, HexCoords b) => new(a.Q + b.Q, a.R + b.R);
        public static HexCoords operator -(HexCoords a, HexCoords b) => new(a.Q - b.Q, a.R - b.R);
        public static HexCoords operator *(HexCoords a, int k)       => new(a.Q * k, a.R * k);

        public static bool operator ==(HexCoords a, HexCoords b) => a.Q == b.Q && a.R == b.R;
        public static bool operator !=(HexCoords a, HexCoords b) => !(a == b);

        public bool Equals(HexCoords other) => Q == other.Q && R == other.R;
        public override bool Equals(object obj) => obj is HexCoords c && Equals(c);
        public override int GetHashCode() => HashCode.Combine(Q, R);
        public override string ToString() => $"({Q},{R})";
    }
}
