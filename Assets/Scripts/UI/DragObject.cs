using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 offset; // 드래그 시작 시 마우스 포인터와 객체의 위치 차이를 저장할 변수

    private void Awake()
    {
        // 오브젝트의 태그가 "DragableObject"인지 확인
        if (gameObject.tag != "DragableObject")
        {
            // 태그가 다르면 경고 메세지 출력
            //Debug.LogWarning("This script should be attached to objects with the 'DragableObject' tag");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 마우스의 위치를 현재 오브젝트의 로컬 좌표계로 변환하고, 그 차이(offset)를 계산하여 저장
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out offset);
    }


    public void OnDrag(PointerEventData eventData)
    {
        DataTable tblController = ClientDatabase.FetchControllerData().Tables[0];
        string[] objName = this.gameObject.name.Split('_');
        string hgid = objName[1];
        string iid = objName[2];
        string cid = objName[3];
        bool isFix = false;
        foreach (DataRow row in tblController.Rows)
        {
            if (row["HGID"].ToString() == hgid && row["ID"].ToString() == iid && row["CID"].ToString() == cid)
            {
                isFix = row["FPP_FIX"].ToString() == "1" ? true : false;                
            }
        }

        if (isFix)
        {
            Vector2 globalMousePos;
            // 드래그 중일 때 마우스의 위치를 캔버스의 좌표계로 변환
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform.parent, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                // 캔버스의 RectTransform을 가져옴
                RectTransform canvasRect = transform.parent as RectTransform;

                // 캔버스 경계 내로 제한된 위치 계산
                globalMousePos.x = Mathf.Clamp(globalMousePos.x, -canvasRect.rect.width / 2 + (transform as RectTransform).rect.width / 2, canvasRect.rect.width / 2 - (transform as RectTransform).rect.width / 2);
                globalMousePos.y = Mathf.Clamp(globalMousePos.y, -canvasRect.rect.height / 2 + (transform as RectTransform).rect.height / 2, canvasRect.rect.height / 2 - (transform as RectTransform).rect.height / 2);

                // 위치를 업데이트
                transform.localPosition = globalMousePos + offset;
            }
        }        
    }


    public void OnEndDrag(PointerEventData eventData)
    {        
        if (this.gameObject.name.StartsWith("FPPMaxSingleItemInstance_"))
        {
            // 게임 오브젝트 이름을 '_' 기준으로 분리
            string[] parts = this.gameObject.name.Split('_');

            string hgid = parts[1];
            string iid = parts[2];
            string cid = parts[3];

            FloorPlanManager.Instance.UpdateFPPLocation(hgid, iid, cid, this.gameObject.GetComponent<RectTransform>().anchoredPosition.x, this.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        }
        
        //if (this.gameObject.name.StartsWith("FPPMinInstance_"))
        //{
        //    // 게임 오브젝트 이름을 '_' 기준으로 분리
        //    string[] parts = this.gameObject.name.Split('_');

        //    string hgid = parts[1];
        //    string iid = parts[2];
        //    string cid = parts[3];
        //    FloorPlanManager.Instance.UpdateFPPLocation(hgid, iid, cid, this.gameObject.GetComponent<RectTransform>().anchoredPosition.x, this.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        //}
    }
}
