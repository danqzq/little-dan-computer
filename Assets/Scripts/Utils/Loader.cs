using System.Collections;
using Danqzq.Networking;
using UnityEngine;

namespace Danqzq
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Loader : MonoBehaviour
    {
        private CanvasGroup _loaderUI;

        private void Awake()
        {
            _loaderUI = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            NetworkingManager.OnSendRequest += Activate;
            NetworkingManager.OnReceiveResponse += Deactivate;
        }

        private void OnDestroy()
        {
            NetworkingManager.OnSendRequest -= Activate;
            NetworkingManager.OnReceiveResponse -= Deactivate;
        }

        public void Activate() => StartCoroutine(ToggleLoadingScreen(true));
        public void Deactivate() => StartCoroutine(ToggleLoadingScreen(false));
        
        private IEnumerator ToggleLoadingScreen(bool enable)
        {
            const float fadeTime = 0.25f;
            var time = 0f;
            var startAlpha = _loaderUI.alpha;
            var endAlpha = enable ? 1 : 0;
            while (time < fadeTime)
            {
                time += Time.deltaTime;
                _loaderUI.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeTime);
                yield return null;
            }
            _loaderUI.Toggle(enable);
        }
    }
}