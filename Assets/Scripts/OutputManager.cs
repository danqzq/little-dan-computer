using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Danqzq
{
    public class OutputManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _outputText;
        [SerializeField] private Scrollbar _scrollbar;

        [SerializeField] private GameObject _outputPlaceholder;
        
        public string GetOutputText() => _outputText.text;
        
        public void Output(short value)
        {
            _outputText.text += value;
            _scrollbar.value = 0;
            _outputPlaceholder.SetActive(false);
        }
        
        public void OutputChar(short value)
        {
            _outputText.text += (char)value;
            _scrollbar.value = 0;
            _outputPlaceholder.SetActive(false);
        }

        public void Clear()
        {
            _outputText.text = string.Empty;
            _outputPlaceholder.SetActive(true);
        }
    }
}