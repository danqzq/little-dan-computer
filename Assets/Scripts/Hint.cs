using TMPro;
using UnityEngine;

namespace Danqzq
{
    public class Hint : MonoBehaviour
    {
        private static Transform _hintTransform;
        private static TMP_Text _hintText;
        
        private static Vector3 _offset = new Vector3(1.5f, -1f, 10f);
        
        private Camera _camera;
        
        private void Awake()
        {
            _hintTransform = transform.GetChild(0);
            _hintText = GetComponentInChildren<TMP_Text>(true);
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        public static void Show(string message)
        {
            _hintTransform.gameObject.SetActive(true);
            _hintText.text = message;
        }
        
        public static void Hide()
        {
            _hintTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_hintTransform.gameObject.activeSelf)
            {
                _hintTransform.position = _camera.ScreenToWorldPoint(Input.mousePosition) + _offset;
            }
        }
    }
}