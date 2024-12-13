using UnityEngine;

namespace Danqzq
{
    [RequireComponent(typeof(Collider2D))]
    public class Hintable : MonoBehaviour
    {
        [SerializeField] [TextArea(3, 5)] private string _hintMessage;

        private bool _isHovering, _isShowing;
        private float _hoverTime;
        
        private const float HOVER_TIME_THRESHOLD = 0.5f;
        
        private void OnMouseEnter()
        {
            _hoverTime = 0;
            _isHovering = true;
        }

        private void OnMouseExit()
        {
            _isHovering = false;
            _isShowing = false;
            Hint.Hide();
        }

        private void Update()
        {
            if (_isShowing || !_isHovering) 
                return;
            
            _hoverTime += Time.deltaTime;
            if (_hoverTime >= HOVER_TIME_THRESHOLD)
            {
                Hint.Show(_hintMessage);
                _isShowing = true;
            }
        }
    }
}