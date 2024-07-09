using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class ImageLoader : MonoBehaviour
{
    public Image imageComponent; // Inspector���� �����ϴ� Image ������Ʈ
    private string imageUrl = "http://systronics1.iptime.org:9907/img/bg_draw_sample1.png"; // �ҷ��� �̹����� URL
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
                // ĵ���� ũ�⿡ ���� �̹��� ũ�� ����
                AdjustImageSize(texture.width, texture.height);
                //Debug.Log($"{texture.width},  {texture.height}");
            }
        }
    }

    void AdjustImageSize(float width, float height)
    {
        // ĵ������ RectTransform�� ������
        RectTransform canvasRectTransform = imageComponent.canvas.GetComponent<RectTransform>();

        // ĵ������ ���� ���� �ʺ� ������
        float canvasWidth = canvasRectTransform.rect.width;

        // �̹����� ���� ������ ���� ���ο� ���� ���
        float newHeight = height * (canvasWidth / width);

        // �̹��� ������Ʈ�� sizeDelta�� ���� ���� �ʺ�� ���̷� ����
        imageComponent.rectTransform.sizeDelta = new Vector2(canvasWidth, newHeight);
    }
}
