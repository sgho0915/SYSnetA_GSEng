using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour, IPointerClickHandler
{
    public Button targetButton;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (targetButton != null)
        {            
            // 사이드 메뉴 제어버튼의 이벤트 시작
            targetButton.onClick.Invoke();
        }
    }
}
