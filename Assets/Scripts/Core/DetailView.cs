using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using System.Collections;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using E2C;
using DG.Tweening;
using System.Linq;
using System.Text.RegularExpressions;
using static ScreenManager;



public class DetailView : MonoBehaviour
{
    ScreenManager screenManager;
    ManufacturingData manufacturingData;
    ControllerStatus status;

    public GameObject detailViewScreen;
    public GameObject dVParentPrefab;
    public GameObject currentDV;
    public GameObject btn_DvMenuOpen;
    public GameObject btn_DvMenuClose;
    private GameObject aoWidgetScrollView;
    private GameObject widgetParentDL;
    public Transform mainWidgetScrollViewContent;
    public Transform aoWidgetScrollViewContent;
    public Transform graphElementScrollViewContent;
    public Transform groupElementScrollViewContent;
    public Transform setElementScrollViewContent;
    public TMP_FontAsset font_Pretendard_Bold;

    public GameObject setting_Numpad;
    public GameObject setting_Dropdown;
    public GameObject setting_Toggle;
    public GameObject setting_Timepicker;

    private Button btnOperateRun; // 우측상단 운전버튼
    private Button btnOperateDefrost; // 우측상단 제상버튼
    private Button btnShowAlarmPopUp; // 우측상단 경보 조회버튼
    private GameObject dvWidgetState;

    private string currentPkey = string.Empty;

    private Animator dvMenuAnim;
    private bool isDVMenuOpen = false;
    private Coroutine currentCoroutine = null; // 현재 실행 중인 코루틴을 추적
    private Coroutine dvUpdateCoroutine = null; // 현재 실행 중인 코루틴을 추적
    private Coroutine operateWaitCoroutine = null; // 현재 실행 중인 코루틴을 추적
    private static readonly WaitForSeconds waitSec = new(0.6f);
    private WaitForSeconds updateInterval = new(1); // DV UI 업데이트 간격
    private DataSet previousRealTimeData;
    private DataSet previousControllerData;
    private DataSet dVRealTimeData;
    private DataSet dVControllerData;
    private bool isDVOpen;
    public int[] parsedPollingData;
    private GameObject graphContainer;
    private Button btn_Graph;
    private GameObject graphActive;
    private GameObject graphDeactive;
    private GameObject graphParent;
    private GameObject dvElementScrollView;
    // 클래스 멤버 변수로 현재 선택된 groupID를 추적
    private string currentSelectedGroupID = string.Empty;
    string groupName = string.Empty;
    private string groupTitle = string.Empty;
    private string currentSelectedGroupTitle = string.Empty;
    public string currentSelectedIID = string.Empty;
    public string currentSelectedCID = string.Empty;
    private bool isGroupCanSetUpWhileRun = false; // 현재 선택된 그룹이 운전 중 제어가 가능한 그룹인지?
    public GameObject waitOperateComplete;
    private bool isOperateComplete = false;
    private int outageRecoveryAddr = 0;
    private int stopDelayAddr = 0;
    private DataTable tblController;
    private string showAddr = string.Empty;
    private WaitForSeconds sec0_1 = new WaitForSeconds(0.15f);
    private WaitForSeconds sec1 = new WaitForSeconds(1f);

    public static Dictionary<string, GameObject> dvTrendNormalWidgetInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> dvTrendAOWidgetInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> dvGraphElementInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> dvGroupElementInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> dvSetElementInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> currentDVInstances = new Dictionary<string, GameObject>();

    private Dictionary<string, Dictionary<string, object>> tagAttr = null;

    Color down3PercentColor = new Color(111 / 255f, 218 / 255f, 224 / 255f, 1f); // 6FDAE0, 투명도 1
    Color down5PercentColor = new Color(111 / 255f, 148 / 255f, 224 / 255f, 1f); // 6F94E0, 투명도 1
    Color up3PercentColor = new Color(255 / 255f, 181 / 255f, 126 / 255f, 1f); // FFB57E, 투명도 1
    Color up5PercentColor = new Color(255 / 255f, 130 / 255f, 126 / 255f, 1f); // FF827E, 투명도 1
    Color normalBlackColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1f); // 2D2D2D, 투명도 1
    Color normalTrendColor = new Color(116 / 255f, 178 / 255f, 8 / 255f, 1f); // 74B208, 투명도 1
    Color normalUnitColor = new Color(153 / 255f, 153 / 255f, 153 / 255f, 1f); // 999999로, 투명도 1




    // 풀무원 디테일뷰 현재값 그래프 분기처리
    string tempList = string.Empty;
    string pressureList = string.Empty;
    string elecList = string.Empty;
    bool dvLoad = false;

    public static DetailView Instance { get; private set; }

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

        screenManager = ScreenManager.Instance;
    }


    // 메인화면에서 디테일뷰 열기
    public void OpenDetailView(string iid, string cid)
    {
        currentSelectedIID = iid;
        currentSelectedCID = cid;

        Button waitOperateCompleteButton = waitOperateComplete.GetComponent<Button>();
        if (waitOperateCompleteButton != null)
        {
            waitOperateCompleteButton.onClick.AddListener(StopIOperateWaitComplete);
        }
        //Debug.Log($"OpenDetailView {currentSelectedIID}   {currentSelectedCID}");

        detailViewScreen.SetActive(true);


        screenManager.CurrentScreenState = ScreenManager.ScreenState.DetailView; // DetailView 상태 진입

        string dvName = $"DVParent_{iid}_{cid}";

        tblController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
        foreach (DataRow row in tblController.Rows)
        {
            showAddr = row["SHOW_ADDR"].ToString();
        }

        GameObject dvObj = null;
        dvObj = Instantiate(dVParentPrefab, detailViewScreen.gameObject.GetComponent<RectTransform>());

        aoWidgetScrollView = dvObj.transform.Find("Center/WidgetParent/AOWidget_ScrollView").gameObject;
        widgetParentDL = dvObj.transform.Find("Center/WidgetParent/DL").gameObject;

        dvObj.name = dvName;
        currentDVInstances[dvObj.name] = dvObj;
        currentDV = dvObj;

        Button btn_CloseDV = currentDV.transform.Find("Top/TopRight/btn_CloseDetailView").gameObject.GetComponent<Button>();
        btn_CloseDV.onClick.RemoveAllListeners();
        btn_CloseDV.onClick.AddListener(() => CloseDetailView());

        mainWidgetScrollViewContent = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content");
        aoWidgetScrollViewContent = currentDV.transform.Find("Center/WidgetParent/AOWidget_ScrollView/Viewport/Content");
        graphElementScrollViewContent = currentDV.transform.Find("Bottom/DVGraph/TrendElementContainer/Scroll View/Viewport/Content");
        groupElementScrollViewContent = currentDV.transform.Find("Bottom/DVCategoryScrollView/Viewport/Content");
        setElementScrollViewContent = currentDV.transform.Find("Bottom/DVElementScrollView/Viewport/Content");

        btnOperateRun = currentDV.gameObject.transform.Find("Top/TopRight/btn_OperateRun").GetComponent<Button>();
        btnOperateDefrost = currentDV.gameObject.transform.Find("Top/TopRight/btn_OperateDefrost").GetComponent<Button>();
        dvWidgetState = currentDV.gameObject.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State").gameObject;

        btnShowAlarmPopUp = currentDV.gameObject.transform.Find("Top/TopRight/btn_Alarm").GetComponent<Button>();

        btn_Graph = currentDV.transform.Find("Bottom/DVCategoryScrollView/Viewport/Content/DVCategory_Trend").gameObject.GetComponent<Button>();
        graphActive = currentDV.transform.Find("Bottom/DVCategoryScrollView/Viewport/Content/DVCategory_Trend/Img_On").gameObject;
        graphDeactive = currentDV.transform.Find("Bottom/DVCategoryScrollView/Viewport/Content/DVCategory_Trend/Img_Off").gameObject;
        graphParent = currentDV.transform.Find("Bottom/DVGraph").gameObject;
        graphContainer = currentDV.transform.Find("Bottom/DVGraph/GraphContainer").gameObject;
        dvElementScrollView = currentDV.transform.Find("Bottom/DVElementScrollView").gameObject;

        isDVOpen = true;
        if (dvUpdateCoroutine == null)
            dvUpdateCoroutine = StartCoroutine(DVUpdate(currentSelectedIID, currentSelectedCID));
        AddListenerForStaticButtons(iid, cid);
        btn_Graph.onClick.RemoveAllListeners();
        btn_Graph.onClick.AddListener(() =>
        {
            graphParent.SetActive(true);
            dvElementScrollView.SetActive(false);
            graphActive.SetActive(true);
            graphDeactive.SetActive(false);
            foreach (var element in dvGroupElementInstances.Values)
            {
                GameObject active = element.transform.Find("Img_On").gameObject;
                GameObject deactive = element.transform.Find("Img_Off").gameObject;
                active.SetActive(false);
                deactive.SetActive(true);
            }
        });

        btnShowAlarmPopUp.onClick.RemoveAllListeners();
        btnShowAlarmPopUp.onClick.AddListener(() =>
        {
            AlarmPopUpManager.Instance.ShowSpecificControllerAlarm(ClientDatabase.realTimeWarningData, currentSelectedIID, currentSelectedCID);
        });

        if (screenManager.CurrentScreenState == ScreenState.FloorPlan)
            FloorPlanManager.Instance.CloseFloorPlan();
        //Debug.Log($"DetailView : Load detailview info for >> interface : {iid}, controller : {cid}");
    }



    // 디테일뷰 닫기
    public void CloseDetailView()
    {
        if (dvUpdateCoroutine != null)
        {
            StopCoroutine(dvUpdateCoroutine);
            dvUpdateCoroutine = null;
        }

        for (int i = detailViewScreen.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(detailViewScreen.transform.GetChild(i).gameObject);
        }
        isDVOpen = false;
        currentSelectedIID = string.Empty;
        currentSelectedCID = string.Empty;
        currentSelectedGroupID = string.Empty;
        currentSelectedGroupTitle = string.Empty;
        outageRecoveryAddr = 0;
        stopDelayAddr = 0;

        tempList = string.Empty;
        pressureList = string.Empty;
        elecList = string.Empty;
        dvLoad = false;

        dvTrendNormalWidgetInstances.Clear();
        dvTrendAOWidgetInstances.Clear();
        dvGraphElementInstances.Clear();
        dvGroupElementInstances.Clear();
        dvSetElementInstances.Clear();
        currentDVInstances.Clear();

        ObjectPool.Instance.CloseDV();

        currentDV = null;

        detailViewScreen.SetActive(false);
    }

    IEnumerator DVUpdate(string iid, string cid)
    {
        //Debug.Log($"DetailView : DV UI element update {iid}, {cid}");
        while (true)
        {
            if (isDVOpen)
            {
                var newControllerData = ClientDatabase.FetchControllerData(iid, cid);
                var newRealTimeData = ClientDatabase.SearchRealTimeData(iid, cid);

                if (!IsDataSetEqual(newRealTimeData, previousRealTimeData) ||
                    !IsDataSetEqual(newControllerData, previousControllerData))
                {
                    previousRealTimeData = newRealTimeData;
                    previousControllerData = newControllerData;
                    dVRealTimeData = previousRealTimeData;
                    dVControllerData = previousControllerData;

                    string protocolKey = dVControllerData.Tables[0].Rows[0]["PKEY"].ToString();
                    currentPkey = protocolKey;
                    XMLParser.Instance.GetXML(protocolKey);
                    UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateUI(dVControllerData, iid, cid, protocolKey));
                }
            }
            yield return updateInterval;
        }
    }

    // DB 문자열 폴링 데이터 파싱
    private void ParsePollingData(string iid, string cid)
    {
        // UI 업데이트를 위한 액션 리스트 생성
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.DetailView)
        {
            // 조건에 맞는 행의 데이터를 활용해 UI를 업데이트한다.
            foreach (DataRow row in dVRealTimeData.Tables[0].Rows)
            {
                string[] stringData = row["STR_DATA"].ToString().Split(',');
                string pkey = row["PKEY"].ToString();
                parsedPollingData = new int[stringData.Length];

                for (int i = 0; i < stringData.Length; i++)
                {
                    if (!int.TryParse(stringData[i], out parsedPollingData[i])) // int로 변환 시도
                    {
                        //Debug.LogError($"Failed to parse value at index {i}: {stringData[i]}");
                        parsedPollingData[i] = -999; // 실패한 경우 기본값으로 -999를 사용합니다.
                    }
                }

                ProcessingTrendDataWithXML(iid, cid, pkey);
                ProcessingSetElementDataWithXML(iid, cid);
            }
        }
    }

    // 프로토콜의 XML 파일을 읽어 해당 트렌드 요소들의 값을 업데이트
    private void ProcessingTrendDataWithXML(string iid, string cid, string pkey)
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> attributes = XMLParser.Instance.GetTrendAltAttributes(XMLParser.Instance.xmlContent);
        bool firstGraphElementAdded = false; // 첫 번째 요소 추가 여부를 체크하는 지역 변수
        string finalUnit = string.Empty;
        string finalMultiply = string.Empty;
        foreach (var group in attributes)
        {
            foreach (var tag in group.Value)
            {
                string tagName = string.Empty;

                if (pkey == "PRDPC3HL20160317" || pkey == "STHCR5NF0513210100500205") // 운전, 제상 없음
                {
                    btnOperateDefrost.gameObject.SetActive(false);
                    btnOperateRun.gameObject.SetActive(false);
                    dvWidgetState.SetActive(false);
                }
                else if (pkey == "07152101-011-00-170" || pkey == "UC0224150200401102" || pkey == "UC0815120104610507") // 운전, 제상 있음
                {
                    btnOperateDefrost.gameObject.SetActive(true);
                    btnOperateRun.gameObject.SetActive(true);
                    dvWidgetState.SetActive(true);
                }
                else if (pkey == "02240601-001-00-208") // 운전 있음, 제상 없음
                {
                    btnOperateDefrost.gameObject.SetActive(false);
                    btnOperateRun.gameObject.SetActive(true);
                    dvWidgetState.SetActive(true);
                }
                else // 기본상태, 운전, 제상 있음
                {
                    btnOperateDefrost.gameObject.SetActive(true);
                    btnOperateRun.gameObject.SetActive(true);
                    dvWidgetState.SetActive(true);
                }


                if (tag.Value.TryGetValue("addr", out var addr) && tag.Value.TryGetValue("name", out var name) && tag.Value.TryGetValue("unit", out var unit) && tag.Value.TryGetValue("multiply", out var multiply))
                {
                    finalUnit = unit;
                    finalMultiply = multiply;
                    tagName = name;
                    if (tag.Value.TryGetValue("itemsSetAddr", out var itemsSetAddr))
                    {
                        int setIndex = int.Parse(itemsSetAddr) < 200 ? parsedPollingData[int.Parse(itemsSetAddr)] : parsedPollingData[int.Parse(itemsSetAddr) - 200];
                        string[] arrUnit = unit.Split(',');
                        string[] arrMultiply = multiply.Split(',');

                        // 위젯 인스턴스 생성
                        string widgetObjName = tag.Value.ContainsKey("ao") && tag.Value["ao"] == "y" ? $"DVWidget_AOValue_{iid}_{cid}_{addr}" : $"DVWidget_Value_{iid}_{cid}_{addr}";
                        GameObject widgetInstance;
                        Dictionary<string, GameObject> widgetObjDictionary = tag.Value.ContainsKey("ao") && tag.Value["ao"] == "y" ? dvTrendAOWidgetInstances : dvTrendNormalWidgetInstances;

                        // 위젯 인스턴스가 이미 존재하는 경우
                        if (widgetObjDictionary.TryGetValue(widgetObjName, out widgetInstance))
                        {
                            

                            // 위젯 인스턴스의 값을 업데이트
                            if (tag.Value.TryGetValue("setAddr", out var setAddr))
                            {
                                UpdateSetValWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply, setAddr);
                            }
                            else
                            {
                                UpdateWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply);
                            }
                        }
                        else
                        {
                            //// 위젯 인스턴스가 존재하지 않은 경우 새로운 위젯 인스턴스 생성 및 딕셔너리에 추가
                            //if (tag.Value.ContainsKey("hide") && tag.Value["hide"] == "y")
                            //    continue;
                            widgetInstance = tag.Value.ContainsKey("ao") && tag.Value["ao"] == "y" ? ObjectPool.Instance.GetDVTrendAOWidgetObject() : ObjectPool.Instance.GetDVTrendNormalWidgetObject();
                            widgetInstance.name = widgetObjName;
                            widgetObjDictionary[widgetObjName] = widgetInstance;

                            // 위젯 인스턴스 초기 설정
                            if (tag.Value.TryGetValue("setAddr", out var setAddr))
                                InitializeSetValWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply, setAddr);
                            else
                                InitializeWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply);
                        }
                    }
                    else
                    {
                        // 위젯 인스턴스 생성
                        string widgetObjName = tag.Value.ContainsKey("ao") && tag.Value["ao"] == "y" ? $"DVWidget_AOValue_{iid}_{cid}_{addr}" : $"DVWidget_Value_{iid}_{cid}_{addr}";
                        GameObject widgetInstance;
                        Dictionary<string, GameObject> widgetObjDictionary = tag.Value.ContainsKey("ao") && tag.Value["ao"] == "y" ? dvTrendAOWidgetInstances : dvTrendNormalWidgetInstances;

                        // 위젯 인스턴스가 이미 존재하는 경우
                        if (widgetObjDictionary.TryGetValue(widgetObjName, out widgetInstance))
                        {
                            // AO 현재 설정 명칭 반영
                            if (widgetInstance.name.Contains("AOValue"))
                            {
                                string[] arrAOWidgetName = widgetInstance.name.Split("_");
                                string aoAddr = arrAOWidgetName[4];                                
                                string aoSetTitleAddr = string.Empty;
                                string aoSetTitle = string.Empty;
                                int aoNameIdx = 0;
                                TextMeshProUGUI txtAOName = widgetInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                                                                
                                if (tag.Value.ContainsKey("setTitle"))
                                {
                                    aoSetTitleAddr = tag.Value["setTitle"];
                                    
                                    if (aoSetTitleAddr != string.Empty)
                                    {
                                        aoNameIdx = int.Parse(aoSetTitleAddr) < 200 ? parsedPollingData[int.Parse(aoSetTitleAddr)] : parsedPollingData[int.Parse(aoSetTitleAddr) - 200];
                                        
                                        if(tagAttr != null)
                                        {
                                            foreach (var aogroup in tagAttr)
                                            {
                                                var groupAttributes = (Dictionary<string, string>)aogroup.Value["attributes"];

                                                if (groupAttributes.TryGetValue("alt", out var altValue) && altValue == "set" && groupAttributes.TryGetValue("title", out var titleValue) && titleValue == "AO")
                                                {
                                                    var stateTags = (Dictionary<string, Dictionary<string, string>>)aogroup.Value["tags"];
                                                    foreach (var tagAddrobj in stateTags)
                                                    {
                                                        var tagAttributes = tagAddrobj.Value;
                                                        if (tagAttributes.TryGetValue("addr", out var aoTitleAddr) && tagAttributes.TryGetValue("items", out var aoNames))
                                                        {
                                                            if (aoTitleAddr == aoSetTitleAddr)
                                                            {
                                                                string[] aoSetName = aoNames.Split(",");
                                                                aoSetTitle = aoSetName[aoNameIdx];
                                                            }
                                                        }
                                                        
                                                    }
                                                }                                                
                                            }
                                            tagName = aoSetTitle;
                                            txtAOName.text = tagName;
                                        }
                                        else
                                        {
                                            Debug.LogError($"tagAttr가 null임");
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError($"인스턴스 {aoAddr}에 대한 adSetAddr를 찾을 수 없음");
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"인스턴스 {aoAddr}에 대한 itemsSetAddr을 찾을 수 없음");
                                }
                            }

                            // 위젯 인스턴스의 값을 업데이트
                            if (tag.Value.TryGetValue("setAddr", out var setAddr))
                            {
                                UpdateSetValWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply, setAddr);
                            }
                            else
                            {
                                UpdateWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply);
                            }
                        }
                        else
                        {
                            //// 위젯 인스턴스가 존재하지 않은 경우 새로운 위젯 인스턴스 생성 및 딕셔너리에 추가
                            //if (tag.Value.ContainsKey("hide") && tag.Value["hide"] == "y")
                            //    continue;
                            widgetInstance = tag.Value.ContainsKey("ao") && tag.Value["ao"] == "y" ? ObjectPool.Instance.GetDVTrendAOWidgetObject() : ObjectPool.Instance.GetDVTrendNormalWidgetObject();
                            widgetInstance.name = widgetObjName;
                            widgetObjDictionary[widgetObjName] = widgetInstance;

                            // 위젯 인스턴스 초기 설정
                            if (tag.Value.TryGetValue("setAddr", out var setAddr))
                                InitializeSetValWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply, setAddr);
                            else
                                InitializeWidgetInstance(widgetInstance, addr, tagName, finalUnit, finalMultiply);
                        }
                    }
                }
            }
        }

        if (dvTrendAOWidgetInstances.Count == 0)
        {
            aoWidgetScrollView.SetActive(false);
            widgetParentDL.SetActive(false);
        }

        if (!dvLoad)
        {
            List<string> pulmuoneNameList = new List<string>();
            foreach (var group in attributes)
            {
                foreach (var tag in group.Value)
                {
                    string tagName = tag.Key;

                    if (tag.Value.TryGetValue("addr", out var addr) && tag.Value.TryGetValue("name", out var name) && tag.Value.TryGetValue("unit", out var unit) && tag.Value.TryGetValue("multiply", out var multiply))
                    {
                        finalUnit = unit;
                        finalMultiply = multiply;
                        if (tag.Value.TryGetValue("itemsSetAddr", out var itemsSetAddr))
                        {
                            int setIndex = int.Parse(itemsSetAddr) < 200 ? parsedPollingData[int.Parse(itemsSetAddr)] : parsedPollingData[int.Parse(itemsSetAddr) - 200];
                            string[] arrUnit = unit.Split(',');
                            string[] arrMultiply = multiply.Split(',');

                            // 트렌드 요소 버튼 인스턴스 생성
                            if (pkey == "UC0224150200401102") // 풀무원 예외처리
                            {
                                if (addr == "222" || addr == "235")
                                    tempList += $"_{addr}";
                                if (addr == "231" || addr == "232")
                                    pressureList += $"_{addr}";
                                if (addr == "240" || addr == "243" || addr == "246" || addr == "249" || addr == "252")
                                    elecList += $"_{addr}";
                            }
                            else
                            {
                                string graphElementObjName = $"TrendElement{addr}";
                                GameObject graphElementInstance;
                                Dictionary<string, GameObject> graphElementDictionary = dvGraphElementInstances;


                                if (!graphElementDictionary.TryGetValue(graphElementObjName, out graphElementInstance))
                                {
                                    if (pkey == "UC0224150200401102") // 풀무원 예외처리
                                    {
                                        if (addr == "223" || addr == "236" || addr == "233" || addr == "234" ||
                                            addr == "242" || addr == "245" || addr == "248" || addr == "251" || addr == "254" ||
                                            addr == "215" || addr == "216" || addr == "217" || addr == "218")
                                            continue;
                                    }
                                    if (pkey == "PRDPC3HL20160317" && tag.Value.ContainsKey("hide"))
                                        continue;

                                    graphElementInstance = ObjectPool.Instance.GetDVGraphElementObject();
                                    graphElementInstance.name = graphElementObjName;
                                    graphElementDictionary[graphElementObjName] = graphElementInstance;
                                    GameObject activeObj = graphElementInstance.transform.Find("Active").gameObject;
                                    GameObject deactiveObj = graphElementInstance.transform.Find("Deactive").gameObject;

                                    TextMeshProUGUI activeName = graphElementInstance.transform.Find("Active/txt_ActiveName").GetComponent<TextMeshProUGUI>();
                                    TextMeshProUGUI deActiveName = graphElementInstance.transform.Find("Deactive/txt_DeactiveName").GetComponent<TextMeshProUGUI>();
                                    activeName.text = tagName;
                                    deActiveName.text = tagName;

                                    // 모든 인스턴스에 리스너 할당
                                    graphElementInstance.GetComponent<Button>().onClick.AddListener(() =>
                                    {
                                        // 클릭된 요소만 활성화하고 나머지는 비활성화
                                        foreach (var element in dvGraphElementInstances.Values)
                                        {
                                            GameObject active = element.transform.Find("Active").gameObject;
                                            GameObject deactive = element.transform.Find("Deactive").gameObject;

                                            if (element == graphElementInstance)
                                            {
                                                // 현재 클릭된 인스턴스
                                                active.SetActive(true);
                                                deactive.SetActive(false);
                                            }
                                            else
                                            {
                                                // 나머지 인스턴스들
                                                active.SetActive(false);
                                                deactive.SetActive(true);
                                            }
                                        }

                                        // 클릭된 요소에 대한 작업 수행
                                        DrawTrendGraph(iid, cid, addr, name, finalUnit);
                                    });

                                    if (!firstGraphElementAdded)
                                    {
                                        // 첫 번째 요소에 대한 리스너 함수 수행
                                        DrawTrendGraph(iid, cid, addr, name, finalUnit);
                                        activeObj.SetActive(true);
                                        deactiveObj.SetActive(false);
                                        firstGraphElementAdded = true; // 첫 번째 요소가 처리되었음을 표시
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 트렌드 요소 버튼 인스턴스 생성
                            if (pkey == "UC0224150200401102") // 풀무원 예외처리
                            {
                                if (addr == "222" || addr == "235")
                                    tempList += $"_{addr}";
                                if (addr == "231" || addr == "232")
                                    pressureList += $"_{addr}";
                                if (addr == "240" || addr == "243" || addr == "246" || addr == "249" || addr == "252")
                                    elecList += $"_{addr}";
                            }
                            else
                            {
                                string graphElementObjName = $"TrendElement{addr}";
                                GameObject graphElementInstance;
                                Dictionary<string, GameObject> graphElementDictionary = dvGraphElementInstances;


                                if (!graphElementDictionary.TryGetValue(graphElementObjName, out graphElementInstance))
                                {
                                    if (pkey == "UC0224150200401102") // 풀무원 예외처리
                                    {
                                        if (addr == "223" || addr == "236" || addr == "233" || addr == "234" ||
                                            addr == "242" || addr == "245" || addr == "248" || addr == "251" || addr == "254" ||
                                            addr == "215" || addr == "216" || addr == "217" || addr == "218")
                                            continue;
                                    }
                                    if (pkey == "PRDPC3HL20160317" && tag.Value.ContainsKey("hide"))
                                        continue;

                                    graphElementInstance = ObjectPool.Instance.GetDVGraphElementObject();
                                    graphElementInstance.name = graphElementObjName;
                                    graphElementDictionary[graphElementObjName] = graphElementInstance;
                                    GameObject activeObj = graphElementInstance.transform.Find("Active").gameObject;
                                    GameObject deactiveObj = graphElementInstance.transform.Find("Deactive").gameObject;

                                    TextMeshProUGUI activeName = graphElementInstance.transform.Find("Active/txt_ActiveName").GetComponent<TextMeshProUGUI>();
                                    TextMeshProUGUI deActiveName = graphElementInstance.transform.Find("Deactive/txt_DeactiveName").GetComponent<TextMeshProUGUI>();
                                    activeName.text = tagName;
                                    deActiveName.text = tagName;

                                    // 모든 인스턴스에 리스너 할당
                                    graphElementInstance.GetComponent<Button>().onClick.AddListener(() =>
                                    {
                                        // 클릭된 요소만 활성화하고 나머지는 비활성화
                                        foreach (var element in dvGraphElementInstances.Values)
                                        {
                                            GameObject active = element.transform.Find("Active").gameObject;
                                            GameObject deactive = element.transform.Find("Deactive").gameObject;

                                            if (element == graphElementInstance)
                                            {
                                                // 현재 클릭된 인스턴스
                                                active.SetActive(true);
                                                deactive.SetActive(false);
                                            }
                                            else
                                            {
                                                // 나머지 인스턴스들
                                                active.SetActive(false);
                                                deactive.SetActive(true);
                                            }
                                        }

                                        // 클릭된 요소에 대한 작업 수행
                                        DrawTrendGraph(iid, cid, addr, name, finalUnit);
                                    });

                                    if (!firstGraphElementAdded)
                                    {
                                        // 첫 번째 요소에 대한 리스너 함수 수행
                                        //Debug.Log($"iid:{iid}, cid:{cid}, addr:{addr}, name:{name}, unit:{finalUnit}");
                                        DrawTrendGraph(iid, cid, addr, name, finalUnit);
                                        activeObj.SetActive(true);
                                        deactiveObj.SetActive(false);
                                        firstGraphElementAdded = true; // 첫 번째 요소가 처리되었음을 표시
                                    }
                                }
                            }
                        }

                    }
                }
                dvLoad = true;
                pulmuoneNameList.Add(tempList);
                pulmuoneNameList.Add(pressureList);
                pulmuoneNameList.Add(elecList);
            }

            if (pkey == "UC0224150200401102")
            {
                foreach (var addrList in pulmuoneNameList)
                {
                    string graphElementObjName = $"TrendElement{addrList}";
                    GameObject graphElementInstance;
                    Dictionary<string, GameObject> graphElementDictionary = dvGraphElementInstances;


                    if (!graphElementDictionary.TryGetValue(graphElementObjName, out graphElementInstance))
                    {
                        graphElementInstance = ObjectPool.Instance.GetDVGraphElementObject();
                        graphElementInstance.name = graphElementObjName;
                        graphElementDictionary[graphElementObjName] = graphElementInstance;
                        GameObject activeObj = graphElementInstance.transform.Find("Active").gameObject;
                        GameObject deactiveObj = graphElementInstance.transform.Find("Deactive").gameObject;

                        TextMeshProUGUI activeName = graphElementInstance.transform.Find("Active/txt_ActiveName").GetComponent<TextMeshProUGUI>();
                        TextMeshProUGUI deActiveName = graphElementInstance.transform.Find("Deactive/txt_DeactiveName").GetComponent<TextMeshProUGUI>();

                        if (addrList == tempList)
                        {
                            activeName.text = "온도";
                            deActiveName.text = "온도";
                        }
                        else if (addrList == pressureList)
                        {
                            activeName.text = "저압/고압";
                            deActiveName.text = "저압/고압";
                        }
                        else if (addrList == elecList)
                        {
                            activeName.text = "전력량";
                            deActiveName.text = "전력량";
                        }


                        // 모든 인스턴스에 리스너 할당
                        graphElementInstance.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            // 클릭된 요소만 활성화하고 나머지는 비활성화
                            foreach (var element in dvGraphElementInstances.Values)
                            {
                                GameObject active = element.transform.Find("Active").gameObject;
                                GameObject deactive = element.transform.Find("Deactive").gameObject;

                                if (element == graphElementInstance)
                                {
                                    // 현재 클릭된 인스턴스
                                    active.SetActive(true);
                                    deactive.SetActive(false);
                                }
                                else
                                {
                                    // 나머지 인스턴스들
                                    active.SetActive(false);
                                    deactive.SetActive(true);
                                }
                            }

                            // 클릭된 요소에 대한 작업 수행
                            DrawMultipleMergedTrendGraph(iid, cid, graphElementInstance.name);
                        });

                        if (!firstGraphElementAdded)
                        {
                            // 첫 번째 요소에 대한 리스너 함수 수행
                            DrawMultipleMergedTrendGraph(iid, cid, graphElementInstance.name);
                            activeObj.SetActive(true);
                            deactiveObj.SetActive(false);
                            firstGraphElementAdded = true; // 첫 번째 요소가 처리되었음을 표시
                        }
                    }
                }
            }
        }
    }



    // 프로토콜의 XML 파일을 읽어 하단 메뉴에 들어갈 설정 메뉴들과 메뉴에 속하는 요소들의 값을 업데이트
    private void ProcessingSetElementDataWithXML(string iid, string cid)
    {
        // XML에서 모든 속성을 가져오는 함수 호출
        var allAttributes = XMLParser.Instance.GetAllXMLAttributes(XMLParser.Instance.xmlContent);


        if (allAttributes.ContainsKey("groups"))
        {
            var groups = (Dictionary<string, Dictionary<string, object>>)allAttributes["groups"];
            tagAttr = groups;
            foreach (var group in groups)
            {
                var groupAttributes = (Dictionary<string, string>)group.Value["attributes"];

                // 'alt' 속성이 'trend'인 경우 인스턴스 생성을 건너뜁니다.
                if (groupAttributes.TryGetValue("alt", out var altValue) && altValue == "trend")
                {
                    continue;
                }

                // group이 상태값 관련(시스템상태, 에러상태, 출력, 입력, 남은시간, 적산시간 등)인 경우에 대한 처리
                if (groupAttributes.TryGetValue("alt", out var altValue2) && altValue2 == "system" || altValue2 == "alarm" || altValue2 == "output" || altValue2 == "input" || altValue2 == "state")
                {
                    // 각 group 태그의 속성 이름과 값 출력
                    foreach (var attribute in groupAttributes)
                    {
                        if (groupAttributes.TryGetValue("id", out var groupID) && groupAttributes.TryGetValue("title", out var title))
                        {
                            groupName = groupID;
                            groupTitle = title;

                            // 하단 메뉴 group 태그 요소 버튼 인스턴스 생성
                            string groupElementObjName = $"DVCategory_{groupID}_{iid}_{cid}";
                            GameObject groupElementInstance;
                            Dictionary<string, GameObject> groupElementDictionary = dvGroupElementInstances;

                            if (!groupElementDictionary.TryGetValue(groupElementObjName, out groupElementInstance))
                            {
                                groupElementInstance = ObjectPool.Instance.GetDVGroupElementObject();
                                groupElementInstance.name = groupElementObjName;
                                groupElementDictionary[groupElementObjName] = groupElementInstance;

                                TextMeshProUGUI activeName = groupElementInstance.transform.Find("Img_On/txt_CategoryName").GetComponent<TextMeshProUGUI>();
                                TextMeshProUGUI deActiveName = groupElementInstance.transform.Find("Img_Off/txt_CategoryName").GetComponent<TextMeshProUGUI>();
                                activeName.text = title;
                                deActiveName.text = title;


                                // 모든 group 태그 요소 버튼 인스턴스에 리스너 할당
                                groupElementInstance.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    graphActive.SetActive(false);
                                    graphDeactive.SetActive(true);
                                    graphParent.SetActive(false);
                                    dvElementScrollView.SetActive(true);

                                    // 현재 클릭된 groupID 저장
                                    currentSelectedGroupID = groupID;
                                    currentSelectedGroupTitle = title;

                                    // 클릭된 요소만 활성화하고 나머지는 비활성화
                                    foreach (var groupElement in dvGroupElementInstances.Values)
                                    {
                                        GameObject active = groupElement.transform.Find("Img_On").gameObject;
                                        GameObject deactive = groupElement.transform.Find("Img_Off").gameObject;

                                        if (groupElement == groupElementInstance)
                                        {
                                            // 현재 클릭된 인스턴스
                                            active.SetActive(true);
                                            deactive.SetActive(false);
                                        }
                                        else
                                        {
                                            // 나머지 인스턴스들
                                            active.SetActive(false);
                                            deactive.SetActive(true);
                                        }
                                    }

                                    foreach (var setElementEntry in dvSetElementInstances)
                                    {
                                        var setElement = setElementEntry.Value;
                                        // setElement의 이름에서 groupID 추출
                                        var setElementNameParts = setElementEntry.Key.Split('_');
                                        if (setElementNameParts.Length > 1)
                                        {
                                            var setElementGroupID = setElementNameParts[1]; // 이름 규칙이 "SetElement_{groupID}_{addr}"라고 가정
                                                                                            // 현재 선택된 groupID와 일치하는지 확인
                                            if (setElementGroupID == currentSelectedGroupID)
                                            {
                                                setElement.SetActive(true);
                                            }
                                            else
                                            {
                                                setElement.SetActive(false);
                                            }
                                        }
                                    }

                                });
                            }
                        }
                    }

                    // tag 태그의 속성들 출력
                    var stateTags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    foreach (var tag in stateTags)
                    {
                        var stateTagAttributes = tag.Value; // 각 태그의 속성들을 담은 Dictionary

                        // 제상을 사용하지 않는 컨트롤러는 제상 버튼을 숨김
                        if (stateTagAttributes.TryGetValue("defrost", out var defrost2) && defrost2 == "")
                        {
                            GameObject btnDefrost = currentDV.gameObject.transform.Find("Top/TopRight/btn_OperateDefrost").gameObject;
                            GameObject dl = currentDV.gameObject.transform.Find("Top/TopRight/DL_BetweenDefrostNRun").gameObject;
                            btnDefrost.SetActive(false);
                            dl.SetActive(false);
                        }

                        // style이 bitState인 경우에 대한 처리
                        if (stateTagAttributes["style"] == "bitState")
                        {
                            //style 값에 따른 분기 처리
                            string bitStateElementName = $"bitStateElement_{groupName}_{stateTagAttributes["addr"]}_{iid}_{cid}";
                            GameObject bitStateElementInstance = null;
                            if (dvSetElementInstances.TryGetValue(bitStateElementName, out var valueStateElementInstance))
                            {
                                // 인스턴스가 존재하는 경우, 새로운 값으로 업데이트
                                UpdateSetElementInstance(iid, cid, valueStateElementInstance, stateTagAttributes);
                            }
                            else
                            {
                                try
                                {
                                    // "bit" 뒤에 숫자만 있는 속성을 필터링
                                    var bitAttributes = stateTagAttributes
                                        .Where(k => Regex.IsMatch(k.Key, @"^bit\d+$"))
                                        .ToList();

                                    if (bitAttributes.Any())
                                    {
                                        foreach (var bitAttr in bitAttributes)
                                        {
                                            // "bit" 다음 오는 숫자를 인덱스로 사용
                                            string bitNumberStr = bitAttr.Key.Replace("bit", "");
                                            string powerOpBitIndex = string.Empty;
                                            string defrostOpBitIndex = string.Empty;

                                            // 운전 및 제상에 대한 비트 인덱스 할당
                                            if (stateTagAttributes.TryGetValue("powerOP", out var powerOPBit))
                                                powerOpBitIndex = stateTagAttributes["powerOP"].Replace("bit", "");
                                            if (stateTagAttributes.TryGetValue("defrostOP", out var defrostOPBit))
                                                defrostOpBitIndex = stateTagAttributes["defrostOP"].Replace("bit", "");

                                            if (int.TryParse(bitNumberStr, out var bitIndex))
                                            {
                                                string specificSetName = $"{bitStateElementName}_Bit{bitIndex}";
                                                var bitName = bitAttr.Value;
                                                if (!dvSetElementInstances.ContainsKey(specificSetName))
                                                {
                                                    bitStateElementInstance = ObjectPool.Instance.GetDVSetElementObject_Toggle(); // 가정: 토글 오브젝트를 올바르게 인스턴스화
                                                    bitStateElementInstance.name = specificSetName;

                                                    // 비트 값에 따른 토글 상태 설정
                                                    int rawValue = int.Parse(stateTagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(stateTagAttributes["addr"])] : parsedPollingData[int.Parse(stateTagAttributes["addr"]) - 200];
                                                    bool isBitSet = (rawValue & (1 << bitIndex)) != 0;
                                                    string currentStr = isBitSet ? stateTagAttributes["val_on"] : stateTagAttributes["val_off"];
                                                    bitStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = currentStr;
                                                    bitStateElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = bitName;

                                                    dvSetElementInstances[specificSetName] = bitStateElementInstance;


                                                }

                                                // 운전 비트 확인 및 운전 버튼 제어 리스너 추가
                                                if (stateTagAttributes.TryGetValue("power", out var power) && powerOpBitIndex == bitIndex.ToString())
                                                {
                                                    int rawValue = int.Parse(stateTagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(stateTagAttributes["addr"])] : parsedPollingData[int.Parse(stateTagAttributes["addr"]) - 200];
                                                    bool isPowerBitSet = (rawValue & (1 << int.Parse(powerOpBitIndex))) != 0;
                                                    string currentStr = isPowerBitSet ? stateTagAttributes["val_on"] : stateTagAttributes["val_off"];
                                                    btnOperateRun.onClick.RemoveAllListeners();
                                                    btnOperateRun.onClick.AddListener(() => OperateRun(stateTagAttributes["addr"], iid, cid, bitName, int.Parse(powerOpBitIndex), status.IsRun, currentStr, stateTagAttributes["val_on"], stateTagAttributes["val_off"], stateTagAttributes));
                                                }
                                                // 제상 비트 확인 및 제상 버튼 제어 리스너 추가
                                                if (stateTagAttributes.TryGetValue("defrost", out var defrost) && defrostOpBitIndex == bitIndex.ToString())
                                                {
                                                    int rawValue = int.Parse(stateTagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(stateTagAttributes["addr"])] : parsedPollingData[int.Parse(stateTagAttributes["addr"]) - 200];
                                                    bool isDefrostBitSet = (rawValue & (1 << int.Parse(defrostOpBitIndex))) != 0;
                                                    string currentStr = isDefrostBitSet ? stateTagAttributes["val_on"] : stateTagAttributes["val_off"];
                                                    btnOperateDefrost.onClick.RemoveAllListeners();
                                                    btnOperateDefrost.onClick.AddListener(() => OperateDefrost(stateTagAttributes["addr"], iid, cid, bitName, int.Parse(defrostOpBitIndex), status.IsDefrost, currentStr, stateTagAttributes["val_on"], stateTagAttributes["val_off"], stateTagAttributes));
                                                }
                                                UpdateSetElementInstance(iid, cid, dvSetElementInstances[specificSetName], stateTagAttributes);
                                            }
                                            else
                                            {
                                                //Debug.LogError($"Invalid bit index format: {bitAttr.Key}");
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Debug.LogError($"Error processing toggle for addr {stateTagAttributes["addr"]}: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            if (stateTagAttributes.TryGetValue("outageReturn", out var outageReturn))
                            {
                                outageRecoveryAddr = int.Parse(outageReturn);
                            }

                            if (stateTagAttributes.TryGetValue("stopDelay", out var stopDelay))
                            {
                                stopDelayAddr = int.Parse(stopDelay);
                            }
                            //Debug.Log($"정전복귀 addr : {outageRecoveryAddr} 정지지연 addr : {stopDelayAddr}");

                            //style 값에 따른 분기 처리
                            string valueStateElementName = $"valueStateElement_{groupName}_{stateTagAttributes["addr"]}_{iid}_{cid}";

                            if (dvSetElementInstances.TryGetValue(valueStateElementName, out var valueStateElementInstance))
                            {
                                // 인스턴스가 존재하는 경우, 새로운 값으로 업데이트
                                UpdateSetElementInstance(iid, cid, valueStateElementInstance, stateTagAttributes);
                            }
                            else
                            {
                                try
                                {
                                    // 모드버스에서 읽어온 값
                                    int rawValue = int.Parse(stateTagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(stateTagAttributes["addr"])] : parsedPollingData[int.Parse(stateTagAttributes["addr"]) - 200];
                                    string unit = stateTagAttributes["unit"];

                                    

                                    if (rawValue >= 32768) rawValue -= 65536;
                                    //// size가 "s2"인 경우, 부호 있는 16비트 정수로 해석
                                    //if (stateTagAttributes["size"] == "s2")
                                    //{
                                    //    // rawValue가 32768 이상인 경우, 음수로 변환
                                        
                                    //}

                                    // 계산된 값을 스케일에 맞춰 변환
                                    float scaledValue = rawValue / float.Parse(stateTagAttributes["multiply"]);

                                    valueStateElementInstance = ObjectPool.Instance.GetDVSetElementObject_Numpad();
                                    valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                                    valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;

                                    // 사용안함 값(nouse)이 존재하는 경우
                                    if (stateTagAttributes.TryGetValue("nouse", out var nouse))
                                    {
                                        if (stateTagAttributes.TryGetValue("nouseStr", out var nouseStr))
                                        {
                                            if (rawValue == int.Parse(nouse))
                                            {
                                                valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{nouseStr}";
                                                valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            
                                            if (rawValue == int.Parse(nouse))
                                            {
                                                valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = "사용안함";
                                                valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 변환된 값을 텍스트로 설정                                            
                                        valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                                        valueStateElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;
                                    }

                                    valueStateElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = stateTagAttributes["name"];
                                    valueStateElementInstance.name = valueStateElementName;
                                    dvSetElementInstances[valueStateElementInstance.name] = valueStateElementInstance;
                                }
                                catch (Exception ex)
                                {
                                    //Debug.LogError(ex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // group이 설정값 관련인 경우에 대한 처리
                    // 각 group 태그의 속성 이름과 값 출력
                    foreach (var attribute in groupAttributes)
                    {
                        if (groupAttributes.TryGetValue("id", out var groupID) && groupAttributes.TryGetValue("title", out var title))
                        {
                            groupName = groupID;
                            groupTitle = title;

                            // 하단 메뉴 group 태그 요소 버튼 인스턴스 생성
                            string groupElementObjName = $"DVCategory_{groupID}_{iid}_{cid}";
                            GameObject groupElementInstance;
                            Dictionary<string, GameObject> groupElementDictionary = dvGroupElementInstances;

                            if (!groupElementDictionary.TryGetValue(groupElementObjName, out groupElementInstance))
                            {
                                groupElementInstance = ObjectPool.Instance.GetDVGroupElementObject();
                                groupElementInstance.name = groupElementObjName;
                                groupElementDictionary[groupElementObjName] = groupElementInstance;

                                TextMeshProUGUI activeName = groupElementInstance.transform.Find("Img_On/txt_CategoryName").GetComponent<TextMeshProUGUI>();
                                TextMeshProUGUI deActiveName = groupElementInstance.transform.Find("Img_Off/txt_CategoryName").GetComponent<TextMeshProUGUI>();
                                activeName.text = title;
                                deActiveName.text = title;


                                // 모든 group 태그 요소 버튼 인스턴스에 리스너 할당
                                groupElementInstance.GetComponent<Button>().onClick.AddListener(() =>
                                {
                                    graphActive.SetActive(false);
                                    graphDeactive.SetActive(true);
                                    graphParent.SetActive(false);
                                    dvElementScrollView.SetActive(true);

                                    // 현재 클릭된 groupID 저장
                                    currentSelectedGroupID = groupID;
                                    currentSelectedGroupTitle = title;

                                    // 엔지니어 메뉴 항목중 일반 설정을 제외한 다른 설정 메뉴는 운전 중 제어가 불가능함
                                    if (groupAttributes.TryGetValue("operateWhileRun", out var operateWhileRun) && operateWhileRun == "y")
                                        isGroupCanSetUpWhileRun = true;
                                    else
                                        isGroupCanSetUpWhileRun = false;

                                    // 클릭된 요소만 활성화하고 나머지는 비활성화
                                    foreach (var groupElement in dvGroupElementInstances.Values)
                                    {
                                        GameObject active = groupElement.transform.Find("Img_On").gameObject;
                                        GameObject deactive = groupElement.transform.Find("Img_Off").gameObject;

                                        if (groupElement == groupElementInstance)
                                        {
                                            // 현재 클릭된 인스턴스
                                            active.SetActive(true);
                                            deactive.SetActive(false);
                                        }
                                        else
                                        {
                                            // 나머지 인스턴스들
                                            active.SetActive(false);
                                            deactive.SetActive(true);
                                        }
                                    }

                                    foreach (var setElementEntry in dvSetElementInstances)
                                    {
                                        var setElement = setElementEntry.Value;
                                        // setElement의 이름에서 groupID 추출
                                        var setElementNameParts = setElementEntry.Key.Split('_');
                                        if (setElementNameParts.Length > 1)
                                        {
                                            var setElementGroupID = setElementNameParts[1]; // 이름 규칙이 "SetElement_{groupID}_{addr}"라고 가정
                                            var setElementIID = setElementNameParts[3];
                                            var setElementCID = setElementNameParts[4];

                                            // 현재 선택된 groupID와 일치하는지 확인
                                            if (setElementGroupID == currentSelectedGroupID && currentSelectedIID == setElementIID && currentSelectedCID == setElementCID)
                                            {
                                                setElement.SetActive(true);
                                            }
                                            else
                                            {
                                                setElement.SetActive(false);
                                            }
                                        }
                                    }

                                });
                            }
                        }
                    }

                    // tag 태그의 속성들 출력
                    var tags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    
                    foreach (var tag in tags)
                    {
                        var tagAttributes = tag.Value; // 각 태그의 속성들을 담은 Dictionary

                        if (tagAttributes.TryGetValue("outageReturn", out var outageReturn))
                        {
                            outageRecoveryAddr = int.Parse(outageReturn);
                        }

                        if (tagAttributes.TryGetValue("stopDelay", out var stopDelay))
                        {
                            stopDelayAddr = int.Parse(stopDelay);
                        }
                        //Debug.Log($"정전복귀 addr : {outageRecoveryAddr} 정지지연 addr : {stopDelayAddr}");

                        if (tagAttributes.TryGetValue("addr", out var addr) &&
                            tagAttributes.TryGetValue("name", out var name) &&
                            tagAttributes.TryGetValue("size", out var size) &&
                            tagAttributes.TryGetValue("style", out var style) &&
                            tagAttributes.TryGetValue("min", out var min) &&
                            tagAttributes.TryGetValue("max", out var max) &&
                            tagAttributes.TryGetValue("multiply", out var multiply) &&
                            tagAttributes.TryGetValue("unit", out var unit))
                        {
                            // style 값에 따른 분기 처리
                            string setElementName = $"SetElement_{groupName}_{addr}_{iid}_{cid}";
                            string finalUnit = string.Empty;
                            string finalMultiply = string.Empty;

                            GameObject SetBitElementInstance = null;
                            if (dvSetElementInstances.TryGetValue(setElementName, out var setElementInstance))
                            {
                                // 인스턴스가 존재하는 경우, 새로운 값으로 업데이트
                                UpdateSetElementInstance(iid, cid, setElementInstance, tagAttributes);
                            }
                            else
                            {
                                switch (style)
                                {
                                    case "numpad":
                                        if (tag.Value.TryGetValue("itemsSetAddr", out var itemsSetAddr))
                                        {
                                            int setIndex = int.Parse(itemsSetAddr) < 200 ? parsedPollingData[int.Parse(itemsSetAddr)] : parsedPollingData[int.Parse(itemsSetAddr) - 200];
                                            
                                            string[] arrUnit = unit.Split(',');
                                            string[] arrMultiply = multiply.Split(',');

                                            finalUnit = arrUnit[setIndex];
                                            finalMultiply = arrMultiply[setIndex];

                                            // 최소값, 최대값에 특정 번지의 설정값이 들어가는 경우
                                            int minSetValue = 0;
                                            int maxSetValue = 0;
                                            if (tagAttributes.TryGetValue("minSetAddr", out var minSetAddr) && tagAttributes.TryGetValue("maxSetAddr", out var maxSetAddr))
                                            {
                                                minSetValue = int.Parse(minSetAddr) < 200 ? parsedPollingData[int.Parse(minSetAddr)] : parsedPollingData[int.Parse(minSetAddr) - 200];
                                                maxSetValue = int.Parse(maxSetAddr) < 200 ? parsedPollingData[int.Parse(maxSetAddr)] : parsedPollingData[int.Parse(maxSetAddr) - 200];
                                            }

                                            // 모드버스에서 읽어온 값                                        
                                            int rawValue = int.Parse(addr) < 200 ? parsedPollingData[int.Parse(addr)] : parsedPollingData[int.Parse(addr) - 200];

                                            // rawValue가 32768 이상인 경우, 음수로 변환
                                            if (rawValue >= 32768) rawValue -= 65536;
                                            //// size가 "s2"인 경우, 부호 있는 16비트 정수로 해석
                                            //if (tagAttributes["size"] == "s2")
                                            //{
                                            //    // rawValue가 32768 이상인 경우, 음수로 변환
                                            //    if (rawValue >= 32768) rawValue -= 65536;
                                            //}

                                            // 계산된 값을 스케일에 맞춰 변환
                                            string scaledValue = string.Empty;
                                            if (finalMultiply == "1.0" || finalMultiply == "1")
                                                scaledValue = (rawValue / float.Parse(multiply)).ToString();
                                            else if (finalMultiply == "10.0" || finalMultiply == "10")
                                                scaledValue = (rawValue / float.Parse(multiply)).ToString("F1");
                                            else if (finalMultiply == "100.0" || finalMultiply == "100")
                                                scaledValue = (rawValue / float.Parse(finalMultiply)).ToString("F2");


                                            setElementInstance = ObjectPool.Instance.GetDVSetElementObject_Numpad();
                                            setElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = name;
                                            setElementInstance.name = setElementName;
                                            dvSetElementInstances[setElementInstance.name] = setElementInstance;
                                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = finalUnit;

                                            // 사용안함 값(nouse)이 존재하는 경우
                                            if (tagAttributes.TryGetValue("nouse", out var nouse))
                                            {
                                                if (tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                                                {
                                                    if (rawValue == int.Parse(nouse))
                                                    {
                                                        // 사용안함 값이 현재 값과 동일하고, 사용안함 값에 대한 별도의 문자열이 있는 경우
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{nouseStr}";
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                                                    }
                                                }
                                                else
                                                {
                                                    if (currentPkey == "UC0815120104610507")
                                                    {
                                                        int delayUnit = parsedPollingData[9];

                                                        if (int.Parse(tagAttributes["addr"]) == 210)
                                                        {
                                                            if (delayUnit == 0)
                                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[10]).ToString();
                                                            else if (delayUnit == 1)
                                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[10] / 60).ToString();
                                                        }
                                                        if (int.Parse(tagAttributes["addr"]) == 213)
                                                        {
                                                            if (delayUnit == 0)
                                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[13]).ToString();
                                                            else if (delayUnit == 1)
                                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[13] / 60).ToString();
                                                        }
                                                    }

                                                    if (rawValue == int.Parse(nouse))
                                                    {
                                                        // 단순 사용안함 값이 존재하고, 사용안함 값이 현재 값과 동일한 경우
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = "사용안함";
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // 사용안함 값에 대한 속성을 사용하지 않는 경우
                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = finalUnit;
                                            }
                                            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                                            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                if (isGroupCanSetUpWhileRun && !status.IsRun)
                                                {
                                                    if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                                                    {
                                                        SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), finalUnit, minSetValue.ToString(), maxSetValue.ToString(), finalMultiply, tagAttributes["size"], tagAttributes);
                                                    }
                                                    else
                                                    {
                                                        SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), finalUnit, min, max, finalMultiply, tagAttributes["size"], tagAttributes);
                                                    }
                                                }
                                                else
                                                {
                                                    if (status.IsRun)
                                                    {
                                                        screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                                        screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                                        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                                        screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                                    }
                                                    else
                                                    {
                                                        if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                                                        {
                                                            SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), finalUnit, minSetValue.ToString(), maxSetValue.ToString(), finalMultiply, tagAttributes["size"], tagAttributes);
                                                        }
                                                        else
                                                        {
                                                            SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), finalUnit, min, max, finalMultiply, tagAttributes["size"], tagAttributes);
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                        else
                                        {
                                            // 최소값, 최대값에 특정 번지의 설정값이 들어가는 경우
                                            int minSetValue = 0;
                                            int maxSetValue = 0;
                                            if (tagAttributes.TryGetValue("minSetAddr", out var minSetAddr) && tagAttributes.TryGetValue("maxSetAddr", out var maxSetAddr))
                                            {
                                                minSetValue = int.Parse(minSetAddr) < 200 ? parsedPollingData[int.Parse(minSetAddr)] : parsedPollingData[int.Parse(minSetAddr) - 200];
                                                maxSetValue = int.Parse(maxSetAddr) < 200 ? parsedPollingData[int.Parse(maxSetAddr)] : parsedPollingData[int.Parse(maxSetAddr) - 200];
                                            }

                                            // 모드버스에서 읽어온 값                                        
                                            int rawValue = int.Parse(addr) < 200 ? parsedPollingData[int.Parse(addr)] : parsedPollingData[int.Parse(addr) - 200];

                                            if (rawValue >= 32768) rawValue -= 65536;
                                            //// size가 "s2"인 경우, 부호 있는 16비트 정수로 해석
                                            //if (tagAttributes["size"] == "s2")
                                            //{
                                            //    // rawValue가 32768 이상인 경우, 음수로 변환
                                            //    if (rawValue >= 32768) rawValue -= 65536;
                                            //}

                                            // 계산된 값을 스케일에 맞춰 변환
                                            string scaledValue = string.Empty;
                                            if (multiply == "1.0" || multiply == "1")
                                                scaledValue = (rawValue / float.Parse(multiply)).ToString();
                                            else if (multiply == "10.0" || multiply == "10")
                                                scaledValue = (rawValue / float.Parse(multiply)).ToString("F1");
                                            else if (multiply == "100.0" || multiply == "100")
                                                scaledValue = (rawValue / float.Parse(multiply)).ToString("F2");


                                            setElementInstance = ObjectPool.Instance.GetDVSetElementObject_Numpad();
                                            setElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = name;
                                            setElementInstance.name = setElementName;
                                            dvSetElementInstances[setElementInstance.name] = setElementInstance;
                                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;

                                            // 사용안함 값(nouse)이 존재하는 경우
                                            if (tagAttributes.TryGetValue("nouse", out var nouse))
                                            {
                                                if (tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                                                {
                                                    if (rawValue == int.Parse(nouse))
                                                    {
                                                        // 사용안함 값이 현재 값과 동일하고, 사용안함 값에 대한 별도의 문자열이 있는 경우
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{nouseStr}";
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                                                    }
                                                }
                                                else
                                                {
                                                    

                                                    if (rawValue == int.Parse(nouse))
                                                    {
                                                        // 단순 사용안함 값이 존재하고, 사용안함 값이 현재 값과 동일한 경우
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = "사용안함";
                                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // 사용안함 값에 대한 속성을 사용하지 않는 경우
                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;
                                            }
                                            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                                            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                if (isGroupCanSetUpWhileRun && !status.IsRun)
                                                {
                                                    if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                                                    {
                                                        SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), unit, minSetValue.ToString(), maxSetValue.ToString(), multiply, tagAttributes["size"], tagAttributes);
                                                    }
                                                    else
                                                    {
                                                        SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), unit, min, max, multiply, tagAttributes["size"], tagAttributes);
                                                    }
                                                }
                                                else
                                                {
                                                    if (status.IsRun)
                                                    {
                                                        screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                                        screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                                        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                                        screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                                    }
                                                    else
                                                    {
                                                        if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                                                        {
                                                            SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), unit, minSetValue.ToString(), maxSetValue.ToString(), multiply, tagAttributes["size"], tagAttributes);
                                                        }
                                                        else
                                                        {
                                                            SetNumpadDatas(addr, iid, cid, rawValue, float.Parse(scaledValue), unit, min, max, multiply, tagAttributes["size"], tagAttributes);
                                                        }
                                                    }
                                                }
                                            });
                                        }
                                        break;
                                    case "dropdown":
                                        setElementInstance = ObjectPool.Instance.GetDVSetElementObject_Dropdown();
                                        setElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = name;
                                        setElementInstance.name = setElementName;
                                        dvSetElementInstances[setElementInstance.name] = setElementInstance;
                                        if (tagAttributes.TryGetValue("items", out var items))
                                        {
                                            string[] itemsData = items.Split(',');

                                            int index = int.Parse(addr) < 200 ? (int)Math.Round(parsedPollingData[int.Parse(addr)] / float.Parse(multiply)) : (int)Math.Round(parsedPollingData[int.Parse(addr) - 200] / float.Parse(multiply));
                                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = itemsData[index];

                                            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                                            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                                            {
                                                if (isGroupCanSetUpWhileRun && !status.IsRun)
                                                {
                                                    SetDropdownDatas(tagAttributes["addr"], iid, cid, itemsData, tagAttributes);
                                                }
                                                else
                                                {
                                                    if (status.IsRun)
                                                    {
                                                        screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                                        screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                                        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                                        screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                                    }
                                                    else
                                                    {
                                                        SetDropdownDatas(tagAttributes["addr"], iid, cid, itemsData, tagAttributes);
                                                    }
                                                }
                                            });

                                            //setElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = name;
                                            //setElementInstance.name = setElementName;
                                            //dvSetElementInstances[setElementInstance.name] = setElementInstance;
                                        }
                                        break;
                                    case "toggle":
                                        var bitAttributes = tagAttributes.Where(k => k.Key.StartsWith("bit")).ToList();

                                        if (bitAttributes.Any())
                                        {
                                            foreach (var bitAttr in bitAttributes)
                                            {
                                                var bitIndex = int.Parse(bitAttr.Key.Replace("bit", "")); // "bit" 다음 오는 숫자를 인덱스로 사용
                                                string specificSetName = $"{setElementName}_Bit{bitIndex}";
                                                var bitName = bitAttr.Value;

                                                if (!dvSetElementInstances.ContainsKey(specificSetName))
                                                {
                                                    SetBitElementInstance = ObjectPool.Instance.GetDVSetElementObject_Toggle(); // 토글 오브젝트를 올바르게 인스턴스화한다고 가정합니다.
                                                    SetBitElementInstance.name = specificSetName;
                                                    dvSetElementInstances[specificSetName] = SetBitElementInstance;

                                                    // 비트 값에 따른 토글 상태 설정

                                                    int rawValue2 = int.Parse(addr) < 200 ? parsedPollingData[int.Parse(addr)] : parsedPollingData[int.Parse(addr) - 200];
                                                    int setValue = 0;

                                                    bool isBitSet = (rawValue2 & (1 << bitIndex)) != 0;
                                                    setValue = isBitSet ? 1 : 0;
                                                    SetBitElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = isBitSet ? tagAttributes["val_on"] : tagAttributes["val_off"];
                                                    SetBitElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = bitName;

                                                    string currentStr = isBitSet ? tagAttributes["val_on"] : tagAttributes["val_off"];

                                                    SetBitElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                                                    SetBitElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                                                    {
                                                        if (isGroupCanSetUpWhileRun && !status.IsRun)
                                                        {
                                                            SetToggleDatas(tagAttributes["addr"], iid, cid, bitName, bitIndex, setValue, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                                                        }
                                                        else
                                                        {
                                                            if (status.IsRun)
                                                            {
                                                                screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                                                screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                                                screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                                                screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                                            }
                                                            else
                                                            {
                                                                SetToggleDatas(tagAttributes["addr"], iid, cid, bitName, bitIndex, setValue, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                                                            }
                                                        }
                                                    });

                                                    //dvSetElementInstances[specificSetName] = SetBitElementInstance;
                                                }

                                                UpdateSetElementInstance(iid, cid, SetBitElementInstance, tagAttributes);
                                            }
                                        }
                                        else
                                        {
                                            // bit로 시작하는 속성이 없는 경우, 해당 name 속성에 대한 인스턴스만 생성
                                            //var setToggleElementName = $"SetElement_{groupName}_{addr}";
                                            if (!dvSetElementInstances.ContainsKey(setElementName))
                                            {
                                                setElementInstance = ObjectPool.Instance.GetDVSetElementObject_Toggle();
                                                setElementInstance.name = setElementName;
                                                dvSetElementInstances[setElementName] = setElementInstance;
                                                setElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = name;

                                                int rawValue3 = int.Parse(addr) < 200 ? parsedPollingData[int.Parse(addr)] : parsedPollingData[int.Parse(addr) - 200];
                                                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (rawValue3 == 1) ? tagAttributes["val_on"] : tagAttributes["val_off"];

                                                string currentStr = (rawValue3 == 1) ? tagAttributes["val_on"] : tagAttributes["val_off"];

                                                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                                                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                                                {
                                                    if (isGroupCanSetUpWhileRun && !status.IsRun)
                                                    {
                                                        SetToggleDatas(tagAttributes["addr"], iid, cid, name, 0, rawValue3, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                                                    }
                                                    else
                                                    {
                                                        if (status.IsRun)
                                                        {
                                                            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                                            screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                                            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                                            screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                                        }
                                                        else
                                                        {
                                                            SetToggleDatas(tagAttributes["addr"], iid, cid, name, 0, rawValue3, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                                                        }
                                                    }
                                                });

                                                //dvSetElementInstances[setElementName] = setElementInstance;
                                            }
                                        }
                                        break;
                                    case "timepicker_23":

                                        int rawValue4 = int.Parse(addr) < 200 ? parsedPollingData[int.Parse(addr)] : parsedPollingData[int.Parse(addr) - 200];
                                        // 상위 바이트와 하위 바이트 추출
                                        int highByte = rawValue4 >> 8; // 상위 바이트
                                        int lowByte = rawValue4 & 0xFF; // 하위 바이트

                                        // min, max, unit 값 파싱
                                        string[] minValues = tagAttributes["min"].Split(',');
                                        string[] maxValues = tagAttributes["max"].Split(',');
                                        string[] unitValues = tagAttributes["unit"].Split(',');

                                        // 상위 바이트와 하위 바이트에 대한 값을 설정
                                        string highByteValue = highByte.ToString();
                                        string lowByteValue = lowByte.ToString();

                                        // min, max 값에 따른 유효성 검사 및 조정이 필요한 경우 여기에 로직 추가
                                        // 예: highByte, lowByte 값이 minValues, maxValues 범위 안에 있는지 확인

                                        // TextMeshProUGUI 컴포넌트에 값 할당
                                        setElementInstance = ObjectPool.Instance.GetDVSetElementObject_TimePicker();
                                        setElementInstance.name = setElementName;
                                        dvSetElementInstances[setElementInstance.name] = setElementInstance;
                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value_MonthOrHour").GetComponent<TextMeshProUGUI>().text = highByteValue;
                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit_MonthOrHour").GetComponent<TextMeshProUGUI>().text = unitValues[0]; // "시" 또는 "월"

                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value_DayOrMinute").GetComponent<TextMeshProUGUI>().text = lowByteValue;
                                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit_DayOrMinute").GetComponent<TextMeshProUGUI>().text = unitValues[1]; // "분" 또는 "일"

                                        setElementInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text = name;
                                        //setElementInstance.name = setElementName;
                                        //dvSetElementInstances[setElementInstance.name] = setElementInstance;

                                        setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                                        setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                                        {
                                            if (isGroupCanSetUpWhileRun)
                                            {
                                                SetTimePickerDatas(addr, iid, cid, highByteValue, unitValues[0], lowByteValue, unitValues[1], minValues[0], minValues[1], maxValues[0], maxValues[1], tagAttributes);
                                            }
                                            else
                                            {
                                                if (status.IsRun)
                                                {
                                                    screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                                    screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                                    screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                                    screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                                }
                                                else
                                                {
                                                    SetTimePickerDatas(addr, iid, cid, highByteValue, unitValues[0], lowByteValue, unitValues[1], minValues[0], minValues[1], maxValues[0], maxValues[1], tagAttributes);
                                                }
                                            }
                                        });
                                        break;
                                    default:
                                        //Debug.LogError($"Unknown style '{style}' for element '{name}'.");
                                        continue; // 이 태그는 처리하지 않고 다음 태그로 넘어갑니다.
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // group 태그에 대한 tag 태그 설정 요소 인스턴스 값 업데이트
    private void UpdateSetElementInstance(string iid, string cid, GameObject setElementInstance, Dictionary<string, string> tagAttributes)
    {
        // numpad 스타일 인스턴스 업데이트 로직
        if (tagAttributes["style"] == "numpad" && tagAttributes.TryGetValue("multiply", out var multiply))
        {
            string finalUnit = string.Empty;
            string finalMultiply = string.Empty;

            if (tagAttributes.TryGetValue("itemsSetAddr", out var itemsSetAddr))
            {
                int setIndex = int.Parse(itemsSetAddr) < 200 ? parsedPollingData[int.Parse(itemsSetAddr)] : parsedPollingData[int.Parse(itemsSetAddr) - 200];
                
                string[] arrUnit = tagAttributes["unit"].Split(',');
                string[] arrMultiply = multiply.Split(',');
                finalUnit = arrUnit[setIndex];
                finalMultiply = arrMultiply[setIndex];

                // 모드버스에서 읽어온 값 업데이트 예시
                int rawValue = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];

                if (rawValue >= 32768) rawValue -= 65536;
                //if (tagAttributes["size"] == "s2")
                //{
                //    // rawValue가 32768 이상인 경우, 음수로 변환
                    
                //}

                string scaledValue = string.Empty;
                if (finalMultiply == "1.0" || finalMultiply == "1")
                    scaledValue = (rawValue / float.Parse(multiply)).ToString();
                else if (finalMultiply == "10.0" || finalMultiply == "10")
                    scaledValue = (rawValue / float.Parse(multiply)).ToString("F1");
                else if (finalMultiply == "100.0" || finalMultiply == "100")
                    scaledValue = (rawValue / float.Parse(finalMultiply)).ToString("F2");
                //string scaledValue = (rawValue / float.Parse(tagAttributes["multiply"])).ToString("F1");

                // UI 컴포넌트에 새로운 값 할당
                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                if (tagAttributes.TryGetValue("unit", out var unit))
                {
                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = finalUnit;
                }

                // 사용안함 값(nouse)이 존재하는 경우
                if (tagAttributes.TryGetValue("nouse", out var nouse))
                {
                    if (tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                    {
                        if (scaledValue == (float.Parse(nouse) / float.Parse(tagAttributes["multiply"])).ToString())
                        {
                            // 사용안함 값이 현재 값과 동일하고, 사용안함 값에 대한 별도의 문자열이 있는 경우
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{nouseStr}";
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                        }
                    }
                    else
                    {
                        if(currentPkey == "UC0815120104610507")
                        {
                            int delayUnit = parsedPollingData[9];

                            if (int.Parse(tagAttributes["addr"]) == 210)
                            {
                                if(delayUnit == 0)
                                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[10]).ToString();
                                else if(delayUnit == 1)
                                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[10] / 60).ToString();
                            }
                            if (int.Parse(tagAttributes["addr"]) == 213)
                            {
                                if (delayUnit == 0)
                                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[13]).ToString();
                                else if (delayUnit == 1)
                                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = (parsedPollingData[13] / 60).ToString();
                            }
                        }
                        if (scaledValue == (float.Parse(nouse) / float.Parse(tagAttributes["multiply"])).ToString())
                        {
                            // 단순 사용안함 값이 존재하고, 사용안함 값이 현재 값과 동일한 경우
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = "사용안함";
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                        }
                    }
                }
                else
                {
                    // 사용안함 값에 대한 속성을 사용하지 않는 경우
                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = finalUnit;
                }


                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                {
                    int minSetValue = 0;
                    int maxSetValue = 0;
                    if (tagAttributes.TryGetValue("minSetAddr", out var minSetAddr) && tagAttributes.TryGetValue("maxSetAddr", out var maxSetAddr))
                    {

                        minSetValue = int.Parse(minSetAddr) < 200 ? parsedPollingData[int.Parse(minSetAddr)] : parsedPollingData[int.Parse(minSetAddr) - 200];
                        maxSetValue = int.Parse(maxSetAddr) < 200 ? parsedPollingData[int.Parse(maxSetAddr)] : parsedPollingData[int.Parse(maxSetAddr) - 200];
                        
                    }

                    if (isGroupCanSetUpWhileRun)
                    {
                        if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                        {
                            SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), finalUnit, minSetValue.ToString(), maxSetValue.ToString(), finalMultiply, tagAttributes["size"], tagAttributes);
                        }
                        else
                        {
                            SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), finalUnit, tagAttributes["min"], tagAttributes["max"], finalMultiply, tagAttributes["size"], tagAttributes);
                        }
                    }
                    else
                    {
                        if (status.IsRun)
                        {
                            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                            screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                        }
                        else
                        {
                            if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                            {
                                SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), finalUnit, minSetValue.ToString(), maxSetValue.ToString(), finalMultiply, tagAttributes["size"], tagAttributes);
                            }
                            else
                            {
                                SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), finalUnit, tagAttributes["min"], tagAttributes["max"], finalMultiply, tagAttributes["size"], tagAttributes);
                            }
                        }
                    }
                });
            }
            else
            {
                // 모드버스에서 읽어온 값 업데이트 예시

                int rawValue = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];

                if (rawValue >= 32768) rawValue -= 65536;
                //if (tagAttributes["size"] == "s2")
                //{
                //    // rawValue가 32768 이상인 경우, 음수로 변환
                    
                //}

                string scaledValue = string.Empty;
                if (multiply == "1.0" || multiply == "1")
                    scaledValue = (rawValue / float.Parse(multiply)).ToString();
                else if (multiply == "10.0" || multiply == "10")
                    scaledValue = (rawValue / float.Parse(multiply)).ToString("F1");
                else if (multiply == "100.0" || multiply == "100")
                    scaledValue = (rawValue / float.Parse(multiply)).ToString("F2");

                // UI 컴포넌트에 새로운 값 할당
                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                if (tagAttributes.TryGetValue("unit", out var unit))
                {
                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;
                }

                // 사용안함 값(nouse)이 존재하는 경우
                if (tagAttributes.TryGetValue("nouse", out var nouse))
                {
                    if (tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                    {
                        if (scaledValue == (float.Parse(nouse) / float.Parse(tagAttributes["multiply"])).ToString())
                        {
                            // 사용안함 값이 현재 값과 동일하고, 사용안함 값에 대한 별도의 문자열이 있는 경우
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{nouseStr}";
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                        }
                    }
                    else
                    {
                        //if (scaledValue == (float.Parse(nouse) / float.Parse(tagAttributes["multiply"])).ToString())
                        if (rawValue == int.Parse(nouse))
                        {
                            // 단순 사용안함 값이 존재하고, 사용안함 값이 현재 값과 동일한 경우
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = "사용안함";
                            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                        }
                    }
                }
                else
                {
                    // 사용안함 값에 대한 속성을 사용하지 않는 경우
                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                    setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;
                }

                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                {
                    int minSetValue = 0;
                    int maxSetValue = 0;
                    if (tagAttributes.TryGetValue("minSetAddr", out var minSetAddr) && tagAttributes.TryGetValue("maxSetAddr", out var maxSetAddr))
                    {

                        minSetValue = int.Parse(minSetAddr) < 200 ? parsedPollingData[int.Parse(minSetAddr)] : parsedPollingData[int.Parse(minSetAddr) - 200];
                        maxSetValue = int.Parse(maxSetAddr) < 200 ? parsedPollingData[int.Parse(maxSetAddr)] : parsedPollingData[int.Parse(maxSetAddr) - 200];
                    }

                    if (isGroupCanSetUpWhileRun)
                    {
                        if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                        {
                            SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), unit, minSetValue.ToString(), maxSetValue.ToString(), multiply, tagAttributes["size"], tagAttributes);
                        }
                        else
                        {
                            SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), unit, tagAttributes["min"], tagAttributes["max"], multiply, tagAttributes["size"], tagAttributes);
                        }
                    }
                    else
                    {
                        if (status.IsRun)
                        {
                            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                            screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                        }
                        else
                        {
                            if (tagAttributes.ContainsKey("minSetAddr") && tagAttributes.ContainsKey("maxSetAddr"))
                            {
                                SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), unit, minSetValue.ToString(), maxSetValue.ToString(), multiply, tagAttributes["size"], tagAttributes);
                            }
                            else
                            {
                                SetNumpadDatas(tagAttributes["addr"], iid, cid, rawValue, float.Parse(scaledValue), unit, tagAttributes["min"], tagAttributes["max"], multiply, tagAttributes["size"], tagAttributes);
                            }
                        }
                    }
                });
            }


        }

        // dropdown 스타일 인스턴스 업데이트 로직
        if (tagAttributes["style"] == "dropdown")
        {
            string[] itemsData = tagAttributes["items"].Split(',');

            int index = int.Parse(tagAttributes["addr"]) < 200 ? (int)Math.Round(parsedPollingData[int.Parse(tagAttributes["addr"])] / float.Parse(tagAttributes["multiply"])) : (int)Math.Round(parsedPollingData[int.Parse(tagAttributes["addr"]) - 200] / float.Parse(tagAttributes["multiply"]));
            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = itemsData[index];

            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (isGroupCanSetUpWhileRun)
                {
                    SetDropdownDatas(tagAttributes["addr"], iid, cid, itemsData, tagAttributes);
                }
                else
                {
                    if (status.IsRun)
                    {
                        screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                        screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                        screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                    }
                    else
                    {
                        SetDropdownDatas(tagAttributes["addr"], iid, cid, itemsData, tagAttributes);
                    }
                }
            });
        }

        // toggle 스타일 인스턴스 업데이트 로직
        if (tagAttributes["style"] == "toggle")
        {
            // 'val_on'과 'val_off' 값 가져오기
            string valOn = tagAttributes.ContainsKey("val_on") ? tagAttributes["val_on"] : "On";
            string valOff = tagAttributes.ContainsKey("val_off") ? tagAttributes["val_off"] : "Off";

            // 모드버스에서 읽어온 값 업데이트 예시

            int rawValue = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            int setValue = 0;
            // bit로 시작하는 속성을 검색
            var bitAttributes = tagAttributes.Where(k => k.Key.StartsWith("bit")).ToList();
            if (bitAttributes.Any())
            {
                // 각 비트별 UI 컴포넌트 업데이트 로직
                foreach (var bitAttr in bitAttributes)
                {
                    var bitIndex = int.Parse(bitAttr.Key.Replace("bit", ""));
                    bool isBitSet = (rawValue & (1 << bitIndex)) != 0;

                    // 비트별 오브젝트 이름 정확히 구성
                    string specificSetName = $"SetElement_{$"{groupName}"}_{tagAttributes["addr"]}_Bit{bitIndex}";

                    if (dvSetElementInstances.TryGetValue(specificSetName, out var specificSetElementInstance))
                    {
                        // 비트별 오브젝트를 찾아 값 업데이트
                        var bitTextComponent = specificSetElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                        bitTextComponent.text = isBitSet ? valOn : valOff;
                        setValue = isBitSet ? 1 : 0;

                        string currentStr = isBitSet ? tagAttributes["val_on"] : tagAttributes["val_off"];
                        specificSetElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                        specificSetElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (isGroupCanSetUpWhileRun)
                            {
                                SetToggleDatas(tagAttributes["addr"], iid, cid, bitAttr.Value, bitIndex, setValue, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                            }
                            else
                            {
                                if (status.IsRun)
                                {
                                    screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                    screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                                    screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                                    screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                                }
                                else
                                {
                                    SetToggleDatas(tagAttributes["addr"], iid, cid, bitAttr.Value, bitIndex, setValue, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                                }
                            }
                        });
                    }
                }
            }
            else
            {
                // 단일 toggle 처리
                var toggleValueText = setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                toggleValueText.text = (rawValue == 1) ? valOn : valOff;
                string currentStr = (rawValue == 1) ? tagAttributes["val_on"] : tagAttributes["val_off"];
                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
                setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (isGroupCanSetUpWhileRun)
                    {
                        SetToggleDatas(tagAttributes["addr"], iid, cid, tagAttributes["name"], 0, rawValue, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                    }
                    else
                    {
                        if (status.IsRun)
                        {
                            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                            screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                        }
                        else
                        {
                            SetToggleDatas(tagAttributes["addr"], iid, cid, tagAttributes["name"], 0, rawValue, currentStr, tagAttributes["val_on"], tagAttributes["val_off"], tagAttributes);
                        }
                    }
                });
            }
        }

        // timepicker_23 스타일 인스턴스 업데이트 로직
        if (tagAttributes["style"] == "timepicker_23")
        {

            int rawValue = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            // 상위 바이트와 하위 바이트 추출
            int highByte = rawValue >> 8; // 상위 바이트
            int lowByte = rawValue & 0xFF; // 하위 바이트

            // min, max, unit 값 파싱
            string[] minValues = tagAttributes["min"].Split(',');
            string[] maxValues = tagAttributes["max"].Split(',');
            string[] unitValues = tagAttributes["unit"].Split(',');

            // 상위 바이트와 하위 바이트에 대한 값을 설정
            string highByteValue = highByte.ToString();
            string lowByteValue = lowByte.ToString();

            // min, max 값에 따른 유효성 검사 및 조정이 필요한 경우 여기에 로직 추가
            // 예: highByte, lowByte 값이 minValues, maxValues 범위 안에 있는지 확인

            // TextMeshProUGUI 컴포넌트에 값 할당            
            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value_MonthOrHour").GetComponent<TextMeshProUGUI>().text = highByteValue;
            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit_MonthOrHour").GetComponent<TextMeshProUGUI>().text = unitValues[0]; // "시" 또는 "월"

            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value_DayOrMinute").GetComponent<TextMeshProUGUI>().text = lowByteValue;
            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit_DayOrMinute").GetComponent<TextMeshProUGUI>().text = unitValues[1]; // "분" 또는 "일"

            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.RemoveAllListeners();
            setElementInstance.transform.Find("btn_Setting").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (isGroupCanSetUpWhileRun)
                {
                    SetTimePickerDatas(tagAttributes["addr"], iid, cid, highByteValue, unitValues[0], lowByteValue, unitValues[1], minValues[0], minValues[1], maxValues[0], maxValues[1], tagAttributes);
                }
                else
                {
                    if (status.IsRun)
                    {
                        screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                        screenManager.txt_PopUpMsg.text = "운전 중에는 설정이 불가능합니다.";
                        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                        screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                    }
                    else
                    {
                        SetTimePickerDatas(tagAttributes["addr"], iid, cid, highByteValue, unitValues[0], lowByteValue, unitValues[1], minValues[0], minValues[1], maxValues[0], maxValues[1], tagAttributes);
                    }
                }
            });

        }

        // bitState 스타일 인스턴스 업데이트 로직
        if (tagAttributes["style"] == "bitState")
        {

            int rawValue = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            // "bit" 뒤에 숫자만 있는 속성을 필터링
            var bitAttributes = tagAttributes
                .Where(k => Regex.IsMatch(k.Key, @"^bit\d+$"))
                .ToList();

            if (bitAttributes.Any())
            {
                foreach (var bitAttr in bitAttributes)
                {
                    // "bit" 다음 오는 숫자를 인덱스로 사용
                    var bitNumberStr = int.Parse(bitAttr.Key.Replace("bit", ""));
                    bool isBitSet = (rawValue & (1 << bitNumberStr)) != 0;

                    // 비트별 오브젝트 이름 정확히 구성
                    string specificSetName = $"bitStateElement_{groupName}_{tagAttributes["addr"]}_{iid}_{cid}_Bit{bitNumberStr}";

                    if (dvSetElementInstances.TryGetValue(specificSetName, out var specificSetElementInstance))
                    {
                        // 비트별 오브젝트를 찾아 값 업데이트
                        var bitTextComponent = specificSetElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                        bitTextComponent.text = isBitSet ? tagAttributes["val_on"] : tagAttributes["val_off"];
                    }
                }
            }
        }

        // valueState 스타일 인스턴스 업데이트 로직
        if (tagAttributes["style"] == "valueState")
        {
            // 모드버스에서 읽어온 값 업데이트 예시

            int rawValue = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            if (tagAttributes["size"] == "s2")
            {
                // rawValue가 32768 이상인 경우, 음수로 변환
                if (rawValue >= 32768) rawValue -= 65536;
            }
            float scaledValue = rawValue / float.Parse(tagAttributes["multiply"]);

            // UI 컴포넌트에 새로운 값 할당
            setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
            if (tagAttributes.TryGetValue("unit", out var unit))
            {
                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;
            }

            // 사용안함 값(nouse)이 존재하는 경우
            if (tagAttributes.TryGetValue("nouse", out var nouse))
            {
                if (tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                {
                    if (scaledValue == int.Parse(nouse))
                    {
                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{nouseStr}";
                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                    }
                }
                else
                {
                    if (scaledValue == int.Parse(nouse))
                    {
                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = "사용안함";
                        setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = string.Empty;
                    }
                }
            }
            else
            {
                // 변환된 값을 텍스트로 설정                                            
                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>().text = $"{scaledValue}";
                setElementInstance.transform.Find("btn_Setting/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>().text = unit;
            }
        }
    }

    // 운전 제어 리스너 함수
    private void OperateRun(string addr, string iid, string cid, string bitName, int bitIndex, bool isBitSet, string currentStr, string valOnStr, string valOffStr, Dictionary<string, string> tagAttributes)
    {
        string jsonString = string.Empty;
        setting_Toggle.SetActive(true);
        SettingToggle.InitToggle();

        SettingToggle.txtCurrentValueStr.text = string.Empty;
        SettingToggle.txtChangeValueStr.text = isBitSet ? $"운전을 정지하시겠습니까?" : $"운전을 시작하시겠습니까?";

        SettingToggle.btnOK.onClick.RemoveAllListeners(); // 기존 리스너 제거
        SettingToggle.btnOK.onClick.AddListener(() =>
        {
            int curVal = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            string curHexValue = "0x" + ((ushort)curVal).ToString("X4");
            // 비트 반전 로직
            int newRawValue = curVal ^ (1 << bitIndex);
            string newHexValue = "0x" + ((ushort)newRawValue).ToString("X4");
            string newSetValue = isBitSet ? valOffStr : valOnStr;

            jsonString = $"{{\"Address\":{addr},\"Value\":\"{newHexValue}\",\"Comment\":\"{bitName}, {currentStr}에서 {newSetValue}(으)로 변경 : {addr}\"}}";

            if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
            {
                SettingToggle.InitToggle();
                setting_Toggle.SetActive(false);
                isOperateComplete = false;
                if (operateWaitCoroutine == null && screenManager.CurrentScreenState == ScreenState.DetailView && outageRecoveryAddr != 0 && stopDelayAddr != 0)
                {
                    //Debug.Log("정전복귀 & 정지지연 카운트 화면 코루틴 시작");
                    operateWaitCoroutine = StartCoroutine(IOperateWaitComplete(isBitSet, outageRecoveryAddr, stopDelayAddr, iid, cid));
                }
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
        });
    }

    private IEnumerator IOperateWaitComplete(bool isBitSet, int outageReturnAddr, int stopDelayAddr, string iid, string cid)
    {
        TextMeshProUGUI txtLoading = waitOperateComplete.transform.Find("GameObject/txt_Loading").GetComponent<TextMeshProUGUI>();
        int remainOutageReturn = 0;
        int remainStopDelay = 0;

        remainOutageReturn = outageReturnAddr < 200 ? parsedPollingData[outageReturnAddr] : parsedPollingData[outageReturnAddr - 200];
        remainStopDelay = stopDelayAddr < 200 ? parsedPollingData[stopDelayAddr] : parsedPollingData[stopDelayAddr - 200];

        if (isBitSet)
        {
            waitOperateComplete.SetActive(true);
            for (int i = remainStopDelay; i >= 0; i--)
            {
                txtLoading.text = $"운전정지까지 {i}초 남았습니다.";
                if (remainStopDelay == 0)
                {
                    StopIOperateWaitComplete();
                }
                yield return sec1;
            }
        }
        else
        {
            waitOperateComplete.SetActive(true);
            for (int i = remainOutageReturn; i >= 0; i--)
            {
                txtLoading.text = $"정전복귀까지 {i}초 남았습니다.";
                if (remainOutageReturn == 0)
                {
                    StopIOperateWaitComplete();
                }
                yield return sec1;
            }            
        }
    }

    private void StopIOperateWaitComplete()
    {
        if (operateWaitCoroutine != null)
        {
            isOperateComplete = true;
            waitOperateComplete.SetActive(false);
            StopCoroutine(operateWaitCoroutine);
            operateWaitCoroutine = null;
        }
    }

    // 제상 제어 리스너 함수
    private void OperateDefrost(string addr, string iid, string cid, string bitName, int bitIndex, bool isBitSet, string currentStr, string valOnStr, string valOffStr, Dictionary<string, string> tagAttributes)
    {
        string jsonString = string.Empty;
        setting_Toggle.SetActive(true);
        SettingToggle.InitToggle();

        SettingToggle.txtCurrentValueStr.text = string.Empty;
        SettingToggle.txtChangeValueStr.text = isBitSet ? $"제상을 정지하시겠습니까?" : $"제상을 시작하시겠습니까?";


        SettingToggle.btnOK.onClick.RemoveAllListeners(); // 기존 리스너 제거
        SettingToggle.btnOK.onClick.AddListener(() =>
        {
            int curVal = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            string curHexValue = "0x" + curVal.ToString("X4");
            // 비트 반전 로직
            int newRawValue = curVal ^ (1 << bitIndex);
            string newHexValue = "0x" + ((ushort)newRawValue).ToString("X4");
            string newSetValue = isBitSet ? valOffStr : valOnStr;

            jsonString = $"{{\"Address\":{addr},\"Value\":\"{newHexValue}\",\"Comment\":\"{bitName}, {currentStr}에서 {newSetValue}(으)로 변경 : {addr}\"}}";
            //Debug.Log(jsonString);

            if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
            {
                SettingToggle.InitToggle();
                setting_Toggle.SetActive(false);
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
        });
    }

    // timepicker 팝업에 대한 리스너
    private void SetTimePickerDatas(string addr, string iid, string cid, string curHighValue, string curHighUnit, string curLowValue, string curLowUnit, string minHourRange, string maxHourRange, string minMinRange, string maxMinRange, Dictionary<string, string> tagAttributes)
    {
        string jsonString = string.Empty;
        string hexValue = string.Empty;
        int rawValue = 0;
        int highByteValue = 0;
        int lowByteValue = 0;

        setting_Timepicker.SetActive(true);
        SettingTimePicker.InitTimePicker();

        SettingTimePicker.txtElementName.text = tagAttributes["name"];
        SettingTimePicker.txtCurrentValue.text = $"현재 값 : {curHighValue}{curHighUnit} {curLowValue}{curLowUnit}";
        SettingTimePicker.txtRangeValue.text = $"범위 : {minHourRange}{curHighUnit} {minMinRange}{curLowUnit} ~ {maxHourRange}{curHighUnit} {maxMinRange}{curLowUnit}";
        SettingTimePicker.txtSetHourValue.text = curHighValue;
        SettingTimePicker.txtSetMinValue.text = curLowValue;
        SettingTimePicker.txtSetHourUnit.text = curHighUnit;
        SettingTimePicker.txtSetMinUnit.text = curLowUnit;

        SettingTimePicker.btnHourValueUp.onClick.AddListener(() =>
        {
            int setHourValue = int.Parse(SettingTimePicker.txtSetHourValue.text);

            if (setHourValue < int.Parse(maxHourRange) && setHourValue >= int.Parse(minHourRange))
            {
                setHourValue += 1;
                SettingTimePicker.txtSetHourValue.text = string.Empty;
                SettingTimePicker.txtSetHourValue.text = setHourValue.ToString();
            }
        });
        SettingTimePicker.btnHourValueDown.onClick.AddListener(() =>
        {
            int setHourValue = int.Parse(SettingTimePicker.txtSetHourValue.text);

            if (setHourValue <= int.Parse(maxHourRange) && setHourValue > int.Parse(minHourRange))
            {
                setHourValue -= 1;
                SettingTimePicker.txtSetHourValue.text = string.Empty;
                SettingTimePicker.txtSetHourValue.text = setHourValue.ToString();
            }
        });
        SettingTimePicker.btnMinValueUp.onClick.AddListener(() =>
        {
            int setMinValue = int.Parse(SettingTimePicker.txtSetMinValue.text);

            if (setMinValue < int.Parse(maxMinRange) && setMinValue >= int.Parse(minMinRange))
            {
                setMinValue += 1;
                SettingTimePicker.txtSetMinValue.text = string.Empty;
                SettingTimePicker.txtSetMinValue.text = setMinValue.ToString();
            }
        });
        SettingTimePicker.btnMinValueDown.onClick.AddListener(() =>
        {
            int setMinValue = int.Parse(SettingTimePicker.txtSetMinValue.text);

            if (setMinValue <= int.Parse(maxMinRange) && setMinValue > int.Parse(minMinRange))
            {
                setMinValue -= 1;
                SettingTimePicker.txtSetMinValue.text = string.Empty;
                SettingTimePicker.txtSetMinValue.text = setMinValue.ToString();
            }
        });



        SettingTimePicker.btnOK.onClick.RemoveAllListeners(); // 기존 리스너 제거
        SettingTimePicker.btnOK.onClick.AddListener(() =>
        {
            string jsonString = string.Empty;

            // 입력된 시간(시간)과 분 값을 정수로 파싱
            highByteValue = int.Parse(SettingTimePicker.txtSetHourValue.text);
            lowByteValue = int.Parse(SettingTimePicker.txtSetMinValue.text);

            // 상위 바이트를 8비트 왼쪽으로 시프트하고 하위 바이트와 결합하여 rawValue 생성
            rawValue = (highByteValue << 8) | lowByteValue;
            hexValue = "0x" + ((ushort)rawValue).ToString("X4");

            jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Comment\":\"{tagAttributes["name"]}, {curHighValue}{curHighUnit} {curLowValue}{curLowUnit}에서 " +
            $"{SettingTimePicker.txtSetHourValue.text}{SettingTimePicker.txtSetHourUnit.text} {SettingTimePicker.txtSetMinValue.text}{SettingTimePicker.txtSetMinUnit.text}(으)로 변경 : {addr}\"}}";

            if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
            {
                SettingTimePicker.InitTimePicker();
                setting_Timepicker.SetActive(false);
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
        });
    }

    // toggle 팝업에 대한 리스너
    private void SetToggleDatas(string addr, string iid, string cid, string bitName, int bitIndex, int rawOrbitValue, string currentStr, string valOnStr, string valOffStr, Dictionary<string, string> tagAttributes)
    {
        string jsonString = string.Empty;
        setting_Toggle.SetActive(true);
        SettingToggle.InitToggle();

        SettingToggle.txtCurrentValueStr.text = $"{bitName} 설정 값을 {currentStr}에서";
        SettingToggle.txtChangeValueStr.text = currentStr == valOnStr ? $"{valOffStr}(으)로 변경하시겠습니까?" : $"{valOnStr}(으)로 변경하시겠습니까?";

        SettingToggle.btnOK.onClick.RemoveAllListeners(); // 기존 리스너 제거
        SettingToggle.btnOK.onClick.AddListener(() =>
        {
            int curVal = int.Parse(tagAttributes["addr"]) < 200 ? parsedPollingData[int.Parse(tagAttributes["addr"])] : parsedPollingData[int.Parse(tagAttributes["addr"]) - 200];
            string curHexValue = "0x" + curVal.ToString("X4");
            // 비트 반전 로직

            int newRawValue = curVal ^ (1 << bitIndex);
            string newHexValue = "0x" + ((ushort)newRawValue).ToString("X4");
            string newSetValue = currentStr == valOnStr ? valOffStr : valOnStr;

            jsonString = $"{{\"Address\":{addr},\"Value\":\"{newHexValue}\",\"Comment\":\"{bitName}, {currentStr}에서 {newSetValue}(으)로 변경 : {addr}\"}}";

            if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
            {
                SettingToggle.InitToggle();
                setting_Toggle.SetActive(false);
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
        });
    }

    // dropdown 팝업에 대한 리스너
    private void SetDropdownDatas(string addr, string iid, string cid, string[] itemsData, Dictionary<string, string> tagAttributes)
    {

        int index = int.Parse(addr) < 200 ? (int)Math.Round(parsedPollingData[int.Parse(addr)] / float.Parse(tagAttributes["multiply"])) : (int)Math.Round(parsedPollingData[int.Parse(addr) - 200] / float.Parse(tagAttributes["multiply"]));
        int selectedIndex = 0;
        string currentValueStr = itemsData[index];
        string selectedValueStr = string.Empty;
        string hexValue = string.Empty;
        setting_Dropdown.SetActive(true);
        SettingDropdown.InitDropdown();
        SettingDropdown.txtElementName.text = tagAttributes["name"];

        // 새로운 옵션 설정을 위한 OptionData 생성
        List<TMP_Dropdown.OptionData> optionList = new List<TMP_Dropdown.OptionData>();

        // items 속성 값 파라미터인 itemsData 배열에 있는 모든 문자열 데이터를 불러와 optionLst에 저장
        foreach (string str in itemsData)
        {
            optionList.Add(new TMP_Dropdown.OptionData(str));
        }

        // 위에서 생성한 optionList를 dropdown의 옵션 값에 추가
        SettingDropdown.dropdownElements.AddOptions(optionList);

        // 현재 dropdown에 선택된 옵션을 현재 설정값의 인덱스로 설정
        SettingDropdown.dropdownElements.value = index;

        SettingDropdown.dropdownElements.onValueChanged.RemoveAllListeners();
        SettingDropdown.dropdownElements.onValueChanged.AddListener((value) =>
        {
            selectedIndex = value;
            selectedValueStr = itemsData[selectedIndex];
        });

        SettingDropdown.btnOK.onClick.RemoveAllListeners(); // 기존 리스너 제거
        SettingDropdown.btnOK.onClick.AddListener(() =>
        {
            string jsonString = string.Empty;
            hexValue = "0x" + ((ushort)selectedIndex).ToString("X4");
            jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Comment\":\"{tagAttributes["name"]}, {currentValueStr}에서 {selectedValueStr}(으)로 변경 : {addr}\"}}";

            if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
            {
                SettingDropdown.InitDropdown();
                setting_Dropdown.SetActive(false);
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
        });
    }

    // numpad 팝업에 대한 리스너
    private void SetNumpadDatas(string addr, string iid, string cid, int rawValue, float scaledValue, string unit, string min, string max, string multiply, string size, Dictionary<string, string> tagAttributes)
    {
        setting_Numpad.SetActive(true);
        SettingNumPad.InitNumpad();
        SettingNumPad.txtCategoryName.text = currentSelectedGroupTitle;
        SettingNumPad.txtElementName.text = tagAttributes["name"];
        if (currentPkey == "UC0815120104610507")
        {
                int delayUnit = parsedPollingData[9];
            if (int.Parse(tagAttributes["addr"]) == 210)
            {
                if (delayUnit == 0)
                {
                    SettingNumPad.txtCurrentValue.text = $"{rawValue}{unit}";
                    SettingNumPad.txtSetValue.text = $"{(parsedPollingData[10]).ToString()}";
                }
                else if (delayUnit == 1)
                {
                    SettingNumPad.txtCurrentValue.text = $"{rawValue / 60}{unit}";
                    SettingNumPad.txtSetValue.text = $"{(parsedPollingData[10] / 60).ToString()}";
                }
            }
            else if (int.Parse(tagAttributes["addr"]) == 213)
            {
                if (delayUnit == 0)
                {
                    SettingNumPad.txtCurrentValue.text = $"{rawValue}{unit}";
                    SettingNumPad.txtSetValue.text = $"{(parsedPollingData[13]).ToString()}";
                }
                else if (delayUnit == 1)
                {
                    SettingNumPad.txtCurrentValue.text = $"{rawValue / 60}{unit}";
                    SettingNumPad.txtSetValue.text = $"{(parsedPollingData[13] / 60).ToString()}";
                }
            }
            else
            {
                SettingNumPad.txtCurrentValue.text = $"{scaledValue}{unit}";
                SettingNumPad.txtSetValue.text = $"{scaledValue}";
            }
        }
        else
        {
            SettingNumPad.txtCurrentValue.text = $"{scaledValue}{unit}";
            SettingNumPad.txtSetValue.text = $"{scaledValue}";
        }
        
        SettingNumPad.txtSetUnit.text = $"{unit}";
        int minValue = 0;
        int maxValue = 0;
        minValue = int.Parse(min);
        maxValue = int.Parse(max);

        if (minValue >= 32768)
            minValue -= 65536;
        if (maxValue >= 32768)
            maxValue -= 65536;

        // 계산된 값을 스케일에 맞춰 변환
        string scaledMinValue = string.Empty;
        string scaledMaxValue = string.Empty;
        if (multiply == "1.0" || multiply == "1")
        {
            scaledMinValue = (minValue / float.Parse(multiply)).ToString();
            scaledMaxValue = (maxValue / float.Parse(multiply)).ToString();
        }
        else if (multiply == "10.0" || multiply == "10")
        {
            scaledMinValue = (minValue / float.Parse(multiply)).ToString("F1");
            scaledMaxValue = (maxValue / float.Parse(multiply)).ToString("F1");
        }
        else if (multiply == "100.0" || multiply == "100")
        {
            scaledMinValue = (minValue / float.Parse(multiply)).ToString("F2");
            scaledMaxValue = (maxValue / float.Parse(multiply)).ToString("F2");
        }

        if (currentPkey == "UC0815120104610507")
        {
            int delayUnit = parsedPollingData[9];

            if (int.Parse(tagAttributes["addr"]) == 210)
            {
                if (delayUnit == 0) // 초
                    scaledMaxValue = (int.Parse(scaledMaxValue)).ToString();
                else if (delayUnit == 1) // 분
                    scaledMaxValue = (int.Parse(scaledMaxValue) / 60).ToString();
            }
            if (int.Parse(tagAttributes["addr"]) == 213)
            {
                if (delayUnit == 0) // 초
                    scaledMaxValue = (int.Parse(scaledMaxValue)).ToString();
                else if (delayUnit == 1) // 분
                    scaledMaxValue = (int.Parse(scaledMaxValue) / 60).ToString();
            }
        }

        SettingNumPad.txtRangeStr.text = $"범위 : {scaledMinValue}~{scaledMaxValue}{unit}";

        // 사용안함 값(nouse)이 존재하는 경우
        if (tagAttributes.TryGetValue("nouse", out var nouse))
        {
            if (tagAttributes.TryGetValue("nouseStr", out var nouseStr))
            {
                if (rawValue == int.Parse(nouse))
                {
                    // 사용안함 값에 대한 별도 문자열이 있으면서 현재 값과 동일한 경우
                    SettingNumPad.toggleNoUse.interactable = true;
                    SettingNumPad.toggleNoUse.isOn = true;
                    SettingNumPad.txtCurrentValue.text = $"{nouseStr}";
                    SettingNumPad.txtNoUseStr.text = $"{nouseStr}";
                    SettingNumPad.txtSetValue.text = nouseStr;
                    SettingNumPad.txtSetUnit.text = string.Empty;

                    SettingNumPad.toggleNoUse.onValueChanged.RemoveAllListeners();
                    SettingNumPad.toggleNoUse.onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn)
                        {
                            // 토글이 켜진 경우, "사용안함" 또는 특정 문자열로 설정
                            SettingNumPad.txtSetValue.text = nouseStr; // "사용안함" 또는 다른 특정 문자열
                            SettingNumPad.txtSetUnit.text = string.Empty;
                        }
                        else
                        {
                            // 토글이 꺼진 경우, 원래의 값(스케일 조정된 값)으로 설정
                            float originalValue = rawValue / float.Parse(multiply);
                            SettingNumPad.txtSetValue.text = originalValue.ToString();
                            SettingNumPad.txtSetUnit.text = $"{unit}";
                        }
                    });
                }
                else
                {
                    // 사용안함 값에 대한 별도 문자열이 있지만 현재 값과 동일하지 않은 경우
                    SettingNumPad.toggleNoUse.interactable = true;
                    SettingNumPad.toggleNoUse.isOn = false;
                    SettingNumPad.txtNoUseStr.text = $"{nouseStr}";

                    SettingNumPad.txtSetValue.text = $"{scaledValue}";
                    SettingNumPad.txtSetUnit.text = $"{unit}";

                    SettingNumPad.toggleNoUse.onValueChanged.RemoveAllListeners();
                    SettingNumPad.toggleNoUse.onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn)
                        {
                            // 토글이 켜진 경우, "사용안함" 또는 특정 문자열로 설정
                            SettingNumPad.txtSetValue.text = nouseStr; // "사용안함" 또는 다른 특정 문자열
                            SettingNumPad.txtSetUnit.text = string.Empty;
                        }
                        else
                        {
                            // 토글이 꺼진 경우, 원래의 값(스케일 조정된 값)으로 설정
                            float originalValue = rawValue / float.Parse(multiply);
                            SettingNumPad.txtSetValue.text = originalValue.ToString();
                            SettingNumPad.txtSetUnit.text = $"{unit}";
                        }
                    });
                }
            }
            else
            {
                if (rawValue == int.Parse(nouse))
                {
                    // 사용안함 값에 대한 고정 문자열이 있고 현재 값과 동일한 경우
                    SettingNumPad.toggleNoUse.interactable = true;
                    SettingNumPad.toggleNoUse.isOn = true;
                    SettingNumPad.txtCurrentValue.text = "사용안함";
                    SettingNumPad.txtNoUseStr.text = "사용안함";

                    SettingNumPad.txtSetValue.text = "사용안함";
                    SettingNumPad.txtSetUnit.text = string.Empty;

                    SettingNumPad.toggleNoUse.onValueChanged.RemoveAllListeners();
                    SettingNumPad.toggleNoUse.onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn)
                        {
                            // 토글이 켜진 경우, "사용안함" 또는 특정 문자열로 설정
                            SettingNumPad.txtSetValue.text = "사용안함"; // "사용안함" 또는 다른 특정 문자열
                            SettingNumPad.txtSetUnit.text = string.Empty;
                        }
                        else
                        {
                            // 토글이 꺼진 경우, 원래의 값(스케일 조정된 값)으로 설정
                            float originalValue = rawValue / float.Parse(multiply);
                            SettingNumPad.txtSetValue.text = originalValue.ToString();
                            SettingNumPad.txtSetUnit.text = $"{unit}";
                        }
                    });
                }
                else
                {
                    // 사용안함 값에 대한 고정 문자열이 있지만 현재 값과 동일하지 않은 경우
                    SettingNumPad.toggleNoUse.interactable = true;
                    SettingNumPad.toggleNoUse.isOn = false;
                    SettingNumPad.txtNoUseStr.text = "사용안함";

                    SettingNumPad.txtSetValue.text = $"{scaledValue}";
                    SettingNumPad.txtSetUnit.text = $"{unit}";

                    SettingNumPad.toggleNoUse.onValueChanged.RemoveAllListeners();
                    SettingNumPad.toggleNoUse.onValueChanged.AddListener((isOn) =>
                    {
                        if (isOn)
                        {
                            // 토글이 켜진 경우, "사용안함" 또는 특정 문자열로 설정
                            SettingNumPad.txtSetValue.text = "사용안함"; // "사용안함" 또는 다른 특정 문자열
                            SettingNumPad.txtSetUnit.text = string.Empty;
                        }
                        else
                        {
                            // 토글이 꺼진 경우, 원래의 값(스케일 조정된 값)으로 설정
                            float originalValue = rawValue / float.Parse(multiply);
                            SettingNumPad.txtSetValue.text = originalValue.ToString();
                            SettingNumPad.txtSetUnit.text = $"{unit}";
                        }
                    });
                }

                if (currentPkey == "UC0815120104610507")
                {
                    int delayUnit = parsedPollingData[9];
                    if (int.Parse(tagAttributes["addr"]) == 210)
                    {
                        if (delayUnit == 0)
                        {
                            SettingNumPad.txtCurrentValue.text = $"{rawValue}{unit}";
                            SettingNumPad.txtSetValue.text = $"{(parsedPollingData[10]).ToString()}";
                        }
                        else if (delayUnit == 1)
                        {
                            SettingNumPad.txtCurrentValue.text = $"{rawValue / 60}{unit}";
                            SettingNumPad.txtSetValue.text = $"{(parsedPollingData[10] / 60).ToString()}";
                        }
                    }
                    else if (int.Parse(tagAttributes["addr"]) == 213)
                    {
                        if (delayUnit == 0)
                        {
                            SettingNumPad.txtCurrentValue.text = $"{rawValue}{unit}";
                            SettingNumPad.txtSetValue.text = $"{(parsedPollingData[13]).ToString()}";
                        }
                        else if (delayUnit == 1)
                        {
                            SettingNumPad.txtCurrentValue.text = $"{rawValue / 60}{unit}";
                            SettingNumPad.txtSetValue.text = $"{(parsedPollingData[13] / 60).ToString()}";
                        }
                    }
                    else
                    {
                        SettingNumPad.txtCurrentValue.text = $"{scaledValue}{unit}";
                        SettingNumPad.txtSetValue.text = $"{scaledValue}";
                    }
                }
                else
                {
                    SettingNumPad.txtCurrentValue.text = $"{scaledValue}{unit}";
                    SettingNumPad.txtSetValue.text = $"{scaledValue}";
                }
            }
        }
        else
        {
            // 사용안함 속성이 tag 태그에 존재하지 않을 경우
            SettingNumPad.toggleNoUse.interactable = false;
            SettingNumPad.toggleNoUse.isOn = false;
            SettingNumPad.txtNoUseStr.text = "사용안함";
        }

        SettingNumPad.btnOK.onClick.RemoveAllListeners(); // 기존 리스너 제거
        SettingNumPad.btnOK.onClick.AddListener(() =>
        {
            if (SettingNumPad.txtSetValue.text == "사용안함" || SettingNumPad.txtSetValue.text == SettingNumPad.txtNoUseStr.text)
            {
                string jsonString = string.Empty;
                string hexValue = string.Empty; // 16진수 값을 저장할 변수

                if (SettingNumPad.toggleNoUse.isOn)
                {
                    int setRawValue = (int)(float.Parse(nouse) * float.Parse(multiply));
                    hexValue = "0x" + ((ushort)setRawValue).ToString("X4");
                    jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Multiply\":\"{multiply}\",\"Size\":\"{size}\",\"Comment\":\"{tagAttributes["name"]}, {SettingNumPad.txtCurrentValue.text}에서 사용안함({nouse}{unit})(으)로 변경 : {addr}\"}}";
                }

                if (SettingNumPad.toggleNoUse.isOn && tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                {
                    int setRawValue = (int)(float.Parse(nouse) * float.Parse(multiply));
                    hexValue = "0x" + ((ushort)setRawValue).ToString("X4");
                    jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Multiply\":\"{multiply}\",\"Size\":\"{size}\",\"Comment\":\"{tagAttributes["name"]}, {SettingNumPad.txtCurrentValue.text}에서 {nouseStr}({nouse}{unit})(으)로 변경 : {addr}\"}}";
                }

                if (!SettingNumPad.toggleNoUse.isOn)
                {
                    int setRawValue = (int)(float.Parse(SettingNumPad.txtSetValue.text) * float.Parse(multiply));
                    hexValue = "0x" + ((ushort)setRawValue).ToString("X4");
                    jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Multiply\":\"{multiply}\",\"Size\":\"{size}\",\"Comment\":\"{tagAttributes["name"]}, {SettingNumPad.txtCurrentValue.text}에서 {SettingNumPad.txtSetValue.text}{unit}(으)로 변경 : {addr}\"}}";
                }

                if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
                {
                    SettingNumPad.InitNumpad();
                    setting_Numpad.SetActive(false);
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                }
            }
            else
            {
                if (float.Parse(SettingNumPad.txtSetValue.text) * float.Parse(multiply) < (float.Parse(scaledMinValue) * float.Parse(multiply)) || float.Parse(SettingNumPad.txtSetValue.text) * float.Parse(multiply) > float.Parse(scaledMaxValue) * float.Parse(multiply))
                {
                    //Debug.Log($"SettingNumPad.txtSetValue.text:{float.Parse(SettingNumPad.txtSetValue.text) * float.Parse(multiply)}, min:{float.Parse(scaledMinValue) * float.Parse(multiply)}, max:{float.Parse(scaledMaxValue) * float.Parse(multiply)}, multiply:{multiply}");
                    screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    screenManager.txt_PopUpMsg.text = "설정 범위 밖의 값이 입력되었습니다.";
                    screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                    screenManager.btnPopUpConfirm.onClick.AddListener(() => screenManager.ClosePopUpMessage());
                }
                else
                {
                    string jsonString = string.Empty;
                    string hexValue = string.Empty; // 16진수 값을 저장할 변수

                    if (SettingNumPad.toggleNoUse.isOn)
                    {
                        int setRawValue = (int)(float.Parse(nouse) * float.Parse(multiply));
                        //hexValue = "0x" + setRawValue.ToString("X4");
                        hexValue = "0x" + ((ushort)setRawValue).ToString("X4");
                        //Debug.Log($"{setRawValue} - {hexValue}");
                        jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Multiply\":\"{multiply}\",\"Size\":\"{size}\",\"Comment\":\"{tagAttributes["name"]}, {SettingNumPad.txtCurrentValue.text}에서 사용안함({nouse}{unit})(으)로 변경 : {addr}\"}}";
                    }

                    if (SettingNumPad.toggleNoUse.isOn && tagAttributes.TryGetValue("nouseStr", out var nouseStr))
                    {
                        int setRawValue = (int)(float.Parse(nouse) * float.Parse(multiply));
                        //hexValue = "0x" + setRawValue.ToString("X4");
                        hexValue = "0x" + ((ushort)setRawValue).ToString("X4");
                        //Debug.Log($"{setRawValue} - {hexValue}");
                        jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Multiply\":\"{multiply}\",\"Size\":\"{size}\",\"Comment\":\"{tagAttributes["name"]}, {SettingNumPad.txtCurrentValue.text}에서 {nouseStr}({nouse}{unit})(으)로 변경 : {addr}\"}}";
                    }

                    if (!SettingNumPad.toggleNoUse.isOn)
                    {
                        int setRawValue = (int)(float.Parse(SettingNumPad.txtSetValue.text) * float.Parse(multiply));
                        //hexValue = "0x" + setRawValue.ToString("X4");
                        hexValue = "0x" + ((ushort)setRawValue).ToString("X4");
                        //Debug.Log($"{setRawValue} - {hexValue}");
                        jsonString = $"{{\"Address\":{addr},\"Value\":\"{hexValue}\",\"Multiply\":\"{multiply}\",\"Size\":\"{size}\",\"Comment\":\"{tagAttributes["name"]}, {SettingNumPad.txtCurrentValue.text}에서 {SettingNumPad.txtSetValue.text}{unit}(으)로 변경 : {addr}\"}}";
                    }

                    if (ControlManager.Instance.SendControlCommand(iid, cid, jsonString))
                    {
                        SettingNumPad.InitNumpad();
                        setting_Numpad.SetActive(false);
                    }
                    else
                    {
                        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                        ScreenManager.Instance.txt_PopUpMsg.text = "설정에 실패했습니다.\n통신상태를 확인해주세요.";
                        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                    }
                }
            }
        });
    }

    // 하단 메뉴 그래프 그리기
    public void DrawTrendGraph(string iid, string cid, string addr, string name, string unit)
    {
        // 테이블 존재 여부 확인
        if (!ClientDatabase.TableExists($"TBL_TREND_{iid}_{cid}"))
        {
            //Debug.LogError($"테이블 TBL_TREND_{iid}_{cid}가 존재하지 않습니다.");
            return;
        }

        // 기존의 차트 컴포넌트 찾기
        E2Chart myChart = graphContainer.GetComponent<E2Chart>();

        // 기존의 차트 컴포넌트가 없으면 새로 추가
        if (myChart == null)
            myChart = graphContainer.AddComponent<E2Chart>();
        else
            myChart.Clear();

        // 분 단위 집계에 대한 SQL 쿼리 실행 및 결과 DataSet 저장        
        string headInfo = string.Empty;
        string findHeadInfoQuery = $"SELECT HEAD_INFO FROM TBL_TREND_{iid}_{cid} ORDER BY LOG_DATE DESC, LOG_HOUR DESC, LOG_MINS DESC LIMIT 1;";
        DataSet headInfoDataSet = ClientDatabase.OnSelectRequest(findHeadInfoQuery, $"TBL_TREND_{iid}_{cid}");
        if (headInfoDataSet == null || headInfoDataSet.Tables.Count == 0)
        {
            return; // 데이터셋이 null이거나 테이블이 없는 경우 리턴
        }
        DataTable table = headInfoDataSet.Tables[0];
        foreach (DataRow row in table.Rows)
        {
            headInfo = row["HEAD_INFO"].ToString();
            //Debug.Log($"DetailView : TBL_TREND_{iid}_{cid}'s latest HEAD_INFO : {headInfo}");
        }

        string[] headInfoParts = headInfo.Split(',');
        int dataIndex = -1;
        for (int i = 0; i < headInfoParts.Length; i++)
        {
            string[] parts = headInfoParts[i].Split('|');
            if (parts[1] == $"{addr}" && parts[0] == $"{name}" && parts[2] == $"{unit}")
            {
                dataIndex = i; // 데이터 인덱스 찾음
                //Debug.Log($"DetailView : inquiry modbus address {addr}'s DATA field index : {dataIndex}");
                break;
            }
        }

        string drawGraphQuery = $@"
                                SELECT 
                                    DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(FLOOR(LOG_HOUR / 2) * 2, 2, '0')), '%m-%d %H시') AS TimeGroup, 
                                    AVG(DATA{dataIndex}) AS AvgData 
                                FROM 
                                    TBL_TREND_{iid}_{cid} 
                                WHERE 
                                    LOG_DATE > DATE_SUB(CURDATE(), INTERVAL 1 DAY) OR 
                                    (LOG_DATE = DATE_SUB(CURDATE(), INTERVAL 1 DAY) AND LOG_HOUR >= HOUR(NOW()) AND LOG_MINS >= MINUTE(NOW()))
                                GROUP BY 
                                    TimeGroup 
                                ORDER BY 
                                    TimeGroup;
                                ";

        DataSet drawGraphDataSet = ClientDatabase.OnSelectRequest(drawGraphQuery, $"TBL_TREND_{iid}_{cid}");
        if (drawGraphDataSet == null || drawGraphDataSet.Tables.Count == 0)
        {
            return; // 데이터셋이 null이거나 테이블이 없는 경우 리턴
        }
        else
        {
            // Chart component 추가            
            myChart.chartType = E2Chart.ChartType.LineChart;

            // Chart options 추가
            myChart.chartOptions = myChart.gameObject.AddComponent<E2ChartOptions>();
            myChart.chartOptions.title.enableTitle = true;
            myChart.chartOptions.title.enableSubTitle = false;
            myChart.chartOptions.title.titleTextOption.font = font_Pretendard_Bold;
            myChart.chartOptions.yAxis.enableTitle = true;
            myChart.chartOptions.yAxis.titleTextOption.font = font_Pretendard_Bold;
            myChart.chartOptions.yAxis.labelTextOption.font = font_Pretendard_Bold;
            myChart.chartOptions.xAxis.titleTextOption.font = font_Pretendard_Bold;
            myChart.chartOptions.xAxis.interval = 1;
            myChart.chartOptions.xAxis.enableGridLine = false;
            myChart.chartOptions.xAxis.labelTextOption.font = font_Pretendard_Bold;
            myChart.chartOptions.label.enable = false;
            myChart.chartOptions.legend.enable = true;
            myChart.chartOptions.legend.textOption.font = font_Pretendard_Bold;
            myChart.chartOptions.legend.textOption.fontSize = 16;
            myChart.chartOptions.chartStyles.barChart.barWidth = 15.0f;
            myChart.chartOptions.chartStyles.lineChart.pointSize = 5f;
            myChart.chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
            myChart.chartOptions.rectOptions.enableZoom = true;
            myChart.chartOptions.tooltip.textOption.font = font_Pretendard_Bold;
            myChart.chartOptions.chartStyles.lineChart.splineCurve = true;

            // Chart data 추가
            myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
            myChart.chartData.series = new List<E2ChartData.Series>(); // 시리즈 리스트 초기화
            myChart.chartData.title = $""; // 제목 변경
            myChart.chartData.yAxisTitle = $"({unit})"; // Y축 제목 변경
            myChart.chartData.xAxisTitle = "시간";

            // 시간 데이터 추출
            List<string> times = new List<string>();
            foreach (DataRow row in drawGraphDataSet.Tables[0].Rows)
            {
                times.Add(row["TimeGroup"].ToString());
            }
            myChart.chartData.categoriesX = times; // X축 카테고리 설정

            // 데이터 시리즈 생성
            E2ChartData.Series newSeries = new E2ChartData.Series();
            newSeries.name = $"{name}"; // 여기 이름으로 치환해야함
            newSeries.dataY = new List<float>();
            foreach (DataRow row in drawGraphDataSet.Tables[0].Rows)
            {
                newSeries.dataY.Add(float.Parse(row["AvgData"].ToString()));
            }

            // 시리즈 리스트에 추가
            myChart.chartData.series.Add(newSeries);

            // 차트 업데이트
            myChart.UpdateChart();
        }

        
    }

    public void DrawMultipleMergedTrendGraph(string iid, string cid, string elementName)
    {
        // 테이블 존재 여부 확인
        if (!ClientDatabase.TableExists($"TBL_TREND_{iid}_{cid}"))
        {
            //Debug.LogError($"테이블 TBL_TREND_{iid}_{cid}가 존재하지 않습니다.");
            return;
        }

        List<DataTable> listTrendTables = new List<DataTable>();
        List<string> chartName = new List<string>();

        // 기존의 차트 컴포넌트 찾기
        E2Chart myChart = graphContainer.GetComponent<E2Chart>();

        // 기존의 차트 컴포넌트가 없으면 새로 추가
        if (myChart == null)
            myChart = graphContainer.AddComponent<E2Chart>();
        else
            myChart.Clear();

        // 분 단위 집계에 대한 SQL 쿼리 실행 및 결과 DataSet 저장        
        string headInfo = string.Empty;
        string findHeadInfoQuery = $"SELECT HEAD_INFO FROM TBL_TREND_{iid}_{cid} ORDER BY LOG_DATE DESC, LOG_HOUR DESC, LOG_MINS DESC LIMIT 1;";
        DataSet headInfoDataSet = ClientDatabase.OnSelectRequest(findHeadInfoQuery, $"TBL_TREND_{iid}_{cid}");
        if (headInfoDataSet == null || headInfoDataSet.Tables.Count == 0)
        {
            return; // 데이터셋이 null이거나 테이블이 없는 경우 리턴
        }
        DataTable table = headInfoDataSet.Tables[0];
        foreach (DataRow row in table.Rows)
        {
            headInfo = row["HEAD_INFO"].ToString();
            //Debug.Log($"DetailView : TBL_TREND_{iid}_{cid}'s latest HEAD_INFO : {headInfo}");
        }

        string[] headInfoParts = headInfo.Split(',');
        int dataIndex = -1;
        for (int i = 0; i < headInfoParts.Length; i++)
        {
            string[] parts = headInfoParts[i].Split('|');
            if (elementName.Contains(parts[1]))
            {
                dataIndex = i; // 데이터 인덱스 찾음
                chartName.Add(parts[0]);
                //Debug.Log($"DetailView : inquiry modbus address {parts[1]}'s DATA field index : {dataIndex}");

                string drawGraphQuery = $@"
                                SELECT 
                                    DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(FLOOR(LOG_HOUR / 2) * 2, 2, '0')), '%m-%d %H시') AS TimeGroup, 
                                    AVG(DATA{dataIndex}) AS AvgData 
                                FROM 
                                    TBL_TREND_{iid}_{cid} 
                                WHERE 
                                    LOG_DATE > DATE_SUB(CURDATE(), INTERVAL 1 DAY) OR 
                                    (LOG_DATE = DATE_SUB(CURDATE(), INTERVAL 1 DAY) AND LOG_HOUR >= HOUR(NOW()) AND LOG_MINS >= MINUTE(NOW()))
                                GROUP BY 
                                    TimeGroup 
                                ORDER BY 
                                    TimeGroup;
                                ";

                listTrendTables.Add(ClientDatabase.OnSelectRequest(drawGraphQuery, $"TBL_TREND_{iid}_{cid}").Tables[0]);
            }
        }

        DataTable mergedTable = InquiryManager.MergeMultipleDataTablesLINQ(listTrendTables);
        if (Application.platform != RuntimePlatform.Android)
        {
            //Debug.Log(listTrendTables.Count);
        }


        // Chart component 추가            
        myChart.chartType = E2Chart.ChartType.LineChart;

        // Chart options 추가
        myChart.chartOptions = myChart.gameObject.AddComponent<E2ChartOptions>();
        myChart.chartOptions.title.enableTitle = true;
        myChart.chartOptions.title.enableSubTitle = false;
        myChart.chartOptions.title.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.yAxis.enableTitle = true;
        myChart.chartOptions.yAxis.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.yAxis.labelTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.xAxis.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.xAxis.interval = 1;
        myChart.chartOptions.xAxis.enableGridLine = false;
        myChart.chartOptions.xAxis.labelTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.label.enable = false;
        myChart.chartOptions.legend.enable = true;
        myChart.chartOptions.legend.textOption.font = font_Pretendard_Bold;
        myChart.chartOptions.legend.textOption.fontSize = 16;
        myChart.chartOptions.chartStyles.barChart.barWidth = 15.0f;
        myChart.chartOptions.chartStyles.lineChart.pointSize = 5f;
        myChart.chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
        myChart.chartOptions.rectOptions.enableZoom = true;
        myChart.chartOptions.tooltip.textOption.font = font_Pretendard_Bold;
        myChart.chartOptions.chartStyles.lineChart.splineCurve = true;

        // Chart data 추가
        myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
        myChart.chartData.series = new List<E2ChartData.Series>(); // 시리즈 리스트 초기화
        myChart.chartData.title = $""; // 제목 변경
        myChart.chartData.yAxisTitle = $"실제 값"; // Y축 제목 변경
        myChart.chartData.xAxisTitle = "시간";

        // 시간 데이터 추출
        List<string> times = new List<string>();
        foreach (DataRow row in mergedTable.Rows)
        {
            times.Add(row["TimeGroup"].ToString());
        }
        myChart.chartData.categoriesX = times; // X축 카테고리 설정

        for (int i = 1; i <= listTrendTables.Count; i++)
        {
            // 데이터 시리즈 생성
            E2ChartData.Series newSeries = new E2ChartData.Series();
            newSeries.name = $"{chartName[i - 1]}"; // 여기 이름으로 치환해야함
            newSeries.dataY = new List<float>();
            foreach (DataRow row in mergedTable.Rows)
            {
                newSeries.dataY.Add(float.Parse(row[$"AvgData{i}"].ToString()));
            }

            // 시리즈 리스트에 추가
            myChart.chartData.series.Add(newSeries);
        }


        // 차트 업데이트
        myChart.UpdateChart();
    }

    // 중앙 위젯의 설정값 요소 업데이트
    private void UpdateSetValWidgetInstance(GameObject widgetInstance, string addr, string tagName, string unit, string multiply, string setAddr)
    {
        // 예시: 현재값과 단위를 업데이트합니다.
        TextMeshProUGUI trendName = widgetInstance.transform.Find("txt_Title").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trendValue = widgetInstance.transform.Find("obj_Value/txt_Value").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trendUnit = widgetInstance.transform.Find("obj_Value/txt_Unit").gameObject.GetComponent<TextMeshProUGUI>();

        if (!widgetInstance.name.Contains("AO"))
        {
            if (showAddr.Contains(addr))
                widgetInstance.gameObject.SetActive(true);
            else
                widgetInstance.gameObject.SetActive(false);
        }

        int addressIndex = int.Parse(addr) < 200 ? int.Parse(addr) : int.Parse(addr) - 200;
        // 원시 데이터 값을 올바르게 처리
        int rawData = parsedPollingData[addressIndex];
        if (rawData >= 32768)
        { // 16비트 정수에서 음수 값 처리
            rawData -= 65536;
        }

        float newValue = rawData / float.Parse(multiply);
        float oldValue = float.Parse(trendValue.text);

        ChangeWidgetColor(widgetInstance, trendName, trendValue, trendUnit, oldValue, newValue);

        trendName.text = tagName;
        if (multiply == "1.0" || multiply == "1")
            trendValue.text = newValue.ToString();
        else if (multiply == "10.0" || multiply == "10")
            trendValue.text = newValue.ToString("F1");
        else if (multiply == "100.0" || multiply == "100")
            trendValue.text = newValue.ToString("F2");
        trendUnit.text = unit;

        // 설정값이 있을 경우 설정값도 업데이트
        if (widgetInstance.transform.Find("obj_SetValue") != null)
        {
            widgetInstance.transform.Find("obj_SetValue").gameObject.SetActive(true);
            TextMeshProUGUI trendSetValue = widgetInstance.transform.Find("obj_SetValue/txt_Value").gameObject.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI trendSetUnit = widgetInstance.transform.Find("obj_SetValue/txt_Unit").gameObject.GetComponent<TextMeshProUGUI>();

            int setAddressIndex = int.Parse(setAddr) < 200 ? int.Parse(setAddr) : int.Parse(setAddr) - 200;

            // 원시 데이터 값을 올바르게 처리
            int setRawData = parsedPollingData[setAddressIndex];
            if (setRawData >= 32768)
            { // 16비트 정수에서 음수 값 처리
                setRawData -= 65536;
            }
            float setValue = setRawData / float.Parse(multiply);

            if (multiply == "1.0" || multiply == "1")
                trendSetValue.text = (setValue).ToString();
            else if (multiply == "10.0" || multiply == "10")
                trendSetValue.text = (setValue).ToString("F1");
            else if (multiply == "100.0" || multiply == "100")
                trendSetValue.text = (setValue).ToString("F2");

            trendSetUnit.text = unit;
        }
    }

    private void ChangeWidgetColor(GameObject widgetInstance, TextMeshProUGUI trendName, TextMeshProUGUI trendValue, TextMeshProUGUI trendUnit, float oldValue, float newValue)
    {
        float percentageChange = (newValue - oldValue) / oldValue * 100; // 변화율 계산

        if (newValue != oldValue)
        {
            // 값이 변경되었다면 색상을 연두색으로 변경하고, 0.5초 후에 다시 흰색으로 되돌립니다.
            Image widgetImage = widgetInstance.GetComponent<Image>(); // Image 컴포넌트 참조

            if (widgetImage != null)
            {
                // 트렌드명 흰색-(0.9초)->normalBlackColor, normalBlackColor-(0.9초)->흰색
                trendName.DOColor(Color.white, 0.9f).OnComplete(() => trendName.DOColor(normalBlackColor, 0.9f));
                // 트렌드 값 흰색-(0.9초)->normalTrendColor, normalTrendColor-(0.9초)->흰색
                trendValue.DOColor(Color.white, 0.9f).OnComplete(() => trendValue.DOColor(normalTrendColor, 0.9f));
                // 단위 흰색-(0.9초)->normalUnitColor, normalUnitColor-(0.9초)->흰색
                trendUnit.DOColor(Color.white, 0.9f).OnComplete(() => trendUnit.DOColor(normalUnitColor, 0.9f));

                // 위젯 배경 흰색-(0.9초)->연두, 연두-(0.9초)->흰색
                widgetImage.DOColor(normalTrendColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));

                //// 변화율에 따라 색상 결정
                //if (percentageChange >= 5)
                //{
                //    // 5% 이상 상승
                //    // 위젯 배경 흰색-(0.9초)->연두, 연두-(0.9초)->흰색
                //    widgetImage.DOColor(up5PercentColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                //}
                //else if (percentageChange >= 3)
                //{
                //    // 3% 이상 상승
                //    // 위젯 배경 흰색-(0.9초)->연두, 연두-(0.9초)->흰색
                //    widgetImage.DOColor(up3PercentColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                //}
                //else if (percentageChange <= -5)
                //{
                //    // 5% 이상 하락
                //    // 위젯 배경 흰색-(0.9초)->연두, 연두-(0.9초)->흰색
                //    widgetImage.DOColor(down5PercentColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                //}
                //else if (percentageChange <= -3)
                //{
                //    // 3% 이상 하락
                //    // 위젯 배경 흰색-(0.9초)->연두, 연두-(0.9초)->흰색
                //    widgetImage.DOColor(down3PercentColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                //}
                //else
                //{
                //    // 위젯 배경 흰색-(0.9초)->연두, 연두-(0.9초)->흰색
                //    widgetImage.DOColor(normalTrendColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                //}
            }
        }
    }

    // 중앙 위젯의 현재값 요소 업데이트
    private void UpdateWidgetInstance(GameObject widgetInstance, string addr, string tagName, string unit, string multiply)
    {
        TextMeshProUGUI trendName = widgetInstance.transform.Find("txt_Title").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trendValue = widgetInstance.transform.Find("obj_Value/txt_Value").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI trendUnit = widgetInstance.transform.Find("obj_Value/txt_Unit").gameObject.GetComponent<TextMeshProUGUI>();

        if (!widgetInstance.name.Contains("AO"))
        {
            if (showAddr.Contains(addr))
                widgetInstance.gameObject.SetActive(true);
            else
                widgetInstance.gameObject.SetActive(false);
        }        

        int addressIndex = int.Parse(addr) < 200 ? int.Parse(addr) : int.Parse(addr) - 200;
        float newValue = parsedPollingData[addressIndex];
        if (newValue >= 32768)
        { // 16비트 정수에서 음수 값 처리
            newValue -= 65536;
        }
        float oldValue = float.Parse(trendValue.text);

        trendName.text = tagName;
        if (multiply == "1.0" || multiply == "1")
            trendValue.text = (newValue / float.Parse(multiply)).ToString();
        else if (multiply == "10.0" || multiply == "10")
            trendValue.text = (newValue / float.Parse(multiply)).ToString("F1");
        else if (multiply == "100.0" || multiply == "100")
            trendValue.text = (newValue / float.Parse(multiply)).ToString("F2");
        trendUnit.text = unit;

        ChangeWidgetColor(widgetInstance, trendName, trendValue, trendUnit, oldValue, float.Parse(trendValue.text));

        // 설정값이 있을 경우 설정값도 업데이트
        if (widgetInstance.transform.Find("obj_SetValue") != null)
        {
            widgetInstance.transform.Find("obj_SetValue").gameObject.SetActive(false);
        }
    }

    // 중앙 위젯의 설정값 요소 초기화
    private void InitializeSetValWidgetInstance(GameObject widgetInstance, string addr, string tagName, string unit, string multiply, string setAddr)
    {
        // 위젯 인스턴스의 초기 설정을 수행하는 로직 (위와 유사)
        UpdateSetValWidgetInstance(widgetInstance, addr, tagName, unit, multiply, setAddr);
    }

    // 중앙 위젯의 현재값 요소 초기화
    private void InitializeWidgetInstance(GameObject widgetInstance, string addr, string tagName, string unit, string multiply)
    {
        // 위젯 인스턴스의 초기 설정을 수행하는 로직 (위와 유사)
        UpdateWidgetInstance(widgetInstance, addr, tagName, unit, multiply);
    }

    // 상위, 하위, 컨트롤러 이름 업데이트
    private void UpdateUI(DataSet dVControllerData, string iid, string cid, string pkey)
    {
        status = ClientDatabase.GetControllerStatus(iid, cid);
        if (currentDV != null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => ParsePollingData(iid, cid));
            UnityMainThreadDispatcher.Instance().Enqueue(() => NameUpdate(dVControllerData, iid, cid)); // 죄측상단 상위, 하위, 컨트롤러명 업데이트
            UnityMainThreadDispatcher.Instance().Enqueue(() => BtnStateUpdate(iid, cid, pkey)); // 우측상단 버튼 상태 업데이트
            UnityMainThreadDispatcher.Instance().Enqueue(() => StateWidgetUpdate(iid, cid, pkey)); // 중앙의 기본 상태위젯 상태 업데이트
        }
        FinalizeUIUpdate();
    }

    // 우측상단 버튼 상태 업데이트
    private void BtnStateUpdate(string iid, string cid, string pkey)
    {
        GameObject imgRunOn = currentDV.transform.Find("Top/TopRight/btn_OperateRun/Img_On").gameObject;
        GameObject imgRunOff = currentDV.transform.Find("Top/TopRight/btn_OperateRun/Img_Off").gameObject;
        GameObject imgDefrostOn = currentDV.transform.Find("Top/TopRight/btn_OperateDefrost/Img_On").gameObject;
        GameObject imgDefrostOff = currentDV.transform.Find("Top/TopRight/btn_OperateDefrost/Img_Off").gameObject;
        GameObject imgAlarmOn = currentDV.transform.Find("Top/TopRight/btn_Alarm/Img_On").gameObject;
        GameObject imgAlarmOff = currentDV.transform.Find("Top/TopRight/btn_Alarm/Img_Off").gameObject;

        if (status.IsRun)
        {
            imgRunOn.SetActive(true);
            imgRunOff.SetActive(false);
        }
        else
        {
            imgRunOn.SetActive(false);
            imgRunOff.SetActive(true);
        }

        if (status.IsDefrost)
        {
            imgDefrostOn.SetActive(true);
            imgDefrostOff.SetActive(false);
        }
        else
        {
            imgDefrostOn.SetActive(false);
            imgDefrostOff.SetActive(true);
        }

        if (status.IsAlarm || status.IsConnChecking || status.IsConnTrying)
        {
            imgAlarmOn.SetActive(true);
            imgAlarmOff.SetActive(false);
        }
        else
        {
            imgAlarmOn.SetActive(false);
            imgAlarmOff.SetActive(true);
        }

        var allAttributes = XMLParser.Instance.GetAllXMLAttributes(XMLParser.Instance.xmlContent);
        if (allAttributes.ContainsKey("groups"))
        {
            var groups = (Dictionary<string, Dictionary<string, object>>)allAttributes["groups"];
            foreach (var group in groups)
            {
                var groupAttributes = (Dictionary<string, string>)group.Value["attributes"];
                if (groupAttributes.TryGetValue("alt", out var altValue2) && altValue2 == "system" || altValue2 == "alarm" || altValue2 == "output" || altValue2 == "input" || altValue2 == "state")
                {
                    var stateTags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    foreach (var tag in stateTags)
                    {
                        if (tag.Value["style"] == "bitState")
                        {
                            // "bit" 뒤에 숫자만 있는 속성을 필터링
                            var bitAttributes = tag.Value
                                .Where(k => Regex.IsMatch(k.Key, @"^bit\d+$"))
                                .ToList();

                            if (bitAttributes.Any())
                            {
                                foreach (var bitAttr in bitAttributes)
                                {
                                    // "bit" 다음 오는 숫자를 인덱스로 사용
                                    string bitNumberStr = bitAttr.Key.Replace("bit", "");
                                    string powerOpBitIndex = string.Empty;
                                    string defrostOpBitIndex = string.Empty;

                                    // 운전 및 제상에 대한 비트 인덱스 할당
                                    if (tag.Value.TryGetValue("powerOP", out var powerOPBit))
                                        powerOpBitIndex = tag.Value["powerOP"].Replace("bit", "");
                                    if (tag.Value.TryGetValue("defrostOP", out var defrostOPBit))
                                        defrostOpBitIndex = tag.Value["defrostOP"].Replace("bit", "");

                                    if (int.TryParse(bitNumberStr, out var bitIndex))
                                    {
                                        if (pkey == "07152101-011-00-170" && tag.Value["addr"] == "224" && bitIndex == 0)
                                        {
                                            // 비트 값에 따른 토글 상태 설정
                                            int rawValue = int.Parse(tag.Value["addr"]) < 200 ? parsedPollingData[int.Parse(tag.Value["addr"])] : parsedPollingData[int.Parse(tag.Value["addr"]) - 200];
                                            bool isBitSet = (rawValue & (1 << bitIndex)) != 0;
                                            //Debug.Log($"{iid}, {cid}, {rawValue}, {isBitSet}");
                                            if (isBitSet)
                                            {
                                                imgRunOn.SetActive(true);
                                                imgRunOff.SetActive(false);
                                            }
                                        }

                                        if (pkey == "07152101-011-00-170" && tag.Value["addr"] == "224" && bitIndex == 1)
                                        {
                                            // 비트 값에 따른 토글 상태 설정
                                            int rawValue = int.Parse(tag.Value["addr"]) < 200 ? parsedPollingData[int.Parse(tag.Value["addr"])] : parsedPollingData[int.Parse(tag.Value["addr"]) - 200];
                                            bool isBitSet = (rawValue & (1 << bitIndex)) != 0;
                                            if (isBitSet)
                                            {
                                                imgDefrostOn.SetActive(true);
                                                imgDefrostOff.SetActive(false);
                                            }

                                        }

                                        if (pkey == "02240601-001-00-208" && tag.Value["addr"] == "238" && bitIndex == 0)
                                        {
                                            // 비트 값에 따른 토글 상태 설정
                                            int rawValue = int.Parse(tag.Value["addr"]) < 200 ? parsedPollingData[int.Parse(tag.Value["addr"])] : parsedPollingData[int.Parse(tag.Value["addr"]) - 200];
                                            bool isBitSet = (rawValue & (1 << bitIndex)) != 0;
                                            if (isBitSet)
                                            {
                                                imgRunOn.SetActive(true);
                                                imgRunOff.SetActive(false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Debug.LogError($"Invalid bit index format: {bitAttr.Key}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // 중앙의 기본 상태위젯 상태 업데이트
    private void StateWidgetUpdate(string iid, string cid, string pkey)
    {
        // 1. 운전정지
        // 2. 운전중, 난방중, 가습중
        // 3. 운전중, 난방중, 제습중
        // 4. 운전중, 냉방중, 가습중
        // 5. 운전중, 냉방중, 제습중
        // 6. 운전중, 난방중, 제상중
        // 7. 운전중, 냉방중, 제상중

        status = ClientDatabase.GetControllerStatus(iid, cid);

        GameObject imgStop = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Stop").gameObject;
        GameObject imgRun = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Run").gameObject;
        GameObject imgDefrost = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Defrost").gameObject;
        GameObject imgCool = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Cool").gameObject;
        GameObject imgHeat = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Heat").gameObject;
        GameObject imgHumi = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Humi").gameObject;
        GameObject imgDehumi = currentDV.transform.Find("Center/WidgetParent/MainWidget_ScrollView/Viewport/Content/DVWidget_State/StatesParent/State_Dehumi").gameObject;

        if (status.IsDefrost)
            imgDefrost.SetActive(true);
        else
            imgDefrost.SetActive(false);

        if (status.IsCool)
            imgCool.SetActive(true);
        else
            imgCool.SetActive(false);

        if (status.IsHeat)
            imgHeat.SetActive(true);
        else
            imgHeat.SetActive(false);

        if (status.IsHumi)
            imgHumi.SetActive(true);
        else
            imgHumi.SetActive(false);

        if (status.IsDehumi)
            imgDehumi.SetActive(true);
        else
            imgDehumi.SetActive(false);

        //if (pkey != "UC0224150200401102")
        //{
        //    if (status.IsHeat)
        //        imgHeat.SetActive(true);
        //    else
        //        imgHeat.SetActive(false);

        //    if (status.IsHumi)
        //        imgHumi.SetActive(true);
        //    else
        //        imgHumi.SetActive(false);

        //    if (status.IsDehumi)
        //        imgDehumi.SetActive(true);
        //    else
        //        imgDehumi.SetActive(false);
        //}        

        if (status.IsRun)
        {
            if (status.IsDefrost || status.IsCool || status.IsHeat || status.IsHumi || status.IsDehumi)
            {
                imgRun.SetActive(false);
                imgStop.SetActive(false);
            }
            else
            {
                imgRun.SetActive(true);
                imgStop.SetActive(false);
            }
        }
        else
        {
            imgStop.SetActive(true);
            imgRun.SetActive(false);
            imgDefrost.SetActive(false);
            imgCool.SetActive(false);
            imgHeat.SetActive(false);
            imgHumi.SetActive(false);
            imgDehumi.SetActive(false);
        }

        var allAttributes = XMLParser.Instance.GetAllXMLAttributes(XMLParser.Instance.xmlContent);
        if (allAttributes.ContainsKey("groups"))
        {
            var groups = (Dictionary<string, Dictionary<string, object>>)allAttributes["groups"];
            foreach (var group in groups)
            {
                var groupAttributes = (Dictionary<string, string>)group.Value["attributes"];
                if (groupAttributes.TryGetValue("alt", out var altValue2) && altValue2 == "system" || altValue2 == "alarm" || altValue2 == "output" || altValue2 == "input" || altValue2 == "state")
                {
                    var stateTags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    foreach (var tag in stateTags)
                    {
                        if (tag.Value["style"] == "bitState")
                        {
                            // "bit" 뒤에 숫자만 있는 속성을 필터링
                            var bitAttributes = tag.Value
                                .Where(k => Regex.IsMatch(k.Key, @"^bit\d+$"))
                                .ToList();

                            if (bitAttributes.Any())
                            {
                                foreach (var bitAttr in bitAttributes)
                                {
                                    // "bit" 다음 오는 숫자를 인덱스로 사용
                                    string bitNumberStr = bitAttr.Key.Replace("bit", "");
                                    string powerOpBitIndex = string.Empty;
                                    string defrostOpBitIndex = string.Empty;

                                    // 운전 및 제상에 대한 비트 인덱스 할당
                                    if (tag.Value.TryGetValue("powerOP", out var powerOPBit))
                                        powerOpBitIndex = tag.Value["powerOP"].Replace("bit", "");
                                    if (tag.Value.TryGetValue("defrostOP", out var defrostOPBit))
                                        defrostOpBitIndex = tag.Value["defrostOP"].Replace("bit", "");

                                    if (int.TryParse(bitNumberStr, out var bitIndex))
                                    {
                                        bool defBit = false;
                                        if (pkey == "07152101-011-00-170" && tag.Value["addr"] == "224" && bitIndex == 1)
                                        {
                                            // 비트 값에 따른 토글 상태 설정
                                            int rawValue = int.Parse(tag.Value["addr"]) < 200 ? parsedPollingData[int.Parse(tag.Value["addr"])] : parsedPollingData[int.Parse(tag.Value["addr"]) - 200];
                                            defBit = (rawValue & (1 << bitIndex)) != 0;
                                        }
                                        else if (pkey == "07152101-011-00-170" && tag.Value["addr"] == "224" && bitIndex == 0)
                                        {
                                            // 비트 값에 따른 토글 상태 설정
                                            int rawValue = int.Parse(tag.Value["addr"]) < 200 ? parsedPollingData[int.Parse(tag.Value["addr"])] : parsedPollingData[int.Parse(tag.Value["addr"]) - 200];
                                            bool isBitSet = (rawValue & (1 << bitIndex)) != 0;
                                            if (isBitSet)
                                            {
                                                if (defBit || status.IsCool || status.IsHeat || status.IsHumi || status.IsDehumi)
                                                {
                                                    imgRun.SetActive(false);
                                                    imgStop.SetActive(false);
                                                }
                                                else
                                                {
                                                    imgRun.SetActive(true);
                                                    imgStop.SetActive(false);
                                                }
                                            }
                                            else
                                            {
                                                imgStop.SetActive(true);
                                                imgRun.SetActive(false);
                                                imgDefrost.SetActive(false);
                                                imgCool.SetActive(false);
                                                imgHeat.SetActive(false);
                                                imgHumi.SetActive(false);
                                                imgDehumi.SetActive(false);
                                            }
                                        }


                                    }
                                    else
                                    {
                                        //Debug.LogError($"Invalid bit index format: {bitAttr.Key}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // 컨트롤러명 & 그룹명 할당
    private void NameUpdate(DataSet dVControllerData, string iid, string cid)
    {
        if (currentDV != null && dVControllerData != null && dVControllerData.Tables.Count > 0 && dVControllerData.Tables[0].Rows.Count > 0 &&
            dVControllerData.Tables[0].Rows[0]["ID"].ToString() == iid && dVControllerData.Tables[0].Rows[0]["CID"].ToString() == cid)
        {
            string hgid = dVControllerData.Tables[0].Rows[0]["HGID"].ToString();
            string lgid = dVControllerData.Tables[0].Rows[0]["LGID"].ToString();
            string hgName = string.Empty;
            string lgName = string.Empty;

            foreach (DataRow hgroupRow in ClientDatabase.highGroupData.Tables[0].Rows)
            {
                if (hgroupRow["FLD_HGID"].ToString() == hgid)
                {
                    hgName = hgroupRow["FLD_NAME"].ToString();

                    foreach (DataRow lgroupRow in ClientDatabase.lowGroupData.Tables[0].Rows)
                    {
                        if (lgroupRow["FLD_HGID"].ToString() == hgid && lgroupRow["FLD_LGID"].ToString() == lgid)
                        {
                            lgName = lgroupRow["FLD_NAME"].ToString();
                            break;
                        }
                    }
                    break;
                }
            }
            if (!string.IsNullOrEmpty(hgName) && !string.IsNullOrEmpty(lgName))
            {
                TextMeshProUGUI highGroupName = currentDV.transform.Find("Top/TopLeft/txt_HighGroupName").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI lowGroupName = currentDV.transform.Find("Top/TopLeft/txt_LowGroupName").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI controllerName = currentDV.transform.Find("Top/TopLeft/txt_ControllerName").GetComponent<TextMeshProUGUI>();

                highGroupName.text = $"{hgName}";
                lowGroupName.text = $"{lgName}";
                controllerName.text = dVControllerData.Tables[0].Rows[0]["CNAME"].ToString();
            }
        }
    }

    private void FinalizeUIUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(mainWidgetScrollViewContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(aoWidgetScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // 하단 메뉴 활성/비활성화 버튼
    public void DVMenuStateChange()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(IStateChange());
    }

    // 하단 메뉴 활성/비활성화에 따른 DB 폴링 제어
    IEnumerator IStateChange()
    {
        dvMenuAnim = currentDV.GetComponent<Animator>();
        if (!isDVMenuOpen)
        {
            ClientDatabase.isPolling = false;
            btn_DvMenuClose.SetActive(true);
            btn_DvMenuOpen.SetActive(false);
            dvMenuAnim.SetBool("DVMenu_Idle", false);
            dvMenuAnim.SetBool("DVMenu_Open", true);
        }
        else
        {
            ClientDatabase.isPolling = false;
            btn_DvMenuClose.SetActive(false);
            btn_DvMenuOpen.SetActive(true);
            dvMenuAnim.SetBool("DVMenu_Open", false);
            dvMenuAnim.SetBool("DVMenu_Close", true);
        }
        isDVMenuOpen = !isDVMenuOpen;

        yield return waitSec;
        ClientDatabase.isPolling = true;
        currentCoroutine = null;
    }

    // 하단메뉴, 닫기, 운전, 제상 버튼에 대한 리스너 추가
    private void AddListenerForStaticButtons(string iid, string cid)
    {
        btn_DvMenuOpen = currentDV.transform.Find("Center/btn_DVMenuOpen").gameObject;
        btn_DvMenuClose = currentDV.transform.Find("Bottom/btn_DVMenuClose").gameObject;
        Button btn_Back = currentDV.transform.Find("Top/TopLeft/btn_Back").gameObject.GetComponent<Button>();

        btn_DvMenuOpen.SetActive(true);
        btn_DvMenuClose.SetActive(false);
        btn_DvMenuOpen.GetComponent<Button>().onClick.RemoveAllListeners();
        btn_DvMenuClose.GetComponent<Button>().onClick.RemoveAllListeners();
        btn_DvMenuOpen.GetComponent<Button>().onClick.AddListener(() => DVMenuStateChange());
        btn_DvMenuClose.GetComponent<Button>().onClick.AddListener(() => DVMenuStateChange());
        btn_Back.onClick.AddListener(() => CloseDetailView());

    }

    // DataSet이 서로 동일한지 체크
    private bool IsDataSetEqual(DataSet dataSet1, DataSet dataSet2)
    {
        if (dataSet1 == null || dataSet2 == null)
            return false;

        if (dataSet1.Tables.Count != dataSet2.Tables.Count)
            return false;

        for (int i = 0; i < dataSet1.Tables.Count; i++)
        {
            DataTable table1 = dataSet1.Tables[i];
            DataTable table2 = dataSet2.Tables[i];

            if (table1.Rows.Count != table2.Rows.Count || table1.Columns.Count != table2.Columns.Count)
                return false;

            for (int j = 0; j < table1.Rows.Count; j++)
            {
                for (int k = 0; k < table1.Columns.Count; k++)
                {
                    if (!table1.Rows[j][k].Equals(table2.Rows[j][k]))
                        return false;
                }
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        //Debug.Log("DetailView : DetailView Object has been Destroyed");
        if (manufacturingData != null)
        {
            manufacturingData.StopLoadingDataset();
        }
    }
}