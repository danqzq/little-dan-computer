using UnityEngine;
using UnityEngine.UI;

namespace Danqzq
{
    [RequireComponent(typeof(Image))]
    public class ColorModifier : MonoBehaviour
    {
        [SerializeField] private bool _persistAlpha;
        
        private Image _image;
        private float _alpha;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _alpha = _image.color.a;
        }
        
        public void SetColor(string hexColor)
        {
            var color = ColorUtility.TryParseHtmlString(hexColor, out var newColor) ? newColor : Color.white;
            if (_persistAlpha)
                color.a = _alpha;
            _image.color = color;
        }
    }
}