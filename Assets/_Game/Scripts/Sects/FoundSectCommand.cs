using UnityEngine;
using UnityEditor;
using TalesOfTao.Core.Commands;
using TalesOfTao.Hex;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Undoable command that founds a sect on the given tile.
    /// Creates SectData, places Temple, removes Founder from roster.
    /// </summary>
    public class FoundSectCommand : Command
    {
        private readonly SectConfigSO _config;
        private readonly int _tileQ;
        private readonly int _tileR;
        private readonly HexGridManager _gridManager;
        private SectData _createdData;
        private bool _executed;

        public SectData CreatedData => _createdData;

        public FoundSectCommand(SectConfigSO config, int tileQ, int tileR, HexGridManager gridManager)
        {
            _config = config;
            _tileQ = tileQ;
            _tileR = tileR;
            _gridManager = gridManager;
        }

        public override bool CanExecute()
        {
            if (_config == null || _gridManager == null) return false;

            var tile = _gridManager.GetTile(_tileQ, _tileR);
            if (tile == null) return false;
            if (tile.IsImpassable) return false;
            if (tile.Terrain?.Type == TerrainType.Lake) return false;

            return true;
        }

        public override void Execute()
        {
            if (!CanExecute()) return;

            var tile = _gridManager.GetTile(_tileQ, _tileR);

            // Create SectData with founding bonuses baked in
            _createdData = new SectData
            {
                SectName = _config.DisplayName,
                Config = _config,
                IsFounded = true,
                FoundingTileQ = _tileQ,
                FoundingTileR = _tileR,
                FoundingStats = new FoundingTileStats
                {
                    BaseQiIncome = GetQiDensityValue(tile.QiDensity) * 10f,
                    CaveBonus = tile.CaveCount * 2f,
                    TerrainDefenseBonus = tile.Terrain?.DefenseBonus ?? 0f,
                },
                Stockpile = new ResourceStockpile
                {
                    Tael = _config.StartingTael,
                    Qi = _config.StartingQi,
                },
                DissentLevel = 0,
            };

            // Add starting peons
            for (int i = 0; i < _config.StartingPeons; i++)
            {
                var peon = new DiscipleData
                {
                    Name = DiscipleData.GenerateName(),
                    Rank = DiscipleRank.Peon,
                    IsAlive = true,
                    Techniques = System.Array.Empty<string>(),
                    Trait = "",
                    BondedBeast = ""
                };
                peon.CalculateStats(_config);
                _createdData.AddDisciple(peon);
            }

            // Mark tile as sect territory
            tile.Control = ControlState.SectTerritory;

            // Place Temple building on the founding tile
            PlaceTempleBuilding(tile);

            _executed = true;
        }

        private void PlaceTempleBuilding(HexTileData tile)
        {
            var templeConfig = FindTempleConfig();
            if (templeConfig == null)
            {
                Debug.LogWarning("[FoundSectCommand] Temple config not found. Skipping building placement.");
                return;
            }

            // Get the tile's world position
            var worldPos = tile.Coords.ToWorldPosition(1.0f);

            // Create the Temple building GameObject
            string objectName = $"Building_Temple_T1";
            var go = new GameObject(objectName);

            // Add MeshFilter + MeshRenderer
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            // Assign mesh from config
            var mesh = templeConfig.GetTierMesh(1);
            if (mesh != null)
            {
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                Debug.LogWarning("[FoundSectCommand] No mesh for Temple T1. Using placeholder.");
                meshFilter.sharedMesh = CreatePlaceholderCube();
            }

            // Assign material
            var woodMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/_Game/Art/Materials/M_BuildingWood.mat");
            if (woodMat != null)
            {
                meshRenderer.sharedMaterial = woodMat;
            }

            // Add collider
            if (templeConfig.RequiresCollider && mesh != null)
            {
                var collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                collider.convex = true;
            }

            // Raycast down from above the tile to find the exact surface position
            float surfaceY = worldPos.y;
            if (Physics.Raycast(worldPos + Vector3.up * 10f, Vector3.down, out var hit, 20f, ~0))
            {
                surfaceY = hit.point.y;
            }

            // Place the building so its base sits on the tile surface
            // The mesh pivot is typically at the center, so we offset by half the mesh height
            float yOffset = 0.01f;
            if (mesh != null)
            {
                yOffset = mesh.bounds.extents.y + 0.01f;
            }

            go.transform.position = new Vector3(worldPos.x, surfaceY + yOffset, worldPos.z);

            // Register the building in SectData
            _createdData.AddBuilding("Temple", 1, go.transform.position);

            Debug.Log($"[FoundSectCommand] Placed Temple at ({_tileQ}, {_tileR})");
        }

        private BuildingConfigSO FindTempleConfig()
        {
            var guids = AssetDatabase.FindAssets("t:BuildingConfigSO Temple");
            if (guids.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            // Fallback: search by name
            var allGuids = AssetDatabase.FindAssets("t:BuildingConfigSO");
            foreach (var guid in allGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var config = AssetDatabase.LoadAssetAtPath<BuildingConfigSO>(path);
                if (config != null && config.BuildingTypeId == "Temple")
                    return config;
            }

            return null;
        }

        private static Mesh CreatePlaceholderCube()
        {
            var mesh = new Mesh { name = "Placeholder_Cube" };
            Vector3[] vertices = {
                new(-0.5f, 0f, -0.5f), new(0.5f, 0f, -0.5f), new(0.5f, 0f, 0.5f), new(-0.5f, 0f, 0.5f),
                new(-0.5f, 1f, -0.5f), new(0.5f, 1f, -0.5f), new(0.5f, 1f, 0.5f), new(-0.5f, 1f, 0.5f)
            };
            int[] triangles = {
                0,2,1, 0,3,2, 4,5,6, 4,6,7,
                0,1,5, 0,5,4, 2,3,7, 2,7,6,
                0,4,7, 0,7,3, 1,2,6, 1,6,5
            };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        public override void Undo()
        {
            if (!_executed || _createdData == null) return;

            var tile = _gridManager.GetTile(_tileQ, _tileR);
            if (tile != null)
            {
                tile.Control = ControlState.Unowned;
            }

            _createdData.IsFounded = false;
            _executed = false;
        }

        private static float GetQiDensityValue(QiDensityLevel density)
        {
            return density switch
            {
                QiDensityLevel.None => 0f,
                QiDensityLevel.Sparse => 5f,
                QiDensityLevel.Moderate => 10f,
                QiDensityLevel.Dense => 15f,
                QiDensityLevel.LeyLine => 25f,
                _ => 0f,
            };
        }
    }
}
