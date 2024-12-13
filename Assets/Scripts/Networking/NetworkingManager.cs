using System.Collections;
using Danqzq.Models;
using Danqzq.Models.ServerResponses;
using UnityEngine;
using UnityEngine.Networking;

namespace Danqzq.Networking
{
    public class NetworkingManager : MonoBehaviour
    {
        private const string SERVER_URL = "https://ldc-server.danqzq.games/";

        public static event System.Action OnSendRequest;
        public static event System.Action OnReceiveResponse;
        
        public static Promise Authorize(string token) =>
            SendPostRequest(PostRoute.Authorize, ("token", token));
        
        public static Promise GetServerStatus() =>
            SendGetRequest(GetRoute.None);
        
        public static Promise<User> GetUserInfo(string userId) =>
            SendGetRequest(GetRoute.GetUserInfo, ("id", userId)).Cast<User>();
        
        public static Promise<User> GetPersonalData() =>
            SendGetRequest(GetRoute.GetPersonalData).Cast<User>();
        
        public static Promise UpdatePersonalData(string username, string bio) =>
            SendPostRequest(PostRoute.UpdatePersonalData, ("username", username), ("bio", bio));

        public static Promise<ProjectList> GetFeaturedProjects() =>
            SendGetRequest(GetRoute.GetFeaturedProjects).Cast<ProjectList>();
        
        public static Promise<ProjectList> GetMyProjects() =>
            SendGetRequest(GetRoute.GetMyProjects).Cast<ProjectList>();

        public static Promise<Project> UploadProject(Project project) =>
            SendPostRequest(PostRoute.UploadProject, JsonUtility.ToJson(project)).Cast<Project>();
        
        public static Promise<Project> UpdateProject(Project project) =>
            SendPostRequest(PostRoute.UpdateProject, JsonUtility.ToJson(project)).Cast<Project>();
        
        public static Promise<RateProjectResponse> RateProject(string projectId, byte rating) =>
            SendPostRequest(PostRoute.RateProject, ("id", projectId), ("rating", rating.ToString()))
                .Cast<RateProjectResponse>();
        
        public static Promise DeleteProject(string projectId) =>
            SendPostRequest(PostRoute.DeleteProject, ("id", projectId));
        
        public static void GetTexture(string url, System.Action<Texture> callback) => 
            _instance.StartCoroutine(ProcessGetTextureRequest(url, callback));
        
        private static NetworkingManager _instance;
        
        private void Awake()
        {
            _instance = this;
        }
        
        private static Promise SendGetRequest(GetRoute route, params (string k, string v)[] query)
        {
            var url = new System.Text.StringBuilder(SERVER_URL);
            url.Append(route.GetStringRoute());
            if (query.Length > 0)
            {
                url.Append('?');
                for (byte i = 0; i < query.Length; i++)
                {
                    url.Append($"{query[i].k}={query[i].v}");
                    if (i < query.Length - 1)
                    {
                        url.Append('&');
                    }
                }
            }

            var promise = new Promise();
            _instance.StartCoroutine(ProcessGetRequest(url.ToString(), promise));
            return promise;
        }
        
        private static Promise SendPostRequest(PostRoute route, params (string k, string v)[] data)
        {
            var jsonData = "";
            if (data.Length > 0)
            {
                jsonData += "{";
                for (byte i = 0; i < data.Length; i++)
                {
                    jsonData += "\"" + data[i].k + "\":\"" + data[i].v + "\"";
                    if (i < data.Length - 1)
                    {
                        jsonData += ",";
                    }
                }
                jsonData += "}";
            }
            
            var promise = new Promise();
            _instance.StartCoroutine(ProcessPostRequest(SERVER_URL + route.GetStringRoute(), jsonData, promise));
            return promise;
        }
        
        private static Promise SendPostRequest(PostRoute route, string jsonData)
        {
            var promise = new Promise();
            _instance.StartCoroutine(ProcessPostRequest(SERVER_URL + route.GetStringRoute(), jsonData, promise));
            return promise;
        }
        
        private static IEnumerator ProcessGetRequest(string url, Promise promise)
        {
            using var request = UnityWebRequest.Get(url);
            FillRequest(request);
            yield return request.SendWebRequest();
            OnRequestProcessed(request, promise);
        }
        
        private static IEnumerator ProcessPostRequest(string url, string jsonData, Promise promise)
        {
            using var request = new UnityWebRequest(url, "POST");
            FillRequest(request);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            yield return request.SendWebRequest();
            OnRequestProcessed(request, promise);
        }
        
        private static void FillRequest(UnityWebRequest request)
        {
            OnSendRequest?.Invoke();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + SaveManager.Load(Globals.ITCH_TOKEN_SAVE_KEY));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new ForceAcceptAllCertificates();
        }
        
        private static void OnRequestProcessed(UnityWebRequest request, Promise promise)
        {
            OnReceiveResponse?.Invoke();
            var result = request.downloadHandler.text;
            var isSuccessful = request.responseCode == 200;
            if (!isSuccessful && (string.IsNullOrEmpty(result) || result.Length > 1000))
            {
                result = "Failed to connect to server!";
            }
            promise.Resolve(result, isSuccessful);
        }

        private class ForceAcceptAllCertificates : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }
        
        private static IEnumerator ProcessGetTextureRequest(string url, System.Action<Texture> callback)
        {
            var uri = new System.Uri(url);
            using var request = UnityWebRequestTexture.GetTexture(uri);
            OnSendRequest?.Invoke();
            yield return request.SendWebRequest();
            OnReceiveResponse?.Invoke();
            var result = DownloadHandlerTexture.GetContent(request);
            callback.Invoke(result);
        }
    }
}