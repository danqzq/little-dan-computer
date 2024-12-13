using System.Collections;
using Danqzq.Models;
using Danqzq.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Danqzq.Globals;

namespace Danqzq
{
    public class ProjectListManager : MonoBehaviour
    {
        [SerializeField] private Form _newProjectForm;
        [SerializeField] private Form _uploadProjectForm;
        [SerializeField] private Form _workshopsForm;
        
        [SerializeField] private GameObject _myProjectsMenu;
        [SerializeField] private Transform _myProjectsContainer;
        [SerializeField] private TMP_Text _myProjectsTitleText;
        
        [SerializeField] private GameObject _featuredProjectsMenu;
        [SerializeField] private Transform _featuredProjectsContainer; 
        [SerializeField] private TMP_Text _featuredProjectsTitleText;
        
        [SerializeField] private GameObject _projectCardPrefab;
        
        [SerializeField] private UserInfoManager _userInfoManager;

        [SerializeField] private TMP_Text _versionText;
        
        [SerializeField] private ConfirmationMenu _confirmationMenu;
        
        private ProjectList _myProjects;
        private ProjectList _featuredProjects;

        private string _userId;
        
        private const string FORM_NAME = "name";
        private const string FORM_DESCRIPTION = "description";
        
        private static AsyncOperation _sceneLoadingOperation;
        
        public enum Mode
        {
            None,
            New,
            Upload,
            Workshops
        }
        
        public static Mode CurrentMode { get; set; }
        
        private void ProcessMode()
        {
            switch (CurrentMode)
            {
                case Mode.New:
                    _newProjectForm.ToggleVisibility(true);
                    _newProjectForm.OnSubmit = OnCreateNewProject;
                    break;
                case Mode.Upload:
                    _uploadProjectForm.ToggleVisibility(true);
                    _uploadProjectForm[FORM_NAME] = ProjectManager.CurrentProject.name;
                    _uploadProjectForm[FORM_DESCRIPTION] = ProjectManager.CurrentProject.description;
                    _uploadProjectForm.OnSubmit = string.IsNullOrEmpty(ProjectManager.CurrentProject.id)
                        ? OnUploadProject
                        : OnUpdateProject;
                    break;
                case Mode.Workshops:
                    _workshopsForm.ToggleVisibility(true);
                    break;
            }
            
            CurrentMode = Mode.None;
        }

        private void Awake()
        {
            _sceneLoadingOperation = null;
            _versionText.text = $"v{Application.version}";
        }

        private void Start()
        {
            ProcessMode();
            FetchPersonalData();
        }

        private void FetchPersonalData()
        {
            const string failedToLoginMessage = @"Failed to login!
This could be due to a server connection error or an expired login token.";
            
            _userInfoManager.Init().Then(userId =>
            {
                _userId = userId;
                OpenMyProjectsMenu();
            }).Catch(err =>
            {
                Logger.Send($"Failed to get personal data: {err}", Logger.MsgType.Error);
                _confirmationMenu.Show(failedToLoginMessage, "Try Again", FetchPersonalData, RetryLogin);
            });
        }
        
        private void RetryLogin()
        {
            SceneManager.LoadScene(SCENE_AUTHORIZATION);
        }

        private void OnCreateNewProject()
        {
            var projectName = _newProjectForm[FORM_NAME];
            if (string.IsNullOrEmpty(projectName))
            {
                Logger.Send("Project name can't be empty.", Logger.MsgType.Error);
                return;
            }
            
            var projectDescription = _newProjectForm[FORM_DESCRIPTION];
            OpenProject(new Project(projectName, projectDescription, _userId));
        }
        
        private void OnUploadProject()
        {
            var project = ProjectManager.CurrentProject;
            project.name = _uploadProjectForm[FORM_NAME];
            project.description = _uploadProjectForm[FORM_DESCRIPTION];
            if (string.IsNullOrEmpty(project.name))
            {
                Logger.Send("Project name can't be empty.", Logger.MsgType.Error);
                return;
            }
            NetworkingManager.UploadProject(project).Then(uploadedProject =>
            {
                Logger.Send("Project uploaded successfully!");
                OnSuccessfulUploadOrUpdate(uploadedProject);
            }).Catch(error =>
            {
                Logger.Send(error, Logger.MsgType.Error);
                OpenProject(project);
            });
            Logger.Send("Uploading project...");
        }

        private void OnUpdateProject()
        {
            var project = ProjectManager.CurrentProject;
            project.name = _uploadProjectForm[FORM_NAME];
            project.description = _uploadProjectForm[FORM_DESCRIPTION];
            if (string.IsNullOrEmpty(project.name))
            {
                Logger.Send("Project name can't be empty.", Logger.MsgType.Error);
                return;
            }
            NetworkingManager.UpdateProject(project).Then(updatedProject =>
            {
                Logger.Send("Project updated successfully!");
                OnSuccessfulUploadOrUpdate(updatedProject);
            }).Catch(error =>
            {
                Logger.Send(error, Logger.MsgType.Error);
                OpenProject(project);
            });
            Logger.Send("Updating project...");
        }

        private void OnSuccessfulUploadOrUpdate(Project project)
        {
            ProjectManager.CurrentProject = project;
            _uploadProjectForm.ToggleVisibility(false);
            FreshOpenMyProjectsMenu();
        }
        
        public void OpenNewProjectMenu()
        {
            _newProjectForm.ToggleVisibility(true);
            _newProjectForm.OnSubmit = OnCreateNewProject;
        }
        
        public void OpenMyProjectsMenu()
        {
            _myProjectsMenu.SetActive(true);
            _featuredProjectsMenu.SetActive(false);
            if (_myProjects == null)
            {
                FreshOpenMyProjectsMenu();
            }
            else
            {
                DisplayMyProjects();
            }
        }

        public void FreshOpenMyProjectsMenu()
        {
            NetworkingManager.GetMyProjects().Then(projectList =>
            {
                _myProjects = projectList;
                DisplayMyProjects();
            }).Catch(error => Logger.Send(error, Logger.MsgType.Error));
        }
            
        public void OpenFeaturedProjectsMenu()
        {
            _myProjectsMenu.SetActive(false);
            _featuredProjectsMenu.SetActive(true);
            if (_featuredProjects == null)
            {
                FreshOpenFeaturedProjectsMenu();
            }
            else
            {
                DisplayFeaturedProjects();
            }
        }
        
        public void FreshOpenFeaturedProjectsMenu()
        {
            NetworkingManager.GetFeaturedProjects().Then(projectList =>
            {
                _featuredProjects = projectList;
                DisplayFeaturedProjects();
            }).Catch(error => Logger.Send(error, Logger.MsgType.Error));
        }
        
        private void OpenProject(Project project)
        {
            ProjectManager.CurrentProject = project;
            ProjectManager.IsCurrentProjectOwned = project.authorUserId == _userId;
            _sceneLoadingOperation ??= SceneManager.LoadSceneAsync(SCENE_MAIN);
            if (_sceneLoadingOperation == null)
            {
                return;
            }
            
            _sceneLoadingOperation.allowSceneActivation = false;
            
            StopCoroutine(nameof(LoadScene));
            _confirmationMenu.Show($"Loading \"{project.name}\"...", "Open Project",
                () => _sceneLoadingOperation.allowSceneActivation = true);
            _confirmationMenu.IsInteractable = false;
            
            IEnumerator LoadScene()
            {
                while (_sceneLoadingOperation.progress < 0.9f)
                {
                    yield return null;
                }
                _confirmationMenu.SetText($"\"{project.name}\" loaded successfully!");
                _confirmationMenu.IsInteractable = true;
            }
            
            StartCoroutine(LoadScene());
        }
        
        private void OnRateProject(string projectId, byte rating)
        {
            NetworkingManager.RateProject(projectId, rating).Then(res =>
            {
                Logger.Send("Project rated successfully");
                var newAverageRating = res.newRating;
                var project = System.Array.Find(_featuredProjects.projects, project => project.id == projectId);
                project.rating = newAverageRating;
                project.UpdateRating(_userId, rating);
                DisplayProjects(_featuredProjectsContainer, _featuredProjects);
            }).Catch(error => Logger.Send(error, Logger.MsgType.Error));
        }
        
        private void OnDeleteProject(string projectId)
        {
            NetworkingManager.DeleteProject(projectId).Then(_ =>
            {
                Logger.Send("Project deleted successfully");
                FreshOpenMyProjectsMenu();
            }).Catch(error => Logger.Send(error, Logger.MsgType.Error));
        }
        
        private void DisplayMyProjects()
        {
            AnimateTextFill(_myProjectsTitleText, "My Projects");
            DisplayProjects(_myProjectsContainer, _myProjects);
        }

        private void DisplayFeaturedProjects()
        {
            AnimateTextFill(_featuredProjectsTitleText, "Featured Projects");
            DisplayProjects(_featuredProjectsContainer, _featuredProjects);
        }

        private void DisplayProjects(Transform container, ProjectList projectList)
        {
            if (projectList == null)
            {
                return;
            }
            
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            
            foreach (var project in projectList.projects)
            {
                var projectCard = Instantiate(_projectCardPrefab, container).GetComponent<ProjectCard>();
                projectCard.Init(project, _userId,
                    () => OpenProject(project),
                    rating => OnRateProject(project.id, rating),
                    () => _confirmationMenu.Show("Are you sure you want to delete this project?",
                        () => OnDeleteProject(project.id)),
                    () => _userInfoManager.DisplayUserInfo(project.authorUserId));
            }
        }
        
        private static void AnimateTextFill(TMP_Text text, string value)
        {
            const float delay = 0.05f;
            
            IEnumerator Animate()
            {
                text.text = string.Empty;
                foreach (var c in value)
                {
                    text.text += c;
                    yield return new WaitForSeconds(delay);
                }
            }
            
            text.StopAllCoroutines();
            text.StartCoroutine(Animate());
        }
    }
}