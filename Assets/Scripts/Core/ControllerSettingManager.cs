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
using System.Security.Cryptography;

public class ControllerSettingManager : MonoBehaviour
{
    ScreenManager screenManager;

    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> toggleElements = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> f_uiElements = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> f_toggleElements = new Dictionary<string, GameObject>();

    // ù ���� ��
    [Header("ù ���� �� ��Ʈ�ѷ� �������")]
    public GameObject f_settingController;
    public GameObject f_controllerListScrollView; // ��Ʈ�ѷ� ����Ʈ ��ũ�Ѻ�
    public static Transform f_controllerListContent; // ��Ʈ�ѷ� ����Ʈ ��ũ�Ѻ� Content
    private Button f_btnClose;
    private Button f_btnAddController;
    private Button f_btnSave;

    [Header("ù ���� �� ��Ʈ�ѷ� �߰�")]
    public GameObject f_settingAddController; // ��Ʈ�ѷ� �߰�ȭ��
    public Button f_btnCloseAddController; // ��Ʈ�ѷ� �߰�ȭ�� �ݱ� ��ư
    public Button f_btnAdd; // ��Ʈ�ѷ� �߰�ȭ�� �ݱ� ��ư
    public TMP_InputField f_inputfield_ControllerName;
    public TMP_Dropdown f_dropdown_HighGroup;
    public TMP_Dropdown f_dropdown_LowGroup;
    public TMP_Dropdown f_dropdown_Interface;
    public TMP_InputField f_inputfield_EquipNum;
    public Toggle f_toggleShare;
    public Toggle f_toggleSMS;
    public TMP_InputField f_inputfield_Protocol;
    public GameObject f_protocolListScrollView; // �������� ����Ʈ ��ũ�Ѻ�
    public static Transform f_protocolListContent; // �������� ����Ʈ ��ũ�Ѻ� Content

    // ù ���� ��
    [Header("��Ʈ�ѷ� �������")]
    public GameObject settingController;
    public GameObject controllerListScrollView; // ��Ʈ�ѷ� ����Ʈ ��ũ�Ѻ�
    public static Transform controllerListContent; // ��Ʈ�ѷ� ����Ʈ ��ũ�Ѻ� Content
    private Button btnClose;
    private Button btnAddController;
    private Button btnSave;

    [Header("��Ʈ�ѷ� �߰�")]
    public GameObject settingAddController; // ��Ʈ�ѷ� �߰�ȭ��
    public Button btnCloseAddController; // ��Ʈ�ѷ� �߰�ȭ�� �ݱ� ��ư
    public Button btnAdd; // ��Ʈ�ѷ� �߰�ȭ�� �ݱ� ��ư
    public TMP_InputField inputfield_ControllerName;
    public TMP_Dropdown dropdown_HighGroup;
    public TMP_Dropdown dropdown_LowGroup;
    public TMP_Dropdown dropdown_Interface;
    public TMP_InputField inputfield_EquipNum;
    public Toggle toggleShare;
    public Toggle toggleSMS;
    public TMP_InputField inputfield_Protocol;
    public GameObject protocolListScrollView; // �������� ����Ʈ ��ũ�Ѻ�
    public static Transform protocolListContent; // �������� ����Ʈ ��ũ�Ѻ� Content
    public static ControllerSettingManager Instance { get; private set; }
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
        f_controllerListContent = f_controllerListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        // ��Ʈ�ѷ� ����
        f_btnClose = f_settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        f_btnClose.onClick.AddListener(() => CloseControllerSetting());
        f_btnSave = f_settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save").GetComponent<Button>();
        f_btnSave.onClick.AddListener(() => {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "��Ʈ�ѷ� ������ �Ϸ� �Ǿ����ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => 
            {
                screenManager.ClosePopUpMessage();
                f_settingController.SetActive(false);
                FirstStartManager.Instance.firstSet_Controller.SetActive(false);
                FirstStartManager.Instance.FinishedFirstSettings();
            });
        });
        f_btnAddController = f_settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/AddController/btn_AddController").GetComponent<Button>();
        f_btnAddController.onClick.AddListener(() => { OpenAddController(); });

        // ��Ʈ�ѷ� �߰�
        f_btnCloseAddController = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        f_btnCloseAddController.onClick.AddListener(() => { CloseAddController(); });
        f_inputfield_ControllerName = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/InputField_ControllerName").GetComponent<TMP_InputField>();
        f_btnAdd = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save").GetComponent<Button>();
        f_dropdown_HighGroup = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Dropdown_HighGroup").GetComponent<TMP_Dropdown>();
        f_dropdown_LowGroup = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Dropdown_LowGroup").GetComponent<TMP_Dropdown>();
        f_dropdown_Interface = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Dropdown_Interface").GetComponent<TMP_Dropdown>();
        f_inputfield_EquipNum = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/InputField_EquipNum").GetComponent<TMP_InputField>();
        f_inputfield_Protocol = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/ProtocolNToggleParent/InputField_Protocol").GetComponent<TMP_InputField>();
        f_toggleShare = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Toggle_Share").GetComponent<Toggle>();
        f_toggleSMS = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Toggle_SMS").GetComponent<Toggle>();
        f_protocolListContent = f_protocolListScrollView.transform.Find("Viewport/ProtocolListContent").GetComponent<Transform>();

        // ù ���� ����
        controllerListContent = controllerListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        // ��Ʈ�ѷ� ����
        btnClose = settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(() => CloseControllerSetting());
        btnSave = settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save").GetComponent<Button>();
        btnSave.onClick.AddListener(() => {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "��Ʈ�ѷ� ������ �Ϸ� �Ǿ����ϴ�.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => { screenManager.ClosePopUpMessage(); settingController.SetActive(false); });
        });
        btnAddController = settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/AddController/btn_AddController").GetComponent<Button>();
        btnAddController.onClick.AddListener(() => { OpenAddController(); });

        // ��Ʈ�ѷ� �߰�
        btnCloseAddController = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        btnCloseAddController.onClick.AddListener(() => { CloseAddController(); });
        inputfield_ControllerName = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/InputField_ControllerName").GetComponent<TMP_InputField>();
        btnAdd = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save").GetComponent<Button>();        
        dropdown_HighGroup = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Dropdown_HighGroup").GetComponent<TMP_Dropdown>();
        dropdown_LowGroup = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Dropdown_LowGroup").GetComponent<TMP_Dropdown>();
        dropdown_Interface = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Dropdown_Interface").GetComponent<TMP_Dropdown>();
        inputfield_EquipNum = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/InputField_EquipNum").GetComponent<TMP_InputField>();
        inputfield_Protocol = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/ProtocolNToggleParent/InputField_Protocol").GetComponent<TMP_InputField>();
        toggleShare = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Toggle_Share").GetComponent<Toggle>();
        toggleSMS = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/ControllerAdjust/GroupNInterfaceParent/Toggle_SMS").GetComponent<Toggle>();
        protocolListContent = protocolListScrollView.transform.Find("Viewport/ProtocolListContent").GetComponent<Transform>();
    }
        
    // ��Ʈ�ѷ� �߰� ȭ�� ����
    public void OpenAddController()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];
            DataTable tableProtocolList = ClientDatabase.FetchProtocolList().Tables[0];

            f_settingAddController.SetActive(true);

            // �ʱ�ȭ
            TextMeshProUGUI txtControllerSetTitle = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "��Ʈ�ѷ� �߰�";
            TextMeshProUGUI txtBtnControllerModify = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "�߰�";
            f_inputfield_ControllerName.text = string.Empty;
            f_dropdown_HighGroup.ClearOptions();
            f_dropdown_LowGroup.ClearOptions();
            f_dropdown_Interface.ClearOptions();
            f_inputfield_EquipNum.text = string.Empty;
            f_toggleShare.isOn = false;
            f_toggleSMS.isOn = false;
            f_inputfield_Protocol.text = string.Empty;

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // �����׷� �ɼ� �߰�
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // �������̽� �ɼ� �߰�
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            f_dropdown_HighGroup.AddOptions(options_HighGroup);
            f_dropdown_Interface.AddOptions(options_Interface);

            // ù ��° �׸��� �⺻ �������� ����
            if (f_dropdown_HighGroup.options.Count > 0)
                f_dropdown_HighGroup.value = 0;
            if (f_dropdown_Interface.options.Count > 0)
                f_dropdown_Interface.value = 0;

            // ���� �׷� ��Ӵٿ� ����� �̺�Ʈ ������
            f_dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });

            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); // �ʱ� ����

            foreach (DataRow protocolRow in tableProtocolList.Rows)
            {
                string pCatName = protocolRow["CATE_NAME"].ToString();
                string pName = protocolRow["NAME"].ToString();
                string pVer = protocolRow["VER"].ToString();
                string pFWCode = protocolRow["FW_CODE"].ToString();
                string pKey = protocolRow["KEY"].ToString();
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (pCatName.Length > 0 && pName.Length > 0 && pVer.Length > 0 && pFWCode.Length > 0 && pKey.Length > 0)
                {
                    if (f_toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                    {
                        // ���� �������� ��� ��Ұ� ������ ���븸 ������Ʈ
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�

                        protocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.f_GetProtocolToggleObject(); // ���ο� �������� ���
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�
                        f_toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // �������� �Է� �ʵ忡 onValueChanged ������ �߰�
            f_inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(f_inputfield_Protocol.text, tableProtocolList); });

            // �ʱ� ���� �� ��� ����� ������
            FilterProtocolToggles("", tableProtocolList);

            f_btnAdd.onClick.RemoveAllListeners();
            f_btnAdd.onClick.AddListener(() =>
            {
                DataTable tblInterface = ClientDatabase.FetchInterfaceData().Tables[0];
                DataTable tblHg = ClientDatabase.FetchHighGroupData().Tables[0];
                DataTable tblLg = ClientDatabase.FetchLowGroupData().Tables[0];
                string id = string.Empty;
                string itype = string.Empty;
                string iid = string.Empty;
                string hgid = string.Empty;
                string lgid = string.Empty;

                foreach (DataRow row in tblInterface.Rows)
                {
                    if (row["INAME"].ToString() == f_dropdown_Interface.options[f_dropdown_Interface.value].text)
                    {
                        id = row["ID"].ToString();
                        itype = row["ITYPE"].ToString();
                        iid = row["IID"].ToString();
                    }
                }
                foreach (DataRow row in tblHg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == f_dropdown_HighGroup.options[f_dropdown_HighGroup.value].text)
                        hgid = row["FLD_HGID"].ToString();
                }
                foreach (DataRow row in tblLg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == f_dropdown_LowGroup.options[f_dropdown_LowGroup.value].text)
                        lgid = row["FLD_LGID"].ToString();
                }

                // ���õ� �������� ��� ã��
                var selectedToggle = f_protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ���
                if (selectedToggle == null) return; // ���õ� ����� ������ ����
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST���� PKEY, PKEY_ST, PKEY_ED �� ���� ����
                DataRow protocolRow = tableProtocolList.Select($"FW_CODE LIKE '%{fwcode}%'").FirstOrDefault();
                string pkey = protocolRow != null ? protocolRow["KEY"].ToString() : "";
                string pkeySt = protocolRow != null ? protocolRow["START"].ToString() : "";
                string pkeyEd = protocolRow != null ? protocolRow["END"].ToString() : "";


                string selectedCName = f_inputfield_ControllerName.text;
                string selectedHg = f_dropdown_HighGroup.options[f_dropdown_HighGroup.value].text;
                string selectedLg = f_dropdown_LowGroup.options[f_dropdown_LowGroup.value].text;
                string selectedInterfacce = f_dropdown_Interface.options[f_dropdown_Interface.value].text;
                string selectedEquipNum = f_inputfield_EquipNum.text;
                string shareState = f_toggleShare.isOn ? "1" : "0";
                string smsState = f_toggleSMS.isOn ? "1" : "0";

                // ����ȣ�� ���� �������� Ȯ��
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "����ȣ�� �������·θ� �Է� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID �� ���� ����
                //string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID �޼���� �����ؾ� �մϴ�.
                // GROUP_ORDER �� ���� ����
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder �޼���� �����ؾ� �մϴ�.

                // ID�� selectedEquipNum�� ���� �ߺ� üũ
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�ش� �������̽��� �̹� ��ϵ�\n����ȣ�� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                    return;
                }


                string tblControllerQuery = $"INSERT INTO TBL_CONTROLLER (ID, CID, CNAME, PKEY, PKEY_ST, PKEY_ED, SMS, SHARE, ITYPE, IID, HGID, LGID, GROUP_ORDER) VALUES ('{id}', '{selectedEquipNum}', '{selectedCName}', '{pkey}', '{pkeySt}', '{pkeyEd}', '{smsState}', '{shareState}', '{itype}', '{iid}', '{hgid}', '{lgid}', '{groupOrder}')";

                //Debug.Log($"('{id}', '{maxCID}', '{selectedCName}', '{pkey}', '{pkeySt}', '{pkeyEd}', '{smsState}', '{shareState}', '{itype}', '{iid}', '{hgid}', '{lgid}', '{groupOrder}')");
                if (ClientDatabase.OnInsertRequest(tblControllerQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    f_settingAddController.SetActive(false);

                    // �ʱ�ȭ
                    f_dropdown_HighGroup.ClearOptions();
                    f_dropdown_LowGroup.ClearOptions();
                    f_dropdown_Interface.ClearOptions();
                    f_inputfield_EquipNum.text = string.Empty;
                    f_toggleShare.isOn = false;
                    f_toggleSMS.isOn = false;
                    f_inputfield_Protocol.text = string.Empty;
                    f_inputfield_ControllerName.text = string.Empty;
                    //txtiName.text = "SERIAL";
                    //Debug.Log($"��Ʈ�ѷ� �߰� �Ϸ� : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "��Ʈ�ѷ� �߰��� �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                }
            });
        }
        else
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];
            DataTable tableProtocolList = ClientDatabase.FetchProtocolList().Tables[0];

            settingAddController.SetActive(true);

            // �ʱ�ȭ
            TextMeshProUGUI txtControllerSetTitle = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "��Ʈ�ѷ� �߰�";
            TextMeshProUGUI txtBtnControllerModify = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "�߰�";
            inputfield_ControllerName.text = string.Empty;
            dropdown_HighGroup.ClearOptions();
            dropdown_LowGroup.ClearOptions();
            dropdown_Interface.ClearOptions();
            inputfield_EquipNum.text = string.Empty;
            toggleShare.isOn = false;
            toggleSMS.isOn = false;
            inputfield_Protocol.text = string.Empty;

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // �����׷� �ɼ� �߰�
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // �������̽� �ɼ� �߰�
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            dropdown_HighGroup.AddOptions(options_HighGroup);
            dropdown_Interface.AddOptions(options_Interface);

            // ù ��° �׸��� �⺻ �������� ����
            if (dropdown_HighGroup.options.Count > 0)
                dropdown_HighGroup.value = 0;
            if (dropdown_Interface.options.Count > 0)
                dropdown_Interface.value = 0;

            // ���� �׷� ��Ӵٿ� ����� �̺�Ʈ ������
            dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });

            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); // �ʱ� ����

            foreach (DataRow protocolRow in tableProtocolList.Rows)
            {
                string pCatName = protocolRow["CATE_NAME"].ToString();
                string pName = protocolRow["NAME"].ToString();
                string pVer = protocolRow["VER"].ToString();
                string pFWCode = protocolRow["FW_CODE"].ToString();
                string pKey = protocolRow["KEY"].ToString();
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (pCatName.Length > 0 && pName.Length > 0 && pVer.Length > 0 && pFWCode.Length > 0 && pKey.Length > 0)
                {
                    if (toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                    {
                        // ���� �������� ��� ��Ұ� ������ ���븸 ������Ʈ
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�

                        protocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.GetProtocolToggleObject(); // ���ο� �������� ���
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�
                        toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // �������� �Է� �ʵ忡 onValueChanged ������ �߰�
            inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(inputfield_Protocol.text, tableProtocolList); });

            // �ʱ� ���� �� ��� ����� ������
            FilterProtocolToggles("", tableProtocolList);

            btnAdd.onClick.RemoveAllListeners();
            btnAdd.onClick.AddListener(() =>
            {
                DataTable tblInterface = ClientDatabase.FetchInterfaceData().Tables[0];
                DataTable tblHg = ClientDatabase.FetchHighGroupData().Tables[0];
                DataTable tblLg = ClientDatabase.FetchLowGroupData().Tables[0];
                string id = string.Empty;
                string itype = string.Empty;
                string iid = string.Empty;
                string hgid = string.Empty;
                string lgid = string.Empty;

                foreach (DataRow row in tblInterface.Rows)
                {
                    if (row["INAME"].ToString() == dropdown_Interface.options[dropdown_Interface.value].text)
                    {
                        id = row["ID"].ToString();
                        itype = row["ITYPE"].ToString();
                        iid = row["IID"].ToString();
                    }
                }
                foreach (DataRow row in tblHg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == dropdown_HighGroup.options[dropdown_HighGroup.value].text)
                        hgid = row["FLD_HGID"].ToString();
                }
                foreach (DataRow row in tblLg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == dropdown_LowGroup.options[dropdown_LowGroup.value].text)
                        lgid = row["FLD_LGID"].ToString();
                }

                // ���õ� �������� ��� ã��
                var selectedToggle = protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ���
                if (selectedToggle == null) return; // ���õ� ����� ������ ����
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST���� PKEY, PKEY_ST, PKEY_ED �� ���� ����
                DataRow protocolRow = tableProtocolList.Select($"FW_CODE LIKE '%{fwcode}%'").FirstOrDefault();
                string pkey = protocolRow != null ? protocolRow["KEY"].ToString() : "";
                string pkeySt = protocolRow != null ? protocolRow["START"].ToString() : "";
                string pkeyEd = protocolRow != null ? protocolRow["END"].ToString() : "";


                string selectedCName = inputfield_ControllerName.text;
                string selectedHg = dropdown_HighGroup.options[dropdown_HighGroup.value].text;
                string selectedLg = dropdown_LowGroup.options[dropdown_LowGroup.value].text;
                string selectedInterfacce = dropdown_Interface.options[dropdown_Interface.value].text;
                string selectedEquipNum = inputfield_EquipNum.text;
                string shareState = toggleShare.isOn ? "1" : "0";
                string smsState = toggleSMS.isOn ? "1" : "0";

                // ����ȣ�� ���� �������� Ȯ��
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "����ȣ�� �������·θ� �Է� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID �� ���� ����
                //string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID �޼���� �����ؾ� �մϴ�.
                // GROUP_ORDER �� ���� ����
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder �޼���� �����ؾ� �մϴ�.

                // ID�� selectedEquipNum�� ���� �ߺ� üũ
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�ش� �������̽��� �̹� ��ϵ�\n����ȣ�� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                    return;
                }


                string tblControllerQuery = $"INSERT INTO TBL_CONTROLLER (ID, CID, CNAME, PKEY, PKEY_ST, PKEY_ED, SMS, SHARE, ITYPE, IID, HGID, LGID, GROUP_ORDER) VALUES ('{id}', '{selectedEquipNum}', '{selectedCName}', '{pkey}', '{pkeySt}', '{pkeyEd}', '{smsState}', '{shareState}', '{itype}', '{iid}', '{hgid}', '{lgid}', '{groupOrder}')";

                //Debug.Log($"('{id}', '{maxCID}', '{selectedCName}', '{pkey}', '{pkeySt}', '{pkeyEd}', '{smsState}', '{shareState}', '{itype}', '{iid}', '{hgid}', '{lgid}', '{groupOrder}')");
                if (ClientDatabase.OnInsertRequest(tblControllerQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    settingAddController.SetActive(false);

                    // �ʱ�ȭ
                    dropdown_HighGroup.ClearOptions();
                    dropdown_LowGroup.ClearOptions();
                    dropdown_Interface.ClearOptions();
                    inputfield_EquipNum.text = string.Empty;
                    toggleShare.isOn = false;
                    toggleSMS.isOn = false;
                    inputfield_Protocol.text = string.Empty;
                    inputfield_ControllerName.text = string.Empty;
                    //txtiName.text = "SERIAL";
                    //Debug.Log($"��Ʈ�ѷ� �߰� �Ϸ� : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "��Ʈ�ѷ� �߰��� �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                        
                    });
                }
            });
        }        
    }

    private void FilterProtocolToggles(string searchText, DataTable tableProtocolList)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            searchText = searchText.ToLower();

            foreach (DataRow protocolRow in tableProtocolList.Rows)
            {
                string pCatName = protocolRow["CATE_NAME"].ToString().ToLower();
                string pName = protocolRow["NAME"].ToString().ToLower();
                string pFWCode = protocolRow["FW_CODE"].ToString().ToLower();

                // ��� �̸� ���� ������ �����Ͽ� ��� �˻�
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (f_toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                {
                    // �˻� �ؽ�Ʈ�� ��ġ�ϰų� �˻� �ؽ�Ʈ�� �� ���ڿ��� ��� ����� ������
                    bool isVisible = string.IsNullOrEmpty(searchText) || pCatName.Contains(searchText) || pName.Contains(searchText) || pFWCode.Contains(searchText);
                    protocolToggle.SetActive(isVisible);
                }
            }
        }
        else
        {
            searchText = searchText.ToLower();

            foreach (DataRow protocolRow in tableProtocolList.Rows)
            {
                string pCatName = protocolRow["CATE_NAME"].ToString().ToLower();
                string pName = protocolRow["NAME"].ToString().ToLower();
                string pFWCode = protocolRow["FW_CODE"].ToString().ToLower();

                // ��� �̸� ���� ������ �����Ͽ� ��� �˻�
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                {
                    // �˻� �ؽ�Ʈ�� ��ġ�ϰų� �˻� �ؽ�Ʈ�� �� ���ڿ��� ��� ����� ������
                    bool isVisible = string.IsNullOrEmpty(searchText) || pCatName.Contains(searchText) || pName.Contains(searchText) || pFWCode.Contains(searchText);
                    protocolToggle.SetActive(isVisible);
                }
            }
        }        
    }

    private void UpdateLowGroupDropdown(DataTable tableHighGroup, DataTable tableLowGroup)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            string selectedHighGroupName = f_dropdown_HighGroup.options[f_dropdown_HighGroup.value].text;
            DataRow[] selectedRows = tableHighGroup.Select($"FLD_NAME = '{selectedHighGroupName}'");
            if (selectedRows.Length > 0)
            {
                string selectedHGID = selectedRows[0]["FLD_HGID"].ToString();

                // LowGroup ��Ӵٿ� �ʱ�ȭ
                f_dropdown_LowGroup.ClearOptions();
                List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();

                // ��ġ�ϴ� FLD_HGID�� ���� LowGroup ������ ���� �� �ɼ� �߰�
                foreach (DataRow row in tableLowGroup.Select($"FLD_HGID = '{selectedHGID}'"))
                {
                    string lgName = row["FLD_NAME"].ToString();
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(lgName);
                    options_LowGroup.Add(option);
                }

                // ��Ӵٿ ���� �׸�� ����
                f_dropdown_LowGroup.AddOptions(options_LowGroup);

                // ù ��° �׸��� �⺻ �������� ����
                if (f_dropdown_LowGroup.options.Count > 0)
                    f_dropdown_LowGroup.value = 0;
            }
        }
        else
        {
            string selectedHighGroupName = dropdown_HighGroup.options[dropdown_HighGroup.value].text;
            DataRow[] selectedRows = tableHighGroup.Select($"FLD_NAME = '{selectedHighGroupName}'");
            if (selectedRows.Length > 0)
            {
                string selectedHGID = selectedRows[0]["FLD_HGID"].ToString();

                // LowGroup ��Ӵٿ� �ʱ�ȭ
                dropdown_LowGroup.ClearOptions();
                List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();

                // ��ġ�ϴ� FLD_HGID�� ���� LowGroup ������ ���� �� �ɼ� �߰�
                foreach (DataRow row in tableLowGroup.Select($"FLD_HGID = '{selectedHGID}'"))
                {
                    string lgName = row["FLD_NAME"].ToString();
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(lgName);
                    options_LowGroup.Add(option);
                }

                // ��Ӵٿ ���� �׸�� ����
                dropdown_LowGroup.AddOptions(options_LowGroup);

                // ù ��° �׸��� �⺻ �������� ����
                if (dropdown_LowGroup.options.Count > 0)
                    dropdown_LowGroup.value = 0;
            }
        }        
    }

    // ��Ʈ�ѷ� �߰� ȭ�� �ݱ�
    public void CloseAddController()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_dropdown_HighGroup.ClearOptions();
            f_dropdown_LowGroup.ClearOptions();
            f_dropdown_Interface.ClearOptions();
            f_toggleShare.isOn = false;
            f_toggleSMS.isOn = false;
            f_inputfield_Protocol.text = string.Empty;
            f_inputfield_ControllerName.text = string.Empty;
            


            for (int i = f_protocolListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_protocolListContent.transform.GetChild(i).gameObject);
            }
            ObjectPool.Instance.f_CloseControllerAddSetting();
            f_toggleElements.Clear();

            StartCoroutine(RefreshUIAfterCleanup());
            f_settingAddController.SetActive(false);
        }
        else
        {
            dropdown_HighGroup.ClearOptions();
            dropdown_LowGroup.ClearOptions();
            dropdown_Interface.ClearOptions();
            toggleShare.isOn = false;
            toggleSMS.isOn = false;
            inputfield_Protocol.text = string.Empty;
            inputfield_ControllerName.text = string.Empty;
            inputfield_EquipNum.text = string.Empty;


            for (int i = protocolListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(protocolListContent.transform.GetChild(i).gameObject);
            }
            ObjectPool.Instance.CloseControllerAddSetting();
            toggleElements.Clear();

            StartCoroutine(RefreshUIAfterCleanup());
            settingAddController.SetActive(false);
        }        
    }

    // ��Ʈ�ѷ� ���� �ݱ�
    public void CloseControllerSetting()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            for (int i = f_controllerListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_controllerListContent.transform.GetChild(i).gameObject);
            }
            ObjectPool.Instance.f_CloseControllerSetting();
            f_uiElements.Clear();
            f_settingController.SetActive(false);
        }
        else
        {
            for (int i = controllerListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(controllerListContent.transform.GetChild(i).gameObject);
            }
            ObjectPool.Instance.CloseControllerSetting();
            uiElements.Clear();
            settingController.SetActive(false);
        }        
    }

    // ��Ʈ�ѷ� ���� ����
    public void OpenControllerSetting()
    {
        SideMenuManager.Instance.SideMenuStateChange();
        LoadControllerAssets();
    }

    // �������̽� �ε�
    public void LoadControllerAssets()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableController = ClientDatabase.FetchControllerData().Tables[0];

            f_settingController.SetActive(true);

            foreach (DataRow controllerRow in tableController.Rows)
            {
                string id = controllerRow["ID"].ToString();
                string cid = controllerRow["CID"].ToString();
                string cname = controllerRow["CNAME"].ToString();
                string pkey = controllerRow["PKEY"].ToString();
                string pkeySt = controllerRow["PKEY_ST"].ToString();
                string pkeyEd = controllerRow["PKEY_ED"].ToString();
                string sms = controllerRow["SMS"].ToString();
                string share = controllerRow["SHARE"].ToString();
                string itype = controllerRow["ITYPE"].ToString();
                string iid = controllerRow["IID"].ToString();
                string alarmMin = controllerRow["ALARM_MIN"].ToString();
                string hgid = controllerRow["HGID"].ToString();
                string lgid = controllerRow["LGID"].ToString();
                string groupOrder = controllerRow["GROUP_ORDER"].ToString();
                string controllerContainerName = $"Container_Controller_{id}_{cid}";

                if (f_uiElements.TryGetValue(controllerContainerName, out var controllerContainer))
                {
                    // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
                    TextMeshProUGUI TxtControllerName = controllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // ��Ʈ�ѷ���

                    controllerContainer.name = controllerContainerName;
                    TxtControllerName.text = cname;
                    AddControllerButtonListener(controllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
                else
                {
                    GameObject newControllerContainer = ObjectPool.Instance.f_GetControllerSetContainerObject(); // ���ο� ��Ʈ�ѷ� �����̳�
                    TextMeshProUGUI newTxtControllerName = newControllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // ��Ʈ�ѷ���
                    f_uiElements[controllerContainerName] = newControllerContainer;

                    newControllerContainer.name = controllerContainerName;
                    newTxtControllerName.text = cname;
                    AddControllerButtonListener(newControllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
            }
        }
        else
        {
            DataTable tableController = ClientDatabase.FetchControllerData().Tables[0];

            settingController.SetActive(true);

            foreach (DataRow controllerRow in tableController.Rows)
            {
                string id = controllerRow["ID"].ToString();
                string cid = controllerRow["CID"].ToString();
                string cname = controllerRow["CNAME"].ToString();
                string pkey = controllerRow["PKEY"].ToString();
                string pkeySt = controllerRow["PKEY_ST"].ToString();
                string pkeyEd = controllerRow["PKEY_ED"].ToString();
                string sms = controllerRow["SMS"].ToString();
                string share = controllerRow["SHARE"].ToString();
                string itype = controllerRow["ITYPE"].ToString();
                string iid = controllerRow["IID"].ToString();
                string alarmMin = controllerRow["ALARM_MIN"].ToString();
                string hgid = controllerRow["HGID"].ToString();
                string lgid = controllerRow["LGID"].ToString();
                string groupOrder = controllerRow["GROUP_ORDER"].ToString();
                string controllerContainerName = $"Container_Controller_{id}_{cid}";

                if (uiElements.TryGetValue(controllerContainerName, out var controllerContainer))
                {
                    // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
                    TextMeshProUGUI TxtControllerName = controllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // ��Ʈ�ѷ���

                    controllerContainer.name = controllerContainerName;
                    TxtControllerName.text = cname;
                    AddControllerButtonListener(controllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
                else
                {
                    GameObject newControllerContainer = ObjectPool.Instance.GetControllerSetContainerObject(); // ���ο� ��Ʈ�ѷ� �����̳�
                    TextMeshProUGUI newTxtControllerName = newControllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // ��Ʈ�ѷ���
                    uiElements[controllerContainerName] = newControllerContainer;

                    newControllerContainer.name = controllerContainerName;
                    newTxtControllerName.text = cname;
                    AddControllerButtonListener(newControllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                }
            }
        }        
    }

    // �������̽� �����̳� ��ư ������
    public void AddControllerButtonListener(GameObject controllerContainer, string id, string cid, string cname, string pkey, string pkeyst, string pkeyed, string sms, string share, string itype, string iid, string alarmmin, string hgid, string lgid, string grouporder)
    {
        Button btnControllerSetting = controllerContainer.transform.Find("btn_Setting").GetComponent<Button>(); // �������̽� ���� ��ư
        Button btnDeleteController = controllerContainer.transform.Find("btn_Delete").GetComponent<Button>(); // �������̽� ���� ��ư

        btnControllerSetting.onClick.RemoveAllListeners();
        btnDeleteController.onClick.RemoveAllListeners();

        btnControllerSetting.onClick.AddListener(() =>
        {
            OpenModifySettingController(id, cid, cname, pkey, pkeyst, pkeyed, sms, share, itype, iid, alarmmin, hgid, lgid, grouporder);
        });
        btnDeleteController.onClick.AddListener(() => DeleteController(controllerContainer, id, cid, cname, pkey, pkeyst, pkeyed, sms, share, itype, iid, alarmmin, hgid, lgid, grouporder));
    }

    // ��Ʈ�ѷ� ����
    public void DeleteController(GameObject controllerContainer, string id, string cid, string cname, string pkey, string pkeyst, string pkeyed, string sms, string share, string itype, string iid, string alarmmin, string hgid, string lgid, string grouporder)
    {

        screenManager.CurrentPopUpState = ScreenManager.PopUpState.Delete;
        screenManager.txt_PopUpMsg.text = $"{cname}��(��) �����Ͻðڽ��ϱ�?";

        screenManager.btnPopUpCancel.onClick.RemoveAllListeners();
        screenManager.btnPopUpCancel.onClick.AddListener(() => screenManager.ClosePopUpMessage());
        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
        screenManager.btnPopUpConfirm.onClick.AddListener(() =>
        {

            string tblControllerQuery = $"DELETE FROM TBL_CONTROLLER WHERE `ID` = '{id}' AND `CID` = '{cid}';";
            string tblRealTimeQuery = $"DELETE FROM TBL_REALTIME WHERE `ID` = '{id}' AND `CID` = '{cid}';";

            // ���� ���� �Լ��� �Ķ���͸� �����ϴ� ������� ����            
            bool controllerDelete = ClientDatabase.OnDeleteRequest(tblControllerQuery);
            bool realtimeDelete = ClientDatabase.OnDeleteRequest(tblRealTimeQuery);

            if (controllerDelete && realtimeDelete)
            {
                //Debug.Log($"��Ʈ�ѷ� ���� ���� �Ϸ� : {cname}, {id}, {cid}");
                controllerContainer.SetActive(false);
                StartCoroutine(RefreshUIAfterCleanup());
                screenManager.ClosePopUpMessage();
                screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();

                List<string> keysToRemove = new List<string>();

                foreach (var kvp in ClientDatabase.controllerGridInstances)
                {
                    string[] parts = kvp.Key.Split('_');
                    string iid = parts[1];
                    string cid = parts[2];

                    string checkExistenceQuery = $"SELECT * FROM TBL_CONTROLLER WHERE ID = '{iid}' AND CID = '{cid}'";
                    if (!ClientDatabase.DoesExistInTableCheck(checkExistenceQuery))
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
                foreach (var kvp in ClientDatabase.controllerListInstances)
                {
                    string[] parts = kvp.Key.Split('_');
                    string iid = parts[1];
                    string cid = parts[2];

                    string checkExistenceQuery = $"SELECT * FROM TBL_CONTROLLER WHERE ID = '{iid}' AND CID = '{cid}'";
                    if (!ClientDatabase.DoesExistInTableCheck(checkExistenceQuery))
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
                foreach (var kvp in ClientDatabase.trendGridInstances)
                {
                    string[] parts = kvp.Key.Split('_');
                    string iid = parts[1];
                    string cid = parts[2];
                    string addr = parts[3];

                    string checkExistenceQuery = $"SELECT * FROM TBL_CONTROLLER WHERE ID = '{iid}' AND CID = '{cid}'";
                    if (!ClientDatabase.DoesExistInTableCheck(checkExistenceQuery))
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }
                foreach (var kvp in ClientDatabase.trendListInstances)
                {
                    string[] parts = kvp.Key.Split('_');
                    string iid = parts[1];
                    string cid = parts[2];
                    string addr = parts[3];

                    string checkExistenceQuery = $"SELECT * FROM TBL_CONTROLLER WHERE ID = '{iid}' AND CID = '{cid}'";
                    if (!ClientDatabase.DoesExistInTableCheck(checkExistenceQuery))
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                // ������ Ű ��Ͽ� ���� �ν��Ͻ� ����
                foreach (var key in keysToRemove)
                {
                    GameObject instanceToRemove;
                    if (ClientDatabase.controllerGridInstances.TryGetValue(key, out instanceToRemove))
                    {
                        Destroy(instanceToRemove);
                        ClientDatabase.controllerGridInstances.Remove(key);
                        ObjectPool.Instance.controllerObjects.Remove(instanceToRemove);
                        ObjectPool.Instance.trendObjects.Remove(instanceToRemove);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    GameObject instanceToRemove;
                    if (ClientDatabase.controllerListInstances.TryGetValue(key, out instanceToRemove))
                    {
                        Destroy(instanceToRemove);
                        ClientDatabase.controllerListInstances.Remove(key);
                        ObjectPool.Instance.controllerObjects.Remove(instanceToRemove);
                        ObjectPool.Instance.trendObjects.Remove(instanceToRemove);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    GameObject instanceToRemove;
                    if (ClientDatabase.trendGridInstances.TryGetValue(key, out instanceToRemove))
                    {
                        Destroy(instanceToRemove);
                        ClientDatabase.trendGridInstances.Remove(key);
                        ObjectPool.Instance.controllerObjects.Remove(instanceToRemove);
                        ObjectPool.Instance.trendObjects.Remove(instanceToRemove);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    GameObject instanceToRemove;
                    if (ClientDatabase.trendListInstances.TryGetValue(key, out instanceToRemove))
                    {
                        Destroy(instanceToRemove);
                        ClientDatabase.trendListInstances.Remove(key);
                        ObjectPool.Instance.controllerObjects.Remove(instanceToRemove);
                        ObjectPool.Instance.trendObjects.Remove(instanceToRemove);
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(controllerListContent.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
            else
            {
                //Debug.Log($"��Ʈ�ѷ� ���� ���� ���� : {cname}, {id}, {cid}");
            }
        });

    }

    // ��Ʈ�ѷ� ����
    public void OpenModifySettingController(string id, string cid, string cname, string pkey, string pkeyst, string pkeyed, string sms, string share, string itype, string iid, string alarmmin, string hgid, string lgid, string grouporder)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];
            DataTable tableProtocolList = ClientDatabase.FetchProtocolList().Tables[0];

            f_settingAddController.SetActive(true);

            // �ʱ�ȭ
            TextMeshProUGUI txtControllerSetTitle = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "��Ʈ�ѷ� ����";
            TextMeshProUGUI txtBtnControllerModify = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "����";
            f_inputfield_ControllerName.text = string.Empty;
            f_dropdown_HighGroup.ClearOptions();
            f_dropdown_LowGroup.ClearOptions();
            f_dropdown_Interface.ClearOptions();
            f_toggleShare.isOn = false;
            f_toggleSMS.isOn = false;
            f_inputfield_Protocol.text = string.Empty;            
            f_inputfield_EquipNum.text = string.Empty;

            // �� �Ҵ�
            f_inputfield_ControllerName.text = cname;
            f_inputfield_EquipNum.text = cid;

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // �����׷� �ɼ� �߰�
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // �������̽� �ɼ� �߰�
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            f_dropdown_HighGroup.AddOptions(options_HighGroup);
            f_dropdown_Interface.AddOptions(options_Interface);

            // ���� �׷� ��Ӵٿ� ����� �̺�Ʈ ������
            f_dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });
            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup);// �ʱ�ȭ


            foreach (DataRow row in tableHighGroup.Rows)
            {
                if (row["FLD_HGID"].ToString() == hgid)
                {
                    // �����׷� ��Ӵٿ��� ���� ���� ����
                    int highGroupIndex = options_HighGroup.FindIndex(option => option.text == row["FLD_NAME"].ToString());
                    if (highGroupIndex != -1)
                    {
                        f_dropdown_HighGroup.value = highGroupIndex;
                    }
                }
            }
            foreach (DataRow row in tableLowGroup.Rows)
            {
                if (row["FLD_HGID"].ToString() == hgid && row["FLD_LGID"].ToString() == lgid)
                {
                    // �����׷� ��Ӵٿ��� ���� ���� ����
                    int lowGroupIndex = options_LowGroup.FindIndex(option => option.text == row["FLD_NAME"].ToString());
                    if (lowGroupIndex != -1)
                    {
                        f_dropdown_LowGroup.value = lowGroupIndex;
                    }
                }
            }
            foreach (DataRow row in tableInterface.Rows)
            {
                if (row["IID"].ToString() == iid && row["ID"].ToString() == id)
                {
                    // �������̽� ��Ӵٿ��� ���� ���� ����
                    int interfaceIndex = options_Interface.FindIndex(option => option.text == row["INAME"].ToString());
                    if (interfaceIndex != -1)
                    {
                        f_dropdown_Interface.value = interfaceIndex;
                    }
                }
            }

            // SMS�� Share ��� ���� ����
            f_toggleShare.isOn = share == "1";
            f_toggleSMS.isOn = sms == "1";

            foreach (DataRow protocolRow in tableProtocolList.Rows)
            {
                string pCatName = protocolRow["CATE_NAME"].ToString();
                string pName = protocolRow["NAME"].ToString();
                string pVer = protocolRow["VER"].ToString();
                string pFWCode = protocolRow["FW_CODE"].ToString();
                string pKey = protocolRow["KEY"].ToString();
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (pCatName.Length > 0 && pName.Length > 0 && pVer.Length > 0 && pFWCode.Length > 0 && pKey.Length > 0)
                {
                    if (f_toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                    {
                        // ���� �������� ��� ��Ұ� ������ ���븸 ������Ʈ
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�

                        protocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.f_GetProtocolToggleObject(); // ���ο� �������� ���
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�
                        f_toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // �������� ��� Ȱ��ȭ
            foreach (DataRow row in tableProtocolList.Rows)
            {
                if (row["KEY"].ToString() == pkey)
                {
                    string fwcode = row["FW_CODE"].ToString();
                    f_toggleElements[$"Toggle_Protocol_{fwcode}"].GetComponent<Toggle>().isOn = true;
                    break;
                }
            }

            // �������� �Է� �ʵ忡 onValueChanged ������ �߰�
            f_inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(f_inputfield_Protocol.text, tableProtocolList); });

            // �ʱ� ���� �� ��� ����� ������
            FilterProtocolToggles("", tableProtocolList);

            f_btnAdd.onClick.RemoveAllListeners();
            f_btnAdd.onClick.AddListener(() =>
            {
                DataTable tblInterface = ClientDatabase.FetchInterfaceData().Tables[0];
                DataTable tblHg = ClientDatabase.FetchHighGroupData().Tables[0];
                DataTable tblLg = ClientDatabase.FetchLowGroupData().Tables[0];
                string id = string.Empty;
                string itype = string.Empty;
                string iid = string.Empty;
                string hgid = string.Empty;
                string lgid = string.Empty;

                foreach (DataRow row in tblInterface.Rows)
                {
                    if (row["INAME"].ToString() == f_dropdown_Interface.options[f_dropdown_Interface.value].text)
                    {
                        id = row["ID"].ToString();
                        itype = row["ITYPE"].ToString();
                        iid = row["IID"].ToString();
                    }
                }
                foreach (DataRow row in tblHg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == f_dropdown_HighGroup.options[f_dropdown_HighGroup.value].text)
                        hgid = row["FLD_HGID"].ToString();
                }
                foreach (DataRow row in tblLg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == f_dropdown_LowGroup.options[f_dropdown_LowGroup.value].text)
                        lgid = row["FLD_LGID"].ToString();
                }

                // ���õ� �������� ��� ã��
                var selectedToggle = f_protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ���
                if (selectedToggle == null) return; // ���õ� ����� ������ ����
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST���� PKEY, PKEY_ST, PKEY_ED �� ���� ����
                DataRow protocolRow = tableProtocolList.Select($"FW_CODE LIKE '%{fwcode}%'").FirstOrDefault();
                string pkey = protocolRow != null ? protocolRow["KEY"].ToString() : "";
                string pkeySt = protocolRow != null ? protocolRow["START"].ToString() : "";
                string pkeyEd = protocolRow != null ? protocolRow["END"].ToString() : "";


                string selectedCName = f_inputfield_ControllerName.text;
                string selectedHg = f_dropdown_HighGroup.options[f_dropdown_HighGroup.value].text;
                string selectedLg = f_dropdown_LowGroup.options[f_dropdown_LowGroup.value].text;
                string selectedInterfacce = f_dropdown_Interface.options[f_dropdown_Interface.value].text;
                string selectedEquipNum = f_inputfield_EquipNum.text;
                string shareState = f_toggleShare.isOn ? "1" : "0";
                string smsState = f_toggleSMS.isOn ? "1" : "0";

                // ����ȣ�� ���� �������� Ȯ��
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "����ȣ�� �������·θ� �Է� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID �� ���� ����
                string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID �޼���� �����ؾ� �մϴ�.
                                                                               // GROUP_ORDER �� ���� ����
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder �޼���� �����ؾ� �մϴ�.

                // ID�� selectedEquipNum�� ���� �ߺ� üũ
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�ش� �������̽��� �̹� ��ϵ�\n����ȣ�� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                    return;
                }

                string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET ID = '{id}', CID = '{selectedEquipNum}', CNAME = '{selectedCName}', PKEY = '{pkey}', PKEY_ST = '{pkeySt}', PKEY_ED = '{pkeyEd}', SMS = '{smsState}', SHARE = '{shareState}', ITYPE = '{itype}', IID = '{iid}', HGID = '{hgid}', LGID = '{lgid}', GROUP_ORDER = '{groupOrder}' WHERE ID = '{id}' AND CID = '{cid}'";


                if (ClientDatabase.OnUpdateRequest(tblControllerQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    f_settingAddController.SetActive(false);

                    // �ʱ�ȭ
                    f_dropdown_HighGroup.ClearOptions();
                    f_dropdown_LowGroup.ClearOptions();
                    f_dropdown_Interface.ClearOptions();
                    f_toggleShare.isOn = false;
                    f_toggleSMS.isOn = false;
                    f_inputfield_Protocol.text = string.Empty;
                    f_inputfield_EquipNum.text = string.Empty;
                    f_inputfield_ControllerName.text = string.Empty;

                    //Debug.Log($"��Ʈ�ѷ� ���� �Ϸ� : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "��Ʈ�ѷ� ������ �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                }
            });
        }
        else
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];
            DataTable tableProtocolList = ClientDatabase.FetchProtocolList().Tables[0];

            settingAddController.SetActive(true);

            // �ʱ�ȭ
            TextMeshProUGUI txtControllerSetTitle = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "��Ʈ�ѷ� ����";
            TextMeshProUGUI txtBtnControllerModify = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "����";
            inputfield_ControllerName.text = string.Empty;
            dropdown_HighGroup.ClearOptions();
            dropdown_LowGroup.ClearOptions();
            dropdown_Interface.ClearOptions();
            toggleShare.isOn = false;
            toggleSMS.isOn = false;
            inputfield_Protocol.text = string.Empty;
            inputfield_EquipNum.text = string.Empty;

            // �� �Ҵ�
            inputfield_ControllerName.text = cname;
            inputfield_EquipNum.text = cid;

            // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // �����׷� �ɼ� �߰�
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // �������̽� �ɼ� �߰�
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // ��Ӵٿ ���� �׸�� ����
            dropdown_HighGroup.AddOptions(options_HighGroup);
            dropdown_Interface.AddOptions(options_Interface);

            // ���� �׷� ��Ӵٿ� ����� �̺�Ʈ ������
            dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });
            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup);// �ʱ�ȭ


            foreach (DataRow row in tableHighGroup.Rows)
            {
                if (row["FLD_HGID"].ToString() == hgid)
                {
                    // �����׷� ��Ӵٿ��� ���� ���� ����
                    int highGroupIndex = options_HighGroup.FindIndex(option => option.text == row["FLD_NAME"].ToString());
                    if (highGroupIndex != -1)
                    {
                        dropdown_HighGroup.value = highGroupIndex;
                    }
                }
            }
            foreach (DataRow row in tableLowGroup.Rows)
            {
                if (row["FLD_HGID"].ToString() == hgid && row["FLD_LGID"].ToString() == lgid)
                {
                    // �����׷� ��Ӵٿ��� ���� ���� ����
                    int lowGroupIndex = options_LowGroup.FindIndex(option => option.text == row["FLD_NAME"].ToString());
                    if (lowGroupIndex != -1)
                    {
                        dropdown_LowGroup.value = lowGroupIndex;
                    }
                }
            }
            foreach (DataRow row in tableInterface.Rows)
            {
                if (row["IID"].ToString() == iid && row["ID"].ToString() == id)
                {
                    // �������̽� ��Ӵٿ��� ���� ���� ����
                    int interfaceIndex = options_Interface.FindIndex(option => option.text == row["INAME"].ToString());
                    if (interfaceIndex != -1)
                    {
                        dropdown_Interface.value = interfaceIndex;
                    }
                }
            }

            // SMS�� Share ��� ���� ����
            toggleShare.isOn = share == "1";
            toggleSMS.isOn = sms == "1";




            foreach (DataRow protocolRow in tableProtocolList.Rows)
            {
                string pCatName = protocolRow["CATE_NAME"].ToString();
                string pName = protocolRow["NAME"].ToString();
                string pVer = protocolRow["VER"].ToString();
                string pFWCode = protocolRow["FW_CODE"].ToString();
                string pKey = protocolRow["KEY"].ToString();
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (pCatName.Length > 0 && pName.Length > 0 && pVer.Length > 0 && pFWCode.Length > 0 && pKey.Length > 0)
                {
                    if (toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                    {
                        // ���� �������� ��� ��Ұ� ������ ���븸 ������Ʈ
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�

                        protocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.GetProtocolToggleObject(); // ���ο� �������� ���
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // �������ݸ�
                        toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // �������� ��� Ȱ��ȭ
            foreach (DataRow row in tableProtocolList.Rows)
            {
                if (row["KEY"].ToString() == pkey)
                {
                    string fwcode = row["FW_CODE"].ToString();
                    toggleElements[$"Toggle_Protocol_{fwcode}"].GetComponent<Toggle>().isOn = true;
                    break;
                }
            }

            // �������� �Է� �ʵ忡 onValueChanged ������ �߰�
            inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(inputfield_Protocol.text, tableProtocolList); });

            // �ʱ� ���� �� ��� ����� ������
            FilterProtocolToggles("", tableProtocolList);

            btnAdd.onClick.RemoveAllListeners();
            btnAdd.onClick.AddListener(() =>
            {
                DataTable tblInterface = ClientDatabase.FetchInterfaceData().Tables[0];
                DataTable tblHg = ClientDatabase.FetchHighGroupData().Tables[0];
                DataTable tblLg = ClientDatabase.FetchLowGroupData().Tables[0];
                string id = string.Empty;
                string itype = string.Empty;
                string iid = string.Empty;
                string hgid = string.Empty;
                string lgid = string.Empty;

                foreach (DataRow row in tblInterface.Rows)
                {
                    if (row["INAME"].ToString() == dropdown_Interface.options[dropdown_Interface.value].text)
                    {
                        id = row["ID"].ToString();
                        itype = row["ITYPE"].ToString();
                        iid = row["IID"].ToString();
                    }
                }
                foreach (DataRow row in tblHg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == dropdown_HighGroup.options[dropdown_HighGroup.value].text)
                        hgid = row["FLD_HGID"].ToString();
                }
                foreach (DataRow row in tblLg.Rows)
                {
                    if (row["FLD_NAME"].ToString() == dropdown_LowGroup.options[dropdown_LowGroup.value].text)
                        lgid = row["FLD_LGID"].ToString();
                }

                // ���õ� �������� ��� ã��
                var selectedToggle = protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ���
                if (selectedToggle == null) return; // ���õ� ����� ������ ����
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST���� PKEY, PKEY_ST, PKEY_ED �� ���� ����
                DataRow protocolRow = tableProtocolList.Select($"FW_CODE LIKE '%{fwcode}%'").FirstOrDefault();
                string pkey = protocolRow != null ? protocolRow["KEY"].ToString() : "";
                string pkeySt = protocolRow != null ? protocolRow["START"].ToString() : "";
                string pkeyEd = protocolRow != null ? protocolRow["END"].ToString() : "";


                string selectedCName = inputfield_ControllerName.text;
                string selectedHg = dropdown_HighGroup.options[dropdown_HighGroup.value].text;
                string selectedLg = dropdown_LowGroup.options[dropdown_LowGroup.value].text;
                string selectedInterfacce = dropdown_Interface.options[dropdown_Interface.value].text;
                string selectedEquipNum = inputfield_EquipNum.text;
                string shareState = toggleShare.isOn ? "1" : "0";
                string smsState = toggleSMS.isOn ? "1" : "0";

                // ����ȣ�� ���� �������� Ȯ��
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "����ȣ�� �������·θ� �Է� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID �� ���� ����
                                                                                            //string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID �޼���� �����ؾ� �մϴ�.
                                                                                            // GROUP_ORDER �� ���� ����
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder �޼���� �����ؾ� �մϴ�.

                // ID�� selectedEquipNum�� ���� �ߺ� üũ
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�ش� �������̽��� �̹� ��ϵ�\n����ȣ�� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                    return;
                }

                string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET ID = '{id}', CID = '{selectedEquipNum}', CNAME = '{selectedCName}', PKEY = '{pkey}', PKEY_ST = '{pkeySt}', PKEY_ED = '{pkeyEd}', SMS = '{smsState}', SHARE = '{shareState}', ITYPE = '{itype}', IID = '{iid}', HGID = '{hgid}', LGID = '{lgid}', GROUP_ORDER = '{groupOrder}' WHERE ID = '{id}' AND CID = '{cid}'";


                if (ClientDatabase.OnUpdateRequest(tblControllerQuery))
                {
                    StartCoroutine(RefreshUIAfterCleanup());
                    settingAddController.SetActive(false);

                    // �ʱ�ȭ
                    dropdown_HighGroup.ClearOptions();
                    dropdown_LowGroup.ClearOptions();
                    dropdown_Interface.ClearOptions();
                    toggleShare.isOn = false;
                    toggleSMS.isOn = false;
                    inputfield_Protocol.text = string.Empty;
                    inputfield_EquipNum.text = string.Empty;
                    inputfield_ControllerName.text = string.Empty;

                    //Debug.Log($"��Ʈ�ѷ� ���� �Ϸ� : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "��Ʈ�ѷ� ������ �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                }
            });
        }        
    }

    // ���� ���� �� ���ΰ�ħ
    private IEnumerator RefreshUIAfterCleanup()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            for (int i = f_controllerListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_controllerListContent.transform.GetChild(i).gameObject);
                yield return new WaitForEndOfFrame();
            }
            ObjectPool.Instance.f_CloseControllerSetting();
            f_uiElements.Clear();

            // ���� �����ӱ��� ��ٸ��ϴ�.
            yield return new WaitForEndOfFrame();

            // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
            LoadControllerAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(f_controllerListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            for (int i = controllerListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(controllerListContent.transform.GetChild(i).gameObject);
                yield return new WaitForEndOfFrame();
            }
            ObjectPool.Instance.CloseControllerSetting();
            uiElements.Clear();

            // ���� �����ӱ��� ��ٸ��ϴ�.
            yield return new WaitForEndOfFrame();

            // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
            LoadControllerAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(controllerListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }
}
