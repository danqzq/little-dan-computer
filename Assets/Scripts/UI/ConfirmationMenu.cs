using UnityEngine;

namespace Danqzq
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ConfirmationMenu : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _messageText;
        [SerializeField] private TMPro.TMP_Text _confirmButtonText;
        
        private System.Action _onConfirm, _onCancel;
        
        private CanvasGroup _canvasGroup;
        
        private const string DEFAULT_CONFIRM_MESSAGE = "Confirm";

        public bool IsInteractable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }
        
        public void SetText(string message) => _messageText.text = message;

        public void Show(string message, System.Action onConfirm, System.Action onCancel = null)
        {
            Show(message, DEFAULT_CONFIRM_MESSAGE, onConfirm, onCancel);
        }
        
        public void Show(string message, string confirmMessage, System.Action onConfirm, System.Action onCancel = null)
        {
            SetText(message);
            _confirmButtonText.text = confirmMessage;
            _onConfirm = onConfirm;
            _onCancel = onCancel;
            _canvasGroup.Toggle(true);
        }
        
        public void OnConfirm()
        {
            _onConfirm?.Invoke();
            _canvasGroup.Toggle(false);
        }

        public void OnCancel()
        {
            _onCancel?.Invoke();
            _canvasGroup.Toggle(false);
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}