using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWidthAdjuster : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshProUGUI textComponent;

    public float padding = 5f; // 추가로 주고 싶은 패딩값 (옵션)

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(UpdateAdjustWidth());
    }

    public void AdjustWidth()
    {
        // Text의 preferredWidth를 사용하여 텍스트의 너비를 얻음
        float textWidth = textComponent.preferredWidth;
        rectTransform.sizeDelta = new Vector2(textWidth + padding, rectTransform.sizeDelta.y);        
    }

    IEnumerator UpdateAdjustWidth()
    {
        AdjustWidth();
        yield return new WaitForSeconds(1f);
        StartCoroutine(UpdateAdjustWidth());
    }
}
