using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWidthAdjuster : MonoBehaviour
{
    private RectTransform rectTransform;
    private TextMeshProUGUI textComponent;

    public float padding = 5f; // �߰��� �ְ� ���� �е��� (�ɼ�)

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
        // Text�� preferredWidth�� ����Ͽ� �ؽ�Ʈ�� �ʺ� ����
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
