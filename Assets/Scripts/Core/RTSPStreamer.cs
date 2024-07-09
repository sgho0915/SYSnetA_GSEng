using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RTSPStreamer : MonoBehaviour
{
    public RawImage rawImage;
    private AndroidJavaObject activityContext;
    private Texture2D texture;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                activityContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }

            // 플레이스홀더 크기로 텍스처 초기화
            texture = new Texture2D(680, 480, TextureFormat.RGBA32, false);
            rawImage.texture = texture;

            PlayRTSP("rtsp://210.99.70.120:1935/live/cctv001.stream");
            // 천안시 세집매 삼거리 : rtsp://210.99.70.120:1935/live/cctv001.stream
            // 연구소 : rtsp://192.168.10.70:554/rtsp/streaming?channel=01&subtype=A
            // 합천 26 : rtsp://youngdo6669.iptime.org:26554/profile3/media.smp
            // 고성 156 : rtsp://hansolfarm.iptime.org:28156/profile3/media.smp
        }
    }

    // RTSP 재생 함수 호출
    private void PlayRTSP(string rtspUrl)
    {
        using (AndroidJavaClass rtspPlayer = new AndroidJavaClass("com.systronics.plugin.RTSPPlayer"))
        {
            rtspPlayer.CallStatic("PlayRTSP", rtspUrl);
        }
    }

    // 프레임 업데이트 함수
    public void UpdateFrame(string frameDataBase64)
    {
        byte[] frameData = Convert.FromBase64String(frameDataBase64);

        // 텍스처가 null이거나 크기가 rawImage의 크기와 다르면 텍스처를 다시 생성
        if (texture == null || texture.width != (int)rawImage.rectTransform.rect.width || texture.height != (int)rawImage.rectTransform.rect.height)
        {
            int width = (int)rawImage.rectTransform.rect.width;
            int height = (int)rawImage.rectTransform.rect.height;

            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            rawImage.texture = texture;
        }

        // 새로운 프레임 데이터로 텍스처 업데이트
        texture.LoadRawTextureData(frameData);
        texture.Apply();
    }
}
