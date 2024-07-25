using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2; // Google.Apis.Auth 네임스페이스 추가

public class FCMNotifier : MonoBehaviour
{
    private const string FCM_URL = "https://fcm.googleapis.com/v1/projects/sysnet-android/messages:send";
    private string accessToken;

    public static FCMNotifier Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public IEnumerator GetAccessTokenAndSendMessage(string token, string title, string body)
    {
        Task<string> task = GetAccessTokenAsync();
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (task.IsFaulted)
        {
            Debug.LogError("Error obtaining access token: " + task.Exception);
            yield break;
        }

        accessToken = task.Result;
        SendFCMMessage($"{token}", $"{title}", $"{body}");
    }

    //private async Task<string> GetAccessTokenAsync()
    //{
    //    string jsonPath = Path.Combine(Application.streamingAssetsPath, "sysnet-android-firebase-adminsdk-idbg2-54fd4ebc7b.json");
    //    string[] scopes = { "https://www.googleapis.com/auth/firebase.messaging" };

    //    GoogleCredential credential;
    //    using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
    //    {
    //        credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
    //    }

    //    return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
    //}

    private async Task<string> GetAccessTokenAsync()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "sysnet-android-firebase-adminsdk-idbg2-54fd4ebc7b.json");
        string[] scopes = { "https://www.googleapis.com/auth/firebase.messaging" };
        string jsonContent = null;
        GoogleCredential credential = null;


        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            UnityWebRequest www = UnityWebRequest.Get(jsonPath);
            var asyncOp = www.SendWebRequest();

            while (!asyncOp.isDone)
                await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load JSON file: " + www.error);
                return null;
            }

            jsonContent = www.downloadHandler.text;

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent)))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }
        }

        return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
    }


    public void SendFCMMessage(string token, string title, string body)
    {
        StartCoroutine(SendNotification(token, title, body));
    }

    private IEnumerator SendNotification(string token, string title, string body)
    {
        //var message = new
        //{
        //    message = new
        //    {
        //        token = token,
        //        notification = new
        //        {
        //            title = title,
        //            body = body
        //        }
        //    }
        //};
        var message = new
        {
            message = new
            {
                token = token,
                notification = new
                {
                    title = title,
                    body = body
                },
                android = new
                {
                    priority = "high"
                }
            }
        };

        string jsonMessage = JObject.FromObject(message).ToString();
        byte[] postData = Encoding.UTF8.GetBytes(jsonMessage);

        UnityWebRequest request = new UnityWebRequest(FCM_URL, "POST");
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"FCM 메시지 전송 성공 : {title}");
        }
        else
        {
            Debug.LogError("FCM 메시지 전송 실패: " + request.error);
        }
    }
}
