using UnityEngine;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Isometric camera controller for the hex grid.
    /// Supports rotation (Q/E), scroll zoom, and right-click drag pan.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class HexCameraController : MonoBehaviour
    {
        [Header("Zoom")]
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private float _minZoom = 3f;
        [SerializeField] private float _maxZoom = 20f;

        [Header("Rotation")]
        [SerializeField] private float _rotationSpeed = 60f;

        [Header("Pan")]
        [SerializeField] private float _panSpeed = 1f;

        private Camera _cam;
        private float _currentZoom;
        private float _currentRotation;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _currentZoom = _cam.orthographicSize;
            _currentRotation = transform.eulerAngles.y;
        }

        private void Update()
        {
            HandleZoom();
            HandleRotation();
            HandlePan();
        }

        private void HandleZoom()
        {
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            _currentZoom -= scroll * _zoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
            _cam.orthographicSize = _currentZoom;
        }

        private void HandleRotation()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                _currentRotation -= _rotationSpeed * Time.deltaTime;
                ApplyRotation();
            }
            if (Input.GetKey(KeyCode.E))
            {
                _currentRotation += _rotationSpeed * Time.deltaTime;
                ApplyRotation();
            }
        }

        private void ApplyRotation()
        {
            var euler = transform.eulerAngles;
            euler.y = _currentRotation;
            transform.eulerAngles = euler;
        }

        private void HandlePan()
        {
            if (!Input.GetMouseButton(2)) return; // Middle mouse

            float h = Input.GetAxis("Mouse X") * _panSpeed * (_currentZoom / 10f);
            float v = Input.GetAxis("Mouse Y") * _panSpeed * (_currentZoom / 10f);

            // Pan relative to camera orientation
            Vector3 pan = transform.right * -h + transform.forward * -v;
            pan.y = 0; // Keep on XZ plane
            transform.position += pan;
        }
    }
}
