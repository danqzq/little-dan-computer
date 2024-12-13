using Danqzq.Models;
using UnityEngine;

namespace Danqzq
{
    public abstract class DeviceComponent : MonoBehaviour
    {
        private Vector3? _dragPoint;
        private Vector3 _offset;
        
        private Vector3 _initialPosition;
        
        private Camera _camera;
        
        public virtual void Import(DeviceComponentData data)
        {
            transform.position = data.position;
        }
        
        public virtual DeviceComponentData Export()
        {
            return new DeviceComponentData(transform.position);
        }

        protected virtual void Start()
        {
            _camera = Camera.main;
        }

        protected virtual void OnMouseDown()
        {
            _initialPosition = transform.position;
            _dragPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            _offset = transform.position - _dragPoint.Value;
        }

        protected virtual void OnMouseUp()
        {
            _dragPoint = null;
        }
        
        protected virtual void Update()
        {
            if (!_dragPoint.HasValue)
            {
                return;
            }

            var currentPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            var newPosition = currentPoint + _offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, _initialPosition.z);
        }
    }
}