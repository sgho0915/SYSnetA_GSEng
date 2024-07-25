using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using TMPro;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;


public class ColorThemeManager : MonoBehaviour
{
    public static ColorThemeManager Instance;

    public Color colorGreen = new Color(116 / 255f, 178 / 255f, 8 / 255f, 1f);         // Green #74B208
    public Color colorBlue = new Color(0 / 255f, 169 / 255f, 196 / 255f, 1f);          // Blue #00A9C4
    public Color colorNavy = new Color(85 / 255f, 100 / 255f, 188 / 255f, 1f);         // Navy #5564BC
    //Color colorRed = new Color(240 / 255f, 94 / 255f, 51 / 255f, 1f);           // Red #F05E33
    public Color colorRed = new Color(221 / 255f, 134 / 255f, 55 / 255f, 1f);           // Red #DD8637
    public Color colorBlack = new Color(45 / 255f, 45 / 255f, 45 / 255f, 1f);          // Black #2D2D2D
    public Color colorBusungBlue = new Color(37 / 255f, 89 / 255f, 165 / 255f, 1f);    // #2559A5

    public RectTransform mainCanvas;
    public List<RoundedCornersX4> BorderList = new List<RoundedCornersX4>();
    public List<Image> BackgroundList = new List<Image>();
    public List<TextMeshProUGUI> TextList = new List<TextMeshProUGUI>();
    public int colorIdx;

    private void Awake()
    {
        Instance = this;       
    }

    private void Start()
    {
        string systemSet = string.Empty;
        int colorThemeData = 0;
        //DataTable tblConfig = ClientDatabase.FetchConfigData().Tables[0];
        DataSet ds = ClientDatabase.FetchConfigData();
        if (ds != null && ds.Tables.Count > 0)
        {
            DataTable tblConfig = ds.Tables[0];
            foreach (DataRow row in tblConfig.Rows)
            {
                systemSet = row["SYSTEM_SET"].ToString(); // 시스템설정 Json
            }
        }
        else
        {
            Debug.LogError("config data is null");
            return;
        }

        SystemSettings systemSetJson = JsonUtility.FromJson<SystemSettings>(systemSet);
        colorThemeData = int.Parse(systemSetJson.colorTheme);
        colorIdx = colorThemeData;
        ApplyColor(colorThemeData);
    }

    public void ApplyTheme()
    {
        string systemSet = string.Empty;
        int colorThemeData = 0;
        //DataTable tblConfig = ClientDatabase.FetchConfigData().Tables[0];
        DataSet ds = ClientDatabase.FetchConfigData();
        if (ds != null && ds.Tables.Count > 0)
        {
            DataTable tblConfig = ds.Tables[0];
            foreach (DataRow row in tblConfig.Rows)
            {
                systemSet = row["SYSTEM_SET"].ToString(); // 시스템설정 Json
            }
        }
        else
        {
            Debug.LogError("config data is null");
            return;
        }

        SystemSettings systemSetJson = JsonUtility.FromJson<SystemSettings>(systemSet);
        colorThemeData = int.Parse(systemSetJson.colorTheme);
        colorIdx = colorThemeData;
        ApplyColor(colorThemeData);
    }

    public void ApplyToObj(Color applyColor)
    {
        foreach (var border in BorderList)
        {
            border.BorderColor = applyColor;
        }
        foreach (var bg in BackgroundList)
        {
            bg.color = applyColor;
        }
        foreach (var txt in TextList)
        {
            txt.color = applyColor;
        }

        //ApplyColor_FilterSelected("Filter_Selected", applyColor);
        //ApplyColor_GroupElementAddr("GraphElement_Addr", applyColor);
        //ApplyColor_GroupElementGroupID("GroupElement_GroupID", applyColor);

        foreach (var border in BorderList)
        {            
            border.BorderWidth += 0.01f;
            border.BorderWidth -= 0.01f;
        }
    }

    public void ApplyColor(int colorTheme)
    {
        Color color = new Color();
        switch (colorTheme)
        {
            case 1:
                color = colorGreen;
                ApplyToObj(color);
                break;
            case 2:
                color = colorBlue;
                ApplyToObj(color);
                break;                
            case 3:
                color = colorNavy;
                ApplyToObj(color);
                break;
            case 4:
                color = colorRed;
                ApplyToObj(color);
                break;
            case 5:
                color = colorBlack;
                ApplyToObj(color);
                break;
            case 6:
                color = colorBusungBlue;
                ApplyToObj(color);
                break;
        }
    }


    //런타임 대체 방안 모색: UnityEditor 기능에 대한 런타임 대체 방안을 모색하세요. 예를 들어, 게임 내에서 특정 자산을 동적으로 로드하거나 수정해야 하는 경우, Resources 폴더나 AssetBundles, Addressables을 사용할 수 있습니다.


    //private void ApplyColor_FilterSelected(string fileName, Color applyColor)
    //{
    //    string path = "Assets/Resources/Prefabs/MainScreen_Filter/" + fileName + ".prefab";
    //    Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
    //    GameObject temp = Instantiate(obj) as GameObject; //인스턴스 만들기

    //    temp.GetComponent<RoundedCornersX4>().BorderColor = applyColor;
    //    temp.transform.Find("txt_Filter").GetComponent<TextMeshProUGUI>().color = applyColor;
    //    temp.transform.Find("btn_ExceptionFilter/Image").GetComponent<Image>().color = applyColor;

    //    bool isSuccess = false;
    //    UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, path, out isSuccess); //저장
    //    UnityEditor.AssetDatabase.Refresh();
    //    Debug.Log($"{fileName} prefab apply color finish : {isSuccess}");
    //}

    //private void ApplyColor_GroupElementAddr(string fileName, Color applyColor)
    //{
    //    string path = "Assets/Resources/Prefabs/DetailView/" + fileName + ".prefab";
    //    Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
    //    GameObject temp = Instantiate(obj) as GameObject; //인스턴스 만들기

    //    temp.transform.Find("Active").GetComponent<Image>().color = applyColor;

    //    bool isSuccess = false;
    //    UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, path, out isSuccess); //저장
    //    UnityEditor.AssetDatabase.Refresh();
    //    Debug.Log($"{fileName} prefab apply color finish : {isSuccess}");
    //}

    //private void ApplyColor_GroupElementGroupID(string fileName, Color applyColor)
    //{
    //    string path = "Assets/Resources/Prefabs/DetailView/" + fileName + ".prefab";
    //    Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
    //    GameObject temp = Instantiate(obj) as GameObject; //인스턴스 만들기

    //    temp.transform.Find("Img_On/Image").GetComponent<Image>().color = applyColor;

    //    bool isSuccess = false;
    //    UnityEditor.PrefabUtility.SaveAsPrefabAsset(temp, path, out isSuccess); //저장
    //    UnityEditor.AssetDatabase.Refresh();
    //    Debug.Log($"{fileName} prefab apply color finish : {isSuccess}");
    //}
}
