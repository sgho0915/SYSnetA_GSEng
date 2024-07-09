using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class UIRecycleViewCell<T> : MonoBehaviour
    {
        public RectTransform CachedRectTransform => GetComponent<RectTransform>();

        // ���� �����ϴ� ����Ʈ �׸��� �ε���
        public int Index { get; set; }

        // ���� ����
        public float Height
        {
            get { return CachedRectTransform.sizeDelta.y; }
            set
            {
                Vector2 sizeDelta = CachedRectTransform.sizeDelta;
                sizeDelta.y = value;
                CachedRectTransform.sizeDelta = sizeDelta;
            }
        }

        // ���� ������ �����ϴ� �޼���
        // ��ӹ��� Ŭ�������� ����
        public abstract void UpdateContent(T itemData);

        // ���� ���� ���� ��ġ
        public Vector2 Top
        {
            get
            {
                Vector3[] corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(corners);
                return CachedRectTransform.anchoredPosition + new Vector2(0.0f, corners[1].y);
            }
            set
            {
                Vector3[] corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(corners);
                CachedRectTransform.anchoredPosition = value - new Vector2(0.0f, corners[1].y);
            }
        }

        // ���� �Ʒ��� ���� ��ġ
        public Vector2 Bottom
        {
            get
            {
                Vector3[] corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(corners);
                return CachedRectTransform.anchoredPosition + new Vector2(0.0f, corners[3].y);
            }
            set
            {
                Vector3[] corners = new Vector3[4];
                CachedRectTransform.GetLocalCorners(corners);
                CachedRectTransform.anchoredPosition = value - new Vector2(0.0f, corners[3].y);
            }
        }
    }
}