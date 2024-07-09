using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using MySqlConnector;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OpenDetailView : MonoBehaviour
{
    XMLParser xmlParser;

    private string converterObjName = string.Empty;     // 컨버터 오브젝트명
    private string controllerObjName = string.Empty;    // 컨트롤러 오브젝트명

    public static string fld_InterfaceId = string.Empty;   // 인터페이스 ID
    public static int fld_ControllerId = 0;                // 컨트롤러 ID
    
    public static string layoutName = string.Empty;     // 디테일뷰 레이아웃 이름
    public static string protocolKey = string.Empty;    // 프로토콜 키

    

    private void Awake()
    {
        xmlParser = XMLParser.Instance;
    }

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    Transform FindConverterObject(Transform startTransform)
    {
        Transform currentTransform = startTransform;
                
        while (currentTransform != null)
        {
            if (currentTransform.name.StartsWith("obj_Converter_"))
            {
                return currentTransform;
            }
            currentTransform = currentTransform.parent;
        }

        return null;
    }

    void OnButtonClick()
    {
        Transform targetTransform = FindConverterObject(transform);
        if (targetTransform != null)
        {
            controllerObjName = this.gameObject.transform.name;
            converterObjName = targetTransform.name;

            GetControllerDetailViewName(converterObjName, controllerObjName);
        }
    }

    /// <summary>
    /// "btn_Controller_" 버튼을 클릭했을 때 호출되는 메서드
    /// </summary>
    public void GetControllerDetailViewName(string converterObjName, string controllerObjName)
    {
        fld_InterfaceId = converterObjName.Substring("obj_Converter_".Length);
        fld_ControllerId = Convert.ToInt32(controllerObjName.Substring("btn_Controller_".Length));

        DataRow[] foundRows = ClientDatabase.SelectRowsFromTable(ClientDatabase.realTimeData, "TBL_REALTIME",
            $"ID = '{fld_InterfaceId}' AND CID = '{fld_ControllerId}'");

        if (foundRows.Length > 0)
        {
            protocolKey = foundRows[0]["PKEY"].ToString();
            //Debug.Log(protocolKey);
        }
        else
        {
            //Debug.LogError("해당 조건에 맞는 protocolKey 데이터를 찾을 수 없습니다.");
            return;
        }

        foundRows = ClientDatabase.SelectRowsFromTable(ClientDatabase.FetchProtocolList(), "TBL_PROTOCOL_LIST", $"`KEY` = '{protocolKey}'");

        if (foundRows.Length > 0)
        {
            layoutName = foundRows[0]["LAYOUT"].ToString();

            //if (string.IsNullOrEmpty(layoutName))
            //{
            //    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            //    ScreenManager.Instance.txt_PopUpMsg.text = "프로토콜 구현이 필요합니다.";
            //    Debug.LogError("layout name is null");
            //    return;
            //}

            OpenDetailViewScreen(layoutName);
        }
        else
        {
            //Debug.LogWarning("해당 조건에 맞는 layoutName데이터를 찾을 수 없습니다.");
        }
    }

    public void OpenDetailViewScreen(string layoutName)
    {
        // 경로에서 프리팹 로드(Old)
        //GameObject detailViewPrefab = Resources.Load<GameObject>("DetailViews/" + layoutName);
        GameObject detailViewPrefab = Resources.Load<GameObject>("DetailViews/DetailViewObj");
        if (detailViewPrefab == null)
        {
            //Debug.LogError("No prefab found for layout : " + layoutName);
            return;
        }

        // Canvas 게임 오브젝트 찾기
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas == null)
        {
            //Debug.LogError("No Canvas object found in the scene.");
            return;
        }

        // Canvas 하위의 MainScreen 찾기 (캐싱을 사용)
        Transform detailviewContainer = canvas.transform.Find("DetailView_Container");
        if (detailviewContainer == null)
        {
            //Debug.LogError("No MainScreen object found under Canvas.");
            return;
        }

        // MainScreen 하위에 프리팹 인스턴스화
        GameObject instance = Instantiate(detailViewPrefab, detailviewContainer);
        if (instance == null)
        {
            //Debug.LogError("Failed to instantiate prefab : " + layoutName);
        }
        else
        {
            // 인스턴스의 이름을 layout 값으로 변경하여 "(Clone)" 문자열 제거
            //instance.name = layoutName;
            instance.name = protocolKey;

            // MainScreen의 자식 오브젝트 중 obj_ControllerScrollViewGridContent 찾아 비활성화
            Transform objControllerScrollView = detailviewContainer.Find("obj_ControllerScrollViewGridContent");
            if (objControllerScrollView != null)
            {
                objControllerScrollView.gameObject.SetActive(false);
            }
            else
            {
                //Debug.LogWarning("obj_ConverterScrollView object not found under MainScreen.");
            }
        }

        // 디테일뷰에 해당하는 프로토콜키를 통해 XML 파싱 시작
        if(protocolKey == string.Empty)
        {
            //Debug.LogError("Cannot Found Protocol Key : " + protocolKey);
        }
        else 
        { 
            xmlParser.GetXML(protocolKey);            
        }
    }
}
