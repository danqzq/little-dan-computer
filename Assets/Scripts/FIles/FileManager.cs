using System.Runtime.InteropServices;

namespace Danqzq.Files
{
    public static class FileManager
    {
        #if UNITY_WEBGL
        
        [DllImport("__Internal")]
        private static extern void LoadFileFromBrowser(string gameObjectName, string methodName);
        
        [DllImport("__Internal")]
        private static extern void DownloadFile(byte[] array, int size, string filename);
        
        public static void Load(string gameObjectName, string methodName) => 
            LoadFileFromBrowser(gameObjectName, methodName);

        public static void Download(byte[] array, string filename) =>
            DownloadFile(array, array.Length, filename);
#endif
    }
}