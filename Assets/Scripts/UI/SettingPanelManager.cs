using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

public class SettingPanelManager : MonoBehaviour
{
    static XMLParser xmlParser;
    public GameObject obj_SettingPanel;
    public static GameObject obj_SettingNumpad;
    public static GameObject obj_SettingDropdown;
    public static GameObject obj_SettingToggle;
    [SerializeField]private GameObject _obj_SettingNumpad;
    [SerializeField]private GameObject _obj_SettingDropdown;
    [SerializeField]private GameObject _obj_SettingToggle;
    public Image btn_Setting;

    public static Transform btn_PrevToMain;
    public static TextMeshProUGUI txt_SettingTitle;

    public static TextMeshProUGUI txt_TitleNumPad;
    public static TextMeshProUGUI txt_TitleDropdown;
    public static TextMeshProUGUI txt_TitleToggle;

    public static TextMeshProUGUI txt_ValueNumPad;
    public static TextMeshProUGUI txt_ValueDropdown;
    public static TextMeshProUGUI txt_ValueToggle;

    public static TextMeshProUGUI txt_ValueRange;
    public static TextMeshProUGUI txt_NewValueNumpad;
    public static TextMeshProUGUI txt_NewValueDropdown;

    public static TMP_Dropdown dropdown_ItemList;
    public static Toggle toggle_Value_NotUse;
    public static Toggle toggle_Value_Toggle;
    public static bool b_Value_NotUse;

    public static bool bIsOpen = false; // �����г� Ȱ������
        
    

    static string selectedSettingName = string.Empty; // ������ ���� ������Ʈ��
    static string selectedSettingTitle = string.Empty; // ������ ���� Ÿ��Ʋ
    static string selectedDetailSettingName = string.Empty; // ������ ���μ��� ������Ʈ��
    static string selectedSettingGroupID = string.Empty; // ������ ���� ������Ʈ�� xml setgroup ID
    static string selectedDetailSettingTitle = string.Empty; // ������ ���μ��� Ÿ��Ʋ
    static string selectedDetailSettingValue = string.Empty; // ������ ���μ��� ���� ������
    static string selectedDetailSettingBitValue = string.Empty; // ������ ���μ��� ���� Bit ��
    static string selectedSettingAddr = string.Empty; // ������ ���μ��� �׸��� ������ Addr
    static string strStyleValue = string.Empty; // ������ ���μ��� �׸��� xml style

    static string strTrueBitValue = string.Empty;
    static string strFalseBitValue = string.Empty;
    public static string strUnitValue = string.Empty;
    private void Awake()
    {
        xmlParser = XMLParser.Instance;
        Transform transform = obj_SettingPanel.transform;
        txt_SettingTitle = obj_SettingPanel.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
        obj_SettingNumpad = _obj_SettingNumpad;
        obj_SettingDropdown = _obj_SettingDropdown;
        obj_SettingToggle = _obj_SettingToggle;
        txt_TitleNumPad = FindDeepChild(obj_SettingNumpad.transform, "txt_TitleNumPad").GetComponent<TextMeshProUGUI>();
        txt_TitleDropdown = FindDeepChild(obj_SettingDropdown.transform, "txt_TitleDropdown").GetComponent<TextMeshProUGUI>();
        txt_TitleToggle = FindDeepChild(obj_SettingToggle.transform, "txt_TitleToggle").GetComponent<TextMeshProUGUI>();
        txt_ValueNumPad = FindDeepChild(obj_SettingNumpad.transform, "txt_CurrentValue").GetComponent<TextMeshProUGUI>();
        txt_ValueDropdown = FindDeepChild(obj_SettingDropdown.transform, "txt_CurrentValue").GetComponent<TextMeshProUGUI>();
        txt_ValueRange = FindDeepChild(obj_SettingNumpad.transform, "txt_RangeValue").GetComponent<TextMeshProUGUI>();
        txt_ValueToggle = FindDeepChild(obj_SettingToggle.transform, "txt_CurrentValue").GetComponent<TextMeshProUGUI>();

        txt_NewValueNumpad = FindDeepChild(obj_SettingNumpad.transform, "txt_NewValue").GetComponent<TextMeshProUGUI>();
        txt_NewValueDropdown = FindDeepChild(obj_SettingDropdown.transform, "txt_NewValue").GetComponent<TextMeshProUGUI>();

        toggle_Value_NotUse = FindDeepChild(obj_SettingNumpad.transform, "Toggle_NotUse").GetComponent<Toggle>();
        toggle_Value_Toggle = FindDeepChild(obj_SettingToggle.transform, "Toggle").GetComponent<Toggle>();
        dropdown_ItemList = FindDeepChild(obj_SettingDropdown.transform, "Dropdown_Options").GetComponent<TMP_Dropdown>();
        btn_PrevToMain = FindDeepChild(transform, "btn_Prev");
    }

    public Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            Transform result = FindDeepChild(child, childName);
            if (result != null)
                return result;
        }
        return null;
    }


    public void OpenPanel()
    {
        btn_PrevToMain.gameObject.SetActive(false);
        bIsOpen = !bIsOpen;
        obj_SettingPanel.SetActive(bIsOpen);
        if (bIsOpen)
        {
            //ScreenManager.Instance.CurrentScreenState = ScreenManager.ScreenState.DetailViewPopUp;
            btn_Setting.color = new Color32(255, 169, 0, 255);
            ProcessingSetGroupDataWithXML();
        }
        else
        {
            ScreenManager.Instance.CurrentScreenState = ScreenManager.ScreenState.DetailView;
            btn_Setting.color = new Color32(101, 101, 101, 255);
            ObjectPool.Instance.ReturnSettingItemObject();
            ObjectPool.Instance.ReturnSettingDetailItemObject();
        }
    }

    public void ClosePanel()
    {
        ScreenManager.Instance.CurrentScreenState = ScreenManager.ScreenState.DetailView;
        bIsOpen = false;
        obj_SettingPanel.SetActive(bIsOpen);
        btn_Setting.color = new Color32(101, 101, 101, 255);
        ObjectPool.Instance.ReturnSettingItemObject();
        ObjectPool.Instance.ReturnSettingDetailItemObject();
    }


    /// <summary>
    /// SetGroup �����͸� XML ������ �����Ͽ� �ҷ���
    /// </summary>    
    public void ProcessingSetGroupDataWithXML()
    {
        Dictionary<string, Dictionary<string, string>> attributes = xmlParser.GetSetGroupAttributes(xmlParser.xmlContent);

        try
        {
            foreach (var group in attributes)
            {
                GameObject btnInstance = ObjectPool.Instance.GetSettingItemObject();
                TextMeshProUGUI settingItemTitleText = btnInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                if (settingItemTitleText != null)
                {
                    settingItemTitleText.text = group.Key.ToString();                 
                }
                foreach (var attribute in group.Value)
                {                    
                    btnInstance.name = "btn_SettingItem_" + attribute.Value.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            //Debug.Log(ex);
        }        
    }



    /// <summary>
    /// SetDetailGroup �����͸� XML ������ �����Ͽ� �ҷ���
    /// </summary>    
    public static void ProcessingSetDetailGroupDataWithXML()
    {
        txt_SettingTitle.text = selectedSettingTitle;
        btn_PrevToMain.gameObject.SetActive(true);

        if (selectedSettingName.StartsWith("btn_SettingItem_"))
        {
            selectedSettingGroupID = selectedSettingName.Substring("btn_SettingItem_".Length);
        }

        Dictionary<string, string> tags = xmlParser.GetTagAttributesBySetGroup(xmlParser.xmlContent, selectedSettingGroupID);
        Dictionary<string, int> bitMask = new Dictionary<string, int>
{
    {"bit0", 0x0001},
    {"bit1", 0x0002},
    {"bit2", 0x0004},
    {"bit3", 0x0008},
    {"bit4", 0x0010},
    {"bit5", 0x0020},
    {"bit6", 0x0040},
    {"bit7", 0x0080},
    {"bit8", 0x0100},
    {"bit9", 0x0200},
    {"bit10", 0x0400},
    {"bit11", 0x0800},
    {"bit12", 0x1000},
    {"bit13", 0x2000},
    {"bit14", 0x4000},
    {"bit15", 0x8000}
};

        foreach (var tag in tags)
        {
            string addr = tag.Value;            
            Dictionary<string, string> attributes = xmlParser.GetTagAttributesByAddr(xmlParser.xmlContent, addr);

            List<string> bitAttributes = new List<string>();
            foreach (var attribute in attributes)
            {
                if (attribute.Key.StartsWith("bit") && !attribute.Key.Contains("NameList"))
                {
                    bitAttributes.Add(attribute.Key);
                }
            }

            if (bitAttributes.Count > 0) // bit �Ӽ��� ���� ��
            {
                for (int i = 0; i < bitAttributes.Count; i++)
                {
                    GameObject btnInstance = ObjectPool.Instance.GetSettingDetailItemObject();
                    TextMeshProUGUI settingItemTitleText = btnInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI settingItemValueText = btnInstance.transform.Find("txt_CurValue").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI settingItemBitValueText = btnInstance.transform.Find("txt_BitValue").GetComponent<TextMeshProUGUI>();
                    int currentData = ManufacturingData.Instance.parsedPollingData[Convert.ToInt32(addr) - 200];

                    if (attributes.TryGetValue(bitAttributes[i], out string value))
                    {                        
                        string bitNameListKey = $"{bitAttributes[i]}NameList";
                        if (attributes.TryGetValue(bitNameListKey, out string bitNameListValue))
                        {
                            string[] names = bitNameListValue.Split(',');
                            if (currentData >= 0 && currentData < names.Length)
                            {
                                settingItemTitleText.text = names[currentData];
                            }
                            else
                            {
                                settingItemTitleText.text = names[0];
                            }
                        }
                        else
                        {
                            settingItemTitleText.text = value;
                        }
                    }
                    else
                    {
                        settingItemTitleText.text = "Unknown";
                    }


                    // �ش� bit�� �� ����
                    int bitValue = (currentData & bitMask[bitAttributes[i]]) > 0 ? 1 : 0;
                    settingItemBitValueText.text = bitValue.ToString();

                    // ��� ����� ���� trueValue�� falseValue�� ���� ��� ��������
                    if (attributes.TryGetValue("trueValue", out string trueBitValue) && attributes.TryGetValue("falseValue", out string falseBitValue))
                    {
                        strTrueBitValue = trueBitValue;
                        strFalseBitValue = falseBitValue;
                    }

                    // ���� bitValue�� �������� trueValue �Ǵ� falseValue ���� �����մϴ�.
                    if (bitValue == 1)
                    {
                        if (attributes.TryGetValue("trueValue", out string trueValue))
                        {
                            settingItemValueText.text = trueValue;
                        }
                    }
                    else
                    {
                        if (attributes.TryGetValue("falseValue", out string falseValue))
                        {
                            settingItemValueText.text = falseValue;
                        }
                    }

                    btnInstance.name = "btn_SettingDetailItem_" + addr + bitAttributes[i];
                }
            }
            else // ������ ��Ĵ�� ó��
            {
                GameObject btnInstance = ObjectPool.Instance.GetSettingDetailItemObject();
                TextMeshProUGUI settingItemTitleText = btnInstance.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI settingItemValueText = btnInstance.transform.Find("txt_CurValue").GetComponent<TextMeshProUGUI>();
                int currentData = ManufacturingData.Instance.parsedPollingData[Convert.ToInt32(addr) - 200];

                string processedData = ProcessData(currentData, attributes);
                SetTitleAndValue(settingItemTitleText, settingItemValueText, processedData, currentData, attributes);

                btnInstance.name = "btn_SettingDetailItem_" + addr;
            }
        }
    }



    private static int ConvertDataBySize(int rawData, string size)
    {
        switch (size)
        {
            case "u1":
                return rawData & 0xFF;
            case "s2":
                return (short)rawData;
            // �߰����� size �Ӽ��� ���� ��ȯ ������ �ʿ��ϸ� ���⿡ �߰��մϴ�.
            default:
                return rawData;
        }
    }

    private static string ProcessData(int currentData, Dictionary<string, string> attributes)
    {
        ProcessDataBySize(ref currentData, attributes);
        return ProcessDataByMultiply(currentData, attributes);
    }

    private static void ProcessDataBySize(ref int currentData, Dictionary<string, string> attributes)
    {
        if (attributes.TryGetValue("size", out string size))
        {
            currentData = ConvertDataBySize(currentData, size);
        }
    }

    private static string ProcessDataByMultiply(int currentData, Dictionary<string, string> attributes)
    {
        if (attributes.TryGetValue("multiply", out string multiplyStr) && float.TryParse(multiplyStr, out float multiplyValue))
        {
            float adjustedData = currentData / multiplyValue;
            return attributes.TryGetValue("unit", out string unitStr) ? $"{adjustedData}{unitStr}" : adjustedData.ToString();
        }
        return currentData.ToString();
    }

    private static void SetTitleAndValue(TextMeshProUGUI titleText, TextMeshProUGUI valueText, string processedData, int currentData, Dictionary<string, string> attributes)
    {
        ProcessDataBySize(ref currentData, attributes);
        if (attributes.TryGetValue("changeTargetAdd", out string changeTargetAddStr) && int.TryParse(changeTargetAddStr, out int changeTargetAddr))
        {
            int changeTargetAddrData = ManufacturingData.Instance.parsedPollingData[changeTargetAddr - 200];
            if (attributes.TryGetValue("nameList", out string nameList))
            {
                string[] names = nameList.Split(',');
                if (changeTargetAddrData >= 0 && changeTargetAddrData < names.Length)
                {
                    titleText.text = names[changeTargetAddrData];
                    valueText.text = processedData;
                }
                else
                {
                    titleText.text = names[0];
                    valueText.text = processedData;
                }
            }
        }
        else if (attributes.TryGetValue("name", out string nameAttrValue) && !string.IsNullOrEmpty(nameAttrValue))
        {
            titleText.text = nameAttrValue;
            if (attributes.TryGetValue("itemList", out string itemList))
            {
                string[] items = itemList.Split(',');
                valueText.text = (processedData != null) ? items[Convert.ToInt32(processedData)] : "null";
            }
            else
            {
                valueText.text = processedData;
            }
        }
        else
        {
            titleText.text = "null";
            valueText.text = processedData;
        }

        if (attributes.TryGetValue("notuse", out string notuseStr) && int.TryParse(notuseStr, out int notuseValue))
        {
            if (currentData == notuseValue)
            {
                valueText.text = "������(" + currentData + ")";
            }
        }
    }

    private static void OpenSettingDetailItemView()
    {
        if (selectedDetailSettingName.StartsWith("btn_SettingDetailItem_"))
        {
            string tmp = selectedDetailSettingName.Substring("btn_SettingDetailItem_".Length);
            int index = tmp.IndexOf('_');
            if (index != -1)
            {
                selectedSettingAddr = tmp.Substring(0, index);
            }
            else
            {
                selectedSettingAddr = tmp;
            }
        }

        // selectedSettingAdd�� ���� ���� Ư�� ������ �Ӽ��� ����
        Dictionary<string, string> attributes = xmlParser.GetTagAttributesByAddr(xmlParser.xmlContent, selectedSettingAddr);

        // Ư�� ������ �Ӽ��� �� selectedSettingAddr�� XML�� tag �±��� addr �Ӽ� ���� ��ġ�ϸ� �ش� �±��� style �Ӽ����� ������
        if (attributes != null && attributes.TryGetValue("style", out string styleValue))
        {
            strStyleValue = styleValue;
            switch (styleValue)
            {
                case "numpad":
                    //Debug.Log("numpad");
                    obj_SettingNumpad.SetActive(true);
                    txt_TitleNumPad.text = selectedDetailSettingTitle;
                    txt_ValueNumPad.text = selectedDetailSettingValue;

                    if(attributes.TryGetValue("lower", out string lowerValue) && attributes.TryGetValue("upper", out string upperValue) && attributes.TryGetValue("unit", out string unitValue))
                    {
                        float multiplyValue;
                        strUnitValue = unitValue;
                        if (attributes.TryGetValue("multiply", out string multiplyStr) && float.TryParse(multiplyStr, out multiplyValue))
                        {
                            float lowerData = int.Parse(lowerValue) / multiplyValue;
                            //Debug.Log($"{lowerData} = {lowerValue} / {multiplyValue}");
                            float upperData = int.Parse(upperValue) / multiplyValue;
                            //Debug.Log($"{upperData} = {upperValue} / {multiplyValue}");
                            txt_ValueRange.text = $"{lowerData} ~ {upperData}{unitValue}";
                        }                        

                        // ���� �� �ִ�, �ּҰ��� Ư�� Addr ���� ���� �����Ǵ� ���
                        if(attributes.TryGetValue("targetLowerAddr", out string targetLowerAddr))
                        {
                            var currentLowerData = ManufacturingData.Instance.parsedPollingData[Convert.ToInt32(targetLowerAddr) - 200];
                            
                            if (float.TryParse(multiplyStr, out multiplyValue))
                            {
                                float adjustedData = currentLowerData / multiplyValue;
                                txt_ValueRange.text = $"{adjustedData} ~ {upperValue}{unitValue}";
                            }
                        }
                        else if (attributes.TryGetValue("targetUpperAddr", out string targetUpperAddr))
                        {
                            var currentUpperData = ManufacturingData.Instance.parsedPollingData[Convert.ToInt32(targetUpperAddr) - 200];
                            if (float.TryParse(multiplyStr, out multiplyValue))
                            {
                                float adjustedData = currentUpperData / multiplyValue;
                                txt_ValueRange.text = $"{lowerValue} ~ {adjustedData}{unitValue}";
                            }                            
                        }
                    }

                    // xml�� ������ ���ΰ� ���� ���
                    b_Value_NotUse = attributes.ContainsKey("notuse");
                    if (b_Value_NotUse)
                    {
                        toggle_Value_NotUse.interactable = true;
                        var currentselectedSettingAddrData = ManufacturingData.Instance.parsedPollingData[Convert.ToInt32(selectedSettingAddr) - 200];
                        float multiplyValue;
                        if (attributes.TryGetValue("multiply", out string multiplyStr) && float.TryParse(multiplyStr, out multiplyValue))
                        {
                            if (attributes.TryGetValue("notuse", out string notuseValue))
                            {
                                if (attributes.TryGetValue("size", out string size))
                                {
                                    string processedData = ConvertDataBySize(currentselectedSettingAddrData, size).ToString();
                                    if (processedData == notuseValue)
                                    {
                                        toggle_Value_NotUse.isOn = true;
                                    }
                                }                                
                            }
                            else
                            {
                                toggle_Value_NotUse.isOn = false;
                            }
                        }
                    }
                    else
                    {
                        toggle_Value_NotUse.interactable = false;
                    }
                    break;
                case "dropdown":
                    //Debug.Log("dropdown");
                    obj_SettingDropdown.SetActive(true);
                    txt_TitleDropdown.text = selectedDetailSettingTitle;
                    txt_ValueDropdown.text = selectedDetailSettingValue;

                    if (attributes.TryGetValue("itemList", out string itemList))
                    {
                        string[] items = itemList.Split(',');

                        // Dropdown ������Ʈ�� options ����� Ŭ����
                        dropdown_ItemList.options.Clear();

                        // �� �������� Dropdown�� options ��Ͽ� �߰�
                        foreach (var item in items)
                        {
                            dropdown_ItemList.options.Add(new TMP_Dropdown.OptionData(item.Trim()));
                        }

                        // Dropdown�� ���� ������Ʈ
                        dropdown_ItemList.RefreshShownValue();
                    }
                    break;
                case "toggle":
                    //Debug.Log("toggle");
                    obj_SettingToggle.SetActive(true);
                    txt_TitleToggle.text = selectedDetailSettingTitle;
                    txt_ValueToggle.text = selectedDetailSettingValue;
                    toggle_Value_Toggle.isOn = (selectedDetailSettingBitValue == 1.ToString()) ? true : false;                    
                    break;
            }
        }        
    }

    public void OnToggleBitValueChanged()
    {
        if (toggle_Value_Toggle.isOn)
        {
            txt_ValueToggle.text = strTrueBitValue;
        }
        else
        {
            txt_ValueToggle.text = strFalseBitValue;
        }
    }


    public void CloseSettingDetailItemView()
    {
        switch(strStyleValue)
        {
            case "numpad":
                obj_SettingNumpad.SetActive(false);
                toggle_Value_NotUse.isOn = false;
                break;
            case "dropdown":
                obj_SettingDropdown.SetActive(false);
                break;
            case "toggle":
                obj_SettingToggle.SetActive(false);
                break;
        }
    }


    private static void SendCommand()
    {

    }



    public static void ReceiveSettingItemButtonData(string buttonName, string buttonTitle)
    {
        selectedSettingName = buttonName;
        selectedSettingTitle = buttonTitle;
        ProcessingSetDetailGroupDataWithXML();
    }

    public static void ReceiveSettingDetailItemButtonData(string buttonName, string buttonTitle, string buttonValue, string bitValue)
    {
        selectedDetailSettingName = buttonName;
        selectedDetailSettingTitle = buttonTitle;
        selectedDetailSettingValue = buttonValue;
        selectedDetailSettingBitValue = bitValue;
        OpenSettingDetailItemView();
    }

    public void ReturnSettingMain()
    {
        txt_SettingTitle.text = "����";
        btn_PrevToMain.gameObject.SetActive(false);
        ObjectPool.Instance.ReturnSettingDetailItemObject();
        ObjectPool.Instance.ReactivationSettingItemObject();
    }
}