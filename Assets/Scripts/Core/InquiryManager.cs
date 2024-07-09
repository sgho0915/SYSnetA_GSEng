using Develop._01GeneralListGUI;
using E2C;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UI.Dates;
using UIWidgets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DatePicker = UI.Dates.DatePicker;



public class InquiryManager : MonoBehaviour
{
    public GameObject inquiryScreen;
    public GameObject togglePrefab;
    public GameObject objInquiryCondition;
    public TMP_FontAsset font_Pretendard_Bold;

    [Header("��ܸ޴�")]
    public Button btnTrend;
    public Button btnLogSet;
    public Button btnLogAlarm;
    public GameObject objTrendSelected;
    public GameObject objTrendNotSelected;
    public GameObject objLogSetSelected;
    public GameObject objLogSetNotSelected;
    public GameObject objLogAlarmSelected;
    public GameObject objLogAlarmNotSelected;
    public GameObject trendContainer;
    public GameObject logSetContainer;
    public GameObject logAlarmContainer;

    [Header("��ũ�Ѻ� ����Ʈ")]
    public Toggle toggleHighGroupSelectAll;
    public Toggle toggleLowGroupSelectAll;
    public Toggle toggleControllerSelectAll;
    public GameObject objHighGroup;
    public GameObject objLowGroup;
    public GameObject objController;
    public GameObject objTrendElement;
    public GameObject hgScrollView;
    public GameObject lgScrollView;
    public GameObject controllerScrollView;
    public GameObject trendElementScrollView;
    public Transform hgContent;
    public Transform lgContent;
    public Transform controllerContent;
    public Transform trendElementContent;

    [Header("��ȸ �Ⱓ")]
    public GameObject objStartDatePicker;
    public GameObject objEndDatePicker;

    [Header("��ȸ �ɼ�")]
    public TMP_Dropdown dropdown_TimeMin;
    public Button btnTimeHour;
    public Button btnTimeDay;
    public Button btnTimeMonth;
    public Button btnList;
    public Button btnChart;
    public Button btnInquiry;
    public Button btnSaveDaily;
    public Button btnSaveExcel;
    public GameObject objMonthSelected;
    public GameObject objMonthNotSelected;
    public GameObject objHourSelected;
    public GameObject objHourNotSelected;
    public GameObject objDaySelected;
    public GameObject objDayNotSelected;
    public GameObject objListSelected;
    public GameObject objListNotSelected;
    public GameObject objChartSelected;
    public GameObject objChartNotSelected;
    private bool is60Min = false;
    private bool is30Min = false;
    private bool is10Min = false;
    private bool is1Min = false;
    private bool isDay = false;
    private bool isMonth = false;
    private bool isList = false;
    private bool isChart = false;

    public static Dictionary<string, GameObject> hgToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> lgToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> controllerToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> trendElementToggleInstances = new Dictionary<string, GameObject>();

    public Button btnInquirySideMenu;
    private bool isOpen = true;
    public GameObject objInquiryMain;
    public GameObject objInquirySideMenu;


    private WebViewObject webViewObject;

    public static InquiryManager Instance { get; private set; }

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

        btnTrend.onClick.RemoveAllListeners();
        btnLogSet.onClick.RemoveAllListeners();
        btnLogAlarm.onClick.RemoveAllListeners();
        btnTrend.onClick.AddListener(() => ChangeTrend());
        btnLogSet.onClick.AddListener(() => ChangeLogSet());
        btnLogAlarm.onClick.AddListener(() => ChangeLogAlarm());

        //btnTimeHour.onClick.RemoveAllListeners();
        btnTimeDay.onClick.RemoveAllListeners();
        btnTimeMonth.onClick.RemoveAllListeners();
        btnList.onClick.RemoveAllListeners();
        btnChart.onClick.RemoveAllListeners();
        btnInquiry.onClick.RemoveAllListeners();
        btnSaveDaily.onClick.RemoveAllListeners();
        btnSaveExcel.onClick.RemoveAllListeners();
        //btnTimeHour.onClick.AddListener(() => BtnHourClick());
        btnTimeDay.onClick.AddListener(() => BtnDayClick());
        btnTimeMonth.onClick.AddListener(() => BtnMonthClick());
        btnList.onClick.AddListener(() =>
        {
            BtnListClick();
        });
        btnChart.onClick.AddListener(() => BtnChartClick());
        
        btnSaveDaily.onClick.AddListener(() =>
        {
            ShowConfirmPopup("������Ʈ �غ� ���� ����Դϴ�.");
        });
        btnSaveExcel.onClick.AddListener(() =>
        {
            ShowConfirmPopup("������Ʈ �غ� ���� ����Դϴ�.");
        });

        btnInquirySideMenu.onClick.RemoveAllListeners();
        btnInquirySideMenu.onClick.AddListener(() =>
        {
            ChangeSideMenuWidth();
        });
    }

    public void ChangeSideMenuWidth()
    {
        RectTransform mainRect = objInquiryMain.GetComponent<RectTransform>();

        if (isOpen)
        {
            // ���̵� �޴��� ����
            mainRect.offsetMin = new Vector2(0, mainRect.offsetMin.y); // Left ���� 0���� ����
            objInquirySideMenu.SetActive(false);
        }
        else
        {
            // ���̵� �޴��� ��
            mainRect.offsetMin = new Vector2(210.5562f, mainRect.offsetMin.y); // Left ���� 210.5562���� ����
            objInquirySideMenu.SetActive(true);
        }
        isOpen = !isOpen; // �޴� ���� ���
    }


    #region ���̵�޴� ��ư �ڵ鷯 �Լ�
    public void BtnMonthClick()
    {
        //objHourSelected.SetActive(false);
        //objHourNotSelected.SetActive(true);
        objMonthSelected.SetActive(true);
        objMonthNotSelected.SetActive(false);
        objDaySelected.SetActive(false);
        objDayNotSelected.SetActive(true);
        dropdown_TimeMin.GetComponent<RoundedCornersX4>().BorderWidth = 0.8f;
        dropdown_TimeMin.GetComponent<Image>().color = new Color(234 / 255f, 234 / 255f, 225 / 255f, 255 / 255f);
        TextMeshProUGUI txtdd = dropdown_TimeMin.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        txtdd.color = new Color(173 / 255f, 173 / 255f, 173 / 255f, 255 / 255f);
        isMonth = true;
        is60Min = false;
        is30Min = false;
        is10Min = false;
        is1Min = false;
        isDay = false;
    }

    public void DropdownMinChangedValue(int ddValue)
    {
        TextMeshProUGUI txtdd = dropdown_TimeMin.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        switch (ddValue)
        {
            case 0:
                isMonth = false;
                is60Min = true;
                is30Min = false;
                is10Min = false;
                is1Min = false;
                isDay = false;
                dropdown_TimeMin.GetComponent<RoundedCornersX4>().BorderWidth = 0;
                txtdd.color = Color.white;
                if (ColorThemeManager.Instance.colorIdx == 1)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorGreen;
                else if (ColorThemeManager.Instance.colorIdx == 2)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlue;
                else if (ColorThemeManager.Instance.colorIdx == 3)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorNavy;
                else if (ColorThemeManager.Instance.colorIdx == 4)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorRed;
                else if (ColorThemeManager.Instance.colorIdx == 5)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlack;
                else if (ColorThemeManager.Instance.colorIdx == 6)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBusungBlue;
                objMonthSelected.SetActive(false);
                objMonthNotSelected.SetActive(true);
                objDaySelected.SetActive(false);
                objDayNotSelected.SetActive(true);
                break;
            case 1:
                isMonth = false;
                is60Min = false;
                is30Min = true;
                is10Min = false;
                is1Min = false;
                isDay = false;
                dropdown_TimeMin.GetComponent<RoundedCornersX4>().BorderWidth = 0;
                txtdd.color = Color.white;
                if (ColorThemeManager.Instance.colorIdx == 1)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorGreen;
                else if (ColorThemeManager.Instance.colorIdx == 2)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlue;
                else if (ColorThemeManager.Instance.colorIdx == 3)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorNavy;
                else if (ColorThemeManager.Instance.colorIdx == 4)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorRed;
                else if (ColorThemeManager.Instance.colorIdx == 5)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlack;
                else if (ColorThemeManager.Instance.colorIdx == 6)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBusungBlue;
                objMonthSelected.SetActive(false);
                objMonthNotSelected.SetActive(true);
                objDaySelected.SetActive(false);
                objDayNotSelected.SetActive(true);
                break;
            case 2:
                isMonth = false;
                is60Min = false;
                is30Min = false;
                is10Min = true;
                is1Min = false;
                isDay = false;
                dropdown_TimeMin.GetComponent<RoundedCornersX4>().BorderWidth = 0;
                txtdd.color = Color.white;
                if (ColorThemeManager.Instance.colorIdx == 1)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorGreen;
                else if (ColorThemeManager.Instance.colorIdx == 2)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlue;
                else if (ColorThemeManager.Instance.colorIdx == 3)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorNavy;
                else if (ColorThemeManager.Instance.colorIdx == 4)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorRed;
                else if (ColorThemeManager.Instance.colorIdx == 5)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlack;
                else if (ColorThemeManager.Instance.colorIdx == 6)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBusungBlue;
                objMonthSelected.SetActive(false);
                objMonthNotSelected.SetActive(true);
                objDaySelected.SetActive(false);
                objDayNotSelected.SetActive(true);
                break;
            case 3:
                isMonth = false;
                is60Min = false;
                is30Min = false;
                is10Min = false;
                is1Min = true;
                isDay = false;
                dropdown_TimeMin.GetComponent<RoundedCornersX4>().BorderWidth = 0;
                txtdd.color = Color.white;
                if (ColorThemeManager.Instance.colorIdx == 1)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorGreen;
                else if (ColorThemeManager.Instance.colorIdx == 2)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlue;
                else if (ColorThemeManager.Instance.colorIdx == 3)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorNavy;
                else if (ColorThemeManager.Instance.colorIdx == 4)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorRed;
                else if (ColorThemeManager.Instance.colorIdx == 5)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBlack;
                else if (ColorThemeManager.Instance.colorIdx == 6)
                    dropdown_TimeMin.GetComponent<Image>().color = ColorThemeManager.Instance.colorBusungBlue;
                objMonthSelected.SetActive(false);
                objMonthNotSelected.SetActive(true);
                objDaySelected.SetActive(false);
                objDayNotSelected.SetActive(true);
                break;
        }
    }

    public void BtnDayClick()
    {
        //objHourSelected.SetActive(false);
        //objHourNotSelected.SetActive(true);
        objMonthSelected.SetActive(false);
        objMonthNotSelected.SetActive(true);
        dropdown_TimeMin.GetComponent<RoundedCornersX4>().BorderWidth = 0.8f;
        dropdown_TimeMin.GetComponent<Image>().color = new Color(234 / 255f, 234 / 255f, 225 / 255f, 255 / 255f);
        TextMeshProUGUI txtdd = dropdown_TimeMin.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        txtdd.color = new Color(173 / 255f, 173 / 255f, 173 / 255f, 255 / 255f);
        objDaySelected.SetActive(true);
        objDayNotSelected.SetActive(false);
        isMonth = false;
        is60Min = false;
        is30Min = false;
        is10Min = false;
        is1Min = false;
        isDay = true;
    }

    public void BtnListClick()
    {
        objListSelected.SetActive(true);
        objListNotSelected.SetActive(false);
        objChartSelected.SetActive(false);
        objChartNotSelected.SetActive(true);
        isList = true;
        isChart = false;
    }

    public void BtnChartClick()
    {
        //if (isChart)
        //{
        //    objChartSelected.SetActive(false);
        //    objChartNotSelected.SetActive(true);
        //}
        //else
        //{
        //    objChartSelected.SetActive(true);
        //    objChartNotSelected.SetActive(false);
        //}
        //isChart = !isChart;

        objListSelected.SetActive(false);
        objListNotSelected.SetActive(true);
        objChartSelected.SetActive(true);
        objChartNotSelected.SetActive(false);
        isList = false;
        isChart = true;
    }
    #endregion

    #region ����� �ڵ鷯 �Լ�
    public void ChangeTrend()
    {
        //BtnHourClick();
        BtnChartClick();
        objTrendSelected.SetActive(true);
        objTrendNotSelected.SetActive(false);
        objLogSetSelected.SetActive(false);
        objLogSetNotSelected.SetActive(true);
        objLogAlarmSelected.SetActive(false);
        objLogAlarmNotSelected.SetActive(true);
        trendContainer.SetActive(true);
        logSetContainer.SetActive(false);
        logAlarmContainer.SetActive(false);

        // ����, �溸�̷� ���� Ȱ��ȭ ��ų ��
        objTrendElement.SetActive(true);
        objInquiryCondition.SetActive(true);


        btnSaveDaily.gameObject.SetActive(false);
        btnSaveExcel.gameObject.SetActive(false);

        // �ʱ�ȭ����                
        if(trendContainer.GetComponent<E2Chart>() != null)
            trendContainer.GetComponent<E2Chart>().Clear();                
        toggleHighGroupSelectAll.interactable = true;
        toggleLowGroupSelectAll.interactable = true;        
        toggleHighGroupSelectAll.isOn = false;
        toggleLowGroupSelectAll.isOn = false;
        toggleControllerSelectAll.isOn = false;
        toggleControllerSelectAll.isOn = false;        
        toggleHighGroupSelectAll.targetGraphic = toggleHighGroupSelectAll.gameObject.transform.Find("Background").GetComponent<Image>();
        toggleLowGroupSelectAll.targetGraphic = toggleLowGroupSelectAll.gameObject.transform.Find("Background").GetComponent<Image>();
        hgScrollView.SetActive(true);
        lgScrollView.SetActive(false);
        controllerScrollView.SetActive(false);
        trendElementScrollView.SetActive(false);

        RefreshSideUI();

        // �ʱ�ȭ
        dropdown_TimeMin.ClearOptions();
        // �ɼ��׸�����
        List<string> times = new List<string> { "1�ð�", "30��", "10��", "1��" };
        // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
        List<TMP_Dropdown.OptionData> options_Time = new List<TMP_Dropdown.OptionData>();
        // baudrate �ɼ� �߰�
        foreach (string time in times)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(time.ToString());
            options_Time.Add(option);
        }
        // ��Ӵٿ ���� �׸�� ����
        dropdown_TimeMin.AddOptions(options_Time);
        dropdown_TimeMin.onValueChanged.RemoveAllListeners();
        dropdown_TimeMin.onValueChanged.AddListener((value) =>
        {
            DropdownMinChangedValue(value);
        });
        // ù ��° �׸��� �⺻ �������� ����
        if (dropdown_TimeMin.options.Count > 0)
        {
            dropdown_TimeMin.value = 1;
            dropdown_TimeMin.value = 0;
        }

        int selectedHourValue = 0 ;

        // ���õ� ��Ʈ�ѷ��� ����Ʈ�� ����
        List<string> selectedController = new List<string>();
        List<string> selectedControllerPKey = new List<string>();
        foreach (var controllerToggleInstance in controllerToggleInstances)
        {
            controllerToggleInstance.Value.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
            controllerToggleInstance.Value.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
            {
                string[] arrControllerKey = controllerToggleInstance.Value.name.Split("_");
                string cKey = $"{arrControllerKey[3]}_{arrControllerKey[4]}_{controllerToggleInstance.Value.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text}"; // iid, cid, ��Ʈ�ѷ��� ����
                string pkey = arrControllerKey[5];
                if (isOn)
                {
                    selectedController.Add(cKey);
                    selectedControllerPKey.Add(pkey);

                    foreach (var selectedPKey in selectedControllerPKey)
                    {
                        if (selectedPKey != pkey)
                        {
                            selectedControllerPKey.Remove(pkey);
                            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            ScreenManager.Instance.txt_PopUpMsg.text = "������ ��Ʈ�ѷ��� �ƴմϴ�.";
                            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                            {
                                ScreenManager.Instance.ClosePopUpMessage();
                                selectedControllerPKey.Clear();
                                foreach (var controllerToggleInstance in controllerToggleInstances)
                                {
                                    controllerToggleInstance.Value.GetComponent<Toggle>().isOn = false;
                                }
                            });
                            
                            break;
                        }
                        else
                        {
                            foreach (var toggle in trendElementToggleInstances)
                            {
                                if (toggle.Value.name.Contains(pkey))
                                {
                                    toggle.Value.SetActive(true);
                                }
                                else
                                {
                                    toggle.Value.SetActive(false);
                                }
                            }
                        }
                    }
                }
                else
                {
                    selectedController.Remove(cKey);
                    selectedControllerPKey.Remove(pkey);
                }

                bool anyControllerIsOn = controllerToggleInstances.Any(kv => kv.Value.GetComponent<Toggle>().isOn);
                trendElementScrollView.SetActive(anyControllerIsOn);
                RefreshSideUI();
            });
        }


        // ��ȸ ��ư�� ���� ������ �ʱ�ȭ �� ������ �Ҵ�
        btnInquiry.onClick.RemoveAllListeners();
        btnInquiry.onClick.AddListener(() =>
        {         
            // ��Ʈ�ѷ��� ���õ��� ���� ���
            if (selectedController.Count == 0)
            {
                ShowErrorPopup("��Ʈ�ѷ��� ���õ��� �ʾҽ��ϴ�.");
                return;
            }

            // ���õ� Ʈ���� �׸� ����
            int activeTrendToggles = trendElementToggleInstances.Count(kv => kv.Value.GetComponent<Toggle>().isOn);
            if (activeTrendToggles == 0)
            {
                ShowErrorPopup("Ʈ���� �׸��� ���õ��� �ʾҽ��ϴ�.");
                return;
            }

            List<string> selectedTrendElement = new List<string>();

            foreach (var kv in trendElementToggleInstances)
            {
                if (kv.Value.GetComponent<Toggle>().isOn)
                {
                    selectedTrendElement.Add(kv.Key);
                    Debug.Log(kv.Key);
                }
            }
            

            // ��ȸ �Ⱓ ����
            DateTime sDate = objStartDatePicker.GetComponent<DatePicker>().SelectedDate;            
            sDate = sDate.Date;
            DateTime eDate = objEndDatePicker.GetComponent<DatePicker>().SelectedDate;
            eDate = eDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            if(sDate > eDate)
            {
                ShowErrorPopup("���� ��¥�� ���� ��¥���� ���� �� �����ϴ�.");
                return;
            }

            selectedHourValue = dropdown_TimeMin.value;
            // ��ȸ ���ǿ� ���� �� ���õ� ��Ʈ�ѷ��� ���� ��ȸ ��� ǥ��
            ExpressionIniquiryResult(selectedHourValue, isDay, isMonth, isList, isChart, selectedController, selectedTrendElement, sDate, eDate);
            selectedTrendElement.Clear();
        });        
        RefreshSideUI();
    }

    void ExpressionIniquiryResult(int selectedHourValue, bool isDay, bool isMonth, bool isList, bool isChart, List<string> selectedController, List<string> selectedTrendElement, DateTime sDate, DateTime eDate)
    {
        if (!isDay && !isMonth)
        {
            if (isList && !isChart) // ����Ʈ
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (!isList && isChart) // ��Ʈ
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (isList && isChart) // ����Ʈ & ��Ʈ
                //Debug.Log("����Ʈ&��Ʈ");
            if (!isList && !isChart) // �̼���
            {
                ShowErrorPopup("Ʈ���� ǥ�� ����� ���õ��� �ʾҽ��ϴ�.");
                return;
            }
        }

        if (isDay)
        {
            if (isList && !isChart) // ����Ʈ
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (!isList && isChart) // ��Ʈ
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (isList && isChart) // ����Ʈ & ��Ʈ
                //Debug.Log("����Ʈ&��Ʈ");
            if (!isList && !isChart) // �̼���
            {
                ShowErrorPopup("Ʈ���� ǥ�� ����� ���õ��� �ʾҽ��ϴ�.");
                return;
            }
        }

        if (isMonth)
        {
            if (isList && !isChart) // ����Ʈ
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (!isList && isChart) // ��Ʈ
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (isList && isChart) // ����Ʈ & ��Ʈ
                //Debug.Log("����Ʈ&��Ʈ");
            if (!isList && !isChart) // �̼���
            {
                ShowErrorPopup("Ʈ���� ǥ�� ����� ���õ��� �ʾҽ��ϴ�.");
                return;
            }
        }       
    }

    void FetchAndMergeTrendData(List<string>selectedController, List<string> selectedTrendElement, DateTime sDate, DateTime eDate, int selectedHourValue, bool isDay, bool isMonth, bool isList, bool isChart)
    {
        List<DataTable> listTrendTables = new List<DataTable>();
        List<string> listName = new List<string>();
        List<string> chartName = new List<string>();
        string unit = string.Empty;
        for (int i = 0; i < selectedController.Count; i++)
        {
            string[] arrSelectedController = selectedController[i].Split("_");
            string iid = arrSelectedController[0];
            string cid = arrSelectedController[1];
            string cname = arrSelectedController[2];

            // ���̺� ���� ���� Ȯ��
            if (!ClientDatabase.TableExists($"TBL_TREND_{iid}_{cid}"))
            {
                ShowErrorPopup("������ ��Ʈ�ѷ��� Ʈ���� �����Ͱ� �������� �ʽ��ϴ�.");
                return;
            }

            // �� ���� ���迡 ���� SQL ���� ���� �� ��� DataSet ����
            string headInfo = string.Empty;
            string findHeadInfoQuery = $"SELECT HEAD_INFO FROM TBL_TREND_{iid}_{cid} ORDER BY LOG_DATE DESC, LOG_HOUR DESC, LOG_MINS DESC LIMIT 1;";
            DataSet headInfoDataSet = ClientDatabase.OnSelectRequest(findHeadInfoQuery, $"TBL_TREND_{iid}_{cid}");
            DataTable table = headInfoDataSet.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                headInfo = row["HEAD_INFO"].ToString();
                //Debug.Log($"DetailView : TBL_TREND_{iid}_{cid}'s latest HEAD_INFO : {headInfo}");

                string[] headInfoParts = headInfo.Split(',');
                int dataIndex = -1;
                for (int j = 0; j < headInfoParts.Length; j++)
                {
                    string[] parts = headInfoParts[j].Split('|');
                    for (int k = 0; k < selectedTrendElement.Count; k++)
                    {
                        string[] arrSelectedTrendPart = selectedTrendElement[k].Split("_");
                        if (parts[1] == $"{arrSelectedTrendPart[2]}")
                        {
                            unit = parts[2];
                            listName.Add($"{cname} {parts[0]}");
                            chartName.Add($"{cname} {parts[0]}");

                            dataIndex = j; // ������ �ε��� ã��
                            //Debug.Log($"DetailView : inquiry modbus address {arrSelectedTrendPart[2]}'s DATA field index : {dataIndex}");

                            string draw60MinGraphQuery = $@"
                                                      SELECT 
                                                          DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0')), '%Y-%m-%d %H:00') AS TimeGroup, 
                                                          AVG(DATA{dataIndex}) AS AvgData 
                                                      FROM 
                                                          TBL_TREND_{iid}_{cid}
                                                      WHERE 
                                                          (LOG_DATE > '{sDate.ToString("yyyy-MM-dd")}' OR 
                                                          (LOG_DATE = '{sDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR >= {sDate.Hour})) AND
                                                          (LOG_DATE < '{eDate.ToString("yyyy-MM-dd")}' OR 
                                                          (LOG_DATE = '{eDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR <= {eDate.Hour}))
                                                      GROUP BY 
                                                          TimeGroup 
                                                      ORDER BY 
                                                          TimeGroup;
                                                      ";

                            string draw30MinGraphQuery = $@"
                                                         SELECT 
                                                             DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(FLOOR(LOG_MINS/30)*30, 2, '0')), '%Y-%m-%d %H:%i') AS TimeGroup, 
                                                             AVG(DATA{dataIndex}) AS AvgData 
                                                         FROM 
                                                             TBL_TREND_{iid}_{cid}
                                                         WHERE 
                                                             (LOG_DATE > '{sDate.ToString("yyyy-MM-dd")}' OR 
                                                             (LOG_DATE = '{sDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR >= {sDate.Hour})) AND
                                                             (LOG_DATE < '{eDate.ToString("yyyy-MM-dd")}' OR 
                                                             (LOG_DATE = '{eDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR <= {eDate.Hour}))
                                                         GROUP BY 
                                                             TimeGroup 
                                                         ORDER BY 
                                                             TimeGroup;
                                                         ";


                            string draw10MinGraphQuery = $@"
                                                         SELECT 
                                                             DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(FLOOR(LOG_MINS/10)*10, 2, '0')), '%Y-%m-%d %H:%i') AS TimeGroup, 
                                                             AVG(DATA{dataIndex}) AS AvgData 
                                                         FROM 
                                                             TBL_TREND_{iid}_{cid}
                                                         WHERE 
                                                             (LOG_DATE > '{sDate.ToString("yyyy-MM-dd")}' OR 
                                                             (LOG_DATE = '{sDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR >= {sDate.Hour})) AND
                                                             (LOG_DATE < '{eDate.ToString("yyyy-MM-dd")}' OR 
                                                             (LOG_DATE = '{eDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR <= {eDate.Hour}))
                                                         GROUP BY 
                                                             TimeGroup 
                                                         ORDER BY 
                                                             TimeGroup;
                                                         ";


                            string draw1MinGraphQuery = $@"
                                                        SELECT 
                                                            DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(LOG_MINS, 2, '0')), '%Y-%m-%d %H:%i') AS TimeGroup, 
                                                            AVG(DATA{dataIndex}) AS AvgData 
                                                        FROM 
                                                            TBL_TREND_{iid}_{cid}
                                                        WHERE 
                                                            (LOG_DATE > '{sDate.ToString("yyyy-MM-dd")}' OR 
                                                            (LOG_DATE = '{sDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR >= {sDate.Hour})) AND
                                                            (LOG_DATE < '{eDate.ToString("yyyy-MM-dd")}' OR 
                                                            (LOG_DATE = '{eDate.ToString("yyyy-MM-dd")}' AND LOG_HOUR <= {eDate.Hour}))
                                                        GROUP BY 
                                                            TimeGroup 
                                                        ORDER BY 
                                                            TimeGroup;
                                                        ";


                            string drawDayGraphQuery = $@"
                                                      SELECT 
                                                          DATE_FORMAT(LOG_DATE, '%Y-%m-%d') AS TimeGroup, 
                                                          AVG(DATA{dataIndex}) AS AvgData 
                                                      FROM 
                                                          TBL_TREND_{iid}_{cid}
                                                      WHERE 
                                                          LOG_DATE >= '{sDate.ToString("yyyy-MM-dd")}' AND
                                                          LOG_DATE <= '{eDate.ToString("yyyy-MM-dd")}'
                                                      GROUP BY 
                                                          TimeGroup 
                                                      ORDER BY 
                                                          TimeGroup;
                                                      ";

                            string drawMonthGraphQuery = $@"
                                                      SELECT 
                                                          DATE_FORMAT(LOG_DATE, '%Y-%m') AS TimeGroup, 
                                                          AVG(DATA{dataIndex}) AS AvgData 
                                                      FROM 
                                                          TBL_TREND_{iid}_{cid}
                                                      WHERE 
                                                          LOG_DATE >= '{sDate.ToString("yyyy-MM-dd")}' AND
                                                          LOG_DATE <= '{eDate.ToString("yyyy-MM-dd")}'
                                                      GROUP BY 
                                                          TimeGroup 
                                                      ORDER BY 
                                                          TimeGroup;
                                                      ";

                            DataTable trendTable = null;
                            if (!isDay && !isMonth)
                            {
                                if (selectedHourValue == 0)
                                    trendTable = ClientDatabase.OnSelectRequest(draw60MinGraphQuery, $"TBL_TREND_{selectedController[i]}").Tables[0];
                                else if (selectedHourValue == 1)
                                    trendTable = ClientDatabase.OnSelectRequest(draw30MinGraphQuery, $"TBL_TREND_{selectedController[i]}").Tables[0];
                                else if (selectedHourValue == 2)
                                    trendTable = ClientDatabase.OnSelectRequest(draw10MinGraphQuery, $"TBL_TREND_{selectedController[i]}").Tables[0];
                                else if (selectedHourValue == 3)
                                    trendTable = ClientDatabase.OnSelectRequest(draw1MinGraphQuery, $"TBL_TREND_{selectedController[i]}").Tables[0];
                            }
                            else if (isDay)
                            {
                                trendTable = ClientDatabase.OnSelectRequest(drawDayGraphQuery, $"TBL_TREND_{selectedController[i]}").Tables[0];
                            }
                            else if (isMonth)
                            {
                                trendTable = ClientDatabase.OnSelectRequest(drawMonthGraphQuery, $"TBL_TREND_{selectedController[i]}").Tables[0];
                            }

                            if (trendTable != null && trendTable.Rows.Count > 0)
                            {
                                listTrendTables.Add(trendTable);
                            }
                            else
                            {
                                ShowErrorPopup($"��ȸ�� �����Ͱ� �����ϴ�. ��Ʈ�ѷ�: {selectedController[i]}, ���: {selectedTrendElement[k]}");
                            }
                        }
                    }
                }
            }
        }

        // DataTable join�� �׷��� �׸��� ����
        DataTable mergedTable = MergeMultipleDataTablesLINQ(listTrendTables);
        if (Application.platform != RuntimePlatform.Android)
        {
            //Debug.Log(listTrendTables.Count);
            LogDataTableContent(mergedTable);
        }

        if (mergedTable == null || mergedTable.Rows.Count == 0)
        {
            ShowErrorPopup("��ȸ�� �����Ͱ� �����ϴ�.");
            return;
        }

        if(isList)
            UpdateListWithData(mergedTable, unit, listName, listTrendTables.Count);
        if(isChart)
            UpdateChartWithData(mergedTable, unit, chartName, listTrendTables.Count);
    }

    void UpdateListWithData(DataTable mergedTable, string unit, List<string> listName, int mergedTablesCnt)
    {
        // ������ ��Ʈ ������Ʈ ã��
        //E2Chart myChart = trendContainer.transform.Find("Scroll View/Viewport/Content/GameObject").GetComponent<E2Chart>();
        
        E2Chart myChart = trendContainer.GetComponent<E2Chart>();

        //myChart = trendContainer.transform.Find("Scroll View/Viewport/Content/GameObject").AddComponent<E2Chart>();
        // ������ ��Ʈ ������Ʈ�� ������ ���� �߰�
        if (myChart == null)
            myChart = trendContainer.AddComponent<E2Chart>();
        else
            myChart.Clear();

        // Chart component �߰�            
        myChart.chartType = E2Chart.ChartType.Table;

        // Chart options �߰�
        myChart.chartOptions = myChart.gameObject.AddComponent<E2ChartOptions>();
        myChart.chartOptions.title.enableTitle = true;
        myChart.chartOptions.title.enableSubTitle = false;
        myChart.chartOptions.title.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.yAxis.enableTitle = false;
        myChart.chartOptions.yAxis.mirrored = true;
        myChart.chartOptions.yAxis.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.yAxis.labelTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.xAxis.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.xAxis.interval = 1;
        myChart.chartOptions.xAxis.enableGridLine = true;        
        myChart.chartOptions.xAxis.labelTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.label.enable = false;
        myChart.chartOptions.legend.enable = false;
        myChart.chartOptions.legend.textOption.font = font_Pretendard_Bold;
        myChart.chartOptions.legend.textOption.fontSize = 16;
        myChart.chartOptions.chartStyles.barChart.barWidth = 15.0f;
        myChart.chartOptions.chartStyles.lineChart.pointSize = 5f;
        myChart.chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
        myChart.chartOptions.rectOptions.enableZoom = true;
        myChart.chartOptions.rectOptions.inverted = true;
        myChart.chartOptions.tooltip.textOption.font = font_Pretendard_Bold;
        myChart.chartOptions.chartStyles.lineChart.splineCurve = true;

        // Chart data �߰�
        myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
        myChart.chartData.series = new List<E2ChartData.Series>(); // �ø��� ����Ʈ �ʱ�ȭ
        myChart.chartData.title = $""; // ���� ����
        myChart.chartData.yAxisTitle = $"���� ��"; // Y�� ���� ����
        myChart.chartData.xAxisTitle = "�ð�";

        // �ð� ������ ����
        List<string> times = new List<string>();
        foreach (DataRow row in mergedTable.Rows)
        {
            times.Add(row["TimeGroup"].ToString());
        }
        myChart.chartData.categoriesX = times; // X�� ī�װ� ����

        for (int i = 1; i <= mergedTablesCnt; i++)
        {
            // ������ �ø��� ����
            E2ChartData.Series newSeries = new E2ChartData.Series();            
            newSeries.name = $"{listName[i - 1]}({unit})"; // ���� �̸����� ġȯ�ؾ���
            newSeries.dataY = new List<float>();
            foreach (DataRow row in mergedTable.Rows)
            {
                newSeries.dataY.Add(float.Parse(row[$"AvgData{i}"].ToString()));
            }
            // �ø��� ����Ʈ�� �߰�
            myChart.chartData.series.Add(newSeries);
        }

        // ��Ʈ ������Ʈ
        myChart.UpdateChart();
    }

    void UpdateChartWithData(DataTable mergedTable, string unit, List<string>listName, int mergedTablesCnt)
    {
        // ������ ��Ʈ ������Ʈ ã��
        E2Chart myChart = trendContainer.GetComponent<E2Chart>();

        // ������ ��Ʈ ������Ʈ�� ������ ���� �߰�
        if (myChart == null)
            myChart = trendContainer.AddComponent<E2Chart>();
        else
            myChart.Clear();

        // Chart component �߰�            
        myChart.chartType = E2Chart.ChartType.LineChart;

        // Chart options �߰�
        myChart.chartOptions = myChart.gameObject.AddComponent<E2ChartOptions>();
        myChart.chartOptions.title.enableTitle = true;
        myChart.chartOptions.title.enableSubTitle = false;
        myChart.chartOptions.title.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.yAxis.enableTitle = true;
        myChart.chartOptions.yAxis.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.yAxis.labelTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.xAxis.titleTextOption.font = font_Pretendard_Bold;
        myChart.chartOptions.xAxis.enableLabel = false;
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

        // Chart data �߰�
        myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
        myChart.chartData.series = new List<E2ChartData.Series>(); // �ø��� ����Ʈ �ʱ�ȭ
        myChart.chartData.title = $""; // ���� ����
        myChart.chartData.yAxisTitle = $"���� ��"; // Y�� ���� ����
        myChart.chartData.xAxisTitle = "�ð�";

        // �ð� ������ ����
        List<string> times = new List<string>();
        foreach (DataRow row in mergedTable.Rows)
        {
            times.Add(row["TimeGroup"].ToString());
        }
        myChart.chartData.categoriesX = times; // X�� ī�װ� ����

        for(int i = 1; i <= mergedTablesCnt; i++)
        {
            // ������ �ø��� ����
            E2ChartData.Series newSeries = new E2ChartData.Series();
            newSeries.name = $"{listName[i - 1]}"; // ���� �̸����� ġȯ�ؾ���
            newSeries.dataY = new List<float>();
            foreach (DataRow row in mergedTable.Rows)
            {
                newSeries.dataY.Add(float.Parse(row[$"AvgData{i}"].ToString()));
            }
            // �ø��� ����Ʈ�� �߰�
            myChart.chartData.series.Add(newSeries);
        }

        // ��Ʈ ������Ʈ
        myChart.UpdateChart();
    }

    public static DataTable MergeMultipleDataTablesLINQ(List<DataTable> dataTables)
    {
        if (dataTables == null || dataTables.Count == 0)
            return null;

        // ù ��° DataTable�� �������� ��� DataTable ����
        DataTable result = dataTables[0].Clone(); // ������ ����

        // ��� �÷��� ���� AvgDataX ���·� �̸� ���� �� �߰�
        foreach (DataColumn col in result.Columns)
        {
            if (col.ColumnName == "AvgData")
            {
                col.ColumnName = "AvgData1"; // ù ��° ���̺��� ������ �÷� �̸� ����
            }
        }

        for (int i = 1; i < dataTables.Count; i++)
        {
            result.Columns.Add($"AvgData{i + 1}", typeof(double)); // �߰� ������ ���̺��� ������ �÷� �߰�
        }

        // ù ��° DataTable�� �����ͷ� ��� DataTable �ʱ�ȭ
        foreach (DataRow row in dataTables[0].Rows)
        {
            var newRow = result.NewRow();
            newRow["TimeGroup"] = row["TimeGroup"];
            newRow["AvgData1"] = row["AvgData"];
            result.Rows.Add(newRow);
        }

        // ������ DataTable���� ���������� ����
        for (int i = 1; i < dataTables.Count; i++)
        {
            var nextTable = dataTables[i];

            var query = from row in result.AsEnumerable()
                        join nextRow in nextTable.AsEnumerable() on row.Field<string>("TimeGroup") equals nextRow.Field<string>("TimeGroup")
                        select new { Row = row, AvgData = nextRow.Field<double>("AvgData") };

            foreach (var item in query)
            {
                item.Row[$"AvgData{i + 1}"] = item.AvgData;
            }
        }

        return result;
    }

    void ShowErrorPopup(string message)
    {
        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
        ScreenManager.Instance.txt_PopUpMsg.text = message;
        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
    }

    void ShowConfirmPopup(string message)
    {
        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
        ScreenManager.Instance.txt_PopUpMsg.text = message;
        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
    }

    // mergedTable�� �÷� ������ �α׷� ����ϴ� �Լ�
    public static void LogDataTableContent(DataTable dataTable)
    {
        if (dataTable != null && dataTable.Rows.Count > 0)
        {
            // �÷� ��� ���
            string header = "| ";
            foreach (DataColumn column in dataTable.Columns)
            {
                header += $"{column.ColumnName} | ";
            }
            //Debug.Log(header);

            // �� ���� ������ ���
            foreach (DataRow row in dataTable.Rows)
            {
                string rowString = "| ";
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowString += $"{row[column]} | ";
                }
                //Debug.Log(rowString);
            }
        }
        else
        {
            //Debug.Log("DataTable is null or empty.");
        }
    }

    private void ChangeLogSet()
    {
        string selectedIID = "all";
        string selectedCID = "all";
        

        objTrendSelected.SetActive(false);
        objTrendNotSelected.SetActive(true);
        objLogSetSelected.SetActive(true);
        objLogSetNotSelected.SetActive(false);
        objLogAlarmSelected.SetActive(false);
        objLogAlarmNotSelected.SetActive(true);
        trendContainer.SetActive(false);
        logSetContainer.SetActive(true);
        logAlarmContainer.SetActive(false);

        // �����̷¿��� ���� ��
        objTrendElement.SetActive(false);
        objInquiryCondition.SetActive(false);
        btnSaveDaily.gameObject.SetActive(false);
        btnSaveExcel.gameObject.SetActive(false);

        // �����̷¿��� ������ų ��
        toggleHighGroupSelectAll.isOn = true;
        toggleLowGroupSelectAll.isOn = true;
        toggleHighGroupSelectAll.interactable = false;
        toggleLowGroupSelectAll.interactable = false;
        toggleHighGroupSelectAll.targetGraphic = toggleHighGroupSelectAll.gameObject.transform.Find("Background/Checkmark").GetComponent<Image>();
        toggleLowGroupSelectAll.targetGraphic = toggleLowGroupSelectAll.gameObject.transform.Find("Background/Checkmark").GetComponent<Image>();
        toggleControllerSelectAll.isOn = true;
        controllerScrollView.gameObject.SetActive(false);

        toggleControllerSelectAll.onValueChanged.RemoveAllListeners();
        toggleControllerSelectAll.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                foreach (var toggle in controllerToggleInstances)
                {
                    toggle.Value.GetComponent<Toggle>().isOn = true;
                    toggle.Value.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                }
                controllerScrollView.gameObject.SetActive(false);

                selectedIID = "all";
                selectedCID = "all";
            }
            else
            {
                foreach (var toggle in controllerToggleInstances)
                {
                    toggle.Value.GetComponent<Toggle>().isOn = false;
                    toggle.Value.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                    toggle.Value.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                    {
                        if (!toggleControllerSelectAll.isOn)
                        {
                            int activeControllerToggles = controllerToggleInstances.Count(kv => kv.Value.GetComponent<Toggle>().isOn);
                            if (activeControllerToggles > 1)
                            {
                                ShowErrorPopup("��Ʈ�ѷ� ��ü Ȥ�� ���� ���ø� �����մϴ�.");

                                foreach (var toggle in controllerToggleInstances)
                                {
                                    toggle.Value.GetComponent<Toggle>().isOn = false;
                                }
                                selectedIID = "none";
                                selectedCID = "none";
                                return;
                            }
                        }
                        string[] strName = toggle.Value.name.Split('_');
                        string iid = strName[3];
                        string cid = strName[4];

                        selectedIID = iid;
                        selectedCID = cid;
                    });
                }
                controllerScrollView.gameObject.SetActive(true);
            }
            RefreshSideUI();
        });


        hgScrollView.gameObject.SetActive(false);
        lgScrollView.gameObject.SetActive(false);

        //objStartDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today - TimeSpan.FromDays(7);
        objStartDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today;
        objEndDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today;

        RefreshSideUI();
        btnInquiry.onClick.RemoveAllListeners();
        btnInquiry.onClick.AddListener(() => {
            InquiryControlData.Instance.InquirySpecificControl(selectedIID, selectedCID);
        });
    }

    private void ChangeLogAlarm()
    {
        string selectedIID = "all";
        string selectedCID = "all";

        objTrendSelected.SetActive(false);
        objTrendNotSelected.SetActive(true);
        objLogSetSelected.SetActive(false);
        objLogSetNotSelected.SetActive(true);
        objLogAlarmSelected.SetActive(true);
        objLogAlarmNotSelected.SetActive(false);
        trendContainer.SetActive(false);
        logSetContainer.SetActive(false);
        logAlarmContainer.SetActive(true);

        // �����̷¿��� ���� ��
        objTrendElement.SetActive(false);
        objInquiryCondition.SetActive(false);
        btnSaveDaily.gameObject.SetActive(false);
        btnSaveExcel.gameObject.SetActive(false);

        // �����̷¿��� ������ų ��
        toggleHighGroupSelectAll.isOn = true;
        toggleLowGroupSelectAll.isOn = true;
        toggleHighGroupSelectAll.interactable = false;
        toggleLowGroupSelectAll.interactable = false;
        toggleHighGroupSelectAll.targetGraphic = toggleHighGroupSelectAll.gameObject.transform.Find("Background/Checkmark").GetComponent<Image>();
        toggleLowGroupSelectAll.targetGraphic = toggleLowGroupSelectAll.gameObject.transform.Find("Background/Checkmark").GetComponent<Image>();
        toggleControllerSelectAll.isOn = true;
        controllerScrollView.gameObject.SetActive(false);

        toggleControllerSelectAll.onValueChanged.RemoveAllListeners();
        toggleControllerSelectAll.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                foreach(var toggle in controllerToggleInstances)
                {
                    toggle.Value.GetComponent<Toggle>().isOn = true;
                    toggle.Value.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                }
                controllerScrollView.gameObject.SetActive(false);
                
                selectedIID = "all";
                selectedCID = "all";
            }
            else
            {
                foreach (var toggle in controllerToggleInstances)
                {
                    toggle.Value.GetComponent<Toggle>().isOn = false;
                    toggle.Value.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                    toggle.Value.GetComponent<Toggle>().onValueChanged.AddListener((value) =>
                    {
                        if (!toggleControllerSelectAll.isOn)
                        {
                            int activeControllerToggles = controllerToggleInstances.Count(kv => kv.Value.GetComponent<Toggle>().isOn);
                            if (activeControllerToggles > 1)
                            {
                                webViewObject.SetVisibility(false);
                                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                ScreenManager.Instance.txt_PopUpMsg.text = "��Ʈ�ѷ� ��ü Ȥ�� ���� ���ø� �����մϴ�.";
                                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
                                    ScreenManager.Instance.ClosePopUpMessage();
                                    webViewObject.SetVisibility(true);
                                });

                                foreach (var toggle in controllerToggleInstances)
                                {
                                    toggle.Value.GetComponent<Toggle>().isOn = false;                                    
                                }
                                selectedIID = "none";
                                selectedCID = "none";
                                return;
                            }
                        }
                        string[] strName = toggle.Value.name.Split('_');
                        string iid = strName[3];
                        string cid = strName[4];

                        selectedIID = iid;
                        selectedCID = cid;
                    });
                }
                controllerScrollView.gameObject.SetActive(true);
            }
            RefreshSideUI();
        });

        
        hgScrollView.gameObject.SetActive(false);
        lgScrollView.gameObject.SetActive(false);

        objStartDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today;
        objEndDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today;

        RefreshSideUI();
        btnInquiry.onClick.RemoveAllListeners();
        btnInquiry.onClick.AddListener(() => {
            InquiryAlarmData.Instance.InquirySpecificAlarm(selectedIID, selectedCID);
        } );
    }

    // 'Element_'�� �����ϴ� ��� �ڽ� ������Ʈ�� ã�Ƽ� ��ȯ
    public List<GameObject> FindChildrenWithPrefix(GameObject obj, string prefix)
    {
        List<GameObject> matchingChildren = new List<GameObject>();

        // �θ� ������Ʈ�� ��� �ڽ��� ��ȸ
        foreach (Transform child in obj.transform)
        {
            // �ڽ� ������Ʈ�� �̸��� ������ prefix�� �����ϴ� ��� ����Ʈ�� �߰�
            if (child.name.StartsWith(prefix))
            {
                matchingChildren.Add(child.gameObject);
            }
        }

        return matchingChildren;
    }
    #endregion

    // ��ȸ �ʱ�ȭ
    public void LoadInquiry()
    {        
        DataTable tblHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
        DataTable tblLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
        DataTable tblController = ClientDatabase.FetchControllerData().Tables[0];

        // ��� �ʱ�ȭ
        foreach (var hgToggleInstance in hgToggleInstances)
        {
            Destroy(hgToggleInstance.Value);
        }
        foreach (var lgToggleInstance in lgToggleInstances)
        {
            Destroy(lgToggleInstance.Value);
        }
        foreach (var controllerToggleInstance in controllerToggleInstances)
        {
            Destroy(controllerToggleInstance.Value);
        }
        foreach (var trendElementToggleInstance in trendElementToggleInstances)
        {
            Destroy(trendElementToggleInstance.Value);
        }
        hgToggleInstances.Clear();
        lgToggleInstances.Clear();
        controllerToggleInstances.Clear();
        trendElementToggleInstances.Clear();
        toggleHighGroupSelectAll.isOn = false;
        toggleLowGroupSelectAll.isOn = false;
        toggleControllerSelectAll.isOn = false;        

        hgScrollView.SetActive(true);
        lgScrollView.SetActive(false);
        controllerScrollView.SetActive(false);
        trendElementScrollView.SetActive(false);
        is60Min = false;
        isDay = false;
        isMonth = false;
        isList = false;
        isChart = false;

        objStartDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today;
        objEndDatePicker.GetComponent<DatePicker>().SelectedDate = DateTime.Today;

        #region ��� ����
        foreach (DataRow row in tblHighGroup.Rows)
        {
            string hgid = row["FLD_HGID"].ToString();
            string hgName = row["FLD_NAME"].ToString();
            string hgToggleName = $"HighGroupToggle_{hgid}";

            if (!hgToggleInstances.ContainsKey(hgToggleName))
            {
                GameObject hgToggleInstance = Instantiate(togglePrefab, hgContent);
                hgToggleInstance.name = hgToggleName;
                hgToggleInstances[hgToggleName] = hgToggleInstance;

                TextMeshProUGUI txtHGName = hgToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                txtHGName.text = hgName;

                hgToggleInstance.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                hgToggleInstance.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        lgScrollView.SetActive(true);
                        RefreshSideUI();
                        DataRow[] selectedRows = tblLowGroup.Select($"FLD_HGID = '{hgid}'");

                        foreach (DataRow selectedRow in selectedRows)
                        {
                            string lgName = selectedRow["FLD_NAME"].ToString();
                            string lgToggleName = $"LowGroupToggle_{hgid}_{selectedRow["FLD_LGID"].ToString()}";

                            // ���� �׷� ��� �̸��� ��ġ�ϴ� lgToggleInstance ã�Ƽ� Ȱ��ȭ
                            if (lgToggleInstances.TryGetValue(lgToggleName, out GameObject lgToggleInstance))
                            {
                                lgToggleInstance.SetActive(true);
                                lgToggleInstance.GetComponent<Toggle>().isOn = false;
                            }
                        }
                    }
                    else
                    {
                        DataRow[] selectedRows = tblLowGroup.Select($"FLD_HGID = '{hgid}'");

                        foreach (DataRow selectedRow in selectedRows)
                        {
                            string lgName = selectedRow["FLD_NAME"].ToString();
                            string lgToggleName = $"LowGroupToggle_{hgid}_{selectedRow["FLD_LGID"].ToString()}";

                            // ���� �׷� ��� �̸��� ��ġ�ϴ� lgToggleInstance ã�Ƽ� ��Ȱ��ȭ
                            if (lgToggleInstances.TryGetValue(lgToggleName, out GameObject lgToggleInstance))
                            {
                                lgToggleInstance.GetComponent<Toggle>().isOn = false;
                                lgToggleInstance.SetActive(false);
                            }
                        }
                    }
                    RefreshSideUI();
                });
            }
        }

        foreach (DataRow row in tblLowGroup.Rows)
        {
            string hgid = row["FLD_HGID"].ToString();
            string lgid = row["FLD_LGID"].ToString();
            string lgName = row["FLD_NAME"].ToString();
            string lgToggleName = $"LowGroupToggle_{hgid}_{lgid}";

            if (!lgToggleInstances.ContainsKey(lgToggleName))
            {
                GameObject lgToggleInstance = Instantiate(togglePrefab, lgContent);
                lgToggleInstance.name = lgToggleName;
                lgToggleInstances[lgToggleName] = lgToggleInstance;

                TextMeshProUGUI txtLGName = lgToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                txtLGName.text = lgName;
                lgToggleInstance.SetActive(false);

                lgToggleInstance.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                lgToggleInstance.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        controllerScrollView.SetActive(true);
                        RefreshSideUI();
                        DataRow[] selectedRows = tblController.Select($"HGID = '{hgid}' AND LGID = '{lgid}'");

                        foreach (DataRow selectedRow in selectedRows)
                        {
                            string controllerName = selectedRow["CNAME"].ToString();
                            string iid = selectedRow["ID"].ToString();
                            string cid = selectedRow["CID"].ToString();
                            string pkey = selectedRow["PKEY"].ToString();
                            string controllerToggleName = $"ControllerToggle_{hgid}_{selectedRow["LGID"].ToString()}_{iid}_{cid}_{pkey}";

                            // ��Ʈ�ѷ� ��� �̸��� ��ġ�ϴ� controllerToggleInstance ã�Ƽ� Ȱ��ȭ
                            if (controllerToggleInstances.TryGetValue(controllerToggleName, out GameObject controllerToggleInstance))
                            {
                                controllerToggleInstance.SetActive(true);
                                controllerToggleInstance.GetComponent<Toggle>().isOn = false;
                            }
                        }
                    }
                    else
                    {
                        DataRow[] selectedRows = tblController.Select($"HGID = '{hgid}' AND LGID = '{lgid}'");

                        foreach (DataRow selectedRow in selectedRows)
                        {
                            string controllerName = selectedRow["CNAME"].ToString();
                            string iid = selectedRow["ID"].ToString();
                            string cid = selectedRow["CID"].ToString();
                            string pkey = selectedRow["PKEY"].ToString();
                            string controllerToggleName = $"ControllerToggle_{hgid}_{selectedRow["LGID"].ToString()}_{iid}_{cid}_{pkey}";

                            // ��Ʈ�ѷ� ��� �̸��� ��ġ�ϴ� controllerToggleInstance ã�Ƽ� ��Ȱ��ȭ
                            if (controllerToggleInstances.TryGetValue(controllerToggleName, out GameObject controllerToggleInstance))
                            {
                                controllerToggleInstance.GetComponent<Toggle>().isOn = false;
                                controllerToggleInstance.SetActive(false);
                            }
                        }
                    }
                });
            }
        }

        foreach (DataRow row in tblController.Rows)
        {
            string iid = row["ID"].ToString();
            string cid = row["CID"].ToString();
            string hgid = row["HGID"].ToString();
            string lgid = row["LGID"].ToString();
            string cname = row["CNAME"].ToString();
            string pkey = row["PKEY"].ToString();
            string controllerToggleName = $"ControllerToggle_{hgid}_{lgid}_{iid}_{cid}_{pkey}";

            if (!controllerToggleInstances.ContainsKey(controllerToggleName))
            {
                GameObject controllerToggleInstance = Instantiate(togglePrefab, controllerContent);
                controllerToggleInstance.name = controllerToggleName;
                controllerToggleInstances[controllerToggleName] = controllerToggleInstance;

                TextMeshProUGUI txtControllerName = controllerToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                txtControllerName.text = cname;

                ProcessingTrendDataWithXML(iid, cid, pkey);
            }
        }        
        #endregion

        toggleHighGroupSelectAll.onValueChanged.AddListener(isOn =>
        {
            foreach (var toggle in hgToggleInstances.Values)
            {
                toggle.GetComponent<Toggle>().isOn = isOn;
            }
            if (!isOn)
            {
                toggleLowGroupSelectAll.isOn = false;
            }
        });

        toggleLowGroupSelectAll.onValueChanged.AddListener(isOn =>
        {
            foreach (var toggle in lgToggleInstances.Values)
            {
                toggle.GetComponent<Toggle>().isOn = isOn;
            }
            if (!isOn)
            {
                toggleControllerSelectAll.isOn = false;
            }
        });

        toggleControllerSelectAll.onValueChanged.AddListener(isOn =>
        {
            foreach (var toggle in controllerToggleInstances.Values)
            {
                toggle.GetComponent<Toggle>().isOn = isOn;
            }
        });

        ChangeTrend();
        RefreshSideUI();
    }

    // ���������� XML ������ �о� �ش� Ʈ���� ��ҵ��� ���� ������Ʈ
    private void ProcessingTrendDataWithXML(string iid, string cid, string pkey)
    {
        XMLParser.Instance.GetXML(pkey);
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> attributes = XMLParser.Instance.GetTrendAltAttributes(XMLParser.Instance.xmlContent);        

        try
        {
            foreach (var group in attributes)
            {
                foreach (var tag in group.Value)
                {
                    string tagName = tag.Key;

                    if (tag.Value.TryGetValue("addr", out var addr) && tag.Value.TryGetValue("name", out var name) && tag.Value.TryGetValue("unit", out var unit) && tag.Value.TryGetValue("multiply", out var multiply))
                    {
                        // Ʈ���� ��� ��ư �ν��Ͻ� ����
                        string trendElementObjName = $"Trend_{pkey}_{addr}";

                        //// ���� ��ŵ
                        //if (pkey == "UC0224150200401102") // Ǯ���� ����ó��
                        //{
                        //    if (addr == "223" || addr == "236" || addr == "233" || addr == "234" ||
                        //        addr == "242" || addr == "245" || addr == "248" || addr == "251" || addr == "254" ||
                        //        addr == "215" || addr == "216" || addr == "217" || addr == "218")
                        //        continue;
                        //}

                        if (!trendElementToggleInstances.ContainsKey(trendElementObjName))
                        {
                            GameObject trendToggleInstance = Instantiate(togglePrefab, trendElementContent);
                            trendToggleInstance.name = trendElementObjName;
                            trendElementToggleInstances[trendElementObjName] = trendToggleInstance;
                                                        
                            TextMeshProUGUI txtTrendName = trendToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                            txtTrendName.text = name;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError($"DetailView : Exception in ProcessingTrendDataWithXML: {ex}");
        }
    }

    // ���̵� �޴� rect ���ΰ�ħ
    private void RefreshSideUI()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(objHighGroup.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(objLowGroup.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(objController.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(objTrendElement.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(inquiryScreen.transform.Find("InquirySide/Center/Scroll View/Viewport/Content").GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }
}