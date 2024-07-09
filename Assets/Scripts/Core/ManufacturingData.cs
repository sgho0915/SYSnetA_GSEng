using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class ManufacturingData : MonoBehaviour
{
    XMLParser xmlParser;
    ScreenManager screenManager;
    DataSet dataSet;
    TextMeshProUGUI[] tmps;

    string interfaceID = string.Empty;
    int controllerID = 0;
    bool bContinueLoading = true;
    bool bLoad = false;

    public string controllerName = string.Empty;
    public int[] parsedPollingData;    
    private static ManufacturingData _instance;

    private WaitForSeconds pollingInterval = new WaitForSeconds(2);

    



    public static ManufacturingData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ManufacturingData>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "ManufacturingData";
                    _instance = obj.AddComponent<ManufacturingData>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        screenManager = ScreenManager.Instance;
        xmlParser = XMLParser.Instance;
    }

    /// <summary>
    /// 컨트롤러 정보 로드
    /// </summary>
    private void ControllerInfoLoad()
    {
        if (!string.IsNullOrEmpty(OpenDetailView.fld_InterfaceId) && OpenDetailView.fld_ControllerId != 0 && !string.IsNullOrEmpty(OpenDetailView.protocolKey))
        {
            interfaceID = OpenDetailView.fld_InterfaceId; // 선택한 컨트롤러의 인터페이스 ID
            controllerID = OpenDetailView.fld_ControllerId; // 선택한 컨트롤러의 컨트롤러 ID
            //Debug.Log($"현재 디테일뷰의 하드웨어 정보는 interface : {interfaceID}, controller : {controllerID}");
            bLoad = true; // 컨트롤러 정보 로드하도록 변경
        }        
    }

    /// <summary>
    /// 폴링중인 실시간 데이터를 Dataset에서 찾음
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadDatasetContents()
    {
        while (bContinueLoading)
        {
            // 컨트롤러 정보가 로드되지 않았다면 로드한다.
            if (!bLoad)
            {
                ControllerInfoLoad();
            }

            dataSet = ClientDatabase.realTimeData;

            // 조건에 맞는 데이터를 저장하기 위한 리스트
            List<DataRow> matchingRows = new List<DataRow>();

            // 모든 테이블과 행을 순회하며 조건에 맞는 데이터를 찾는다.
            foreach (DataTable table in dataSet.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (row["ID"].ToString() == interfaceID &&
                        int.TryParse(row["CID"].ToString(), out int retrievedControllerID) &&
                        retrievedControllerID == controllerID)
                    {
                        matchingRows.Add(row);
                    }
                }
            }

            // UI 업데이트를 위한 액션 리스트 생성
            if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.DetailView)
            {                
                // 조건에 맞는 행의 데이터를 활용해 UI를 업데이트한다.
                foreach (DataRow row in matchingRows)
                {
                    string pollingData = row["STR_DATA"].ToString();
                    string controllerName = row["CNAME"].ToString();

                    
                    ParsePollingData(pollingData);
                }
            }
            
            yield return pollingInterval;
        }
    }

    /// <summary>
    /// 찾은 실시간 데이터를 Split하여 배열에 담음
    /// </summary>
    /// <param name="pollingData">LoadDatasetContents 코루틴 함수 참고</param>
    public void ParsePollingData(string pollingData)
    {
        string[] stringData = pollingData.Split(',');
        parsedPollingData = new int[stringData.Length];

        for (int i = 0; i < stringData.Length; i++)
        {
            if (!int.TryParse(stringData[i], out parsedPollingData[i])) // int로 변환 시도
            {
                //Debug.LogError($"Failed to parse value at index {i}: {stringData[i]}");
                parsedPollingData[i] = -999; // 실패한 경우 기본값으로 -999를 사용합니다.
            }
        }

        ProcessingRealTimeDataWithXML();
    }

    /// <summary>
    /// 각 어드레스의 실시간 데이터를 XML 정보와 조합하여 불러옴
    /// </summary>    
    public void ProcessingRealTimeDataWithXML()
    {
        // XML 데이터의 시작 주소와 끝 주소값을 가져와 읽음
        for (int i = int.Parse(xmlParser.protocol_Start); i <= int.Parse(xmlParser.protocol_End); i++)
        {
            string addr = i.ToString();

            // XML 내용과 i번째 addr 값을 가진 tag 태그의 속성들을 가져옴
            Dictionary<string, string> attributes = xmlParser.GetTagAttributesByAddr(xmlParser.xmlContent, addr);
            // 반환된 속성들이 null이 아니라면 각 속성들을 순회하며 로그로 출력
            if (attributes != null)
            {
                string combinedData = string.Empty; // 각 addr의 xml 속성 값 문자열
                int currentData = 0; // 각 addr의 실시간 데이터
                string processedData = string.Empty; // 가공 완료된 데이터
                foreach (KeyValuePair<string, string> entry in attributes)
                {
                    combinedData += $"{entry.Key}:{entry.Value},";
                }

                if (parsedPollingData.Length > i)
                {
                    currentData = parsedPollingData[i - 200];

                    // 배율 값으로 데이터를 나눈 후, 단위 값을 붙여 processedData에 저장
                    float multiplyValue;
                    if (attributes.TryGetValue("multiply", out string multiplyStr) && float.TryParse(multiplyStr, out multiplyValue))
                    {
                        float adjustedData = currentData / multiplyValue;
                        if (attributes.TryGetValue("unit", out string unitStr))
                        {
                            processedData = $"{adjustedData}{unitStr}";
                        }
                        else
                        {
                            processedData = adjustedData.ToString();
                        }
                    }
                    else
                    {
                        processedData = currentData.ToString();
                    }
                }

                foreach (var tmp in tmps)
                {
                    if (tmp.name.StartsWith("value_"))
                    {
                        string extractedAddr = tmp.name.Split('_')[1];
                        if (extractedAddr == addr)
                        {
                            if (tmp.name.Contains("_bit"))
                            {
                                int bitIndex = int.Parse(tmp.name.Split('_')[2].Substring(3)); // bitX에서 X 추출
                                int maskValue = 1 << bitIndex; // 해당 비트 위치의 mask value 생성
                                if ((currentData & maskValue) == maskValue) // 해당 비트가 1인지 확인
                                {
                                    // hex 값으로 변환하고 tmp.text에 할당
                                    //tmp.text = maskValue.ToString("X");
                                    tmp.text = "ON";
                                }
                                else
                                {
                                    tmp.text = "OFF";
                                }
                            }
                            else
                            {
                                tmp.text = processedData;
                            }
                        }
                    }
                    if (tmp.name.StartsWith("title_"))
                    {
                        string[] nameParts = tmp.name.Split('_');
                        string extractedAddr = nameParts[1];
                        if (extractedAddr == addr)
                        {
                            try
                            {
                                // changeTargetAdd 속성이 존재하는 경우
                                if (attributes.TryGetValue("changeTargetAdd", out string changeTargetAddrStr) && int.TryParse(changeTargetAddrStr, out int changeTargetAddr))
                                {
                                    int adjustedIndex = changeTargetAddr - 200;
                                    if (adjustedIndex >= 0 && adjustedIndex < parsedPollingData.Length)
                                    {
                                        int changeValue = parsedPollingData[adjustedIndex];
                                        NameListProcessByChangeTargerAddr(changeValue, attributes, tmp);
                                        //Debug.Log($"parsedPollingData 길이는 {parsedPollingData.Length}이고 changeTargetAddr 값 {changeTargetAddr}이 정상적으로 들어와 parsedPollingData 배열의 범위 {adjustedIndex}에 있음");
                                    }
                                    else
                                    {
                                        //Debug.LogError($"parsedPollingData 길이는 {parsedPollingData.Length}이고 changeTargetAddr 값 {changeTargetAddr}로 인해 parsedPollingData 배열의 범위를 벗어난 인덱스 {adjustedIndex}가 발생하였습니다.");
                                    }
                                }
                                // "bitX" 형식의 속성이 존재하는 경우
                                else if (nameParts.Length > 2 && nameParts[2].StartsWith("bit") && attributes.TryGetValue(nameParts[2], out string bitValue))
                                {
                                    tmp.text = bitValue;
                                }
                                // 기본 "name" 속성 값 할당
                                else if (attributes.TryGetValue("name", out string nameAttrValue))
                                {
                                    tmp.text = nameAttrValue;
                                }
                            }
                            catch (IndexOutOfRangeException ex)
                            {
                                //Debug.LogError(ex.Message);
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// XML의 특정 addr 태그 내부의 `changeTargetAdd` 속성을 기반으로 TextMeshProUGUI 오브젝트의 텍스트를 처리
    /// 특정 `addr`에 해당하는 XML 태그에 `changeTargetAdd` 속성이 있다면, 이 속성의 값을 인덱스로 사용
    /// 그 후 `nameList` 속성을 배열로 분할하고, 제공된 인덱스를 사용하여 해당 배열에서 값을 가져와 `tmp.text`에 설정
    /// </summary>
    /// <param name="currentValue">`changeTargetAdd` 속성에서 파생된 `nameList` 배열에 접근하는 데 사용되는 인덱스</param>
    /// <param name="attributes">XML 태그 요소에서 가져온 속성들의 딕셔너리</param>
    /// <param name="tmp">텍스트 속성이 처리되는 TextMeshProUGUI 오브젝트</param>
    public void NameListProcessByChangeTargerAddr(int currentValue, Dictionary<string, string> attributes, TextMeshProUGUI tmp)
    {
        if (attributes.TryGetValue("nameList", out string nameListStr))
        {
            string[] nameListArray = nameListStr.Split(',');
            if (currentValue >= 0 && currentValue < nameListArray.Length)
            {
                tmp.text = nameListArray[currentValue];
            }
        }
    }

    /// <summary>
    /// 디테일뷰 객체 Instantiate시 호출
    /// </summary>
    public void StartLoadingDataset()
    {
        bContinueLoading = true;
        StartCoroutine(LoadDatasetContents());
    }

    /// <summary>
    /// 디테일뷰 객체 Destroy시 호출
    /// </summary>
    public void StopLoadingDataset()
    {
        //Debug.Log("LoadDatasetContents coroutine is inturrupted");
        bContinueLoading = false;
        bLoad = false;
        interfaceID = string.Empty;
        controllerID = 0;
        parsedPollingData = null;
        controllerName = string.Empty;
        xmlParser.protocol_Start = string.Empty;
        xmlParser.protocol_End = string.Empty;
    }    
}