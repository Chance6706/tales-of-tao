using UnityEngine;
using UnityEngine.InputSystem;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Strategy-game style camera controller for the hex grid.
    ///
    /// Controls (Civ-like):
    ///   Scroll Wheel          — Zoom in/out
    ///   Middle Mouse Drag     — Pan
    ///   Right Mouse Drag      — Orbit rotate around pivot
    ///   Q / E                 — Rotate left/right
    ///   R / F                 — Tilt up/down
    ///   Arrow Keys            — Pan
    ///   Page Up / Down        — Tilt
    ///   Home                  — Reset to default position
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class HexCameraController : MonoBehaviour
    {
        [Header("Zoom")]
        [SerializeField] private float _zoomSpeed = 8f;
        [SerializeField] private float _minZoom = 3f;
        [SerializeField] private float _maxZoom = 30f;

        [Header("Pan")]
        [SerializeField] private float _panSpeed = 1.5f;
        [SerializeField] private float _keyboardPanSpeed = 10f;

        [Header("Rotation")]
        [SerializeField] private float _rotationSpeed = 45f;
        [SerializeField] private float _mouseRotationSpeed = 2f;

        [Header("Tilt")]
        [SerializeField] private float _minTilt = 20f;
        [SerializeField] private float _maxTilt = 80f;
        [SerializeField] private float _tiltSpeed = 30f;

        [Header("Pivot")]
        [SerializeField] private Vector3 _pivot = Vector3.zero;

        private Camera _cam;
        private float _currentZoom;
        private float _currentRotation; // Y-axis rotation (horizontal orbit)
        private float _currentTilt;     // X-axis rotation (vertical tilt)
        private Vector3 _lastMousePos;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _currentZoom = _cam.orthographicSize;
            _currentRotation = 45f;
            _currentTilt = 45f;
            ApplyTransform();
        }

        private void Update()
        {
            HandleZoom();
            HandlePan();
            HandleRotation();
            HandleTilt();
            HandleReset();
        }

        private void HandleZoom()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            float scroll = mouse.scrollDelta.ReadValue().y;
            if (Mathf.Abs(scroll) < 0.01f) return;

            _currentZoom -= scroll * _zoomSpeed * 0.01f;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
            _cam.orthographicSize = _currentZoom;
            ApplyTransform();
        }

        private void HandlePan()
        {
            Vector3 pan = Vector3.zero;

            // Keyboard panning — WASD + Arrow keys
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) pan.x -= 1;
                if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) pan.x += 1;
                if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) pan.z += 1;
                if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) pan.z -= 1;
            }

            // Middle mouse drag panning
            var mouse = Mouse.current;
            if (mouse != null && mouse.middleButton.isPressed)
            {
                var delta = mouse.delta.ReadValue();
                pan.x -= delta.x * _panSpeed * 0.01f * (_currentZoom / 10f);
                pan.z -= delta.y * _panSpeed * 0.01f * (_currentZoom / 10f);
            }

            if (pan.sqrMagnitude < 0.001f) return;

            // Transform pan direction by camera rotation (Y-axis only)
            float rad = _currentRotation * Mathf.Deg2Rad;
            Vector3 worldPan = new Vector3(
                pan.x * Mathf.Cos(rad) - pan.z * Mathf.Sin(rad),
                0,
                pan.x * Mathf.Sin(rad) + pan.z * Mathf.Cos(rad)
            );

            float speed = keyboard != null && keyboard.anyKey.isPressed ? _keyboardPanSpeed : 1f;
            _pivot += worldPan * speed * Time.deltaTime;
            ApplyTransform();
        }

        private void HandleRotation()
        {
            float rotation = 0;

            // Q/E keys
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.qKey.isPressed) rotation += 1;
                if (keyboard.eKey.isPressed) rotation -= 1;
            }

            // Right mouse drag rotation
            var mouse = Mouse.current;
            if (mouse != null && mouse.rightButton.isPressed)
            {
                var delta = mouse.delta.ReadValue();
                rotation += delta.x * _mouseRotationSpeed;
            }

            if (Mathf.Abs(rotation) < 0.01f) return;

            _currentRotation += rotation * _rotationSpeed * Time.deltaTime;
            _currentRotation = NormalizeAngle(_currentRotation);
            ApplyTransform();
        }

        private void HandleTilt()
        {
            float tilt = 0;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.rKey.isPressed || keyboard.pageUpKey.isPressed) tilt += 1;
                if (keyboard.fKey.isPressed || keyboard.pageDownKey.isPressed) tilt -= 1;
            }

            if (Mathf.Abs(tilt) < 0.01f) return;

            _currentTilt += tilt * _tiltSpeed * Time.deltaTime;
            _currentTilt = Mathf.Clamp(_currentTilt, _minTilt, _maxTilt);
            ApplyTransform();
        }

        private void HandleReset()
        {
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.homeKey.wasPressedThisFrame)
            {
                _pivot = Vector3.zero;
                _currentZoom = 10f;
                _currentRotation = 45f;
                _currentTilt = 45f;
                _cam.orthographicSize = _currentZoom;
                ApplyTransform();
            }
        }

        private void ApplyTransform()
        {
            // Position camera at the correct distance/angle from the pivot
            float rotRad = _currentRotation * Mathf.Deg2Rad;
            float tiltRad = _currentTilt * Mathf.Deg2Rad;

            // Distance from pivot based on zoom
            float distance = _currentZoom * 2f;

            // Calculate offset from pivot using spherical coordinates
            float x = distance * Mathf.Cos(tiltRad) * Mathf.Sin(rotRad);
            float y = distance * Mathf.Sin(tiltRad);
            float z = distance * Mathf.Cos(tiltRad) * Mathf.Cos(rotRad);

            transform.position = _pivot + new Vector3(x, y, z);
            transform.LookAt(_pivot);
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle < 0) angle += 360;
            while (angle >= 360) angle -= 360;
            return angle;
        }
    }
}
