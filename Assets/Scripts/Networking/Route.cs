namespace Danqzq.Networking
{
    public enum GetRoute : byte
    {
        None,
        GetFeaturedProjects,
        GetMyProjects,
        GetUserInfo,
        GetPersonalData
    }
    
    public enum PostRoute : byte
    {
        Authorize,
        UploadProject,
        UpdateProject,
        DeleteProject,
        RateProject,
        UpdatePersonalData
    }

    public static class Route
    {
        public static string GetStringRoute(this GetRoute route) => route switch
        {
            GetRoute.None => "",
            GetRoute.GetFeaturedProjects => "api/get-featured-projects",
            GetRoute.GetMyProjects => "api/get-my-projects",
            GetRoute.GetUserInfo => "api/get-user-info",
            GetRoute.GetPersonalData => "api/get-personal-data",
            _ => throw new System.ArgumentOutOfRangeException(nameof(route), route, null)
        };

        public static string GetStringRoute(this PostRoute route) => route switch
        {
            PostRoute.Authorize => "login",
            PostRoute.UploadProject => "api/upload-project",
            PostRoute.UpdateProject => "api/update-project",
            PostRoute.DeleteProject => "api/delete-project",
            PostRoute.RateProject => "api/rate-project",
            PostRoute.UpdatePersonalData => "api/update-personal-data",
            _ => throw new System.ArgumentOutOfRangeException(nameof(route), route, null)
        };
    }
}