using UnityEngine;

namespace Danqzq.UI
{
    public class MousePointer : MonoBehaviour
    {
        [SerializeField] private Vector2 _minMaxSize = new Vector2(0.25f, 1f);
        
        private Transform _mouseTransform;
        
        private float _mouseMoveSpeed;
        private float _mouseScale;
        private Vector2 _lastMousePosition;

        private float _mouseMoveTimer;

        private const float MOUSE_MOVE_DISTANCE_THRESHOLD = 200f;
        private const float MOUSE_MOVE_SPEED_THRESHOLD = 20f;
        private const float MOUSE_MOVE_TIME_THRESHOLD = 1.25f;
        
        private const float MOUSE_SIZE_INCREASE_SPEED = 7.5f;
        private const float MOUSE_SIZE_DECREASE_SPEED = 15f;
        private const float MOUSE_MOVE_TIMER_DECREASE_SPEED = 10f;
        
        private void Start()
        {
            Cursor.visible = false;
            
            _mouseTransform = transform.GetChild(0);
            _mouseScale = _mouseTransform.localScale.x;
        }

        private void UpdateMousePosition()
        {
            var mousePosition = Input.mousePosition;
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

            UpdateMouseScale();
            
            _lastMousePosition = Input.mousePosition;
        }
        
        private void UpdateMouseScale()
        {
            var delta = Vector2.Distance(_lastMousePosition, Input.mousePosition);
            _mouseMoveSpeed = delta < MOUSE_MOVE_DISTANCE_THRESHOLD ? delta / Time.deltaTime : 0;

            _mouseTransform.localScale = Vector3.one * Mathf.Clamp(_mouseScale, _minMaxSize.x, _minMaxSize.y);
            
            if (_mouseMoveSpeed > MOUSE_MOVE_SPEED_THRESHOLD)
            {
                _mouseMoveTimer += Time.deltaTime;
            }
            else
            {
                _mouseMoveTimer = Mathf.Lerp(_mouseMoveTimer, 0, Time.deltaTime * MOUSE_MOVE_TIMER_DECREASE_SPEED);
            }
            
            if (_mouseMoveTimer > MOUSE_MOVE_TIME_THRESHOLD)
            {
                _mouseScale = Mathf.Clamp(_mouseScale + MOUSE_SIZE_INCREASE_SPEED * Time.deltaTime, _minMaxSize.x, _minMaxSize.y);
            }
            
            _mouseScale = Mathf.Lerp(_mouseScale, _minMaxSize.x, Time.deltaTime * MOUSE_SIZE_DECREASE_SPEED);
        }
        
        private void Update()
        {
            UpdateMousePosition();
        }
    }
}