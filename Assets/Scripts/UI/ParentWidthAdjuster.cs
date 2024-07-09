using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ParentWidthAdjuster : MonoBehaviour
{
    private RectTransform parentRectTransform;
    private TextMeshProUGUI textComponent;

    public float padding = 5f; // �߰��� �ְ� ���� �е��� (�ɼ�)

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
            // Text�� preferredWidth�� ����Ͽ� �ؽ�Ʈ�� �ʺ� ����
            float textWidth = textComponent.preferredWidth;
            parentRectTransform.sizeDelta = new Vector2(textWidth + padding, parentRectTransform.sizeDelta.y);
        }
    }
}
