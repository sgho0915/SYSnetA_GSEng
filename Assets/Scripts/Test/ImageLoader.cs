using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ImageLoader : MonoBehaviour
{
    public Image imageComponent; // Inspector에서 설정하는 Image 컴포넌트
    private string imageUrl = "http://systronics1.iptime.org:9907/img/bg_draw_sample1.png"; // 불러올 이미지의 URL
    private string urlwide = "https://wallpaperswide.com/download/beautiful_colorful_abstract_art-wallpaper-3440x1440.jpg";

    void Start()
    {
        StartCoroutine(LoadImage());
    }

    IEnumerator LoadImage()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                //Debug.Log(uwr.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                //Debug.Log($"{texture.width},  {texture.height}");
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                //Debug.Log($"{texture.width},  {texture.height}");
                imageComponent.sprite = sprite;
                // 캔버스 크기에 맞춰 이미지 크기 조정
                AdjustImageSize(texture.width, texture.height);
                //Debug.Log($"{texture.width},  {texture.height}");
            }
        }
    }

    void AdjustImageSize(float width, float height)
    {
        // 캔버스의 RectTransform을 가져옴
        RectTransform canvasRectTransform = imageComponent.canvas.GetComponent<RectTransform>();

        // 캔버스의 현재 가로 너비를 가져옴
        float canvasWidth = canvasRectTransform.rect.width;

        // 이미지의 원본 비율에 따라 새로운 높이 계산
        float newHeight = height * (canvasWidth / width);

        // 이미지 컴포넌트의 sizeDelta를 새로 계산된 너비와 높이로 설정
        imageComponent.rectTransform.sizeDelta = new Vector2(canvasWidth, newHeight);
    }
}
