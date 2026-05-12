using System.Collections;
using NUnit.Framework;
using TalesOfTao.Hex;
using UnityEngine;
using UnityEngine.TestTools;

namespace TalesOfTao.Tests.Hex
{
    // PlayMode — needs the Unity runtime to execute MonoBehaviour.Start().
    // Run from: Window > General > Test Runner > PlayMode.
    public class HexTileMeshTests
    {
        private GameObject _go;

        [SetUp]
        public void SetUp()
        {
            // RequireComponent auto-adds MeshFilter, MeshRenderer, MeshCollider.
            _go = new GameObject("TestHexTile");
            _go.AddComponent<MeshFilter>();
            _go.AddComponent<MeshRenderer>();
            _go.AddComponent<MeshCollider>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
                Object.Destroy(_go);
        }

        // ── Static mesh generation (no MonoBehaviour lifecycle needed) ────────

        [Test]
        public void GenerateHexMesh_Has12Vertices()
        {
            var mesh = HexTile.GenerateHexMesh(1f, 0.3f);
            Assert.AreEqual(12, mesh.vertexCount);
        }

        [Test]
        public void GenerateHexMesh_Has60Indices()
        {
            var mesh = HexTile.GenerateHexMesh(1f, 0.3f);
            Assert.AreEqual(60, mesh.triangles.Length);
        }

        [Test]
        public void GenerateHexMesh_BoundsWidthApproximatesTwiceSize()
        {
            const float size = 2f;
            var mesh = HexTile.GenerateHexMesh(size, 0.3f);
            // Flat-top hex outer radius = size; width = 2 * size.
            Assert.AreEqual(size * 2f, mesh.bounds.size.x, 0.001f);
        }

        [Test]
        public void GenerateHexMesh_BoundsHeightMatchesParameter()
        {
            const float height = 0.5f;
            var mesh = HexTile.GenerateHexMesh(1f, height);
            Assert.AreEqual(height, mesh.bounds.size.y, 0.001f);
        }

        // ── PlayMode: verify Start() wires the mesh into MeshFilter/MeshCollider ──

        [UnityTest]
        public IEnumerator Start_AssignsMeshToMeshFilter()
        {
            _go.AddComponent<HexTile>();
            yield return null; // one frame — lets Start() execute

            var filter = _go.GetComponent<MeshFilter>();
            Assert.IsNotNull(filter.sharedMesh, "MeshFilter.sharedMesh should not be null after Start()");
            Assert.AreEqual(12, filter.sharedMesh.vertexCount);
        }

        [UnityTest]
        public IEnumerator Start_AssignsMeshToMeshCollider()
        {
            _go.AddComponent<HexTile>();
            yield return null;

            var col = _go.GetComponent<MeshCollider>();
            Assert.IsNotNull(col.sharedMesh, "MeshCollider.sharedMesh should not be null after Start()");
        }

        [UnityTest]
        public IEnumerator Start_WithTerrainOverride_DataContainsTerrain()
        {
            var terrain = ScriptableObject.CreateInstance<TerrainTypeSO>();
            var tile    = _go.AddComponent<HexTile>();

            // Inject via reflection so we can test without Inspector wiring.
            var field = typeof(HexTile).GetField(
                "_terrainOverride",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(tile, terrain);

            yield return null;

            Assert.AreSame(terrain, tile.Data.Terrain);

            Object.DestroyImmediate(terrain);
        }
    }
}
