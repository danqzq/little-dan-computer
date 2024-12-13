using TMPro;
using UnityEngine;

namespace Danqzq
{
    public sealed class Chunk : ValueDisplay
    {
        [SerializeField] private TMP_Text _addressText;

        [SerializeField] private GameObject _canvas;
        [SerializeField] private TMP_InputField _valueInputField;
        
        private float _lastClickTime;
        private bool _isSelected;

        public short Address
        {
            get => _address;
            private set
            {
                _address = value;
                _addressText.text = _address.Display();
            }
        }
        
        private short _address;

        public void Init(byte address, byte value)
        {
            Address = address;
            Value = value;
        }

        public override void UpdateUI()
        {
            base.UpdateUI();
            _addressText.text = _address.Display();
        }

        public void OnMouseDown()
        {
            if (_lastClickTime + 0.5f > Time.time)
            {
                _canvas.SetActive(true);
                _valueInputField.text = Value.Display(NumericFormat.Decimal);
            }
            _lastClickTime = Time.time;
        }

        private void LateUpdate()
        {
            if (_canvas.activeSelf && !_isSelected)
            {
                _valueInputField.Select();
                _isSelected = true;
            }
        }

        public void Deselect()
        {
            if (string.IsNullOrEmpty(_valueInputField.text))
            {
                _valueInputField.text = Value.Display(NumericFormat.Decimal);
            }

            if (short.TryParse(_valueInputField.text, out var val))
            {
                Value = val;
            }
            else
            {
                Logger.Send("Invalid input!", Logger.MsgType.Error);
            }

            _valueInputField.text = string.Empty;
            _canvas.SetActive(false);
            _isSelected = false;
        }
    }
}
