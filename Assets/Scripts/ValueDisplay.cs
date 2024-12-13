using System.Collections;
using TMPro;
using UnityEngine;

namespace Danqzq
{
    public abstract class ValueDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private GameObject _highlight;
        
        public static bool IsHighlightsEnabled { get; set; }

        public static float HighlightDelay { get; set; }
        
        private short _value;
        
        public short Value
        {
            get
            {
                Highlight();
                return _value;
            }
            set
            {
                _value = value;
                UpdateUI();
                Highlight();
            }
        }

        public virtual void UpdateUI()
        {
            _valueText.text = _value.Display();
        }

        private void Highlight()
        {
            IEnumerator HighlightCoroutine()
            {
                _highlight.SetActive(true);
                yield return new WaitForSeconds(HighlightDelay);
                _highlight.SetActive(false);
            }
            
            if (IsHighlightsEnabled)
            {
                StopAllCoroutines();
                StartCoroutine(HighlightCoroutine());
            }
        }
    }
}