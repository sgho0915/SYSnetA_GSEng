using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragControllerManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 offset; // 드래그 시작 시 마우스 포인터와 객체의 위치 차이를 저장할 변수


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ControllerStyleManager.bSetUse)
        {
            // 드래그 시작 시 자식 오브젝트를 GridLayoutGroup의 관리에서 제거
            transform.SetParent(transform.parent.parent);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ControllerStyleManager.bSetUse)
        {
            Vector2 globalMousePos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                transform.position = transform.parent.TransformPoint(globalMousePos + offset);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ControllerStyleManager.bSetUse)
        {
            if (ClientDatabase.isGridView)
            {
                // 드래그 종료 시, 드래그 위치에 따라 적절한 위치의 하이어라키로 오브젝트를 재삽입
                Transform parent = GameObject.Find("ContentGrid").transform; // 'Content'는 GridLayoutGroup이 있는 부모 오브젝트의 이름입니다.
                transform.SetParent(parent);

                // GridLayoutGroup의 자식으로 다시 추가
                int indexToPlace = CalculateIndexBasedOnPosition(transform.position, parent);
                
                transform.SetSiblingIndex(indexToPlace);

                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                // 드래그 종료 시, 드래그 위치에 따라 적절한 위치의 하이어라키로 오브젝트를 재삽입
                Transform parent = GameObject.Find("ContentList").transform; // 'Content'는 GridLayoutGroup이 있는 부모 오브젝트의 이름입니다.
                transform.SetParent(parent);

                // GridLayoutGroup의 자식으로 다시 추가
                int indexToPlace = CalculateIndexBasedOnPosition(transform.position, parent);
                transform.SetSiblingIndex(indexToPlace);

                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
        }
    }

    private int CalculateIndexBasedOnPosition(Vector3 position, Transform parent)
    {
        if (ClientDatabase.isGridView)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (position.x < parent.GetChild(i).position.x && position.y > parent.GetChild(i).position.y)
                {
                    return i;
                }
            }
            return parent.childCount - 1;
        }
        else
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                if (position.y > parent.GetChild(i).position.y)
                {
                    return i;
                }
            }
            return parent.childCount - 1;
        }
    }
}