namespace Danqzq.Models
{
    [System.Serializable]
    public class User : IServerObject
    {
        public string id;
        public string username;
        public string bio;

        public string url;
        public string coverUrl;
    }
}