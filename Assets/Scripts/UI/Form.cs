using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Danqzq
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Form : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private TMP_InputField[] _inputs;
        
        private Dictionary<string, TMP_InputField> _nameToInput;

        public System.Action OnSubmit { get; set; }
        
        public string this[string n]
        {
            get => GetValue(n);
            set => SetValue(n, value);
        }
        
        private string GetValue(string n)
        {
            return _nameToInput.TryGetValue(n, out var input) ? input.text : "";
        }

        private void SetValue(string n, string v)
        {
            if (_nameToInput.TryGetValue(n, out var input))
            {
                input.text = v;
            }
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _inputs = GetComponentsInChildren<TMP_InputField>();
            
            _nameToInput = new Dictionary<string, TMP_InputField>();
            foreach (var input in _inputs)
            {
                _nameToInput.Add(input.name, input);
            }
        }
        
        public void ToggleVisibility(bool visible)
        {
            _canvasGroup.Toggle(visible);
        }

        public void Submit()
        {
            OnSubmit?.Invoke();
        }
    }
}