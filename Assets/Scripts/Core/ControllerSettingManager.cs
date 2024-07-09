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

    // 첫 실행 후
    [Header("첫 실행 시 컨트롤러 설정요소")]
    public GameObject f_settingController;
    public GameObject f_controllerListScrollView; // 컨트롤러 리스트 스크롤뷰
    public static Transform f_controllerListContent; // 컨트롤러 리스트 스크롤뷰 Content
    private Button f_btnClose;
    private Button f_btnAddController;
    private Button f_btnSave;

    [Header("첫 실행 시 컨트롤러 추가")]
    public GameObject f_settingAddController; // 컨트롤러 추가화면
    public Button f_btnCloseAddController; // 컨트롤러 추가화면 닫기 버튼
    public Button f_btnAdd; // 컨트롤러 추가화면 닫기 버튼
    public TMP_InputField f_inputfield_ControllerName;
    public TMP_Dropdown f_dropdown_HighGroup;
    public TMP_Dropdown f_dropdown_LowGroup;
    public TMP_Dropdown f_dropdown_Interface;
    public TMP_InputField f_inputfield_EquipNum;
    public Toggle f_toggleShare;
    public Toggle f_toggleSMS;
    public TMP_InputField f_inputfield_Protocol;
    public GameObject f_protocolListScrollView; // 프로토콜 리스트 스크롤뷰
    public static Transform f_protocolListContent; // 프로토콜 리스트 스크롤뷰 Content

    // 첫 실행 전
    [Header("컨트롤러 설정요소")]
    public GameObject settingController;
    public GameObject controllerListScrollView; // 컨트롤러 리스트 스크롤뷰
    public static Transform controllerListContent; // 컨트롤러 리스트 스크롤뷰 Content
    private Button btnClose;
    private Button btnAddController;
    private Button btnSave;

    [Header("컨트롤러 추가")]
    public GameObject settingAddController; // 컨트롤러 추가화면
    public Button btnCloseAddController; // 컨트롤러 추가화면 닫기 버튼
    public Button btnAdd; // 컨트롤러 추가화면 닫기 버튼
    public TMP_InputField inputfield_ControllerName;
    public TMP_Dropdown dropdown_HighGroup;
    public TMP_Dropdown dropdown_LowGroup;
    public TMP_Dropdown dropdown_Interface;
    public TMP_InputField inputfield_EquipNum;
    public Toggle toggleShare;
    public Toggle toggleSMS;
    public TMP_InputField inputfield_Protocol;
    public GameObject protocolListScrollView; // 프로토콜 리스트 스크롤뷰
    public static Transform protocolListContent; // 프로토콜 리스트 스크롤뷰 Content
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
        // 첫 실행 시
        f_controllerListContent = f_controllerListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        // 컨트롤러 설정
        f_btnClose = f_settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        f_btnClose.onClick.AddListener(() => CloseControllerSetting());
        f_btnSave = f_settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save").GetComponent<Button>();
        f_btnSave.onClick.AddListener(() => {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "컨트롤러 설정이 완료 되었습니다.";
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

        // 컨트롤러 추가
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

        // 첫 실행 이후
        controllerListContent = controllerListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        // 컨트롤러 설정
        btnClose = settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(() => CloseControllerSetting());
        btnSave = settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save").GetComponent<Button>();
        btnSave.onClick.AddListener(() => {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            screenManager.txt_PopUpMsg.text = "컨트롤러 설정이 완료 되었습니다.";
            screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
            screenManager.btnPopUpConfirm.onClick.AddListener(() => { screenManager.ClosePopUpMessage(); settingController.SetActive(false); });
        });
        btnAddController = settingController.transform.Find("SettingControllerParent/obj_Setting_Controller/AddController/btn_AddController").GetComponent<Button>();
        btnAddController.onClick.AddListener(() => { OpenAddController(); });

        // 컨트롤러 추가
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
        
    // 컨트롤러 추가 화면 열기
    public void OpenAddController()
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];
            DataTable tableProtocolList = ClientDatabase.FetchProtocolList().Tables[0];

            f_settingAddController.SetActive(true);

            // 초기화
            TextMeshProUGUI txtControllerSetTitle = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "컨트롤러 추가";
            TextMeshProUGUI txtBtnControllerModify = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "추가";
            f_inputfield_ControllerName.text = string.Empty;
            f_dropdown_HighGroup.ClearOptions();
            f_dropdown_LowGroup.ClearOptions();
            f_dropdown_Interface.ClearOptions();
            f_inputfield_EquipNum.text = string.Empty;
            f_toggleShare.isOn = false;
            f_toggleSMS.isOn = false;
            f_inputfield_Protocol.text = string.Empty;

            // 드롭다운 목록에 새로운 선택 항목 추가
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // 상위그룹 옵션 추가
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // 인터페이스 옵션 추가
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // 드롭다운에 선택 항목들 설정
            f_dropdown_HighGroup.AddOptions(options_HighGroup);
            f_dropdown_Interface.AddOptions(options_Interface);

            // 첫 번째 항목을 기본 선택으로 설정
            if (f_dropdown_HighGroup.options.Count > 0)
                f_dropdown_HighGroup.value = 0;
            if (f_dropdown_Interface.options.Count > 0)
                f_dropdown_Interface.value = 0;

            // 상위 그룹 드롭다운 변경시 이벤트 리스너
            f_dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });

            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); // 초기 실행

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
                        // 기존 프로토콜 토글 요소가 있으면 내용만 업데이트
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명

                        protocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.f_GetProtocolToggleObject(); // 새로운 프로토콜 토글
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명
                        f_toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // 프로토콜 입력 필드에 onValueChanged 리스너 추가
            f_inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(f_inputfield_Protocol.text, tableProtocolList); });

            // 초기 실행 시 모든 토글을 보여줌
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

                // 선택된 프로토콜 토글 찾기
                var selectedToggle = f_protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // 현재 선택된 토글
                if (selectedToggle == null) return; // 선택된 토글이 없으면 종료
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST에서 PKEY, PKEY_ST, PKEY_ED 값 설정 로직
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

                // 장비번호가 숫자 형식인지 확인
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "장비번호는 숫자형태로만 입력 가능합니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID 값 설정 로직
                //string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID 메서드는 구현해야 합니다.
                // GROUP_ORDER 값 설정 로직
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder 메서드는 구현해야 합니다.

                // ID와 selectedEquipNum에 대해 중복 체크
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "해당 인터페이스에 이미 등록된\n장비번호가 존재합니다.";
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

                    // 초기화
                    f_dropdown_HighGroup.ClearOptions();
                    f_dropdown_LowGroup.ClearOptions();
                    f_dropdown_Interface.ClearOptions();
                    f_inputfield_EquipNum.text = string.Empty;
                    f_toggleShare.isOn = false;
                    f_toggleSMS.isOn = false;
                    f_inputfield_Protocol.text = string.Empty;
                    f_inputfield_ControllerName.text = string.Empty;
                    //txtiName.text = "SERIAL";
                    //Debug.Log($"컨트롤러 추가 완료 : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러 추가에 실패했습니다.";
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

            // 초기화
            TextMeshProUGUI txtControllerSetTitle = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "컨트롤러 추가";
            TextMeshProUGUI txtBtnControllerModify = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "추가";
            inputfield_ControllerName.text = string.Empty;
            dropdown_HighGroup.ClearOptions();
            dropdown_LowGroup.ClearOptions();
            dropdown_Interface.ClearOptions();
            inputfield_EquipNum.text = string.Empty;
            toggleShare.isOn = false;
            toggleSMS.isOn = false;
            inputfield_Protocol.text = string.Empty;

            // 드롭다운 목록에 새로운 선택 항목 추가
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // 상위그룹 옵션 추가
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // 인터페이스 옵션 추가
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // 드롭다운에 선택 항목들 설정
            dropdown_HighGroup.AddOptions(options_HighGroup);
            dropdown_Interface.AddOptions(options_Interface);

            // 첫 번째 항목을 기본 선택으로 설정
            if (dropdown_HighGroup.options.Count > 0)
                dropdown_HighGroup.value = 0;
            if (dropdown_Interface.options.Count > 0)
                dropdown_Interface.value = 0;

            // 상위 그룹 드롭다운 변경시 이벤트 리스너
            dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });

            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); // 초기 실행

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
                        // 기존 프로토콜 토글 요소가 있으면 내용만 업데이트
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명

                        protocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.GetProtocolToggleObject(); // 새로운 프로토콜 토글
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명
                        toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // 프로토콜 입력 필드에 onValueChanged 리스너 추가
            inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(inputfield_Protocol.text, tableProtocolList); });

            // 초기 실행 시 모든 토글을 보여줌
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

                // 선택된 프로토콜 토글 찾기
                var selectedToggle = protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // 현재 선택된 토글
                if (selectedToggle == null) return; // 선택된 토글이 없으면 종료
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST에서 PKEY, PKEY_ST, PKEY_ED 값 설정 로직
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

                // 장비번호가 숫자 형식인지 확인
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "장비번호는 숫자형태로만 입력 가능합니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID 값 설정 로직
                //string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID 메서드는 구현해야 합니다.
                // GROUP_ORDER 값 설정 로직
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder 메서드는 구현해야 합니다.

                // ID와 selectedEquipNum에 대해 중복 체크
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "해당 인터페이스에 이미 등록된\n장비번호가 존재합니다.";
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

                    // 초기화
                    dropdown_HighGroup.ClearOptions();
                    dropdown_LowGroup.ClearOptions();
                    dropdown_Interface.ClearOptions();
                    inputfield_EquipNum.text = string.Empty;
                    toggleShare.isOn = false;
                    toggleSMS.isOn = false;
                    inputfield_Protocol.text = string.Empty;
                    inputfield_ControllerName.text = string.Empty;
                    //txtiName.text = "SERIAL";
                    //Debug.Log($"컨트롤러 추가 완료 : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러 추가에 실패했습니다.";
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

                // 토글 이름 생성 로직을 재사용하여 토글 검색
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (f_toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                {
                    // 검색 텍스트와 일치하거나 검색 텍스트가 빈 문자열인 경우 토글을 보여줌
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

                // 토글 이름 생성 로직을 재사용하여 토글 검색
                string protocolToggleName = $"Toggle_Protocol_{pFWCode}";

                if (toggleElements.TryGetValue(protocolToggleName, out var protocolToggle))
                {
                    // 검색 텍스트와 일치하거나 검색 텍스트가 빈 문자열인 경우 토글을 보여줌
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

                // LowGroup 드롭다운 초기화
                f_dropdown_LowGroup.ClearOptions();
                List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();

                // 일치하는 FLD_HGID를 가진 LowGroup 데이터 추출 및 옵션 추가
                foreach (DataRow row in tableLowGroup.Select($"FLD_HGID = '{selectedHGID}'"))
                {
                    string lgName = row["FLD_NAME"].ToString();
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(lgName);
                    options_LowGroup.Add(option);
                }

                // 드롭다운에 선택 항목들 설정
                f_dropdown_LowGroup.AddOptions(options_LowGroup);

                // 첫 번째 항목을 기본 선택으로 설정
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

                // LowGroup 드롭다운 초기화
                dropdown_LowGroup.ClearOptions();
                List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();

                // 일치하는 FLD_HGID를 가진 LowGroup 데이터 추출 및 옵션 추가
                foreach (DataRow row in tableLowGroup.Select($"FLD_HGID = '{selectedHGID}'"))
                {
                    string lgName = row["FLD_NAME"].ToString();
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(lgName);
                    options_LowGroup.Add(option);
                }

                // 드롭다운에 선택 항목들 설정
                dropdown_LowGroup.AddOptions(options_LowGroup);

                // 첫 번째 항목을 기본 선택으로 설정
                if (dropdown_LowGroup.options.Count > 0)
                    dropdown_LowGroup.value = 0;
            }
        }        
    }

    // 컨트롤러 추가 화면 닫기
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

    // 컨트롤러 설정 닫기
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

    // 컨트롤러 설정 열기
    public void OpenControllerSetting()
    {
        SideMenuManager.Instance.SideMenuStateChange();
        LoadControllerAssets();
    }

    // 인터페이스 로딩
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
                    // 기존 상위그룹 요소가 있으면 내용만 업데이트
                    TextMeshProUGUI TxtControllerName = controllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // 컨트롤러명

                    controllerContainer.name = controllerContainerName;
                    TxtControllerName.text = cname;
                    AddControllerButtonListener(controllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
                }
                else
                {
                    GameObject newControllerContainer = ObjectPool.Instance.f_GetControllerSetContainerObject(); // 새로운 컨트롤러 컨테이너
                    TextMeshProUGUI newTxtControllerName = newControllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // 컨트롤러명
                    f_uiElements[controllerContainerName] = newControllerContainer;

                    newControllerContainer.name = controllerContainerName;
                    newTxtControllerName.text = cname;
                    AddControllerButtonListener(newControllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
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
                    // 기존 상위그룹 요소가 있으면 내용만 업데이트
                    TextMeshProUGUI TxtControllerName = controllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // 컨트롤러명

                    controllerContainer.name = controllerContainerName;
                    TxtControllerName.text = cname;
                    AddControllerButtonListener(controllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
                }
                else
                {
                    GameObject newControllerContainer = ObjectPool.Instance.GetControllerSetContainerObject(); // 새로운 컨트롤러 컨테이너
                    TextMeshProUGUI newTxtControllerName = newControllerContainer.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>(); // 컨트롤러명
                    uiElements[controllerContainerName] = newControllerContainer;

                    newControllerContainer.name = controllerContainerName;
                    newTxtControllerName.text = cname;
                    AddControllerButtonListener(newControllerContainer, id, cid, cname, pkey, pkeySt, pkeyEd, sms, share, itype, iid, alarmMin, hgid, lgid, groupOrder); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
                }
            }
        }        
    }

    // 인터페이스 컨테이너 버튼 리스너
    public void AddControllerButtonListener(GameObject controllerContainer, string id, string cid, string cname, string pkey, string pkeyst, string pkeyed, string sms, string share, string itype, string iid, string alarmmin, string hgid, string lgid, string grouporder)
    {
        Button btnControllerSetting = controllerContainer.transform.Find("btn_Setting").GetComponent<Button>(); // 인터페이스 설정 버튼
        Button btnDeleteController = controllerContainer.transform.Find("btn_Delete").GetComponent<Button>(); // 인터페이스 삭제 버튼

        btnControllerSetting.onClick.RemoveAllListeners();
        btnDeleteController.onClick.RemoveAllListeners();

        btnControllerSetting.onClick.AddListener(() =>
        {
            OpenModifySettingController(id, cid, cname, pkey, pkeyst, pkeyed, sms, share, itype, iid, alarmmin, hgid, lgid, grouporder);
        });
        btnDeleteController.onClick.AddListener(() => DeleteController(controllerContainer, id, cid, cname, pkey, pkeyst, pkeyed, sms, share, itype, iid, alarmmin, hgid, lgid, grouporder));
    }

    // 컨트롤러 삭제
    public void DeleteController(GameObject controllerContainer, string id, string cid, string cname, string pkey, string pkeyst, string pkeyed, string sms, string share, string itype, string iid, string alarmmin, string hgid, string lgid, string grouporder)
    {

        screenManager.CurrentPopUpState = ScreenManager.PopUpState.Delete;
        screenManager.txt_PopUpMsg.text = $"{cname}을(를) 삭제하시겠습니까?";

        screenManager.btnPopUpCancel.onClick.RemoveAllListeners();
        screenManager.btnPopUpCancel.onClick.AddListener(() => screenManager.ClosePopUpMessage());
        screenManager.btnPopUpConfirm.onClick.RemoveAllListeners();
        screenManager.btnPopUpConfirm.onClick.AddListener(() =>
        {

            string tblControllerQuery = $"DELETE FROM TBL_CONTROLLER WHERE `ID` = '{id}' AND `CID` = '{cid}';";
            string tblRealTimeQuery = $"DELETE FROM TBL_REALTIME WHERE `ID` = '{id}' AND `CID` = '{cid}';";

            // 쿼리 실행 함수에 파라미터를 전달하는 방식으로 변경            
            bool controllerDelete = ClientDatabase.OnDeleteRequest(tblControllerQuery);
            bool realtimeDelete = ClientDatabase.OnDeleteRequest(tblRealTimeQuery);

            if (controllerDelete && realtimeDelete)
            {
                //Debug.Log($"컨트롤러 정보 삭제 완료 : {cname}, {id}, {cid}");
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

                // 구성된 키 목록에 따라 인스턴스 삭제
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
                //Debug.Log($"컨트롤러 정보 삭제 실패 : {cname}, {id}, {cid}");
            }
        });

    }

    // 컨트롤러 수정
    public void OpenModifySettingController(string id, string cid, string cname, string pkey, string pkeyst, string pkeyed, string sms, string share, string itype, string iid, string alarmmin, string hgid, string lgid, string grouporder)
    {
        if (screenManager.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableInterface = ClientDatabase.FetchInterfaceData().Tables[0];
            DataTable tableProtocolList = ClientDatabase.FetchProtocolList().Tables[0];

            f_settingAddController.SetActive(true);

            // 초기화
            TextMeshProUGUI txtControllerSetTitle = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "컨트롤러 수정";
            TextMeshProUGUI txtBtnControllerModify = f_settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "수정";
            f_inputfield_ControllerName.text = string.Empty;
            f_dropdown_HighGroup.ClearOptions();
            f_dropdown_LowGroup.ClearOptions();
            f_dropdown_Interface.ClearOptions();
            f_toggleShare.isOn = false;
            f_toggleSMS.isOn = false;
            f_inputfield_Protocol.text = string.Empty;            
            f_inputfield_EquipNum.text = string.Empty;

            // 값 할당
            f_inputfield_ControllerName.text = cname;
            f_inputfield_EquipNum.text = cid;

            // 드롭다운 목록에 새로운 선택 항목 추가
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // 상위그룹 옵션 추가
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // 인터페이스 옵션 추가
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // 드롭다운에 선택 항목들 설정
            f_dropdown_HighGroup.AddOptions(options_HighGroup);
            f_dropdown_Interface.AddOptions(options_Interface);

            // 상위 그룹 드롭다운 변경시 이벤트 리스너
            f_dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });
            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup);// 초기화


            foreach (DataRow row in tableHighGroup.Rows)
            {
                if (row["FLD_HGID"].ToString() == hgid)
                {
                    // 상위그룹 드롭다운의 현재 값을 설정
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
                    // 상위그룹 드롭다운의 현재 값을 설정
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
                    // 인터페이스 드롭다운의 현재 값을 설정
                    int interfaceIndex = options_Interface.FindIndex(option => option.text == row["INAME"].ToString());
                    if (interfaceIndex != -1)
                    {
                        f_dropdown_Interface.value = interfaceIndex;
                    }
                }
            }

            // SMS와 Share 토글 상태 설정
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
                        // 기존 프로토콜 토글 요소가 있으면 내용만 업데이트
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명

                        protocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.f_GetProtocolToggleObject(); // 새로운 프로토콜 토글
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명
                        f_toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = f_protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // 프로토콜 토글 활성화
            foreach (DataRow row in tableProtocolList.Rows)
            {
                if (row["KEY"].ToString() == pkey)
                {
                    string fwcode = row["FW_CODE"].ToString();
                    f_toggleElements[$"Toggle_Protocol_{fwcode}"].GetComponent<Toggle>().isOn = true;
                    break;
                }
            }

            // 프로토콜 입력 필드에 onValueChanged 리스너 추가
            f_inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(f_inputfield_Protocol.text, tableProtocolList); });

            // 초기 실행 시 모든 토글을 보여줌
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

                // 선택된 프로토콜 토글 찾기
                var selectedToggle = f_protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // 현재 선택된 토글
                if (selectedToggle == null) return; // 선택된 토글이 없으면 종료
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST에서 PKEY, PKEY_ST, PKEY_ED 값 설정 로직
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

                // 장비번호가 숫자 형식인지 확인
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "장비번호는 숫자형태로만 입력 가능합니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID 값 설정 로직
                string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID 메서드는 구현해야 합니다.
                                                                               // GROUP_ORDER 값 설정 로직
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder 메서드는 구현해야 합니다.

                // ID와 selectedEquipNum에 대해 중복 체크
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "해당 인터페이스에 이미 등록된\n장비번호가 존재합니다.";
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

                    // 초기화
                    f_dropdown_HighGroup.ClearOptions();
                    f_dropdown_LowGroup.ClearOptions();
                    f_dropdown_Interface.ClearOptions();
                    f_toggleShare.isOn = false;
                    f_toggleSMS.isOn = false;
                    f_inputfield_Protocol.text = string.Empty;
                    f_inputfield_EquipNum.text = string.Empty;
                    f_inputfield_ControllerName.text = string.Empty;

                    //Debug.Log($"컨트롤러 수정 완료 : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러 수정에 실패했습니다.";
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

            // 초기화
            TextMeshProUGUI txtControllerSetTitle = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
            txtControllerSetTitle.text = "컨트롤러 수정";
            TextMeshProUGUI txtBtnControllerModify = settingAddController.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
            txtBtnControllerModify.text = "수정";
            inputfield_ControllerName.text = string.Empty;
            dropdown_HighGroup.ClearOptions();
            dropdown_LowGroup.ClearOptions();
            dropdown_Interface.ClearOptions();
            toggleShare.isOn = false;
            toggleSMS.isOn = false;
            inputfield_Protocol.text = string.Empty;
            inputfield_EquipNum.text = string.Empty;

            // 값 할당
            inputfield_ControllerName.text = cname;
            inputfield_EquipNum.text = cid;

            // 드롭다운 목록에 새로운 선택 항목 추가
            List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_LowGroup = new List<TMP_Dropdown.OptionData>();
            List<TMP_Dropdown.OptionData> options_Interface = new List<TMP_Dropdown.OptionData>();

            // 상위그룹 옵션 추가
            foreach (DataRow row in tableHighGroup.Rows)
            {
                string hgName = row["FLD_NAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
                options_HighGroup.Add(option);
            }
            // 인터페이스 옵션 추가
            foreach (DataRow row in tableInterface.Rows)
            {
                string iName = row["INAME"].ToString();
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(iName);
                options_Interface.Add(option);
            }

            // 드롭다운에 선택 항목들 설정
            dropdown_HighGroup.AddOptions(options_HighGroup);
            dropdown_Interface.AddOptions(options_Interface);

            // 상위 그룹 드롭다운 변경시 이벤트 리스너
            dropdown_HighGroup.onValueChanged.AddListener(delegate { UpdateLowGroupDropdown(tableHighGroup, tableLowGroup); });
            UpdateLowGroupDropdown(tableHighGroup, tableLowGroup);// 초기화


            foreach (DataRow row in tableHighGroup.Rows)
            {
                if (row["FLD_HGID"].ToString() == hgid)
                {
                    // 상위그룹 드롭다운의 현재 값을 설정
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
                    // 상위그룹 드롭다운의 현재 값을 설정
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
                    // 인터페이스 드롭다운의 현재 값을 설정
                    int interfaceIndex = options_Interface.FindIndex(option => option.text == row["INAME"].ToString());
                    if (interfaceIndex != -1)
                    {
                        dropdown_Interface.value = interfaceIndex;
                    }
                }
            }

            // SMS와 Share 토글 상태 설정
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
                        // 기존 프로토콜 토글 요소가 있으면 내용만 업데이트
                        TextMeshProUGUI txtProtocolName = protocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명

                        protocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        protocolToggle.name = protocolToggleName;
                        txtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                    else
                    {
                        GameObject newProtocolToggle = ObjectPool.Instance.GetProtocolToggleObject(); // 새로운 프로토콜 토글
                        TextMeshProUGUI newTxtProtocolName = newProtocolToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>(); // 프로토콜명
                        toggleElements[protocolToggleName] = newProtocolToggle;

                        newProtocolToggle.GetComponent<Toggle>().group = protocolListContent.GetComponent<ToggleGroup>();

                        newProtocolToggle.name = protocolToggleName;
                        newTxtProtocolName.text = $"{pName} Ver.{pVer}";
                    }
                }
            }

            // 프로토콜 토글 활성화
            foreach (DataRow row in tableProtocolList.Rows)
            {
                if (row["KEY"].ToString() == pkey)
                {
                    string fwcode = row["FW_CODE"].ToString();
                    toggleElements[$"Toggle_Protocol_{fwcode}"].GetComponent<Toggle>().isOn = true;
                    break;
                }
            }

            // 프로토콜 입력 필드에 onValueChanged 리스너 추가
            inputfield_Protocol.onValueChanged.AddListener(delegate { FilterProtocolToggles(inputfield_Protocol.text, tableProtocolList); });

            // 초기 실행 시 모든 토글을 보여줌
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

                // 선택된 프로토콜 토글 찾기
                var selectedToggle = protocolListContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // 현재 선택된 토글
                if (selectedToggle == null) return; // 선택된 토글이 없으면 종료
                string fwcode = selectedToggle.name.Replace("Toggle_Protocol_", "");
                // TBL_PROTOCOL_LIST에서 PKEY, PKEY_ST, PKEY_ED 값 설정 로직
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

                // 장비번호가 숫자 형식인지 확인
                if (!int.TryParse(selectedEquipNum, out _))
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "장비번호는 숫자형태로만 입력 가능합니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }

                // CID 값 설정 로직
                                                                                            //string maxCID = (ClientDatabase.GetMaxCID(id) + 1).ToString(); // GetMaxCID 메서드는 구현해야 합니다.
                                                                                            // GROUP_ORDER 값 설정 로직
                string groupOrder = ClientDatabase.GetMaxGroupOrder(hgid, lgid).ToString(); // GetMaxGroupOrder 메서드는 구현해야 합니다.

                // ID와 selectedEquipNum에 대해 중복 체크
                string checkDuplicateQuery = $"SELECT COUNT(*) FROM TBL_CONTROLLER WHERE ID = '{id}' AND CID = '{selectedEquipNum}'";
                int duplicateCount = ClientDatabase.ExecuteScalarQuery(checkDuplicateQuery);

                if (duplicateCount > 0)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "해당 인터페이스에 이미 등록된\n장비번호가 존재합니다.";
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

                    // 초기화
                    dropdown_HighGroup.ClearOptions();
                    dropdown_LowGroup.ClearOptions();
                    dropdown_Interface.ClearOptions();
                    toggleShare.isOn = false;
                    toggleSMS.isOn = false;
                    inputfield_Protocol.text = string.Empty;
                    inputfield_EquipNum.text = string.Empty;
                    inputfield_ControllerName.text = string.Empty;

                    //Debug.Log($"컨트롤러 수정 완료 : {selectedCName} {id} {maxCID}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러 수정에 실패했습니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();

                    });
                }
            });
        }        
    }

    // 설정 변경 후 새로고침
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

            // 다음 프레임까지 기다립니다.
            yield return new WaitForEndOfFrame();

            // 이제 모든 오브젝트가 제거되었으므로 UI를 새로고침합니다.
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

            // 다음 프레임까지 기다립니다.
            yield return new WaitForEndOfFrame();

            // 이제 모든 오브젝트가 제거되었으므로 UI를 새로고침합니다.
            LoadControllerAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(controllerListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }
}
