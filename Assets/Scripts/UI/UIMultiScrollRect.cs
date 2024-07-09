using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

// 중첩된 스크롤 뷰에서의 드래그 이벤트를 적절하게 부모 또는 자식 스크롤 뷰로 라우팅 이벤트 처리를 위한 클래스
public class UIMultiScrollRect : ScrollRect
{
    // 부모 스크롤 뷰로 이벤트를 전달해야 하는지의 여부
    private bool routeToParent = false;

    // 주어진 타입의 이벤트 핸들러를 가진 부모 컴포넌트에 대해 액션을 수행하는 메서드
    private void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
    {
        Transform parent = transform.parent;
        // 모든 부모 객체를 순회
        while (parent != null)
        {
            // 해당 부모의 모든 컴포넌트 검색
            foreach (var component in parent.GetComponents<Component>())
            {
                // 해당 타입의 이벤트 핸들러를 가진 컴포넌트에 액션 수행
                if (component is T)
                    action((T)(IEventSystemHandler)component);
            }
            parent = parent.parent;
        }
    }

    // 드래그 시작 가능성 초기화
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
        base.OnInitializePotentialDrag(eventData);
    }

    // 드래그 이벤트 처리
    public override void OnDrag(PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        else
            base.OnDrag(eventData);
    }

    // 드래그 시작 이벤트 처리
    public override void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 방향에 따라 routeToParent 결정
        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
            routeToParent = true;
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
            routeToParent = true;
        else
            routeToParent = false;

        if (routeToParent)
            DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        else
            base.OnBeginDrag(eventData);
    }

    // 드래그 종료 이벤트 처리
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        else
            base.OnEndDrag(eventData);
        routeToParent = false;
    }
}
