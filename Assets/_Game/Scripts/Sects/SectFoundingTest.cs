using UnityEngine;
using UnityEngine.InputSystem;
using TalesOfTao.Core;
using TalesOfTao.Core.TurnSystem;
using TalesOfTao.Hex;

namespace TalesOfTao.Sects
{
    /// <summary>
    /// Test harness for sect founding. Self-bootstrapping.
    /// Press T on a valid tile during Action phase to found a sect.
    /// </summary>
    public class SectFoundingTest : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private SectConfigSO _sectConfig;

        [Header("State")]
        [SerializeField] private bool _founded;
        [SerializeField] private string _statusMessage = "Wait for Action phase, then press T on a tile to found sect";

        private TurnDriver _turnDriver;
        private SectManager _sectManager;
        private HexGridManager _gridManager;
        private Camera _camera;

        private void Start()
        {
            _gridManager = HexGridManager.Instance;
            _camera = Camera.main;

            // Find existing turn driver (created by TurnTestHUD or scene)
            _turnDriver = FindAnyObjectByType<TurnDriver>();
            if (_turnDriver == null)
            {
                // Create minimal turn system
                var calGO = new GameObject("ZodiacCalendar");
                var calendar = calGO.AddComponent<ZodiacCalendar>();
                var driverGO = new GameObject("TurnDriver");
                _turnDriver = driverGO.AddComponent<TurnDriver>();
                _turnDriver.Initialize(calendar, null, null, null, 0f);
                _turnDriver.StartTurn();
            }
            // else: existing driver is already running, just use it

            // Create SectManager
            var mgrGO = new GameObject("SectManager");
            _sectManager = mgrGO.AddComponent<SectManager>();
        }

        private void Update()
        {
            if (_founded) return;

            // Only allow founding during Action phase
            if (_turnDriver != null && _turnDriver.CurrentPhase != GamePhase.Action)
                return;

            // Check for T key (T for Temple/found)
            bool tPressed = false;
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.tKey.wasPressedThisFrame)
                    tPressed = true;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.T))
                    tPressed = true;
            }

            if (!tPressed) return;

            // Raycast to find tile under mouse
            var mouse = Mouse.current;
            Vector3 mousePos = mouse != null ? mouse.position.ReadValue() : (Vector3)Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, ~0))
            {
                var hexCoords = WorldToHex(hit.point, 1f);
                var tile = _gridManager.GetTile(hexCoords.Q, hexCoords.R);

                if (tile != null && !tile.IsImpassable && tile.Terrain?.Type != TerrainType.Lake)
                {
                    FoundSect(hexCoords.Q, hexCoords.R);
                }
                else
                {
                    _statusMessage = "Invalid tile: impassable or water";
                }
            }
        }

        private void FoundSect(int q, int r)
        {
            if (_sectConfig == null)
            {
                _statusMessage = "ERROR: No SectConfigSO assigned!";
                Debug.LogError("[SectFoundingTest] No SectConfigSO assigned!");
                return;
            }

            var command = new FoundSectCommand(_sectConfig, q, r, _gridManager);
            if (command.CanExecute())
            {
                command.Execute();
                _sectManager.SetSectData(command.CreatedData);
                _founded = true;
                _statusMessage = $"Sect '{_sectConfig.DisplayName}' founded at ({q},{r})!";
                Debug.Log($"[SectFoundingTest] {_statusMessage}");
            }
            else
            {
                _statusMessage = "Cannot found sect on this tile";
            }
        }

        private void OnGUI()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                normal = { textColor = Color.white }
            };

            float y = Screen.height - 30;

            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(new Rect(10, y - 5, 500, 30), Texture2D.whiteTexture);
            GUI.color = Color.white;

            GUI.Label(new Rect(20, y, 480, 25), _statusMessage, style);

            if (_founded && _sectManager.HasFoundedSect)
            {
                var data = _sectManager.Data;
                y -= 28;
                GUI.color = new Color(0, 0, 0, 0.7f);
                GUI.DrawTexture(new Rect(10, y - 5, 500, 30), Texture2D.whiteTexture);
                GUI.color = Color.white;
                GUI.Label(new Rect(20, y, 480, 25),
                    $"Tael: {data.Stockpile.Tael} | Qi: {data.Stockpile.Qi} | Peons: {data.PeonCount} | Dissent: {data.DissentLevel}", style);
            }
        }

        private static HexCoords WorldToHex(Vector3 worldPos, float size)
        {
            float sqrt3 = 1.732051f;
            float q = (2f / 3f * worldPos.x) / size;
            float r = (-1f / 3f * worldPos.x + sqrt3 / 3f * worldPos.z) / size;
            float s = -q - r;

            int rq = Mathf.RoundToInt(q);
            int rr = Mathf.RoundToInt(r);
            int rs = Mathf.RoundToInt(s);

            float dq = Mathf.Abs(rq - q);
            float dr = Mathf.Abs(rr - r);
            float ds = Mathf.Abs(rs - s);

            if (dq > dr && dq > ds)
                rq = -rr - rs;
            else if (dr > ds)
                rr = -rq - rs;

            return new HexCoords(rq, rr);
        }
    }
}
