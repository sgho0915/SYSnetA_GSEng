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

    [Header("상단메뉴")]
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

    [Header("스크롤뷰 리스트")]
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

    [Header("조회 기간")]
    public GameObject objStartDatePicker;
    public GameObject objEndDatePicker;

    [Header("조회 옵션")]
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
            ShowConfirmPopup("업데이트 준비 중인 기능입니다.");
        });
        btnSaveExcel.onClick.AddListener(() =>
        {
            ShowConfirmPopup("업데이트 준비 중인 기능입니다.");
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
            // 사이드 메뉴를 닫음
            mainRect.offsetMin = new Vector2(0, mainRect.offsetMin.y); // Left 값을 0으로 설정
            objInquirySideMenu.SetActive(false);
        }
        else
        {
            // 사이드 메뉴를 염
            mainRect.offsetMin = new Vector2(210.5562f, mainRect.offsetMin.y); // Left 값을 210.5562으로 설정
            objInquirySideMenu.SetActive(true);
        }
        isOpen = !isOpen; // 메뉴 상태 토글
    }


    #region 사이드메뉴 버튼 핸들러 함수
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

    #region 토글쪽 핸들러 함수
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

        // 설정, 경보이력 제외 활성화 시킬 것
        objTrendElement.SetActive(true);
        objInquiryCondition.SetActive(true);


        btnSaveDaily.gameObject.SetActive(false);
        btnSaveExcel.gameObject.SetActive(false);

        // 초기화상태                
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

        // 초기화
        dropdown_TimeMin.ClearOptions();
        // 옵션항목정의
        List<string> times = new List<string> { "1시간", "30분", "10분", "1분" };
        // 드롭다운 목록에 새로운 선택 항목 추가
        List<TMP_Dropdown.OptionData> options_Time = new List<TMP_Dropdown.OptionData>();
        // baudrate 옵션 추가
        foreach (string time in times)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(time.ToString());
            options_Time.Add(option);
        }
        // 드롭다운에 선택 항목들 설정
        dropdown_TimeMin.AddOptions(options_Time);
        dropdown_TimeMin.onValueChanged.RemoveAllListeners();
        dropdown_TimeMin.onValueChanged.AddListener((value) =>
        {
            DropdownMinChangedValue(value);
        });
        // 첫 번째 항목을 기본 선택으로 설정
        if (dropdown_TimeMin.options.Count > 0)
        {
            dropdown_TimeMin.value = 1;
            dropdown_TimeMin.value = 0;
        }

        int selectedHourValue = 0 ;

        // 선택된 컨트롤러의 리스트를 저장
        List<string> selectedController = new List<string>();
        List<string> selectedControllerPKey = new List<string>();
        foreach (var controllerToggleInstance in controllerToggleInstances)
        {
            controllerToggleInstance.Value.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
            controllerToggleInstance.Value.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
            {
                string[] arrControllerKey = controllerToggleInstance.Value.name.Split("_");
                string cKey = $"{arrControllerKey[3]}_{arrControllerKey[4]}_{controllerToggleInstance.Value.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text}"; // iid, cid, 컨트롤러명 조합
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
                            ScreenManager.Instance.txt_PopUpMsg.text = "동일한 컨트롤러가 아닙니다.";
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


        // 조회 버튼에 대한 리스너 초기화 및 리스너 할당
        btnInquiry.onClick.RemoveAllListeners();
        btnInquiry.onClick.AddListener(() =>
        {         
            // 컨트롤러가 선택되지 않은 경우
            if (selectedController.Count == 0)
            {
                ShowErrorPopup("컨트롤러가 선택되지 않았습니다.");
                return;
            }

            // 선택된 트렌드 항목 추출
            int activeTrendToggles = trendElementToggleInstances.Count(kv => kv.Value.GetComponent<Toggle>().isOn);
            if (activeTrendToggles == 0)
            {
                ShowErrorPopup("트렌드 항목이 선택되지 않았습니다.");
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
            

            // 조회 기간 설정
            DateTime sDate = objStartDatePicker.GetComponent<DatePicker>().SelectedDate;            
            sDate = sDate.Date;
            DateTime eDate = objEndDatePicker.GetComponent<DatePicker>().SelectedDate;
            eDate = eDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            if(sDate > eDate)
            {
                ShowErrorPopup("종료 날짜는 시작 날짜보다 빠를 수 없습니다.");
                return;
            }

            selectedHourValue = dropdown_TimeMin.value;
            // 조회 조건에 따라 각 선택된 컨트롤러에 대한 조회 결과 표현
            ExpressionIniquiryResult(selectedHourValue, isDay, isMonth, isList, isChart, selectedController, selectedTrendElement, sDate, eDate);
            selectedTrendElement.Clear();
        });        
        RefreshSideUI();
    }

    void ExpressionIniquiryResult(int selectedHourValue, bool isDay, bool isMonth, bool isList, bool isChart, List<string> selectedController, List<string> selectedTrendElement, DateTime sDate, DateTime eDate)
    {
        if (!isDay && !isMonth)
        {
            if (isList && !isChart) // 리스트
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (!isList && isChart) // 차트
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (isList && isChart) // 리스트 & 차트
                //Debug.Log("리스트&차트");
            if (!isList && !isChart) // 미선택
            {
                ShowErrorPopup("트렌드 표현 방식이 선택되지 않았습니다.");
                return;
            }
        }

        if (isDay)
        {
            if (isList && !isChart) // 리스트
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (!isList && isChart) // 차트
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (isList && isChart) // 리스트 & 차트
                //Debug.Log("리스트&차트");
            if (!isList && !isChart) // 미선택
            {
                ShowErrorPopup("트렌드 표현 방식이 선택되지 않았습니다.");
                return;
            }
        }

        if (isMonth)
        {
            if (isList && !isChart) // 리스트
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (!isList && isChart) // 차트
                FetchAndMergeTrendData(selectedController, selectedTrendElement, sDate, eDate, selectedHourValue, isDay, isMonth, isList, isChart);
            if (isList && isChart) // 리스트 & 차트
                //Debug.Log("리스트&차트");
            if (!isList && !isChart) // 미선택
            {
                ShowErrorPopup("트렌드 표현 방식이 선택되지 않았습니다.");
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

            // 테이블 존재 여부 확인
            if (!ClientDatabase.TableExists($"TBL_TREND_{iid}_{cid}"))
            {
                ShowErrorPopup("선택한 컨트롤러의 트렌드 데이터가 존재하지 않습니다.");
                return;
            }

            // 분 단위 집계에 대한 SQL 쿼리 실행 및 결과 DataSet 저장
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

                            dataIndex = j; // 데이터 인덱스 찾음
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
                                ShowErrorPopup($"조회된 데이터가 없습니다. 컨트롤러: {selectedController[i]}, 요소: {selectedTrendElement[k]}");
                            }
                        }
                    }
                }
            }
        }

        // DataTable join후 그래프 그리기 시작
        DataTable mergedTable = MergeMultipleDataTablesLINQ(listTrendTables);
        if (Application.platform != RuntimePlatform.Android)
        {
            //Debug.Log(listTrendTables.Count);
            LogDataTableContent(mergedTable);
        }

        if (mergedTable == null || mergedTable.Rows.Count == 0)
        {
            ShowErrorPopup("조회된 데이터가 없습니다.");
            return;
        }

        if(isList)
            UpdateListWithData(mergedTable, unit, listName, listTrendTables.Count);
        if(isChart)
            UpdateChartWithData(mergedTable, unit, chartName, listTrendTables.Count);
    }

    void UpdateListWithData(DataTable mergedTable, string unit, List<string> listName, int mergedTablesCnt)
    {
        // 기존의 차트 컴포넌트 찾기
        //E2Chart myChart = trendContainer.transform.Find("Scroll View/Viewport/Content/GameObject").GetComponent<E2Chart>();
        
        E2Chart myChart = trendContainer.GetComponent<E2Chart>();

        //myChart = trendContainer.transform.Find("Scroll View/Viewport/Content/GameObject").AddComponent<E2Chart>();
        // 기존의 차트 컴포넌트가 없으면 새로 추가
        if (myChart == null)
            myChart = trendContainer.AddComponent<E2Chart>();
        else
            myChart.Clear();

        // Chart component 추가            
        myChart.chartType = E2Chart.ChartType.Table;

        // Chart options 추가
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

        for (int i = 1; i <= mergedTablesCnt; i++)
        {
            // 데이터 시리즈 생성
            E2ChartData.Series newSeries = new E2ChartData.Series();            
            newSeries.name = $"{listName[i - 1]}({unit})"; // 여기 이름으로 치환해야함
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

    void UpdateChartWithData(DataTable mergedTable, string unit, List<string>listName, int mergedTablesCnt)
    {
        // 기존의 차트 컴포넌트 찾기
        E2Chart myChart = trendContainer.GetComponent<E2Chart>();

        // 기존의 차트 컴포넌트가 없으면 새로 추가
        if (myChart == null)
            myChart = trendContainer.AddComponent<E2Chart>();
        else
            myChart.Clear();

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

        for(int i = 1; i <= mergedTablesCnt; i++)
        {
            // 데이터 시리즈 생성
            E2ChartData.Series newSeries = new E2ChartData.Series();
            newSeries.name = $"{listName[i - 1]}"; // 여기 이름으로 치환해야함
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

    public static DataTable MergeMultipleDataTablesLINQ(List<DataTable> dataTables)
    {
        if (dataTables == null || dataTables.Count == 0)
            return null;

        // 첫 번째 DataTable을 기준으로 결과 DataTable 생성
        DataTable result = dataTables[0].Clone(); // 구조만 복사

        // 모든 컬럼에 대해 AvgDataX 형태로 이름 변경 및 추가
        foreach (DataColumn col in result.Columns)
        {
            if (col.ColumnName == "AvgData")
            {
                col.ColumnName = "AvgData1"; // 첫 번째 테이블의 데이터 컬럼 이름 변경
            }
        }

        for (int i = 1; i < dataTables.Count; i++)
        {
            result.Columns.Add($"AvgData{i + 1}", typeof(double)); // 추가 데이터 테이블의 데이터 컬럼 추가
        }

        // 첫 번째 DataTable의 데이터로 결과 DataTable 초기화
        foreach (DataRow row in dataTables[0].Rows)
        {
            var newRow = result.NewRow();
            newRow["TimeGroup"] = row["TimeGroup"];
            newRow["AvgData1"] = row["AvgData"];
            result.Rows.Add(newRow);
        }

        // 나머지 DataTable들을 순차적으로 조인
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

    // mergedTable의 컬럼 구조를 로그로 출력하는 함수
    public static void LogDataTableContent(DataTable dataTable)
    {
        if (dataTable != null && dataTable.Rows.Count > 0)
        {
            // 컬럼 헤더 출력
            string header = "| ";
            foreach (DataColumn column in dataTable.Columns)
            {
                header += $"{column.ColumnName} | ";
            }
            //Debug.Log(header);

            // 각 행의 데이터 출력
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

        // 설정이력에서 숨길 것
        objTrendElement.SetActive(false);
        objInquiryCondition.SetActive(false);
        btnSaveDaily.gameObject.SetActive(false);
        btnSaveExcel.gameObject.SetActive(false);

        // 설정이력에서 고정시킬 것
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
                                ShowErrorPopup("컨트롤러 전체 혹은 개별 선택만 가능합니다.");

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

        // 설정이력에서 숨길 것
        objTrendElement.SetActive(false);
        objInquiryCondition.SetActive(false);
        btnSaveDaily.gameObject.SetActive(false);
        btnSaveExcel.gameObject.SetActive(false);

        // 설정이력에서 고정시킬 것
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
                                ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러 전체 혹은 개별 선택만 가능합니다.";
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

    // 'Element_'로 시작하는 모든 자식 오브젝트를 찾아서 반환
    public List<GameObject> FindChildrenWithPrefix(GameObject obj, string prefix)
    {
        List<GameObject> matchingChildren = new List<GameObject>();

        // 부모 오브젝트의 모든 자식을 순회
        foreach (Transform child in obj.transform)
        {
            // 자식 오브젝트의 이름이 지정된 prefix로 시작하는 경우 리스트에 추가
            if (child.name.StartsWith(prefix))
            {
                matchingChildren.Add(child.gameObject);
            }
        }

        return matchingChildren;
    }
    #endregion

    // 조회 초기화
    public void LoadInquiry()
    {        
        DataTable tblHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
        DataTable tblLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
        DataTable tblController = ClientDatabase.FetchControllerData().Tables[0];

        // 토글 초기화
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

        #region 토글 생성
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

                            // 하위 그룹 토글 이름과 일치하는 lgToggleInstance 찾아서 활성화
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

                            // 하위 그룹 토글 이름과 일치하는 lgToggleInstance 찾아서 비활성화
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

                            // 컨트롤러 토글 이름과 일치하는 controllerToggleInstance 찾아서 활성화
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

                            // 컨트롤러 토글 이름과 일치하는 controllerToggleInstance 찾아서 비활성화
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

    // 프로토콜의 XML 파일을 읽어 해당 트렌드 요소들의 값을 업데이트
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
                        // 트렌드 요소 버튼 인스턴스 생성
                        string trendElementObjName = $"Trend_{pkey}_{addr}";

                        //// 습도 스킵
                        //if (pkey == "UC0224150200401102") // 풀무원 예외처리
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

    // 사이드 메뉴 rect 새로고침
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