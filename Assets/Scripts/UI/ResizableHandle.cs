using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeHandle : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    // 모서리 핸들의 타입을 나타내는 열거형 (TopLeft, TopRight, BottomLeft, BottomRight)
    public enum HandleType { TopLeft, TopRight, BottomLeft, BottomRight }
    public HandleType handleType; // 현재 핸들의 타입

    private RectTransform parentRectTransform; // 현재 객체의 부모 RectTransform
    private Vector2 originalSizeDelta; // 드래그 시작 시의 크기
    private Vector2 originalAnchoredPosition; // 드래그 시작 시의 위치
    private Vector2 startPointerPosition; // 드래그 시작 시의 마우스 포인터 위치

    private void Start()
    {
        // 시작 시 부모의 RectTransform 컴포넌트를 찾아 저장
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시의 데이터를 저장
        startPointerPosition = eventData.position;
        originalSizeDelta = parentRectTransform.sizeDelta;
        originalAnchoredPosition = parentRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그한 변화량 계산 후, 0.75를 곱해 조금 더 부드럽게 만듦
        Vector2 dragDifference = (eventData.position - startPointerPosition) * 0.75f;

        float newWidth = 0;
        float newHeight = 0;

        // 현재 핸들의 타입에 따라 크기 조절 로직을 다르게 적용
        switch (handleType)
        {
            case HandleType.TopLeft:
                newWidth = originalSizeDelta.x - dragDifference.x;
                newHeight = originalSizeDelta.y + dragDifference.y;
                break;
            case HandleType.TopRight:
                newWidth = originalSizeDelta.x + dragDifference.x;
                newHeight = originalSizeDelta.y + dragDifference.y;
                break;
            case HandleType.BottomLeft:
                newWidth = originalSizeDelta.x - dragDifference.x;
                newHeight = originalSizeDelta.y - dragDifference.y;
                break;
            case HandleType.BottomRight:
                newWidth = originalSizeDelta.x + dragDifference.x;
                newHeight = originalSizeDelta.y - dragDifference.y;
                break;
        }

        // 너비와 높이의 최소값 제한 (width는 165, height는 100)
        newWidth = Mathf.Clamp(newWidth, 165, float.MaxValue);
        newHeight = Mathf.Clamp(newHeight, 100, float.MaxValue);

        // 계산된 새로운 크기와 위치를 부모 RectTransform에 적용
        parentRectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        parentRectTransform.anchoredPosition = originalAnchoredPosition + new Vector2(dragDifference.x * 0.5f, dragDifference.y * 0.5f);
    }
}
