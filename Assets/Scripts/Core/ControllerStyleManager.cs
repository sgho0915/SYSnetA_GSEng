using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using E2C;



public class ControllerStyleManager : MonoBehaviour
{
    public Button btnControllerSetting;
    public GameObject controllerUISetScreen;
    public GameObject trendScrollView;
    public Transform trendScrollViewContent;
    public GameObject toggleTrendPrefab;
    public ToggleGroup trendToggleGroup;
    public TMP_FontAsset font_Pretendard_Bold;

    public TMP_InputField inputField_CName;
    public Toggle toggleFlex;
    public Toggle toggleChart;
    public static Dictionary<string, GameObject> trendElementToggleInstances = new Dictionary<string, GameObject>();
    public Toggle toggleSelectAll;
    public Toggle toggleReflectAll;
    public Button btnSave;
    public Button btnFilter;
    public Button btnView;
    public TMP_InputField inputfield_ControllerSearch;

    private Dictionary<GameObject, Sequence> activeSequences = new Dictionary<GameObject, Sequence>();
    public static bool bSetUse = false;

    Color notClickedColor = new Color(127 / 255f, 127 / 255f, 127 / 255f, 1f); // 7F7F7F, 투명도 1
    Color clickedColor = new Color(116 / 255f, 178 / 255f, 8 / 255f, 1f); // 74B208, 투명도 1

    public static ControllerStyleManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        toggleFlex.group = trendToggleGroup;
        toggleChart.group = trendToggleGroup;

        btnControllerSetting.onClick.AddListener(() =>
        {
            ChangeControllerClickState();
        });
    }

    public void ChangeControllerClickState()
    {
        if (!bSetUse)
        {
            btnFilter.interactable = false;
            btnView.interactable = false;
            inputfield_ControllerSearch.interactable = false;

            btnControllerSetting.gameObject.GetComponent<Image>().color = clickedColor;
            StartShakingEffect();
        }
        else
        {
            btnFilter.interactable = true;
            btnView.interactable = true;
            inputfield_ControllerSearch.interactable = true;
            
            btnControllerSetting.gameObject.GetComponent<Image>().color = notClickedColor;
            StopShakingEffect();

            string[] arrName = null;
            if (ClientDatabase.isGridView)
            {
                foreach (var gridObj in ClientDatabase.controllerGridInstances)
                {
                    arrName = gridObj.Value.name.Split('_');
                    string controllerIID = arrName[1];
                    string controllerCID = arrName[2];
                    int sortedIdx = gridObj.Value.transform.GetSiblingIndex();

                    gridObj.Value.transform.SetSiblingIndex(sortedIdx);

                    string sql = $"UPDATE TBL_CONTROLLER SET ITEM_SORT ='{sortedIdx}', EDIT = 'local' WHERE ID = '{controllerIID}' AND CID = '{controllerCID}'";
                    if (ClientDatabase.OnUpdateRequest(sql))
                    {
                        Debug.Log($"컨트롤러 grid 인스턴스 순서 변경 완료 iid:{controllerIID}, cid:{controllerCID}, itemsort:{sortedIdx}");
                    }
                    else
                    {
                        Debug.Log($"컨트롤러 grid 인스턴스 순서 변경 실패 iid:{controllerIID}, cid:{controllerCID}, itemsort:{sortedIdx}");
                    }
                }
            }
            else
            {
                foreach (var listObj in ClientDatabase.controllerListInstances)
                {
                    arrName = listObj.Value.name.Split('_');
                    string controllerIID = arrName[1];
                    string controllerCID = arrName[2];
                    int sortedIdx = listObj.Value.transform.GetSiblingIndex();

                    listObj.Value.transform.SetSiblingIndex(sortedIdx);

                    string sql = $"UPDATE TBL_CONTROLLER SET ITEM_SORT ='{sortedIdx}', EDIT = 'local' WHERE ID = '{controllerIID}' AND CID = '{controllerCID}'";
                    if (ClientDatabase.OnUpdateRequest(sql))
                    {
                        Debug.Log($"컨트롤러 grid 인스턴스 순서 변경 완료 iid:{controllerIID}, cid:{controllerCID}, itemsort:{sortedIdx}");
                    }
                    else
                    {
                        Debug.Log($"컨트롤러 grid 인스턴스 순서 변경 실패 iid:{controllerIID}, cid:{controllerCID}, itemsort:{sortedIdx}");
                    }
                }
            }
        }
        bSetUse = !bSetUse;
    }

    public void OpenControllerSetUI(string iid, string cid)
    {
        controllerUISetScreen.SetActive(true);
        DataTable tblController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];

        string pkey = string.Empty;
        string xml = string.Empty;
        string newCName = string.Empty;
        string showAddr = string.Empty;
        string[] arrShowAddr = null;
        string showType = string.Empty;
        string newShowAddr = string.Empty;
        string skin = string.Empty;

        foreach (DataRow row in tblController.Rows)
        {
            inputField_CName.text = row["CNAME"].ToString();
            newCName = inputField_CName.text;

            pkey = row["PKEY"].ToString();
            showAddr = row["SHOW_ADDR"].ToString();
            skin = row["SKIN"].ToString();
        }

        if (pkey == string.Empty)
            return;

        ProcessingTrendDataWithXML(iid, cid, pkey);

        arrShowAddr = showAddr.Split(',');
        showType = arrShowAddr[0];

        if(showType == "flex")
        {
            toggleFlex.isOn = true;
            showType = "flex";
        }
        else if(showType == "chart")
        {
            toggleChart.isOn = true;
            showType = "chart";
        }
        else
        {
            toggleFlex.isOn = true;
            showType = "flex";
        }

        List<string> selectedAddrs = new List<string>();
        foreach (var obj in trendElementToggleInstances)
        {
            

            bool isOn = false;
            for (int i = 1; i < arrShowAddr.Length; i++)
            {
                if (obj.Value.name.Contains(arrShowAddr[i]))
                {
                    obj.Value.GetComponent<Toggle>().isOn = true;
                    isOn = true;
                    break;
                }
            }
            if (!isOn)
            {
                obj.Value.GetComponent<Toggle>().isOn = false;
            }

            string addr = obj.Value.name.Split('_')[1];
            bool shouldBeOn = arrShowAddr.Contains(addr);
            obj.Value.GetComponent<Toggle>().isOn = shouldBeOn;
            if (shouldBeOn)
            {
                selectedAddrs.Add(addr);
            }

            if (skin.Contains("Run") && obj.Value.name == "Trend_Run")
                obj.Value.GetComponent<Toggle>().isOn = true;

            if (skin.Contains("Cool") && obj.Value.name == "Trend_Cool")
                obj.Value.GetComponent<Toggle>().isOn = true;

            if (skin.Contains("Heat") && obj.Value.name == "Trend_Heat")
                obj.Value.GetComponent<Toggle>().isOn = true;

            if (skin.Contains("Def") && obj.Value.name == "Trend_Def")
                obj.Value.GetComponent<Toggle>().isOn = true;

            if (skin.Contains("Fan") && obj.Value.name == "Trend_Fan")
                obj.Value.GetComponent<Toggle>().isOn = true;

            obj.Value.GetComponent<Toggle>().onValueChanged.AddListener((isOn) => {
                if (obj.Value.name == "Trend_Run" ||
                obj.Value.name == "Trend_Cool" ||
                obj.Value.name == "Trend_Heat" ||
                obj.Value.name == "Trend_Def" ||
                obj.Value.name == "Trend_Fan")
                {
                    
                }
                else
                {
                    if (isOn && !selectedAddrs.Contains(addr))
                    {
                        selectedAddrs.Add(addr);
                    }
                    else if (!isOn && selectedAddrs.Contains(addr))
                    {
                        selectedAddrs.Remove(addr);
                    }
                }
                    newShowAddr = UpdateShowAddr(showType, selectedAddrs);
                
            });
        }

        UpdateShowAddr(showType, selectedAddrs);

        toggleFlex.onValueChanged.AddListener(isOn => {
            if (isOn)
            {
                showType = "flex";
                toggleChart.isOn = false;
                newShowAddr = UpdateShowAddr(showType, selectedAddrs);
            }
        });

        toggleChart.onValueChanged.AddListener(isOn => {
            if (isOn)
            {
                showType = "chart";
                toggleFlex.isOn = false;
                newShowAddr = UpdateShowAddr(showType, selectedAddrs);
            }
        });

        toggleSelectAll.onValueChanged.AddListener(isOn =>
        {
            foreach (var obj in trendElementToggleInstances)
            {
                obj.Value.GetComponent<Toggle>().isOn = isOn;
            }
        });
                
        toggleReflectAll.onValueChanged.AddListener(isOn =>
        {
            newShowAddr = UpdateShowAddr(showType, selectedAddrs);
        });

        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            SaveControllerSetUI(newCName, newShowAddr, iid, cid, pkey);
        });

        inputField_CName.onValueChanged.AddListener((value) =>
        {
            newCName = value;
        });
    }

    public void UpdateStyle()
    {
        DataTable tblController = ClientDatabase.controllerData.Tables[0];
        string iid = string.Empty;
        string cid = string.Empty;
        string showAddr = string.Empty;
        List<string> selectedAddrs = null;
        foreach (DataRow row in tblController.Rows)
        {
            iid = row["ID"].ToString();
            cid = row["CID"].ToString();
            showAddr = row["SHOW_ADDR"].ToString();
            string[] arr = showAddr.Split(',');
            
            if (ClientDatabase.isGridView)
            {
                foreach (var controllerInstance in ClientDatabase.controllerGridInstances)
                {
                    GameObject graphContainer = controllerInstance.Value.transform.Find("obj_GraphContainer").gameObject;
                    GameObject scrollView = controllerInstance.Value.transform.Find("obj_FLD_StatusValueScrollView").gameObject;

                    if (controllerInstance.Value.name.Split('_')[1] == iid &&
                        controllerInstance.Value.name.Split('_')[2] == cid)
                    {
                        if (row["SHOW_ADDR"].ToString().Contains("chart"))
                        {
                            controllerInstance.Value.transform.Find("obj_FLD_StatusValueScrollView").gameObject.SetActive(false);
                            controllerInstance.Value.transform.Find("obj_GraphContainer").gameObject.SetActive(true);
                            for (int i = 1; arr.Length > i; i++)
                            {
                                Debug.Log($"{iid}-{cid}, {arr[i]}");
                                selectedAddrs.Add(arr[i]);
                            }
                            FetchAndMergeTrendData(iid, cid, selectedAddrs, graphContainer);
                            selectedAddrs.Clear();
                        }
                        else
                        {
                            controllerInstance.Value.transform.Find("obj_FLD_StatusValueScrollView").gameObject.SetActive(true);
                            controllerInstance.Value.transform.Find("obj_GraphContainer").gameObject.SetActive(false);
                        }
                    }
                }
            }            
        }
    }

    void FetchAndMergeTrendData(string iid, string cid, List<string> selectedTrendElement, GameObject graphContainer)
    {
        List<DataTable> listTrendTables = new List<DataTable>();
        string unit = string.Empty;

            // 테이블 존재 여부 확인
            if (!ClientDatabase.TableExists($"TBL_TREND_{iid}_{cid}"))
            {
                Debug.LogError("선택한 컨트롤러의 트렌드 데이터가 존재하지 않습니다.");
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


                string[] headInfoParts = headInfo.Split(',');
                int dataIndex = -1;
                for (int j = 0; j < headInfoParts.Length; j++)
                {
                    string[] parts = headInfoParts[j].Split('|');
                    for (int k = 0; k < selectedTrendElement.Count; k++)
                    {
                        //string[] arrSelectedTrendPart = selectedTrendElement[k].Split("_");
                        if (parts[1] == $"{selectedTrendElement[k]}")
                        {
                            unit = parts[2];

                            dataIndex = j; // 데이터 인덱스 찾음

                            string drawDayGraphQuery = $@"
                                SELECT 
                                    DATE_FORMAT(CONCAT(LOG_DATE, ' ', LPAD(FLOOR(LOG_HOUR / 2) * 2, 2, '0')), '%m-%d %H시') AS TimeGroup, 
                                    AVG(DATA{dataIndex}) AS AvgData 
                                FROM 
                                    TBL_TREND_{iid}_{cid} 
                                WHERE 
                                    LOG_DATE > DATE_SUB(CURDATE(), INTERVAL 1 DAY) OR 
                                    (LOG_DATE = DATE_SUB(CURDATE(), INTERVAL 1 DAY) AND LOG_HOUR >= HOUR(NOW()) AND LOG_MINS >= MINUTE(NOW()))
                                GROUP BY 
                                    TimeGroup 
                                ORDER BY 
                                    TimeGroup;
                                ";

                        DataTable trendTable = null;
                                trendTable = ClientDatabase.OnSelectRequest(drawDayGraphQuery, $"TBL_TREND_{iid}_{cid}").Tables[0];

                            if (trendTable != null && trendTable.Rows.Count > 0)
                            {
                                listTrendTables.Add(trendTable);
                            }
                            else
                            {
                                Debug.LogError($"조회된 데이터가 없습니다. 컨트롤러: {iid}-{cid}, 요소: {selectedTrendElement[k]}");
                            }
                        }
                    }
                }
            }
        

        // DataTable join후 그래프 그리기 시작
        DataTable mergedTable = InquiryManager.MergeMultipleDataTablesLINQ(listTrendTables);
        if (Application.platform != RuntimePlatform.Android)
        {
            //Debug.Log(listTrendTables.Count);
            InquiryManager.LogDataTableContent(mergedTable);
        }

        if (mergedTable == null || mergedTable.Rows.Count == 0)
        {
            Debug.LogError("조회된 데이터가 없습니다.");
            return;
        }

        UpdateChartWithData(mergedTable, unit, listTrendTables.Count, graphContainer);
    }

    void UpdateChartWithData(DataTable mergedTable, string unit, int mergedTablesCnt, GameObject graphContainer)
    {
        // 기존의 차트 컴포넌트 찾기
        E2Chart myChart = graphContainer.GetComponent<E2Chart>();

        // 기존의 차트 컴포넌트가 없으면 새로 추가
        if (myChart == null)
            myChart = graphContainer.AddComponent<E2Chart>();
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

        for (int i = 1; i <= mergedTablesCnt; i++)
        {
            // 데이터 시리즈 생성
            E2ChartData.Series newSeries = new E2ChartData.Series();
            newSeries.name = $"머지이게"; // 여기 이름으로 치환해야함
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

    private string UpdateShowAddr(string showType, List<string> selectedAddrs)
    {
        string newShowAddr = showType + "," + string.Join(",", selectedAddrs);
        Debug.Log("Updated Show Addr: " + newShowAddr);
        return newShowAddr;
    }

    public void SaveControllerSetUI(string newcname, string showaddr, string iid, string cid, string pkey)
    {
        string strStatusResult = string.Empty;
        string sql = string.Empty;
        string sql2 = string.Empty;
        foreach (var toggle in trendElementToggleInstances) {
            if (toggle.Value.name == "Trend_Run" && toggle.Value.GetComponent<Toggle>().isOn)
                strStatusResult += "Run,";
            if (toggle.Value.name == "Trend_Cool" && toggle.Value.GetComponent<Toggle>().isOn)
                strStatusResult += "Cool,";
            if (toggle.Value.name == "Trend_Heat" && toggle.Value.GetComponent<Toggle>().isOn)
                strStatusResult += "Heat,";
            if (toggle.Value.name == "Trend_Def" && toggle.Value.GetComponent<Toggle>().isOn)
                strStatusResult += "Def,";
            if (toggle.Value.name == "Trend_Fan" && toggle.Value.GetComponent<Toggle>().isOn)
                strStatusResult += "Fan,";
        }

        if(!toggleReflectAll.isOn)
        {
            sql = $"UPDATE TBL_CONTROLLER SET CNAME = '{newcname}', SKIN = '{strStatusResult}', SHOW_ADDR ='{showaddr}' WHERE ID = '{iid}' AND CID = '{cid}'";
        }
        else
        {            
            sql = $"UPDATE TBL_CONTROLLER SET CNAME = '{newcname}' WHERE ID = '{iid}' AND CID = '{cid}'";
            sql2 = $"UPDATE TBL_CONTROLLER SET SKIN = '{strStatusResult}', SHOW_ADDR ='{showaddr}' WHERE PKEY = '{pkey}'";
        }


        if(!toggleReflectAll.isOn)
        {
            if (ClientDatabase.OnUpdateRequest(sql))
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정이 저장 되었습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                    CloseControllerSetUI();
                });
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정 저장에 실패했습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                    CloseControllerSetUI();
                });
            }
        }
        else
        {
            if (ClientDatabase.OnUpdateRequest(sql) && ClientDatabase.OnUpdateRequest(sql2))
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정이 저장 되었습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                    CloseControllerSetUI();
                });
            }
            else
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "설정 저장에 실패했습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                    CloseControllerSetUI();
                });
            }
        }
    }

    public void CloseControllerSetUI()
    {
        inputField_CName.text = string.Empty;
        toggleFlex.isOn = true;
        foreach (var obj in trendElementToggleInstances)
        {
            Destroy(obj.Value.gameObject);
        }
        trendElementToggleInstances.Clear();
        toggleSelectAll.isOn = false;
        toggleReflectAll.isOn = false;
        controllerUISetScreen.SetActive(false);
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
                        string trendElementObjName = $"Trend_{addr}";

                        if (!trendElementToggleInstances.ContainsKey(trendElementObjName))
                        {
                            GameObject trendToggleInstance = Instantiate(toggleTrendPrefab, trendScrollViewContent);
                            trendToggleInstance.name = trendElementObjName;
                            trendElementToggleInstances[trendElementObjName] = trendToggleInstance;

                            TextMeshProUGUI txtTrendName = trendToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                            txtTrendName.text = name;
                        }
                    }
                }
            }

            string trendElementObjName_Run = $"Trend_Run";
            string trendElementObjName_Cool = $"Trend_Cool";
            string trendElementObjName_Heat = $"Trend_Heat";
            string trendElementObjName_Def = $"Trend_Def";
            string trendElementObjName_Fan = $"Trend_Fan";

            if (!trendElementToggleInstances.ContainsKey(trendElementObjName_Run) &&
                !trendElementToggleInstances.ContainsKey(trendElementObjName_Def) &&
                !trendElementToggleInstances.ContainsKey(trendElementObjName_Cool) &&
                !trendElementToggleInstances.ContainsKey(trendElementObjName_Fan) &&
                !trendElementToggleInstances.ContainsKey(trendElementObjName_Heat))
            {
                GameObject trendToggleInstance_Run = Instantiate(toggleTrendPrefab, trendScrollViewContent);
                GameObject trendToggleInstance_Cool = Instantiate(toggleTrendPrefab, trendScrollViewContent);
                GameObject trendToggleInstance_Heat = Instantiate(toggleTrendPrefab, trendScrollViewContent);
                GameObject trendToggleInstance_Def = Instantiate(toggleTrendPrefab, trendScrollViewContent);
                GameObject trendToggleInstance_Fan = Instantiate(toggleTrendPrefab, trendScrollViewContent);
                trendToggleInstance_Run.name = trendElementObjName_Run;
                trendToggleInstance_Cool.name = trendElementObjName_Cool;
                trendToggleInstance_Heat.name = trendElementObjName_Heat;
                trendToggleInstance_Def.name = trendElementObjName_Def;
                trendToggleInstance_Fan.name = trendElementObjName_Fan;
                trendElementToggleInstances[trendElementObjName_Run] = trendToggleInstance_Run;
                trendElementToggleInstances[trendElementObjName_Cool] = trendToggleInstance_Cool;
                trendElementToggleInstances[trendElementObjName_Heat] = trendToggleInstance_Heat;
                trendElementToggleInstances[trendElementObjName_Def] = trendToggleInstance_Def;
                trendElementToggleInstances[trendElementObjName_Fan] = trendToggleInstance_Fan;

                TextMeshProUGUI txtTrendName_Run = trendToggleInstance_Run.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtTrendName_Cool = trendToggleInstance_Cool.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtTrendName_Heat = trendToggleInstance_Heat.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtTrendName_Def = trendToggleInstance_Def.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtTrendName_Fan = trendToggleInstance_Fan.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                txtTrendName_Run.text = "운전 상태";
                txtTrendName_Cool.text = "냉방 상태";
                txtTrendName_Heat.text = "난방 상태";
                txtTrendName_Def.text = "제상 상태";
                txtTrendName_Fan.text = "팬 상태";
            }
        }
        catch (Exception ex)
        {
            //Debug.LogError($"DetailView : Exception in ProcessingTrendDataWithXML: {ex}");
        }
    }

    public void StartShakingEffect()
    {
        if (ClientDatabase.isGridView)
        {
            foreach (var instance in ClientDatabase.controllerGridInstances)
            {
                Transform transform = instance.Value.transform;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(transform.DORotate(new Vector3(0f, 0f, 0.65f), 0.04f).SetEase(Ease.InOutSine).SetRelative());
                sequence.Append(transform.DORotate(new Vector3(0f, 0f, -0.65f), 0.04f).SetEase(Ease.InOutSine).SetRelative());
                sequence.Append(transform.DORotate(new Vector3(0f, 0f, -0.65f), 0.04f).SetEase(Ease.InOutSine).SetRelative());
                sequence.Append(transform.DORotate(new Vector3(0f, 0f, 0.65f), 0.04f).SetEase(Ease.InOutSine).SetRelative());
                sequence.SetLoops(-1, LoopType.Restart);
                activeSequences[instance.Value] = sequence;  // Store sequence reference

                transform.Find("obj_UISet").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var instance in ClientDatabase.controllerListInstances)
            {
                Transform transform = instance.Value.transform;
                Sequence sequence = DOTween.Sequence();

                sequence.Append(transform.DORotate(new Vector3(0f, 5f, 0f), 0.08f).SetEase(Ease.InOutSine).SetRelative());
                sequence.Append(transform.DORotate(new Vector3(0f, -5f, 0f), 0.08f).SetEase(Ease.InOutSine).SetRelative());
                sequence.Append(transform.DORotate(new Vector3(0f, -5f, 0f), 0.08f).SetEase(Ease.InOutSine).SetRelative());
                sequence.Append(transform.DORotate(new Vector3(0f, 5f, 0f), 0.08f).SetEase(Ease.InOutSine).SetRelative());
                sequence.SetLoops(-1, LoopType.Restart);
                activeSequences[instance.Value] = sequence;  // Store sequence reference

                transform.Find("obj_UISet").gameObject.SetActive(true);
            }
        }        
    }

    public void StopShakingEffect()
    {
        if (ClientDatabase.isGridView)
        {
            foreach (var pair in activeSequences)
            {
                pair.Value.Kill(complete: true);  // Kill the sequence and complete it
                pair.Key.transform.rotation = Quaternion.identity;  // Reset rotation            
            }
            activeSequences.Clear();  // Clear references
            foreach (var instance in ClientDatabase.controllerGridInstances)
            {
                Transform transform = instance.Value.transform;
                transform.Find("obj_UISet").gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var pair in activeSequences)
            {
                pair.Value.Kill(complete: true);  // Kill the sequence and complete it
                pair.Key.transform.rotation = Quaternion.identity;  // Reset rotation            
            }
            activeSequences.Clear();  // Clear references
            foreach (var instance in ClientDatabase.controllerListInstances)
            {
                Transform transform = instance.Value.transform;
                transform.Find("obj_UISet").gameObject.SetActive(false);
            }
        }
        
    }
}
