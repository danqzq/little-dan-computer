using Danqzq.Workshops;
using TMPro;
using UnityEngine;

namespace Danqzq
{
    public class AppManager : MonoBehaviour
    {
        [Header("Value Displays")]
        [SerializeField] private TMP_Text _numericFormatText;
        [SerializeField] private TMP_InputField[] _inputFields;

        [Header("Camera Settings")]
        [SerializeField] private float _cameraMoveSpeed;
        [SerializeField] private float _cameraScrollSpeed;
        [SerializeField] private Vector2 _cameraZoomRange;
        [SerializeField] private Vector4 _cameraBounds;
        
        [Header("Workshops")]
        [SerializeField] private GameObject _workshopMenu;
        [SerializeField] private TMP_Text _workshopNameText;
        [SerializeField] private TMP_Text _workshopDescriptionText;
        
        public static NumericFormat numericFormat;
        
        public bool IsCameraMovementAllowed { get; set; } = true;
        
        private Camera _camera;
        private Transform _cameraTransform;
        private float _targetOrthographicSize;
        
        private ValueDisplay[] _valueDisplays;
        
        private void Awake()
        {
            numericFormat = NumericFormat.Decimal;
            _numericFormatText.text = numericFormat.Display();

            _valueDisplays = FindObjectsByType<ValueDisplay>(FindObjectsSortMode.None);
        }

        private void Start()
        {
            _camera = Camera.main;
            _cameraTransform = _camera!.transform;
            _targetOrthographicSize = _camera.orthographicSize;

            var workshop = WorkshopManager.CurrentWorkshop;
            if (workshop == null)
            {
                return;
            }
            
            _workshopMenu.SetActive(true);
            _workshopNameText.text = $"Workshop: {workshop.Name}";
            _workshopDescriptionText.text = workshop.Description;
        }

        public void ToggleByteFormat()
        {
            numericFormat += 1; 
            if ((byte) numericFormat > 2)
            {
                numericFormat = 0;
            }
            _numericFormatText.text = numericFormat.Display();
            UpdateUI();
        }
        
        public void UnselectAll()
        {
            foreach (var inputField in _inputFields)
            {
                inputField.DeactivateInputField();
            }
        }

        private void UpdateUI()
        {
            foreach (var display in _valueDisplays)
            {
                display.UpdateUI();
            }
        }

        private void HandleCameraMovement()
        {
            _cameraTransform.position += new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * 
                (_cameraMoveSpeed * Time.deltaTime);
            var orthographicSize = _camera.orthographicSize;
            _targetOrthographicSize = Mathf.Clamp(_targetOrthographicSize -
                Input.mouseScrollDelta.y * _cameraScrollSpeed, _cameraZoomRange.x, _cameraZoomRange.y);
            _camera.orthographicSize = Mathf.Lerp(orthographicSize, _targetOrthographicSize, Time.deltaTime * 10f);
        }

        private void Update()
        {
            if (IsCameraMovementAllowed)
            {
                HandleCameraMovement();
            }
        }
        
        private void LateUpdate()
        {
            var position = _cameraTransform.position;
            position = new Vector3(Mathf.Clamp(position.x, _cameraBounds.x, _cameraBounds.y),
                Mathf.Clamp(position.y, _cameraBounds.z, _cameraBounds.w), position.z);
            _cameraTransform.position = position;
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(_cameraBounds.x, _cameraBounds.z), new Vector3(_cameraBounds.x, _cameraBounds.w));
            Gizmos.DrawLine(new Vector3(_cameraBounds.x, _cameraBounds.z), new Vector3(_cameraBounds.y, _cameraBounds.z));
            Gizmos.DrawLine(new Vector3(_cameraBounds.x, _cameraBounds.w), new Vector3(_cameraBounds.y, _cameraBounds.w));
            Gizmos.DrawLine(new Vector3(_cameraBounds.y, _cameraBounds.z), new Vector3(_cameraBounds.y, _cameraBounds.w));
        }
#endif
    }
}
