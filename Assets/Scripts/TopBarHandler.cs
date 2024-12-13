using Danqzq.Files;
using Danqzq.Models;
using Danqzq.Workshops;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Danqzq.Globals;

namespace Danqzq
{
    public class TopBarHandler : MonoBehaviour
    {
        [SerializeField] private Assembler _assembler;
        [SerializeField] private ProjectDataController _projectDataController;
        [SerializeField] private GameObject _creditsMenu;
        
        [SerializeField] private TMPro.TMP_Text _projectNameText;
        
        [SerializeField] private ConfirmationMenu _confirmationMenu;

        [SerializeField] private GameObject _workshopButton;
        [SerializeField] private GameObject _workshopHintMenu;
        [SerializeField] private TMPro.TMP_Text _workshopHintText;

        private int _hintIndex;
        
        private const string UNTITLED_PROJECT_NAME = "Untitled";
        private const string FILE_FORMAT = "ldc";

        private static bool IsGuestLogin => ProjectManager.CurrentProject == null;

        private void Start()
        {
            _hintIndex = 0;
            _projectNameText.text = ProjectManager.CurrentProject?.name ??
                                    WorkshopManager.CurrentWorkshop?.Name ?? 
                                    UNTITLED_PROJECT_NAME;
            
            _workshopButton.SetActive(WorkshopManager.CurrentWorkshop != null);
        }

        private void CheckProjectThenDo(System.Action onContinue, string action)
        {
            if (!ProjectManager.IsCurrentProjectOwned || !_assembler.IsAssemblyEdited)
            {
                onContinue.Invoke();
                return;
            }
            
            _confirmationMenu.Show($"You have unsaved changes. Do you still want to {action}?", "Yes", onContinue);
        }
        
        #region Project
        
        public void OnNewProject()
        {
            void OnNewProjectCallback()
            {
                if (IsGuestLogin)
                {
                    SceneManager.LoadScene(SCENE_MAIN);
                    return;
                }
            
                ProjectListManager.CurrentMode = ProjectListManager.Mode.New;
                SceneManager.LoadScene(SCENE_PROJECT_LIST);
            }
            
            CheckProjectThenDo(OnNewProjectCallback, "create a new project");
        }
        
        public void OnUploadProject()
        {
            if (IsGuestLogin)
            {
                Logger.Send("You must login to upload a project.", Logger.MsgType.Warning);
                return;
            }
            
            if (!ProjectManager.IsCurrentProjectOwned)
            {
                Logger.Send("This project is not yours. You can't update it.", Logger.MsgType.Warning);
                return;
            }
            
            ProjectManager.CurrentProject.projectData = _projectDataController.ExportToJson();
            ProjectListManager.CurrentMode = ProjectListManager.Mode.Upload;
            SceneManager.LoadScene(SCENE_PROJECT_LIST);
        }
        
        public void BackToProjectList()
        {
            void OnBackToProjectListCallback()
            {
                if (IsGuestLogin)
                {
                    SceneManager.LoadScene(SCENE_AUTHORIZATION);
                    return;
                }
                SceneManager.LoadScene(SCENE_PROJECT_LIST);
            }
            
            CheckProjectThenDo(OnBackToProjectListCallback, "exit");
        }
        
        #endregion
        
        #region Edit

        public void OnExport()
        {
            if (IsGuestLogin)
            {
                _assembler.IsAssemblyEdited = false;
            }
            
            var projectData = _projectDataController.Export();
            var json = JsonUtility.ToJson(projectData);
            var byteArray = System.Text.Encoding.UTF8.GetBytes(json);
            var currentProjectName = ProjectManager.CurrentProject.name;
            if (string.IsNullOrEmpty(currentProjectName))
                currentProjectName = UNTITLED_PROJECT_NAME;
            FileManager.Download(byteArray, $"{currentProjectName}.{FILE_FORMAT}");
        }
        
        public void OnImport()
        {
            FileManager.Load(gameObject.name, nameof(OnImportCallback));
        }
        
        public void OnImportCallback(string fileContent)
        {
            var projectData = JsonUtility.FromJson<ProjectData>(fileContent);
            _confirmationMenu.Show("File loaded. Do you want to import the project?",
                () => OnImportConfirmed(projectData));
        }
        
        private void OnImportConfirmed(ProjectData projectData)
        {
            _assembler.IsAssemblyEdited = false;
            _projectDataController.Import(projectData);
        }

        #endregion
        
        #region Help
        
        public void OnInstructionSet()
        {
            Application.OpenURL(URL_INSTRUCTION_SET);
        }
        
        public void OnCredits()
        {
            Application.OpenURL(URL_PORTFOLIO);
        }
        
        #endregion
        
        #region Workshop

        public void OnShowHint()
        {
            var workshop = WorkshopManager.CurrentWorkshop;
            _workshopHintMenu.SetActive(true);
            _workshopHintText.text = workshop.Hints[_hintIndex++];
            if (_hintIndex >= workshop.Hints.Length)
            {
                _hintIndex = 0;
            }
        }
        
        #endregion
    }
}