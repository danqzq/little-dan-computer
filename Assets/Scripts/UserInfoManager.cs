using Danqzq.Models;
using Danqzq.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Danqzq
{
    public class UserInfoManager : MonoBehaviour
    {
        [SerializeField] private Form _updatePersonalDataForm;

        [SerializeField] private TMP_Text _loggedInAsText;
        
        [SerializeField] private Animator _userInfoMenu;
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private TMP_Text _bioText;
        [SerializeField] private RawImage _coverImage;
        [SerializeField] private Button _viewProfileButton;
        
        private static readonly int OpenAnimatorTrigger = Animator.StringToHash("open");

        private const string FORM_USERNAME = "username";
        private const string FORM_BIO = "bio";
        
        private Promise<User> _displayUserInfoPromise;
        
        public Promise Init()
        {
            var promise = new Promise();
            NetworkingManager.GetServerStatus().Then(res =>
            {
                Logger.Send(res);
                NetworkingManager.GetPersonalData().Then(user =>
                {
                    OnGetPersonalData(user);
                    promise.Resolve(user.id);
                })
                .Catch(err =>
                {
                    promise.Resolve(err, false);
                });
            })
            .Catch(err =>
            {
                promise.Resolve(err, false);
            });
            return promise;
        }
        
        private void OnGetPersonalData(User user)
        {
            _updatePersonalDataForm[FORM_USERNAME] = user.username;
            _updatePersonalDataForm[FORM_BIO] = user.bio;
            _updatePersonalDataForm.OnSubmit = UpdatePersonalData;
            
            _loggedInAsText.text = $"Logged in as: {user.username}";
        }
        
        private void UpdatePersonalData()
        {
            var username = _updatePersonalDataForm[FORM_USERNAME];
            var bio = _updatePersonalDataForm[FORM_BIO];

            NetworkingManager.UpdatePersonalData(username, bio)
                .Then(_ => Logger.Send("Personal data updated successfully"))
                .Catch(err => Logger.Send($"Failed to update personal data: {err}", Logger.MsgType.Error));
        }
        
        public void DisplayUserInfo(string authorId)
        {
            if (_displayUserInfoPromise is { IsResolved: false })
            {
                return;
            }
            _displayUserInfoPromise = NetworkingManager.GetUserInfo(authorId);
            _displayUserInfoPromise.Then(OnGetUserInfo)
                .Catch(err => Logger.Send($"Failed to get user info: {err}", Logger.MsgType.Error));
        }
        
        private void OnGetUserInfo(User user)
        {
            _usernameText.text = $"<b>Username</b>: {user.username}";
            _bioText.text = string.IsNullOrEmpty(user.bio) ? "No bio provided." : user.bio;
            _viewProfileButton.onClick.RemoveAllListeners();
            _viewProfileButton.onClick.AddListener(() => Application.OpenURL(user.url));

            if (!string.IsNullOrEmpty(user.coverUrl))
            {
                NetworkingManager.GetTexture(user.coverUrl, texture => _coverImage.texture = texture);
            }
            
            _userInfoMenu.SetTrigger(OpenAnimatorTrigger);
        }
    }
}