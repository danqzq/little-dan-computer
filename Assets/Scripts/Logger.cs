using System.Collections;
using TMPro;
using UnityEngine;

namespace Danqzq
{
    public sealed class Logger : MonoBehaviour
    {
        private const string LOG_TEXT_RESOURCE = "LogText";
        
        public enum MsgType
        {
            Info,
            Warning,
            Error
        }
        
        public static void Send(string message, MsgType type = MsgType.Info)
        {
#if UNITY_EDITOR
            switch (type)
            {
                case MsgType.Info:
                    Debug.Log(message);
                    break;
                case MsgType.Warning:
                    Debug.LogWarning(message);
                    break;
                case MsgType.Error:
                    Debug.LogError(message);
                    break;
            }
#endif
            var color = type switch
            {
                MsgType.Info => Color.green,
                MsgType.Warning => Color.yellow,
                MsgType.Error => Color.red,
                _ => Color.white
            };
            CreateLogObject(message, color);
        }

        private static void CreateLogObject(string text, Color color)
        {
            var obj = Instantiate(Resources.Load<GameObject>(LOG_TEXT_RESOURCE));
            var textObj = obj.GetComponentInChildren<TMP_Text>();
            textObj.text = text;
            textObj.color = color;
            textObj.StartCoroutine(AnimateText(textObj));
        }

        private static IEnumerator AnimateText(TMP_Text text)
        {
            const float animationTime = 2f;
            const float fadeTime = 1f;
            const float fadeStartTime = animationTime - fadeTime;
            var t = text.rectTransform;
            var time = 0f;
            var startPos = t.position;
            var endPos = startPos + Vector3.up * 50f;
            while (time < animationTime)
            {
                time += Time.deltaTime;
                var easeOut = Mathf.SmoothStep(0f, 1f, time / animationTime);
                t.position = Vector3.Lerp(startPos, endPos, easeOut);
                if (time >= fadeStartTime)
                {
                    var fadeEase = Mathf.SmoothStep(1f, 0f, (time - fadeStartTime) / fadeTime);
                    text.color = new Color(text.color.r, text.color.g, text.color.b, fadeEase);
                }
                yield return null;
            }
            
            Destroy(t.parent.gameObject);
        }
    }
}