using System.Collections;
using UnityEngine;

using static Danqzq.Globals;

namespace Danqzq.Networking
{
    public class AutoSaver : MonoBehaviour
    {
        [SerializeField] private Assembler _assembler;
        [SerializeField] private TMPro.TMP_Text _lastSavedAtText;

        [SerializeField] private GameObject _autoSaveCheckmark;
        
        private bool _autoSaveEnabled = true;
        
        private const float SAVE_INTERVAL = 15f;
        
        public void ToggleAutoSave()
        {
            var currentProject = ProjectManager.CurrentProject;
            if (currentProject == null || string.IsNullOrEmpty(currentProject.id))
            {
                return;
            }
            
            _autoSaveEnabled = !_autoSaveEnabled;
            SaveManager.SaveBool(PREFS_AUTO_SAVE_ENABLED, _autoSaveEnabled);
            _autoSaveCheckmark.SetActive(_autoSaveEnabled);
            if (_autoSaveEnabled)
            {
                StartCoroutine(SaveScheduler());
            }
            else
            {
                StopAllCoroutines();
            }
        }
        
        private IEnumerator SaveScheduler()
        {
            var time = 0f;
            yield return new WaitUntil(() =>
            {
                if (!_assembler.IsAssemblyEdited)
                {
                    time = 0;
                    return false;
                }
                time += Time.deltaTime;
                return time >= SAVE_INTERVAL;
            });
            OnSave();
            StartCoroutine(SaveScheduler());
        }
        
        private void OnSave()
        {
            if (!_assembler.IsAssemblyEdited)
            {
                Logger.Send("No changes to save!", Logger.MsgType.Warning);
                return;
            }
            
            if (ProjectManager.CurrentProject == null)
            {
                Logger.Send("You must login to save!", Logger.MsgType.Warning);
                return;
            }

            if (string.IsNullOrEmpty(ProjectManager.CurrentProject.id))
            {
                Logger.Send("You must first upload the project!", Logger.MsgType.Warning);
                return;
            }
            
            Logger.Send("Saving project...", Logger.MsgType.Warning);
            NetworkingManager.UpdateProject(ProjectManager.CurrentProject)
                .Then(OnProjectSaved);
        }

        private void OnProjectSaved(Models.Project project)
        {
            Logger.Send("Project saved!");
            ProjectManager.CurrentProject = project;
            _assembler.IsAssemblyEdited = false;
            DisplayLastSavedAt(project.updatedAt);
        }
        
        private void DisplayLastSavedAt(string updatedAt)
        {
            var date = System.DateTime.Parse(updatedAt).ToLocalTime();
            _lastSavedAtText.text = $"Last saved at: {date:dd/MM/yyyy HH:mm:ss}";
        }

        private void Start()
        {
            var currentProject = ProjectManager.CurrentProject;
            if (currentProject == null || string.IsNullOrEmpty(currentProject.id))
            {
                return;
            }
            
            _autoSaveEnabled = SaveManager.LoadBool(PREFS_AUTO_SAVE_ENABLED, true);
            if (_autoSaveEnabled)
            {
                StartCoroutine(SaveScheduler());
                _autoSaveCheckmark.SetActive(true);
            }
            
            DisplayLastSavedAt(currentProject.updatedAt);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
            {
                OnSave();
            }
        }
    }
}