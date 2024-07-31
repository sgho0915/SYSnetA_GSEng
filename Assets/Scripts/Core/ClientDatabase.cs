using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System;
using MySqlConnector;
using TMPro;
using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Security.Cryptography;
using static ScreenManager;
using System.Xml.Linq;
using UnityEngine.Networking;
using Unity.VisualScripting;
using System.Linq;
//using Renci.SshNet;
//using MySql.Data.MySqlClient;



public class ControllerStatus
{
    public bool IsConnChecking { get; set; } = false;
    public bool IsConnTrying { get; set; } = false;
    public bool IsConn { get; set; } = false;
    public bool IsRun { get; set; } = false;
    public bool IsPower { get; set; } = false;
    public bool IsAlarm { get; set; } = false;
    public bool IsDefrost { get; set; } = false;
    public bool IsCool { get; set; } = false;
    public bool IsFan { get; set; } = false;
    public bool IsHeat { get; set; } = false;
    public bool IsHumi { get; set; } = false;
    public bool IsDehumi { get; set; } = false;
}


public class ClientDatabase : MonoBehaviour
{
    ScreenManager screenManager;

    public static DataSet realTimeData;
    public static DataSet specificRealTimeData;
    public static DataSet specificControllerData;
    public static DataSet interfaceData;
    public static DataSet controllerData;
    public static DataSet highGroupData;
    public static DataSet lowGroupData;
    public static DataSet realTimeWarningData;
    public static DataSet protocolListData;
    public static DataSet configData;

    public GameObject controllerScrollViewGrid;
    public GameObject controllerScrollViewList;
    public static Transform controllerContentGrid;
    public static Transform controllerContentList;
    public static Transform controllerTrendContentGrid;
    public static Transform controllerTrendContentList;
    public GameObject imgGrid;
    public GameObject imgList;
    public TextMeshProUGUI txtErrCnt;
    public GameObject alarmCount;

    public List<Sprite> defrostImgList = new List<Sprite>();
    public List<Sprite> fanImgList = new List<Sprite>();
    public List<Sprite> controllerIconImgList = new List<Sprite>();
    private Dictionary<GameObject, Coroutine> defrostGridCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, Coroutine> defrostListCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, Coroutine> fanGridCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, Coroutine> fanListCoroutines = new Dictionary<GameObject, Coroutine>();
    private WaitForSeconds sec0_1 = new WaitForSeconds(0.15f);
    public int[] parsedPollingData;

    public static string db_HostAddress = string.Empty;
    public static string db_port = string.Empty;
    public static string db_ID = string.Empty;
    public static string db_PW = string.Empty;
    public static string db_Name = string.Empty;
    public static int db_MinPoolSize = 1;
    public static int db_MaxPoolSize = 20;
    public static string strConn = string.Empty;

    private int pollingTime;
    private WaitForSeconds pollingInterval = new(1f); // DB 폴링 간격
    private WaitForSeconds processInterval = new(0.05f); // DB 폴링 간격

    // 클래스 수준에서 controllerInstances 딕셔너리를 선언
    public static Dictionary<string, GameObject> controllerGridInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> controllerListInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> trendGridInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> trendListInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, ControllerStatus> controllerStatuses = new Dictionary<string, ControllerStatus>();

    public GameObject obj_ConsoleGUIController;
    public GameObject obj_startFade;

    public static bool isTestMode = false;
    public Button btnTestMode;

    public GameObject obj_Loading;
    private bool isLoading = true;
    public static bool isGridView = true;
    public static bool isPolling = false;
    bool roqudtlstoRlemf = false;

    string vendorName = string.Empty;
    string logoImg = string.Empty;
    string urlLogoImg = string.Empty;
    public Image logoImage;
    public TextMeshProUGUI logoText;
    public TextMeshProUGUI logoWelcomeText;

    Color alarmColor = new Color(240 / 255f, 94 / 255f, 91 / 255f, 1f); // f05e33, 투명도 1
    Color defrostColor = new Color(32 / 255f, 191 / 255f, 209 / 255f, 1f); // 20BFD1, 투명도 1
    Color colorBlue = new Color(0 / 255f, 169 / 255f, 196 / 255f, 1f);          // Blue #00A9C4
    Color runColor = new Color(0 / 255f, 178 / 255f, 127 / 255f, 1f); // 00B27f, 투명도 1
    Color stopColor = new Color(173 / 255f, 173 / 255f, 173 / 255f, 1f); // ADADAD, 투명도 1
    Color normalBlackColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1f); // 2D2D2D, 투명도 1
    Color stopBGColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f); // E3E3E3, 투명도 1
    Color noColor = new Color(0f, 0f, 0f, 0f); // ffffff, 투명도 0
    Color normalTrendColor = new Color(116 / 255f, 178 / 255f, 8 / 255f, 1f); // 74B208, 투명도 1

    Color down3PercentColor = new Color(111 / 255f, 218 / 255f, 224 / 255f, 1f); // 6FDAE0, 투명도 1
    Color down5PercentColor = new Color(111 / 255f, 148 / 255f, 224 / 255f, 1f); // 6F94E0, 투명도 1
    Color up3PercentColor = new Color(255 / 255f, 181 / 255f, 126 / 255f, 1f); // FFB57E, 투명도 1
    Color up5PercentColor = new Color(255 / 255f, 130 / 255f, 126 / 255f, 1f); // FF827E, 투명도 1
    Color normalUnitColor = new Color(153 / 255f, 153 / 255f, 153 / 255f, 1f); // 999999로, 투명도 1

    private bool isThemeLoad = false;

    private void Awake()
    {
        if (isTestMode)
        {
            Debug.Log("------------------------------------------------------------");
            Debug.Log("----------------------This is Test App----------------------");
            Debug.Log("------------------------------------------------------------");
            // 테스트 옵션 체크 시 로깅 GUI 표시
            obj_ConsoleGUIController.SetActive(true);
            btnTestMode.gameObject.SetActive(true);
            btnTestMode.onClick.AddListener(() => btnTestMode.gameObject.SetActive(false));
        }
        else
        {
            // 테스트 옵션 체크 시 로깅 GUI 표시
            obj_ConsoleGUIController.SetActive(false);
            btnTestMode.gameObject.SetActive(false);
        }
        DOTween.SetTweensCapacity(500, 50);
    }

    private IEnumerator Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            // 실제 출고시 로컬호스트 추종
            db_HostAddress = ConfigManager.Instance.GetSetting("SERVER_IP");
            db_port = ConfigManager.Instance.GetSetting("DB_PORT");
            db_ID = ConfigManager.Instance.GetSetting("DB_ID");
            db_PW = ConfigManager.Instance.GetSetting("DB_PW");
            db_Name = ConfigManager.Instance.GetSetting("DB_NAME");
            db_MinPoolSize = int.Parse(ConfigManager.Instance.GetSetting("DB_MIN_POOL_SIZE"));
            db_MaxPoolSize = int.Parse(ConfigManager.Instance.GetSetting("DB_MAX_POOL_SIZE"));
            strConn = $"server={db_HostAddress}; port={db_port}; user={db_ID}; password={db_PW}; database={db_Name}; charset=utf8; Pooling=true; Min Pool Size={db_MinPoolSize}; Max Pool Size={db_MaxPoolSize}; SslMode=none;";
            pollingTime = int.Parse(ConfigManager.Instance.GetSetting("DB_POLLING_INTERVAL")); // DB 폴링 간격
            pollingInterval = new WaitForSeconds(pollingTime);
        }
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            // 테스트시 외부접속
            db_HostAddress = "systronics1.iptime.org";
            //db_HostAddress = "localhost";
            db_port = "28367";
            //db_port = "12324";
            db_Name = "kiosk";
            db_ID = "root";
            db_PW = "!@#QWE123";
            db_MinPoolSize = 1;
            db_MaxPoolSize = 20;
            strConn = $"server={db_HostAddress}; port={db_port}; user={db_ID}; password={db_PW}; database={db_Name}; charset=utf8; Pooling=true; Min Pool Size={db_MinPoolSize}; Max Pool Size={db_MaxPoolSize}; SslMode=none;";
            pollingTime = 1; // DB 폴링 간격
            pollingInterval = new WaitForSeconds(pollingTime);
        }

        // 로딩 이미지 활성화
        obj_Loading.SetActive(true);
        obj_Loading.transform.Find("GameObject/txt_Loading").GetComponent<TextMeshProUGUI>().text = "컨트롤러 정보를 가져오고 있습니다.\n잠시만 기다려 주세요.";

        obj_startFade.SetActive(true);

        if (isLoading)
        {
            controllerScrollViewList.gameObject.SetActive(true);
        }

        Image image = obj_startFade.GetComponent<Image>();
        if (image != null)
        {
            // Image의 색상을 1초 동안 완전 불투명에서 완전 투명으로 변경합니다.
            image.DOFade(0f, 1f).OnComplete(() =>
            {
                // 애니메이션이 완료되면 obj_startFade를 비활성화합니다.
                obj_startFade.SetActive(false);
            });
        }

        string isFirstStart = ConfigManager.Instance.GetSetting("FIRSTSTART");

        screenManager = ScreenManager.Instance;

        yield return new WaitForSeconds(3f);

        configData = FetchConfigData();
        realTimeData = FetchRealTimeData();
        interfaceData = FetchInterfaceData();
        controllerData = FetchControllerData();
        highGroupData = FetchHighGroupData();
        lowGroupData = FetchLowGroupData();
        realTimeWarningData = FetchRealTimeWarningData();
        protocolListData = FetchProtocolList();
        isPolling = true;

       

        StartCoroutine(DBPolling());

        controllerContentGrid = screenManager.obj_MainScreen.transform.Find("obj_ControllerScrollViewGrid/Viewport/ContentGrid").gameObject.transform;
        controllerContentList = screenManager.obj_MainScreen.transform.Find("obj_ControllerScrollViewList/Viewport/ContentList").gameObject.transform;

        DOTween.SetTweensCapacity(1000, 100);
    }

    IEnumerator DownloadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            logoImage.gameObject.SetActive(false);
            logoText.text = vendorName;
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            logoImage.sprite = sprite;
            logoText.gameObject.SetActive(false);
        }
    }

    public void LoadTheme()
    {
        if (configData != null && configData.Tables.Count > 0)
        {
            DataTable tblConfig = configData.Tables[0];
            foreach (DataRow row in tblConfig.Rows)
            {
                vendorName = row["VENDOR_NAME"].ToString();
                logoImg = row["SEL_UI"].ToString();
                urlLogoImg = $"http://cloud.systronics.co.kr/img/def_skin/{logoImg}";

                logoWelcomeText.text = vendorName;

                if (logoImg.Length == 0)
                {
                    logoImage.gameObject.SetActive(false);
                    logoText.text = vendorName;
                }
                else
                {
                    StartCoroutine(DownloadImage(urlLogoImg));
                }
            }

            ColorThemeManager.Instance.ApplyTheme();
        }
    }

    private void ParsePollingData(string iid, string cid)
    {
        // 조건에 맞는 행의 데이터를 활용해 UI를 업데이트한다.
        foreach (DataRow row in realTimeData.Tables[0].Rows)
        {
            if (row["ID"].ToString() == iid && (row["CID"].ToString() == cid))
            {
                string[] stringData = row["STR_DATA"].ToString().Split(',');

                parsedPollingData = new int[stringData.Length];

                for (int i = 0; i < stringData.Length; i++)
                {
                    if (!int.TryParse(stringData[i], out parsedPollingData[i])) // int로 변환 시도
                    {
                        parsedPollingData[i] = -999; // 실패한 경우 기본값으로 -999를 사용합니다.
                    }
                }
            }

        }
    }

    IEnumerator DBPolling()
    {
        while (true)
        {
            if (!isThemeLoad)
            {
                //yield return new WaitUntil(() => ConfigManager.Instance.GetSetting("FIRSTSTART") == "false");
                LoadTheme();
                isThemeLoad = true;
            }

            if (isPolling)
            {
                configData = FetchConfigData();
                realTimeData = FetchRealTimeData();
                yield return processInterval;
                interfaceData = FetchInterfaceData();
                yield return processInterval;
                controllerData = FetchControllerData();
                yield return processInterval;
                highGroupData = FetchHighGroupData();
                yield return processInterval;
                lowGroupData = FetchLowGroupData();
                yield return processInterval;
                realTimeWarningData = FetchRealTimeWarningData();
                yield return processInterval;
                protocolListData = FetchProtocolList();
                yield return processInterval;
                UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateUI());                
                yield return processInterval;
                UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateFPPUI());
            }
            yield return pollingInterval;
        }
    }

    private void UpdateFPPUI()
    {
        if (screenManager.CurrentScreenState == ScreenState.FloorPlan)
        {
            foreach (DataRow rowCtrl in controllerData.Tables[0].Rows)
            {
                if (rowCtrl["FPP_GEN"].ToString() == "1")
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => FloorPlanManager.Instance.UpdateFloorPlanPanelUI(protocolListData, rowCtrl["HGID"].ToString(), rowCtrl["ID"].ToString(), rowCtrl["CID"].ToString()));
                }
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() => FloorPlanManager.Instance.UpdateFPBottomCount(controllerData, realTimeData)); // 평면도 하단 정보 업데이트
        }
    }

    // DB 폴링 주기마다 UI 갱신    
    private void UpdateUI()
    {
        
        if (controllerData.Tables.Count <= 0 || controllerData.Tables[0].Rows.Count <= 0)
        {
            Debug.LogError("controllerData is null");
            return;
        }
        else
        {
            if (screenManager.CurrentScreenState == ScreenState.Main || screenManager.CurrentScreenState == ScreenState.DetailView || screenManager.CurrentScreenState == ScreenState.FloorPlan)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateUIForControllers(controllerData)); // 메인화면 컨트롤러 인스턴스 UI 업데이트
                if (!roqudtlstoRlemf)
                {
                    //UnityMainThreadDispatcher.Instance().Enqueue(() => ControllerStyleManager.instance.UpdateStyle()); // 메인화면 컨트롤러 인스턴스 UI 업데이트
                    roqudtlstoRlemf = true;
                }
            }

            if (realTimeData.Tables.Count > 0 && realTimeData.Tables[0].Rows.Count > 0 && (screenManager.CurrentScreenState != ScreenState.None))
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => UpdateErrorCount(realTimeData)); // 에러 카운트 업데이트
            }

            if (realTimeWarningData.Tables.Count > 0 && realTimeWarningData.Tables[0].Rows.Count > 0 && (screenManager.CurrentScreenState != ScreenState.None))
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => AlarmPopUpManager.Instance.AlarmTracking(realTimeWarningData)); // 알람 팝업 업데이트
            }
        }
    }

    // 연결된 컨트롤러에 대한 대시보드 오브젝트 생성 및 UI 업데이트
    private void UpdateUIForControllers(DataSet controllerDataSet)
    {
        DataTable tblController = controllerDataSet.Tables[0];

        // GROUP_ORDER 기준으로 DataRow[] 정렬
        DataRow[] sortedRows = tblController.Select("", "ITEM_SORT ASC");

        foreach (DataRow row in sortedRows)
        {
            string iid = row["ID"].ToString();
            string cid = row["CID"].ToString();
            string order = row["ITEM_SORT"].ToString();
            string TargetName = $"Controller_{iid}_{cid}";

            if (isLoading)
            {
                if (!controllerGridInstances.ContainsKey(TargetName))
                {
                    GameObject controllerInstance = ObjectPool.Instance.GetControllerGridObject();
                    controllerInstance.name = TargetName;
                    controllerGridInstances[TargetName] = controllerInstance;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateGridControllers(controllerInstance, iid, cid));
                }

                if (!controllerListInstances.ContainsKey(TargetName))
                {
                    GameObject controllerInstance = ObjectPool.Instance.GetControllerListObject();
                    controllerInstance.name = TargetName;
                    controllerListInstances[TargetName] = controllerInstance;
                    UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateListControllers(controllerInstance, iid, cid));
                }
            }
            else
            {
                if (isGridView)
                {
                    if (!controllerGridInstances.ContainsKey(TargetName))
                    {
                        GameObject controllerInstance = ObjectPool.Instance.GetControllerGridObject();
                        controllerInstance.name = TargetName;
                        controllerGridInstances[TargetName] = controllerInstance;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateGridControllers(controllerInstance, iid, cid));
                    }
                    else
                    {
                        if(!ControllerStyleManager.bSetUse)
                            controllerGridInstances[TargetName].transform.SetSiblingIndex(int.Parse(order));
                    }
                }
                else
                {
                    if (!controllerListInstances.ContainsKey(TargetName))
                    {
                        GameObject controllerInstance = ObjectPool.Instance.GetControllerListObject();
                        controllerInstance.name = TargetName;
                        controllerListInstances[TargetName] = controllerInstance;
                        UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateListControllers(controllerInstance, iid, cid));
                    }
                    else
                    {
                        if (!ControllerStyleManager.bSetUse)
                            controllerListInstances[TargetName].transform.SetSiblingIndex(int.Parse(order));
                    }
                }
            }            
        }

        // 각 컨트롤러 인스턴스에 대해 UI 업데이트 수행
        if (isGridView)
        {
            foreach (var kvp in controllerGridInstances)
            {
                string[] parts = kvp.Key.Split('_');
                string iid = parts[1];
                string cid = parts[2];
                UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateGridControllers(kvp.Value, iid, cid));
            }
        }
        else
        {
            foreach (var kvp in controllerListInstances)
            {
                string[] parts = kvp.Key.Split('_');
                string iid = parts[1];
                string cid = parts[2];
                UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateListControllers(kvp.Value, iid, cid));
            }
        }

        IsLoadingFinished();
    }

    // Grid 컨트롤러 오브젝트 정보 갱신
    private void PopulateGridControllers(GameObject controllerInstance, string iid, string cid)
    {        
        string idKey = $"{iid}_{cid}";
        string pkey = string.Empty;
        string showAddr = string.Empty;
        string[] arrShowAddr = null;
        string hgid = string.Empty;
        string lgid = string.Empty;
        string hgName = string.Empty;
        string lgName = string.Empty;
        string cName = string.Empty;
        string skin = string.Empty;
        if (!controllerStatuses.ContainsKey(idKey))
        {
            controllerStatuses.Add(idKey, new ControllerStatus());
        }

        var status = controllerStatuses[idKey];

        specificRealTimeData = SearchRealTimeData(iid, cid);
        specificControllerData = FetchControllerData(iid, cid);
        
        Image controllerIcon = controllerInstance.transform.Find("obj_UpperOfController/UpperLeft_Img/ControllerIcon").GetComponent<Image>();
        TextMeshProUGUI lastUpdateTime = controllerInstance.transform.Find("obj_UpperOfController/UpperCenter_InfoValue/LastUpdateTime/LastUpdateTimeVal").GetComponent<TextMeshProUGUI>();
        Transform obj_lossConText = controllerInstance.transform.Find("LossOfConn");
        controllerTrendContentGrid = controllerInstance.transform.Find("obj_FLD_StatusValueScrollView/Scroll View/Viewport/Content");
        Button btnUISet = controllerInstance.transform.Find("obj_UISet/obj_UISetParent/btn_UISet").GetComponent<Button>();
        btnUISet.onClick.RemoveAllListeners();
        btnUISet.onClick.AddListener(() =>
        {
            ControllerStyleManager.instance.OpenControllerSetUI(iid, cid);
        });

        // 컨트롤러명 & 그룹명 할당
        if (specificControllerData != null && specificControllerData.Tables.Count > 0 && specificControllerData.Tables[0].Rows.Count > 0)
        {
            hgid = specificControllerData.Tables[0].Rows[0]["HGID"].ToString();
            lgid = specificControllerData.Tables[0].Rows[0]["LGID"].ToString();
            hgName = string.Empty;
            lgName = string.Empty;
            cName = specificControllerData.Tables[0].Rows[0]["CNAME"].ToString();
            pkey = specificControllerData.Tables[0].Rows[0]["PKEY"].ToString();
            showAddr = specificControllerData.Tables[0].Rows[0]["SHOW_ADDR"].ToString();
            skin = specificControllerData.Tables[0].Rows[0]["SKIN"].ToString();

            arrShowAddr = showAddr.Split(',');
            

            if (pkey == "UC0224150200401102" || pkey == "02240601-001-00-208" || pkey == "UC0815120104610507" || pkey == "UC0713020103611349" || pkey == "UC0224150200501110")
                controllerIcon.sprite = controllerIconImgList[0]; // 유니트쿨러
            else if (pkey == "sensor")
                controllerIcon.sprite = controllerIconImgList[1]; // 센서
            else if (pkey == "PRDPC3HL20160317")
                controllerIcon.sprite = controllerIconImgList[2]; // 압력센서
            else if (pkey == "STHCR5NF0513210100500205" || pkey == "07152101-011-00-170")
                controllerIcon.sprite = controllerIconImgList[3]; // 온습도센서
            else
                controllerIcon.sprite = controllerIconImgList[0]; // 기본

            TextMeshProUGUI controllerName = controllerInstance.transform.Find("obj_UpperOfController/UpperCenter_InfoValue/ControllerName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI groupName = controllerInstance.transform.Find("obj_UpperOfController/UpperCenter_InfoValue/GroupName").GetComponent<TextMeshProUGUI>();
            controllerName.text = cName;

            foreach (DataRow hgroupRow in highGroupData.Tables[0].Rows)
            {
                if (hgroupRow["FLD_HGID"].ToString() == hgid)
                {
                    hgName = hgroupRow["FLD_NAME"].ToString();

                    foreach (DataRow lgroupRow in lowGroupData.Tables[0].Rows)
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
                groupName.text = $"{hgName} {lgName}";
            }
            else
            {
                groupName.text = string.Empty;
            }
        }

        ChangeDesignControllerGridInstance(controllerInstance, status, iid, cid, pkey);

        // 업데이트 시간에 따른 통신 경보 처리, 트렌드 표시, 운전&제상&경보 표시
        if (specificRealTimeData != null && specificRealTimeData.Tables.Count > 0 && specificRealTimeData.Tables[0].Rows.Count > 0)
        {
            string pkeyType = specificRealTimeData.Tables[0].Rows[0]["PKEY_TYPE"].ToString();
            string xml = string.Empty;
            string systemSize = string.Empty;
            string systemAddr = string.Empty;
            string outputSize = string.Empty;
            string outputAddr = string.Empty;
            int iSystemRawValue = 0;
            int iOutputRawValue = 0;


            if (pkey == "UC0815120104610507") // 표준 유니트쿨러 5.04~5.07
            {
                foreach (DataRow pt in protocolListData.Tables[0].Rows)
                {
                    // KEY 필드의 값이 pkey와 일치하는지 확인
                    if (pt["KEY"].ToString() == pkey)
                    {
                        // XML 필드의 값을 xml 변수에 할당
                        xml = pt["XML"].ToString();
                        break; // 조건에 맞는 행을 찾으면 루프 종료
                    }
                }

                var allAttributes = XMLParser.Instance.GetAllSystemAttributes(xml, iid, cid);

                foreach (var group in (Dictionary<string, Dictionary<string, object>>)allAttributes["groups"])
                {
                    var attributes = (Dictionary<string, string>)group.Value["attributes"];

                    var tags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    foreach (var tag in tags)
                    {
                        var tagAttributes = tag.Value;
                        systemAddr = tagAttributes["addr"];
                        systemSize = tagAttributes["size"];
                        ParsePollingData(iid, cid);
                        iSystemRawValue = int.Parse(systemAddr) < 200 ? parsedPollingData[int.Parse(systemAddr)] : parsedPollingData[int.Parse(systemAddr) - 200];
                    }
                }

                string binaryValue = Convert.ToString(iSystemRawValue, 2).PadLeft(8, '0');
                char[] reversedBinary = binaryValue.Reverse().ToArray(); // 문자열을 역순으로 만듦


                if (systemSize == "u1" || systemSize == "u2")
                {
                    //Debug.Log($"{idKey}, {systemAddr}, {systemSize}, {irawValue}, {binaryValue}");
                    status.IsConnTrying = false;
                    status.IsConn = specificRealTimeData.Tables[0].Rows[0]["CONN"].ToString() == "1" ? true : false;
                    status.IsAlarm = int.Parse(specificRealTimeData.Tables[0].Rows[0]["ALARM"].ToString()) >= int.Parse("1") ? true : false;
                    status.IsPower = specificRealTimeData.Tables[0].Rows[0]["POWER"].ToString() == "1" ? true : false;
                    status.IsRun = reversedBinary[0] == '1';
                    status.IsDefrost = reversedBinary[1] == '1';
                    status.IsCool = reversedBinary[2] == '1';
                    status.IsFan = reversedBinary[4] == '1';
                    status.IsHeat = reversedBinary[6] == '1';
                    status.IsHumi = false;
                    status.IsDehumi = false;
                }
            }
            else if (pkey == "UC0713020103611349") //표준 유니트쿨러 분리형 1스텝 3.49
            {
                foreach (DataRow pt in protocolListData.Tables[0].Rows)
                {
                    // KEY 필드의 값이 pkey와 일치하는지 확인
                    if (pt["KEY"].ToString() == pkey)
                    {
                        // XML 필드의 값을 xml 변수에 할당
                        xml = pt["XML"].ToString();
                        break; // 조건에 맞는 행을 찾으면 루프 종료
                    }
                }

                var systemAttributes = XMLParser.Instance.GetAllSystemAttributes(xml, iid, cid);
                foreach (var group in (Dictionary<string, Dictionary<string, object>>)systemAttributes["groups"])
                {
                    var attributes = (Dictionary<string, string>)group.Value["attributes"];

                    var tags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    foreach (var tag in tags)
                    {
                        var tagAttributes = tag.Value;
                        systemAddr = tagAttributes["addr"];
                        systemSize = tagAttributes["size"];
                        ParsePollingData(iid, cid);
                        iSystemRawValue = int.Parse(systemAddr) < 200 ? parsedPollingData[int.Parse(systemAddr)] : parsedPollingData[int.Parse(systemAddr) - 200];
                    }
                }

                var outputAttributes = XMLParser.Instance.GetAllOutputAttributes(xml, iid, cid);
                foreach (var group in (Dictionary<string, Dictionary<string, object>>)outputAttributes["groups"])
                {
                    var attributes = (Dictionary<string, string>)group.Value["attributes"];

                    var tags = (Dictionary<string, Dictionary<string, string>>)group.Value["tags"];
                    foreach (var tag in tags)
                    {
                        var tagAttributes = tag.Value;
                        outputAddr = tagAttributes["addr"];
                        outputSize = tagAttributes["size"];
                        ParsePollingData(iid, cid);
                        iOutputRawValue = int.Parse(outputAddr) < 200 ? parsedPollingData[int.Parse(outputAddr)] : parsedPollingData[int.Parse(outputAddr) - 200];
                    }
                }

                string binaryValueSystem = Convert.ToString(iSystemRawValue, 2).PadLeft(8, '0');
                char[] reversedBinarySystem = binaryValueSystem.Reverse().ToArray(); // 문자열을 역순으로 만듦
                string binaryValueOutput = Convert.ToString(iOutputRawValue, 2).PadLeft(8, '0');
                char[] reversedBinaryOutput = binaryValueOutput.Reverse().ToArray(); // 문자열을 역순으로 만듦


                if (systemSize == "u1" || systemSize == "u2")
                {
                    //Debug.Log($"{idKey}, {systemAddr}, {systemSize}, {irawValue}, {binaryValue}");
                    status.IsConnTrying = false;
                    status.IsConn = specificRealTimeData.Tables[0].Rows[0]["CONN"].ToString() == "1" ? true : false;
                    status.IsAlarm = int.Parse(specificRealTimeData.Tables[0].Rows[0]["ALARM"].ToString()) >= int.Parse("1") ? true : false;
                    status.IsPower = specificRealTimeData.Tables[0].Rows[0]["POWER"].ToString() == "1" ? true : false;
                    status.IsRun = reversedBinarySystem[0] == '1';
                    status.IsDefrost = reversedBinarySystem[1] == '1';
                    status.IsCool = reversedBinaryOutput[1] == '1';
                    status.IsFan = reversedBinaryOutput[0] == '1';
                    status.IsHeat = reversedBinaryOutput[3] == '1' || reversedBinaryOutput[4] == '1';
                    status.IsHumi = false;
                    status.IsDehumi = false;
                }
            }
            else
            {
                status.IsConnTrying = false;
                status.IsConn = specificRealTimeData.Tables[0].Rows[0]["CONN"].ToString() == "1" ? true : false;
                status.IsRun = specificRealTimeData.Tables[0].Rows[0]["POWER"].ToString() == "1" ? true : false;                
                status.IsAlarm = int.Parse(specificRealTimeData.Tables[0].Rows[0]["ALARM"].ToString()) >= int.Parse("1") ? true : false;
                status.IsDefrost = specificRealTimeData.Tables[0].Rows[0]["DEFROST"].ToString() == "1" ? true : false;
                status.IsCool = specificRealTimeData.Tables[0].Rows[0]["COOL"].ToString() == "1" ? true : false;
                status.IsFan = specificRealTimeData.Tables[0].Rows[0]["PKEY_TYPE"].ToString() == "FAN:1" ? true : false;
                status.IsHeat = specificRealTimeData.Tables[0].Rows[0]["HEAT"].ToString() == "1" ? true : false;
                status.IsHumi = specificRealTimeData.Tables[0].Rows[0]["HUMI"].ToString() == "1" ? true : false;
                status.IsDehumi = specificRealTimeData.Tables[0].Rows[0]["DEHUMI"].ToString() == "1" ? true : false;
            }


            lastUpdateTime.text = specificRealTimeData.Tables[0].Rows[0]["UPDATE_TIME"].ToString();
            string trendCount = specificRealTimeData.Tables[0].Rows[0]["TREND_CNT"].ToString();
            string strdbTime = specificRealTimeData.Tables[0].Rows[0]["UPDATE_TIME"].ToString();
            string packet = specificRealTimeData.Tables[0].Rows[0]["PACKET"].ToString();
            string strData = specificRealTimeData.Tables[0].Rows[0]["STR_DATA"].ToString();
            DateTime dbTime;

            if (DateTime.TryParse(strdbTime, out dbTime))
            {
                TimeSpan timeDifference = DateTime.Now - dbTime;

                // 패킷 데이터 없음 : 마지막 업데이트 시간과 현재 시간이 1분 이상 차이가 나면서 PACKET, STR_DATA 필드에 데이터가 없음
                if (timeDifference.TotalMinutes >= 1 && packet.Length == 0 && strData.Length == 0)
                {
                    status.IsConnChecking = true;
                    controllerTrendContentGrid.gameObject.SetActive(false);
                    obj_lossConText.gameObject.SetActive(true);
                    TextMeshProUGUI lossConText = obj_lossConText.GetComponent<TextMeshProUGUI>();                    
                    lossConText.text = "통신 끊김\n연결 재시도 중...";
                }
                else
                {
                    // 마지막 업데이트 시간과 현재 시간이 1분 내로 유지될 때
                    status.IsConnChecking = false;
                    obj_lossConText.gameObject.SetActive(false);
                    controllerTrendContentGrid.gameObject.SetActive(true);

                    if (Convert.ToInt32(trendCount) > 0)
                    {
                        for (int i = 0; i < Convert.ToInt32(trendCount); i++)
                        {
                            string strTrend = specificRealTimeData.Tables[0].Rows[0][$"TREND{i}"].ToString();
                            string[] strTrendParts = strTrend.Split('|');


                            if (strTrendParts.Length >= 3)
                            {
                                string title = strTrendParts[0];
                                string value = strTrendParts[1];
                                string unit = strTrendParts[2];
                                string addr = strTrendParts[3];
                                string multiply = strTrendParts[4];

                                //if (strTrendParts[5] != null && strTrendParts[5] == "y" && pkey == "PRDPC3HL20160317")
                                //    continue;

                                string trendObjectName = $"TrendGrid_{iid}_{cid}_{addr}";

                                GameObject trendObject;
                                Transform existingTrend = controllerTrendContentGrid.Find(trendObjectName);

                                if (existingTrend)
                                {
                                    // 이미 존재하는 트렌드 오브젝트의 데이터만 업데이트
                                    trendObject = existingTrend.gameObject;

                                    // 선택된 트렌드 항목만 표시
                                    string objIid = trendObject.name.Split('_')[1];
                                    string objCid = trendObject.name.Split('_')[2];
                                    string objAddr = trendObject.name.Split('_')[3];
                                    if (trendObject.name.Contains(objIid) && trendObject.name.Contains(objCid) && showAddr.Contains(objAddr))
                                        trendObject.SetActive(true);
                                    else
                                        trendObject.SetActive(false);
                                }
                                else
                                {
                                    // 새로운 트렌드 오브젝트 생성
                                    trendObject = ObjectPool.Instance.GetTrendGridObject();
                                    trendObject.name = trendObjectName;
                                    trendGridInstances[trendObjectName] = trendObject;
                                    trendObject.transform.SetParent(controllerTrendContentGrid, false);
                                }                                

                                TextMeshProUGUI titleText = trendObject.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                                TextMeshProUGUI valueText = trendObject.transform.Find("obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                                TextMeshProUGUI unitText = trendObject.transform.Find("obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();


                                float rawValue = (float.Parse(strTrendParts[1]) * float.Parse(strTrendParts[4]));

                                if (rawValue >= 32768)
                                { // 16비트 정수에서 음수 값 처리
                                    rawValue -= 65536;
                                }

                                float newValue = rawValue / float.Parse(multiply);
                                string strNewValue = string.Empty;
                                if (multiply == "1")
                                    strNewValue = newValue.ToString();
                                else if (multiply == "10")
                                    strNewValue = newValue.ToString("F1");
                                else if (multiply == "100")
                                    strNewValue = newValue.ToString("F2");
                                //Debug.Log(valueText.text);
                                //float oldValue = float.Parse(valueText.text);
                                //float oldValue = 0;

                                ChangeWidgetColor(status, pkey, trendObject, titleText, valueText, unitText, valueText.text, strNewValue);

                                titleText.text = title;
                                if (multiply == "1")
                                    valueText.text = newValue.ToString();
                                else if (multiply == "10")
                                    valueText.text = newValue.ToString("F1");
                                else if (multiply == "100")
                                    valueText.text = newValue.ToString("F2");
                                
                                unitText.text = unit;
                            }
                        }
                    }

                    string runObjName = $"TrendGrid_{iid}_{cid}_Run";
                    string defObjName = $"TrendGrid_{iid}_{cid}_Def";
                    string coolObjName = $"TrendGrid_{iid}_{cid}_Cool";
                    string fanObjName = $"TrendGrid_{iid}_{cid}_Fan";
                    string heatObjName = $"TrendGrid_{iid}_{cid}_Heat";
                    GameObject stateObjectRun;
                    GameObject stateObjectDef;
                    GameObject stateObjectCool;
                    GameObject stateObjectFan;
                    GameObject stateObjectHeat;                    
                    Transform existingStateRun = controllerTrendContentGrid.Find(runObjName);
                    Transform existingStateDef = controllerTrendContentGrid.Find(defObjName);
                    Transform existingStateCool = controllerTrendContentGrid.Find(coolObjName);
                    Transform existingStateFan = controllerTrendContentGrid.Find(fanObjName);
                    Transform existingStateHeat = controllerTrendContentGrid.Find(heatObjName);

                    if (existingStateRun && existingStateDef && existingStateCool && existingStateFan && existingStateHeat)
                    {
                        // 이미 존재하는 트렌드 오브젝트의 데이터만 업데이트
                        stateObjectRun = existingStateRun.gameObject;
                        stateObjectCool = existingStateCool.gameObject;
                        stateObjectHeat = existingStateHeat.gameObject;
                        stateObjectDef = existingStateDef.gameObject;
                        stateObjectFan = existingStateFan.gameObject;


                        // 선택된 트렌드 항목만 표시
                        if (stateObjectRun.name.Contains(iid) && stateObjectRun.name.Contains(cid) && skin.Contains("Run"))
                            stateObjectRun.SetActive(true);
                        else
                            stateObjectRun.SetActive(false);

                        if (stateObjectCool.name.Contains(iid) && stateObjectCool.name.Contains(cid) && skin.Contains("Cool"))
                            stateObjectCool.SetActive(true);
                        else
                            stateObjectCool.SetActive(false);

                        if (stateObjectHeat.name.Contains(iid) && stateObjectHeat.name.Contains(cid) && skin.Contains("Heat"))
                            stateObjectHeat.SetActive(true);
                        else
                            stateObjectHeat.SetActive(false);

                        if (stateObjectDef.name.Contains(iid) && stateObjectDef.name.Contains(cid) && skin.Contains("Def"))
                            stateObjectDef.SetActive(true);
                        else
                            stateObjectDef.SetActive(false);

                        if (stateObjectFan.name.Contains(iid) && stateObjectFan.name.Contains(cid) && skin.Contains("Fan"))
                            stateObjectFan.SetActive(true);
                        else
                            stateObjectFan.SetActive(false);
                    }
                    else
                    {
                        // 새로운 트렌드 오브젝트 생성
                        stateObjectRun = ObjectPool.Instance.GetTrendGridObject();
                        stateObjectCool = ObjectPool.Instance.GetTrendGridObject();
                        stateObjectHeat = ObjectPool.Instance.GetTrendGridObject();
                        stateObjectDef = ObjectPool.Instance.GetTrendGridObject();
                        stateObjectFan = ObjectPool.Instance.GetTrendGridObject();

                        stateObjectRun.name = runObjName;
                        stateObjectCool.name = coolObjName;
                        stateObjectHeat.name = heatObjName;
                        stateObjectDef.name = defObjName;
                        stateObjectFan.name = fanObjName;

                        trendGridInstances[runObjName] = stateObjectRun;
                        trendGridInstances[coolObjName] = stateObjectCool;
                        trendGridInstances[heatObjName] = stateObjectHeat;
                        trendGridInstances[defObjName] = stateObjectDef;
                        trendGridInstances[fanObjName] = stateObjectFan;

                        stateObjectRun.transform.SetParent(controllerTrendContentGrid, false);
                        stateObjectCool.transform.SetParent(controllerTrendContentGrid, false);
                        stateObjectHeat.transform.SetParent(controllerTrendContentGrid, false);
                        stateObjectDef.transform.SetParent(controllerTrendContentGrid, false);
                        stateObjectFan.transform.SetParent(controllerTrendContentGrid, false);
                    }

                    TextMeshProUGUI titleTextRun = stateObjectRun.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI valueTextRun = stateObjectRun.transform.Find("obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI unitTextRun = stateObjectRun.transform.Find("obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();
                    titleTextRun.text = "운전 상태";
                    valueTextRun.text = status.IsRun ? "ON" : "OFF";
                    unitTextRun.text = string.Empty;

                    TextMeshProUGUI titleTextCool = stateObjectCool.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI valueTextCool = stateObjectCool.transform.Find("obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI unitTextCool = stateObjectCool.transform.Find("obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();
                    titleTextCool.text = "냉방 상태";
                    valueTextCool.text = status.IsCool ? "ON" : "OFF";
                    unitTextCool.text = string.Empty;

                    TextMeshProUGUI titleTextHeat = stateObjectHeat.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI valueTextHeat = stateObjectHeat.transform.Find("obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI unitTextHeat = stateObjectHeat.transform.Find("obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();
                    titleTextHeat.text = "난방 상태";
                    valueTextHeat.text = status.IsHeat ? "ON" : "OFF";
                    unitTextHeat.text = string.Empty;

                    TextMeshProUGUI titleTextDef = stateObjectDef.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI valueTextDef = stateObjectDef.transform.Find("obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI unitTextDef = stateObjectDef.transform.Find("obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();
                    titleTextDef.text = "제상 상태";
                    valueTextDef.text = status.IsDefrost ? "ON" : "OFF";
                    unitTextDef.text = string.Empty;

                    TextMeshProUGUI titleTextFan = stateObjectFan.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI valueTextFan = stateObjectFan.transform.Find("obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI unitTextFan = stateObjectFan.transform.Find("obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();
                    titleTextFan.text = "팬 상태";
                    valueTextFan.text = status.IsFan ? "ON" : "OFF";
                    unitTextFan.text = string.Empty;
                }

                // 패킷 데이터 받은적 있음 : 마지막 업데이트 시간과 현재 시간이 1분 이상 차이가 나면서 PACKET, STR_DATA 필드에 데이터가 있음, 업데이트는 안되는중
                if (timeDifference.TotalMinutes >= 1 && packet.Length > 0 && strData.Length > 0)
                {
                    status.IsConnTrying = true;
                    controllerTrendContentGrid.gameObject.SetActive(false);
                    obj_lossConText.gameObject.SetActive(true);
                    TextMeshProUGUI lossConText = obj_lossConText.GetComponent<TextMeshProUGUI>();
                    lossConText.text = "통신 끊김\n연결 재시도 중...";
                }
            }
        }
        else
        {
            controllerInstance.GetComponent<Image>().color = stopBGColor;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() => AddMoveToDetailViewListener(status, controllerInstance, iid, cid));

        FinalizeUIUpdate();
    }

    private void ChangeWidgetColor(ControllerStatus status, string pkey, GameObject widgetInstance, TextMeshProUGUI trendName, TextMeshProUGUI trendValue, TextMeshProUGUI trendUnit, string oldValue, string newValue)
    {
        if (newValue != oldValue)
        {
            // 값이 변경되었다면 색상을 연두색으로 변경하고, 0.5초 후에 다시 흰색으로 되돌립니다.
            Image widgetImage = widgetInstance.GetComponent<Image>(); // Image 컴포넌트 참조

            if (widgetImage != null)
            {
                // 트렌드명 흰색-(0.9초)->normalBlackColor, normalBlackColor-(0.9초)->흰색
                trendName.DOColor(Color.white, 0.9f).OnComplete(() => trendName.DOColor(normalBlackColor, 0.9f));
                // 단위 흰색-(0.9초)->normalUnitColor, normalUnitColor-(0.9초)->흰색
                trendUnit.DOColor(Color.white, 0.9f).OnComplete(() => trendUnit.DOColor(normalUnitColor, 0.9f));

                if (status.IsRun)
                {
                    trendValue.DOColor(Color.white, 0.9f).OnComplete(() => trendValue.DOColor(colorBlue, 0.9f));
                    widgetImage.DOColor(colorBlue, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                }
                else
                {
                    foreach (DataRow row in protocolListData.Tables[0].Rows)
                    {
                        if (row["KEY"].ToString() == pkey)
                        {
                            if (row["OPTION1"].ToString().Contains("ALWAYS_ON"))
                            {
                                trendValue.DOColor(Color.white, 0.9f).OnComplete(() => trendValue.DOColor(colorBlue, 0.9f));
                                widgetImage.DOColor(colorBlue, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                            }
                            else
                            {
                                trendValue.DOColor(Color.white, 0.9f).OnComplete(() => trendValue.DOColor(stopColor, 0.9f));
                                widgetImage.DOColor(stopColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                            }
                        }
                    }                    
                }

                if (status.IsDefrost)
                {
                    trendValue.DOColor(Color.white, 0.9f).OnComplete(() => trendValue.DOColor(runColor, 0.9f));
                    widgetImage.DOColor(runColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                }
                if (status.IsAlarm)
                {
                    trendValue.DOColor(Color.white, 0.9f).OnComplete(() => trendValue.DOColor(alarmColor, 0.9f));
                    widgetImage.DOColor(alarmColor, 0.9f).OnComplete(() => widgetImage.DOColor(Color.white, 0.9f));
                }
            }
        }
    }

    // List 컨트롤러 오브젝트 정보 갱신
    private void PopulateListControllers(GameObject controllerInstance, string iid, string cid)
    {
        string idKey = $"{iid}_{cid}";
        string pkey = string.Empty;
        string showAddr = string.Empty;
        string[] arrShowAddr = null;
        string hgid = string.Empty;
        string lgid = string.Empty;
        string hgName = string.Empty;
        string lgName = string.Empty;
        string cName = string.Empty;
        if (!controllerStatuses.ContainsKey(idKey))
        {
            controllerStatuses.Add(idKey, new ControllerStatus());
        }

        var status = controllerStatuses[idKey];

        specificRealTimeData = SearchRealTimeData(iid, cid);
        specificControllerData = FetchControllerData(iid, cid);

        Image controllerIcon = controllerInstance.transform.Find("ListParent/obj_ProductName/Img_Product/ControllerIcon").GetComponent<Image>();
        Transform obj_lossConText = controllerInstance.transform.Find("ListParent/obj_Trend/LossOfConn");

        controllerTrendContentList = controllerInstance.transform.Find("ListParent/obj_Trend/Scroll View/Viewport/Content");

        Button btnUISet = controllerInstance.transform.Find("obj_UISet/obj_UISetParent/btn_UISet").GetComponent<Button>();
        btnUISet.onClick.RemoveAllListeners();
        btnUISet.onClick.AddListener(() =>
        {
            ControllerStyleManager.instance.OpenControllerSetUI(iid, cid);
        });

        // 그룹명 & 컨트롤러명 할당
        if (specificControllerData != null && specificControllerData.Tables.Count > 0 && specificControllerData.Tables[0].Rows.Count > 0)
        {
            hgid = specificControllerData.Tables[0].Rows[0]["HGID"].ToString();
            lgid = specificControllerData.Tables[0].Rows[0]["LGID"].ToString();
            hgName = string.Empty;
            lgName = string.Empty;
            cName = specificControllerData.Tables[0].Rows[0]["CNAME"].ToString();
            pkey = specificControllerData.Tables[0].Rows[0]["PKEY"].ToString();
            showAddr = specificControllerData.Tables[0].Rows[0]["SHOW_ADDR"].ToString();
            arrShowAddr = showAddr.Split(',');

            if (pkey == "UC0224150200401102" || pkey == "02240601-001-00-208" || pkey == "UC0815120104610507" || pkey == "UC0713020103611349" || pkey == "UC0224150200501110")
                controllerIcon.sprite = controllerIconImgList[0]; // 유니트쿨러
            else if (pkey == "sensor")
                controllerIcon.sprite = controllerIconImgList[1]; // 센서
            else if (pkey == "PRDPC3HL20160317")
                controllerIcon.sprite = controllerIconImgList[2]; // 압력센서
            else if (pkey == "STHCR5NF0513210100500205" || pkey == "07152101-011-00-170")
                controllerIcon.sprite = controllerIconImgList[3]; // 온습도센서
            else
                controllerIcon.sprite = controllerIconImgList[0]; // 기본

            TextMeshProUGUI controllerName = controllerInstance.transform.Find("ListParent/obj_ProductName/ControllerName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI highGroupName = controllerInstance.transform.Find("ListParent/obj_Group/txt_HighGroupName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI lowGroupName = controllerInstance.transform.Find("ListParent/obj_Group/txt_LowGroupName").GetComponent<TextMeshProUGUI>();

            controllerName.text = cName;

            foreach (DataRow hgroupRow in highGroupData.Tables[0].Rows)
            {
                if (hgroupRow["FLD_HGID"].ToString() == hgid)
                {
                    hgName = hgroupRow["FLD_NAME"].ToString();

                    foreach (DataRow lgroupRow in lowGroupData.Tables[0].Rows)
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
                highGroupName.text = hgName;
                lowGroupName.text = lgName;
            }
            else
            {
                highGroupName.text = string.Empty;
                lowGroupName.text = string.Empty;
            }
        }

        ChangeDesignControllerListInstance(controllerInstance, status, pkey, iid, cid);

        // 업데이트 시간에 따른 통신 경보 처리, 트렌드 표시, 운전&제상&경보 표시
        if (specificRealTimeData != null && specificRealTimeData.Tables.Count > 0 && specificRealTimeData.Tables[0].Rows.Count > 0)
        {
            status.IsConnTrying = false;
            status.IsConn = specificRealTimeData.Tables[0].Rows[0]["CONN"].ToString() == "1" ? true : false;
            status.IsRun = specificRealTimeData.Tables[0].Rows[0]["POWER"].ToString() == "1" ? true : false;
            status.IsAlarm = int.Parse(specificRealTimeData.Tables[0].Rows[0]["ALARM"].ToString()) >= int.Parse("1") ? true : false;
            status.IsDefrost = specificRealTimeData.Tables[0].Rows[0]["DEFROST"].ToString() == "1" ? true : false;
            status.IsCool = specificRealTimeData.Tables[0].Rows[0]["COOL"].ToString() == "1" ? true : false;
            status.IsFan = specificRealTimeData.Tables[0].Rows[0]["PKEY_TYPE"].ToString() == "FAN:1" ? true : false;
            status.IsHeat = specificRealTimeData.Tables[0].Rows[0]["HEAT"].ToString() == "1" ? true : false;
            status.IsHumi = specificRealTimeData.Tables[0].Rows[0]["HUMI"].ToString() == "1" ? true : false;
            status.IsDehumi = specificRealTimeData.Tables[0].Rows[0]["DEHUMI"].ToString() == "1" ? true : false;

            string trendCount = specificRealTimeData.Tables[0].Rows[0]["TREND_CNT"].ToString();
            string strdbTime = specificRealTimeData.Tables[0].Rows[0]["UPDATE_TIME"].ToString();
            string packet = specificRealTimeData.Tables[0].Rows[0]["PACKET"].ToString();
            string strData = specificRealTimeData.Tables[0].Rows[0]["STR_DATA"].ToString();
            DateTime dbTime;

            // 업데이트 시간에 따른 통신 경보 처리, 트렌드 표시
            if (DateTime.TryParse(strdbTime, out dbTime))
            {
                TimeSpan timeDifference = DateTime.Now - dbTime;

                // 패킷 데이터 없음 : 마지막 업데이트 시간과 현재 시간이 1분 이상 차이가 나면서 PACKET, STR_DATA 필드에 데이터가 없음
                if (timeDifference.TotalMinutes >= 1 && packet.Length == 0 && strData.Length == 0)
                {
                    status.IsConnChecking = true;
                    controllerTrendContentList.gameObject.SetActive(false);
                    obj_lossConText.gameObject.SetActive(true);
                    TextMeshProUGUI lossConText = obj_lossConText.GetComponent<TextMeshProUGUI>();
                    //lossConText.text = "통신 확인 필요";
                    lossConText.text = "통신 끊김, 연결 재시도 중...";
                }
                else
                {
                    // 마지막 업데이트 시간과 현재 시간이 1분 내로 유지될 때
                    status.IsConnChecking = false;
                    obj_lossConText.gameObject.SetActive(false);
                    controllerTrendContentList.gameObject.SetActive(true);

                    if (Convert.ToInt32(trendCount) > 0)
                    {
                        for (int i = 0; i < Convert.ToInt32(trendCount); i++)
                        {
                            string strTrend = specificRealTimeData.Tables[0].Rows[0][$"TREND{i}"].ToString();
                            string[] strTrendParts = strTrend.Split('|');

                            if (strTrendParts.Length >= 3)
                            {
                                string title = strTrendParts[0];
                                string value = strTrendParts[1];
                                string unit = strTrendParts[2];
                                string addr = strTrendParts[3];
                                string multiply = strTrendParts[4];

                                if (title.Contains("설정"))
                                    continue;
                                string trendObjectName = $"TrendList_{iid}_{cid}_{addr}";
                                GameObject trendObject;
                                Transform existingTrend = controllerTrendContentList.Find(trendObjectName);

                                if (existingTrend)
                                {
                                    // 이미 존재하는 트렌드 오브젝트의 데이터만 업데이트
                                    trendObject = existingTrend.gameObject;

                                    // 선택된 트렌드 항목만 표시
                                    string objIid = trendObject.name.Split('_')[1];
                                    string objCid = trendObject.name.Split('_')[2];
                                    string objAddr = trendObject.name.Split('_')[3];
                                    if (trendObject.name.Contains(objIid) && trendObject.name.Contains(objCid) && showAddr.Contains(objAddr))
                                        trendObject.SetActive(true);
                                    else
                                        trendObject.SetActive(false);
                                }
                                else
                                {
                                    // 새로운 트렌드 오브젝트 생성
                                    trendObject = ObjectPool.Instance.GetTrendListObject();
                                    trendObject.name = trendObjectName;
                                    trendListInstances[trendObjectName] = trendObject;
                                    trendObject.transform.SetParent(controllerTrendContentList, false);
                                }

                                TextMeshProUGUI titleText = trendObject.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                                TextMeshProUGUI valueText = trendObject.transform.Find("obj_Value/txt_ValueNUnit").GetComponent<TextMeshProUGUI>();


                                float rawValue = (float.Parse(strTrendParts[1]) * float.Parse(strTrendParts[4]));

                                if (rawValue >= 32768)
                                { // 16비트 정수에서 음수 값 처리
                                    rawValue -= 65536;
                                }

                                float newValue = rawValue / float.Parse(multiply);

                                if (titleText != null && valueText != null)
                                {
                                    titleText.text = title;
                                    if (multiply == "1")
                                        valueText.text = $"{newValue.ToString()}{unit}";
                                    else if (multiply == "10")
                                        valueText.text = $"{newValue.ToString("F1")}{unit}";
                                    else if (multiply == "100")
                                        valueText.text = $"{newValue.ToString("F2")}{unit}";
                                }
                            }
                        }
                    }
                }

                // 패킷 데이터 받은적 있음 : 마지막 업데이트 시간과 현재 시간이 1분 이상 차이가 나면서 PACKET, STR_DATA 필드에 데이터가 있음, 업데이트는 안되는중
                if (timeDifference.TotalMinutes >= 1 && packet.Length > 0 && strData.Length > 0)
                {
                    status.IsConnTrying = true;
                    controllerTrendContentList.gameObject.SetActive(false);
                    obj_lossConText.gameObject.SetActive(true);
                    TextMeshProUGUI lossConText = obj_lossConText.GetComponent<TextMeshProUGUI>();
                    lossConText.text = "통신 끊김, 연결 재시도 중...";
                }
            }
        }
        else
        {
            controllerInstance.GetComponent<Image>().color = stopBGColor;
            controllerInstance.transform.Find("ListParent/btn_DetailView").GetComponent<Image>().color = stopBGColor;
        }

        Button btn_moveDV = controllerInstance.transform.Find("ListParent/btn_DetailView").gameObject.GetComponent<Button>();
        UnityMainThreadDispatcher.Instance().Enqueue(() => AddMoveToDetailViewListener(status, btn_moveDV.gameObject, iid, cid));

        FinalizeUIUpdate();
    }


    // Grid 컨트롤러 인스턴스 아웃라인, 상태 아이콘 업데이트
    private void ChangeDesignControllerGridInstance(GameObject controllerInstance, ControllerStatus status, string iid, string cid, string pkey)
    {
        Outline controllerOutline = controllerInstance.GetComponent<Outline>();
        GameObject parentCool = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Cool").gameObject;
        GameObject parentFan = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Fan").gameObject;
        GameObject parentRun = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Run").gameObject;
        GameObject parentAlarm = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Alarm").gameObject;
        GameObject parentHeat = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Heat").gameObject;
        GameObject parentDefrost = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Defrost").gameObject;

        GameObject statusCool_On = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Cool/Img_On").gameObject;
        GameObject statusCool_Off = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Cool/Img_Off").gameObject;
        GameObject statusFan_On = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Fan/Img_On").gameObject;
        GameObject statusFan_Off = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Fan/Img_Off").gameObject;
        GameObject statusRun_On = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Run/Img_On").gameObject;
        GameObject statusRun_Off = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Run/Img_Off").gameObject;
        GameObject statusAlarm_On = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Alarm/Img_On").gameObject;
        GameObject statusAlarm_Off = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Alarm/Img_Off").gameObject;
        GameObject statusHeat_On = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Heat/Img_On").gameObject;
        GameObject statusHeat_Off = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Heat/Img_Off").gameObject;
        GameObject statusDefrost_On = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Defrost/Img_On").gameObject;
        GameObject statusDefrost_Off = controllerInstance.transform.Find("obj_UpperOfController/obj_ControllerStatusIcon/Status_Defrost/Img_Off").gameObject;
        Image imgControllerIconBG = controllerInstance.transform.Find("obj_UpperOfController/UpperLeft_Img").GetComponent<Image>();

        TextMeshProUGUI txtControllerName = controllerInstance.transform.Find("obj_UpperOfController/UpperCenter_InfoValue/ControllerName").GetComponent<TextMeshProUGUI>();

        // 송풍 상태에 따른 아이콘 변경
        if (status.IsFan)
        {
            statusFan_On.SetActive(true);
            statusFan_Off.SetActive(false);
        }
        else
        {
            statusFan_On.SetActive(false);
            statusFan_Off.SetActive(true);
        }

        // 냉방 상태에 따른 아이콘 변경
        if (status.IsCool)
        {
            statusCool_On.SetActive(true);
            statusCool_Off.SetActive(false);
            txtControllerName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
        }
        else
        {
            statusCool_On.SetActive(false);
            statusCool_Off.SetActive(true);
            txtControllerName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
        }

        // 제상 상태에 따른 아이콘 변경
        if (status.IsDefrost)
        {
            statusDefrost_On.SetActive(true);
            statusDefrost_Off.SetActive(false);
            imgControllerIconBG.color = runColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            ChangeTrendTextColor(controllerInstance, normalTrendColor, iid, cid);
        }
        else
        {
            statusDefrost_On.SetActive(false);
            statusDefrost_Off.SetActive(true);
            controllerInstance.GetComponent<Image>().color = Color.white;
        }

        // 히터 상태에 따른 아이콘 변경
        if (status.IsHeat)
        {
            statusHeat_On.SetActive(true);
            statusHeat_Off.SetActive(false);
            controllerInstance.GetComponent<Image>().color = Color.white;
        }
        else
        {
            statusHeat_On.SetActive(false);
            statusHeat_Off.SetActive(true);
            controllerInstance.GetComponent<Image>().color = Color.white;
        }

        // 운전 상태에 따른 아이콘 변경
        if (status.IsRun)
        {
            statusRun_On.SetActive(true);
            statusRun_Off.SetActive(false);
            imgControllerIconBG.color = colorBlue;
            txtControllerName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            controllerOutline.effectColor = stopColor;
            ChangeTrendTextColor(controllerInstance, colorBlue, iid, cid);
        }
        else
        {
            statusRun_On.SetActive(false);
            statusRun_Off.SetActive(true);
            controllerOutline.effectColor = stopColor;
            imgControllerIconBG.color = stopColor;
            txtControllerName.color = stopColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            ChangeTrendTextColor(controllerInstance, stopColor, iid, cid);
        }

        // 제상 상태에 따른 아이콘 변경
        if (status.IsDefrost)
        {
            statusDefrost_On.SetActive(true);
            statusDefrost_Off.SetActive(false);
            imgControllerIconBG.color = runColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            ChangeTrendTextColor(controllerInstance, normalTrendColor, iid, cid);
        }
        else
        {
            statusDefrost_On.SetActive(false);
            statusDefrost_Off.SetActive(true);
            controllerInstance.GetComponent<Image>().color = Color.white;
        }

        // 경보 상태에 따른 아이콘 변경        
        if (status.IsAlarm)
        {
            statusAlarm_On.SetActive(true);
            statusAlarm_Off.SetActive(false);
            imgControllerIconBG.color = alarmColor;
            controllerOutline.effectColor = alarmColor;
            txtControllerName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            ChangeTrendTextColor(controllerInstance, alarmColor, iid, cid);
        }
        else
        {
            if (status.IsConnTrying || status.IsConnChecking)
            {
                statusAlarm_On.SetActive(false);
                statusAlarm_Off.SetActive(true);
                imgControllerIconBG.color = stopColor;
                txtControllerName.color = stopColor;
                controllerInstance.GetComponent<Image>().color = stopBGColor;
                ChangeTrendTextColor(controllerInstance, stopBGColor, iid, cid);
            }
            else
            {
                statusAlarm_On.SetActive(false);
                statusAlarm_Off.SetActive(true);                        
                txtControllerName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                //ChangeTrendTextColor(controllerInstance, colorBlue, iid, cid);
            }
        }


        // TIC-4M 1.70 예외처리
        if (pkey == "07152101-011-00-170") 
        {
            parentCool.SetActive(false);
            parentHeat.SetActive(false);            

            string[] arrCtrlName = controllerInstance.name.Split('_');
            string arrIID = arrCtrlName[1];
            string arrCID = arrCtrlName[2];

            if (arrIID == iid && arrCID == cid)
            {
                ParsePollingData(iid, cid);
                int rawValue = parsedPollingData[24]; // 시스템상태
                bool isRunBitSet = (rawValue & (1 << 0)) != 0;
                bool isDefBitSet = (rawValue & (1 << 1)) != 0;

                if (isRunBitSet)
                {
                    statusRun_On.SetActive(true);
                    statusRun_Off.SetActive(false);
                    imgControllerIconBG.color = runColor;
                    txtControllerName.color = normalBlackColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    ChangeTrendTextColor(controllerInstance, normalTrendColor, iid, cid);

                    if (isDefBitSet)
                    {
                        statusDefrost_On.SetActive(true);
                        statusDefrost_Off.SetActive(false);
                        controllerInstance.GetComponent<Image>().color = Color.white;
                        ChangeTrendTextColor(controllerInstance, defrostColor, iid, cid);
                    }
                }
                else
                {
                    statusRun_On.SetActive(false);
                    statusRun_Off.SetActive(true);
                    imgControllerIconBG.color = stopColor;
                    txtControllerName.color = stopColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    ChangeTrendTextColor(controllerInstance, stopColor, iid, cid);
                }

                if (status.IsAlarm)
                {
                    statusAlarm_On.SetActive(true);
                    statusAlarm_Off.SetActive(false);
                    imgControllerIconBG.color = alarmColor;
                    controllerOutline.effectColor = alarmColor;
                    txtControllerName.color = normalBlackColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    ChangeTrendTextColor(controllerInstance, alarmColor, iid, cid);
                }
                else
                {
                    if (status.IsConnTrying || status.IsConnChecking)
                    {
                        statusAlarm_On.SetActive(false);
                        statusAlarm_Off.SetActive(true);
                        imgControllerIconBG.color = stopColor;
                        txtControllerName.color = stopColor;
                        controllerInstance.GetComponent<Image>().color = stopBGColor;
                        ChangeTrendTextColor(controllerInstance, stopBGColor, iid, cid);
                    }
                    else
                    {
                        statusAlarm_On.SetActive(false);
                        statusAlarm_Off.SetActive(true);
                        txtControllerName.color = normalBlackColor;
                        controllerInstance.GetComponent<Image>().color = Color.white;
                    }
                }
            }
        }

        // SPG-FC, 온습도 센서 STH-CR5N_F 2.05 예외처리
        if (pkey == "PRDPC3HL20160317" || pkey == "STHCR5NF0513210100500205")
        {
            parentCool.SetActive(false);
            parentRun.SetActive(false);
            parentFan.SetActive(false);
            parentHeat.SetActive(false);
            parentDefrost.SetActive(false);

            statusRun_On.SetActive(false);
            statusRun_Off.SetActive(false);
            statusCool_On.SetActive(false);
            statusCool_Off.SetActive(false);
            statusFan_On.SetActive(false);
            statusFan_Off.SetActive(false);
            statusAlarm_On.SetActive(false);
            statusAlarm_Off.SetActive(false);
            statusHeat_On.SetActive(false);
            statusHeat_Off.SetActive(false);
            statusDefrost_On.SetActive(false);
            statusDefrost_Off.SetActive(false);
            imgControllerIconBG.color = colorBlue;
            txtControllerName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            ChangeTrendTextColor(controllerInstance, colorBlue, iid, cid);

            if (status.IsAlarm)
            {
                statusAlarm_On.SetActive(true);
                statusAlarm_Off.SetActive(false);
                imgControllerIconBG.color = alarmColor;
                controllerOutline.effectColor = alarmColor;
                txtControllerName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                ChangeTrendTextColor(controllerInstance, alarmColor, iid, cid);
            }
            else
            {
                if (status.IsConnTrying || status.IsConnChecking)
                {
                    statusAlarm_On.SetActive(false);
                    statusAlarm_Off.SetActive(true);
                    imgControllerIconBG.color = stopColor;
                    txtControllerName.color = stopColor;
                    controllerInstance.GetComponent<Image>().color = stopBGColor;
                    ChangeTrendTextColor(controllerInstance, stopBGColor, iid, cid);
                }
                else
                {
                    statusAlarm_On.SetActive(false);
                    statusAlarm_Off.SetActive(true);
                    txtControllerName.color = normalBlackColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    //ChangeTrendTextColor(controllerInstance, colorBlue, iid, cid);
                }
            }
        }
    }

    private void ChangeTrendTextColor(GameObject controllerInstance, Color color, string iid, string cid)
    {
        if (controllerInstance == null)
        {
            Debug.LogError("controllerInstance is null");
            return;
        }

        Transform contentTransform = controllerInstance.transform.Find("obj_FLD_StatusValueScrollView/Scroll View/Viewport/Content");
        if (contentTransform == null)
        {
            //Debug.LogError($"Content transform not found : {controllerInstance.name}, {iid}, {cid}");
            return;
        }

        GameObject TrendContent = contentTransform.gameObject;
        if (TrendContent == null)
        {
            Debug.LogError("TrendContent is null");
            return;
        }

        foreach (Transform child in TrendContent.transform)
        {
            Transform valueTransform = child.Find("obj_Value/txt_Value");
            if (valueTransform == null)
            {
                Debug.Log("Value transform not found for child: " + child.name);
                continue;
            }

            TextMeshProUGUI textComponent = valueTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogError("TextMeshProUGUI component not found on obj_Value/txt_Value");
                continue;
            }

            textComponent.color = color;
        }
    }


    // List 컨트롤러 인스턴스 아웃라인, 상태 아이콘 업데이트
    private void ChangeDesignControllerListInstance(GameObject controllerInstance, ControllerStatus status, string pkey, string iid, string cid)
    {
        Outline controllerOutline = controllerInstance.GetComponent<Outline>();
        GameObject statusRun_On = controllerInstance.transform.Find("ListParent/obj_Status/Status_Run/Img_On").gameObject;
        GameObject statusRun_Off = controllerInstance.transform.Find("ListParent/obj_Status/Status_Run/Img_Off").gameObject;
        GameObject statusDefrost_On = controllerInstance.transform.Find("ListParent/obj_Status/Status_Defrost/Img_On").gameObject;
        GameObject statusDefrost_Off = controllerInstance.transform.Find("ListParent/obj_Status/Status_Defrost/Img_Off").gameObject;
        GameObject statusCool_On = controllerInstance.transform.Find("ListParent/obj_Status/Status_Cool/Img_On").gameObject;
        GameObject statusCool_Off = controllerInstance.transform.Find("ListParent/obj_Status/Status_Cool/Img_Off").gameObject;
        GameObject statusFan_On = controllerInstance.transform.Find("ListParent/obj_Status/Status_Fan/Img_On").gameObject;
        GameObject statusFan_Off = controllerInstance.transform.Find("ListParent/obj_Status/Status_Fan/Img_Off").gameObject;
        GameObject statusAlarm_On = controllerInstance.transform.Find("ListParent/obj_Status/Status_Alarm/Img_On").gameObject;
        GameObject statusAlarm_Off = controllerInstance.transform.Find("ListParent/obj_Status/Status_Alarm/Img_Off").gameObject;
        Image imgControllerIconBG = controllerInstance.transform.Find("ListParent/obj_ProductName/Img_Product").GetComponent<Image>();
        Image imgBtnDVIconBG = controllerInstance.transform.Find("ListParent/btn_DetailView").GetComponent<Image>();

        TextMeshProUGUI txtControllerName = controllerInstance.transform.Find("ListParent/obj_ProductName/ControllerName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txthgName = controllerInstance.transform.Find("ListParent/obj_Group/txt_HighGroupName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtlgName = controllerInstance.transform.Find("ListParent/obj_Group/txt_LowGroupName").GetComponent<TextMeshProUGUI>();


        // 운전 상태에 따른 아이콘 변경
        if (status.IsRun)
        {
            statusRun_On.SetActive(true);
            statusRun_Off.SetActive(false);
            imgControllerIconBG.color = runColor;
            txtControllerName.color = normalBlackColor;
            txthgName.color = normalBlackColor;
            txtlgName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            imgBtnDVIconBG.color = Color.white;
        }
        else
        {
            statusRun_On.SetActive(false);
            statusRun_Off.SetActive(true);
            imgControllerIconBG.color = stopColor;
            txtControllerName.color = stopColor;
            txthgName.color = stopColor;
            txtlgName.color = stopColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            imgBtnDVIconBG.color = Color.white;
        }

        // 경보 상태에 따른 아이콘 변경
        //if (status.IsConnTrying || status.IsConnChecking || !status.IsConn || status.IsAlarm)
        if (status.IsAlarm)
        {
            statusAlarm_On.SetActive(true);
            statusAlarm_Off.SetActive(false);
            txtControllerName.color = normalBlackColor;
            txthgName.color = normalBlackColor;
            txtlgName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            imgBtnDVIconBG.color = Color.white;
        }
        else
        {
            if (status.IsConnTrying || status.IsConnChecking)
            {
                statusAlarm_On.SetActive(false);
                statusAlarm_Off.SetActive(true);
                txtControllerName.color = stopColor;
                txthgName.color = stopColor;
                txtlgName.color = stopColor;
                controllerInstance.GetComponent<Image>().color = stopBGColor;
                imgBtnDVIconBG.color = stopBGColor;
            }
            else
            {
                statusAlarm_On.SetActive(false);
                statusAlarm_Off.SetActive(true);
                txtControllerName.color = normalBlackColor;
                txthgName.color = normalBlackColor;
                txtlgName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                imgBtnDVIconBG.color = Color.white;
            }
        }

        // 송풍 상태에 따른 아이콘 변경
        if (status.IsFan)
        {
            if (!status.IsDefrost)
            {
                statusFan_On.SetActive(true);
                statusFan_Off.SetActive(false);
                //if (!fanListCoroutines.ContainsKey(controllerInstance) || fanListCoroutines[controllerInstance] == null)
                //{
                //    fanListCoroutines[controllerInstance] = StartCoroutine(IPlayListFanAnim(controllerInstance));
                //}
            }           
        }
        else
        {
            statusFan_On.SetActive(false);
            statusFan_Off.SetActive(true);
            if (fanListCoroutines.ContainsKey(controllerInstance))
            {
                if (fanListCoroutines[controllerInstance] != null)
                    StopCoroutine(fanListCoroutines[controllerInstance]);
                fanListCoroutines[controllerInstance] = null; // 코루틴 참조 제거
            }
        }

        // 제상 상태에 따른 아이콘 변경
        if (status.IsDefrost)
        {
            statusDefrost_On.SetActive(true);
            statusDefrost_Off.SetActive(false);
            txtControllerName.color = normalBlackColor;
            txthgName.color = normalBlackColor;
            txtlgName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            imgBtnDVIconBG.color = Color.white;
            //if (!defrostListCoroutines.ContainsKey(controllerInstance) || defrostListCoroutines[controllerInstance] == null)
            //{
            //    defrostListCoroutines[controllerInstance] = StartCoroutine(IPlayListDefrostAnim(controllerInstance));
            //}
            if (status.IsAlarm)
            {
                statusAlarm_On.SetActive(true);
                statusAlarm_Off.SetActive(false);
                controllerOutline.effectColor = alarmColor;
                imgControllerIconBG.color = alarmColor;
                txtControllerName.color = normalBlackColor;
                txthgName.color = normalBlackColor;
                txtlgName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                imgBtnDVIconBG.color = Color.white;
            }
            else
            {
                if (status.IsConnTrying || status.IsConnChecking)
                {
                    statusAlarm_On.SetActive(false);
                    statusAlarm_Off.SetActive(true);
                    controllerOutline.effectColor = stopColor;
                    imgControllerIconBG.color = stopColor;
                    txtControllerName.color = stopColor;
                    txthgName.color = stopColor;
                    txtlgName.color = stopColor;
                    controllerInstance.GetComponent<Image>().color = stopBGColor;
                    imgBtnDVIconBG.color = stopBGColor;
                }
                else
                {
                    controllerOutline.effectColor = defrostColor;
                    imgControllerIconBG.color = defrostColor;
                    txtControllerName.color = normalBlackColor;
                    txthgName.color = normalBlackColor;
                    txtlgName.color = normalBlackColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    imgBtnDVIconBG.color = Color.white;
                }
            }
        }
        else
        {
            statusDefrost_On.SetActive(false);
            statusDefrost_Off.SetActive(false);
            txtControllerName.color = normalBlackColor;
            txthgName.color = normalBlackColor;
            txtlgName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            imgBtnDVIconBG.color = Color.white;
            if (defrostListCoroutines.ContainsKey(controllerInstance))
            {
                StopCoroutine(defrostListCoroutines[controllerInstance]);
                defrostListCoroutines[controllerInstance] = null; // 코루틴 참조 제거
            }

            // 냉방 상태에 따른 아이콘 변경
            if (status.IsCool)
            {
                statusCool_On.SetActive(true);
                statusCool_Off.SetActive(false);
                txtControllerName.color = normalBlackColor;
                txthgName.color = normalBlackColor;
                txtlgName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                imgBtnDVIconBG.color = Color.white;
            }
            else
            {
                statusCool_On.SetActive(false);
                statusCool_Off.SetActive(true);
                txtControllerName.color = normalBlackColor;
                txthgName.color = normalBlackColor;
                txtlgName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                imgBtnDVIconBG.color = Color.white;
            }

            if (status.IsAlarm)
            {
                controllerOutline.effectColor = alarmColor;
                imgControllerIconBG.color = alarmColor;
                txtControllerName.color = normalBlackColor;
                txthgName.color = normalBlackColor;
                txtlgName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                imgBtnDVIconBG.color = Color.white;
            }
            else
            {
                if (status.IsConnTrying || status.IsConnChecking)
                {
                    controllerOutline.effectColor = stopColor;
                    imgControllerIconBG.color = stopColor;
                    txtControllerName.color = stopColor;
                    txthgName.color = stopColor;
                    txtlgName.color = stopColor;
                    controllerInstance.GetComponent<Image>().color = stopBGColor;
                    imgBtnDVIconBG.color = stopBGColor;
                }
                else
                {
                    controllerOutline.effectColor = noColor;
                    if (status.IsRun)
                    {
                        imgControllerIconBG.color = runColor;
                        txtControllerName.color = normalBlackColor;
                        txthgName.color = normalBlackColor;
                        txtlgName.color = normalBlackColor;
                        controllerInstance.GetComponent<Image>().color = Color.white;
                        imgBtnDVIconBG.color = Color.white;
                    }
                    else
                    {
                        if (status.IsConnTrying || status.IsConnChecking)
                        {
                            controllerOutline.effectColor = stopColor;
                            imgControllerIconBG.color = stopColor;
                            txtControllerName.color = stopColor;
                            txthgName.color = stopColor;
                            txtlgName.color = stopColor;
                            controllerInstance.GetComponent<Image>().color = stopBGColor;
                            imgBtnDVIconBG.color = stopBGColor;
                        }
                        else
                        {
                            imgControllerIconBG.color = stopColor;
                            txtControllerName.color = stopColor;
                            txthgName.color = stopColor;
                            txtlgName.color = stopColor;
                            controllerInstance.GetComponent<Image>().color = Color.white;
                            imgBtnDVIconBG.color = Color.white;
                        }
                    }
                }
            }
        }

        if (pkey == "07152101-011-00-170")
        {
            string[] arrCtrlName = controllerInstance.name.Split('_');
            string arrIID = arrCtrlName[1];
            string arrCID = arrCtrlName[2];

            if (arrIID == iid && arrCID == cid)
            {
                ParsePollingData(iid, cid);
                int rawValue = parsedPollingData[24];
                bool isRunBitSet = (rawValue & (1 << 0)) != 0;
                bool isDefBitSet = (rawValue & (1 << 1)) != 0;

                if (isRunBitSet)
                {
                    statusRun_On.SetActive(true);
                    statusRun_Off.SetActive(false);
                    imgControllerIconBG.color = runColor;
                    txtControllerName.color = normalBlackColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    ChangeTrendTextColor(controllerInstance, normalTrendColor, iid, cid);
                    if (isDefBitSet)
                    {
                        statusDefrost_On.SetActive(true);
                        statusDefrost_Off.SetActive(false);
                        txtControllerName.color = normalBlackColor;
                        txthgName.color = normalBlackColor;
                        txtlgName.color = normalBlackColor;
                        controllerInstance.GetComponent<Image>().color = Color.white;
                        imgBtnDVIconBG.color = Color.white;
                        //if (!defrostListCoroutines.ContainsKey(controllerInstance) || defrostListCoroutines[controllerInstance] == null)
                        //{
                        //    defrostListCoroutines[controllerInstance] = StartCoroutine(IPlayListDefrostAnim(controllerInstance));
                        //}
                    }
                }
                else
                {
                    statusRun_On.SetActive(false);
                    statusRun_Off.SetActive(true);
                    imgControllerIconBG.color = stopColor;
                    txtControllerName.color = stopColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    ChangeTrendTextColor(controllerInstance, stopColor, iid, cid);
                }
            }
        }

        if (pkey == "PRDPC3HL20160317")
        {
            statusRun_On.SetActive(true);
            statusRun_Off.SetActive(false);
            imgControllerIconBG.color = runColor;
            txtControllerName.color = normalBlackColor;
            txthgName.color = normalBlackColor;
            txtlgName.color = normalBlackColor;
            controllerInstance.GetComponent<Image>().color = Color.white;
            imgBtnDVIconBG.color = Color.white;

            if (status.IsAlarm)
            {
                statusAlarm_On.SetActive(true);
                statusAlarm_Off.SetActive(false);
                txtControllerName.color = normalBlackColor;
                txthgName.color = normalBlackColor;
                txtlgName.color = normalBlackColor;
                controllerInstance.GetComponent<Image>().color = Color.white;
                imgBtnDVIconBG.color = Color.white;
            }
            else
            {
                if (status.IsConnTrying || status.IsConnChecking)
                {
                    statusAlarm_On.SetActive(false);
                    statusAlarm_Off.SetActive(true);
                    txtControllerName.color = stopColor;
                    txthgName.color = stopColor;
                    txtlgName.color = stopColor;
                    controllerInstance.GetComponent<Image>().color = stopBGColor;
                    imgBtnDVIconBG.color = stopBGColor;
                }
                else
                {
                    statusAlarm_On.SetActive(false);
                    statusAlarm_Off.SetActive(true);
                    txtControllerName.color = normalBlackColor;
                    txthgName.color = normalBlackColor;
                    txtlgName.color = normalBlackColor;
                    controllerInstance.GetComponent<Image>().color = Color.white;
                    imgBtnDVIconBG.color = Color.white;
                }
            }
        }
    }

    private void AddMoveToDetailViewListener(ControllerStatus status, GameObject controllerInstance, string iid, string cid)
    {
        // 패킷 데이터 없음(IsConnChecking) : 마지막 업데이트 시간과 현재 시간이 1분 이상 차이가 나면서 PACKET, STR_DATA 필드에 데이터가 없음
        // 패킷 데이터 받은적 있음(IsConnTrying) : 마지막 업데이트 시간과 현재 시간이 1분 이상 차이가 나면서 PACKET, STR_DATA 필드에 데이터가 있음, 업데이트는 안되는중
        controllerInstance.GetComponent<Button>().onClick.RemoveAllListeners();

        // 컨트롤러가 실제로 데이터를 받고있어 realtime 테이블에 존재하는지 확인
        //bool bControllerExist = realTimeData.Tables[0].AsEnumerable().Any(row => row.Field<int>("ID").ToString() == iid && row.Field<int>("CID").ToString() == cid);

        bool bControllerExist = realTimeData.Tables[0].AsEnumerable().Any(row =>
        {
            string rowId = Convert.ToString(row["ID"]);
            string rowCid = Convert.ToString(row["CID"]);
            return rowId == iid && rowCid == cid;
        });

        if (status.IsConnChecking || status.IsConnTrying || !bControllerExist)
        {
            controllerInstance.GetComponent<Button>().onClick.AddListener(() =>
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러의 통신 상태를 확인해주세요.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            });
        }
        else
        {
            controllerInstance.GetComponent<Button>().onClick.AddListener(() =>
            {
                DetailView.Instance.OpenDetailView(iid, cid);
            });
        }
    }

    // 우측 상단 모든 컨트롤러의 알람 개수 출력
    private void UpdateErrorCount(DataSet realTimeData)
    {
        DataTable realTimeTable = realTimeData.Tables[0];
        int totalErrCnt = 0;
        foreach (DataRow row in realTimeTable.Rows)
        {
            // 'ALARM' 필드의 값을 정수로 변환하여 합산
            int alarmCount = 0;
            // TryParse를 사용하여 변환 실패 시 0을 반환하도록 처리
            if (int.TryParse(row["ALARM"].ToString(), out alarmCount))
            {
                totalErrCnt += alarmCount;
            }
        }
        // 합산된 알람 개수를 문자열로 변환하여 txtErrCnt 텍스트 필드에 할당
        txtErrCnt.text = $"{totalErrCnt}";
        if (totalErrCnt == 0)
        {
            alarmCount.SetActive(false);
        }
        else
        {
            alarmCount.SetActive(true);
        }
    }

    // 레이아웃 업데이트
    private void FinalizeUIUpdate()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(controllerContentGrid.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(controllerContentList.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // 데이터 로드 완료 시 로딩 이미지 비활성화
    void IsLoadingFinished()
    {
        if (isLoading)
        {
            obj_Loading.SetActive(false);
            obj_Loading.transform.Find("GameObject/txt_Loading").GetComponent<TextMeshProUGUI>().text = string.Empty;
            isLoading = false;
            controllerScrollViewList.gameObject.SetActive(false);
        }
    }

    // 컨트롤러 뷰 변경
    public void ChangeControllerView()
    {
        if (isGridView)
        {
            controllerScrollViewGrid.SetActive(false);
            controllerScrollViewList.SetActive(true);
            Transform controllerListColumn = controllerScrollViewList.transform.Find("ControllerColumn");
            controllerListColumn.gameObject.SetActive(true);
            imgGrid.SetActive(true);
            imgList.SetActive(false);
        }
        else
        {
            Transform controllerListColumn = controllerScrollViewList.transform.Find("ControllerColumn");
            controllerListColumn.gameObject.SetActive(false);
            controllerScrollViewGrid.SetActive(true);
            controllerScrollViewList.SetActive(false);
            imgGrid.SetActive(false);
            imgList.SetActive(true);
        }
        isGridView = !isGridView;
    }

    // 컨트롤러 상태 접근자 메소드 추가
    public static ControllerStatus GetControllerStatus(string iid, string cid)
    {
        string idKey = $"{iid}_{cid}";

        if (controllerStatuses.ContainsKey(idKey))
        {
            return controllerStatuses[idKey];
        }
        else
        {
            return null; // 또는 적절한 기본값 반환
        }
    }

    #region DB 조회
    // 테이블 조회
    public static DataSet OnSelectRequest(string p_query, string table_name)
    {
        DataSet ds = new DataSet();

        using (var connection = new MySqlConnection(strConn))
        {
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(p_query, connection);
                MySqlDataAdapter sd = new MySqlDataAdapter(cmd);
                sd.Fill(ds, table_name);
            }
            catch (Exception ex)
            {
                //Debug.LogError("ClientDatabase : Failed to connect to the database: " + ex.ToString());
            }
        }

        return ds;
    }

    // 단일 값을 반환하는 ExecuteScalar 메서드
    public static int ExecuteScalarQuery(string p_query)
    {
        int result = 0;

        using (var connection = new MySqlConnection(strConn))
        {
            try
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand(p_query, connection);
                object scalarResult = cmd.ExecuteScalar();
                if (scalarResult != null && int.TryParse(scalarResult.ToString(), out int count))
                {
                    result = count;
                }
            }
            catch (Exception ex)
            {
                //Debug.LogError("ClientDatabase : Failed to execute scalar query: " + ex.ToString());
            }
        }

        return result;
    }

    public static bool TableExists(string tableName)
    {
        // 테이블 존재 여부를 확인하는 쿼리
        string query = $"SHOW TABLES LIKE '{tableName}';";

        // MySqlConnection 객체 생성
        using (var connection = new MySqlConnection(strConn))
        {
            try
            {
                // 데이터베이스 연결
                connection.Open();

                // MySqlCommand 객체 생성
                using (var cmd = new MySqlCommand(query, connection))
                {
                    // 쿼리 실행
                    using (var reader = cmd.ExecuteReader())
                    {
                        // 결과가 있으면 테이블이 존재함
                        return reader.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error checking table existence: {ex.Message}");
                return false;
            }
        }
    }

    // 데이터 삽입
    public static bool OnInsertRequest(string p_query)
    {
        try
        {
            using (var connection = new MySqlConnection(strConn))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = p_query;

                cmd.ExecuteNonQuery();

                return true;
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError("ClientDatabase : Failed to connect to the database: " + ex.ToString());
            return false;
        }
    }

    // 데이터 업데이트
    public static bool OnUpdateRequest(string p_query)
    {
        try
        {
            using (var connection = new MySqlConnection(strConn))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = p_query;

                cmd.ExecuteNonQuery();

                return true;
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError("ClientDatabase : Failed to connect to the database: " + ex.ToString());
            return false;
        }
    }

    // 데이터 삭제
    public static bool OnDeleteRequest(string p_query)
    {
        try
        {
            using (var connection = new MySqlConnection(strConn))
            {
                connection.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = p_query;

                cmd.ExecuteNonQuery();

                return true;
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError("ClientDatabase : Failed to connect to the database: " + ex.ToString());
            return false;
        }
    }

    public static int GetMaxCID(string id)
    {
        // 데이터베이스 연결 및 쿼리 실행 예시 코드
        string query = $"SELECT MAX(CID) AS MaxCID FROM TBL_CONTROLLER WHERE ID = '{id}'";
        using (var connection = new MySqlConnection(strConn))
        {
            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return 0; // CID가 없는 경우, 0 반환
                }
            }
        }
    }

    public static int GetMaxGroupOrder(string hgid, string lgid)
    {
        // 데이터베이스 연결 및 쿼리 실행 예시 코드
        string query = $"SELECT MAX(GROUP_ORDER) AS MaxGroupOrder FROM TBL_CONTROLLER WHERE HGID = '{hgid}' AND LGID = '{lgid}'";
        using (var connection = new MySqlConnection(strConn))
        {
            connection.Open();
            using (var command = new MySqlCommand(query, connection))
            {
                var result = command.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result) + 1; // 다음 GROUP_ORDER 값을 위해 +1
                }
                else
                {
                    return 1; // GROUP_ORDER가 없는 경우, 1로 시작
                }
            }
        }
    }

    public static DataSet FetchUserData()
    {
        return OnSelectRequest($"SELECT FLD_ID, FLD_NAME, FLD_PW, FLD_EMAIL, FLD_SMS_PHONE, FLD_RECV_SMS, FLD_RECV_PUSH, FLD_AUTH, FLD_ACCESS, FLD_LANGUAGE, FLD_FCM_TOKEN, FLD_DESC FROM TBL_USER", "TBL_USER");
    }

    // TBL_CONFIG 테이블 DataSet 반환
    public static DataSet FetchConfigData()
    {
        return OnSelectRequest($"SELECT NOX, WHOAMI, ETH0, ETH0_MAC, WLAN0, WLAN0_MAC, WORK_ETA, DEV_REG, CLOUD_ID, VENDOR_ID, VENDOR_NAME, WORK_DAYS, SEL_UI, DARKMODE, LANGUAGE, DEMO_MODE, RUSTDESK_ID, NORMAL_SET, SYSTEM_SET, CLOUD_SERVER_URL, CLOUD_REQUEST_URL, BCODE, UPDATE_IDX FROM TBL_CONFIG", "TBL_CONFIG");
    }

    public static DataSet FetchControllingData(DateTime sDate, DateTime eDate, string iid, string cid)
    {
        Debug.Log($"{sDate}, {eDate}, {iid}, {cid}");

        string startDate = sDate.ToString("yyyy-MM-dd HH:mm:ss");
        string endDate = eDate.ToString("yyyy-MM-dd HH:mm:ss");
        if (iid == "all" && cid == "all")
            return OnSelectRequest($"SELECT `ATIME`, `ID`, `CID`, `CNAME`, `DESC`, `CTL_USER` FROM TBL_CONTROLLING WHERE `ATIME` BETWEEN '{startDate}' AND '{endDate}'", "TBL_CONTROLLING");
        else
            return OnSelectRequest($"SELECT `ATIME`, `ID`, `CID`, `CNAME`, `DESC`, `CTL_USER` FROM TBL_CONTROLLING WHERE `ID` = '{iid}' AND `CID` = '{cid}' AND `ATIME` BETWEEN '{startDate}' AND '{endDate}'", "TBL_CONTROLLING");
    }

    public static DataSet FetchWarningData(DateTime sDate, DateTime eDate, string iid, string cid)
    {
        string startDate = sDate.ToString("yyyy-MM-dd HH:mm:ss");
        string endDate = eDate.ToString("yyyy-MM-dd HH:mm:ss");
        if (iid == "all" && cid == "all")
            return OnSelectRequest($"SELECT `ID`, `CID`, `CNAME`, `OCCUR_TIME`, `UNSET_TIME`, `DESC` FROM TBL_WARNING WHERE `OCCUR_TIME` BETWEEN '{startDate}' AND '{endDate}'", "TBL_WARNING");
        else
            return OnSelectRequest($"SELECT `ID`, `CID`, `CNAME`, `OCCUR_TIME`, `UNSET_TIME`, `DESC` FROM TBL_WARNING WHERE `ID` = '{iid}' AND `CID` = '{cid}' AND `OCCUR_TIME` BETWEEN '{startDate}' AND '{endDate}'", "TBL_WARNING");
    }

    // 테이블에서 특정 조건에 맞는 데이터가 존재하는지 확인
    public static bool DoesExistInTableCheck(string p_query)
    {
        try
        {
            using (var connection = new MySqlConnection(strConn))
            {

                connection.Open();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = p_query;

                object result = cmd.ExecuteScalar();

                return result != null;
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError("ClientDatabase : Failed to connect to the database: " + ex.ToString());
            return false;
        }
    }

    // TBL_REALTIME 테이블 DataSet 반환
    public static DataSet FetchRealTimeData()
    {
        return OnSelectRequest("SELECT ID, " +    // 인터페이스 번호(PK)
                                       "CID, " +   // 컨트롤러 번호(PK)
                                       "CONVERT(CNAME USING utf8) AS CNAME, " +  // 컨트롤러 이름
                                       "PKEY, " +    // 프로토콜 코드
                                       "PKEY_TYPE, " +
                                       "UPDATE_TIME, " +   // 갱신시간
                                       "CONN, " +  // 연결여부
                                       "ALARM, " +   // 경고/알람여부(또는 카운트)
                                       "RUN, " +    // 운전여부(0:중지, 1:동작)
                                       "CONVERT(PACKET USING utf8) AS PACKET, " +    // 모드버스 rowdata
                                       "CONVERT(STR_DATA USING utf8) AS STR_DATA, " +    // 모드버스 rowdata를 string으로 변환
                                       "LEN, " +  // 데이터 길이
                                       "LOGS, " +  // 에러로그
                                       "ITYPE, " +    // 인터페이스 구분
                                       "IID, " +    // 인터페이스 ID
                                       "TREND_CNT, " +    // 트렌드 데이터 개수
                                       "TREND0, " +    // 트렌드 인덱스0
                                       "TREND1, " +    // 트렌드 인덱스1
                                       "TREND2, " +    // 트렌드 인덱스2
                                       "TREND3, " +    // 트렌드 인덱스3
                                       "TREND4, " +    // 트렌드 인덱스4
                                       "TREND5, " +    // 트렌드 인덱스5
                                       "TREND6, " +    // 트렌드 인덱스6
                                       "TREND7, " +    // 트렌드 인덱스7
                                       "TREND8, " +    // 트렌드 인덱스8
                                       "TREND9, " +    // 트렌드 인덱스9
                                       "TREND10, " +    // 트렌드 인덱스10
                                       "TREND11, " +    // 트렌드 인덱스11
                                       "TREND12, " +    // 트렌드 인덱스12
                                       "TREND13, " +    // 트렌드 인덱스13
                                       "TREND14, " +    // 트렌드 인덱스14
                                       "TREND15, " +    // 트렌드 인덱스15
                                       "TREND16, " +    // 트렌드 인덱스16
                                       "TREND17, " +    // 트렌드 인덱스17
                                       "TREND18, " +    // 트렌드 인덱스18
                                       "TREND19, " +    // 트렌드 인덱스19
                                       "TREND20, " +    // 트렌드 인덱스20
                                       "TREND21, " +    // 트렌드 인덱스21
                                       "TREND22, " +    // 트렌드 인덱스22
                                       "TREND23, " +    // 트렌드 인덱스23
                                       "TREND24, " +    // 트렌드 인덱스24
                                       "TREND25, " +    // 트렌드 인덱스25
                                       "POWER, " +    // 운전상태
                                       "DEFROST, " +    // 제상상태
                                       "COOL, " +    // 냉방상태
                                       "HEAT, " +    // 난방상태
                                       "HUMI, " +    // 가습상태
                                       "DEHUMI " +    // 제습상태
                                       "FROM TBL_REALTIME", "TBL_REALTIME"); // 인터페이스 COM/IP/PORT
    }

    // TBL_REALTIME_WARNINGS 테이블 DataSet 반환
    public static DataSet FetchRealTimeWarningData()
    {
        return OnSelectRequest("SELECT WTIME, " +    // 발생시간
                                       "`ID`, " +   // 인터페이스 ID
                                       "`CID`, " +  // 컨트롤러 ID
                                       "`INDX`, " +    // 인덱스 번호
                                       "`ADDR`, " +   // 알람 address
                                       "`MASK`, " +   // 알람 bit mask
                                       "`DESC`, " +  // 알람 이름
                                       "`HGID`, " +   // 상위그룹 ID
                                       "`LGID`, " +    // 하위그룹 ID
                                       "`HGNAME`, " +    // 상위그룹명
                                       "`LGNAME`, " +    // 하위그룹명
                                       "`CNAME` " +  // 컨트롤러 이름
                                       "FROM `TBL_REALTIME_WARNINGS`", "TBL_REALTIME_WARNINGS");
    }

    // TBL_REALTIME 테이블 InterfaceID, ControllerID에 해당하는 DataSet 반환
    public static DataSet SearchRealTimeData(string interfaceID, string controllerID)
    {
        return OnSelectRequest("SELECT ID, " +    // 인터페이스 번호(PK)
                                       "CID, " +   // 컨트롤러 번호(PK)
                                       "CONVERT(CNAME USING utf8) AS CNAME, " +  // 컨트롤러 이름
                                       "PKEY, " +    // 프로토콜 코드
                                       "PKEY_TYPE, " +
                                       "UPDATE_TIME, " +   // 갱신시간
                                       "CONN, " +  // 연결여부
                                       "ALARM, " +   // 경고/알람여부(또는 카운트)
                                       "RUN, " +    // 운전여부(0:중지, 1:동작)
                                       "CONVERT(PACKET USING utf8) AS PACKET, " +    // 모드버스 rowdata
                                       "CONVERT(STR_DATA USING utf8) AS STR_DATA, " +    // 모드버스 rowdata를 string으로 변환
                                       "LEN, " +  // 데이터 길이
                                       "LOGS, " +  // 에러로그
                                       "ITYPE, " +    // 인터페이스 구분
                                       "IID, " +    // 인터페이스 ID
                                       "TREND_CNT, " +    // 트렌드 데이터 개수
                                       "TREND0, " +    // 트렌드 인덱스0
                                       "TREND1, " +    // 트렌드 인덱스1
                                       "TREND2, " +    // 트렌드 인덱스2
                                       "TREND3, " +    // 트렌드 인덱스3
                                       "TREND4, " +    // 트렌드 인덱스4
                                       "TREND5, " +    // 트렌드 인덱스5
                                       "TREND6, " +    // 트렌드 인덱스6
                                       "TREND7, " +    // 트렌드 인덱스7
                                       "TREND8, " +    // 트렌드 인덱스8
                                       "TREND9, " +    // 트렌드 인덱스9
                                       "TREND10, " +    // 트렌드 인덱스10
                                       "TREND11, " +    // 트렌드 인덱스11
                                       "TREND12, " +    // 트렌드 인덱스12
                                       "TREND13, " +    // 트렌드 인덱스13
                                       "TREND14, " +    // 트렌드 인덱스14
                                       "TREND15, " +    // 트렌드 인덱스15
                                       "TREND16, " +    // 트렌드 인덱스16
                                       "TREND17, " +    // 트렌드 인덱스17
                                       "TREND18, " +    // 트렌드 인덱스18
                                       "TREND19, " +    // 트렌드 인덱스19
                                       "TREND20, " +    // 트렌드 인덱스20
                                       "TREND21, " +    // 트렌드 인덱스21
                                       "TREND22, " +    // 트렌드 인덱스22
                                       "TREND23, " +    // 트렌드 인덱스23
                                       "TREND24, " +    // 트렌드 인덱스24
                                       "TREND25, " +    // 트렌드 인덱스25
                                       "POWER, " +    // 운전상태
                                       "DEFROST, " +    // 제상상태
                                       "COOL, " +    // 냉방상태
                                       "HEAT, " +    // 난방상태
                                       "HUMI, " +    // 가습상태
                                       "DEHUMI " +    // 제습상태
                                       $"FROM TBL_REALTIME WHERE ID = '{interfaceID}' AND CID = '{controllerID}'", "TBL_REALTIME"); // 인터페이스 COM/IP/PORT
    }

    // TBL_INTERFACE 테이블 InterfaceID에 해당하는 DataSet 반환
    public static DataSet FetchInterfaceData(string interfaceID)
    {
        return OnSelectRequest($"SELECT ID, ITYPE, IID, INAME, ICONFIG, MEMO FROM TBL_INTERFACE WHERE ID = '{interfaceID}'", "TBL_INTERFACE");
    }

    // TBL_INTERFACE 테이블 DataSet 반환
    public static DataSet FetchInterfaceData()
    {
        return OnSelectRequest($"SELECT ID, ITYPE, IID, INAME, ICONFIG, MEMO FROM TBL_INTERFACE", "TBL_INTERFACE");
    }

    // TBL_CONTROLLER 테이블 InterfaceID, ControllerID에 해당하는 DataSet 반환
    public static DataSet FetchControllerData(string interfaceID, string controllerID)
    {
        return OnSelectRequest($"SELECT ID, CID, CNAME, PKEY, PKEY_TYPE, PKEY_ST, PKEY_ED, SKIN, SMS, SHARE, REMARK, ITYPE, IID, ALARM_MIN, HGID, LGID, GROUP_ORDER, FPP_GEN, FPP_MINMAX, FPP_FIX, FPP_X, FPP_Y, FPP_STYLE, SHOW_ADDR, ITEM_SORT, EDIT FROM TBL_CONTROLLER WHERE ID = '{interfaceID}' AND CID = '{controllerID}'", "TBL_CONTROLLER");
    }

    // TBL_CONTROLLER 테이블 InterfaceID에 해당하는 DataSet 반환
    public static DataSet FetchControllerData(string interfaceID)
    {
        return OnSelectRequest($"SELECT ID, CID, CNAME, PKEY, PKEY_TYPE, PKEY_ST, PKEY_ED, SKIN, SMS, SHARE, REMARK, ITYPE, IID, ALARM_MIN, HGID, LGID, GROUP_ORDER, FPP_GEN, FPP_MINMAX, FPP_FIX, FPP_X, FPP_Y, FPP_STYLE, SHOW_ADDR, ITEM_SORT, EDIT FROM TBL_CONTROLLER WHERE ID = '{interfaceID}'", "TBL_CONTROLLER");
    }

    // TBL_CONTROLLER 테이블 DataSet 반환
    public static DataSet FetchControllerData()
    {
        return OnSelectRequest($"SELECT ID, CID, CNAME, PKEY, PKEY_TYPE, PKEY_ST, PKEY_ED, SKIN, SMS, SHARE, REMARK, ITYPE, IID, ALARM_MIN, HGID, LGID, GROUP_ORDER, FPP_GEN, FPP_MINMAX, FPP_FIX, FPP_X, FPP_Y, FPP_STYLE, SHOW_ADDR, ITEM_SORT, EDIT FROM TBL_CONTROLLER", "TBL_CONTROLLER");
    }

    // TBL_HIGH_GROUP 테이블 DataSet 반환
    public static DataSet FetchHighGroupData()
    {
        return OnSelectRequest($"SELECT FLD_HGID, FLD_NAME, FLD_STYLE, FLD_IMG_PATH, FLD_IMG_WIDTH, FLD_IMG_HEIGHT, FLD_UNITY_WIDTH, FLD_UNITY_HEIGHT FROM TBL_HIGH_GROUP", "TBL_HIGH_GROUP");
    }

    // TBL_LOW_GROUP 테이블 DataSet 반환
    public static DataSet FetchLowGroupData()
    {
        return OnSelectRequest($"SELECT FLD_HGID, FLD_LGID, FLD_NAME, FLD_STYLE FROM TBL_LOW_GROUP", "TBL_LOW_GROUP");
    }

    // TBL_PROTOCOL_LIST 테이블 DataSet 반환
    public static DataSet FetchProtocolList()
    {
        return OnSelectRequest($"SELECT `NO`, `USE`, `CATE`, `CATE_NAME`, `NAME`, `VER`, FW_CODE, `KEY`, `XML`, XML2, `LAYOUT`, LAYOUT2, OPTION1, OPTION2, `START`, `END`, REMARK, CLOUD_ID, DEVICE_IMG, CAST(`UPDATE` AS CHAR) AS `UPDATE` FROM TBL_PROTOCOL_LIST", "TBL_PROTOCOL_LIST");

    }

    // TBL_PROTOCOL_LIST 테이블 DataSet 반환
    public static DataSet FetchSerialList()
    {
        return OnSelectRequest($"SELECT `FLD_NAME`, `FLD_UPDATE_TIME` FROM TBL_SERIAL_LIST", "TBL_SERIAL_LIST");
    }

    // 데이터셋에서 조건에 따른 검색 결과를 반환함
    public static DataRow[] SelectRowsFromTable(DataSet dataSet, string tableName, string filterExpression)
    {
        if (dataSet != null && dataSet.Tables.Contains(tableName))
        {
            return dataSet.Tables[tableName].Select(filterExpression);
        }
        else
        {
            return new DataRow[0];
        }
    }
    #endregion

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

    private void OnApplicationQuit()
    {
        //Debug.Log("ClientDatabase : Application Quit");
    }
}
