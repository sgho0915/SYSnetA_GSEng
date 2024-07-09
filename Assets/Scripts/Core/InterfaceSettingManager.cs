using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEngine.Rendering;
using System.Xml.Linq;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

// JSON �����͸� ���� Ŭ���� ����
[Serializable]
public class InterfaceConfig
{
    public string comport;
    public string baudrate;
    public string stopbits;
    public string bytesize;
    public string parity;
    public string DTR;
    public string RTS;
}

public class InterfaceSettingManager : MonoBehaviour
{
    ScreenManager screenManager;

    // ���� ù ���� �� ���
    private Dictionary<string, GameObject> f_uiElements = new Dictionary<string, GameObject>();

    [Header("ù ���� �� �������̽� �������")]
    public GameObject f_settingInterface;
    public GameObject f_intefaceListScrollView; // �������̽� ����Ʈ ��ũ�Ѻ�
    public static Transform f_interfaceListContent; // �������̽� ����Ʈ ��ũ�Ѻ� Content
    private Button f_btnClose;
    private Button f_btnAddInterface;
    private Button f_btnSave;

    [Header("ù ���� �� �������̽� �߰�")]
    public GameObject f_settingAddInterface; // �������̽� �߰�ȭ��
    public Button f_btnCloseAddInterface; // �������̽� �߰�ȭ�� �ݱ� ��ư
    public Button f_btnNext;
    public Button f_btnSkip;
    public ToggleGroup f_interfaceToggleGroup;
    public Toggle f_toggleSerial;
    public Toggle f_toggleRtuTcp;
    private string f_selectedToggle;

    [Header("ù ���� �� �������̽� ����ȭ�� - SERIAL Ÿ��")]
    public GameObject f_settingSerial;
    public TMP_Dropdown f_dropdown_ComPort;
    public TMP_InputField f_inputfield_InterfaceName;
    public TMP_Dropdown f_dropdown_Baudrate;
    public TMP_Dropdown f_dropdown_StopBits;
    public TMP_Dropdown f_dropdown_ByteSize;
    public TMP_Dropdown f_dropdown_Parity;
    public Toggle f_toggle_DTR;
    public Toggle f_toggle_RTS;
    public Button f_btnCloseSerialSet;
    public Button f_btnSaveSerialSet;

    [Header("ù ���� �� �������̽� ����ȭ�� - RTU TCP Ÿ��")]
    public GameObject f_settingRTUTCP;
    public TMP_InputField f_inputfield_IP;
    public TMP_InputField f_inputfield_IName;
    public Button f_btnCloseRTUTCPSet;
    public Button f_btnSaveRTUTCPSet;

    // ���� ù ���� �Ϸ� �� ���
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    [Header("�������̽� �������")]
    public GameObject settingInterface;
    public GameObject intefaceListScrollView; // �������̽� ����Ʈ ��ũ�Ѻ�
    public static Transform interfaceListContent; // �������̽� ����Ʈ ��ũ�Ѻ� Content
    private Button btnClose;
    private Button btnAddInterface;
    private Button btnSave;

    [Header("�������̽� �߰�")]
    public GameObject settingAddInterface; // �������̽� �߰�ȭ��
    public Button btnCloseAddInterface; // �������̽� �߰�ȭ�� �ݱ� ��ư
    public Button btnNext;
    public Button btnSkip;
    public ToggleGroup interfaceToggleGroup;
    public Toggle toggleSerial;
    public Toggle toggleRtuTcp;
    private string selectedToggle;

    [Header("�������̽� ����ȭ�� - SERIAL Ÿ��")]
    public GameObject settingSerial;
    public TMP_Dropdown dropdown_ComPort;
    public TMP_InputField inputfield_InterfaceName;
    public TMP_Dropdown dropdown_Baudrate;
    public TMP_Dropdown dropdown_StopBits;
    public TMP_Dropdown dropdown_ByteSize;
    public TMP_Dropdown dropdown_Parity;
    public Toggle toggle_DTR;
    public Toggle toggle_RTS;
    public Button btnCloseSerialSet;
    public Button btnSaveSerialSet;

    [Header("�������̽� ����ȭ�� - RTU TCP Ÿ��")]
    public GameObject settingRTUTCP;
    public TMP_InputField inputfield_IP;
    public TMP_InputField inputfield_IName;
    public Button btnCloseRTUTCPSet;
    public Button btnSaveRTUTCPSet;

    public static InterfaceSettingManager Instance { get; private set; }
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

        // ù ���� ��
        f_interfaceListContent = f_intefaceListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        // �������̽� ����
        f_btnClose = f_settingInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        f_btnClose.onClick.AddListener(() => CloseInterfaceSetting());
        f_btnSave = f_settingInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Save").GetComponent<Button>();
        f_btnSave.onClick.AddListener(() => {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "�������̽� ������ �Ϸ� �Ǿ����ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => 
            { 
                screenManager.ClosePopUpMessage();
                f_settingInterface.SetActive(false);
                FirstStartManager.Instance.firstSet_Interface.SetActive(false);
                FirstStartManager.Instance.StartControllerSettings();
            });
        });
        f_btnAddInterface = f_settingInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/AddInterface/btn_AddInterface").GetComponent<Button>();
        f_btnAddInterface.onClick.AddListener(() => { OpenAddInterface(); });

        // �������̽� �߰�
        f_toggleSerial = f_settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/InterfaceListScrollView/Viewport/Content/Toggle_Serial").GetComponent<Toggle>();
        f_toggleRtuTcp = f_settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/InterfaceListScrollView/Viewport/Content/Toggle_RTUTCP").GetComponent<Toggle>();
        f_btnCloseAddInterface = f_settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        f_btnNext = f_settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Next").GetComponent<Button>();
        f_btnSkip = f_settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Skip").GetComponent<Button>();
        f_btnCloseAddInterface.onClick.AddListener(() => { CloseAddInterface(); });
        f_toggleSerial.onValueChanged.AddListener(delegate {
            OnToggleChanged(f_toggleSerial);
        });
        f_toggleRtuTcp.onValueChanged.AddListener(delegate {
            OnToggleChanged(f_toggleRtuTcp);
        });
        f_btnNext.onClick.AddListener(() =>
        {
            if (f_selectedToggle == "Toggle_Serial")
            {
                //Debug.Log("OpenSettingSerial");
                OpenSettingSerial();
            }
            else
            {
                //Debug.Log("OpenSettingRTUTCP");
                OpenSettingRTUTCP();
            }
        });

        // �������̽� �ø��� Ÿ��
        f_dropdown_ComPort = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_Com/Dropdown").GetComponent<TMP_Dropdown>();
        f_inputfield_InterfaceName = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_InterfaceName/InputField (TMP)").GetComponent<TMP_InputField>();
        f_dropdown_Baudrate = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_Baudrate/Dropdown").GetComponent<TMP_Dropdown>();
        f_dropdown_StopBits = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_StopBits/Dropdown").GetComponent<TMP_Dropdown>();
        f_dropdown_ByteSize = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_ByteSize/Dropdown").GetComponent<TMP_Dropdown>();
        f_dropdown_Parity = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_Parity/Dropdown").GetComponent<TMP_Dropdown>();
        f_toggle_DTR = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_DTRRTS/ToggleDTR").GetComponent<Toggle>();
        f_toggle_RTS = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_DTRRTS/ToggleRTS").GetComponent<Toggle>();
        f_btnCloseSerialSet = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        f_btnCloseSerialSet.onClick.AddListener(() =>
        {
            f_dropdown_ComPort.ClearOptions();
            f_dropdown_Baudrate.ClearOptions();
            f_dropdown_StopBits.ClearOptions();
            f_dropdown_ByteSize.ClearOptions();
            f_dropdown_Parity.ClearOptions();
            f_inputfield_InterfaceName.text = string.Empty;
            f_toggle_DTR.isOn = false;
            f_toggle_RTS.isOn = false;
            f_settingSerial.SetActive(false);
        });
        f_btnSaveSerialSet = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Save").GetComponent<Button>();

        // �������̽� RTU TCP Ÿ��
        f_inputfield_IP = f_settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_IP/InputField (TMP)").GetComponent<TMP_InputField>();
        f_inputfield_IName = f_settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_InterfaceName/InputField (TMP)").GetComponent<TMP_InputField>();
        f_btnCloseRTUTCPSet = f_settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        f_btnCloseRTUTCPSet.onClick.AddListener(() =>
        {
            f_inputfield_IP.text = string.Empty;
            f_inputfield_InterfaceName.text = string.Empty;
            TextMeshProUGUI txtiName = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = "Modbus RTU over TCP/IP Client";
            f_settingRTUTCP.SetActive(false);
        });
        f_btnSaveRTUTCPSet = f_settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Save").GetComponent<Button>();






        // ù ���� ����
        interfaceListContent = intefaceListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        // �������̽� ����
        btnClose = settingInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(() => CloseInterfaceSetting());
        btnSave = settingInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Save").GetComponent<Button>();
        btnSave.onClick.AddListener(() => {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "�������̽� ������ �Ϸ� �Ǿ����ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => { screenManager.ClosePopUpMessage(); settingInterface.SetActive(false); });
        });
        btnAddInterface = settingInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/AddInterface/btn_AddInterface").GetComponent<Button>();
        btnAddInterface.onClick.AddListener(() => { OpenAddInterface(); });

        // �������̽� �߰�
        toggleSerial = settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/InterfaceListScrollView/Viewport/Content/Toggle_Serial").GetComponent<Toggle>();
        toggleRtuTcp = settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/InterfaceListScrollView/Viewport/Content/Toggle_RTUTCP").GetComponent<Toggle>();
        btnCloseAddInterface = settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        btnNext = settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Next").GetComponent<Button>();
        btnSkip = settingAddInterface.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Skip").GetComponent<Button>();
        btnCloseAddInterface.onClick.AddListener(() => { CloseAddInterface(); });
        toggleSerial.onValueChanged.AddListener(delegate {
            OnToggleChanged(toggleSerial);
        });
        toggleRtuTcp.onValueChanged.AddListener(delegate {
            OnToggleChanged(toggleRtuTcp);
        });
        btnNext.onClick.AddListener(() => 
        {
            if (selectedToggle == "Toggle_Serial")
            {
                //Debug.Log("OpenSettingSerial");
                OpenSettingSerial();
            }
            else
            {
                //Debug.Log("OpenSettingRTUTCP");
                OpenSettingRTUTCP();
            }
        });

        // �������̽� �ø��� Ÿ��
        dropdown_ComPort = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_Com/Dropdown").GetComponent<TMP_Dropdown>();
        inputfield_InterfaceName = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_InterfaceName/InputField (TMP)").GetComponent<TMP_InputField>();
        dropdown_Baudrate = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_Baudrate/Dropdown").GetComponent<TMP_Dropdown>();
        dropdown_StopBits = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_StopBits/Dropdown").GetComponent<TMP_Dropdown>();
        dropdown_ByteSize = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_ByteSize/Dropdown").GetComponent<TMP_Dropdown>();
        dropdown_Parity = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_Parity/Dropdown").GetComponent<TMP_Dropdown>();
        toggle_DTR = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_DTRRTS/ToggleDTR").GetComponent<Toggle>();
        toggle_RTS = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_DTRRTS/ToggleRTS").GetComponent<Toggle>();
        btnCloseSerialSet = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        btnCloseSerialSet.onClick.AddListener(() =>
        {
            dropdown_ComPort.ClearOptions();
            dropdown_Baudrate.ClearOptions();
            dropdown_StopBits.ClearOptions();
            dropdown_ByteSize.ClearOptions();
            dropdown_Parity.ClearOptions();
            inputfield_InterfaceName.text = string.Empty;
            toggle_DTR.isOn = false;
            toggle_RTS.isOn = false;
            settingSerial.SetActive(false);
        });
        btnSaveSerialSet = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Save").GetComponent<Button>();

        // �������̽� RTU TCP Ÿ��
        inputfield_IP = settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_IP/InputField (TMP)").GetComponent<TMP_InputField>();
        inputfield_IName = settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/InterfaceList/Element_InterfaceName/InputField (TMP)").GetComponent<TMP_InputField>();
        btnCloseRTUTCPSet = settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/btn_Close").GetComponent<Button>();
        btnCloseRTUTCPSet.onClick.AddListener(() =>
        {
            inputfield_IP.text = string.Empty;
            inputfield_InterfaceName.text = string.Empty;
            TextMeshProUGUI txtiName = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = "Modbus RTU over TCP/IP Client";
            settingRTUTCP.SetActive(false);
        });
        btnSaveRTUTCPSet = settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Bottom/btn_Save").GetComponent<Button>();
    }

    // �������̽� ���� RTU over TCP Ÿ�� �߰�
    public void OpenSettingRTUTCP()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_settingRTUTCP.SetActive(true);

            TextMeshProUGUI txtiName = f_settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = "Modbus RTU over TCP/IP Client";

            f_inputfield_IP.text = string.Empty;
            f_inputfield_IName.text = string.Empty;

            f_btnSaveRTUTCPSet.onClick.RemoveAllListeners();
            f_btnSaveRTUTCPSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string ipAddress = f_inputfield_IP.text;
                string interfaceName = f_inputfield_IName.text;

                string tblInterfaceQuery = $"INSERT INTO TBL_INTERFACE (ITYPE, IID, INAME) VALUES ('RTU_TCP_CLIENT', '{ipAddress}', '{interfaceName}')";

                if (ClientDatabase.OnInsertRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    f_settingRTUTCP.SetActive(false);
                    f_settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    f_inputfield_IP.text = string.Empty;
                    f_inputfield_InterfaceName.text = string.Empty;
                    txtiName.text = "Modbus RTU over TCP/IP Client";
                    //Debug.Log($"�������̽� �߰� �Ϸ� : {ipAddress} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� �߰� ���� : {ipAddress} {interfaceName}");
                }
            });
        }
        else
        {
            settingRTUTCP.SetActive(true);

            TextMeshProUGUI txtiName = settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = "Modbus RTU over TCP/IP Client";

            inputfield_IP.text = string.Empty;
            inputfield_IName.text = string.Empty;

            btnSaveRTUTCPSet.onClick.RemoveAllListeners();
            btnSaveRTUTCPSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string ipAddress = inputfield_IP.text;
                string interfaceName = inputfield_IName.text;

                string tblInterfaceQuery = $"INSERT INTO TBL_INTERFACE (ITYPE, IID, INAME) VALUES ('RTU_TCP_CLIENT', '{ipAddress}', '{interfaceName}')";

                if (ClientDatabase.OnInsertRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    settingRTUTCP.SetActive(false);
                    settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    inputfield_IP.text = string.Empty;
                    inputfield_InterfaceName.text = string.Empty;
                    txtiName.text = "Modbus RTU over TCP/IP Client";
                    //Debug.Log($"�������̽� �߰� �Ϸ� : {ipAddress} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� �߰� ���� : {ipAddress} {interfaceName}");
                }
            });
        }        
    }

    // �������̽� ���� RTU over TCP Ÿ�� ����
    public void OpenModifySettingRTUTCP(string id, string iid, string itype, string iname, string iconfig)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_settingRTUTCP.SetActive(true);

            TextMeshProUGUI txtiName = f_settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = $"{iname}";

            // �ʱ�ȭ
            f_inputfield_IP.text = string.Empty;
            f_inputfield_IName.text = string.Empty;

            // �� �Ҵ�
            f_inputfield_IP.text = iid;
            f_inputfield_IName.text = iname;


            f_btnSaveRTUTCPSet.onClick.RemoveAllListeners();
            f_btnSaveRTUTCPSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string ipAddress = f_inputfield_IP.text;
                string interfaceName = f_inputfield_IName.text;

                string tblInterfaceQuery = $"UPDATE TBL_INTERFACE SET IID = '{ipAddress}', INAME = '{interfaceName}' WHERE ID = '{id}'";

                if (ClientDatabase.OnUpdateRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    f_settingRTUTCP.SetActive(false);
                    f_settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    f_inputfield_IP.text = string.Empty;
                    f_inputfield_IName.text = string.Empty;
                    txtiName.text = "Modbus RTU over TCP/IP Client";
                    //Debug.Log($"�������̽� �߰� �Ϸ� : {ipAddress} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� �߰� ���� : {ipAddress} {interfaceName}");
                }
            });
        }
        else
        {
            settingRTUTCP.SetActive(true);

            TextMeshProUGUI txtiName = settingRTUTCP.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = $"{iname}";

            // �ʱ�ȭ
            inputfield_IP.text = string.Empty;
            inputfield_IName.text = string.Empty;

            // �� �Ҵ�
            inputfield_IP.text = iid;
            inputfield_IName.text = iname;


            btnSaveRTUTCPSet.onClick.RemoveAllListeners();
            btnSaveRTUTCPSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string ipAddress = inputfield_IP.text;
                string interfaceName = inputfield_IName.text;

                string tblInterfaceQuery = $"UPDATE TBL_INTERFACE SET IID = '{ipAddress}', INAME = '{interfaceName}' WHERE ID = '{id}'";

                if (ClientDatabase.OnUpdateRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    settingRTUTCP.SetActive(false);
                    settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    inputfield_IP.text = string.Empty;
                    inputfield_IName.text = string.Empty;
                    txtiName.text = "Modbus RTU over TCP/IP Client";
                    //Debug.Log($"�������̽� �߰� �Ϸ� : {ipAddress} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� �߰� ���� : {ipAddress} {interfaceName}");
                }
            });
        }        
    }

    // �������̽� ���� Serial Ÿ�� �߰�
    public void OpenSettingSerial()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableSerialList = ClientDatabase.FetchSerialList().Tables[0];
            f_settingSerial.SetActive(true);

            TextMeshProUGUI txtiName = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = "SERIAL";

            // �ʱ�ȭ
            f_dropdown_ComPort.ClearOptions();
            f_dropdown_Baudrate.ClearOptions();
            f_dropdown_StopBits.ClearOptions();
            f_dropdown_ByteSize.ClearOptions();
            f_dropdown_Parity.ClearOptions();
            f_inputfield_InterfaceName.text = string.Empty;
            f_toggle_DTR.isOn = false;
            f_toggle_RTS.isOn = false;

            // �ɼ� �׸� ����
            List<int> baudRates = new List<int> { 9600, 19200, 38400, 57600, 115200 };
            List<int> byteSizes = new List<int> { 8, 7 };
            List<int> stopBits = new List<int> { 1, 2 };
            List<string> parities = new List<string> { "None", "Even", "Odd" };

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_COM = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_baudRates = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_byteSizes = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_stopBits = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_parities = new List<TMP_Dropdown.OptionData>();

            // Comport �ɼ� �߰�
            foreach (DataRow row in tableSerialList.Rows)
            {
                string comPortName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(comPortName);
                options_COM.Add(option);
            }
            // baudrate �ɼ� �߰�
            foreach (int baudRate in baudRates)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(baudRate.ToString());
                options_baudRates.Add(option);
            }
            // byteSizes �ɼ� �߰�
            foreach (int byteSize in byteSizes)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(byteSize.ToString());
                options_byteSizes.Add(option);
            }
            // stopBits �ɼ� �߰�
            foreach (int stopBit in stopBits)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(stopBit.ToString());
                options_stopBits.Add(option);
            }
            // parities �ɼ� �߰�
            foreach (string parity in parities)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(parity.ToString());
                options_parities.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            f_dropdown_ComPort.AddOptions(options_COM);
            f_dropdown_Baudrate.AddOptions(options_baudRates);
            f_dropdown_StopBits.AddOptions(options_stopBits);
            f_dropdown_ByteSize.AddOptions(options_byteSizes);
            f_dropdown_Parity.AddOptions(options_parities);

            // ù ��° �׸��� �⺻ �������� ����
            if (f_dropdown_ComPort.options.Count > 0)
                f_dropdown_ComPort.value = 0;
            if (f_dropdown_Baudrate.options.Count > 0)
                f_dropdown_Baudrate.value = 0;
            if (f_dropdown_StopBits.options.Count > 0)
                f_dropdown_StopBits.value = 0;
            if (f_dropdown_ByteSize.options.Count > 0)
                f_dropdown_ByteSize.value = 0;
            if (f_dropdown_Parity.options.Count > 0)
                f_dropdown_Parity.value = 0;

            f_btnSaveSerialSet.onClick.RemoveAllListeners();
            f_btnSaveSerialSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string selectedComPort = f_dropdown_ComPort.options[f_dropdown_ComPort.value].text;
                string interfaceName = f_inputfield_InterfaceName.text;

                // ICONFIG ���� ���ڿ� ����
                string selectedBaudRate = f_dropdown_Baudrate.options[f_dropdown_Baudrate.value].text;
                string selectedByteSize = f_dropdown_ByteSize.options[f_dropdown_ByteSize.value].text;
                string selectedStopBits = f_dropdown_StopBits.options[f_dropdown_StopBits.value].text;
                string selectedParity = f_dropdown_Parity.options[f_dropdown_Parity.value].text;
                switch (f_dropdown_Parity.options[f_dropdown_Parity.value].text)
                {
                    case "None":
                        selectedParity = "N";
                        break;
                    case "Even":
                        selectedParity = "E";
                        break;
                    case "Odd":
                        selectedParity = "O";
                        break;
                }
                string dtrState = f_toggle_DTR.isOn ? "1" : "0";
                string rtsState = f_toggle_RTS.isOn ? "1" : "0";

                string jsonString = $"{{\"comport\":\"{selectedComPort}\",\"baudrate\":\"{selectedBaudRate}\",\"stopbits\":{selectedStopBits},\"bytesize\":\"{selectedByteSize}\",\"parity\":\"{selectedParity}\",\"DTR\":\"{dtrState}\",\"RTS\":\"{rtsState}\"}}";
                string tblInterfaceQuery = $"INSERT INTO TBL_INTERFACE (ITYPE, IID, INAME, ICONFIG) VALUES ('SERIAL', '{selectedComPort}', '{interfaceName}', '{jsonString}')";

                if (ClientDatabase.OnInsertRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    f_settingSerial.SetActive(false);
                    f_settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    f_dropdown_ComPort.ClearOptions();
                    f_dropdown_Baudrate.ClearOptions();
                    f_dropdown_StopBits.ClearOptions();
                    f_dropdown_ByteSize.ClearOptions();
                    f_dropdown_Parity.ClearOptions();
                    f_inputfield_InterfaceName.text = string.Empty;
                    f_toggle_DTR.isOn = false;
                    f_toggle_RTS.isOn = false;
                    txtiName.text = "SERIAL";
                    //Debug.Log($"�������̽� �߰� �Ϸ� : {selectedComPort} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� �߰� ���� : {selectedComPort} {interfaceName}");
                }
            });
        }
        else
        {
            DataTable tableSerialList = ClientDatabase.FetchSerialList().Tables[0];
            settingSerial.SetActive(true);

            TextMeshProUGUI txtiName = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = "SERIAL";

            // �ʱ�ȭ
            dropdown_ComPort.ClearOptions();
            dropdown_Baudrate.ClearOptions();
            dropdown_StopBits.ClearOptions();
            dropdown_ByteSize.ClearOptions();
            dropdown_Parity.ClearOptions();
            inputfield_InterfaceName.text = string.Empty;
            toggle_DTR.isOn = false;
            toggle_RTS.isOn = false;

            // �ɼ� �׸� ����
            List<int> baudRates = new List<int> { 9600, 19200, 38400, 57600, 115200 };
            List<int> byteSizes = new List<int> { 8, 7 };
            List<int> stopBits = new List<int> { 1, 2 };
            List<string> parities = new List<string> { "None", "Even", "Odd" };

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_COM = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_baudRates = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_byteSizes = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_stopBits = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_parities = new List<TMP_Dropdown.OptionData>();

            // Comport �ɼ� �߰�
            foreach (DataRow row in tableSerialList.Rows)
            {
                string comPortName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(comPortName);
                options_COM.Add(option);
            }
            // baudrate �ɼ� �߰�
            foreach (int baudRate in baudRates)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(baudRate.ToString());
                options_baudRates.Add(option);
            }
            // byteSizes �ɼ� �߰�
            foreach (int byteSize in byteSizes)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(byteSize.ToString());
                options_byteSizes.Add(option);
            }
            // stopBits �ɼ� �߰�
            foreach (int stopBit in stopBits)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(stopBit.ToString());
                options_stopBits.Add(option);
            }
            // parities �ɼ� �߰�
            foreach (string parity in parities)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(parity.ToString());
                options_parities.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            dropdown_ComPort.AddOptions(options_COM);
            dropdown_Baudrate.AddOptions(options_baudRates);
            dropdown_StopBits.AddOptions(options_stopBits);
            dropdown_ByteSize.AddOptions(options_byteSizes);
            dropdown_Parity.AddOptions(options_parities);

            // ù ��° �׸��� �⺻ �������� ����
            if (dropdown_ComPort.options.Count > 0)
                dropdown_ComPort.value = 0;
            if (dropdown_Baudrate.options.Count > 0)
                dropdown_Baudrate.value = 0;
            if (dropdown_StopBits.options.Count > 0)
                dropdown_StopBits.value = 0;
            if (dropdown_ByteSize.options.Count > 0)
                dropdown_ByteSize.value = 0;
            if (dropdown_Parity.options.Count > 0)
                dropdown_Parity.value = 0;

            btnSaveSerialSet.onClick.RemoveAllListeners();
            btnSaveSerialSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string selectedComPort = dropdown_ComPort.options[dropdown_ComPort.value].text;
                string interfaceName = inputfield_InterfaceName.text;

                // ICONFIG ���� ���ڿ� ����
                string selectedBaudRate = dropdown_Baudrate.options[dropdown_Baudrate.value].text;
                string selectedByteSize = dropdown_ByteSize.options[dropdown_ByteSize.value].text;
                string selectedStopBits = dropdown_StopBits.options[dropdown_StopBits.value].text;
                string selectedParity = dropdown_Parity.options[dropdown_Parity.value].text;
                switch (dropdown_Parity.options[dropdown_Parity.value].text)
                {
                    case "None":
                        selectedParity = "N";
                        break;
                    case "Even":
                        selectedParity = "E";
                        break;
                    case "Odd":
                        selectedParity = "O";
                        break;
                }
                string dtrState = toggle_DTR.isOn ? "1" : "0";
                string rtsState = toggle_RTS.isOn ? "1" : "0";

                string jsonString = $"{{\"comport\":\"{selectedComPort}\",\"baudrate\":\"{selectedBaudRate}\",\"stopbits\":{selectedStopBits},\"bytesize\":\"{selectedByteSize}\",\"parity\":\"{selectedParity}\",\"DTR\":\"{dtrState}\",\"RTS\":\"{rtsState}\"}}";
                string tblInterfaceQuery = $"INSERT INTO TBL_INTERFACE (ITYPE, IID, INAME, ICONFIG) VALUES ('SERIAL', '{selectedComPort}', '{interfaceName}', '{jsonString}')";

                if (ClientDatabase.OnInsertRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    settingSerial.SetActive(false);
                    settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    dropdown_ComPort.ClearOptions();
                    dropdown_Baudrate.ClearOptions();
                    dropdown_StopBits.ClearOptions();
                    dropdown_ByteSize.ClearOptions();
                    dropdown_Parity.ClearOptions();
                    inputfield_InterfaceName.text = string.Empty;
                    toggle_DTR.isOn = false;
                    toggle_RTS.isOn = false;
                    txtiName.text = "SERIAL";
                    //Debug.Log($"�������̽� �߰� �Ϸ� : {selectedComPort} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� �߰� ���� : {selectedComPort} {interfaceName}");
                }
            });
        }        
    }

    // �������̽� ���� Serial Ÿ�� ����
    public void OpenModifySettingSerial(string id, string iid, string itype, string iname, string iconfig)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // JSON ������ �Ľ�
            InterfaceConfig config = JsonUtility.FromJson<InterfaceConfig>(iconfig);

            // serialList ��������
            DataTable tableSerialList = ClientDatabase.FetchSerialList().Tables[0];
            f_settingSerial.SetActive(true);

            TextMeshProUGUI txtiName = f_settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = iname;

            // �ʱ�ȭ
            f_dropdown_ComPort.ClearOptions();
            f_dropdown_Baudrate.ClearOptions();
            f_dropdown_StopBits.ClearOptions();
            f_dropdown_ByteSize.ClearOptions();
            f_dropdown_Parity.ClearOptions();
            f_inputfield_InterfaceName.text = string.Empty;
            f_toggle_DTR.isOn = false;
            f_toggle_RTS.isOn = false;

            // �� �Ҵ�
            f_inputfield_InterfaceName.text = iname;

            // ��Ӵٿ� �ɼ� �׸� ����
            List<int> baudRates = new List<int> { 9600, 19200, 38400, 57600, 115200 };
            List<int> byteSizes = new List<int> { 8, 7 };
            List<int> stopBits = new List<int> { 1, 2 };
            List<string> parities = new List<string> { "None", "Even", "Odd" };

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_COM = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_baudRates = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_byteSizes = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_stopBits = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_parities = new List<TMP_Dropdown.OptionData>();

            // Comport �ɼ� �߰�
            foreach (DataRow row in tableSerialList.Rows)
            {
                string comPortName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(comPortName);
                options_COM.Add(option);
            }
            // baudrate �ɼ� �߰�
            foreach (int baudRate in baudRates)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(baudRate.ToString());
                options_baudRates.Add(option);
            }
            // byteSizes �ɼ� �߰�
            foreach (int byteSize in byteSizes)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(byteSize.ToString());
                options_byteSizes.Add(option);
            }
            // stopBits �ɼ� �߰�
            foreach (int stopBit in stopBits)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(stopBit.ToString());
                options_stopBits.Add(option);
            }
            // parities �ɼ� �߰�
            foreach (string parity in parities)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(parity.ToString());
                options_parities.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            f_dropdown_ComPort.AddOptions(options_COM);
            f_dropdown_Baudrate.AddOptions(options_baudRates);
            f_dropdown_StopBits.AddOptions(options_stopBits);
            f_dropdown_ByteSize.AddOptions(options_byteSizes);
            f_dropdown_Parity.AddOptions(options_parities);

            // ��Ӵٿ� ���� ����
            f_dropdown_ComPort.value = f_dropdown_ComPort.options.FindIndex(option => option.text == config.comport);
            f_dropdown_Baudrate.value = f_dropdown_Baudrate.options.FindIndex(option => option.text == config.baudrate);
            f_dropdown_StopBits.value = f_dropdown_StopBits.options.FindIndex(option => option.text == config.stopbits);
            f_dropdown_ByteSize.value = f_dropdown_ByteSize.options.FindIndex(option => option.text == config.bytesize);
            f_dropdown_Parity.value = f_dropdown_Parity.options.FindIndex(option => option.text == config.parity);

            // ��� ����
            f_toggle_DTR.isOn = config.DTR == "1";
            f_toggle_RTS.isOn = config.RTS == "1";

            f_btnSaveSerialSet.onClick.RemoveAllListeners();
            f_btnSaveSerialSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string selectedComPort = f_dropdown_ComPort.options[f_dropdown_ComPort.value].text;
                string interfaceName = f_inputfield_InterfaceName.text;

                // ICONFIG ���� ���ڿ� ����
                string selectedBaudRate = f_dropdown_Baudrate.options[f_dropdown_Baudrate.value].text;
                string selectedByteSize = f_dropdown_ByteSize.options[f_dropdown_ByteSize.value].text;
                string selectedStopBits = f_dropdown_StopBits.options[f_dropdown_StopBits.value].text;
                string selectedParity = f_dropdown_Parity.options[f_dropdown_Parity.value].text;
                switch (f_dropdown_Parity.options[f_dropdown_Parity.value].text)
                {
                    case "None":
                        selectedParity = "N";
                        break;
                    case "Even":
                        selectedParity = "E";
                        break;
                    case "Odd":
                        selectedParity = "O";
                        break;
                }
                string dtrState = f_toggle_DTR.isOn ? "1" : "0";
                string rtsState = f_toggle_RTS.isOn ? "1" : "0";

                string jsonString = $"{{\"comport\":\"{selectedComPort}\",\"baudrate\":\"{selectedBaudRate}\",\"stopbits\":{selectedStopBits},\"bytesize\":\"{selectedByteSize}\",\"parity\":\"{selectedParity}\",\"DTR\":\"{dtrState}\",\"RTS\":\"{rtsState}\"}}";
                string tblInterfaceQuery = $"UPDATE TBL_INTERFACE SET IID = '{selectedComPort}', INAME = '{interfaceName}', ICONFIG = '{jsonString}' WHERE ID = '{id}'";


                if (ClientDatabase.OnUpdateRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    f_settingSerial.SetActive(false);
                    f_settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    f_dropdown_ComPort.ClearOptions();
                    f_dropdown_Baudrate.ClearOptions();
                    f_dropdown_StopBits.ClearOptions();
                    f_dropdown_ByteSize.ClearOptions();
                    f_dropdown_Parity.ClearOptions();
                    f_inputfield_InterfaceName.text = string.Empty;
                    f_toggle_DTR.isOn = false;
                    f_toggle_RTS.isOn = false;
                    txtiName.text = "SERIAL";
                    //Debug.Log($"�������̽� ���� �Ϸ� : {selectedComPort} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� ���� ���� : {selectedComPort} {interfaceName}");
                }
            });
        }
        else
        {
            // JSON ������ �Ľ�
            InterfaceConfig config = JsonUtility.FromJson<InterfaceConfig>(iconfig);

            // serialList ��������
            DataTable tableSerialList = ClientDatabase.FetchSerialList().Tables[0];
            settingSerial.SetActive(true);

            TextMeshProUGUI txtiName = settingSerial.transform.Find("SettingInterfaceParent/obj_Setting_Interface/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
            txtiName.text = iname;

            // �ʱ�ȭ
            dropdown_ComPort.ClearOptions();
            dropdown_Baudrate.ClearOptions();
            dropdown_StopBits.ClearOptions();
            dropdown_ByteSize.ClearOptions();
            dropdown_Parity.ClearOptions();
            inputfield_InterfaceName.text = string.Empty;
            toggle_DTR.isOn = false;
            toggle_RTS.isOn = false;

            // �� �Ҵ�
            inputfield_InterfaceName.text = iname;

            // ��Ӵٿ� �ɼ� �׸� ����
            List<int> baudRates = new List<int> { 9600, 19200, 38400, 57600, 115200 };
            List<int> byteSizes = new List<int> { 8, 7 };
            List<int> stopBits = new List<int> { 1, 2 };
            List<string> parities = new List<string> { "None", "Even", "Odd" };

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_COM = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_baudRates = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_byteSizes = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_stopBits = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_parities = new List<TMP_Dropdown.OptionData>();

            // Comport �ɼ� �߰�
            foreach (DataRow row in tableSerialList.Rows)
            {
                string comPortName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(comPortName);
                options_COM.Add(option);
            }
            // baudrate �ɼ� �߰�
            foreach (int baudRate in baudRates)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(baudRate.ToString());
                options_baudRates.Add(option);
            }
            // byteSizes �ɼ� �߰�
            foreach (int byteSize in byteSizes)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(byteSize.ToString());
                options_byteSizes.Add(option);
            }
            // stopBits �ɼ� �߰�
            foreach (int stopBit in stopBits)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(stopBit.ToString());
                options_stopBits.Add(option);
            }
            // parities �ɼ� �߰�
            foreach (string parity in parities)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(parity.ToString());
                options_parities.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            dropdown_ComPort.AddOptions(options_COM);
            dropdown_Baudrate.AddOptions(options_baudRates);
            dropdown_StopBits.AddOptions(options_stopBits);
            dropdown_ByteSize.AddOptions(options_byteSizes);
            dropdown_Parity.AddOptions(options_parities);

            // ��Ӵٿ� ���� ����
            dropdown_ComPort.value = dropdown_ComPort.options.FindIndex(option => option.text == config.comport);
            dropdown_Baudrate.value = dropdown_Baudrate.options.FindIndex(option => option.text == config.baudrate);
            dropdown_StopBits.value = dropdown_StopBits.options.FindIndex(option => option.text == config.stopbits);
            dropdown_ByteSize.value = dropdown_ByteSize.options.FindIndex(option => option.text == config.bytesize);
            dropdown_Parity.value = dropdown_Parity.options.FindIndex(option => option.text == config.parity);

            // ��� ����
            toggle_DTR.isOn = config.DTR == "1";
            toggle_RTS.isOn = config.RTS == "1";

            btnSaveSerialSet.onClick.RemoveAllListeners();
            btnSaveSerialSet.onClick.AddListener(() =>
            {
                // ��Ӵٿ�� ���õ� �׸��� ���ڿ� ��������
                string selectedComPort = dropdown_ComPort.options[dropdown_ComPort.value].text;
                string interfaceName = inputfield_InterfaceName.text;

                // ICONFIG ���� ���ڿ� ����
                string selectedBaudRate = dropdown_Baudrate.options[dropdown_Baudrate.value].text;
                string selectedByteSize = dropdown_ByteSize.options[dropdown_ByteSize.value].text;
                string selectedStopBits = dropdown_StopBits.options[dropdown_StopBits.value].text;
                string selectedParity = dropdown_Parity.options[dropdown_Parity.value].text;
                switch (dropdown_Parity.options[dropdown_Parity.value].text)
                {
                    case "None":
                        selectedParity = "N";
                        break;
                    case "Even":
                        selectedParity = "E";
                        break;
                    case "Odd":
                        selectedParity = "O";
                        break;
                }
                string dtrState = toggle_DTR.isOn ? "1" : "0";
                string rtsState = toggle_RTS.isOn ? "1" : "0";

                string jsonString = $"{{\"comport\":\"{selectedComPort}\",\"baudrate\":\"{selectedBaudRate}\",\"stopbits\":{selectedStopBits},\"bytesize\":\"{selectedByteSize}\",\"parity\":\"{selectedParity}\",\"DTR\":\"{dtrState}\",\"RTS\":\"{rtsState}\"}}";
                string tblInterfaceQuery = $"UPDATE TBL_INTERFACE SET IID = '{selectedComPort}', INAME = '{interfaceName}', ICONFIG = '{jsonString}' WHERE ID = '{id}'";


                if (ClientDatabase.OnUpdateRequest(tblInterfaceQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    settingSerial.SetActive(false);
                    settingAddInterface.SetActive(false);

                    // �ʱ�ȭ
                    dropdown_ComPort.ClearOptions();
                    dropdown_Baudrate.ClearOptions();
                    dropdown_StopBits.ClearOptions();
                    dropdown_ByteSize.ClearOptions();
                    dropdown_Parity.ClearOptions();
                    inputfield_InterfaceName.text = string.Empty;
                    toggle_DTR.isOn = false;
                    toggle_RTS.isOn = false;
                    txtiName.text = "SERIAL";
                    //Debug.Log($"�������̽� ���� �Ϸ� : {selectedComPort} {interfaceName}");
                }
                else
                {
                    //Debug.Log($"�������̽� ���� ���� : {selectedComPort} {interfaceName}");
                }
            });
        }        
    }

    // �������̽� �߰� ��� ���º�ȭ
    public void OnToggleChanged(Toggle changedToggle)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            GameObject imgSelected = changedToggle.gameObject.transform.Find("Img_Selected").gameObject;
            if (changedToggle.isOn)
            {
                f_selectedToggle = changedToggle.name;
                imgSelected.SetActive(true);
            }
            else
            {
                imgSelected.SetActive(false);
            }
        }
        else
        {
            GameObject imgSelected = changedToggle.gameObject.transform.Find("Img_Selected").gameObject;
            if (changedToggle.isOn)
            {
                selectedToggle = changedToggle.name;
                imgSelected.SetActive(true);
            }
            else
            {
                imgSelected.SetActive(false);
            }
        }        
    }

    // �������̽� �߰� ȭ�� ����
    public void OpenAddInterface()
    {
        if(screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_toggleSerial.isOn = true;
            
            f_selectedToggle = "Toggle_Serial";
            f_settingAddInterface.SetActive(true);
        }
        else
        {
            toggleSerial.isOn = true;

            selectedToggle = "Toggle_Serial";
            settingAddInterface.SetActive(true);
        }        
    }

    // �������̽� �߰� ȭ�� �ݱ�
    public void CloseAddInterface()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_toggleRtuTcp.isOn = false;
            f_selectedToggle = string.Empty;
            f_settingAddInterface.SetActive(false);

        }
        else
        {
            toggleRtuTcp.isOn = false;
            selectedToggle = string.Empty;
            settingAddInterface.SetActive(false);
        }
    }

    // �������̽� ���� �ݱ�
    public void CloseInterfaceSetting()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            for (int i = f_interfaceListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_interfaceListContent.transform.GetChild(i).gameObject);
            }
            ObjectPool.Instance.f_CloseInterfaceSetting();
            f_uiElements.Clear();
            f_settingInterface.SetActive(false);

        }
        else
        {
            for (int i = interfaceListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(interfaceListContent.transform.GetChild(i).gameObject);
            }
            ObjectPool.Instance.CloseInterfaceSetting();
            uiElements.Clear();
            settingInterface.SetActive(false);
        }
    }

    // �������̽� ���� ����
    public void OpenInterfaceSetting()
    {
        SideMenuManager.Instance.SideMenuStateChange();
        LoadInterfaceAssets();
    }

    // �������̽� �ε�
    public void LoadInterfaceAssets()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];

            f_settingInterface.SetActive(true);

            foreach (DataRow InterfaceRow in tableInterface.Rows)
            {
                string id = InterfaceRow["ID"].ToString();
                string iType = InterfaceRow["ITYPE"].ToString();
                string iid = InterfaceRow["IID"].ToString();
                string iName = InterfaceRow["INAME"].ToString();
                string iConfig = InterfaceRow["ICONFIG"].ToString();
                string interfaceContainerName = $"Container_Interface_{id}";

                if (f_uiElements.TryGetValue(interfaceContainerName, out var interfaceContainer))
                {
                    // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
                    TextMeshProUGUI TxtInterfaceName = interfaceContainer.transform.Find("txt_InterfaceName").GetComponent<TextMeshProUGUI>(); // �������̽���

                    interfaceContainer.name = interfaceContainerName;
                    TxtInterfaceName.text = iName;
                    AddInterfaceButtonListener(interfaceContainer, id, iid, iType, iName, iConfig); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
                else
                {
                    GameObject newInterfaceContainer = ObjectPool.Instance.f_GetInterfaceContainerObject(); // ���ο� �������̽� �����̳�
                    TextMeshProUGUI newTxtInterfaceName = newInterfaceContainer.transform.Find("txt_InterfaceName").GetComponent<TextMeshProUGUI>(); // �������̽���
                    f_uiElements[interfaceContainerName] = newInterfaceContainer;

                    newInterfaceContainer.name = interfaceContainerName;
                    newTxtInterfaceName.text = iName;
                    AddInterfaceButtonListener(newInterfaceContainer, id, iid, iType, iName, iConfig); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
            }
        }
        else
        {
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];

            settingInterface.SetActive(true);

            foreach (DataRow InterfaceRow in tableInterface.Rows)
            {
                string id = InterfaceRow["ID"].ToString();
                string iType = InterfaceRow["ITYPE"].ToString();
                string iid = InterfaceRow["IID"].ToString();
                string iName = InterfaceRow["INAME"].ToString();
                string iConfig = InterfaceRow["ICONFIG"].ToString();
                string interfaceContainerName = $"Container_Interface_{id}";

                if (uiElements.TryGetValue(interfaceContainerName, out var interfaceContainer))
                {
                    // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
                    TextMeshProUGUI TxtInterfaceName = interfaceContainer.transform.Find("txt_InterfaceName").GetComponent<TextMeshProUGUI>(); // �������̽���

                    interfaceContainer.name = interfaceContainerName;
                    TxtInterfaceName.text = iName;
                    AddInterfaceButtonListener(interfaceContainer, id, iid, iType, iName, iConfig); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
                else
                {
                    GameObject newInterfaceContainer = ObjectPool.Instance.GetInterfaceContainerObject(); // ���ο� �������̽� �����̳�
                    TextMeshProUGUI newTxtInterfaceName = newInterfaceContainer.transform.Find("txt_InterfaceName").GetComponent<TextMeshProUGUI>(); // �������̽���
                    uiElements[interfaceContainerName] = newInterfaceContainer;

                    newInterfaceContainer.name = interfaceContainerName;
                    newTxtInterfaceName.text = iName;
                    AddInterfaceButtonListener(newInterfaceContainer, id, iid, iType, iName, iConfig); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
            }
        }        
    }

    // �������̽� �����̳� ��ư ������
    public void AddInterfaceButtonListener(GameObject interfaceContainer, string id, string iid, string itype, string iname, string iconfig)
    {
        Button btnInterfaceSetting = interfaceContainer.transform.Find("btn_Setting").GetComponent<Button>(); // �������̽� ���� ��ư
        Button btnDeleteInterface = interfaceContainer.transform.Find("btn_Delete").GetComponent<Button>(); // �������̽� ���� ��ư

        btnInterfaceSetting.onClick.RemoveAllListeners();
        btnDeleteInterface.onClick.RemoveAllListeners();

        btnInterfaceSetting.onClick.AddListener(() =>
        {
            if(itype == "SERIAL")
            {
                OpenModifySettingSerial(id, iid, itype, iname, iconfig);
            }
            else
            {
                OpenModifySettingRTUTCP(id, iid, itype, iname, iconfig);
            }
        });
        btnDeleteInterface.onClick.AddListener(() => DeleteInterface(interfaceContainer, id, iid, itype, iname, iconfig));
    }

    // �������̽� ����
    public void DeleteInterface(GameObject interfaceContainer, string id, string iid, string itype, string iname, string iconfig)
    {
        if(screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Delete;
            screenManager.txt_PopUpMsg.text = $"{iname}��(��) �����Ͻðڽ��ϱ�?\n�ش� �������̽��� ����� ��Ʈ�ѷ� ������ �����˴ϴ�.";

            screenManager.btnPopUpCancel.onClick.RemoveAllListeners();
            screenManager.btnPopUpCancel.onClick.AddListener(() => screenManager.ClosePopUpMessage());
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => {

                string tblInterfaceQuery = $"DELETE FROM TBL_INTERFACE WHERE `ID` = '{id}' AND `IID` = '{iid}';";
                string tblControllerQuery = $"DELETE FROM TBL_CONTROLLER WHERE `ID` = '{id}' AND `IID` = '{iid}';";


                // ���� ���� �Լ��� �Ķ���͸� �����ϴ� ������� ����
                bool interfaceDeleted = ClientDatabase.OnDeleteRequest(tblInterfaceQuery);
                bool controllerUpdated = ClientDatabase.OnUpdateRequest(tblControllerQuery);


                if (interfaceDeleted && controllerUpdated)
                {
                    //Debug.Log($"�������̽� ���� ���� �Ϸ� : {iname}, {iid}");
                    interfaceContainer.SetActive(false);
                    StartCoroutine(RefreshUIAfterCleanup());
                    screenManager.ClosePopUpMessage();
                    screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();

                    LayoutRebuilder.ForceRebuildLayoutImmediate(f_interfaceListContent.GetComponent<RectTransform>());
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    //Debug.Log($"�������̽� ���� ���� ���� : {iname}, {iid}");
                }
            });
        }
        else
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Delete;
            screenManager.txt_PopUpMsg.text = $"{iname}��(��) �����Ͻðڽ��ϱ�?\n�ش� �������̽��� ����� ��Ʈ�ѷ� ������ �����˴ϴ�.";

            screenManager.btnPopUpCancel.onClick.RemoveAllListeners();
            screenManager.btnPopUpCancel.onClick.AddListener(() => screenManager.ClosePopUpMessage());
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => {

                string tblInterfaceQuery = $"DELETE FROM TBL_INTERFACE WHERE `ID` = '{id}' AND `IID` = '{iid}';";
                string tblControllerQuery = $"DELETE FROM TBL_CONTROLLER WHERE `ID` = '{id}' AND `IID` = '{iid}';";


                // ���� ���� �Լ��� �Ķ���͸� �����ϴ� ������� ����
                bool interfaceDeleted = ClientDatabase.OnDeleteRequest(tblInterfaceQuery);
                bool controllerUpdated = ClientDatabase.OnUpdateRequest(tblControllerQuery);


                if (interfaceDeleted && controllerUpdated)
                {
                    //Debug.Log($"�������̽� ���� ���� �Ϸ� : {iname}, {iid}");
                    interfaceContainer.SetActive(false);
                    StartCoroutine(RefreshUIAfterCleanup());
                    screenManager.ClosePopUpMessage();
                    screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();

                    LayoutRebuilder.ForceRebuildLayoutImmediate(interfaceListContent.GetComponent<RectTransform>());
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    //Debug.Log($"�������̽� ���� ���� ���� : {iname}, {iid}");
                }
            });
        }        
    }

    // ���� ���� �� ���ΰ�ħ
    private IEnumerator RefreshUIAfterCleanup()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            for (int i = f_interfaceListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_interfaceListContent.transform.GetChild(i).gameObject);
                yield return new WaitForEndOfFrame();
            }
            ObjectPool.Instance.f_CloseInterfaceSetting();
            f_uiElements.Clear();

            // ���� �����ӱ��� ��ٸ��ϴ�.
            yield return new WaitForEndOfFrame();

            // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
            LoadInterfaceAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(f_interfaceListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            for (int i = interfaceListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(interfaceListContent.transform.GetChild(i).gameObject);
                yield return new WaitForEndOfFrame();
            }
            ObjectPool.Instance.CloseInterfaceSetting();
            uiElements.Clear();

            // ���� �����ӱ��� ��ٸ��ϴ�.
            yield return new WaitForEndOfFrame();

            // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
            LoadInterfaceAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(interfaceListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }
}