using UnityEngine;

namespace Danqzq
{
    public static class ExtensionMethods
    {
        public static string Display(this short n)
        {
            return AppManager.numericFormat switch
            {
                NumericFormat.Hexadecimal => n.ToString("X4"),
                NumericFormat.Octal => System.Convert.ToString(n, 8).PadLeft(6, '0'),
                _ => n.ToString()
            };
        }
        
        public static string Display(this short n, NumericFormat numericFormat)
        {
            return numericFormat switch
            {
                NumericFormat.Hexadecimal => n.ToString("X4"),
                NumericFormat.Octal => System.Convert.ToString(n, 8).PadLeft(6, '0'),
                _ => n.ToString()
            };
        }

        public static string Display(this NumericFormat b)
        {
            return b switch
            {
                NumericFormat.Hexadecimal => "HEX",
                NumericFormat.Decimal => "DEC",
                NumericFormat.Octal => "OCT",
                _ => "???"
            };
        }
        
        public static void Fade(this CanvasGroup canvasGroup, bool fadeIn)
        {
            canvasGroup.alpha = fadeIn ? 1 : 0;
        }
        
        public static void Toggle(this CanvasGroup canvasGroup, bool on)
        {
            canvasGroup.Fade(on);
            canvasGroup.blocksRaycasts = on;
            canvasGroup.interactable = on;
        }
    }
}