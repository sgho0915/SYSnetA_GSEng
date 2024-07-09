using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScreenManager;

public class SideMenuManager : MonoBehaviour
{
    public GameObject sideMenu;
    private Animator sideMenuAnim;
    private bool isSideMenuOpen = false;
    private Coroutine currentCoroutine = null; // 현재 실행 중인 코루틴을 추적
    private static readonly WaitForSeconds waitSec = new WaitForSeconds(0.6f);

    //UI 요소 추적을 위한 딕셔너리
    public Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    public GameObject sideMenuScrollView; // SideMenu_ScrollView
    public Transform sideMenuContent; // SideMenu_ScrollView의 Content
    public Transform hgContainerTransform; // Container_HighGroup의 Transform
    public Transform lgContainerTransform; // Container_LowGroup의 Transform

    private string selectedIid = null;
    private string selectedCid = null;


    public static SideMenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        sideMenuContent = sideMenuScrollView.transform.Find("Viewport/Content");
    }

    private void Start()
    {
        sideMenuAnim = sideMenu.GetComponent<Animator>();
    }

    public void SideMenuStateChange()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(IStateChange());
    }

    IEnumerator IStateChange()
    {
        if (!isSideMenuOpen)
        {
            LoadGroupAssets();
            ClientDatabase.isPolling = false;
            sideMenuAnim.SetBool("SideMenu_Idle", false);
            sideMenuAnim.SetBool("SideMenu_Open", true);
            sideMenu.GetComponent<DropShadow>().enabled = true;
            DecisionShowFPPScreen();
        }
        else
        {
            ClientDatabase.isPolling = false;
            sideMenuAnim.SetBool("SideMenu_Open", false);
            sideMenuAnim.SetBool("SideMenu_Close", true);
            sideMenu.GetComponent<DropShadow>().enabled = false;

            ObjectPool.Instance.CloseSideMenu();
            uiElements.Clear();
            for (int i = sideMenuContent.transform.childCount - 1; i >= 0; i--)
            {                
                Destroy(sideMenuContent.transform.GetChild(i).gameObject);
            }

            hgContainerTransform = null;
            lgContainerTransform = null;
        }
        isSideMenuOpen = !isSideMenuOpen;

        yield return waitSec;
        ClientDatabase.isPolling = true;
        currentCoroutine = null;
    }

    // 설정 변경 후 새로고침
    public IEnumerator RefreshUIAfterCleanup()
    {
        for (int i = sideMenuContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(sideMenuContent.transform.GetChild(i).gameObject);
        }
        ObjectPool.Instance.CloseSideMenu();
        uiElements.Clear();

        // 다음 프레임까지 기다립니다.
        yield return new WaitForSeconds(0.45f);

        // 이제 모든 오브젝트가 제거되었으므로 UI를 새로고침합니다.
        LoadGroupAssets();
    }

    public static GameObject FindChildStartingWithName(Transform parent, string nameStart)
    {
        foreach (Transform child in parent)
        {
            if (child.name.StartsWith(nameStart))
            {
                return child.gameObject;
            }
        }
        return null; // 찾지 못한 경우 null 반환
    }

    // 각 그룹 요소에 Button 생성
    public void LoadGroupAssets()
    {
        DataTable tableHighGroup = ClientDatabase.highGroupData.Tables[0];
        DataTable tableLowGroup = ClientDatabase.lowGroupData.Tables[0];
        DataTable tableController = ClientDatabase.controllerData.Tables[0];

        if(tableHighGroup.Rows.Count > 0)
        {
            foreach (DataRow highGroupRow in tableHighGroup.Rows)
            {
                string hgid = highGroupRow["FLD_HGID"].ToString();
                string hgName = highGroupRow["FLD_NAME"].ToString();
                string hgContainerName = $"Container_HighGroup_{hgid}";

                if (uiElements.TryGetValue(hgContainerName, out var highGroupContainer))
                {
                    // 기존 상위그룹 요소가 있으면 내용만 업데이트
                    GameObject btnHighGroup = FindChildStartingWithName(highGroupContainer.transform, "btn_HighGroup");
                    TextMeshProUGUI txtHighGroupName = FindChildStartingWithName(btnHighGroup.transform, "txt_HighGroupName").GetComponent<TextMeshProUGUI>();

                    GameObject activeImage = highGroupContainer.transform.Find($"btn_HighGroup_{hgid}/Img_Active").gameObject;
                    GameObject deactiveImage = highGroupContainer.transform.Find($"btn_HighGroup_{hgid}/Img_Deactive").gameObject;
                    deactiveImage.SetActive(true);
                    activeImage.SetActive(false);

                    hgContainerTransform = highGroupContainer.transform;
                    txtHighGroupName.text = hgName;
                }
                else
                {
                    // 상위그룹 요소 생성
                    GameObject newhighGroupContainer = ObjectPool.Instance.GetHighGroupContainerObject();
                    GameObject newbtnHighGroup = newhighGroupContainer.transform.Find("btn_HighGroup").gameObject;
                    TextMeshProUGUI newtxtHighGroupName = newbtnHighGroup.transform.Find("txt_HighGroupName").GetComponent<TextMeshProUGUI>();
                    AddHighGroupButtonListener(newbtnHighGroup, hgid); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
                    uiElements[hgContainerName] = newhighGroupContainer;

                    hgContainerTransform = newhighGroupContainer.transform;
                    newhighGroupContainer.name = $"Container_HighGroup_{hgid}";
                    newbtnHighGroup.name = $"btn_HighGroup_{hgid}";
                    newtxtHighGroupName.text = hgName;
                }

                // 상위그룹에 해당하는 하위그룹만 필터링하여 추가            
                var filteredLowGroups = tableLowGroup.AsEnumerable().Where(row => row["FLD_HGID"] != DBNull.Value && row.Field<short>("FLD_HGID").ToString() == hgid);

                if(tableLowGroup.Rows.Count > 0)
                {
                    // 상위그룹의 LowGroupContainer에 하위그룹 요소 생성
                    foreach (DataRow lowGroupRow in filteredLowGroups)
                    {
                        string lgid = lowGroupRow["FLD_LGID"].ToString();
                        string lgName = lowGroupRow["FLD_NAME"].ToString();
                        string lgContainerName = $"Container_LowGroup_{hgid}_{lgid}";

                        if (uiElements.TryGetValue(lgContainerName, out var lowGroupContainer))
                        {
                            // 기존 하위그룹 요소가 있으면 내용만 업데이트
                            GameObject btnLowGroup = FindChildStartingWithName(lowGroupContainer.transform, "btn_LowGroup");
                            TextMeshProUGUI txtLowGroupName = FindChildStartingWithName(btnLowGroup.transform, "txt_LowGroupName").GetComponent<TextMeshProUGUI>();
                            GameObject activeImage = lowGroupContainer.transform.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Active").gameObject;
                            GameObject deactiveImage = lowGroupContainer.transform.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Deactive").gameObject;
                            deactiveImage.SetActive(true);
                            activeImage.SetActive(false);
                            lgContainerTransform = lowGroupContainer.transform;
                            txtLowGroupName.text = lgName;
                        }
                        else
                        {
                            // 하위그룹 요소 생성
                            GameObject newlowGroupContainer = ObjectPool.Instance.GetLowGroupContainerObject();
                            GameObject newbtnLowGroup = newlowGroupContainer.transform.Find("btn_LowGroup").gameObject;
                            TextMeshProUGUI newtxtLowGroupName = newbtnLowGroup.transform.Find("txt_LowGroupName").GetComponent<TextMeshProUGUI>();
                            AddLowGroupButtonListener(newbtnLowGroup, hgid, lgid); // 이벤트 리스너 추가(화살표 변경 및 컨트롤러 숨김 핸들러)
                            uiElements[lgContainerName] = newlowGroupContainer;

                            lgContainerTransform = newlowGroupContainer.transform;
                            newlowGroupContainer.name = $"Container_LowGroup_{hgid}_{lgid}";
                            newbtnLowGroup.name = $"btn_LowGroup_{hgid}_{lgid}";
                            newtxtLowGroupName.text = lgName;
                        }

                        // 하위그룹에 해당하는 컨트롤러만 필터링하여 추가
                        var filteredControllers = tableController.AsEnumerable().Where(row => row["HGID"] != DBNull.Value && row["LGID"] != DBNull.Value &&
                                                                                        row.Field<int>("HGID").ToString() == hgid && row.Field<int>("LGID").ToString() == lgid);

                        if (tableController.Rows.Count > 0)
                        {
                            // 하위그룹에 컨트롤러 요소 생성
                            foreach (DataRow controllerRow in filteredControllers)
                            {
                                string iid = controllerRow["ID"].ToString();
                                string cid = controllerRow["CID"].ToString();
                                string cName = controllerRow["CNAME"].ToString();
                                string controllerContainerName = $"Container_Controller_{iid}_{cid}";

                                if (uiElements.TryGetValue(controllerContainerName, out var controllerContainer))
                                {
                                    // 기존 컨트롤러 요소가 있으면 내용만 업데이트
                                    GameObject btnController = FindChildStartingWithName(controllerContainer.transform, "btn_Controller");
                                    TextMeshProUGUI txtControllerName = FindChildStartingWithName(btnController.transform, "txt_ControllerName").GetComponent<TextMeshProUGUI>();

                                    txtControllerName.text = cName;
                                }
                                else
                                {
                                    // 컨트롤러 요소 생성
                                    GameObject newcontrollerContainer = ObjectPool.Instance.GetControllerContainerObject();
                                    GameObject newbtnController = newcontrollerContainer.transform.Find("btn_Controller").gameObject;
                                    TextMeshProUGUI newtxtControllerName = newbtnController.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>();
                                    AddControllerButtonListener(newcontrollerContainer, iid, cid); // 이벤트 리스너 추가(컨트롤러 클릭 시 핸들러)
                                    uiElements[controllerContainerName] = newcontrollerContainer;

                                    newcontrollerContainer.name = $"Container_Controller_{iid}_{cid}";
                                    newbtnController.name = $"btn_Controller_{iid}_{cid}";
                                    newtxtControllerName.text = cName;
                                }
                            }
                        }                        
                    }
                }                
            }

            foreach (var uiElement in uiElements)
            {
                // 'Container_HighGroup_'로 시작하지 않는 모든 오브젝트를 비활성화
                if (!uiElement.Key.StartsWith("Container_HighGroup_"))
                {
                    GameObject obj = uiElement.Value;
                    if (obj != null) // 오브젝트가 null이 아닌지 확인
                    {
                        obj.SetActive(false);
                    }
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(sideMenuContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgContainerTransform.GetComponent<RectTransform>());
            //LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        
    }

    // 상위그룹 버튼 리스너
    private void AddHighGroupButtonListener(GameObject btnHighGroup, string hgid)
    {
        btnHighGroup.GetComponent<Button>().onClick.AddListener(() => ToggleLowGroupVisibility(hgid));
    }

    // 하위그룹 버튼 리스너
    private void AddLowGroupButtonListener(GameObject btnLowGroup, string hgid, string lgid)
    {
        btnLowGroup.GetComponent<Button>().onClick.AddListener(() => ToggleControllerVisibility(hgid, lgid));
    }

    // 컨트롤러 버튼 리스너
    private void AddControllerButtonListener(GameObject newcontrollerContainer, string iid, string cid)
    {
        GameObject newbtnController = newcontrollerContainer.transform.Find("btn_Controller").gameObject;
        GameObject newbtnControllerStyle = newcontrollerContainer.transform.Find("btn_ControllerStyle").gameObject;

        newbtnController.GetComponent<Button>().onClick.RemoveAllListeners();
        newbtnController.GetComponent<Button>().onClick.AddListener(() => {
            // 현재 클릭된 컨트롤러를 선택된 상태로 업데이트
            selectedIid = iid;
            selectedCid = cid;

            // 모든 컨트롤러에 대해 상태 업데이트
            UpdateAllControllerStates();
            DetailView.Instance.CloseDetailView();
            //DetailView.Instance.OpenDetailView(newbtnController, iid, cid);
            DetailView.Instance.OpenDetailView(iid, cid);
        });
        newbtnControllerStyle.GetComponent<Button>().onClick.RemoveAllListeners();
        newbtnControllerStyle.GetComponent<Button>().onClick.AddListener(() => {
            // 현재 클릭된 컨트롤러를 선택된 상태로 업데이트
            selectedIid = iid;
            selectedCid = cid;

            // 모든 컨트롤러에 대해 상태 업데이트
            //UpdateAllControllerStates();

            if (ScreenManager.Instance.CurrentScreenState == ScreenState.FloorPlan)
            {
                DataTable tblController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
                bool isfppGen = false;
                // 상위그룹, 컨트롤러명 할당
                foreach (DataRow row in tblController.Rows)
                {
                    if (iid == row["ID"].ToString() && cid == row["CID"].ToString())
                    {
                        isfppGen = row["FPP_GEN"].ToString() == "1" ? true : false;
                    }
                }

                if(isfppGen)
                    FloorPlanManager.Instance.ModifyFPPSetting(iid, cid);
                else
                    FloorPlanManager.Instance.OpenFPPSetting(iid, cid);
            }
        });
    }

    // 화면 상태에 따른 컨트롤러 평면도 추가, 설정 버튼 활성, 비활성
    public void DecisionShowFPPScreen()
    {
        foreach(var controllerContainer in ObjectPool.Instance.ControllerContainerObjects)
        {
            if(ScreenManager.Instance.CurrentScreenState == ScreenState.FloorPlan)
                controllerContainer.transform.Find("btn_ControllerStyle").GetComponent<Button>().gameObject.SetActive(true);
            else
                controllerContainer.transform.Find("btn_ControllerStyle").GetComponent<Button>().gameObject.SetActive(false);

        }
    }

    // 상위그룹 버튼 핸들러
    private void ToggleLowGroupVisibility(string hgid)
    {
        Transform highGroupContainer = uiElements[$"Container_HighGroup_{hgid}"].transform;
        bool isActive = false; // 초기 상태 설정

        // btn_HighGroup 하위의 활성화/비활성화 이미지 토글
        GameObject activeImage = highGroupContainer.Find($"btn_HighGroup_{hgid}/Img_Active").gameObject;
        GameObject deactiveImage = highGroupContainer.Find($"btn_HighGroup_{hgid}/Img_Deactive").gameObject;

        isActive = !activeImage.activeSelf; // 현재 상태에 따라 반전

        activeImage.SetActive(isActive);
        deactiveImage.SetActive(!isActive);

        // Container_LowGroup로 시작하는 모든 자식 오브젝트의 활성화 상태 토글
        foreach (Transform child in highGroupContainer)
        {
            if (child.name.StartsWith("Container_LowGroup"))
            {
                child.gameObject.SetActive(isActive);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(sideMenuContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(highGroupContainer.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // 하위그룹 버튼 핸들러
    private void ToggleControllerVisibility(string hgid, string lgid)
    {
        Transform lowGroupContainer = uiElements[$"Container_LowGroup_{hgid}_{lgid}"].transform;
        bool isActive = false; // 초기 상태 설정

        // btn_LowGroup 하위의 활성화/비활성화 이미지 토글
        GameObject activeImage = lowGroupContainer.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Active").gameObject;
        GameObject deactiveImage = lowGroupContainer.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Deactive").gameObject;

        isActive = !activeImage.activeSelf; // 현재 상태에 따라 반전

        activeImage.SetActive(isActive);
        deactiveImage.SetActive(!isActive);

        // Container_Controller로 시작하는 모든 자식 오브젝트의 활성화 상태 토글
        foreach (Transform child in lowGroupContainer)
        {
            if (child.name.StartsWith("Container_Controller"))
            {
                child.gameObject.SetActive(isActive);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(sideMenuContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(lowGroupContainer.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(lowGroupContainer.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // 모든 컨트롤러의 상태 업데이트를 위한 함수
    private void UpdateAllControllerStates()
    {
        foreach (var item in uiElements)
        {
            if (item.Key.StartsWith("Container_Controller_"))
            {
                var parts = item.Key.Split('_');
                if (parts.Length >= 4) // Key 포맷이 예상대로인지 확인
                {
                    string currentIid = parts[2];
                    string currentCid = parts[3];
                    UpdateControllerState(currentIid, currentCid);
                }
            }
        }
    }

    // 개별 컨트롤러 상태 업데이트
    private void UpdateControllerState(string iid, string cid)
    {
        Transform controllerContainer = uiElements[$"Container_Controller_{iid}_{cid}"].transform;
        TextMeshProUGUI controllerName = controllerContainer.Find($"btn_Controller_{iid}_{cid}/txt_ControllerName").GetComponent<TextMeshProUGUI>();
        Image controllerStyle = controllerContainer.Find($"btn_ControllerStyle/Img_ControllerStyle").GetComponent<Image>();

        // 선택된 컨트롤러인지 확인
        bool isSelected = (iid == selectedIid && cid == selectedCid);

        // 선택 상태에 따라 색상 변경
        controllerName.color = isSelected ? Color.white : ColorUtility.TryParseHtmlString("#2D2D2D", out var color) ? color : Color.gray;
        controllerStyle.color = isSelected ? Color.white : ColorUtility.TryParseHtmlString("#999999", out var color2) ? color2 : Color.gray;
    }

    public void ResetControllerTextColors()
    {
        if (uiElements.Count > 0)
        {
            foreach (var item in uiElements)
            {
                // Container_Controller_로 시작하는 모든 아이템을 대상으로 함
                if (item.Key.StartsWith("Container_Controller_"))
                {
                    // 선택된 컨트롤러를 제외하고 색상 변경
                    if (!item.Key.Equals($"Container_Controller_{selectedIid}_{selectedCid}"))
                    {
                        Transform controllerContainer = item.Value.transform;
                        TextMeshProUGUI controllerName = controllerContainer.Find($"btn_Controller_{controllerContainer.name.Split('_')[2]}_{controllerContainer.name.Split('_')[3]}/txt_ControllerName").GetComponent<TextMeshProUGUI>();
                        Color color;
                        if (ColorUtility.TryParseHtmlString("#2D2D2D", out color))
                        {
                            controllerName.color = color;
                        }
                    }
                }
            }
            // 선택 상태 초기화
            selectedIid = null;
            selectedCid = null;
        }
    }
}