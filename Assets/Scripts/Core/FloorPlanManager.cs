using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

[Serializable]
public class FPPStyle
{
    public string[] trend;
    public string showSetValue;
    public string style;
    public string color;
}

public class FloorPlanManager : MonoBehaviour
{
    ControllerStatus status;
    private WaitForSeconds updateInterval = new(2); // DV UI ������Ʈ ����
    private bool isFPPOpen = false;
    public GameObject obj_Loading;
    bool isLoading = true;

    int[] parsedPollingData = null;

    public List<Sprite> defrostImgList = new List<Sprite>();
    public List<Sprite> fanImgList = new List<Sprite>();
    private Dictionary<GameObject, Coroutine> defrostCoroutines = new Dictionary<GameObject, Coroutine>();
    //private Dictionary<GameObject, Coroutine> coolCoroutines = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, Coroutine> fanCoroutines = new Dictionary<GameObject, Coroutine>();
    private WaitForSeconds sec0_1 = new WaitForSeconds(0.15f);


    public static Dictionary<string, GameObject> trendToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> fpImageInstances = new Dictionary<string, GameObject>();

    [Header("��鵵 �̹��� �� ���ϴ� ��� ����")]
    public GameObject floorPlanScreen;
    public GameObject fpScrollView;
    public Transform fpContent;
    public TMP_Dropdown dropdown_HighGroup;
    public GameObject floorPlanScrollView;
    public TextMeshProUGUI txtTotalControllerCnt;
    public TextMeshProUGUI txtOnControllerCnt;
    public TextMeshProUGUI txtOffControllerCnt;
    public TextMeshProUGUI txtDefControllerCnt;

    [Header("��鵵 �г� �̸�����")]
    public Sprite basicBlack;
    public Sprite basicBlue;
    public Sprite basicGreen;
    public Sprite basicNavy;
    public Sprite basicRed;
    public Sprite basicYellow;
    public Sprite basicSetValueBlack;
    public Sprite basicSetValueBlue;
    public Sprite basicSetValueGreen;
    public Sprite basicSetValueNavy;
    public Sprite basicSetValueRed;
    public Sprite basicSetValueYellow;
    public Sprite bigBlack;
    public Sprite bigBlue;
    public Sprite bigGreen;
    public Sprite bigNavy;
    public Sprite bigRed;
    public Sprite bigYellow;
    public Sprite bigSetValueBlack;
    public Sprite bigSetValueBlue;
    public Sprite bigSetValueGreen;
    public Sprite bigSetValueNavy;
    public Sprite bigSetValueRed;
    public Sprite bigSetValueYellow;


    [Header("��鵵 �г� ���� ����")]
    public GameObject fppSettingScreen;
    public GameObject trendListScrollView;
    public GameObject colorParent;
    public Transform trendListContent;
    private Toggle toggleAllCategory;
    private Toggle toggleShowSetValue; // ������ǥ��
    private Toggle toggleStyleDefault; // �⺻��Ÿ��
    private Toggle toggleStyleBigText; // ū�۾���Ÿ��
    private Toggle toggleStyleSingleItem; // ���� �׸�
    private Toggle toggleColorGreen; // ��鵵 ���� �ʷ�
    private Toggle toggleColorYellow; // ��鵵 ���� ���
    private Toggle toggleColorBlue; // ��鵵 ���� �Ķ�
    private Toggle toggleColorPurple; // ��鵵 ���� ����
    private Toggle toggleColorRed; // ��鵵 ���� ����
    private Toggle toggleColorBlack; // ��鵵 ���� ����
    private Image imgExampleFPP;
    private Image imgExampleFPPSingle;
    private Button btnClose;
    private Button btnSave;
    private Button btnModify;
    private Button btnDelete;

    [Header("��鵵 �г� ���� ����")]
    //public static Dictionary<string, GameObject> fppMinInstances = new Dictionary<string, GameObject>();
    //public static Dictionary<string, GameObject> fppMaxInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> fppMaxSingleItemInstances = new Dictionary<string, GameObject>();
    //public GameObject fppMinInstance;
    //public GameObject fppMaxInstance;
    public GameObject fppMaxSingleItemInstance;

    [Header("��鵵 �г� ��Ÿ�� ����")]
    public static Dictionary<string, GameObject> basicStyleInstances = new Dictionary<string, GameObject>();
    //public static Dictionary<string, GameObject> basicSetValStyleInstances = new Dictionary<string, GameObject>();
    //public static Dictionary<string, GameObject> bigTextStyleInstances = new Dictionary<string, GameObject>();
    //public static Dictionary<string, GameObject> bigTextSetValStyleInstances = new Dictionary<string, GameObject>();
    private Transform basicStyleContent;
    //private Transform basicSetValStyleContent;
    //private Transform bigTextStyleContent;
    //private Transform bigTextSetValStyleContent;
    public GameObject basicStyleElementPrefab;
    //public GameObject basicSetValStyleElementPrefab;
    //public GameObject bigTextStyleElementPrefab;
    //public GameObject bigTextSetValStyleElementPrefab;
    public Sprite spriteNoFix;
    public Sprite spriteGreenFix;
    public Sprite spriteYellowFix;
    public Sprite spriteBlueFix;
    public Sprite spritePurpleFix;
    public Sprite spriteRedFix;
    public Sprite spriteBlackFix;
    private string selectedTrend = string.Empty;
    private string selectedShowSetValue = string.Empty;
    private string selectedStyle = string.Empty;
    private string selectedColor = string.Empty;

    public Sprite minFPP_Normal;
    public Sprite minFPP_Cool;
    public Sprite minFPP_Defrost;
    public Sprite minFPP_Alarm;
    public Sprite minFPP_Stop;



    Color alarmColor = new Color(240 / 255f, 94 / 255f, 91 / 255f, 1f); // f05e33, ���� 1
    Color defrostColor = new Color(32 / 255f, 191 / 255f, 209 / 255f, 1f); // 20BFD1, ���� 1
    Color runColor = new Color(0 / 255f, 178 / 255f, 127 / 255f, 1f); // 00B27f, ���� 1
    Color colorBlue = new Color(0 / 255f, 169 / 255f, 196 / 255f, 1f);          // Blue #00A9C4
    Color stopColor = new Color(173 / 255f, 173 / 255f, 173 / 255f, 1f); // ADADAD, ���� 1
    Color normalBlackColor = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1f); // 2D2D2D, ���� 1
    Color stopBGColor = new Color(227 / 255f, 227 / 255f, 227 / 255f, 1f); // E3E3E3, ���� 1
    Color noColor = new Color(0f, 0f, 0f, 0f); // ffffff, ���� 0
    Color normalTrendColor = new Color(116 / 255f, 178 / 255f, 8 / 255f, 1f); // 74B208, ���� 1

    Color themeGreen = new Color(116 / 255f, 178 / 255f, 8 / 255f, 1f); // 74b208, ���� 1
    Color themeYellow = new Color(255 / 255f, 205 / 255f, 0 / 255f, 1f); // ffcd00, ���� 1
    Color themeBlue = new Color(0 / 255f, 169 / 255f, 196 / 255f, 1f); // 00a9c4, ���� 1
    Color themePurple = new Color(85 / 255f, 100 / 255f, 188 / 255f, 1f); // 5564bc, ���� 1
    Color themeRed = new Color(240 / 255f, 94 / 255f, 91 / 255f, 1f); // f05e33, ���� 1
    Color themeBlack = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1f); // 2d2d2d, ���� 1

    public static string currentSelectedHGID = string.Empty;

    public static FloorPlanManager Instance { get; private set; }
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

        fpContent = fpScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();

        dropdown_HighGroup = floorPlanScreen.transform.Find("UpperControlBar/Dropdown_HighGroup").GetComponent<TMP_Dropdown>();
        floorPlanScrollView = floorPlanScreen.transform.Find("FPScrollView").gameObject;
        txtTotalControllerCnt = floorPlanScreen.transform.Find("BottomControlBar/Bottom_Total/txtTotalCount").GetComponent<TextMeshProUGUI>();
        txtOnControllerCnt = floorPlanScreen.transform.Find("BottomControlBar/Bottom_On/txtOnCount").GetComponent<TextMeshProUGUI>();
        txtOffControllerCnt = floorPlanScreen.transform.Find("BottomControlBar/Bottom_Off/txtOffCount").GetComponent<TextMeshProUGUI>();
        txtDefControllerCnt = floorPlanScreen.transform.Find("BottomControlBar/Bottom_Def/txtDefCount").GetComponent<TextMeshProUGUI>();

        trendListContent = trendListScrollView.transform.Find("Viewport/TrendListContent").GetComponent<Transform>();
        toggleAllCategory = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/TrendToggleParent/Toggle_AllCategory").GetComponent<Toggle>();
        imgExampleFPP = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/ExampleParent/Example/Image").GetComponent<Image>();
        imgExampleFPPSingle = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/ExampleParent/Example/ImageSingle").GetComponent<Image>();

        btnClose = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/btn_Close").GetComponent<Button>();
        btnSave = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Bottom/BottomButtonParent/btn_Save").GetComponent<Button>();
        btnModify = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Bottom/BottomButtonParent/btn_Modify").GetComponent<Button>();
        btnDelete = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Bottom/BottomButtonParent/btn_Remove").GetComponent<Button>();

        btnClose.onClick.AddListener(() => CloseFPPSetting());

        toggleShowSetValue = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/StyleList/Top/Toggle_ShowSetValue").GetComponent<Toggle>();
        toggleStyleDefault = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/StyleList/Center/Toggle_Default").GetComponent<Toggle>();
        toggleStyleBigText = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/StyleList/Center/Toggle_BigText").GetComponent<Toggle>();
        toggleStyleSingleItem = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/StyleList/Center/Toggle_SingleItem").GetComponent<Toggle>();
        toggleColorGreen = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/ColorParent/ToggleContainer_MainView/Toggle_Green").GetComponent<Toggle>();
        toggleColorYellow = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/ColorParent/ToggleContainer_MainView/Toggle_Yellow").GetComponent<Toggle>();
        toggleColorBlue = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/ColorParent/ToggleContainer_MainView/Toggle_Blue").GetComponent<Toggle>();
        toggleColorPurple = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/ColorParent/ToggleContainer_MainView/Toggle_Purple").GetComponent<Toggle>();
        toggleColorRed = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/ColorParent/ToggleContainer_MainView/Toggle_Red").GetComponent<Toggle>();
        toggleColorBlack = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/FPPAdjust/StyleToggleParent/ColorParent/ToggleContainer_MainView/Toggle_Black").GetComponent<Toggle>();
        toggleShowSetValue.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleStyleDefault.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleStyleBigText.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleStyleSingleItem.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleColorGreen.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleColorYellow.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleColorBlue.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleColorPurple.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleColorRed.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
        toggleColorBlack.onValueChanged.AddListener(isOn => { UpdateExampleImage(); });
    }

    private void ParsePollingData(string iid, string cid)
    {
        // ���ǿ� �´� ���� �����͸� Ȱ���� UI�� ������Ʈ�Ѵ�.
        foreach (DataRow row in ClientDatabase.realTimeData.Tables[0].Rows)
        {
            if (row["ID"].ToString() == iid && (row["CID"].ToString() == cid))
            {
                string[] stringData = row["STR_DATA"].ToString().Split(',');

                parsedPollingData = new int[stringData.Length];

                for (int i = 0; i < stringData.Length; i++)
                {
                    if (!int.TryParse(stringData[i], out parsedPollingData[i])) // int�� ��ȯ �õ�
                    {
                        //Debug.LogError($"Failed to parse value at index {i}: {stringData[i]}");
                        parsedPollingData[i] = -999; // ������ ��� �⺻������ -999�� ����մϴ�.
                    }
                }
            }

        }
    }

    // ��鵵 �г� ���� ����
    public void SaveFPPSettings(string command, string iid, string cid)
    {
        switch (command)
        {
            case "Add":
                // ������ ǥ��
                if (toggleStyleSingleItem.isOn)
                    selectedShowSetValue = "n";
                else
                    selectedShowSetValue = toggleShowSetValue.isOn ? "y" : "n";

                // �г� ��Ÿ��
                if (toggleStyleDefault.isOn)
                    selectedStyle = "Default";                
                if(toggleStyleBigText.isOn)
                    selectedStyle = "BigText";
                if (toggleStyleSingleItem.isOn)
                    selectedStyle = "SingleItem";

                // �г� ���� ��Ÿ��
                if (toggleStyleSingleItem.isOn)
                {
                    selectedColor = "Blue";
                }
                else
                {
                    if (toggleColorGreen.isOn)
                        selectedColor = "Green";
                    if (toggleColorYellow.isOn)
                        selectedColor = "Yellow";
                    if (toggleColorBlue.isOn)
                        selectedColor = "Blue";
                    if (toggleColorPurple.isOn)
                        selectedColor = "Purple";
                    if (toggleColorRed.isOn)
                        selectedColor = "Red";
                    if (toggleColorBlack.isOn)
                        selectedColor = "Black";
                }                

                // ��Ʈ�ѷ��� ���õ� Ʈ���� ��� ����
                List<string> selectedTogglesInfo = new List<string>();
                foreach (Transform toggleTransform in trendListContent)
                {
                    Toggle toggle = toggleTransform.GetComponent<Toggle>();
                    if (toggle != null && toggle.isOn)
                    {
                        string selectedTrendAddr = toggle.gameObject.name.Replace("TrendToggle_", "");
                        string selectedTrendName = toggle.transform.Find("txtTrendName").GetComponent<TextMeshProUGUI>().text;
                        selectedTogglesInfo.Add($"\"{selectedTrendAddr}|{selectedTrendName}\"");
                    }
                }

                // selectedTogglesInfo ����Ʈ�� JSON �迭 ���ڿ��� ��ȯ
                string trendsJsonArray = "[" + string.Join(",", selectedTogglesInfo) + "]";
                string jsonString = $"{{\"trend\":{trendsJsonArray},\"showSetValue\":\"{selectedShowSetValue}\",\"style\":\"{selectedStyle}\",\"color\":\"{selectedColor}\"}}";

                // ��鵵 �̹����� �����ǥ ���� - �ʱ⿡�� �߾ӿ� ����
                DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
                DataTable tableController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
                string hgid = string.Empty;
                string imgPath = string.Empty;
                string imgWidth = string.Empty;
                string imgHeight = string.Empty;

                foreach (DataRow row in tableController.Rows)
                {
                    if (row["ID"].ToString() == iid && row["CID"].ToString() == cid)
                    {
                        hgid = row["HGID"].ToString();
                    }
                }
                foreach (DataRow row in tableHighGroup.Rows)
                {
                    if (row["FLD_HGID"].ToString() == hgid)
                    {
                        hgid = row["FLD_HGID"].ToString();
                        imgPath = row["FLD_IMG_PATH"].ToString();
                        imgWidth = row["FLD_IMG_WIDTH"].ToString();
                        imgHeight = row["FLD_IMG_HEIGHT"].ToString();
                    }
                }

                float fppX = 0;
                float fppY = 0;
                string fppQuery = $"UPDATE TBL_CONTROLLER SET FPP_GEN = 1, FPP_STYLE = '{jsonString}', FPP_X = '{fppX}', FPP_Y = '{fppY}' WHERE ID = '{iid}' AND CID = '{cid}'";

                if (ClientDatabase.OnUpdateRequest(fppQuery))
                {
                    foreach (var fppImg in fpImageInstances)
                    {
                        if (fppImg.Value.name.Replace("FPIMG_", "") == hgid)
                            GenerateFPP(fppImg.Value, hgid);
                    }

                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�г��� �߰��Ǿ����ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); CloseFPPSetting(); });

                    //Debug.Log($"��鵵 �г� �߰� �Ϸ� : {iid}, {cid}, {jsonString}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�г� �߰��� �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); CloseFPPSetting(); });
                    //Debug.Log($"��鵵 �г� �߰� ���� : {iid}, {cid}, {jsonString}");
                }
                break;
            case "Modify":
                // ������ ǥ��
                if (toggleStyleSingleItem.isOn)
                    selectedShowSetValue = "n";
                else
                    selectedShowSetValue = toggleShowSetValue.isOn ? "y" : "n";

                // �г� ��Ÿ��
                if (toggleStyleDefault.isOn)
                    selectedStyle = "Default";
                if (toggleStyleBigText.isOn)
                    selectedStyle = "BigText";
                if (toggleStyleSingleItem.isOn)
                    selectedStyle = "SingleItem";

                // �г� ���� ��Ÿ��
                if (toggleStyleSingleItem.isOn)
                {
                    selectedColor = "Blue";
                }
                else
                {
                    if (toggleColorGreen.isOn)
                        selectedColor = "Green";
                    if (toggleColorYellow.isOn)
                        selectedColor = "Yellow";
                    if (toggleColorBlue.isOn)
                        selectedColor = "Blue";
                    if (toggleColorPurple.isOn)
                        selectedColor = "Purple";
                    if (toggleColorRed.isOn)
                        selectedColor = "Red";
                    if (toggleColorBlack.isOn)
                        selectedColor = "Black";
                }

                // ��Ʈ�ѷ��� ���õ� Ʈ���� ��� ����
                List<string> selectedTogglesInfoModify = new List<string>();
                foreach (Transform toggleTransform in trendListContent)
                {
                    Toggle toggle = toggleTransform.GetComponent<Toggle>();
                    if (toggle != null && toggle.isOn)
                    {
                        string selectedTrendAddr = toggle.gameObject.name.Replace("TrendToggle_", "");
                        string selectedTrendName = toggle.transform.Find("txtTrendName").GetComponent<TextMeshProUGUI>().text;
                        selectedTogglesInfoModify.Add($"\"{selectedTrendAddr}|{selectedTrendName}\"");
                    }
                }

                // selectedTogglesInfo ����Ʈ�� JSON �迭 ���ڿ��� ��ȯ
                string trendsJsonArrayModify = "[" + string.Join(",", selectedTogglesInfoModify) + "]";
                string jsonStringModify = $"{{\"trend\":{trendsJsonArrayModify},\"showSetValue\":\"{selectedShowSetValue}\",\"style\":\"{selectedStyle}\",\"color\":\"{selectedColor}\"}}";
                string jsonStringBefore = string.Empty;

                // ��鵵 �̹����� �����ǥ ���� - �ʱ⿡�� �߾ӿ� ����
                DataTable tableControllerModify = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
                string hgidModify = string.Empty;

                foreach (DataRow row in tableControllerModify.Rows)
                {
                    if (row["ID"].ToString() == iid && row["CID"].ToString() == cid)
                    {
                        hgidModify = row["HGID"].ToString();
                        jsonStringBefore = row["FPP_STYLE"].ToString();
                    }
                }

                // Ʈ���� �׸� ���泻���� json���� ��
                FPPStyle fppStyleModify = JsonUtility.FromJson<FPPStyle>(jsonStringModify);
                FPPStyle fppStyleBefore = JsonUtility.FromJson<FPPStyle>(jsonStringBefore);
                List<string> arrModifyAddr = new List<string>();
                List<string> arrBeforeAddr = new List<string>();
                foreach (string trendEntry in fppStyleModify.trend)
                {
                    string[] parts = trendEntry.Split('|');
                    if (parts.Length == 2)
                    {
                        string modifyAddr = parts[0];
                        arrModifyAddr.Add(modifyAddr);
                    }
                }
                foreach (string trendEntry in fppStyleBefore.trend)
                {
                    string[] parts = trendEntry.Split('|');
                    if (parts.Length == 2)
                    {
                        string beforeAddr = parts[0];
                        arrBeforeAddr.Add(beforeAddr);
                    }
                }

                // fppStyleBefore������ �����ϴ� �׸� ã��
                var missingInModify = arrBeforeAddr.Except(arrModifyAddr).ToList();

                if (missingInModify.Count > 0)
                {
                    // fppStyleBefore ��� fppStyleModify���� ������ �׸�� ���
                    foreach (var modifyAddr in missingInModify)
                    {
                        List<string> keysToRemove = new List<string>();

                        // ������ Ű�� ã��
                        foreach (var key in basicStyleInstances.Keys)
                        {
                            if (key.StartsWith($"BasicStyleElement_{iid}_{cid}_"))
                            {
                                keysToRemove.Add(key);
                                Destroy(basicStyleInstances[key]);
                            }
                        }
                        //foreach (var key in basicSetValStyleInstances.Keys)
                        //{
                        //    if (key.StartsWith($"BasicSetValStyleElement_{iid}_{cid}_"))
                        //    {
                        //        keysToRemove.Add(key);
                        //        Destroy(basicSetValStyleInstances[key]);
                        //    }
                        //}
                        //foreach (var key in bigTextStyleInstances.Keys)
                        //{
                        //    if (key.StartsWith($"BigTextStyleElement_{iid}_{cid}_"))
                        //    {
                        //        keysToRemove.Add(key);
                        //        Destroy(bigTextStyleInstances[key]);
                        //    }
                        //}
                        //foreach (var key in bigTextSetValStyleInstances.Keys)
                        //{
                        //    if (key.StartsWith($"BigTextSetValStyleElement_{iid}_{cid}_"))
                        //    {
                        //        keysToRemove.Add(key);
                        //        Destroy(bigTextSetValStyleInstances[key]);
                        //    }
                        //}
                        foreach (var key in keysToRemove)
                        {
                            basicStyleInstances.Remove(key);
                            //basicSetValStyleInstances.Remove(key);
                            //bigTextStyleInstances.Remove(key);
                            //bigTextSetValStyleInstances.Remove(key);
                        }
                    }
                }

                string fppQueryModify = $"UPDATE TBL_CONTROLLER SET FPP_GEN = 1, FPP_STYLE = '{jsonStringModify}' WHERE ID = '{iid}' AND CID = '{cid}'";

                if (ClientDatabase.OnUpdateRequest(fppQueryModify))
                {
                    foreach (var fppImg in fpImageInstances)
                    {
                        if (fppImg.Value.name.Replace("FPIMG_", "") == hgidModify)
                            GenerateFPP(fppImg.Value, hgidModify);
                    }
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�г� ������ �Ϸ�Ǿ����ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); CloseFPPSetting(); });
                    //Debug.Log($"��鵵 �г� ���� �Ϸ� : {iid}, {cid}, {jsonStringModify}");
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�г� ������ �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); CloseFPPSetting(); });
                    //Debug.Log($"��鵵 �г� ���� ���� : {iid}, {cid}, {jsonStringModify}");
                }
                break;
            case "Remove":
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
                ScreenManager.Instance.txt_PopUpMsg.text = $"��鵵 �г��� �����Ͻðڽ��ϱ�?";

                ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET FPP_GEN = 0, FPP_MINMAX = 0, FPP_FIX = 1, FPP_STYLE = '', FPP_X = '', FPP_Y = '' WHERE ID = {iid} AND CID = {cid};";
                    if (ClientDatabase.OnUpdateRequest(tblControllerQuery))
                    {
                        DataTable tableController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
                        string hgid = string.Empty;

                        foreach (DataRow row in tableController.Rows)
                        {
                            if (row["ID"].ToString() == iid && row["CID"].ToString() == cid)
                            {
                                hgid = row["HGID"].ToString();
                            }
                        }

                        //Destroy(fppMinInstances[$"FPPMinInstance_{hgid}_{iid}_{cid}"]);
                        //Destroy(fppMaxInstances[$"FPPMaxInstance_{hgid}_{iid}_{cid}"]);
                        Destroy(fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"]);

                        //fppMinInstances.Remove($"FPPMinInstance_{hgid}_{iid}_{cid}");
                        //fppMaxInstances.Remove($"FPPMaxInstance_{hgid}_{iid}_{cid}");
                        fppMaxSingleItemInstances.Remove($"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}");

                        List<string> keysToRemove = new List<string>();

                        // ������ Ű�� ã��
                        foreach (var key in basicStyleInstances.Keys)
                        {
                            if (key.StartsWith($"BasicStyleElement_{iid}_{cid}_"))
                            {
                                keysToRemove.Add(key);
                            }
                        }
                        //foreach (var key in basicSetValStyleInstances.Keys)
                        //{
                        //    if (key.StartsWith($"BasicSetValStyleElement_{iid}_{cid}_"))
                        //    {
                        //        keysToRemove.Add(key);
                        //    }
                        //}
                        //foreach (var key in bigTextStyleInstances.Keys)
                        //{
                        //    if (key.StartsWith($"bigTextStyleElement_{iid}_{cid}_"))
                        //    {
                        //        keysToRemove.Add(key);
                        //    }
                        //}
                        //foreach (var key in bigTextSetValStyleInstances.Keys)
                        //{
                        //    if (key.StartsWith($"bigTextSetValStyleElement_{iid}_{cid}_"))
                        //    {
                        //        keysToRemove.Add(key);
                        //    }
                        //}
                        foreach (var key in keysToRemove)
                        {
                            basicStyleInstances.Remove(key);
                            //basicSetValStyleInstances.Remove(key);
                            //bigTextStyleInstances.Remove(key);
                            //bigTextSetValStyleInstances.Remove(key);
                        }



                        foreach (var fppImg in fpImageInstances)
                        {
                            if (fppImg.Value.name.Replace("FPIMG_", "") == hgid)
                                GenerateFPP(fppImg.Value, hgid);
                        }

                        CloseFPPSetting();
                        ScreenManager.Instance.ClosePopUpMessage();
                        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();

                        LayoutRebuilder.ForceRebuildLayoutImmediate(fpContent.GetComponent<RectTransform>());
                        Canvas.ForceUpdateCanvases();

                        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                        ScreenManager.Instance.txt_PopUpMsg.text = "�г��� �����Ǿ����ϴ�.";
                        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); });
                        //Debug.Log($"��鵵 �г� ���� �Ϸ� : {iid}, {cid}");
                    }
                    else
                    {
                        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                        ScreenManager.Instance.txt_PopUpMsg.text = "�г� ������ �����߽��ϴ�.";
                        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); });
                        //Debug.Log($"��鵵 �г� ���� ���� : {iid}, {cid}");
                    }
                });
                break;
        }
    }

    // ���� �̹��� ������Ʈ
    public void UpdateExampleImage()
    {
        if (toggleStyleSingleItem.isOn)
        {
            toggleShowSetValue.gameObject.SetActive(false);
            imgExampleFPP.gameObject.SetActive(false);
            imgExampleFPPSingle.gameObject.SetActive(true);

            foreach(var toggle in trendToggleInstances)
            {
                toggle.Value.GetComponent<Toggle>().isOn = false;
            }

            foreach (Transform child in colorParent.transform)
            {                
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            toggleShowSetValue.gameObject.SetActive(true);
            imgExampleFPP.gameObject.SetActive(true);
            imgExampleFPPSingle.gameObject.SetActive(false);

            foreach (Transform child in colorParent.transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        if (toggleShowSetValue.isOn)
        {
            if (toggleStyleDefault.isOn && !toggleStyleBigText.isOn)
            {
                if (toggleColorGreen.isOn)
                {
                    imgExampleFPP.sprite = basicSetValueGreen; // ������on, �⺻��Ÿ��, �ʷ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Green"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorYellow.isOn)
                {
                    imgExampleFPP.sprite = basicSetValueYellow; // ������on, �⺻��Ÿ��, ���
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Yellow"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlue.isOn)
                {
                    imgExampleFPP.sprite = basicSetValueBlue; // ������on, �⺻��Ÿ��, �Ķ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Blue"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorPurple.isOn)
                {
                    imgExampleFPP.sprite = basicSetValueNavy; // ������on, �⺻��Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Purple"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorRed.isOn)
                {
                    imgExampleFPP.sprite = basicSetValueRed; // ������on, �⺻��Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Red"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlack.isOn)
                {
                    imgExampleFPP.sprite = basicSetValueBlack; // ������on, �⺻��Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Black"; // Green, Yellow, Blue, Purple, Red, Black
                }
            }
            else
            {
                if (toggleColorGreen.isOn)
                {
                    imgExampleFPP.sprite = bigSetValueGreen; // ������on, ū���ڽ�Ÿ��, �ʷ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Green"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorYellow.isOn)
                {
                    imgExampleFPP.sprite = bigSetValueYellow; // ������on, ū���ڽ�Ÿ��, ���
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Yellow"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlue.isOn)
                {
                    imgExampleFPP.sprite = bigSetValueBlue; // ������on, ū���ڽ�Ÿ��, �Ķ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Blue"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorPurple.isOn)
                {
                    imgExampleFPP.sprite = bigSetValueNavy; // ������on, ū���ڽ�Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Purple"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorRed.isOn)
                {
                    imgExampleFPP.sprite = bigSetValueRed; // ������on, ū���ڽ�Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Red"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlack.isOn)
                {
                    imgExampleFPP.sprite = bigSetValueBlack; // ������on, ū���ڽ�Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Black"; // Green, Yellow, Blue, Purple, Red, Black
                }
            }
        }
        else
        {
            if (toggleStyleDefault.isOn && !toggleStyleBigText.isOn)
            {
                if (toggleColorGreen.isOn)
                {
                    imgExampleFPP.sprite = basicGreen; // ������on, �⺻��Ÿ��, �ʷ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Green"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorYellow.isOn)
                {
                    imgExampleFPP.sprite = basicYellow; // ������on, �⺻��Ÿ��, ���
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Yellow"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlue.isOn)
                {
                    imgExampleFPP.sprite = basicBlue; // ������on, �⺻��Ÿ��, �Ķ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Blue"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorPurple.isOn)
                {
                    imgExampleFPP.sprite = basicNavy; // ������on, �⺻��Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Purple"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorRed.isOn)
                {
                    imgExampleFPP.sprite = basicRed; // ������on, �⺻��Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Red"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlack.isOn)
                {
                    imgExampleFPP.sprite = basicBlack; // ������on, �⺻��Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Black"; // Green, Yellow, Blue, Purple, Red, Black
                }
            }
            else
            {
                if (toggleColorGreen.isOn)
                {
                    imgExampleFPP.sprite = bigGreen; // ������on, ū���ڽ�Ÿ��, �ʷ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Green"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorYellow.isOn)
                {
                    imgExampleFPP.sprite = bigYellow; // ������on, ū���ڽ�Ÿ��, ���
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Yellow"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlue.isOn)
                {
                    imgExampleFPP.sprite = bigBlue; // ������on, ū���ڽ�Ÿ��, �Ķ�
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Blue"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorPurple.isOn)
                {
                    imgExampleFPP.sprite = bigNavy; // ������on, ū���ڽ�Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Purple"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorRed.isOn)
                {
                    imgExampleFPP.sprite = bigRed; // ������on, ū���ڽ�Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Red"; // Green, Yellow, Blue, Purple, Red, Black
                }

                if (toggleColorBlack.isOn)
                {
                    imgExampleFPP.sprite = bigBlack; // ������on, ū���ڽ�Ÿ��, ����
                    selectedShowSetValue = "n";
                    selectedStyle = "Default"; // Default, BigText
                    selectedColor = "Black"; // Green, Yellow, Blue, Purple, Red, Black
                }
            }
        }
    }

    // trendListContent�� ��� ����� ���¸� �����ϴ� �޼���
    public void SetAllToggles(bool isOn)
    {
        foreach (Transform toggleTransform in trendListContent)
        {
            Toggle toggle = toggleTransform.GetComponent<Toggle>();
            if (toggle != null)
            {

                toggle.isOn = isOn;
            }
        }
    }

    public void CloseFPPSetting()
    {
        selectedTrend = string.Empty;
        selectedShowSetValue = string.Empty;
        selectedStyle = string.Empty;
        selectedColor = string.Empty;
        TextMeshProUGUI txthgName = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtControllerName = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();

        txthgName.text = string.Empty;
        txtControllerName.text = string.Empty;

        toggleAllCategory.isOn = false;
        toggleShowSetValue.isOn = false;
        toggleStyleDefault.isOn = true;
        toggleColorGreen.isOn = true;

        ObjectPool.Instance.CloseFPPTrendToggleSetting();
        trendToggleInstances.Clear();
        for (int i = trendListContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(trendListContent.transform.GetChild(i).gameObject);
        }
        fppSettingScreen.SetActive(false);
    }

    // ��鵵 �г� �߰� ȭ�� ����
    public void OpenFPPSetting(string iid, string cid)
    {
        DataTable tblController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
        DataTable tblRealTime = ClientDatabase.SearchRealTimeData(iid, cid).Tables[0];
        DataTable tblHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];

        fppSettingScreen.SetActive(true);

        TextMeshProUGUI txthgName = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtControllerName = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
        string cName = string.Empty;
        string hgName = string.Empty;
        string hgid = string.Empty;
        string lgid = string.Empty;
        string fppGen = string.Empty;
        string fppX = string.Empty;
        string fppY = string.Empty;
        string fppStyleJson = string.Empty;
        string pkey = string.Empty;

        // �����׷�, ��Ʈ�ѷ��� �Ҵ�
        foreach (DataRow row in tblController.Rows)
        {
            if (iid == row["ID"].ToString() && cid == row["CID"].ToString())
            {
                cName = row["CNAME"].ToString();
                pkey = row["PKEY"].ToString();
                hgid = row["HGID"].ToString();
                lgid = row["LGID"].ToString();
                fppGen = row["FPP_GEN"].ToString();
                fppX = row["FPP_X"].ToString();
                fppY = row["FPP_Y"].ToString();
                fppStyleJson = row["FPP_STYLE"].ToString();

                foreach (DataRow row2 in tblHighGroup.Rows)
                {
                    if (hgid == row2["FLD_HGID"].ToString())
                    {
                        hgName = row2["FLD_NAME"].ToString();
                        txthgName.text = hgName;
                    }
                }
                txtControllerName.text = cName;
                ProcessingTrendToggleWithXML(iid, cid, pkey);
            }
        }

        // toggleAllCategory�� isOn ���� ����� �� ��� ��� ����
        toggleAllCategory.onValueChanged.RemoveAllListeners();
        toggleAllCategory.onValueChanged.AddListener(delegate { SetAllToggles(toggleAllCategory.isOn); });

        btnSave.gameObject.SetActive(true);
        btnModify.gameObject.SetActive(false);
        btnDelete.gameObject.SetActive(false);

        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() => SaveFPPSettings("Add", iid, cid));

        btnModify.onClick.RemoveAllListeners();
        btnDelete.onClick.RemoveAllListeners();
    }

    // ��鵵 �г� ���� ȭ�� ����
    public void ModifyFPPSetting(string iid, string cid)
    {
        DataTable tblController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
        DataTable tblRealTime = ClientDatabase.SearchRealTimeData(iid, cid).Tables[0];
        DataTable tblHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];

        fppSettingScreen.SetActive(true);

        TextMeshProUGUI txthgName = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/txt_UpperTitle").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtControllerName = fppSettingScreen.transform.Find("SettingFPPParent/obj_Setting_FPP/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
        string cName = string.Empty;
        string hgName = string.Empty;
        string hgid = string.Empty;
        string lgid = string.Empty;
        string fppGen = string.Empty;
        string fppX = string.Empty;
        string fppY = string.Empty;
        string fppStyleJson = string.Empty;
        string pkey = string.Empty;

        // �����׷�, ��Ʈ�ѷ��� �Ҵ�
        foreach (DataRow row in tblController.Rows)
        {
            if (iid == row["ID"].ToString() && cid == row["CID"].ToString())
            {
                cName = row["CNAME"].ToString();
                pkey = row["PKEY"].ToString();
                hgid = row["HGID"].ToString();
                lgid = row["LGID"].ToString();
                fppGen = row["FPP_GEN"].ToString();
                fppX = row["FPP_X"].ToString();
                fppY = row["FPP_Y"].ToString();
                fppStyleJson = row["FPP_STYLE"].ToString();

                FPPStyle fppStyle = JsonUtility.FromJson<FPPStyle>(fppStyleJson);

                string showSetValue = fppStyle.showSetValue;
                string style = fppStyle.style;
                string color = fppStyle.color;

                foreach (DataRow row2 in tblHighGroup.Rows)
                {
                    if (hgid == row2["FLD_HGID"].ToString())
                    {
                        hgName = row2["FLD_NAME"].ToString();
                        txthgName.text = hgName;
                    }
                }
                txtControllerName.text = cName;
                ProcessingTrendToggleWithXML(iid, cid, pkey);

                // Ʈ���� �迭 �Ľ�
                foreach (string trendEntry in fppStyle.trend)
                {
                    string[] parts = trendEntry.Split('|');
                    if (parts.Length == 2) // ��Ȯ�� �� �κ����� ������� �մϴ�.
                    {
                        string jsonAddr = parts[0];
                        string jsonTrendName = parts[1];

                        foreach (var trendToggle in trendToggleInstances)
                        {
                            if (trendToggle.Value.transform.Find("txtTrendName").GetComponent<TextMeshProUGUI>().text == jsonTrendName)
                            {
                                trendToggle.Value.GetComponent<Toggle>().isOn = true;
                            }
                        }
                    }
                }

                if (showSetValue == "y")
                    toggleShowSetValue.isOn = true;
                else
                    toggleShowSetValue.isOn = false;

                if (style == "Default")
                {
                    toggleStyleDefault.isOn = true;
                }

                if (style == "BigText")
                {
                    toggleStyleBigText.isOn = true;
                }

                if (style == "SingleItem")
                {
                    toggleStyleBigText.isOn = true;
                }

                switch (color)
                {
                    case "Green":
                        toggleColorGreen.isOn = true;
                        break;
                    case "Yellow":
                        toggleColorYellow.isOn = true;
                        break;
                    case "Blue":
                        toggleColorBlue.isOn = true;
                        break;
                    case "Purple":
                        toggleColorPurple.isOn = true;
                        break;
                    case "Red":
                        toggleColorRed.isOn = true;
                        break;
                    case "Black":
                        toggleColorBlack.isOn = true;
                        break;
                }
            }
        }

        // toggleAllCategory�� isOn ���� ����� �� ��� ��� ����
        toggleAllCategory.onValueChanged.RemoveAllListeners();
        toggleAllCategory.onValueChanged.AddListener(delegate { SetAllToggles(toggleAllCategory.isOn); });

        btnSave.gameObject.SetActive(false);
        btnModify.gameObject.SetActive(true);
        btnDelete.gameObject.SetActive(true);

        btnSave.onClick.RemoveAllListeners();
        btnModify.onClick.RemoveAllListeners();
        btnModify.onClick.AddListener(() => SaveFPPSettings("Modify", iid, cid));

        btnDelete.onClick.RemoveAllListeners();
        btnDelete.onClick.AddListener(() => SaveFPPSettings("Remove", iid, cid));
    }

    // Ʈ����� �ε�
    public void ProcessingTrendToggleWithXML(string iid, string cid, string pkey)
    {
        XMLParser.Instance.GetXML(pkey);
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> attributes = XMLParser.Instance.GetTrendAltAttributes(XMLParser.Instance.xmlContent);

        foreach (var group in attributes)
        {
            foreach (var tag in group.Value)
            {
                string toggleObjName = $"TrendToggle_{tag.Value["addr"]}";
                GameObject trendToggleInstance;
                Dictionary<string, GameObject> toggleObjDictionary = trendToggleInstances;

                if (!toggleObjDictionary.TryGetValue(toggleObjName, out trendToggleInstance))
                {
                    trendToggleInstance = ObjectPool.Instance.GetFPPTrendToggleObject();
                    trendToggleInstance.name = toggleObjName;
                    toggleObjDictionary[toggleObjName] = trendToggleInstance;

                    trendToggleInstance.transform.Find("txtTrendName").GetComponent<TextMeshProUGUI>().text = tag.Value["name"];

                    trendToggleInstance.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                    {
                        if (toggleStyleSingleItem.isOn)
                        {
                            int selectedCount = 0;

                            foreach (var togglePair in toggleObjDictionary)
                            {
                                Toggle toggle = togglePair.Value.GetComponent<Toggle>();
                                if (toggle.isOn)
                                {
                                    selectedCount++;
                                }
                            }

                            if (selectedCount >= 2)
                            {
                                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                                ScreenManager.Instance.txt_PopUpMsg.text = "���� �׸��� �� ������\nƮ���� �׸� ���� �����մϴ�.";
                                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                                {
                                    ScreenManager.Instance.ClosePopUpMessage();
                                    trendToggleInstance.GetComponent<Toggle>().isOn = false;
                                });
                            }
                        }
                    });
                }
            }
        }
    }

    public void CloseFloorPlan()
    {
        isFPPOpen = false;
    }

    // ��鵵 ����
    public void OpenFloorPlan()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (isLoading)
            {
                obj_Loading.SetActive(true);
                obj_Loading.transform.Find("GameObject/txt_Loading").GetComponent<TextMeshProUGUI>().text = "��鵵 ������ �������� �ֽ��ϴ�.\n��ø� ��ٷ� �ּ���.";

                DataTable tableController = ClientDatabase.controllerData.Tables[0];
                bool allZero = true; // ��� ���� FPP_GEN ���� 0���� Ȯ���ϴ� �÷���

                foreach (DataRow row in tableController.Rows)
                {
                    if (row["FPP_GEN"].ToString() != "0") // FPP_GEN ���� "0"�� �ƴ϶��
                    {
                        allZero = false; // allZero �÷��׸� false�� ����
                        break; // �� �̻� Ȯ���� �ʿ䰡 �����Ƿ� �ݺ� ����
                    }
                }

                if (allZero) // ��� ���� FPP_GEN ���� "0"�� ���
                {
                    obj_Loading.SetActive(false);
                    obj_Loading.transform.Find("GameObject/txt_Loading").GetComponent<TextMeshProUGUI>().text = string.Empty;
                    isLoading = false;

                    //ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    //ScreenManager.Instance.txt_PopUpMsg.text = "��鵵 ��";
                    //ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    //ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    //{
                    //    ScreenManager.Instance.ClosePopUpMessage();
                    //});
                }
            }

        }
        DataTable tableHighGroup = ClientDatabase.highGroupData.Tables[0];

        // �ʱ�ȭ
        dropdown_HighGroup.ClearOptions();

        // ��Ӵٿ� ��Ͽ� ���ο� ���� �׸� �߰�
        List<TMP_Dropdown.OptionData> options_HighGroup = new List<TMP_Dropdown.OptionData>();

        // �����׷� �ɼ� �߰�
        foreach (DataRow row in tableHighGroup.Rows)
        {
            string hgName = row["FLD_NAME"].ToString();
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(hgName);
            options_HighGroup.Add(option);
        }

        // ��Ӵٿ ���� �׸�� ����
        dropdown_HighGroup.AddOptions(options_HighGroup);

        // ù ��° �׸��� �⺻ �������� ����
        if (dropdown_HighGroup.options.Count > 0)
        {
            dropdown_HighGroup.value = 0;
        }

        // ���� �׷� ��Ӵٿ� ����� �̺�Ʈ ������
        dropdown_HighGroup.onValueChanged.AddListener(delegate { ChangeFloorPlan(); });
        ChangeFloorPlan();

        isFPPOpen = true;
    }

    // ��Ӵٿ�� ���ο� �����׷쿡 ���� ��鵵 �ε�
    public void LoadFloorPlan()
    {
        UpdateExampleImage();
        DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
        string hgid = string.Empty;
        string imgPath = string.Empty;
        string imgWidth = string.Empty;
        string imgHeight = string.Empty;

        foreach (DataRow row in tableHighGroup.Rows)
        {
            hgid = row["FLD_HGID"].ToString();
            imgPath = row["FLD_IMG_PATH"].ToString();
            imgWidth = row["FLD_IMG_WIDTH"].ToString();
            imgHeight = row["FLD_IMG_HEIGHT"].ToString();
            string fpImgObjName = $"FPIMG_{hgid}";

            if (fpImageInstances.TryGetValue(fpImgObjName, out var fpImgObj))
            {
                // ��鵵 �̹��� ������Ʈ�� �����ϸ� ���븸 ������Ʈ
                fpImgObj.name = fpImgObjName;
                Image fpImg = fpImgObj.GetComponent<Image>();
                StartCoroutine(LoadImageCoroutine(tableHighGroup, hgid, fpImg, imgPath));
            }
            else
            {
                // ��鵵 �̹��� ������Ʈ�� ������ ���� ����
                GameObject newfpImgObj = ObjectPool.Instance.GetFPPImageObject(); // ���ο� ��鵵 �̹��� ������Ʈ

                newfpImgObj.name = fpImgObjName;
                fpImageInstances[fpImgObjName] = newfpImgObj;
                Image newfpImg = newfpImgObj.GetComponent<Image>();
                StartCoroutine(LoadImageCoroutine(tableHighGroup, hgid, newfpImg, imgPath));
                GenerateFPP(newfpImgObj, hgid);
            }
        }

        DataTable tableController = ClientDatabase.FetchControllerData().Tables[0];

        foreach (DataRow row in tableController.Rows)
        {
            float fppX, fppY;
            if (float.TryParse(row["FPP_X"].ToString(), out fppX) && float.TryParse(row["FPP_Y"].ToString(), out fppY))
            {
                FPPLocation(row["HGID"].ToString(), row["ID"].ToString(), row["CID"].ToString(), fppX, fppY);
            }
        }
    }

    // ��鵵 �г� ���� �� ������Ʈ
    public void GenerateFPP(GameObject fpImgObj, string hgid)
    {
        DataTable tableController = ClientDatabase.FetchControllerData().Tables[0];

        foreach (DataRow row in tableController.Rows)
        {
            if (hgid == row["HGID"].ToString() && row["FPP_GEN"].ToString() == "1")
            {
                string iid = row["ID"].ToString();
                string cid = row["CID"].ToString();
                string cName = row["CNAME"].ToString();
                string fppStyle = row["FPP_STYLE"].ToString();
                string fppX = row["FPP_X"].ToString();
                string fppY = row["FPP_Y"].ToString();
                string fppMinMax = row["FPP_MINMAX"].ToString();
                string fppFix = row["FPP_FIX"].ToString();


                // �г��� ���� �׸� ������ ���                    
                string fppMaxSingleItemInstanceName = $"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}";

                if (fppMaxSingleItemInstances.TryGetValue(fppMaxSingleItemInstanceName, out var MaxFPPSingleItemInstance))
                {
                    MaxFPPSingleItemInstance.name = fppMaxSingleItemInstanceName;
                    RectTransform rectTransform = MaxFPPSingleItemInstance.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(float.Parse(fppX), float.Parse(fppY));

                    // �ּ�ȭ�� �г��� ��Ʈ�ѷ��� �Ҵ�
                    MaxFPPSingleItemInstance.transform.Find("Top/TopTop/btn_Minimizing/txtControllerName").GetComponent<TextMeshProUGUI>().text = cName;
                }
                else
                {
                    GameObject newMaxFPPSingleItemInstance = Instantiate(fppMaxSingleItemInstance, fpImgObj.transform);
                    newMaxFPPSingleItemInstance.name = fppMaxSingleItemInstanceName;
                    fppMaxSingleItemInstances[fppMaxSingleItemInstanceName] = newMaxFPPSingleItemInstance;
                    //Button btnFPPMinimizing = newMaxFPPInstance.transform.Find("Top/btn_Minimizing").GetComponent<Button>();
                    Button btnFPPFix = newMaxFPPSingleItemInstance.transform.Find("Bottom/btn_Fix").GetComponent<Button>();
                    Button btnGotoDV = newMaxFPPSingleItemInstance.transform.Find("Bottom/btn_GotoDV").GetComponent<Button>();
                    Image imgFix = btnFPPFix.transform.Find("Image").GetComponent<Image>();
                    imgFix.sprite = spriteNoFix;
                    Button btnFPPModify = newMaxFPPSingleItemInstance.transform.Find("Bottom/btn_ModifyFPP").GetComponent<Button>();
                    btnFPPModify.onClick.RemoveAllListeners();
                    btnFPPModify.onClick.AddListener(() => ModifyFPPSetting(iid, cid));

                    btnGotoDV.onClick.RemoveAllListeners();
                    btnGotoDV.onClick.AddListener(() =>
                    {                        
                        DetailView.Instance.OpenDetailView(iid, cid);
                    });

                    btnFPPFix.onClick.RemoveAllListeners();
                    btnFPPFix.onClick.AddListener(() =>
                    {
                        FPPFix(newMaxFPPSingleItemInstance, imgFix, hgid, iid, cid);
                    });


                    RectTransform rectTransform = newMaxFPPSingleItemInstance.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(float.Parse(fppX), float.Parse(fppY));

                    // �ּ�ȭ�� �г��� ��Ʈ�ѷ��� �Ҵ�
                    newMaxFPPSingleItemInstance.transform.Find("Top/TopTop/btn_Minimizing/txtControllerName").GetComponent<TextMeshProUGUI>().text = cName;
                }
            }
        }
    }

    // ��鵵 �г� ����
    public void FPPFix(GameObject maxFPPInstance, Image imgFix, string hgid, string iid, string cid)
    {
        DataTable tblController = ClientDatabase.controllerData.Tables[0];

        bool isFix = false;
        string fppStyleJson = string.Empty;
        string color = string.Empty;

        foreach (DataRow row in tblController.Rows)
        {
            if (row["HGID"].ToString() == hgid && row["ID"].ToString() == iid && row["CID"].ToString() == cid)
            {
                isFix = row["FPP_FIX"].ToString() == "1" ? true : false;
                fppStyleJson = row["FPP_STYLE"].ToString();
                FPPStyle fppStyle = JsonUtility.FromJson<FPPStyle>(fppStyleJson);
                color = fppStyle.color;
            }
        }

        if (isFix)
        {
            string query = $"UPDATE TBL_CONTROLLER SET FPP_FIX = 0 WHERE ID = '{iid}' AND CID = '{cid}'";
            if (ClientDatabase.OnUpdateRequest(query))
            {
                //Debug.Log($"{iid}_{cid} ��鵵 �г� ����");
                if (isFix)
                {
                    switch (color)
                    {
                        case "Green":
                            imgFix.sprite = spriteGreenFix;
                            break;
                        case "Yellow":
                            imgFix.sprite = spriteYellowFix;
                            break;
                        case "Blue":
                            imgFix.sprite = spriteBlueFix;
                            break;
                        case "Purple":
                            imgFix.sprite = spritePurpleFix;
                            break;
                        case "Red":
                            imgFix.sprite = spriteRedFix;
                            break;
                        case "Black":
                            imgFix.sprite = spriteBlackFix;
                            break;
                    }
                }
            }
        }
        else
        {
            string query = $"UPDATE TBL_CONTROLLER SET FPP_FIX = 1 WHERE ID = '{iid}' AND CID = '{cid}'";
            if (ClientDatabase.OnUpdateRequest(query))
            {
                //Debug.Log($"{iid}_{cid} ��鵵 �г� ��������");
                imgFix.sprite = spriteNoFix;
            }
        }
    }


    // �г� �巡�װ� ������ ���ο� ��ǥ�� DB�� ������Ʈ ��Ŵ
    public void UpdateFPPLocation(string hgid, string iid, string cid, float fppX, float fppY)
    {
        string query = $"UPDATE TBL_CONTROLLER SET FPP_X = {fppX}, FPP_Y = {fppY} WHERE ID = '{iid}' AND CID = '{cid}' AND HGID = '{hgid}'";
        ClientDatabase.OnUpdateRequest(query);
        if (fppMaxSingleItemInstances.TryGetValue($"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}", out var fppmax))
            fppmax.GetComponent<RectTransform>().anchoredPosition = new Vector2(fppX, fppY);
        //if (fppMaxInstances.TryGetValue($"FPPMaxInstance_{hgid}_{iid}_{cid}", out var fppmax))
        //    fppmax.GetComponent<RectTransform>().anchoredPosition = new Vector2(fppX, fppY);
        //if (fppMinInstances.TryGetValue($"FPPMinInstance_{hgid}_{iid}_{cid}", out var fppmin))
        //    fppmin.GetComponent<RectTransform>().anchoredPosition = new Vector2(fppX, fppY);
    }

    // �г� �巡�װ� ������ ���ο� ��ǥ�� DB�� ������Ʈ ��Ŵ
    public void FPPLocation(string hgid, string iid, string cid, float fppX, float fppY)
    {
        if (fppMaxSingleItemInstances.TryGetValue($"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}", out var fppmax))
            fppmax.GetComponent<RectTransform>().anchoredPosition = new Vector2(fppX, fppY);
        //if (fppMaxInstances.TryGetValue($"FPPMaxInstance_{hgid}_{iid}_{cid}", out var fppmax))
        //    fppmax.GetComponent<RectTransform>().anchoredPosition = new Vector2(fppX, fppY);
        //if (fppMinInstances.TryGetValue($"FPPMinInstance_{hgid}_{iid}_{cid}", out var fppmin))
        //    fppmin.GetComponent<RectTransform>().anchoredPosition = new Vector2(fppX, fppY);
    }

    // ����, ����, ��Ʈ�ѷ� �̸� ������Ʈ
    public void UpdateFloorPlanPanelUI(DataSet protocolListData, string hgid, string iid, string cid)
    {
        status = ClientDatabase.GetControllerStatus(iid, cid);

        DataTable tableSpecificController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
        DataTable tableSpecificRealTime = ClientDatabase.SearchRealTimeData(iid, cid).Tables[0];
        DataTable tblProtocolList = protocolListData.Tables[0];

        





        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ ���ⱸ��
        if (fppMaxSingleItemInstances.ContainsKey($"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"))
        {
            GameObject statusRun_On = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/Status/Status_Run/Img_On").gameObject;
            GameObject statusRun_Off = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/Status/Status_Run/Img_Off").gameObject;
            GameObject statusDefrost_On = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/Status/Status_Defrost/Img_On").gameObject;
            GameObject statusDefrost_Off = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/Status/Status_Defrost/Img_Off").gameObject;
            GameObject statusAlarm_On = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/Status/Status_Alarm/Img_On").gameObject;
            GameObject statusAlarm_Off = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/Status/Status_Alarm/Img_Off").gameObject;
            TextMeshProUGUI txtControllerName = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top/TopTop/btn_Minimizing/txtControllerName").GetComponent<TextMeshProUGUI>();

            GameObject basicScrollView = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Center/BasicStyleScrollView").gameObject;
            basicStyleContent = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Center/BasicStyleScrollView/Viewport/BasicStyleContent");

            Image imgTopBG = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Top").GetComponent<Image>();
            Image imgCenterBG = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Center").GetComponent<Image>();

            Outline maxFPPOutline = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].GetComponent<Outline>();

            bool isFix = false;
            Button btnFPPFix = fppMaxSingleItemInstances[$"FPPMaxSingleItemInstance_{hgid}_{iid}_{cid}"].transform.Find("Bottom/btn_Fix").GetComponent<Button>();
            Image imgFix = btnFPPFix.transform.Find("Image").GetComponent<Image>();

            Dictionary<string, Dictionary<string, Dictionary<string, string>>> attributes = null;

            string showSetValue = string.Empty;
            string style = string.Empty;
            string color = string.Empty;
            FPPStyle fppStyle = null;

            foreach (DataRow rowCtrl in tableSpecificController.Rows)
            {
                if (hgid == rowCtrl["HGID"].ToString() && rowCtrl["ID"].ToString() == iid && rowCtrl["CID"].ToString() == cid)
                {
                    isFix = rowCtrl["FPP_FIX"].ToString() == "1" ? false : true;
                    string fppStyleJson = rowCtrl["FPP_STYLE"].ToString();

                    fppStyle = JsonUtility.FromJson<FPPStyle>(fppStyleJson);

                    showSetValue = fppStyle.showSetValue;
                    //style = fppStyle.style;
                    //color = fppStyle.color;

                    // ������ ���� ��ư ���� ����
                    //if (isFix)
                    //{
                    //    switch (color)
                    //    {
                    //        case "Green":
                    //            imgFix.sprite = spriteGreenFix;
                    //            break;
                    //        case "Yellow":
                    //            imgFix.sprite = spriteYellowFix;
                    //            break;
                    //        case "Blue":
                    //            imgFix.sprite = spriteBlueFix;
                    //            break;
                    //        case "Purple":
                    //            imgFix.sprite = spritePurpleFix;
                    //            break;
                    //        case "Red":
                    //            imgFix.sprite = spriteRedFix;
                    //            break;
                    //        case "Black":
                    //            imgFix.sprite = spriteBlackFix;
                    //            break;
                    //    }
                    //}

                    // ����, �溸 ������ �� ���º� Outline ���� ����
                    foreach (DataRow rowRT in tableSpecificRealTime.Rows)
                    {
                        if (rowRT["ID"].ToString() == iid && rowRT["CID"].ToString() == cid)
                        {
                            string pkey = rowRT["PKEY"].ToString();

                            

                            // ���� ���¿� ���� ������ ����
                            if (status.IsRun)
                            {
                                statusRun_On.SetActive(true);
                                statusRun_Off.SetActive(false);
                                imgTopBG.color = Color.white;
                                imgCenterBG.color = Color.white;
                                maxFPPOutline.effectColor = stopBGColor;
                                txtControllerName.color = colorBlue;
                            }
                            else
                            {
                                statusRun_On.SetActive(false);
                                statusRun_Off.SetActive(true);
                                imgTopBG.color = Color.white;
                                imgCenterBG.color = Color.white;
                                maxFPPOutline.effectColor = stopBGColor;
                                txtControllerName.color = stopColor;
                            }

                            // ���� ���¿� ���� ������ ����
                            if (status.IsDefrost)
                            {
                                statusDefrost_On.SetActive(true);
                                statusDefrost_Off.SetActive(false);
                                imgTopBG.color = Color.white;
                                imgCenterBG.color = Color.white;
                                maxFPPOutline.effectColor = runColor;
                                txtControllerName.color = runColor;
                            }
                            else
                            {
                                statusDefrost_On.SetActive(false);
                                statusDefrost_Off.SetActive(true);
                                imgTopBG.color = Color.white;
                                imgCenterBG.color = Color.white;
                            }

                            // �溸 ���¿� ���� ������ ����
                            if (status.IsAlarm)
                            {
                                statusAlarm_On.SetActive(true);
                                statusAlarm_Off.SetActive(false);
                                imgTopBG.color = Color.white;
                                imgCenterBG.color = Color.white;
                                maxFPPOutline.effectColor = alarmColor;
                                txtControllerName.color = alarmColor;
                            }
                            else
                            {
                                if (status.IsConnTrying || status.IsConnChecking)
                                {
                                    statusAlarm_On.SetActive(false);
                                    statusAlarm_Off.SetActive(true);
                                    imgTopBG.color = stopBGColor;
                                    imgCenterBG.color = stopBGColor;
                                    maxFPPOutline.effectColor = stopBGColor;
                                    txtControllerName.color = stopColor;
                                }
                                else
                                {
                                    statusAlarm_On.SetActive(false);
                                    statusAlarm_Off.SetActive(true);
                                    imgTopBG.color = Color.white;
                                    imgCenterBG.color = Color.white;
                                }
                            }


                            
                        }
                    }
                }
            }

            // Ʈ���� ��� ����
            foreach (DataRow rowRTTrend in tableSpecificRealTime.Rows)
            {
                if (iid == rowRTTrend["ID"].ToString() && cid == rowRTTrend["CID"].ToString())
                {
                    string trendCount = rowRTTrend["TREND_CNT"].ToString();

                    
                    

                    Image imgSet = null;

                    // Ʈ���� ��� ���� �� ������Ʈ
                    if (Convert.ToInt32(trendCount) > 0)
                    {
                        for (int i = 0; i < Convert.ToInt32(trendCount); i++)
                        {
                            // Ʈ���� �迭 �Ľ�
                            foreach (string trendEntry in fppStyle.trend)
                            {
                                string[] parts = trendEntry.Split('|');
                                if (parts.Length == 2) // ��Ȯ�� �� �κ����� ������� �մϴ�.
                                {
                                    string pkey = string.Empty;
                                    string xml = string.Empty;
                                    string basicInstanceName = string.Empty;
                                    //string basicSetValInstanceName = string.Empty;
                                    //string bigtextInstanceName = string.Empty;
                                    //string bigtextSetValInstanceName = string.Empty;

                                    string jsonAddr = parts[0];
                                    string jsonTrendName = parts[1];

                                    string strTrend = rowRTTrend[$"TREND{i}"].ToString();
                                    string[] strTrendParts = strTrend.Split('|');

                                    foreach (DataRow row in tableSpecificController.Rows)
                                    {
                                        if (iid == row["ID"].ToString() && cid == row["CID"].ToString())
                                        {
                                            pkey = row["PKEY"].ToString();
                                        }
                                    }
                                    foreach (DataRow row2 in tblProtocolList.Rows)
                                    {
                                        if (pkey == row2["KEY"].ToString())
                                        {
                                            xml = row2["XML"].ToString();
                                        }
                                    }

                                    attributes = XMLParser.Instance.GetTrendAltAttributes(xml);
                                    foreach (var group in attributes)
                                    {
                                        foreach (var tag in group.Value)
                                        {
                                            string setAddrValue = "0";

                                            if (tag.Value.TryGetValue("setAddr", out var setAddr))
                                            {
                                                setAddrValue = setAddr;
                                                basicInstanceName = $"BasicStyleElement_{iid}_{cid}_{jsonAddr}";
                                            }
                                            else
                                            {
                                                basicInstanceName = $"BasicStyleElement_{iid}_{cid}_{jsonAddr}";
                                            }

                                            if (strTrendParts.Length >= 3)
                                            {
                                                string title = strTrendParts[0];
                                                string value = strTrendParts[1];
                                                string unit = strTrendParts[2];
                                                string addr = strTrendParts[3];
                                                string multiply = strTrendParts[4];

                                                if (jsonAddr == addr && jsonAddr == tag.Value["addr"].ToString() && tag.Value["addr"].ToString() == addr && jsonTrendName == title)
                                                {
                                                    GameObject basicInstance;

                                                    if (!basicStyleInstances.TryGetValue(basicInstanceName, out basicInstance))
                                                    {
                                                        basicInstance = Instantiate(basicStyleElementPrefab, basicStyleContent);
                                                        basicInstance.name = basicInstanceName;
                                                        basicStyleInstances[basicInstance.name] = basicInstance;
                                                        basicInstance.transform.SetParent(basicStyleContent, false);
                                                    }


                                                    // �ǽð� ������ �Ҵ� �� ������Ʈ
                                                    #region �⺻
                                                    if (fppStyle.showSetValue == "n" && fppStyle.style == "SingleItem")
                                                    {
                                                        //TextMeshProUGUI txtBasicTitle = basicInstance.transform.Find("txtTrendName").GetComponent<TextMeshProUGUI>();
                                                        TextMeshProUGUI txtBasicValue = basicInstance.transform.Find("txtTrendValue").GetComponent<TextMeshProUGUI>();

                                                        float rawValue = (float.Parse(strTrendParts[1]) * float.Parse(strTrendParts[4]));

                                                        if (rawValue >= 32768)
                                                        { // 16��Ʈ �������� ���� �� ó��
                                                            rawValue -= 65536;
                                                        }

                                                        float newValue = rawValue / float.Parse(multiply);

                                                        if (txtBasicValue != null)
                                                        {
                                                            //txtBasicTitle.text = title;
                                                            if (multiply == "1")
                                                                txtBasicValue.text = $"{newValue.ToString()}{unit}";
                                                            else if (multiply == "10")
                                                                txtBasicValue.text = $"{newValue.ToString("F1")}{unit}";
                                                            else if (multiply == "100")
                                                                txtBasicValue.text = $"{newValue.ToString("F2")}{unit}";
                                                            //txtBasicValue.text = $"{value}{unit}";
                                                        }
                                                    }

                                                    #endregion

                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }


        if (isLoading)
        {
            obj_Loading.SetActive(false);
            obj_Loading.transform.Find("GameObject/txt_Loading").GetComponent<TextMeshProUGUI>().text = string.Empty;
            isLoading = false;
        }
    }

    private void ProcessingBasicSetValSetValue(GameObject basicSetValInstance, DataTable tableRealTime, string xml, string hgid, string iid, string cid, string trendAddr, string setAddr, string multiply, string unit)
    {
        TextMeshProUGUI txtBasicSetValSetValue = basicSetValInstance.transform.Find("txtSetValue").GetComponent<TextMeshProUGUI>();
        string[] arrInstance = basicSetValInstance.name.Split('_');
        string instanceAddr = arrInstance[3];
        string instanceSetAddr = arrInstance[4];
        string strSetValue = string.Empty;

        int setAddressIndex = int.Parse(setAddr) - 200;
        foreach (DataRow row in tableRealTime.Rows)
        {
            if (iid == row["ID"].ToString() && cid == row["CID"].ToString() && instanceAddr == trendAddr && setAddr == instanceSetAddr)
            {
                string[] stringData = row["STR_DATA"].ToString().Split(',');
                parsedPollingData = new int[stringData.Length];

                for (int j = 0; j < stringData.Length; j++)
                {
                    if (!int.TryParse(stringData[j], out parsedPollingData[j])) // int�� ��ȯ �õ�
                    {
                        //Debug.LogError($"Failed to parse value at index {j}: {stringData[j]}");
                    }
                    else
                    {
                        // 16��Ʈ �������� ���� �� ó��
                        if (parsedPollingData[j] >= 32768)
                        {
                            parsedPollingData[j] -= 65536;
                        }
                    }
                }

                if (setAddressIndex >= 0 && setAddressIndex < parsedPollingData.Length)
                {
                    float setValue = parsedPollingData[setAddressIndex] / float.Parse(multiply);
                    strSetValue = setValue.ToString(multiply == "1.0" ? "0" : multiply == "10.0" ? "F1" : "F2") + unit;
                    txtBasicSetValSetValue.text = strSetValue;
                }
            }
        }

        if (instanceSetAddr == "0")
        {
            txtBasicSetValSetValue.text = string.Empty;
        }
    }



    private void ProcessingBigTextSetValSetValue(GameObject bigTextSetValInstance, DataTable tableRealTime, string xml, string hgid, string iid, string cid, string trendAddr, string setAddr, string multiply, string unit)
    {
        TextMeshProUGUI txtBigTextSetValSetValue = bigTextSetValInstance.transform.Find("txtSetValue").GetComponent<TextMeshProUGUI>();
        string[] arrInstance = bigTextSetValInstance.name.Split('_');
        string instanceAddr = arrInstance[3];
        string instanceSetAddr = arrInstance[4];

        int setAddressIndex = int.Parse(setAddr) - 200;
        foreach (DataRow row in tableRealTime.Rows)
        {
            if (iid == row["ID"].ToString() && cid == row["CID"].ToString() && instanceAddr == trendAddr && instanceSetAddr == setAddr)
            {
                string[] stringData = row["STR_DATA"].ToString().Split(',');
                parsedPollingData = new int[stringData.Length];
                for (int j = 0; j < stringData.Length; j++)
                {
                    if (!int.TryParse(stringData[j], out parsedPollingData[j])) // int�� ��ȯ �õ�
                    {
                        //Debug.LogError($"Failed to parse value at index {j}: {stringData[j]}");
                    }
                    else
                    {
                        // 16��Ʈ �������� ���� �� ó��
                        if (parsedPollingData[j] >= 32768)
                        {
                            parsedPollingData[j] -= 65536;
                        }
                    }
                }

                if (setAddressIndex >= 0 && setAddressIndex < parsedPollingData.Length)
                {
                    float setValue = parsedPollingData[setAddressIndex] / float.Parse(multiply);
                    string strSetValue = setValue.ToString(multiply == "1.0" ? "0" : multiply == "10.0" ? "F1" : "F2") + unit;
                    txtBigTextSetValSetValue.text = strSetValue;
                }
            }

            if (instanceSetAddr == "0")
            {
                txtBigTextSetValSetValue.text = string.Empty;
            }
        }
    }


    //IEnumerator IPlayDefrostAnim(GameObject fppMaxInstance)
    //{
    //    while (true)
    //    {
    //        foreach (Sprite defrost in defrostImgList)
    //        {
    //            fppMaxInstance.transform.Find("Top/TopTop/Status_Others/Img_DefrostOn/Image").GetComponent<Image>().sprite = defrost;
    //            yield return sec0_1;
    //        }
    //    }
    //}

    //IEnumerator IPlayFanAnim(GameObject fppMaxInstance)
    //{
    //    while (true)
    //    {
    //        foreach (Sprite fan in fanImgList)
    //        {
    //            fppMaxInstance.transform.Find("Top/TopTop/Status_Others/Img_FanOn/Image").GetComponent<Image>().sprite = fan;
    //            yield return sec0_1;
    //        }
    //    }
    //}



    // ��Ӵٿ�� ���ο� �����׷쿡 ���� ��鵵 �ε�
    public void ChangeFloorPlan()
    {
        DataTable tableHighGroup = ClientDatabase.highGroupData.Tables[0];
        string hgid = string.Empty;
        string imgPath = string.Empty;
        string imgWidth = string.Empty;
        string imgHeight = string.Empty;

        foreach (DataRow row in tableHighGroup.Rows)
        {
            if (row["FLD_NAME"].ToString() == dropdown_HighGroup.options[dropdown_HighGroup.value].text)
            {
                hgid = row["FLD_HGID"].ToString();
                imgPath = row["FLD_IMG_PATH"].ToString();
                imgWidth = row["FLD_IMG_WIDTH"].ToString();
                imgHeight = row["FLD_IMG_HEIGHT"].ToString();

                currentSelectedHGID = hgid;
            }
        }

        foreach (GameObject obj in fpImageInstances.Values)
        {
            obj.SetActive(false);
        }
        fpImageInstances[$"FPIMG_{currentSelectedHGID}"].SetActive(true);
        Image newfpImg = fpImageInstances[$"FPIMG_{currentSelectedHGID}"].GetComponent<Image>();

        //#if UNITY_ANDROID

        //        StartCoroutine(LoadImageCoroutine(newfpImg, imgPath));
        //#endif
    }

    // �ȵ���̵� ����̽��� Ư�� ��ο� ����� �̹��� ������ �ҷ���
    private IEnumerator LoadImageCoroutine(DataTable tableHighGroup, string hgid, Image fpImg, string imagePath)
    {
        if (imagePath == "this group have to img path define" || imagePath.Length == 0)
        {
            //Debug.LogError("�ش� �׷쿡 ������ ��鵵 �̹����� �������� �ʽ��ϴ�.");
        }
        else
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + imagePath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    fpImg.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    // Content�� RectTransform�� ������
                    RectTransform contentRectTransform = floorPlanScrollView.transform.Find("Viewport/Content").GetComponent<RectTransform>();

                    // Content�� �ʺ� �������� �̹��� ������ �°� ���� ����
                    float contentWidth = contentRectTransform.rect.width;
                    float imageAspectRatio = (float)texture.width / (float)texture.height;
                    float adjustedHeight = contentWidth / imageAspectRatio;

                    // �̹����� RectTransform�� sizeDelta�� �����Ͽ� Content ũ�⿡ ����
                    fpImg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(contentWidth, adjustedHeight);

                    string query = $"UPDATE TBL_HIGH_GROUP SET FLD_UNITY_WIDTH = '{contentWidth}', FLD_UNITY_HEIGHT = '{adjustedHeight}' WHERE FLD_HGID = '{hgid}'";
                    if (ClientDatabase.OnInsertRequest(query))
                    {
                        //Debug.Log($"UPDATE Complete : {query}");
                    }
                    else
                    {
                        //Debug.Log($"UPDATE Failed : {query}");
                    }
                    //foreach (DataRow row in tableHighGroup.Rows)
                    //{
                    //    if(hgid == row["FLD_HGID"].ToString())
                    //    {
                    //    }                        
                    //}
                    //Debug.Log($"�̹��� ũ�� ����: �ʺ� = {contentWidth}, ���� = {adjustedHeight}");
                }
                else
                {
                    //Debug.LogError("Image load failed. Error: " + uwr.error);
                }
            }
        }
    }

    // �ϴ��� �׷캰 ���� ���� ī��Ʈ ������Ʈ
    public void UpdateFPBottomCount(DataSet controllerDataSet, DataSet realTimeDataSet)
    {
        DataTable tblController = controllerDataSet.Tables[0];
        DataTable tblRealTime = realTimeDataSet.Tables[0];

        int totalCnt = 0;
        int onCnt = 0;
        int offCnt = 0;
        int defCnt = 0;

        // currentSelectedHGID�� int�� �����ϰ� ��ȯ
        if (int.TryParse(currentSelectedHGID, out int parsedHGID))
        {
            // ���� ���õ� �����׷�ID�� ��ġ�ϴ� HGID�� ���� ��Ʈ�ѷ� ���͸�
            DataRow[] filteredControllers = tblController.Select($"HGID = {parsedHGID}");

            totalCnt = filteredControllers.Length; // �� ��Ʈ�ѷ� ��

            foreach (DataRow controllerRow in filteredControllers)
            {
                string controllerId = controllerRow["ID"].ToString();
                string controllerCid = controllerRow["CID"].ToString();

                // tblRealTime���� �ش� ��Ʈ�ѷ��� RUN ���¸� Ȯ��
                DataRow[] realTimeRows = tblRealTime.Select($"ID = '{controllerId}' AND CID = '{controllerCid}'");
                foreach (DataRow realTimeRow in realTimeRows)
                {
                    string pkey = realTimeRow["PKEY"].ToString();
                    string conn = realTimeRow["CONN"].ToString();
                    int runStatus = Convert.ToInt32(realTimeRow["POWER"]);
                    int def = Convert.ToInt32(realTimeRow["DEFROST"]);

                    if (pkey == "07152101-011-00-170")
                    {
                        ParsePollingData(controllerId, controllerCid);
                        int rawValue = parsedPollingData[24];
                        bool isRunBitSet = (rawValue & (1 << 0)) != 0;
                        bool isDefBitSet = (rawValue & (1 << 1)) != 0;
                        if (isRunBitSet)
                        {
                            onCnt++; // ���� ���� ��Ʈ�ѷ� ��
                        }
                        else
                        {
                            offCnt++; // ���� ������ ��Ʈ�ѷ� ��
                        }
                    }
                    else if (pkey == "PRDPC3HL20160317")
                    {
                        if (conn == "1")
                        {
                            onCnt++; // ���� ���� ��Ʈ�ѷ� ��
                        }
                        else
                        {
                            offCnt++; // ���� ������ ��Ʈ�ѷ� ��
                        }
                    }
                    else
                    {
                        if (runStatus == 1)
                            onCnt++; // ���� ���� ��Ʈ�ѷ� ��
                        else if (runStatus == 0)
                            offCnt++; // ���� ������ ��Ʈ�ѷ� ��

                        if(def == 1) 
                            defCnt++; // ���� ���� ��Ʈ�ѷ� ��
                    }
                }
            }
        }
        else
        {
            //Debug.LogError($"HGID ����ȯ ����: {currentSelectedHGID}");
        }



        // UI�� ī��Ʈ ������Ʈ
        txtTotalControllerCnt.text = $"{totalCnt}";
        txtOnControllerCnt.text = $"{onCnt}";
        txtOffControllerCnt.text = $"{offCnt}";
        txtDefControllerCnt.text = $"{defCnt}";
    }


}