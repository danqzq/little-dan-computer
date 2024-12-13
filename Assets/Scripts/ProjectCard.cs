using Danqzq.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Danqzq
{
    public class ProjectCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _authorUsernameText;

        [SerializeField] private TMP_Text _ratingText;
        
        [SerializeField] private Button[] _rateButtons;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _viewAuthorButton;
        
        private Project _project;
        private System.Action _onClick;

        private static Color _defaultButtonColor = Color.white;
        private static Color _selectedButtonColor = Color.yellow;

        public void Init(Project project, string userId, System.Action onClick, System.Action<byte> onRate,
            System.Action onDelete, System.Action onViewAuthor)
        {
            _project = project;
            _nameText.text = project.name;
            _descriptionText.text = project.description;
            _authorUsernameText.text = $"by {project.authorUsername}";
            _ratingText.text = $"Rating:\n{_project.rating:F1}";
            _onClick = onClick;

            if (_project.authorUserId == userId)
            {
                foreach (var button in _rateButtons)
                {
                    button.gameObject.SetActive(false);
                }
                
                _deleteButton.gameObject.SetActive(true);
                _deleteButton.onClick.AddListener(onDelete.Invoke);
            }
            else
            {
                ValidateRating(userId);
                for (int i = 0; i < _rateButtons.Length; i++)
                {
                    var rating = i + 1;
                    _rateButtons[i].onClick.AddListener(() =>
                    {
                        onRate.Invoke((byte) rating);
                        foreach (var button in _rateButtons)
                        {
                            button.image.color = _defaultButtonColor;
                        }
                        for (int j = 0; j < rating; j++)
                        {
                            _rateButtons[j].image.color = _selectedButtonColor;
                        }
                    });
                }
            }
            
            _viewAuthorButton.onClick.AddListener(onViewAuthor.Invoke);
        }

        public void OnClick()
        {
            ProjectManager.CurrentProject = _project;
            _onClick.Invoke();
        }

        private void ValidateRating(string userId)
        {
            if (_project.ratings == null)
            {
                return;
            }
            
            foreach (var rating in _project.ratings)
            {
                if (rating.userId != userId)
                {
                    continue;
                }
                
                for (int i = 0; i < rating.rating; i++)
                {
                    _rateButtons[i].image.color = _selectedButtonColor;
                }
                break;
            }
        }
    }
}