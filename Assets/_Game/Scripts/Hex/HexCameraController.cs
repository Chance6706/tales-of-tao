using UnityEngine;
using UnityEngine.InputSystem;

namespace TalesOfTao.Hex
{
    /// <summary>
    /// Strategy-game style camera controller for the hex grid.
    /// Auto-detects new vs old Input System based on Player Settings.
    ///
    /// Controls (Civ-like):
    ///   Scroll Wheel          - Zoom in/out
    ///   Middle Mouse Drag     - Pan
    ///   Right Mouse Drag      - Orbit rotate around pivot
    ///   Q / E                 - Rotate left/right
    ///   R / F                 - Tilt up/down
    ///   WASD / Arrow Keys     - Pan
    ///   Page Up / Down        - Tilt
    ///   Home                  - Reset to default position
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
        [SerializeField] private float _keyboardPanSpeed = 15f;

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
        private float _currentRotation;
        private float _currentTilt;
        private bool _useNewInput;
        private Mouse _mouse;
        private Keyboard _keyboard;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            _currentZoom = _cam.orthographicSize;
            _currentRotation = 45f;
            _currentTilt = 45f;
            _useNewInput = Mouse.current != null;
            if (_useNewInput)
            {
                _mouse = Mouse.current;
                _keyboard = Keyboard.current;
            }
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
            float scroll = 0f;
            if (_useNewInput)
                scroll = _mouse != null ? _mouse.scroll.ReadValue().y : 0f;
            else
                scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) < 0.001f) return;

            _currentZoom -= scroll * _zoomSpeed;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
            _cam.orthographicSize = _currentZoom;
            ApplyTransform();
        }

        private void HandlePan()
        {
            float panX = 0f, panZ = 0f;

            if (_useNewInput)
            {
                if (_keyboard != null)
                {
                    if (_keyboard.leftArrowKey.isPressed || _keyboard.aKey.isPressed) panX -= 1f;
                    if (_keyboard.rightArrowKey.isPressed || _keyboard.dKey.isPressed) panX += 1f;
                    if (_keyboard.upArrowKey.isPressed || _keyboard.wKey.isPressed) panZ += 1f;
                    if (_keyboard.downArrowKey.isPressed || _keyboard.sKey.isPressed) panZ -= 1f;
                }

                if (_mouse != null && _mouse.middleButton.isPressed)
                {
                    var delta = _mouse.delta.ReadValue();
                    // Match keyboard pan speed (panSpeed * zoomScale * dt equivalent)
                    float panSpeed = _panSpeed * (_currentZoom / 5f) * Time.deltaTime * 60f;
                    panX += delta.x * panSpeed;
                    panZ -= delta.y * panSpeed;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) panX -= 1f;
                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) panX += 1f;
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) panZ += 1f;
                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) panZ -= 1f;

                if (Input.GetMouseButton(2))
                {
                    float zoomScale = _currentZoom / 10f;
                    panX -= Input.GetAxis("Mouse X") * _panSpeed * zoomScale;
                    panZ -= Input.GetAxis("Mouse Y") * _panSpeed * zoomScale;
                }
            }

            if (Mathf.Abs(panX) < 0.001f && Mathf.Abs(panZ) < 0.001f) return;

            float rad = _currentRotation * Mathf.Deg2Rad;
            // Transform local pan direction to world space rotation only (no pitch)
            // Negate Z so W moves north (toward -Z on the map)
            Vector3 worldPan = new Vector3(-panX, 0f, -panZ);
            worldPan = Quaternion.Euler(0f, _currentRotation, 0f) * worldPan;

            bool keyboardInput = Mathf.Abs(panX) > 0.001f || Mathf.Abs(panZ) > 0.001f;
            float speed = keyboardInput ? _keyboardPanSpeed : 1f;
            _pivot += worldPan * speed * Time.deltaTime;
            ApplyTransform();
        }

        private void HandleRotation()
        {
            float rotation = 0f;

            if (_useNewInput)
            {
                if (_keyboard != null)
                {
                    if (_keyboard.qKey.isPressed) rotation += 1f;
                    if (_keyboard.eKey.isPressed) rotation -= 1f;
                }
                if (_mouse != null && _mouse.rightButton.isPressed)
                {
                    rotation += _mouse.delta.ReadValue().x * _mouseRotationSpeed;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Q)) rotation += 1f;
                if (Input.GetKey(KeyCode.E)) rotation -= 1f;
                if (Input.GetMouseButton(1))
                {
                    rotation += Input.GetAxis("Mouse X") * _mouseRotationSpeed;
                }
            }

            if (Mathf.Abs(rotation) < 0.01f) return;

            _currentRotation += rotation * _rotationSpeed * Time.deltaTime;
            _currentRotation = NormalizeAngle(_currentRotation);
            ApplyTransform();
        }

        private void HandleTilt()
        {
            float tilt = 0f;

            if (_useNewInput)
            {
                if (_keyboard != null)
                {
                    if (_keyboard.rKey.isPressed || _keyboard.pageUpKey.isPressed) tilt += 1f;
                    if (_keyboard.fKey.isPressed || _keyboard.pageDownKey.isPressed) tilt -= 1f;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp)) tilt += 1f;
                if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown)) tilt -= 1f;
            }

            if (Mathf.Abs(tilt) < 0.01f) return;

            _currentTilt += tilt * _tiltSpeed * Time.deltaTime;
            _currentTilt = Mathf.Clamp(_currentTilt, _minTilt, _maxTilt);
            ApplyTransform();
        }

        private void HandleReset()
        {
            bool homePressed;
            if (_useNewInput)
                homePressed = _keyboard != null && _keyboard.homeKey.wasPressedThisFrame;
            else
                homePressed = Input.GetKeyDown(KeyCode.Home);

            if (homePressed)
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
            float rotRad = _currentRotation * Mathf.Deg2Rad;
            float tiltRad = _currentTilt * Mathf.Deg2Rad;
            float distance = _currentZoom * 1.5f;

            float x = distance * Mathf.Cos(tiltRad) * Mathf.Sin(rotRad);
            float y = distance * Mathf.Sin(tiltRad);
            float z = distance * Mathf.Cos(tiltRad) * Mathf.Cos(rotRad);

            transform.position = _pivot + new Vector3(x, y, z);
            transform.LookAt(_pivot);
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle < 0f) angle += 360f;
            while (angle >= 360f) angle -= 360f;
            return angle;
        }
    }
}
