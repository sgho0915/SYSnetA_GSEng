using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragControllerManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 offset; // �巡�� ���� �� ���콺 �����Ϳ� ��ü�� ��ġ ���̸� ������ ����


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ControllerStyleManager.bSetUse)
        {
            // �巡�� ���� �� �ڽ� ������Ʈ�� GridLayoutGroup�� �������� ����
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
                // �巡�� ���� ��, �巡�� ��ġ�� ���� ������ ��ġ�� ���̾��Ű�� ������Ʈ�� �����
                Transform parent = GameObject.Find("ContentGrid").transform; // 'Content'�� GridLayoutGroup�� �ִ� �θ� ������Ʈ�� �̸��Դϴ�.
                transform.SetParent(parent);

                // GridLayoutGroup�� �ڽ����� �ٽ� �߰�
                int indexToPlace = CalculateIndexBasedOnPosition(transform.position, parent);
                
                transform.SetSiblingIndex(indexToPlace);

                GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                // �巡�� ���� ��, �巡�� ��ġ�� ���� ������ ��ġ�� ���̾��Ű�� ������Ʈ�� �����
                Transform parent = GameObject.Find("ContentList").transform; // 'Content'�� GridLayoutGroup�� �ִ� �θ� ������Ʈ�� �̸��Դϴ�.
                transform.SetParent(parent);

                // GridLayoutGroup�� �ڽ����� �ٽ� �߰�
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