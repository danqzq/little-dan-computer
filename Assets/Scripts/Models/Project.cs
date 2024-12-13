namespace Danqzq.Models
{
    [System.Serializable]
    public class Project : IServerObject
    {
        public string id;
        public string name;
        public string description;
        public string authorUsername;
        public string authorUserId;
        public float rating;
        public string projectData;
        public string createdAt;
        public string updatedAt;

        public Rating[] ratings;
        
        public Project(string name, string description, string authorUserId)
        {
            this.name = name;
            this.description = description;
            this.authorUserId = authorUserId;
        }

        public void UpdateRating(string userId, int newRating)
        {
            for (int i = 0; i < ratings.Length; i++)
            {
                if (ratings[i].userId == userId)
                {
                    ratings[i].rating = (byte) newRating;
                    return;
                }
            }
        }
    }
    
    [System.Serializable]
    public struct Rating
    {
        public string userId;
        public byte rating;
    }
}