using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LottiePlugin.UI;
using System;
using UnityEngine.SceneManagement;
using PimDeWitte.UnityMainThreadDispatcher;



public class ScreenManager : MonoBehaviour
{
    [Header("디바이스 분류 및 해상도 비율 조정 위한 변수")]
    public GameObject obj_MainCanvas;
    public static bool isTablet;
    public CanvasScaler canvasScaler;
    public ScreenOrientation currentOrientation; // 디바이스의 방향 상태를 저장할 변수
    public Vector2 screenSize; // 디바이스의 스크린 사이즈를 저장할 변수

    [Header("DPI에 따른 MainScreen의 오브젝트 Scale 조절")]    
    public RectTransform obj_DetailView; // 디테일뷰UI
    public GameObject obj_ControllerScrollViewGrid;

    [Header("상단 메뉴 오브젝트")]
    public TextMeshProUGUI txt_RealTime;

    [Header("상단 메뉴 오브젝트")]
    public GameObject objBtnSideMenu;
    public GameObject obj_MainScreen;       // 메인화면
    public GameObject obj_FloorPlanScreen;  // 평면도
    public GameObject obj_SetScreen;        // 환경설정
    public GameObject obj_InquiryScreen;    // 조회
    public GameObject btn_MainScreen;       // 메인화면 버튼
    public GameObject btn_FloorPlanScreen;  // 평면도 버튼
    public GameObject btn_SetScreen;        // 환경설정 버튼
    public GameObject btn_InquiryScreen;    // 조회 버튼
    private GameObject btn_Active_MainScreen;       // 메인화면 버튼 활성화 상태
    private GameObject btn_Active_FloorPlanScreen;  // 평면도 버튼 활성화 상태
    private GameObject btn_Active_SetScreen;        // 환경설정 버튼 활성화 상태
    private GameObject btn_Active_InquiryScreen;    // 조회 버튼 활성화 상태
    private GameObject btn_Deactive_MainScreen;       // 메인화면 버튼 비활성화 상태
    private GameObject btn_Deactive_FloorPlanScreen;  // 평면도 버튼 비활성화 상태
    private GameObject btn_Deactive_SetScreen;        // 환경설정 버튼 비활성화 상태
    private GameObject btn_Deactive_InquiryScreen;    // 조회 버튼 비활성화 상태


    [Header("설정 화면 오브젝트")]
    public GameObject obj_Setting_AddConverter;

    [Header("팝업 메세지의 상태 관련 오브젝트")]
    public GameObject obj_PopUpMsg;
    public Image img_notiState; // 상단 타이틀 이미지
    public Sprite sprite_Notification; // 상단 타이틀(알림)
    public Sprite sprite_Confirm; // 상단 타이틀(확인)
    public Sprite sprite_ErrorWarning; // 상단 타이틀(에러 혹은 경고)
    public Sprite sprite_Delete; // 삭제
    public TextMeshProUGUI txt_PopUpMsg; // 팝업 메세지
    public Button btnPopUpCancel;
    public Button btnPopUpConfirm;

    private GameObject loaingObj;
    private WaitForSeconds sec_1 = new WaitForSeconds(1f);

    // singleton
    private static ScreenManager _instance;
    private ScreenState _currentScreenState;
    private ResolutionState _currentResolutionState;
    private PopUpState _currentPopUpState;

    public static ScreenManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScreenManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = "ScreenManager";
                    _instance = obj.AddComponent<ScreenManager>();
                }
            }
            return _instance;
        }
    }

    #region 구조체 목록
    // 팝업 관리 구조체
    public enum PopUpState
    {
        None,        
        Notification,    // 알림
        Confirm,        // 확인
        ErrorWarning,   // 에러 혹은 경고
        Delete // 삭제
    }

    // 화면 상태
    public enum ScreenState
    {
        None,
        Main,           // 메인화면
        DetailView,     // 디테일뷰        
        FloorPlan,      // 평면도
        Settings,       // 설정
        Inquiry         // 제어이력
    }

    // 현재 모니터 해상도에 따라 DetailView 오브젝트의 Scale을 조절하기 위한 구조체
    public enum ResolutionState
    {
        None,
        LowResolution,
        MiddleResolution,
        HighResolution
    }

    public PopUpState CurrentPopUpState
    {
        get => _currentPopUpState;
        set
        {
            _currentPopUpState = value;
            OpenPopUpMessage();
        }
    }

    public ScreenState CurrentScreenState
    {
        get => _currentScreenState;
        set
        {
            _currentScreenState = value;
            HandleStateChange();
        }
    }

    public ResolutionState CurrentResolutionState
    {
        get => _currentResolutionState;
        set
        {
            _currentResolutionState = value;
            // 화면 상태 변경시 필요한 로직을 여기에 추가            
        }
    }
    #endregion

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // 애플리케이션 실행 시 시작 화면은 항상 Main        
        UpperMenuBtnInit();
        canvasScaler = obj_MainCanvas.GetComponent<CanvasScaler>();
        ResizeScreen();
        currentOrientation = Screen.orientation;
        StartCoroutine(CheckForOrientationChange());
        screenSize = new Vector2(Screen.width, Screen.height);
        StartCoroutine(CheckForScreenSizeChange());

        loaingObj = obj_MainCanvas.transform.Find("Loading").gameObject;

        btnPopUpCancel = obj_PopUpMsg.transform.Find("Panel/Bottom/btn_Cancel").GetComponent<Button>();
        btnPopUpConfirm = obj_PopUpMsg.transform.Find("Panel/Bottom/btn_Confirm").GetComponent<Button>();
    }

    private void Start()
    {
        //canvasScaler = obj_MainCanvas.GetComponent<CanvasScaler>();
        //ResizeScreen();        

        //currentOrientation = Screen.orientation;
        //StartCoroutine(CheckForOrientationChange());

        //screenSize = new Vector2(Screen.width, Screen.height);
        //StartCoroutine(CheckForScreenSizeChange());
        //CurrentScreenState = ScreenState.None;
        UnityMainThreadDispatcher.Instance().Enqueue(() => StartCoroutine(UpdateTime()));
    }

    public IEnumerator UpdateTime()
    {
        txt_RealTime.gameObject.SetActive(true);
        DateTime currentTime = DateTime.Now;
        string formattedTime = currentTime.ToString("yy.MM.dd  hh:mm tt");
        txt_RealTime.text = formattedTime;

        while (true)
        {
            if (SettingManager.timeUse == 1)
            {
                txt_RealTime.gameObject.SetActive(true);
                currentTime = DateTime.Now;
                formattedTime = currentTime.ToString("yy.MM.dd  hh:mm tt");
                txt_RealTime.text = formattedTime;
            }
            else
            {
                txt_RealTime.gameObject.SetActive(false);
            }
            yield return sec_1;
        }        
    }

    public void GotoMain()
    {
        //TTSManager.instance.RunTTS($"메인화면");
        if(CurrentScreenState == ScreenState.DetailView)
            DetailView.Instance.CloseDetailView();
        if(CurrentScreenState == ScreenState.FloorPlan)
            FloorPlanManager.Instance.CloseFloorPlan();
        CurrentScreenState = ScreenState.Main;
        objBtnSideMenu.SetActive(true);
    }

    public void GotoFloorPlan()
    {
        //TTSManager.instance.RunTTS($"평면도");
        if (CurrentScreenState == ScreenState.DetailView)
            DetailView.Instance.CloseDetailView();
        CurrentScreenState = ScreenState.FloorPlan;
        objBtnSideMenu.SetActive(true);
    }

    public void GotoSettings()
    {
        //TTSManager.instance.RunTTS($"설정");
        if (CurrentScreenState == ScreenState.DetailView)
            DetailView.Instance.CloseDetailView();
        if (CurrentScreenState == ScreenState.FloorPlan)
            FloorPlanManager.Instance.CloseFloorPlan();
        CurrentScreenState = ScreenState.Settings;
        objBtnSideMenu.SetActive(false);
    }

    public void GotoInquiry()
    {
        //TTSManager.instance.RunTTS($"조회");
        if (CurrentScreenState == ScreenState.DetailView)
            DetailView.Instance.CloseDetailView();
        if (CurrentScreenState == ScreenState.FloorPlan)
            FloorPlanManager.Instance.CloseFloorPlan();
        CurrentScreenState = ScreenState.Inquiry;
        objBtnSideMenu.SetActive(false);
        InquiryManager.Instance.LoadInquiry();        
    }

    #region 상태에 따른 UI 변경    
    // 팝업 메세지의 상태에 따른 이벤트 처리    
    private void OpenPopUpMessage()
    {
        switch (_currentPopUpState)
        {
            case PopUpState.Notification:
                obj_PopUpMsg.SetActive(true);
                btnPopUpCancel.gameObject.SetActive(false);
                if (img_notiState != null && sprite_Notification != null)
                    img_notiState.sprite = sprite_Notification;
                break;
            case PopUpState.Confirm:
                obj_PopUpMsg.SetActive(true);
                btnPopUpCancel.gameObject.SetActive(false);
                if (img_notiState != null && sprite_Confirm != null)
                    img_notiState.sprite = sprite_Confirm;
                break;
            case PopUpState.ErrorWarning:
                obj_PopUpMsg.SetActive(true);
                btnPopUpCancel.gameObject.SetActive(false);
                if (img_notiState != null && sprite_ErrorWarning != null)
                    img_notiState.sprite = sprite_ErrorWarning;
                break;
            case PopUpState.Delete:
                obj_PopUpMsg.SetActive(true);
                btnPopUpCancel.gameObject.SetActive(true);
                if (img_notiState != null && sprite_Delete != null)
                    img_notiState.sprite = sprite_Delete;
                break;
            case PopUpState.None:
                if (img_notiState != null && sprite_ErrorWarning != null)
                    img_notiState.sprite = null;
                break;
        }
    }
    
    
    // 팝업창 닫기    
    public void ClosePopUpMessage()
    {
        // 팝업 내용 초기화
        obj_PopUpMsg.SetActive(false);
        _currentPopUpState = PopUpState.None;
        txt_PopUpMsg.text = string.Empty;
    }

    public void UpperMenuBtnInit()
    {
        btn_Active_MainScreen = btn_MainScreen.transform.Find("Active").gameObject;
        btn_Deactive_MainScreen = btn_MainScreen.transform.Find("Deactive").gameObject;
        btn_Active_FloorPlanScreen = btn_FloorPlanScreen.transform.Find("Active").gameObject;
        btn_Deactive_FloorPlanScreen = btn_FloorPlanScreen.transform.Find("Deactive").gameObject;
        btn_Active_SetScreen = btn_SetScreen.transform.Find("Active").gameObject;
        btn_Deactive_SetScreen = btn_SetScreen.transform.Find("Deactive").gameObject;
        btn_Active_InquiryScreen = btn_InquiryScreen.transform.Find("Active").gameObject;
        btn_Deactive_InquiryScreen = btn_InquiryScreen.transform.Find("Deactive").gameObject;
    }
    
    
    // ScreenState에 따른 상단 UI 변경    
    private void HandleStateChange()
    {
        switch (_currentScreenState)
        {
            case ScreenState.DetailView:
                HandleDetailViewChange();                
                SideMenuManager.Instance.DecisionShowFPPScreen();
                //InquiryManager.Instance.CloseLogWebview();
                break;
            case ScreenState.Main:                
                ShowMainScreenUI();
                SideMenuManager.Instance.DecisionShowFPPScreen();
                //InquiryManager.Instance.CloseLogWebview();
                break;
            case ScreenState.FloorPlan:
                ShowFloorPlanScreenUI();
                SideMenuManager.Instance.DecisionShowFPPScreen();
                //InquiryManager.Instance.CloseLogWebview();
                break;
            case ScreenState.Settings:
                ShowSettingScreenUI();
                loaingObj.SetActive(false);
                SideMenuManager.Instance.DecisionShowFPPScreen();
                //InquiryManager.Instance.CloseLogWebview();
                break;
            case ScreenState.Inquiry:
                ShowInquiryScreenUI();
                //SideMenuManager.Instance.DecisionShowFPPScreen();                
                break;
            default:
                break;
        }
    }

    // ScreenState.DetailView 상태일 경우에 해당하는 로직
    private void HandleDetailViewChange()
    {
        
    }

    private void SetButtonActive(GameObject activeButton, GameObject deactiveButton)
    {
        activeButton.SetActive(true);
        deactiveButton.SetActive(false);
    }

    private void ShowMainScreenUI()
    {        
        obj_MainScreen.SetActive(true);
        SetButtonActive(btn_Active_MainScreen, btn_Deactive_MainScreen);

        obj_FloorPlanScreen.SetActive(false);
        SetButtonActive(btn_Deactive_FloorPlanScreen, btn_Active_FloorPlanScreen);

        obj_SetScreen.SetActive(false);
        SetButtonActive(btn_Deactive_SetScreen, btn_Active_SetScreen);

        obj_InquiryScreen.SetActive(false);
        SetButtonActive(btn_Deactive_InquiryScreen, btn_Active_InquiryScreen);
    }

    private void ShowFloorPlanScreenUI()
    {
        obj_MainScreen.SetActive(false);
        SetButtonActive(btn_Deactive_MainScreen, btn_Active_MainScreen);

        obj_FloorPlanScreen.SetActive(true);
        SetButtonActive(btn_Active_FloorPlanScreen, btn_Deactive_FloorPlanScreen);

        obj_SetScreen.SetActive(false);
        SetButtonActive(btn_Deactive_SetScreen, btn_Active_SetScreen);

        obj_InquiryScreen.SetActive(false);
        SetButtonActive(btn_Deactive_InquiryScreen, btn_Active_InquiryScreen);

        FloorPlanManager.Instance.LoadFloorPlan();        
        FloorPlanManager.Instance.OpenFloorPlan();
    }

    private void ShowSettingScreenUI()
    {
        obj_MainScreen.SetActive(false);
        SetButtonActive(btn_Deactive_MainScreen, btn_Active_MainScreen);

        obj_FloorPlanScreen.SetActive(false);
        SetButtonActive(btn_Deactive_FloorPlanScreen, btn_Active_FloorPlanScreen);

        obj_SetScreen.SetActive(true);
        SetButtonActive(btn_Active_SetScreen, btn_Deactive_SetScreen);

        obj_InquiryScreen.SetActive(false);
        SetButtonActive(btn_Deactive_InquiryScreen, btn_Active_InquiryScreen);

        SettingManager.Instance.LoadSettingScreen();
    }

    private void ShowInquiryScreenUI()
    {
        obj_MainScreen.SetActive(false);
        SetButtonActive(btn_Deactive_MainScreen, btn_Active_MainScreen);

        obj_FloorPlanScreen.SetActive(false);
        SetButtonActive(btn_Deactive_FloorPlanScreen, btn_Active_FloorPlanScreen);

        obj_SetScreen.SetActive(false);
        SetButtonActive(btn_Deactive_SetScreen, btn_Active_SetScreen);

        obj_InquiryScreen.SetActive(true);
        SetButtonActive(btn_Active_InquiryScreen, btn_Deactive_InquiryScreen);
    }
    #endregion



    public void ResizeScreen()
    {
        SetReferenceResolution();
        if (Application.isEditor)
        {
            return;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(obj_MainCanvas.GetComponent<RectTransform>());
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        canvasScaler.scaleFactor = Screen.dpi / 160;
#if UNITY_ANDROID
        canvasScaler.scaleFactor = Screen.dpi / 160;
        //Debug.Log("Android OS에서 실행 중 : scaleFactor 값" + canvasScaler.scaleFactor + "로 자동 적용됩니다.");
#endif 
    }

    public void SetReferenceResolution()
    {
        float aspectRatio = 0;

        // 디바이스의 화면 방향과 무관하게 화면비를 측정
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            aspectRatio = (float)Screen.width / Screen.height;
        else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            aspectRatio = (float)Screen.height / Screen.width;

        // 16:10은 1.33:1, 16:10은 1.6:1, 16:9는 1.77:1, 19.5:9(아이폰, 갤23)는 2.16:1, 22:9(플립)은 2.44:1
        // 화면비가 1.8 이하일 경우 태블릿으로 분류
        isTablet = aspectRatio <= 1.8f;        

        string str = string.Empty;
        if (isTablet)
        {
            str = "Type of Tablet";
        }
        else
        {
            str = "Type of MobileDevice";
            if(Application.platform == RuntimePlatform.Android)
            {
                if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                {
                    // https://ryeggg.tistory.com/62
                    // 안드로이드 스마트폰에서 실행 중이며, 세로 방향일 경우 상태바(Safe Area) 활성화
                    AndroidThemeControl.StatusBarControl(true);
                    AndroidThemeControl.NavigationBarControl(true);
                    AndroidThemeControl.StatusBarColorControl(0x000000);
                    AndroidThemeControl.NavigationBarColorControl(0x000000);
                }
                else
                {
                    // 안드로이드 스마트폰에서 실행 중이며, 가로 방향일 경우 Safe Area는 유지, 상태바만 비활성화
                    AndroidThemeControl.StatusBarControl(false);
                    AndroidThemeControl.NavigationBarControl(false);
                }
            }            
        }

    }


    // 디바이스 화면 방향 변경 감지
    public IEnumerator CheckForOrientationChange()
    {        
        yield return new WaitUntil(() => Screen.orientation != currentOrientation);

        ResizeScreen();
        currentOrientation = Screen.orientation;

        StartCoroutine(CheckForOrientationChange());
    }

    // 디바이스 스크린 사이즈 변경 감지
    public IEnumerator CheckForScreenSizeChange()
    {
        yield return new WaitUntil(() => screenSize.x != Screen.width || screenSize.y != Screen.height);

        // 화면 크기가 변경되었을 때 실행할 코드
        screenSize = new Vector2(Screen.width, Screen.height);

        // 코루틴 재시작
        StartCoroutine(CheckForScreenSizeChange());
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
}