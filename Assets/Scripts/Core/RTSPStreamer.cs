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

            // �÷��̽�Ȧ�� ũ��� �ؽ�ó �ʱ�ȭ
            texture = new Texture2D(680, 480, TextureFormat.RGBA32, false);
            rawImage.texture = texture;

            PlayRTSP("rtsp://210.99.70.120:1935/live/cctv001.stream");
            // õ�Ƚ� ������ ��Ÿ� : rtsp://210.99.70.120:1935/live/cctv001.stream
            // ������ : rtsp://192.168.10.70:554/rtsp/streaming?channel=01&subtype=A
            // ��õ 26 : rtsp://youngdo6669.iptime.org:26554/profile3/media.smp
            // �� 156 : rtsp://hansolfarm.iptime.org:28156/profile3/media.smp
        }
    }

    // RTSP ��� �Լ� ȣ��
    private void PlayRTSP(string rtspUrl)
    {
        using (AndroidJavaClass rtspPlayer = new AndroidJavaClass("com.systronics.plugin.RTSPPlayer"))
        {
            rtspPlayer.CallStatic("PlayRTSP", rtspUrl);
        }
    }

    // ������ ������Ʈ �Լ�
    public void UpdateFrame(string frameDataBase64)
    {
        byte[] frameData = Convert.FromBase64String(frameDataBase64);

        // �ؽ�ó�� null�̰ų� ũ�Ⱑ rawImage�� ũ��� �ٸ��� �ؽ�ó�� �ٽ� ����
        if (texture == null || texture.width != (int)rawImage.rectTransform.rect.width || texture.height != (int)rawImage.rectTransform.rect.height)
        {
            int width = (int)rawImage.rectTransform.rect.width;
            int height = (int)rawImage.rectTransform.rect.height;

            texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            rawImage.texture = texture;
        }

        // ���ο� ������ �����ͷ� �ؽ�ó ������Ʈ
        texture.LoadRawTextureData(frameData);
        texture.Apply();
    }
}
