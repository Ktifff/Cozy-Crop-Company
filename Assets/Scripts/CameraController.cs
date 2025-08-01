using UnityEngine;
using System;

namespace Game.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public static event Action OnClick;

        [Header("Movement Settings")]
        [SerializeField] private float _clickThreshold = 10f;
        [SerializeField] private float _dragSensitivity = 0.2f;
        [SerializeField] private Vector2 _minPosition = new Vector2(-50, -50);
        [SerializeField] private Vector2 _maxPosition = new Vector2(50, 50);

        private Camera _camera;
        private Vector3 _lastPointerPosition;
        private bool _isDragging = false;
        private bool _isClickCandidate = false;
        private Vector3 _clickStartPosition;

        private void Awake()
        {
            _camera = Camera.main;
            Application.targetFrameRate = 120;
        }

        private void Update()
        {
            HandleMouseDrag();
        }

        private void HandleMouseDrag()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lastPointerPosition = Input.mousePosition;
                _clickStartPosition = _lastPointerPosition;
                _isDragging = true;
                _isClickCandidate = true;
            }
            else if (Input.GetMouseButton(0) && _isDragging)
            {
                Vector3 delta = Input.mousePosition - _lastPointerPosition;
                PanCamera(delta);
                _lastPointerPosition = Input.mousePosition;
                if (Vector3.Distance(_clickStartPosition, Input.mousePosition) > _clickThreshold)
                {
                    _isClickCandidate = false;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                if (_isClickCandidate)
                {
                    OnClick?.Invoke();
                }
                _isClickCandidate = false;
            }
        }

        private void PanCamera(Vector3 screenDelta)
        {
            Vector3 right = _camera.transform.right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
            Vector3 movement = (-right * screenDelta.x - forward * screenDelta.y) * _dragSensitivity * Time.deltaTime;
            Vector3 newPos = transform.position + movement;
            newPos.x = Mathf.Clamp(newPos.x, _minPosition.x, _maxPosition.x);
            newPos.z = Mathf.Clamp(newPos.z, _minPosition.y, _maxPosition.y);
            transform.position = newPos;
        }
    }
}