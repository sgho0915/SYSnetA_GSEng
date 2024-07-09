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
    public GameObject dropdown_Group;       // �׷켱��
    public GameObject dropdown_Controller;  // ��Ʈ�ѷ� ����
    public GameObject datePicker_StartDate; // ��ȸ ������
    public GameObject datePicker_EndDate;   // ��ȸ ������
    public GameObject toggle_Hour;          // �ô��� ���(�⺻��)
    public GameObject toggle_Minute;        // �д��� ���
    public GameObject trend_Setting;        // Ʈ���� ����ȭ��
    public GameObject graphContainer;       // Ʈ���� �׷��� �����̳�
    public GameObject emptyContainer;       // ù ��ȸ �� �����̳�
    public TMP_FontAsset font_NotoSansKR;   // NotoSansKR
    //public GameObject firstImage;           // ù ��ȸ �� �����̳� �ִϸ��̼� �̹���

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

    // �ܼ� ��ȸ�Ⱓ �񱳸� ���� ����
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

    #region ��Ʈ�ѷ� ��Ӵٿ�
    // ��Ʈ�ѷ� ��Ӵٿ� �׸��� �����ϸ� ID�� CID�� �����
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

    // DataSet controllerData�� �����ϴ� CNAME �ʵ� �׸���� dropdown_Controller ��Ӵٿ� ����Ʈ�� �߰�
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

    // Ʈ���� ��ȸ ����
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
            ScreenManager.Instance.txt_PopUpMsg.text = "��Ʈ�ѷ��� ���õǾ����� Ȯ�����ּ���.";
            ret = false;
        }
        if (_datePicker_StartDate.SelectedDate.ToString() == null || _datePicker_EndDate.SelectedDate.ToString() == null)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "��ȸ �Ⱓ�� Ȯ�����ּ���.";
            ret = false;
        }
        if (formattedDate_StartDate > formattedDate_EndDate)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "��ȸ �Ⱓ�� Ȯ�����ּ���.";
            ret = false;
        }
        if (_toggle_Hour.isOn == false && _toggle_Minute.isOn == false)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "��� ��ȸ ������ Ȯ�����ּ���.";
            ret = false;
        }
        if (_toggle_Hour.isOn == true && _toggle_Minute.isOn == true)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "��� ��ȸ ������ �� ������ ���� �����մϴ�.";
            ret = false;
        }
        ret = true;
        DrawTrendGraph();
    }

    // Ʈ���� �׷��� �׸���
    public void DrawTrendGraph()
    {
        if (ret)
        {
            trend_Setting.SetActive(false);
            emptyContainer.SetActive(false);

            // ������ ��Ʈ ������Ʈ ã��
            E2Chart myChart = graphContainer.GetComponent<E2Chart>();

            // ������ ��Ʈ ������Ʈ�� ������ ���� �߰�
            if (myChart == null)
            {
                myChart = graphContainer.AddComponent<E2Chart>();
            }
            else
            {
                myChart.Clear();
            }

            // �� ���� ���迡 ���� SQL ���� ���� �� ��� DataSet ����
            string query = $"SELECT CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(LOG_MINS, 2, '0')) AS Time, ROUND(AVG(DATA0), 2) AS AvgData0, ROUND(AVG(DATA1), 2) AS AvgData1, ROUND(AVG(DATA2), 2) AS AvgData2, ROUND(AVG(DATA3), 2) AS AvgData3, ROUND(AVG(DATA4), 2) AS AvgData4, ROUND(AVG(DATA5), 2) AS AvgData5 FROM TBL_TREND_{interfaceID}_{controllerID} WHERE LOG_DATE BETWEEN '{startDate}' AND '{endDate}' GROUP BY CONCAT(LOG_DATE, ' ', LPAD(LOG_HOUR, 2, '0'), ':', LPAD(LOG_MINS, 2, '0')) ORDER BY Time";
            DataSet trendDataSet = ClientDatabase.OnSelectRequest(query, $"TBL_TREND_{interfaceID}_{controllerID}");
            //Debug.Log(trendDataSet.Tables[0].Rows.Count);

            // Chart component �߰�            
            myChart.chartType = E2Chart.ChartType.LineChart;

            // Chart options �߰�
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

            // Chart data �߰�
            myChart.chartData = myChart.gameObject.AddComponent<E2ChartData>();
            myChart.chartData.series = new List<E2ChartData.Series>(); // �ø��� ����Ʈ �ʱ�ȭ
            myChart.chartData.title = $"{selectedName} Trend Data"; // ���� ����
            myChart.chartData.yAxisTitle = "Values"; // Y�� ���� ����

            // �ð� ������ ����
            List<string> times = new List<string>();
            foreach (DataRow row in trendDataSet.Tables[0].Rows)
            {
                times.Add(row["Time"].ToString());
            }
            myChart.chartData.categoriesX = times; // X�� ī�װ� ����

            // ������ �ø��� ����
            for (int i = 0; i < 6; i++) // DATA0���� DATA5����
            {
                E2ChartData.Series newSeries = new E2ChartData.Series();
                newSeries.name = "DATA" + i; // ���� �̸����� ġȯ�ؾ���
                newSeries.dataY = new List<float>();

                foreach (DataRow row in trendDataSet.Tables[0].Rows)
                {
                    newSeries.dataY.Add(float.Parse(row["AvgData" + i].ToString()));
                }

                // �ø��� ����Ʈ�� �߰�
                myChart.chartData.series.Add(newSeries);
            }

            // ��Ʈ ������Ʈ
            myChart.UpdateChart();
        }
    }
    void LogDataSet(DataSet dataSet)
    {
        foreach (DataTable table in dataSet.Tables)
        {
            //Debug.Log("Table: " + table.TableName);

            // ���� ��� ���
            string columnsHeader = "";
            foreach (DataColumn column in table.Columns)
            {
                columnsHeader += column.ColumnName + "\t";
            }
            //Debug.Log(columnsHeader);

            // �� ���� ������ ���
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