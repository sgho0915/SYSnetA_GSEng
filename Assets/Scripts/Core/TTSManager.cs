using System.Collections;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class TTSManager : MonoBehaviour
{
    //TTS에서 사용할 오디오 소스
    public AudioSource mAudio;

    //문자열을 계속 바꾸기에 빌더를 사용한다.
    private StringBuilder mStrBuilder;

    //구글 TTS를 이용할 오리지널 앞 주소
    private string mPrefixURL;
    public static TTSManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mPrefixURL = "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q=";

        //this.AddComponent<AudioSource>();
        //mAudio = GetComponent<AudioSource>();
        mStrBuilder = new StringBuilder();

        //RunTTS("풀무원 함안1에서 압력센서1 통신 경보가 발생했습니다.");
    }

    //외부에서 호출되며 문자열, 언어를 받아 코루틴을 실행시킨다.
    public void RunTTS(string text, SystemLanguage language = SystemLanguage.Korean)
    {
        StartCoroutine(DownloadTheAudio(text, language));
    }

    //오디오를 다운로드 받는다.
    IEnumerator DownloadTheAudio(string text, SystemLanguage language = SystemLanguage.Korean)
    {
        mStrBuilder.Clear();

        //텍스트 앞 Origin URL
        mStrBuilder.Append(mPrefixURL);

        //TTS로 변환할 텍스트
        mStrBuilder.Append(text);
        mStrBuilder.Replace('\n', '.');

        //언어 인식을 위한 태그 추가 &tl=
        mStrBuilder.Append("&tl=");

        //언어 식별
        switch (language)
        {
            case SystemLanguage.Korean:
                {
                    mStrBuilder.Append("Ko-kr");
                    break;
                }

            case SystemLanguage.English:
            default:
                {
                    mStrBuilder.Append("En-gb");
                    break;
                }
        }

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(mStrBuilder.ToString(), AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                mAudio.clip = DownloadHandlerAudioClip.GetContent(www);
                mAudio.pitch = 1.1f;
                
                mAudio.Play();
            }
        }
    }
}