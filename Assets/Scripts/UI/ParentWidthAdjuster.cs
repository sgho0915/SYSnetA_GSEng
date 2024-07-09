using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ParentWidthAdjuster : MonoBehaviour
{
    private RectTransform parentRectTransform;
    private TextMeshProUGUI textComponent;

    public float padding = 5f; // 추가로 주고 싶은 패딩값 (옵션)

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        AdjustParentWidth();
    }

    public void AdjustParentWidth()
    {
        if (parentRectTransform != null)
        {
            // Text의 preferredWidth를 사용하여 텍스트의 너비를 얻음
            float textWidth = textComponent.preferredWidth;
            parentRectTransform.sizeDelta = new Vector2(textWidth + padding, parentRectTransform.sizeDelta.y);
        }
    }
}
