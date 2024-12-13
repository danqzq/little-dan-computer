using Danqzq.Models;

namespace Danqzq
{
    public static class ProjectManager
    {
        public static Project CurrentProject { get; set; }
        
        public static bool IsCurrentProjectOwned { get; set; }
    }
}