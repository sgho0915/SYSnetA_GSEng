using System.Collections.Generic;
using UnityEngine;
using E2C;
using System.Data;
using UnityEngine.UI;
using TMPro;
using UI.Dates;

using System;

using LottiePlugin.UI;


public class TrendGraphManager : MonoBehaviour
{
    public GameObject dropdown_Group;       // 그룹선택
    public GameObject dropdown_Controller;  // 컨트롤러 선택
    public GameObject datePicker_StartDate; // 조회 시작일
    public GameObject datePicker_EndDate;   // 조회 종료일
    public GameObject toggle_Hour;          // 시단위 토글(기본값)
    public GameObject toggle_Minute;        // 분단위 토글
    public GameObject trend_Setting;        // 트렌드 설정화면
    public GameObject graphContainer;       // 트렌드 그래프 컨테이너
    public GameObject emptyContainer;       // 첫 조회 전 컨테이너
    public TMP_FontAsset font_NotoSansKR;   // NotoSansKR
    //public GameObject firstImage;           // 첫 조회 전 컨테이너 애니메이션 이미지

    TMP_Dropdown _dropdown_Controller;
    DatePicker _datePicker_StartDate;
    DatePicker _datePicker_EndDate;
    Toggle _toggle_Hour;
    Toggle _toggle_Minute;

    private DataSet controllerData;
    private string interfaceID;
    private string controllerID;
    private string startDate;
    private string endDate;
    private string selectedName;

    // 단순 조회기간 비교를 위한 변수
    private int formattedDate_StartDate;
    private int formattedDate_EndDate;

    bool ret;

    public void OpenTrendSettings()
    {
        trend_Setting.SetActive(true);

        _dropdown_Controller = dropdown_Controller.GetComponent<TMP_Dropdown>();
        _datePicker_StartDate = datePicker_StartDate.GetComponent<DatePicker>();
        _datePicker_EndDate = datePicker_EndDate.GetComponent<DatePicker>();
        _toggle_Hour = toggle_Hour.GetComponent<Toggle>();
        _toggle_Minute = toggle_Minute.GetComponent<Toggle>();

        _dropdown_Controller.options.Clear();
        controllerID = string.Empty;
        interfaceID = string.Empty;
        startDate = string.Empty;
        endDate = string.Empty;
        _toggle_Hour.isOn = true;
        _toggle_Minute.isOn = false;
        selectedName = string.Empty;
        ret = false;


        AddControllerListToDropdown();

        _dropdown_Controller.onValueChanged.AddListener(delegate { DropdownItemSelected(_dropdown_Controller); });
    }

    #region 컨트롤러 드롭다운
    // 컨트롤러 드롭다운 항목을 선택하면 ID와 CID가 저장됨
    private void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        selectedName = dropdown.options[dropdown.value].text;

        foreach (DataRow row in controllerData.Tables[0].Rows)
        {
            if (row["CNAME"].ToString() == selectedName)
            {
                interfaceID = row["ID"].ToString();
                controllerID = row["CID"].ToString();

                //Debug.Log("Selected ID: " + interfaceID + ", Selected CID: " + controllerID);
                break;
            }
        }
    }

    // DataSet controllerData에 존재하는 CNAME 필드 항목들을 dropdown_Controller 드롭다운 리스트로 추가
    public void AddControllerListToDropdown()
    {
        controllerData = ClientDatabase.FetchControllerData();

        TMP_Dropdown _dropdown_Controller = dropdown_Controller.GetComponent<TMP_Dropdown>();

        _dropdown_Controller.options.Clear();

        if (controllerData != null && controllerData.Tables.Count > 0)
        {
            foreach (DataRow row in controllerData.Tables[0].Rows)
            {
                string cname = row["CNAME"].ToString();

                _dropdown_Controller.options.Add(new TMP_Dropdown.OptionData(cname));
            }
        }
        _dropdown_Controller.value = 0;
        _dropdown_Controller.RefreshShownValue();
        DropdownItemSelected(_dropdown_Controller);
    }
    #endregion

    // 트렌드 조회 시작
    public void InquiryTrend()
    {
        string str_StartDate = _datePicker_StartDate.SelectedDate.ToString();
        string str_EndDate = _datePicker_EndDate.SelectedDate.ToString();

        DateTime parsedDate;

        if (DateTime.TryParse(str_StartDate, out parsedDate))
        {
            formattedDate_StartDate = Int32.Parse(parsedDate.ToString("yyyyMMdd"));
            startDate = parsedDate.ToString("yyyy-MM-dd");
        }
        if (DateTime.TryParse(str_EndDate, out parsedDate))
        {
            formattedDate_EndDate = Int32.Parse(parsedDate.ToString("yyyyMMdd"));
            endDate = parsedDate.ToString("yyyy-MM-dd");
        }

        if (interfaceID == null || controllerID == null)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "컨트롤러가 선택되었는지 확인해주세요.";
            ret = false;
        }
        if (_datePicker_StartDate.SelectedDate.ToString() == null || _datePicker_EndDate.SelectedDate.ToString() == null)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "조회 기간을 확인해주세요.";
            ret = false;
        }
        if (formattedDate_StartDate > formattedDate_EndDate)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "조회 기간을 확인해주세요.";
            ret = false;
        }
        if (_toggle_Hour.isOn == false && _toggle_Minute.isOn == false)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "평균 조회 단위를 확인해주세요.";
            ret = false;
        }
        if (_toggle_Hour.isOn == true && _toggle_Minute.isOn == true)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "평균 조회 단위는 한 가지만 선택 가능합니다.";
            ret = false;
        }
        ret = true;
        DrawTrendGraph();
    }

    // 트렌드 그래프 그리기
    public void DrawTrendGraph()
    {
        if (ret)
        {
            trend_Setting.SetActive(false);
            emptyContainer.SetActive(false);

            // 기존의 차트 컴포넌트 찾기
            E2Chart myChart = graphContainer.GetComponent<E2Chart>();

            // 기존의 차트 컴포넌트가 없으면 새로 추가
            if (myChart == null)
            {
                myChart = graphContainer.AddComponent<E2Chart>();
            }
            else
            {
                myChart.Clear();
            }

            // 분 단위 집계에 대한 SQL 쿼리 실행 및 결과 DataSet 저장
            string query = $"SELECT CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(LOG_MINS, 2, '0')) AS Time, ROUND(AVG(DATA0), 2) AS AvgData0, ROUND(AVG(DATA1), 2) AS AvgData1, ROUND(AVG(DATA2), 2) AS AvgData2, ROUND(AVG(DATA3), 2) AS AvgData3, ROUND(AVG(DATA4), 2) AS AvgData4, ROUND(AVG(DATA5), 2) AS AvgData5 FROM TBL_TREND_{interfaceID}_{controllerID} WHERE LOG_DATE BETWEEN '{startDate}' AND '{endDate}' GROUP BY CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(LOG_MINS, 2, '0')) ORDER BY Time";
            DataSet trendDataSet = ClientDatabase.OnSelectRequest(query, $"TBL_TREND_{interfaceID}_{controllerID}");
            //Debug.Log(trendDataSet.Tables[0].Rows.Count);

            // Chart component 추가            
            myChart.chartType = E2Chart.ChartType.LineChart;

            // Chart options 추가
            myChart.chartOptions = myChart.gameObject.AddComponent<E2ChartOptions>();
            myChart.chartOptions.title.enableTitle = true;
            myChart.chartOptions.title.enableSubTitle = false;
            myChart.chartOptions.yAxis.enableTitle = true;
            myChart.chartOptions.label.enable = true;
            myChart.chartOptions.legend.enable = true;
            myChart.chartOptions.chartStyles.barChart.barWidth = 15.0f;
            myChart.chartOptions.plotOptions.mouseTracking = E2ChartOptions.MouseTracking.BySeries;
            myChart.chartOptions.rectOptions.enableZoom = true;
            myChart.chartOptions.label.enable = false;
            myChart.chartOptions.chartStyles.lineChart.pointSize = 5f;
            myChart.chartOptions.xAxis.interval = 120;
            myChart.chartOptions.title.titleTextOption.font = font_NotoSansKR;

            // Chart data 추가
            myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
            myChart.chartData.series = new List<E2ChartData.Series>(); // 시리즈 리스트 초기화
            myChart.chartData.title = $"{selectedName} Trend Data"; // 제목 변경
            myChart.chartData.yAxisTitle = "Values"; // Y축 제목 변경

            // 시간 데이터 추출
            List<string> times = new List<string>();
            foreach (DataRow row in trendDataSet.Tables[0].Rows)
            {
                times.Add(row["Time"].ToString());
            }
            myChart.chartData.categoriesX = times; // X축 카테고리 설정

            // 데이터 시리즈 생성
            for (int i = 0; i < 6; i++) // DATA0부터 DATA5까지
            {
                E2ChartData.Series newSeries = new E2ChartData.Series();
                newSeries.name = "DATA" + i; // 여기 이름으로 치환해야함
                newSeries.dataY = new List<float>();

                foreach (DataRow row in trendDataSet.Tables[0].Rows)
                {
                    newSeries.dataY.Add(float.Parse(row["AvgData" + i].ToString()));
                }

                // 시리즈 리스트에 추가
                myChart.chartData.series.Add(newSeries);
            }

            // 차트 업데이트
            myChart.UpdateChart();
        }
    }
    void LogDataSet(DataSet dataSet)
    {
        foreach (DataTable table in dataSet.Tables)
        {
            //Debug.Log("Table: " + table.TableName);

            // 열의 헤더 출력
            string columnsHeader = "";
            foreach (DataColumn column in table.Columns)
            {
                columnsHeader += column.ColumnName + "\t";
            }
            //Debug.Log(columnsHeader);

            // 각 행의 데이터 출력
            foreach (DataRow row in table.Rows)
            {
                string rowData = "";
                foreach (var item in row.ItemArray)
                {
                    rowData += item.ToString() + "\t";
                }
                //Debug.Log(rowData);
            }
        }
    }
}