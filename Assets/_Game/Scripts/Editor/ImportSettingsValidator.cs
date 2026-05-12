using UnityEngine;
using UnityEditor;

namespace TalesOfTao.Editor
{
    // TalesOfTao > 3 - Validate Mesh Import Settings
    //
    // Batch-configures every .obj file under Assets/_Game/Art/Meshes/ to:
    //   • Scale Factor  = 1 (prevents unexpected size differences between models)
    //   • Normals       = Import (preserves artist-authored normals for correct lighting)
    //   • Read/Write    = false (saves GPU memory; enable per-mesh if CPU access needed)
    //
    // Run this after dropping .obj files from:
    //   C:\...\Wu_Dang_Unity_Assets  → Assets/_Game/Art/Meshes/Units/ and Buildings/
    //   C:\...\Wu_Dang_Hex_System    → Assets/_Game/Art/Meshes/HexTiles/Base/ and Features/
    public static class ImportSettingsValidator
    {
        private const string MeshRoot = "Assets/_Game/Art/Meshes";

        [MenuItem("TalesOfTao/3 - Validate Mesh Import Settings")]
        public static void ValidateMeshImports()
        {
            int fixed_ = 0, skipped = 0;

            string[] guids = AssetDatabase.FindAssets("t:DefaultAsset", new[] { MeshRoot });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null) { skipped++; continue; }

                bool changed = false;

                if (!Mathf.Approximately(importer.globalScale, 1f))
                {
                    importer.globalScale = 1f;
                    changed = true;
                }

                if (importer.importNormals != ModelImporterNormals.Import)
                {
                    importer.importNormals = ModelImporterNormals.Import;
                    changed = true;
                }

                if (importer.isReadable)
                {
                    importer.isReadable = false;
                    changed = true;
                }

                if (changed)
                {
                    importer.SaveAndReimport();
                    fixed_++;
                    Debug.Log($"[ImportSettings] Fixed: {path}");
                }
            }

            Debug.Log($"[TalesOfTao] Import validation complete — {fixed_} fixed, {skipped} skipped.");
            AssetDatabase.Refresh();
        }
    }
}
