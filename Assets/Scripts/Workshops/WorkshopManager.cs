using UnityEngine;
using UnityEngine.SceneManagement;
using static Danqzq.Globals;

namespace Danqzq.Workshops
{
    public class WorkshopManager : MonoBehaviour
    {
        [SerializeField] private Workshop[] _workshops;

        [SerializeField] private Transform _workshopContainer;
        [SerializeField] private GameObject _workshopPrefab;
        
        private WorkshopProgress _workshopProgress;
        
        public static Workshop CurrentWorkshop { get; private set; }
        
        private void Start()
        {
            CurrentWorkshop = null;
            _workshopProgress = SaveManager.LoadObject<WorkshopProgress>(WORKSHOP_PROGRESS_SAVE_KEY);
            if (_workshopProgress.workshops == null)
            {
                _workshopProgress = new WorkshopProgress(_workshops.Length);
                SaveManager.SaveObject(WORKSHOP_PROGRESS_SAVE_KEY, _workshopProgress);
            }
            for (var i = 0; i < _workshops.Length; i++)
            {
                var workshop = _workshops[i];
                workshop.ID = i;
                
                var progress = _workshopProgress[i];
                var workshopObject = Instantiate(_workshopPrefab, _workshopContainer);
                workshopObject.GetComponent<WorkshopUI>().SetWorkshop(workshop, progress,
                    () => OnOpenWorkshop(workshop));
            }
        }
        
        private void OnOpenWorkshop(Workshop workshop)
        {
            CurrentWorkshop = workshop;
            SceneManager.LoadScene(SCENE_MAIN);
        }
        
        public static void UpdateProgress(int workshopIndex, byte progress, string code)
        {
            SaveManager.Save(WORKSHOP_CODE_SAVE_KEY + workshopIndex, code);
            
            var workshopProgress = SaveManager.LoadObject<WorkshopProgress>(WORKSHOP_PROGRESS_SAVE_KEY);
            if (progress <= workshopProgress[workshopIndex])
            {
                return;
            }
            
            workshopProgress[workshopIndex] = progress;
            SaveManager.SaveObject(WORKSHOP_PROGRESS_SAVE_KEY, workshopProgress);
        }
    }
}