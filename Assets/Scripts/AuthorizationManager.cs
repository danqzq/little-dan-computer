using Danqzq.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

using static Danqzq.Globals;

namespace Danqzq
{
    public class AuthorizationManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _loginCanvasGroup;
        [SerializeField] private CanvasGroup _tokenCanvasGroup;

        [SerializeField] private TMP_InputField _tokenInputField;
        
        [SerializeField] private Loader _loader;
        
        private void Start()
        {
            if (!string.IsNullOrEmpty(SaveManager.Load(ITCH_TOKEN_SAVE_KEY)))
            {
                ProceedToNextScene();
            }
            else
            {
                _loader.Deactivate();
            }
        }

        public void OpenTokenMenu()
        {
            Application.OpenURL(URL_ITCH_AUTH);
            
            _loginCanvasGroup.Toggle(false);
            _tokenCanvasGroup.Toggle(true);
        }
        
        public void OnTokenInput()
        {
            var token = _tokenInputField.text;
            if (string.IsNullOrEmpty(token))
            {
                Logger.Send("Token is empty.", Logger.MsgType.Error);
                return;
            }
            
            _loader.Activate();
            NetworkingManager.Authorize(_tokenInputField.text).Then(_ =>
            {
                Logger.Send("Authorization successful.");
                SaveManager.Save(ITCH_TOKEN_SAVE_KEY, token);
                ProceedToNextScene();
            }).Catch(error =>
            {
                _loader.Deactivate();
                Logger.Send(error, Logger.MsgType.Error);
            });
        }
        
        public void OnGoBack()
        {
            _loginCanvasGroup.Toggle(true);
            _tokenCanvasGroup.Toggle(false);
        }
        
        public void GuestLogin()
        {
            SceneManager.LoadScene(SCENE_MAIN);
        }
        
        private void ProceedToNextScene()
        {
            SceneManager.LoadSceneAsync(SCENE_PROJECT_LIST);
        }
    }
}
