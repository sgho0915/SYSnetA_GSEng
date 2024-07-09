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

    private void Start()
    {
        StartCoroutine(GetAccessTokenAndSendMessage());
    }

    private IEnumerator GetAccessTokenAndSendMessage()
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
        SendFCMMessage("cveygNPOS9ygDBPc1sUGWw:APA91bEs8bmdLO8Uoyl3ZY8eyCT2o7zOTwBZ8LhJUG_CJwCru1Cn80FfJlh17mXM4Eb871Br0-88nOUQ4sQez3aRzj5Jkh1PJ5UUbws8jiFjp5z5KATljcz7fXgEo86qO68XK5Ig24UG", "유니티 푸시 전송 테스트 title", "유니티 푸시 테스트 body");
    }

    private async Task<string> GetAccessTokenAsync()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "sysnet-android-firebase-adminsdk-idbg2-42574b9fb6.json");
        string[] scopes = { "https://www.googleapis.com/auth/firebase.messaging" };

        GoogleCredential credential;
        using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
        }

        return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
    }

    public void SendFCMMessage(string token, string title, string body)
    {
        StartCoroutine(SendNotification(token, title, body));
    }

    private IEnumerator SendNotification(string token, string title, string body)
    {
        var message = new
        {
            message = new
            {
                token = token,
                notification = new
                {
                    title = title,
                    body = body
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
            Debug.Log("FCM 메시지 전송 성공");
        }
        else
        {
            Debug.LogError("FCM 메시지 전송 실패: " + request.error);
        }
    }
}
