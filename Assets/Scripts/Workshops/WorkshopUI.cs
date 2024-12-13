using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Danqzq.Workshops
{
    public class WorkshopUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _workshopNameText;

        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _progressText;
        
        private System.Action _onClick;
        
        public void SetWorkshop(Workshop workshop, byte progress, System.Action onClick)
        {
            _onClick = onClick;
            
            _workshopNameText.text = workshop.Name;
            
            _progressBar.value = progress / 100f;
            _progressText.text = $"{progress}%";
        }
        
        public void OnClick()
        {
            _onClick?.Invoke();
        }
    }
}