using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; // Newtonsoft.Json 라이브러리를 사용합니다.

public class ClientAJAXJson : MonoBehaviour
{
    public string URL = "http://192.168.10.122:8080/s_json.php";
    public float interval = 0.5f; // 요청 간격을 0.5초로 설정합니다.

    // 게임이 시작될 때 데이터를 받아옵니다.
    void Start()
    {
        StartCoroutine(FetchData());
    }

    IEnumerator FetchData()
    {
        while (true) // 무한 반복을 통해 0.5초 간격으로 데이터를 계속 받아옵니다.
        {
            // UnityWebRequest.Get을 사용하여 URL에서 GET 요청을 보냅니다.
            using (UnityWebRequest www = UnityWebRequest.Get(URL))
            {
                yield return www.SendWebRequest(); // 요청이 완료될 때까지 기다립니다.

                // 요청이 실패한 경우 오류 메시지를 출력합니다.
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else // 요청이 성공한 경우
                {
                    // www.downloadHandler.text는 응답 본문을 문자열로 반환합니다.
                    // 이 문자열을 JsonConvert.DeserializeObject를 사용하여 C# 객체로 변환합니다.
                    var data = JsonConvert.DeserializeObject(www.downloadHandler.text);

                    // 변환된 데이터를 사용하여 필요한 작업을 수행합니다.
                    // 예를 들어, 'name' 필드의 값을 콘솔에 출력하려면 다음과 같이 사용합니다:
                    // Debug.Log(data.name);
                    Debug.Log(data.ToString());
                }

                yield return new WaitForSeconds(interval); // 다음 요청까지 0.5초 대기합니다.
            }
        }
    }
}
