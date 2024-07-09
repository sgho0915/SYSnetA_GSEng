using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class NormalSettings
{
    public string pollingInt; // ���� ����
    public string noRespCnt; // ������ �Ǵ� Ƚ��
    public string sendSMS; // �ø��� ���� ���� �Ұ� �� SMS �߼�
    public string trendSaveInt; // Ʈ���� ���� ����
    public string dataArchive; // ������ ���� ���
    public string archivePeriod; // ������ ���� �Ⱓ
}

[Serializable]
public class SystemSettings
{
    public string volLevel; // ���� ũ��
    public string touchSound; // ��ġ��
    public string alarmSound; // �溸��
    public string colorTheme; // �⺻ ���� ����
    public string fieldAddr; // ��� ���� ���θ� �ּ�
    public string weatherUse; // ���� ������ ��� ����
    public string timeUse; // �޴��� ��¥/�ð� ��� ����    
}

public class SettingManager : MonoBehaviour
{
    ScreenManager screenManager;

    public GameObject settingScreen;
    private WebViewObject webViewObject;

    [Header("���� ����")]
    public Button btnServerSet;
    public GameObject screenServerSet;
    public GameObject item_ServerUse; // ����/Ŭ���̾�Ʈ ���
    public GameObject item_ServerIP; // ���� IP
    public GameObject item_DBPort; // DB Port
    public GameObject serverContent;
    private Button btnSaveServerSet;

    [Header("�Ϲ� ����")]
    public Button btnNormalSet;
    public GameObject screenNormalSet;
    public GameObject item_PollingInt; // ���� ����
    public GameObject item_NoRespCnt; // ������ Ƚ��
    public GameObject item_SendSMS; // �ø��� ��Ʈ ���� �Ұ� �� SMS �߼�
    public GameObject item_TrendSaveInt; // Ʈ���� ���� ����
    public GameObject item_DataArchive; // ������ ���� ��� �� �Ⱓ
    private Button btnSaveNormalSet;

    [Header("�ý��� ����")]
    public Button btnSystemSet;
    public GameObject screenSystemSet;
    public GameObject item_VolLevel; // ���� ũ��
    public GameObject item_TouchSnd; // ��ġ��
    public GameObject item_AlarmSnd; // �溸��
    public GameObject item_ColorTheme; // �⺻ ����
    public GameObject item_FieldAddr; // ������� ���θ� �ּ�
    public GameObject item_WeatherUse; // ���� ������ ���
    public GameObject item_TimeUse; // �ð� ���
    public GameObject item_LockUse; // ȭ�� ��� ���
    public GameObject item_ChangePW; // ��й�ȣ ����
    public GameObject settingPasswd; // ��й�ȣ ����ȭ��
    public GameObject lockButton;
    private Button btnSaveSystemSet;    

    [Header("������Ʈ")]
    public Button btnUpdateSet;
    public GameObject screenUpdateSet;
    public GameObject item_ChangeLog; // ü���� �α�
    public GameObject item_DownloadInfo; // �ٿ�ε� ����
    public GameObject item_UpdateInfo; // ������Ʈ ����
    public GameObject item_AutoUpdateCheck; // �ڵ� ������Ʈ üũ
    public Button btnSave; // ���� ����
    public Button btnUpdate; // ������Ʈ �ٿ�ε� �� ��ġ
    public Transform updateInfoTransform;

    [Header("�Ŵ���")]
    public Button btnManualSet;
    public GameObject screenManual;
    public List<Sprite> listManualPages;
    public GameObject pagePrefab;
    public Transform manualScrollViewContent;
    public static Dictionary<string, GameObject> pageInstances = new Dictionary<string, GameObject>();

    [Header("����")]
    public Button btnInfoSet;
    public GameObject screenInfoSet;
    public GameObject item_Logo; // �ΰ�
    public GameObject item_SwInfo; // sw ����
    public GameObject item_EthInfo; // �̴��� ����
    public GameObject item_WlanInfo; // ������ ����
    public GameObject item_CloudInfo; // Ŭ���� ����
    public GameObject item_RuskDeskInfo; // �������� ����
    public GameObject item_StorageInfo; // ����� ����
    private int clickCount = 0;

    // ���̵� �޴� ����
    private GameObject objServerSelected;
    private GameObject objServerNotSelected;
    private GameObject objNormalSelected;
    private GameObject objNormalNotSelected;
    private GameObject objSystemSelected;
    private GameObject objSystemNotSelected;
    private GameObject objUpdateSelected;
    private GameObject objUpdateNotSelected;
    private GameObject objManualSelected;
    private GameObject objManualNotSelected;
    private GameObject objInfoSelected;
    private GameObject objInfoNotSelected;

    private List<GameObject> uiList = new List<GameObject>();
    public GameObject gpioScreen;

    // DB �ʵ�
    public static string nox = string.Empty; // ����
    public static string whoami = string.Empty; // ��� ���� Termux ���� ID
    public static string eth0 = string.Empty; // ������ IP
    public static string eth0_Mac = string.Empty; // ������ �ƾ�巹��
    public static string wlan0 = string.Empty; // ������ IP
    public static string wlan0_Mac = string.Empty; // ������ �ƾ�巹��
    public static string workEta = string.Empty; // �۵� ���� �Ⱓ(�Ⱓ �� ���� �ϼ� ����Ͽ� ����)
    public static string devReg = string.Empty; // Ŭ���� ����� ��¥
    public static string cloudId = string.Empty; // Ŭ���� ����� ID ����
    public static string vendorID = string.Empty; // ���ó �Ǵ� ��, �븮�� ID
    public static string vendorName = string.Empty; // ���ó �Ǵ� ��, �븮�� �̸�
    public static string workDays = string.Empty; // �۵� ���� �ϼ�(�¶��� ��� �� �ڵ� ���̳ʽ� ���� ó��)
    public static string selUi = string.Empty; // ���õ� �ý��� UI
    public static string darkMode = string.Empty; // ��ũ��� ���
    public static string language = string.Empty; // ��� ȯ�� ���
    public static string demoMode = string.Empty; // ������
    public static string normalSet = string.Empty; // �Ϲݼ��� Json
    public static string systemSet = string.Empty; // �ý��ۼ��� Json

    // �Ϲݼ��� json ����
    public static int pollingInt = 0;
    public static int noRespCnt = 0;
    public static int sendSMS = 0;
    public static int trendSaveInt = 0;
    public static int dataArchive = 0;
    public static int archivePeriod = 0;

    // �ý��ۼ��� json ����
    public static int volLevel = 0;
    public static int touchSound = 0;
    public static int alarmSound = 0;
    public static int colorTheme = 0;
    public static string fieldAddr = string.Empty;
    public static int weatherUse = 0;
    public static int timeUse = 0;
    public static string lockUse = string.Empty;
    public static string passwd = string.Empty;
    public static string lockState = string.Empty;

    public static SettingManager Instance { get; private set; }

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

        btnServerSet.onClick.RemoveAllListeners();
        btnNormalSet.onClick.RemoveAllListeners();
        btnSystemSet.onClick.RemoveAllListeners();
        btnUpdateSet.onClick.RemoveAllListeners();
        btnManualSet.onClick.RemoveAllListeners();
        btnInfoSet.onClick.RemoveAllListeners();

        objServerSelected = btnServerSet.transform.Find("Selected").gameObject;
        objServerNotSelected = btnServerSet.transform.Find("NotSelected").gameObject;
        objNormalSelected = btnNormalSet.transform.Find("Selected").gameObject;
        objNormalNotSelected = btnNormalSet.transform.Find("NotSelected").gameObject;
        objSystemSelected = btnSystemSet.transform.Find("Selected").gameObject;
        objSystemNotSelected = btnSystemSet.transform.Find("NotSelected").gameObject;
        objUpdateSelected = btnUpdateSet.transform.Find("Selected").gameObject;
        objUpdateNotSelected = btnUpdateSet.transform.Find("NotSelected").gameObject;
        objManualSelected = btnManualSet.transform.Find("Selected").gameObject;
        objManualNotSelected = btnManualSet.transform.Find("NotSelected").gameObject;
        objInfoSelected = btnInfoSet.transform.Find("Selected").gameObject;
        objInfoNotSelected = btnInfoSet.transform.Find("NotSelected").gameObject;

        uiList.Add(objServerSelected);
        uiList.Add(objServerNotSelected);
        uiList.Add(objNormalSelected);
        uiList.Add(objNormalNotSelected);
        uiList.Add(objSystemSelected);
        uiList.Add(objSystemNotSelected);
        uiList.Add(objUpdateSelected);
        uiList.Add(objUpdateNotSelected);
        uiList.Add(objManualSelected);
        uiList.Add(objManualNotSelected);
        uiList.Add(objInfoSelected);
        uiList.Add(objInfoNotSelected);
        uiList.Add(screenServerSet);
        uiList.Add(screenNormalSet);
        uiList.Add(screenSystemSet);
        uiList.Add(screenUpdateSet);
        uiList.Add(screenManual);
        uiList.Add(screenInfoSet);

        btnServerSet.onClick.AddListener(() => ChangeServerSetting());
        btnNormalSet.onClick.AddListener(() => ChangeNormalSetting());
        btnSystemSet.onClick.AddListener(() => ChangeSystemSetting());
        btnUpdateSet.onClick.AddListener(() => ChangeUpdateSetting());
        btnManualSet.onClick.AddListener(() => ChangeManualSetting());
        btnInfoSet.onClick.AddListener(() => ChangeInfoSetting());        
    }

    private void Start()
    {
        LoadSettingScreen();
    }

    public void LoadSettingScreen()
    {
        ChangeServerSetting();
        DataSet ds = ClientDatabase.FetchConfigData();
        if (ds != null && ds.Tables.Count > 0)
        {
            DataTable tblConfig = ds.Tables[0];
            foreach (DataRow row in tblConfig.Rows)
            {
                nox = row["NOX"].ToString(); // ����
                whoami = row["WHOAMI"].ToString(); // ��� ���� Termux ���� ID
                eth0 = row["ETH0"].ToString(); // ������ IP
                eth0_Mac = row["ETH0_MAC"].ToString(); // ������ �ƾ�巹��
                wlan0 = row["WLAN0"].ToString(); // ������ IP
                wlan0_Mac = row["WLAN0_MAC"].ToString(); // ������ �ƾ�巹��
                workEta = row["WORK_ETA"].ToString(); // �۵� ���� �Ⱓ(�Ⱓ �� ���� �ϼ� ����Ͽ� ����)
                devReg = row["DEV_REG"].ToString(); // Ŭ���� ����� ��¥
                cloudId = row["CLOUD_ID"].ToString(); // Ŭ���� ����� ID ����
                vendorID = row["VENDOR_ID"].ToString(); // ���ó �Ǵ� ��, �븮�� ID
                vendorName = row["VENDOR_NAME"].ToString(); // ���ó �Ǵ� ��, �븮�� �̸�
                workDays = row["WORK_DAYS"].ToString(); // �۵� ���� �ϼ�(�¶��� ��� �� �ڵ� ���̳ʽ� ���� ó��)
                selUi = row["SEL_UI"].ToString(); // ���õ� �ý��� UI
                darkMode = row["DARKMODE"].ToString(); // ��ũ��� ���
                language = row["LANGUAGE"].ToString(); // ��� ȯ�� ���
                demoMode = row["DEMO_MODE"].ToString(); // ������
                normalSet = row["NORMAL_SET"].ToString(); // �Ϲݼ��� Json
                systemSet = row["SYSTEM_SET"].ToString(); // �ý��ۼ��� Json
            }
        }
        else
        {
            Debug.LogError("config data is null");
            return;
        }

        

        NormalSettings normalSetJson = JsonUtility.FromJson<NormalSettings>(normalSet);
        pollingInt = int.Parse(normalSetJson.pollingInt);
        noRespCnt = int.Parse(normalSetJson.noRespCnt);
        sendSMS = int.Parse(normalSetJson.sendSMS);
        trendSaveInt = int.Parse(normalSetJson.trendSaveInt);
        dataArchive = int.Parse(normalSetJson.dataArchive);
        archivePeriod = int.Parse(normalSetJson.archivePeriod);

        SystemSettings systemSetJson = JsonUtility.FromJson<SystemSettings>(systemSet);
        volLevel = int.Parse(systemSetJson.volLevel);
        touchSound = int.Parse(systemSetJson.touchSound);
        alarmSound = int.Parse(systemSetJson.alarmSound);
        colorTheme = int.Parse(systemSetJson.colorTheme);
        fieldAddr = systemSetJson.fieldAddr;
        weatherUse = int.Parse(systemSetJson.weatherUse);
        timeUse = int.Parse(systemSetJson.timeUse);

        //if (ScreenManager.isTablet)
        //{
        //    volLevel = int.Parse(systemSetJson.volLevel);
        //    touchSound = int.Parse(systemSetJson.touchSound);
        //    alarmSound = int.Parse(systemSetJson.alarmSound);
        //    colorTheme = int.Parse(systemSetJson.colorTheme);
        //    fieldAddr = systemSetJson.fieldAddr;
        //    weatherUse = int.Parse(systemSetJson.weatherUse);
        //    timeUse = int.Parse(systemSetJson.timeUse);
        //}
        //else
        //{
        //    volLevel = int.Parse(ConfigManager.Instance.GetSetting("VOLUME_LEVEL"));
        //    touchSound = int.Parse(ConfigManager.Instance.GetSetting("TOUCH_SOUND"));
        //    alarmSound = int.Parse(ConfigManager.Instance.GetSetting("ALARM_SOUND"));
        //    fieldAddr = string.Empty;
        //    colorTheme = int.Parse(ConfigManager.Instance.GetSetting("COLOR_THEME"));            
        //    weatherUse = int.Parse(ConfigManager.Instance.GetSetting("WEATHER_USE"));
        //    timeUse = 0;
        //}        

        GotoDeviceSettings();
    }

    private void GotoDeviceSettings()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Button btnGoToSettings = item_SwInfo.transform.Find("btnValueHidden").GetComponent<Button>();
            btnGoToSettings.onClick.RemoveAllListeners();
            btnGoToSettings.onClick.AddListener(() =>
            {
                clickCount++;

                if (clickCount >= 5) // Ŭ�� ī��Ʈ�� 5 �̻��̸�
                {
                    using (AndroidJavaClass javaClass = new AndroidJavaClass("com.systronics.plugin.SettingsOpener"))
                    {
                        javaClass.CallStatic("openSettings"); // ���� �� ����
                    }
                    clickCount = 0; // ī��Ʈ ����
                }
            });

            Button btnShowGPIO = item_EthInfo.transform.Find("btnShowGPIO").GetComponent<Button>();
            btnShowGPIO.onClick.RemoveAllListeners();
            btnShowGPIO.onClick.AddListener(() =>
            {
                clickCount++;

                if (clickCount >= 5) // Ŭ�� ī��Ʈ�� 5 �̻��̸�
                {
                    gpioScreen.SetActive(true);
                    clickCount = 0; // ī��Ʈ ����
                }
            });
        }            
    }

    #region �������
    // ���� ���� �ε�
    public void LoadServerSettings()
    {
        TMP_InputField if_ServerID = item_ServerIP.transform.Find("InputField_Value").GetComponent<TMP_InputField>();
        TMP_InputField if_DBPort = item_DBPort.transform.Find("InputField_Value").GetComponent<TMP_InputField>();
        Button btnServerUse = item_ServerUse.transform.Find("btnUse").GetComponent<Button>();
        GameObject objServerUse_Selected = btnServerUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objServerUse_NotSelected = btnServerUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnServerNotUse = item_ServerUse.transform.Find("btnNotUse").GetComponent<Button>();
        GameObject objServerNotUse_Selected = btnServerNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objServerNotUse_NotSelected = btnServerNotUse.gameObject.transform.Find("NotSelected").gameObject;

        string currentMode = ConfigManager.Instance.GetSetting("MODE");
        if_ServerID.text = ConfigManager.Instance.GetSetting("SERVER_IP");
        if_DBPort.text = ConfigManager.Instance.GetSetting("DB_PORT");

        string tmpServerID = ConfigManager.Instance.GetSetting("SERVER_IP");
        string tmpDBPort = ConfigManager.Instance.GetSetting("DB_PORT");

        // ����/Ŭ���̾�Ʈ ���        
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
            item_ServerIP.gameObject.SetActive(false);
            item_DBPort.gameObject.SetActive(false);
            if_ServerID.text = "127.0.0.1";
            if_DBPort.text = "28365";
            LayoutRebuilder.ForceRebuildLayoutImmediate(serverContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        });
        btnServerNotUse.onClick.AddListener(() =>
        {
            currentMode = "CLIENT";
            objServerUse_Selected.SetActive(false);
            objServerUse_NotSelected.SetActive(true);
            objServerNotUse_Selected.SetActive(true);
            objServerNotUse_NotSelected.SetActive(false);
            item_ServerIP.gameObject.SetActive(true);
            item_DBPort.gameObject.SetActive(true);
            if_ServerID.text = tmpServerID;
            if_DBPort.text = tmpDBPort;
            LayoutRebuilder.ForceRebuildLayoutImmediate(serverContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        });

        btnSaveServerSet = screenServerSet.transform.Find("Bottom/btnSave").GetComponent<Button>();
        btnSaveServerSet.onClick.RemoveAllListeners();
        btnSaveServerSet.onClick.AddListener(() =>
        {
            //SaveServerSettings(if_ServerID.text, if_DBPort.text, currentMode);
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
                screenManager.txt_PopUpMsg.text = "DB ���ῡ �����߽��ϴ�\n�������� Ȥ�� ���ȯ���� Ȯ�����ּ���.";
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

    // ���� ���� ����
    public void SaveServerSettings(string serverID, string dbPort, string mode)
    {
        ClientDatabase.db_HostAddress = serverID;
        ClientDatabase.db_port = dbPort;
        ClientDatabase.strConn = $"server={serverID}; port={dbPort}; user={ClientDatabase.db_ID}; password={ClientDatabase.db_PW}; database={ClientDatabase.db_Name}; charset=utf8; Pooling=true; Min Pool Size={ClientDatabase.db_MinPoolSize}; Max Pool Size={ClientDatabase.db_MaxPoolSize}; SslMode=none;";

        ConfigManager.Instance.SetSetting("MODE", $"{mode}"); // ����/Ŭ���̾�Ʈ ��� ����
        ConfigManager.Instance.SetSetting("SERVER_IP", $"{serverID}"); // ���� IP ����
        ConfigManager.Instance.SetSetting("DB_PORT", $"{dbPort}"); // DB Port ����

        screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
        screenManager.txt_PopUpMsg.text = "������ ����Ǿ����ϴ�.";
        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
        screenManager.btnPopUpConfirm.onClick.AddListener(() =>
        {
            screenManager.ClosePopUpMessage();
        });
    }

    // ���� ���� ��ȯ
    public void ChangeServerSetting()
    {
        HideUI();
        objServerSelected.SetActive(true);
        objNormalNotSelected.SetActive(true);
        objSystemNotSelected.SetActive(true);
        objUpdateNotSelected.SetActive(true);
        objManualNotSelected.SetActive(true);
        objInfoNotSelected.SetActive(true);
        screenServerSet.SetActive(true);

        LoadServerSettings();
    }
    #endregion

    #region �Ϲ� ����
    // �Ϲ� ���� �ε�
    public void LoadNormalSettings(int pollingInt, int noRespCnt, int sendSMS, int trendSaveInt, int dataArchive, int archivePeriod)
    {
        TMP_InputField if_PollingInterval = item_PollingInt.transform.Find("InputField_Value").GetComponent<TMP_InputField>();
        TMP_InputField if_NoRespCnt = item_NoRespCnt.transform.Find("InputField_Value").GetComponent<TMP_InputField>();
        Toggle tg_SendSMS = item_SendSMS.transform.Find("Toggle").GetComponent<Toggle>();
        TMP_InputField if_TrendSaveInt = item_TrendSaveInt.transform.Find("InputField_Value").GetComponent<TMP_InputField>();
        Toggle tg_UseDataArchive = item_DataArchive.transform.Find("ToggleDataArchiveUse/Toggle").GetComponent<Toggle>();
        TMP_InputField if_DataArchiveInt = item_DataArchive.transform.Find("InputField_Value").GetComponent<TMP_InputField>();

        if_PollingInterval.text = pollingInt.ToString();
        if_NoRespCnt.text = noRespCnt.ToString();
        tg_SendSMS.isOn = sendSMS == 1 ? true : false;
        if_TrendSaveInt.text = trendSaveInt.ToString();
        tg_UseDataArchive.isOn = dataArchive == 1 ? true : false;
        if_DataArchiveInt.text = archivePeriod.ToString();

        if(tg_UseDataArchive.isOn)
            if_DataArchiveInt.enabled = true;
        else
            if_DataArchiveInt.enabled = false;
        tg_UseDataArchive.onValueChanged.AddListener((value) => {
            if_DataArchiveInt.enabled = value;
        });

        btnSaveNormalSet = screenNormalSet.transform.Find("Bottom/btnSave").GetComponent<Button>();
        btnSaveNormalSet.onClick.RemoveAllListeners();
        btnSaveNormalSet.onClick.AddListener(() =>
        {
            SaveNormalSettings(int.Parse(if_PollingInterval.text), int.Parse(if_NoRespCnt.text), tg_SendSMS.isOn ? 1 : 0, int.Parse(if_TrendSaveInt.text), tg_UseDataArchive.isOn ? 1 : 0, int.Parse(if_DataArchiveInt.text));
        });
    }

    // �Ϲ� ���� ����
    public void SaveNormalSettings(int p_pollingInt, int p_noRespCnt, int p_sendSMS, int p_trendSaveInt, int p_dataArchive, int p_archivePeriod)
    {
        string normalSetJson = $"{{\"pollingInt\":{p_pollingInt},\"noRespCnt\":{p_noRespCnt},\"sendSMS\":{p_sendSMS},\"trendSaveInt\":{p_trendSaveInt},\"dataArchive\":{p_dataArchive},\"archivePeriod\":{p_archivePeriod}}}";
        string query = $"UPDATE TBL_CONFIG SET NORMAL_SET = '{normalSetJson}'";
        
        pollingInt = p_pollingInt;
        noRespCnt = p_noRespCnt;
        sendSMS = p_sendSMS;
        trendSaveInt = p_trendSaveInt;
        dataArchive = p_dataArchive;
        archivePeriod = p_archivePeriod;

        if (ClientDatabase.OnUpdateRequest(query))
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "������ ����Ǿ����ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() =>
            {
                screenManager.ClosePopUpMessage();
            });
        }
        else
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "���� ���忡 �����߽��ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() =>
            {
                screenManager.ClosePopUpMessage();
            });
        }
    }

    // �Ϲ� ���� ��ȯ
    public void ChangeNormalSetting()
    {
        HideUI();
        objNormalSelected.SetActive(true);
        objSystemNotSelected.SetActive(true);
        objServerNotSelected.SetActive(true);
        objUpdateNotSelected.SetActive(true);
        objManualNotSelected.SetActive(true);
        objInfoNotSelected.SetActive(true);
        screenNormalSet.SetActive(true);

        DataTable tblConfig = ClientDatabase.FetchConfigData().Tables[0];
        foreach (DataRow row in tblConfig.Rows)
        {
            nox = row["NOX"].ToString(); // ����
            whoami = row["WHOAMI"].ToString(); // ��� ���� Termux ���� ID
            eth0 = row["ETH0"].ToString(); // ������ IP
            eth0_Mac = row["ETH0_MAC"].ToString(); // ������ �ƾ�巹��
            wlan0 = row["WLAN0"].ToString(); // ������ IP
            wlan0_Mac = row["WLAN0_MAC"].ToString(); // ������ �ƾ�巹��
            workEta = row["WORK_ETA"].ToString(); // �۵� ���� �Ⱓ(�Ⱓ �� ���� �ϼ� ����Ͽ� ����)
            devReg = row["DEV_REG"].ToString(); // Ŭ���� ����� ��¥
            cloudId = row["CLOUD_ID"].ToString(); // Ŭ���� ����� ID ����
            vendorID = row["VENDOR_ID"].ToString(); // ���ó �Ǵ� ��, �븮�� ID
            vendorName = row["VENDOR_NAME"].ToString(); // ���ó �Ǵ� ��, �븮�� �̸�
            workDays = row["WORK_DAYS"].ToString(); // �۵� ���� �ϼ�(�¶��� ��� �� �ڵ� ���̳ʽ� ���� ó��)
            selUi = row["SEL_UI"].ToString(); // ���õ� �ý��� UI
            darkMode = row["DARKMODE"].ToString(); // ��ũ��� ���
            language = row["LANGUAGE"].ToString(); // ��� ȯ�� ���
            demoMode = row["DEMO_MODE"].ToString(); // ������
            normalSet = row["NORMAL_SET"].ToString(); // �Ϲݼ��� Json
            systemSet = row["SYSTEM_SET"].ToString(); // �ý��ۼ��� Json
        }

        NormalSettings normalSetJson = JsonUtility.FromJson<NormalSettings>(normalSet);
        pollingInt = int.Parse(normalSetJson.pollingInt);
        noRespCnt = int.Parse(normalSetJson.noRespCnt);
        sendSMS = int.Parse(normalSetJson.sendSMS);
        trendSaveInt = int.Parse(normalSetJson.trendSaveInt);
        dataArchive = int.Parse(normalSetJson.dataArchive);
        archivePeriod = int.Parse(normalSetJson.archivePeriod);

        LoadNormalSettings(pollingInt, noRespCnt, sendSMS, trendSaveInt, dataArchive, archivePeriod);
    }
    #endregion

    #region �ý��� ����
    // �ý��� ���� �ε�
    public void LoadSystemSettings(int volLevel, int touchSound, int alarmSound, int colorTheme, string fieldAddr, int weatherUse, int timeUse, string lockUse, string passwd, string lockState)
    {
        Slider sld_VolLevel = item_VolLevel.transform.Find("Slider").GetComponent<Slider>();
        TMP_Dropdown dd_TouchSnd = item_TouchSnd.transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        TMP_Dropdown dd_AlarmSnd = item_AlarmSnd.transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        Toggle tg_Green = item_ColorTheme.transform.Find("ToggleColorContainer/Toggle_Green").GetComponent<Toggle>();
        Toggle tg_BusungBlue = item_ColorTheme.transform.Find("ToggleColorContainer/Toggle_BusungBlue").GetComponent<Toggle>();
        Toggle tg_Blue = item_ColorTheme.transform.Find("ToggleColorContainer/Toggle_Blue").GetComponent<Toggle>();
        Toggle tg_Navy = item_ColorTheme.transform.Find("ToggleColorContainer/Toggle_Navy").GetComponent<Toggle>();
        Toggle tg_Red = item_ColorTheme.transform.Find("ToggleColorContainer/Toggle_Red").GetComponent<Toggle>();
        Toggle tg_Black = item_ColorTheme.transform.Find("ToggleColorContainer/Toggle_Black").GetComponent<Toggle>();
        TMP_InputField if_FieldAddr = item_FieldAddr.transform.Find("InputField_Value").GetComponent<TMP_InputField>();
        Button btnWeatherUse = item_WeatherUse.transform.Find("btnUse").GetComponent<Button>();
        GameObject objWeatherUse_Selected = btnWeatherUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objWeatherUse_NotSelected = btnWeatherUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnWeatherNotUse = item_WeatherUse.transform.Find("btnNotUse").GetComponent<Button>();
        GameObject objWeatherNotUse_Selected = btnWeatherNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objWeatherNotUse_NotSelected = btnWeatherNotUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnTimeUse = item_TimeUse.transform.Find("btnUse").GetComponent<Button>();
        GameObject objTimeUse_Selected = btnTimeUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objTimeUse_NotSelected = btnTimeUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnTimeNotUse = item_TimeUse.transform.Find("btnNotUse").GetComponent<Button>();
        GameObject objTimeNotUse_Selected = btnTimeNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objTimeNotUse_NotSelected = btnTimeNotUse.gameObject.transform.Find("NotSelected").gameObject;

        Button btnLockUse = item_LockUse.gameObject.transform.Find("btnUse").GetComponent<Button>();
        GameObject objLockUse_Selected = btnLockUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objLockUse_NotSelected = btnLockUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnLockNotUse = item_LockUse.transform.Find("btnNotUse").GetComponent<Button>();
        GameObject objLockNotUse_Selected = btnLockNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objLockNotUse_NotSelected = btnLockNotUse.gameObject.transform.Find("NotSelected").gameObject;

        // ���� ũ��
        sld_VolLevel.value = volLevel;
        sld_VolLevel.onValueChanged.AddListener((value) => {
            volLevel = (int)value;        
        });

        // ��ġ��
        dd_TouchSnd.onValueChanged.RemoveAllListeners();
        dd_TouchSnd.ClearOptions();
        List<string> touchSounds = new List<string> { "����", "��ġ��1", "��ġ��2", "��ġ��3" };
        List<TMP_Dropdown.OptionData> options_touchSounds = new List<TMP_Dropdown.OptionData>();
        foreach (string sound in touchSounds)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(sound);
            options_touchSounds.Add(option);
        }
        dd_TouchSnd.AddOptions(options_touchSounds);
        dd_TouchSnd.value = touchSound;
        dd_TouchSnd.onValueChanged.AddListener(value => 
        {
            int selectedIdx = value - 1;
            if (!(selectedIdx < 0))
            {
                SoundManager.Instance.ExampleAudioSource.clip = SoundManager.Instance.touchSoundList[selectedIdx];
                SoundManager.Instance.ExampleAudioSource.volume = volLevel;
                StartCoroutine(ExampleSoundPlay(volLevel));
            }
            else
            {
                SoundManager.Instance.ExampleAudioSource.volume = 0;
            }
            
        });

        // �溸��
        dd_AlarmSnd.onValueChanged.RemoveAllListeners();
        dd_AlarmSnd.ClearOptions();
        List<string> alarmSounds = new List<string> { "����", "�溸��1", "�溸��2", "�溸��3", "���� �ȳ�(���ͳ� ���� �ʿ�)" };
        List<TMP_Dropdown.OptionData> options_alarmSounds = new List<TMP_Dropdown.OptionData>();
        foreach (string sound in alarmSounds)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(sound);
            options_alarmSounds.Add(option);
        }
        dd_AlarmSnd.AddOptions(options_alarmSounds);
        dd_AlarmSnd.value = alarmSound;
        dd_AlarmSnd.onValueChanged.AddListener(value =>
        {
            if(value == 4)
            {
                TTSManager.instance.RunTTS($"�溸 �׸� ���� ���� �ȳ��� �մϴ�.");
                return;
            }

            int selectedIdx = value - 1;
            if (!(selectedIdx < 0))
            {
                SoundManager.Instance.ExampleAudioSource.clip = SoundManager.Instance.alarmSoundList[selectedIdx];
                SoundManager.Instance.ExampleAudioSource.volume = volLevel;
                StartCoroutine(ExampleSoundPlay(volLevel));
            }
            else
            {
                SoundManager.Instance.ExampleAudioSource.volume = 0;
            }
        });


        // ���� ����
        switch (colorTheme)
        {
            case 1:
                tg_Green.isOn = true;
                break;
            case 2:
                tg_Blue.isOn = true;
                break;
            case 3:
                tg_Navy.isOn = true;
                break;
            case 4:
                tg_Red.isOn = true;
                break;
            case 5:
                tg_Black.isOn = true;
                break;
            case 6:
                tg_BusungBlue.isOn = true;
                break;
        }
        if(tg_Green.isOn)
            colorTheme = 1;
        if (tg_Blue.isOn)
            colorTheme = 2;
        if (tg_Navy.isOn)
            colorTheme = 3;
        if (tg_Red.isOn)
            colorTheme = 4;
        if (tg_Black.isOn)
            colorTheme = 5;
        if (tg_BusungBlue.isOn)
            colorTheme = 6;
        tg_Green.onValueChanged.AddListener((value) => {
            if (value)
                colorTheme = 1;
        });
        tg_Blue.onValueChanged.AddListener((value) => {
            if (value)
                colorTheme = 2;
        });
        tg_Navy.onValueChanged.AddListener((value) => {
            if (value)
                colorTheme = 3;
        });
        tg_Red.onValueChanged.AddListener((value) => {
            if (value)
                colorTheme = 4;
        });
        tg_Black.onValueChanged.AddListener((value) => {
            if (value)
                colorTheme = 5;
        });
        tg_BusungBlue.onValueChanged.AddListener((value) => {

            if (value)
                colorTheme = 6;
        });

        // ���� �ּ� �Է�
        if_FieldAddr.text = fieldAddr;

        // ���� ���
        if (weatherUse == 1)
        {
            weatherUse = 1;
            objWeatherUse_Selected.SetActive(true);
            objWeatherUse_NotSelected.SetActive(false);
            objWeatherNotUse_Selected.SetActive(false);
            objWeatherNotUse_NotSelected.SetActive(true);
        }
        else
        {
            weatherUse = 0;
            objWeatherUse_Selected.SetActive(false);
            objWeatherUse_NotSelected.SetActive(true);
            objWeatherNotUse_Selected.SetActive(true);
            objWeatherNotUse_NotSelected.SetActive(false);
        }
        btnWeatherUse.onClick.RemoveAllListeners();
        btnWeatherNotUse.onClick.RemoveAllListeners();
        btnWeatherUse.onClick.AddListener(() =>
        {
            weatherUse = 1;
            objWeatherUse_Selected.SetActive(true);
            objWeatherUse_NotSelected.SetActive(false);
            objWeatherNotUse_Selected.SetActive(false);
            objWeatherNotUse_NotSelected.SetActive(true);
        });
        btnWeatherNotUse.onClick.AddListener(() =>
        {
            weatherUse = 0;
            objWeatherUse_Selected.SetActive(false);
            objWeatherUse_NotSelected.SetActive(true);
            objWeatherNotUse_Selected.SetActive(true);
            objWeatherNotUse_NotSelected.SetActive(false);
        });

        // �ð� ���
        if (timeUse == 1)
        {
            timeUse = 1;
            objTimeUse_Selected.SetActive(true);
            objTimeUse_NotSelected.SetActive(false);
            objTimeNotUse_Selected.SetActive(false);
            objTimeNotUse_NotSelected.SetActive(true);
        }
        else
        {
            timeUse = 0;
            objTimeUse_Selected.SetActive(false);
            objTimeUse_NotSelected.SetActive(true);
            objTimeNotUse_Selected.SetActive(true);
            objTimeNotUse_NotSelected.SetActive(false);
        }
        btnTimeUse.onClick.RemoveAllListeners();
        btnTimeNotUse.onClick.RemoveAllListeners();
        btnTimeUse.onClick.AddListener(() =>
        {
            timeUse = 1;
            objTimeUse_Selected.SetActive(true);
            objTimeUse_NotSelected.SetActive(false);
            objTimeNotUse_Selected.SetActive(false);
            objTimeNotUse_NotSelected.SetActive(true);
        });
        btnTimeNotUse.onClick.AddListener(() =>
        {
            timeUse = 0;
            objTimeUse_Selected.SetActive(false);
            objTimeUse_NotSelected.SetActive(true);
            objTimeNotUse_Selected.SetActive(true);
            objTimeNotUse_NotSelected.SetActive(false);
        });

        // ȭ�� ��� ���
        if (lockUse == "true")
        {
            lockUse = "true";
            objLockUse_Selected.SetActive(true);
            objLockUse_NotSelected.SetActive(false);
            objLockNotUse_Selected.SetActive(false);
            objLockNotUse_NotSelected.SetActive(true);
        }
        else
        {
            lockUse = "false";
            objLockUse_Selected.SetActive(false);
            objLockUse_NotSelected.SetActive(true);
            objLockNotUse_Selected.SetActive(true);
            objLockNotUse_NotSelected.SetActive(false);
        }
        btnLockUse.onClick.RemoveAllListeners();
        btnLockNotUse.onClick.RemoveAllListeners();
        btnLockUse.onClick.AddListener(() =>
        {
            // ��й�ȣ�� ���������� ���� ���(�ʱ����)
            if(passwd == "0000")
            {
                screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                screenManager.txt_PopUpMsg.text = "�ʱ� ��й�ȣ�� '0000' �Դϴ�.";
                screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
                screenManager.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    screenManager.ClosePopUpMessage();
                });
            }

            lockUse = "true";
            objLockUse_Selected.SetActive(true);
            objLockUse_NotSelected.SetActive(false);
            objLockNotUse_Selected.SetActive(false);
            objLockNotUse_NotSelected.SetActive(true);
        });
        btnLockNotUse.onClick.AddListener(() =>
        {
            lockUse = "false";
            objLockUse_Selected.SetActive(false);
            objLockUse_NotSelected.SetActive(true);
            objLockNotUse_Selected.SetActive(true);
            objLockNotUse_NotSelected.SetActive(false);
        });

        // ��й�ȣ ����
        Button btnSetPasswd = item_ChangePW.transform.Find("btnChangePW").GetComponent<Button>();
        btnSetPasswd.onClick.RemoveAllListeners();
        btnSetPasswd.onClick.AddListener(() =>
        {
            ScreenLockManager.Instance.isSetPWMode = true;
            ScreenLockManager.Instance.isUnlockMode = false;            
            settingPasswd.SetActive(true);
            SetScreenPW.Instance.InitPassword();
        });

        btnSaveSystemSet = screenSystemSet.transform.Find("Bottom/btnSave").GetComponent<Button>();
        btnSaveSystemSet.onClick.RemoveAllListeners();
        btnSaveSystemSet.onClick.AddListener(() =>
        {
            SaveSystemSettings(volLevel, dd_TouchSnd.value, dd_AlarmSnd.value, colorTheme, if_FieldAddr.text, weatherUse, timeUse, lockUse, passwd, lockState);
        });
    }

    // �ý��� ���� ����
    public void SaveSystemSettings(int p_volLevel, int p_touchSound, int p_alarmSound, int p_colorTheme, string p_fieldAddr, int p_weatherUse, int p_timeUse, string p_lockUse, string p_passwd, string p_lockState)
    {
        string systemSetJson = $"{{\"volLevel\":{p_volLevel},\"touchSound\":{p_touchSound},\"alarmSound\":{p_alarmSound},\"colorTheme\":{p_colorTheme},\"fieldAddr\":\"{p_fieldAddr}\",\"weatherUse\":{p_weatherUse},\"timeUse\":{p_timeUse}}}";
        string query = $"UPDATE TBL_CONFIG SET SYSTEM_SET = '{systemSetJson}'";
        if (ClientDatabase.OnUpdateRequest(query))
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "������ ����Ǿ����ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() =>
            {
                screenManager.ClosePopUpMessage();
            });
            volLevel = p_volLevel;
            touchSound = p_touchSound;
            alarmSound = p_alarmSound;
            colorTheme = p_colorTheme;
            fieldAddr = p_fieldAddr;
            weatherUse = p_weatherUse;
            timeUse = p_timeUse;

            SoundManager.Instance.SaveSoundsData(volLevel, touchSound, alarmSound); // ����, ��ġ��, �溸�� ����
            GeoCountry.Instance.SaveWeatherUseData(weatherUse); // ���� ����
            ColorThemeManager.Instance.ApplyColor(colorTheme);
            ConfigManager.Instance.SetSetting("LOCK_USE", p_lockUse); // ȭ�� ��� ����
            ScreenLockManager.Instance.InitLock();
        }
        else
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "���� ���忡 �����߽��ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() =>
            {
                screenManager.ClosePopUpMessage();
            });
        }
    }
    
    // �ý��� ���� ��ȯ
    public void ChangeSystemSetting()
    {
        HideUI();
        objNormalNotSelected.SetActive(true);
        objSystemSelected.SetActive(true);
        objServerNotSelected.SetActive(true);
        objUpdateNotSelected.SetActive(true);
        objManualNotSelected.SetActive(true);
        objInfoNotSelected.SetActive(true);
        screenSystemSet.SetActive(true);

        DataTable tblConfig = ClientDatabase.FetchConfigData().Tables[0];
        foreach (DataRow row in tblConfig.Rows)
        {
            nox = row["NOX"].ToString(); // ����
            whoami = row["WHOAMI"].ToString(); // ��� ���� Termux ���� ID
            eth0 = row["ETH0"].ToString(); // ������ IP
            eth0_Mac = row["ETH0_MAC"].ToString(); // ������ �ƾ�巹��
            wlan0 = row["WLAN0"].ToString(); // ������ IP
            wlan0_Mac = row["WLAN0_MAC"].ToString(); // ������ �ƾ�巹��
            workEta = row["WORK_ETA"].ToString(); // �۵� ���� �Ⱓ(�Ⱓ �� ���� �ϼ� ����Ͽ� ����)
            devReg = row["DEV_REG"].ToString(); // Ŭ���� ����� ��¥
            cloudId = row["CLOUD_ID"].ToString(); // Ŭ���� ����� ID ����
            vendorID = row["VENDOR_ID"].ToString(); // ���ó �Ǵ� ��, �븮�� ID
            vendorName = row["VENDOR_NAME"].ToString(); // ���ó �Ǵ� ��, �븮�� �̸�
            workDays = row["WORK_DAYS"].ToString(); // �۵� ���� �ϼ�(�¶��� ��� �� �ڵ� ���̳ʽ� ���� ó��)
            selUi = row["SEL_UI"].ToString(); // ���õ� �ý��� UI
            darkMode = row["DARKMODE"].ToString(); // ��ũ��� ���
            language = row["LANGUAGE"].ToString(); // ��� ȯ�� ���
            demoMode = row["DEMO_MODE"].ToString(); // ������
            normalSet = row["NORMAL_SET"].ToString(); // �Ϲݼ��� Json
            systemSet = row["SYSTEM_SET"].ToString(); // �ý��ۼ��� Json
        }

        SystemSettings systemSetJson = JsonUtility.FromJson<SystemSettings>(systemSet);
        volLevel = int.Parse(systemSetJson.volLevel);
        touchSound = int.Parse(systemSetJson.touchSound);
        alarmSound = int.Parse(systemSetJson.alarmSound);
        colorTheme = int.Parse(systemSetJson.colorTheme);
        fieldAddr = systemSetJson.fieldAddr;
        weatherUse = int.Parse(systemSetJson.weatherUse);
        timeUse = int.Parse(systemSetJson.timeUse);

        lockUse = ConfigManager.Instance.GetSetting("LOCK_USE");
        lockState = ConfigManager.Instance.GetSetting("LOCK_STATE");
        passwd = ConfigManager.Instance.GetSetting("LOCK_PW");

        
        LoadSystemSettings(volLevel, touchSound, alarmSound, colorTheme, fieldAddr, weatherUse, timeUse, lockUse, passwd, lockState);
    }
    #endregion

    #region ������Ʈ ����
    // ������Ʈ ���� ��ȯ
    public void ChangeUpdateSetting()
    {
        HideUI();
        objNormalNotSelected.SetActive(true);
        objSystemNotSelected.SetActive(true);
        objServerNotSelected.SetActive(true);
        objUpdateSelected.SetActive(true);
        objManualNotSelected.SetActive(true);
        objInfoNotSelected.SetActive(true);
        screenUpdateSet.SetActive(true);

        CheckOTA.Instance.LoadUpdateInfo(false);
    }
    #endregion

    #region �Ŵ���    
    public void ChangeManualSetting()
    {
        HideUI();
        objNormalNotSelected.SetActive(true);
        objSystemNotSelected.SetActive(true);
        objServerNotSelected.SetActive(true);
        objUpdateNotSelected.SetActive(true);
        objManualSelected.SetActive(true);
        objInfoNotSelected.SetActive(true);
        screenManual.SetActive(true);

        StartWebView();
    }

    public void StartWebView()
    {
        if(pageInstances.Count == 0)
        {
            for (int i = 0; i < listManualPages.Count; i++)
            {
                GameObject instance = Instantiate(pagePrefab, manualScrollViewContent);
                instance.GetComponent<Image>().sprite = listManualPages[i];
                instance.name = $"Page{i}";
                pageInstances[$"Page{i}"] = instance;
            }
        }
        

        //string strUrl = "http://cloud.systronics.co.kr/manual/%E1%84%91%E1%85%AE%E1%86%AF%E1%84%86%E1%85%AE%E1%84%8B%E1%85%AF%E1%86%AB_%E1%84%8F%E1%85%B3%E1%86%AF%E1%84%85%E1%85%A1%E1%84%8B%E1%85%AE%E1%84%83%E1%85%B3W%E1%84%86%E1%85%A6%E1%84%82%E1%85%B2%E1%84%8B%E1%85%A5%E1%86%AFV1.0.0-01.png";

        //// screenManual�� Ȱ��ȭ�Ǿ� �ְ� RectTransform�� �ִ��� Ȯ��
        //if (screenManual.activeInHierarchy && screenManual.GetComponent<RectTransform>() != null)
        //{
        //    RectTransform rt = screenManual.GetComponent<RectTransform>();

        //    // RectTransform�� �𼭸��� ȭ�� ��ǥ�� ��ȯ
        //    Vector2 screenPoint = Vector2.zero;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, rt.rect.min, null, out screenPoint);
        //    int left = (int)screenPoint.x;
        //    int top = (int)screenPoint.y;

        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, rt.rect.max, null, out screenPoint);
        //    int right = (int)(Screen.width - screenPoint.x);
        //    int bottom = (int)(Screen.height - screenPoint.y);

        //    if (webViewObject == null)
        //    {
        //        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        //        webViewObject.Init((msg) =>
        //        {
        //            Debug.Log(string.Format("CallFromJS[{0}]", msg));
        //        });

        //        webViewObject.LoadURL(strUrl);
        //        webViewObject.SetVisibility(true);
        //        webViewObject.SetMargins(left, top, right, bottom);
        //    }
        //    else
        //    {
        //        webViewObject.SetVisibility(true);
        //        webViewObject.SetMargins(left, top, right, bottom);
        //    }
        //}
        //else
        //{
        //    Debug.LogError("screenManual�� ��Ȱ��ȭ�Ǿ� �ְų� RectTransform ������Ʈ�� �����ϴ�!");
        //}
    }

    #endregion

    #region �ý��� ����


    // �ý��� ���� ��ȯ
    public void ChangeInfoSetting()
    {
        HideUI();
        objNormalNotSelected.SetActive(true);
        objSystemNotSelected.SetActive(true);
        objServerNotSelected.SetActive(true);
        objUpdateNotSelected.SetActive(true);
        objManualNotSelected.SetActive(true);
        objInfoSelected.SetActive(true);
        screenInfoSet.SetActive(true);

        InformationManager.Instance.LoadSystemInfo();
    }
    #endregion
    IEnumerator ExampleSoundPlay(int volLevel)
    {
        SoundManager.Instance.touchSndAudioSource.volume = 0;
        SoundManager.Instance.ExampleAudioSource.Play();
        yield return new WaitForSeconds(SoundManager.Instance.ExampleAudioSource.clip.length + 0.8f);
        SoundManager.Instance.touchSndAudioSource.volume = volLevel;
    }

    // ��ư Ŭ���� ���� UI �ʱ�ȭ
    public void HideUI()
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].SetActive(false);
        }
        if(webViewObject != null)
            webViewObject.SetVisibility(false);
    }
}
