using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Danqzq
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        
        private bool _isInputSubmitted;
        private Outline _outline;

        private float _blinkTime;
        
        private Queue<string> _inputQueue = new Queue<string>();
        
        public short Value { get; private set; }

        private void Awake()
        {
            _outline = _inputField.GetComponent<Outline>();
            _inputQueue = new Queue<string>();
        }

        public void SetInput(IEnumerable<string> input)
        {
            foreach (var i in input)
            {
                _inputQueue.Enqueue(i);
            }
        }

        public IEnumerator Read()
        {
            bool IsInputSubmitted()
            {
                _blinkTime += Time.deltaTime;
                if (_blinkTime >= 0.5f)
                {
                    _outline.enabled = !_outline.enabled;
                    _blinkTime = 0;
                }

                return _isInputSubmitted;
            }

            if (_inputQueue.Count == 0)
            {
                yield return new WaitUntil(IsInputSubmitted);
            }
            else
            {
                _inputField.text = _inputQueue.Dequeue();
            }
            
            _outline.enabled = false;
            _isInputSubmitted = false;
            if (short.TryParse(_inputField.text, out var val))
            {
                if (val is < -999 or > 999)
                {
                    Logger.Send("Input out of bounds! [-999; 999]", Logger.MsgType.Error);
                    yield return Read();
                }
                Value = val;
                _inputField.text = string.Empty;
            }
            else
            {
                Logger.Send("Invalid input!", Logger.MsgType.Error);
                yield return Read();
            }
        }
        
        public void Submit()
        {
            if (_inputQueue.Count > 0)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(_inputField.text))
            {
                return;
            }
            _isInputSubmitted = true;
        }
        
        public void Clear()
        {
            _isInputSubmitted = false;
            _inputField.text = string.Empty;
            _outline.enabled = false;
            
            _inputQueue.Clear();
        }
    }
}