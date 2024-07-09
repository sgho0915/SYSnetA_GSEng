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
    /// ��Ʈ�ѷ� ���� �ε�
    /// </summary>
    private void ControllerInfoLoad()
    {
        if (!string.IsNullOrEmpty(OpenDetailView.fld_InterfaceId) && OpenDetailView.fld_ControllerId != 0 && !string.IsNullOrEmpty(OpenDetailView.protocolKey))
        {
            interfaceID = OpenDetailView.fld_InterfaceId; // ������ ��Ʈ�ѷ��� �������̽� ID
            controllerID = OpenDetailView.fld_ControllerId; // ������ ��Ʈ�ѷ��� ��Ʈ�ѷ� ID
            //Debug.Log($"���� �����Ϻ��� �ϵ���� ������ interface : {interfaceID}, controller : {controllerID}");
            bLoad = true; // ��Ʈ�ѷ� ���� �ε��ϵ��� ����
        }        
    }

    /// <summary>
    /// �������� �ǽð� �����͸� Dataset���� ã��
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadDatasetContents()
    {
        while (bContinueLoading)
        {
            // ��Ʈ�ѷ� ������ �ε���� �ʾҴٸ� �ε��Ѵ�.
            if (!bLoad)
            {
                ControllerInfoLoad();
            }

            dataSet = ClientDatabase.realTimeData;

            // ���ǿ� �´� �����͸� �����ϱ� ���� ����Ʈ
            List<DataRow> matchingRows = new List<DataRow>();

            // ��� ���̺�� ���� ��ȸ�ϸ� ���ǿ� �´� �����͸� ã�´�.
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

            // UI ������Ʈ�� ���� �׼� ����Ʈ ����
            if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.DetailView)
            {                
                // ���ǿ� �´� ���� �����͸� Ȱ���� UI�� ������Ʈ�Ѵ�.
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
    /// ã�� �ǽð� �����͸� Split�Ͽ� �迭�� ����
    /// </summary>
    /// <param name="pollingData">LoadDatasetContents �ڷ�ƾ �Լ� ����</param>
    public void ParsePollingData(string pollingData)
    {
        string[] stringData = pollingData.Split(',');
        parsedPollingData = new int[stringData.Length];

        for (int i = 0; i < stringData.Length; i++)
        {
            if (!int.TryParse(stringData[i], out parsedPollingData[i])) // int�� ��ȯ �õ�
            {
                //Debug.LogError($"Failed to parse value at index {i}: {stringData[i]}");
                parsedPollingData[i] = -999; // ������ ��� �⺻������ -999�� ����մϴ�.
            }
        }

        ProcessingRealTimeDataWithXML();
    }

    /// <summary>
    /// �� ��巹���� �ǽð� �����͸� XML ������ �����Ͽ� �ҷ���
    /// </summary>    
    public void ProcessingRealTimeDataWithXML()
    {
        // XML �������� ���� �ּҿ� �� �ּҰ��� ������ ����
        for (int i = int.Parse(xmlParser.protocol_Start); i <= int.Parse(xmlParser.protocol_End); i++)
        {
            string addr = i.ToString();

            // XML ����� i��° addr ���� ���� tag �±��� �Ӽ����� ������
            Dictionary<string, string> attributes = xmlParser.GetTagAttributesByAddr(xmlParser.xmlContent, addr);
            // ��ȯ�� �Ӽ����� null�� �ƴ϶�� �� �Ӽ����� ��ȸ�ϸ� �α׷� ���
            if (attributes != null)
            {
                string combinedData = string.Empty; // �� addr�� xml �Ӽ� �� ���ڿ�
                int currentData = 0; // �� addr�� �ǽð� ������
                string processedData = string.Empty; // ���� �Ϸ�� ������
                foreach (KeyValuePair<string, string> entry in attributes)
                {
                    combinedData += $"{entry.Key}:{entry.Value},";
                }

                if (parsedPollingData.Length > i)
                {
                    currentData = parsedPollingData[i - 200];

                    // ���� ������ �����͸� ���� ��, ���� ���� �ٿ� processedData�� ����
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
                                int bitIndex = int.Parse(tmp.name.Split('_')[2].Substring(3)); // bitX���� X ����
                                int maskValue = 1 << bitIndex; // �ش� ��Ʈ ��ġ�� mask value ����
                                if ((currentData & maskValue) == maskValue) // �ش� ��Ʈ�� 1���� Ȯ��
                                {
                                    // hex ������ ��ȯ�ϰ� tmp.text�� �Ҵ�
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
                                // changeTargetAdd �Ӽ��� �����ϴ� ���
                                if (attributes.TryGetValue("changeTargetAdd", out string changeTargetAddrStr) && int.TryParse(changeTargetAddrStr, out int changeTargetAddr))
                                {
                                    int adjustedIndex = changeTargetAddr - 200;
                                    if (adjustedIndex >= 0 && adjustedIndex < parsedPollingData.Length)
                                    {
                                        int changeValue = parsedPollingData[adjustedIndex];
                                        NameListProcessByChangeTargerAddr(changeValue, attributes, tmp);
                                        //Debug.Log($"parsedPollingData ���̴� {parsedPollingData.Length}�̰� changeTargetAddr �� {changeTargetAddr}�� ���������� ���� parsedPollingData �迭�� ���� {adjustedIndex}�� ����");
                                    }
                                    else
                                    {
                                        //Debug.LogError($"parsedPollingData ���̴� {parsedPollingData.Length}�̰� changeTargetAddr �� {changeTargetAddr}�� ���� parsedPollingData �迭�� ������ ��� �ε��� {adjustedIndex}�� �߻��Ͽ����ϴ�.");
                                    }
                                }
                                // "bitX" ������ �Ӽ��� �����ϴ� ���
                                else if (nameParts.Length > 2 && nameParts[2].StartsWith("bit") && attributes.TryGetValue(nameParts[2], out string bitValue))
                                {
                                    tmp.text = bitValue;
                                }
                                // �⺻ "name" �Ӽ� �� �Ҵ�
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
    /// XML�� Ư�� addr �±� ������ `changeTargetAdd` �Ӽ��� ������� TextMeshProUGUI ������Ʈ�� �ؽ�Ʈ�� ó��
    /// Ư�� `addr`�� �ش��ϴ� XML �±׿� `changeTargetAdd` �Ӽ��� �ִٸ�, �� �Ӽ��� ���� �ε����� ���
    /// �� �� `nameList` �Ӽ��� �迭�� �����ϰ�, ������ �ε����� ����Ͽ� �ش� �迭���� ���� ������ `tmp.text`�� ����
    /// </summary>
    /// <param name="currentValue">`changeTargetAdd` �Ӽ����� �Ļ��� `nameList` �迭�� �����ϴ� �� ���Ǵ� �ε���</param>
    /// <param name="attributes">XML �±� ��ҿ��� ������ �Ӽ����� ��ųʸ�</param>
    /// <param name="tmp">�ؽ�Ʈ �Ӽ��� ó���Ǵ� TextMeshProUGUI ������Ʈ</param>
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
    /// �����Ϻ� ��ü Instantiate�� ȣ��
    /// </summary>
    public void StartLoadingDataset()
    {
        bContinueLoading = true;
        StartCoroutine(LoadDatasetContents());
    }

    /// <summary>
    /// �����Ϻ� ��ü Destroy�� ȣ��
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