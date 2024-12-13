using UnityEngine;

namespace Danqzq
{
    public static class SaveManager
    {
        public static void Save(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
        
        public static void SaveObject<T>(string key, T value)
        {
            Save(key, JsonUtility.ToJson(value));
        }
        
        public static void SaveBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }
        
        public static string Load(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }
        
        public static T LoadObject<T>(string key, T defaultValue = default)
        {
            var json = Load(key);
            return string.IsNullOrEmpty(json) ? defaultValue : JsonUtility.FromJson<T>(json);
        }
        
        public static bool LoadBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }
    }
}