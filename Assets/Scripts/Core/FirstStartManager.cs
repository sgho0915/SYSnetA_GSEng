using MySqlConnector;
using System;
using System.Data;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // 씬 관리를 위해 추가
using System.Collections;
using System.Collections.Generic;

public class FirstStartManager : MonoBehaviour
{
    ScreenManager screenManager;

    public GameObject firstStartScreen;

    public GameObject firstSet_Start;
    public GameObject firstSet_Server;
    public GameObject firstSet_Server_IP;
    public GameObject firstSet_Server_DBPort;
    public GameObject firstSet_Server_Content;
    public GameObject firstSet_Group;
    public GameObject firstSet_Interface;
    public GameObject firstSet_Controller;
    public GameObject firstSet_Finish;

    public Image logoImage;
    public TextMeshProUGUI logoText;
    public TextMeshProUGUI logoWelcomeText;

    string vendorName = string.Empty;
    string logoImg = string.Empty;
    string urlLogoImg = string.Empty;

    public static FirstStartManager Instance { get; private set; }
    void Awake()
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

        ConfigManager.Instance.LoadSettings();
    }

    private void Start()
    {
        // config.ini에서 IsFirstStart 설정 확인
        string isFirstStart = ConfigManager.Instance.GetSetting("FIRSTSTART");

        if (isFirstStart != "true")
        {
            // 초기 세팅을 이미 마쳤다면 바로 메인화면으로 이동
            screenManager.CurrentScreenState = ScreenManager.ScreenState.Main;
            firstStartScreen.SetActive(false);
        }
        else
        {
            ClientDatabase.isPolling = false;
            screenManager.CurrentScreenState = ScreenManager.ScreenState.None;
            firstStartScreen.SetActive(true);

            StartFirstSetting();
        }
    }

    // 첫 실행 화면
    public void StartFirstSetting()
    {
        firstSet_Start.SetActive(true);
        Button btnGotoServerSet = firstSet_Start.transform.Find("btn_GotoServerSet").GetComponent<Button>();
        btnGotoServerSet.onClick.RemoveAllListeners();
        btnGotoServerSet.onClick.AddListener(() =>
        {
            firstSet_Start.SetActive(false);
                StartServerSettings();
        });
    }

    public void StartServerSettings()
    {
        firstSet_Server.SetActive(true);
        LoadServerSettings();
    }
    public void LoadServerSettings()
    {
        TMP_InputField if_ServerID = firstSet_Server.transform.Find("Setting_Server/SettingServerParent/obj_Setting_Server/List/GroupListScrollView/Viewport/Content/Server/ServerIP/InputField_Value").GetComponent<TMP_InputField>();
        TMP_InputField if_DBPort = firstSet_Server.transform.Find("Setting_Server/SettingServerParent/obj_Setting_Server/List/GroupListScrollView/Viewport/Content/Server/DBPort/InputField_Value").GetComponent<TMP_InputField>();
        Button btnServerUse = firstSet_Server.transform.Find("Setting_Server/SettingServerParent/obj_Setting_Server/List/GroupListScrollView/Viewport/Content/Server/Mode/btnUse").GetComponent<Button>();
        GameObject objServerUse_Selected = btnServerUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objServerUse_NotSelected = btnServerUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnServerNotUse = firstSet_Server.transform.Find("Setting_Server/SettingServerParent/obj_Setting_Server/List/GroupListScrollView/Viewport/Content/Server/Mode/btnNotUse").GetComponent<Button>();
        GameObject objServerNotUse_Selected = btnServerNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objServerNotUse_NotSelected = btnServerNotUse.gameObject.transform.Find("NotSelected").gameObject;
         
        string currentMode = ConfigManager.Instance.GetSetting("MODE");
        if_ServerID.text = ConfigManager.Instance.GetSetting("SERVER_IP");
        if_DBPort.text = ConfigManager.Instance.GetSetting("DB_PORT");
        string tmpServerID = ConfigManager.Instance.GetSetting("SERVER_IP");
        string tmpDBPort = ConfigManager.Instance.GetSetting("DB_PORT");

        // 서버/클라이언트 모드
        if (currentMode == "SERVER")
        {
            currentMode = "SERVER";
            objServerUse_Selected.SetActive(true);
            objServerUse_NotSelected.SetActive(false);
            objServerNotUse_Selected.SetActive(false);
            objServerNotUse_NotSelected.SetActive(true);
        }
        else
        {
            currentMode = "CLIENT";
            objServerUse_Selected.SetActive(false);
            objServerUse_NotSelected.SetActive(true);
            objServerNotUse_Selected.SetActive(true);
            objServerNotUse_NotSelected.SetActive(false);
        }

        btnServerUse.onClick.RemoveAllListeners();
        btnServerNotUse.onClick.RemoveAllListeners();
        btnServerUse.onClick.AddListener(() =>
        {
            currentMode = "SERVER";
            objServerUse_Selected.SetActive(true);
            objServerUse_NotSelected.SetActive(false);
            objServerNotUse_Selected.SetActive(false);
            objServerNotUse_NotSelected.SetActive(true);
            firstSet_Server_IP.SetActive(false);
            firstSet_Server_DBPort.SetActive(false);
            if_ServerID.text = "127.0.0.1";
            if_DBPort.text = "28365";
            LayoutRebuilder.ForceRebuildLayoutImmediate(firstSet_Server_Content.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        });
        btnServerNotUse.onClick.AddListener(() =>
        {
            currentMode = "CLIENT";
            objServerUse_Selected.SetActive(false);
            objServerUse_NotSelected.SetActive(true);
            objServerNotUse_Selected.SetActive(true);
            objServerNotUse_NotSelected.SetActive(false);
            firstSet_Server_IP.SetActive(true);
            firstSet_Server_DBPort.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(firstSet_Server_Content.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        });

        Button btnSaveServerSet = firstSet_Server.transform.Find("Setting_Server/SettingServerParent/obj_Setting_Server/Bottom/btn_Save").GetComponent<Button>();
        btnSaveServerSet.onClick.RemoveAllListeners();
        btnSaveServerSet.onClick.AddListener(() =>
        {
            string query = $"SELECT NOX FROM TBL_CONFIG";

            ClientDatabase.db_HostAddress = if_ServerID.text;
            ClientDatabase.db_port = if_DBPort.text;
            ClientDatabase.strConn = $"server={if_ServerID.text}; port={if_DBPort.text}; user={ClientDatabase.db_ID}; password={ClientDatabase.db_PW}; database={ClientDatabase.db_Name}; charset=utf8; Pooling=true; Min Pool Size={ClientDatabase.db_MinPoolSize}; Max Pool Size={ClientDatabase.db_MaxPoolSize}; SslMode=none;";

            if (ClientDatabase.DoesExistInTableCheck(query))
            {
                SaveServerSettings(if_ServerID.text, if_DBPort.text, currentMode);
            }
            else
            {
                screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                screenManager.txt_PopUpMsg.text = "DB 연결에 실패했습니다\n접속정보 혹은 통신환경을 확인해주세요.";
                screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                screenManager.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    screenManager.ClosePopUpMessage();
                    if_ServerID.text = tmpServerID;
                    if_DBPort.text = tmpDBPort;

                    ClientDatabase.db_HostAddress = tmpServerID;
                    ClientDatabase.db_port = tmpDBPort;
                    ClientDatabase.strConn = $"server={tmpServerID}; port={tmpDBPort}; user={ClientDatabase.db_ID}; password={ClientDatabase.db_PW}; database={ClientDatabase.db_Name}; charset=utf8; Pooling=true; Min Pool Size={ClientDatabase.db_MinPoolSize}; Max Pool Size={ClientDatabase.db_MaxPoolSize}; SslMode=none;";
                });
            }
        });
    }

    // 서버 설정 저장
    public void SaveServerSettings(string serverID, string dbPort, string mode)
    {
        ClientDatabase.db_HostAddress = serverID;
        ClientDatabase.db_port = dbPort;
        ClientDatabase.strConn = $"server={serverID}; port={dbPort}; user={ClientDatabase.db_ID}; password={ClientDatabase.db_PW}; database={ClientDatabase.db_Name}; charset=utf8; Pooling=true; Min Pool Size={ClientDatabase.db_MinPoolSize}; Max Pool Size={ClientDatabase.db_MaxPoolSize}; SslMode=none;";

        ConfigManager.Instance.SetSetting("MODE", $"{mode}"); // 서버/클라이언트 모드 저장
        ConfigManager.Instance.SetSetting("SERVER_IP", $"{serverID}"); // 서버 IP 저장
        ConfigManager.Instance.SetSetting("DB_PORT", $"{dbPort}"); // DB Port 저장

        screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
        screenManager.txt_PopUpMsg.text = "설정이 저장되었습니다.";
        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
        screenManager.btnPopUpConfirm.onClick.AddListener(() =>
        {
            screenManager.ClosePopUpMessage();
            firstSet_Server.SetActive(false);
            StartGroupSettings();
        });
    }

    // 그룹 설정 화면
    public void StartGroupSettings()
    {
        firstSet_Group.SetActive(true);
        GameObject settingGroup = firstSet_Group.transform.Find("Setting_Group").gameObject;        
        settingGroup.SetActive(true);
        GroupSettingManager.Instance.LoadGroupAssets();
    }

    // 인터페이스 설정 화면
    public void StartInterfaceSettings()
    {
        firstSet_Interface.SetActive(true);
        GameObject settingInterface = firstSet_Interface.transform.Find("Setting_Interface").gameObject;
        settingInterface.SetActive(true);
        InterfaceSettingManager.Instance.LoadInterfaceAssets();
    }

    // 컨트롤러 설정 화면
    public void StartControllerSettings()
    {
        firstSet_Controller.SetActive(true);
        GameObject settingController = firstSet_Controller.transform.Find("Setting_Controller").gameObject;
        settingController.SetActive(true);
        ControllerSettingManager.Instance.LoadControllerAssets();
    }

    // 마지막 화면
    public void FinishedFirstSettings()
    {
        firstSet_Finish.SetActive(true);
        Button btnGotoMain = firstSet_Finish.transform.Find("btn_GotoMainMenu").GetComponent<Button>();
        btnGotoMain.onClick.RemoveAllListeners();
        btnGotoMain.onClick.AddListener(() =>
        {
            ClientDatabase.isPolling = true;
            //Debug.Log("FirstStartManager : FinishedFirstSettings");
            ConfigManager.Instance.SetSetting("FIRSTSTART", "false");
            firstSet_Finish.SetActive(false);
            firstStartScreen.SetActive(false);
            screenManager.GotoMain();

            if (ClientDatabase.configData != null && ClientDatabase.configData.Tables.Count > 0)
            {
                DataTable tblConfig = ClientDatabase.configData.Tables[0];
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
        });
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
}