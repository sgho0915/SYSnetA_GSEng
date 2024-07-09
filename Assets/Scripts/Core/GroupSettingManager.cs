using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class GroupSettingManager : MonoBehaviour
{
    // 이하 첫 설정 시 요소
    private Dictionary<string, GameObject> f_uiElements = new Dictionary<string, GameObject>();
    [Header("첫 설정 그룹 설정요소")]
    public GameObject f_settingGroup;
    public GameObject f_groupListScrollView; // 그룹 리스트 스크롤뷰
    public static Transform f_groupListContent; // 그룹 리스트 스크롤뷰 Content
    public static Transform f_hgContainerTransform; // Container_HighGroup의 Transform
    public static Transform f_lgContainerTransform; // Container_LowGroup의 Transform
    private Button f_btnClose;
    private Button f_btnAddHighGroup;
    private Button f_btnSave;

    [Header("첫 설정 상위그룹 설정요소")]
    public GameObject f_settingHighGroup; // 상위그룹 설정화면
    private TMP_InputField f_inputFieldHGName; // 상위그룹명 설정 시 사용될 인풋필드

    [Header("첫 설정 설정할 내용들을 저장할 쿼리 문자열 배열")]
    public string[] f_arrSettingQuery;
    public string f_hgElement_Id = string.Empty;
    public string f_hgElement_Name = string.Empty;
    public string f_hgElement_ImgPath = string.Empty;
    public string f_selectedImgPath = string.Empty;

    private int f_selectedImgWidth;
    private int f_selectedImgHeight;

    // 이하 첫 설정 이후 사용 시 요소
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();
    [Header("그룹 설정요소")]
    public GameObject settingGroup;
    public GameObject groupListScrollView; // 그룹 리스트 스크롤뷰
    public static Transform groupListContent; // 그룹 리스트 스크롤뷰 Content
    public static Transform hgContainerTransform; // Container_HighGroup의 Transform
    public static Transform lgContainerTransform; // Container_LowGroup의 Transform
    private Button btnClose;
    private Button btnAddHighGroup;
    private Button btnSave;

    [Header("상위그룹 설정요소")]
    public GameObject settingHighGroup; // 상위그룹 설정화면
    private TMP_InputField inputFieldHGName; // 상위그룹명 설정 시 사용될 인풋필드

    [Header("설정할 내용들을 저장할 쿼리 문자열 배열")]
    public string[] arrSettingQuery;
    public string hgElement_Id = string.Empty;
    public string hgElement_Name = string.Empty;
    public string hgElement_ImgPath = string.Empty;
    public string selectedImgPath = string.Empty;

    private int selectedImgWidth;
    private int selectedImgHeight;


    public static GroupSettingManager Instance { get; private set; }
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

        // 첫 설정 시
        f_groupListContent = f_groupListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();
        f_btnAddHighGroup = f_settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/AddGroup/btn_AddGroup").GetComponent<Button>();
        f_btnAddHighGroup.onClick.AddListener(() => AddHighGroupContainer());
        f_btnClose = f_settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Close").GetComponent<Button>();
        f_btnClose.onClick.AddListener(() => CloseGroupSetting());
        f_btnSave = f_settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Save").GetComponent<Button>();
        f_btnSave.onClick.AddListener(() => {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = "그룹 설정이 완료 되었습니다.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => 
            {
                ScreenManager.Instance.ClosePopUpMessage();
                f_settingGroup.SetActive(false);
                FirstStartManager.Instance.firstSet_Group.SetActive(false);
                FirstStartManager.Instance.StartInterfaceSettings();
            });
        });

        f_inputFieldHGName = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/InputField_DefineHighGroupName").GetComponent<TMP_InputField>();

        // 첫 설정 이후
        groupListContent = groupListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();
        btnAddHighGroup = settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/AddGroup/btn_AddGroup").GetComponent<Button>();
        btnAddHighGroup.onClick.AddListener(() => AddHighGroupContainer());
        btnClose = settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(() => CloseGroupSetting());
        btnSave = settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Save").GetComponent<Button>();
        btnSave.onClick.AddListener(() => {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = "그룹 설정이 완료 되었습니다.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => { ScreenManager.Instance.ClosePopUpMessage(); settingGroup.SetActive(false); });
        });

        inputFieldHGName = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/InputField_DefineHighGroupName").GetComponent<TMP_InputField>();
    }

    public void CloseGroupSetting()
    {        
        if(ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_hgElement_Id = string.Empty;
            f_hgElement_Name = string.Empty;
            f_hgElement_ImgPath = string.Empty;
            f_hgContainerTransform = null;
            f_lgContainerTransform = null;

            ObjectPool.Instance.f_CloseGroupSetting();
            f_uiElements.Clear();
            for (int i = f_groupListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_groupListContent.transform.GetChild(i).gameObject);
            }
            f_settingGroup.SetActive(false);
        }
        else
        {
            hgElement_Id = string.Empty;
            hgElement_Name = string.Empty;
            hgElement_ImgPath = string.Empty;
            hgContainerTransform = null;
            lgContainerTransform = null;

            ObjectPool.Instance.CloseGroupSetting();
            uiElements.Clear();
            for (int i = groupListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(groupListContent.transform.GetChild(i).gameObject);
            }
            settingGroup.SetActive(false);
        }
    }

    // 상위 그룹 컨테이너 추가
    private void AddHighGroupContainer()
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // 상위그룹에 대해 이미 존재하는 하위그룹 ID의 최대값 +1에 대한 새로운 하위그룹 INSERT
            DataTable hgTableMaxId = ClientDatabase.FetchHighGroupData().Tables[0];
            var maxHgIdRow = hgTableMaxId.AsEnumerable()
                .Where(r => r.Field<string>("FLD_IMG_PATH") == "this group have to img path define")
                .Select(r => r.Field<short>("FLD_HGID"))
                .DefaultIfEmpty((short)0) // 기본값 설정을 통해 하위 그룹이 없을 경우를 처리
                .Max();
            int newHgId = maxHgIdRow + 1; // 새로운 FLD_LGID 값을 결정

            // 상위그룹 요소 생성        
            string query = $"INSERT INTO TBL_HIGH_GROUP (FLD_NAME, FLD_IMG_PATH, FLD_IMG_WIDTH, FLD_IMG_HEIGHT) VALUES ('새 상위그룹{newHgId}', 'this group have to img path define', '0', '0')";
            ClientDatabase.OnInsertRequest(query);

            // 새롭게 추가된 행의 FLD_HGID 데이터 추출
            DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
            var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"새 상위그룹{newHgId}" && r.Field<string>("FLD_IMG_PATH") == "this group have to img path define");

            // 그룹 설정 화면에 새로운 상위그룹 컨테이너 인스턴스 생성
            GameObject newhighGroupContainer = ObjectPool.Instance.f_GetGroupSettingHGContainerObject(); // 새로운 상위그룹 컨테이너
            string hgContainerName = $"Container_HighGroup_{row["FLD_HGID"].ToString()}";
            newhighGroupContainer.name = hgContainerName;

            GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // 컨테이너 속 상위그룹 본체        
            Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // 상위그룹 설정 버튼
            TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // 상위그룹명
            Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // 상위그룹 삭제 버튼
            Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // 하위그룹 추가 버튼
            f_uiElements[hgContainerName] = newhighGroupContainer;

            f_hgContainerTransform = newhighGroupContainer.transform;
            Transform hgTrs = f_hgContainerTransform;

            newObjHighGroup.name = $"HighGroup_{row["FLD_HGID"].ToString()}";
            newTxtHighGroupName.text = row["FLD_NAME"].ToString();
            AddHighGroupButtonListener(newObjHighGroup, hgTrs, row["FLD_HGID"].ToString(), row["FLD_NAME"].ToString(), row["FLD_IMG_PATH"].ToString(), row["FLD_IMG_WIDTH"].ToString(), row["FLD_IMG_HEIGHT"].ToString()); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)


            LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(f_hgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            // 상위그룹에 대해 이미 존재하는 하위그룹 ID의 최대값 +1에 대한 새로운 하위그룹 INSERT
            DataTable hgTableMaxId = ClientDatabase.FetchHighGroupData().Tables[0];
            var maxHgIdRow = hgTableMaxId.AsEnumerable()
                .Where(r => r.Field<string>("FLD_IMG_PATH") == "this group have to img path define")
                .Select(r => r.Field<short>("FLD_HGID"))
                .DefaultIfEmpty((short)0) // 기본값 설정을 통해 하위 그룹이 없을 경우를 처리
                .Max();
            int newHgId = maxHgIdRow + 1; // 새로운 FLD_LGID 값을 결정

            // 상위그룹 요소 생성        
            string query = $"INSERT INTO TBL_HIGH_GROUP (FLD_NAME, FLD_IMG_PATH, FLD_IMG_WIDTH, FLD_IMG_HEIGHT) VALUES ('새 상위그룹{newHgId}', 'this group have to img path define', '0', '0')";
            ClientDatabase.OnInsertRequest(query);

            // 새롭게 추가된 행의 FLD_HGID 데이터 추출
            DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
            var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"새 상위그룹{newHgId}" && r.Field<string>("FLD_IMG_PATH") == "this group have to img path define");

            // 그룹 설정 화면에 새로운 상위그룹 컨테이너 인스턴스 생성
            GameObject newhighGroupContainer = ObjectPool.Instance.GetGroupSettingHGContainerObject(); // 새로운 상위그룹 컨테이너
            string hgContainerName = $"Container_HighGroup_{row["FLD_HGID"].ToString()}";
            newhighGroupContainer.name = hgContainerName;

            GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // 컨테이너 속 상위그룹 본체        
            Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // 상위그룹 설정 버튼
            TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // 상위그룹명
            Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // 상위그룹 삭제 버튼
            Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // 하위그룹 추가 버튼
            uiElements[hgContainerName] = newhighGroupContainer;

            hgContainerTransform = newhighGroupContainer.transform;
            Transform hgTrs = hgContainerTransform;

            newObjHighGroup.name = $"HighGroup_{row["FLD_HGID"].ToString()}";
            newTxtHighGroupName.text = row["FLD_NAME"].ToString();
            AddHighGroupButtonListener(newObjHighGroup, hgTrs, row["FLD_HGID"].ToString(), row["FLD_NAME"].ToString(), row["FLD_IMG_PATH"].ToString(), row["FLD_IMG_WIDTH"].ToString(), row["FLD_IMG_HEIGHT"].ToString()); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)


            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
    }

    // 하위 그룹 컨테이너 추가
    private void AddLowGroupContainer(string hgid, Transform hgTransform)
    {
        if(ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // 상위그룹에 대해 이미 존재하는 하위그룹 ID의 최대값 +1에 대한 새로운 하위그룹 INSERT
            DataTable lgTableMaxId = ClientDatabase.FetchLowGroupData().Tables[0];
            var maxLgIdRow = lgTableMaxId.AsEnumerable()
                .Where(r => r.Field<short>("FLD_HGID") == short.Parse(hgid))
                .Select(r => r.Field<short>("FLD_LGID"))
                .DefaultIfEmpty((short)0) // 기본값 설정을 통해 하위 그룹이 없을 경우를 처리
                .Max();
            int newLgId = maxLgIdRow + 1; // 새로운 FLD_LGID 값을 결정
            string query = $"INSERT INTO TBL_LOW_GROUP (FLD_HGID, FLD_LGID, FLD_NAME) VALUES ('{hgid}', '{newLgId}', '새 하위그룹{newLgId}')";
            ClientDatabase.OnInsertRequest(query);

            // 새롭게 추가된 행의 FLD_LGID 추출
            DataTable lgTable = ClientDatabase.FetchLowGroupData().Tables[0];

            // FLD_HGID가 hgid이고 FLD_LGID가 newLgId인 행 찾기
            DataRow row = lgTable.AsEnumerable()
                .FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"새 하위그룹{newLgId}");

            // 조건에 맞는 행이 있는지 확인하고, 해당 행의 데이터 처리
            if (row != null)
            {
                // 조건에 맞는 행이 있다면, 해당 행의 데이터를 이용한 처리를 진행
                string lgContainerName = $"Container_LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";

                // 그룹 설정 화면에 새로운 상위그룹 컨테이너 인스턴스 생성
                GameObject newlowGroupContainer = Instantiate(ObjectPool.Instance.f_groupSettingLGContainerPrefab, hgTransform);
                ObjectPool.Instance.f_GroupSettingHGContainerObjects.Add(newlowGroupContainer);

                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // 컨테이너 속 하위그룹 본체
                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // 하위그룹 삭제 버튼

                f_uiElements[lgContainerName] = newlowGroupContainer;

                f_lgContainerTransform = newlowGroupContainer.transform;
                newlowGroupContainer.name = lgContainerName;
                newObjLowGroup.name = $"LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";
                ipLowGroupName.placeholder.enabled = false;
                ipLowGroupName.text = row["FLD_NAME"].ToString();
                AddLowGroupButtonListener(newObjLowGroup, hgid, row["FLD_LGID"].ToString(), row["FLD_NAME"].ToString()); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
            }
            else
            {
                // 조건에 맞는 행이 없는 경우의 처리
                //Debug.Log("No matching row found.");
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgTransform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(f_lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            // 상위그룹에 대해 이미 존재하는 하위그룹 ID의 최대값 +1에 대한 새로운 하위그룹 INSERT
            DataTable lgTableMaxId = ClientDatabase.FetchLowGroupData().Tables[0];
            var maxLgIdRow = lgTableMaxId.AsEnumerable()
                .Where(r => r.Field<short>("FLD_HGID") == short.Parse(hgid))
                .Select(r => r.Field<short>("FLD_LGID"))
                .DefaultIfEmpty((short)0) // 기본값 설정을 통해 하위 그룹이 없을 경우를 처리
                .Max();
            int newLgId = maxLgIdRow + 1; // 새로운 FLD_LGID 값을 결정
            string query = $"INSERT INTO TBL_LOW_GROUP (FLD_HGID, FLD_LGID, FLD_NAME) VALUES ('{hgid}', '{newLgId}', '새 하위그룹{newLgId}')";
            ClientDatabase.OnInsertRequest(query);

            // 새롭게 추가된 행의 FLD_LGID 추출
            DataTable lgTable = ClientDatabase.FetchLowGroupData().Tables[0];

            // FLD_HGID가 hgid이고 FLD_LGID가 newLgId인 행 찾기
            DataRow row = lgTable.AsEnumerable()
                .FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"새 하위그룹{newLgId}");

            // 조건에 맞는 행이 있는지 확인하고, 해당 행의 데이터 처리
            if (row != null)
            {
                // 조건에 맞는 행이 있다면, 해당 행의 데이터를 이용한 처리를 진행
                string lgContainerName = $"Container_LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";

                // 그룹 설정 화면에 새로운 상위그룹 컨테이너 인스턴스 생성
                GameObject newlowGroupContainer = Instantiate(ObjectPool.Instance.groupSettingLGContainerPrefab, hgTransform);
                ObjectPool.Instance.GroupSettingHGContainerObjects.Add(newlowGroupContainer);

                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // 컨테이너 속 하위그룹 본체
                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // 하위그룹 삭제 버튼

                uiElements[lgContainerName] = newlowGroupContainer;

                lgContainerTransform = newlowGroupContainer.transform;
                newlowGroupContainer.name = lgContainerName;
                newObjLowGroup.name = $"LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";
                ipLowGroupName.placeholder.enabled = false;
                ipLowGroupName.text = row["FLD_NAME"].ToString();
                AddLowGroupButtonListener(newObjLowGroup, hgid, row["FLD_LGID"].ToString(), row["FLD_NAME"].ToString()); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
            }
            else
            {
                // 조건에 맞는 행이 없는 경우의 처리
                //Debug.Log("No matching row found.");
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgTransform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        
    }

    // 하위그룹 버튼 리스너
    private void AddLowGroupButtonListener(GameObject objLowGroup, string hgid, string lgid, string lgName)
    {
        TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(objLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>(); // 하위그룹 이름 설정 인풋필드
        Button btnDeleteLowGroup = objLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // 하위그룹 삭제 버튼

        btnDeleteLowGroup.GetComponent<Button>().onClick.RemoveAllListeners();
        btnDeleteLowGroup.GetComponent<Button>().onClick.AddListener(() => DeleteLowGroup(objLowGroup, hgid, lgid, lgName));

        // onValueChanged 리스너 추가
        ipLowGroupName.onEndEdit.RemoveAllListeners(); // 기존 리스너 제거
        ipLowGroupName.onEndEdit.AddListener(delegate
        {
            if (ipLowGroupName.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "하위 그룹명이 입력되지 않았습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
            else
            {
                UpdateLowGroupName(hgid, lgid, lgName, ipLowGroupName.text);

                // 하위그룹 삭제 리스너도 변경된 이름으로 업데이트
                btnDeleteLowGroup.GetComponent<Button>().onClick.RemoveAllListeners();
                btnDeleteLowGroup.GetComponent<Button>().onClick.AddListener(() => DeleteLowGroup(objLowGroup, hgid, lgid, ipLowGroupName.text));
            }
        }); // 새로운 값으로 DB 업데이트
    }

    // 하위그룹 인풋필드 이름변경 완료 리스너
    private void UpdateLowGroupName(string hgid, string lgid, string beforeValue, string changeValue)
    {
        // 쿼리 문법 수정: FLD_NAME의 값을 올바른 changeValue로 업데이트하고, 문자열 값에 대해 작은 따옴표(') 추가
        string query = $"UPDATE TBL_LOW_GROUP SET FLD_NAME = '{changeValue}' WHERE FLD_HGID = '{hgid}' AND FLD_LGID = '{lgid}'";
        if (ClientDatabase.OnUpdateRequest(query))
        {
            //Debug.Log($"하위그룹명을 기존 {beforeValue}에서 {changeValue}로 변경 완료");
        }
        else
        {
            //Debug.Log($"하위그룹명 변경 실패");
        }
    }

    // 상위그룹 버튼 리스너
    private void AddHighGroupButtonListener(GameObject objHighGroup, Transform hgTransform, string hgid, string hgName, string fpImgpath, string fpImgWidth, string fpImgHeight)
    {
        Button btnOpenGroupList = objHighGroup.transform.Find("btn_OpenGroupList").GetComponent<Button>(); // 하위그룹 리스트 열기 버튼
        Button btnHighGroupSetting = objHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // 상위그룹 설정 버튼
        Button btnDeleteHighGroup = objHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // 상위그룹 삭제 버튼
        Button btnAddLowGroup = objHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // 하위그룹 추가 버튼

        btnOpenGroupList.GetComponent<Button>().onClick.AddListener(() => HandleLowGroupVisibility(hgid));
        btnHighGroupSetting.GetComponent<Button>().onClick.AddListener(() => HandleHighGroup(hgid, hgName, fpImgpath, fpImgWidth, fpImgHeight));
        btnDeleteHighGroup.GetComponent<Button>().onClick.AddListener(() => DeleteHighGroup(objHighGroup, hgTransform, hgid, hgName, fpImgpath, fpImgWidth, fpImgHeight));
        btnAddLowGroup.GetComponent<Button>().onClick.AddListener(() => AddLowGroupContainer(hgid, hgTransform));
    }

    // 상위 그룹 삭제
    public void DeleteHighGroup(GameObject objHighGroup, Transform hgTransform, string hgid, string hgName, string fpImgpath, string fpImgWidth, string fpImgHeight)
    {
        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
        ScreenManager.Instance.txt_PopUpMsg.text = $"{hgName} 그룹을 삭제하시겠습니까?\n연결된 평면도, 하위그룹, 컨트롤러의\n그룹정보도 함께 삭제됩니다.";
        
        ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
        ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
            // TBL_CONTROLLER에 연결된 그룹정보 초기화
            string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET HGID = 0, LGID = 0, GROUP_ORDER = 0 WHERE HGID = {hgid};";
            // TBL_LOW_GROUP에 연결된 그룹정보 삭제
            string tblLowGroupQuery = $"DELETE FROM TBL_LOW_GROUP WHERE FLD_HGID = {hgid};";
            // TBL_HIGH_GROUP에 해당하는 그룹정보 삭제
            string tblHighGroupQuery = $"DELETE FROM TBL_HIGH_GROUP WHERE FLD_HGID = {hgid};";

            bool controllerUpdated = ClientDatabase.OnDeleteRequest(tblControllerQuery);
            bool lowGroupDeleted = ClientDatabase.OnDeleteRequest(tblLowGroupQuery);
            bool highGroupDeleted = ClientDatabase.OnDeleteRequest(tblHighGroupQuery);

            if (controllerUpdated && lowGroupDeleted && highGroupDeleted)
            {
                StartCoroutine(RefreshUIAfterCleanup());
                ScreenManager.Instance.ClosePopUpMessage();
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                //Debug.Log($"상위그룹 정보 삭제 완료 : {hgName}, {hgid}");
            }
            else
            {
                //Debug.Log($"상위그룹 정보 삭제 실패 : {hgName}, {hgid}");
            }
        });
    }

    // 하위 그룹 삭제
    public void DeleteLowGroup(GameObject objLowGroup, string hgid, string lgid, string lgName)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
            ScreenManager.Instance.txt_PopUpMsg.text = $"{lgName} 그룹을 삭제하시겠습니까?\n연결된 컨트롤러의 그룹정보도\n함께 삭제됩니다.";

            ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
                // TBL_CONTROLLER에 연결된 그룹정보 초기화
                string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET HGID = 0, LGID = 0, GROUP_ORDER = 0 WHERE HGID = {hgid} AND LGID = {lgid};";
                // TBL_LOW_GROUP에 연결된 그룹정보 삭제
                string tblLowGroupQuery = $"DELETE FROM TBL_LOW_GROUP WHERE FLD_HGID = {hgid} AND FLD_LGID = {lgid};";

                bool controllerUpdated = ClientDatabase.OnDeleteRequest(tblControllerQuery);
                bool lowGroupDeleted = ClientDatabase.OnDeleteRequest(tblLowGroupQuery);

                if (controllerUpdated && lowGroupDeleted)
                {
                    //Debug.Log($"하위그룹 정보 삭제 완료 : {lgName}, {lgid}");
                    StartCoroutine(RefreshUIAfterCleanup());
                    ScreenManager.Instance.ClosePopUpMessage();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();

                    LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(f_hgContainerTransform.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(f_lgContainerTransform.GetComponent<RectTransform>());
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    //Debug.Log($"상위그룹 정보 삭제 실패 : {lgName}, {lgid}");
                }
            });
        }
        else
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
            ScreenManager.Instance.txt_PopUpMsg.text = $"{lgName} 그룹을 삭제하시겠습니까?\n연결된 컨트롤러의 그룹정보도\n함께 삭제됩니다.";

            ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
                // TBL_CONTROLLER에 연결된 그룹정보 초기화
                string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET HGID = 0, LGID = 0, GROUP_ORDER = 0 WHERE HGID = {hgid} AND LGID = {lgid};";
                // TBL_LOW_GROUP에 연결된 그룹정보 삭제
                string tblLowGroupQuery = $"DELETE FROM TBL_LOW_GROUP WHERE FLD_HGID = {hgid} AND FLD_LGID = {lgid};";

                bool controllerUpdated = ClientDatabase.OnDeleteRequest(tblControllerQuery);
                bool lowGroupDeleted = ClientDatabase.OnDeleteRequest(tblLowGroupQuery);

                if (controllerUpdated && lowGroupDeleted)
                {
                    //Debug.Log($"하위그룹 정보 삭제 완료 : {lgName}, {lgid}");
                    StartCoroutine(RefreshUIAfterCleanup());
                    ScreenManager.Instance.ClosePopUpMessage();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();

                    LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(hgContainerTransform.GetComponent<RectTransform>());
                    LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
                    Canvas.ForceUpdateCanvases();
                }
                else
                {
                    //Debug.Log($"상위그룹 정보 삭제 실패 : {lgName}, {lgid}");
                }
            });
        }        
    }

    public void OpenGroupSetting()
    {        
        SideMenuManager.Instance.SideMenuStateChange();
        LoadGroupAssets();
    }

    // 각 그룹 요소에 Button 생성
    public void LoadGroupAssets()
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableController = ClientDatabase.FetchControllerData().Tables[0];

            f_settingGroup.SetActive(true);

            if (tableHighGroup.Rows.Count > 0)
            {
                foreach (DataRow highGroupRow in tableHighGroup.Rows)
                {
                    string hgid = highGroupRow["FLD_HGID"].ToString();
                    string hgName = highGroupRow["FLD_NAME"].ToString();
                    string fpImgPath = highGroupRow["FLD_IMG_PATH"].ToString();
                    string fpImgWidth = highGroupRow["FLD_IMG_WIDTH"].ToString();
                    string fpImgHeight = highGroupRow["FLD_IMG_HEIGHT"].ToString();
                    string hgContainerName = $"Container_HighGroup_{hgid}";

                    if (f_uiElements.TryGetValue(hgContainerName, out var highGroupContainer))
                    {
                        // 기존 상위그룹 요소가 있으면 내용만 업데이트
                        GameObject btnHighGroup = SideMenuManager.FindChildStartingWithName(highGroupContainer.transform, "HighGroup_");
                        TextMeshProUGUI txtHighGroupName = SideMenuManager.FindChildStartingWithName(btnHighGroup.transform, "btn_GroupSetting/txt_GroupName").GetComponent<TextMeshProUGUI>();

                        GameObject activeImage = highGroupContainer.transform.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Open").gameObject;
                        GameObject deactiveImage = highGroupContainer.transform.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Close").gameObject;
                        deactiveImage.SetActive(true);
                        activeImage.SetActive(false);

                        f_hgContainerTransform = highGroupContainer.transform;
                        txtHighGroupName.text = hgName;
                    }
                    else
                    {
                        GameObject newhighGroupContainer = ObjectPool.Instance.f_GetGroupSettingHGContainerObject(); // 새로운 상위그룹 컨테이너
                        GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // 컨테이너 속 상위그룹 본체                
                        Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // 상위그룹 설정 버튼
                        TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // 상위그룹명
                        Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // 상위그룹 삭제 버튼
                        Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // 하위그룹 추가 버튼
                        f_uiElements[hgContainerName] = newhighGroupContainer;

                        f_hgContainerTransform = newhighGroupContainer.transform;
                        newhighGroupContainer.name = $"Container_HighGroup_{hgid}";
                        newObjHighGroup.name = $"HighGroup_{hgid}";
                        newTxtHighGroupName.text = hgName;
                        AddHighGroupButtonListener(newObjHighGroup, f_hgContainerTransform, hgid, hgName, fpImgPath, fpImgWidth, fpImgHeight); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
                    }

                    // 상위그룹에 해당하는 하위그룹만 필터링하여 추가            
                    var filteredLowGroups = tableLowGroup.AsEnumerable().Where(row => row["FLD_HGID"] != DBNull.Value && row.Field<short>("FLD_HGID").ToString() == hgid);

                    if (tableLowGroup.Rows.Count > 0)
                    {
                        // 상위그룹의 LowGroupContainer에 하위그룹 요소 생성
                        foreach (DataRow lowGroupRow in filteredLowGroups)
                        {
                            string lgid = lowGroupRow["FLD_LGID"].ToString();
                            string lgName = lowGroupRow["FLD_NAME"].ToString();
                            string lgContainerName = $"Container_LowGroup_{hgid}_{lgid}";

                            if (f_uiElements.TryGetValue(lgContainerName, out var lowGroupContainer))
                            {
                                // 기존 하위그룹 요소가 있으면 내용만 업데이트
                                GameObject btnLowGroup = SideMenuManager.FindChildStartingWithName(lowGroupContainer.transform, "LowGroup_");
                                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(btnLowGroup.transform, "txt_LowGroupName").GetComponent<TMP_InputField>();
                                TextMeshProUGUI txtLowGroupName = SideMenuManager.FindChildStartingWithName(btnLowGroup.transform, "txt_LowGroupName").GetComponent<TextMeshProUGUI>();
                                GameObject activeImage = lowGroupContainer.transform.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Active").gameObject;
                                GameObject deactiveImage = lowGroupContainer.transform.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Deactive").gameObject;
                                deactiveImage.SetActive(true);
                                activeImage.SetActive(false);
                                f_lgContainerTransform = lowGroupContainer.transform;
                                txtLowGroupName.text = lgName;
                            }
                            else
                            {
                                // 하위그룹 요소 생성
                                GameObject newlowGroupContainer = ObjectPool.Instance.f_GetGroupSettingLGContainerObject(); // 새로운 하위그룹 컨테이너
                                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // 컨테이너 속 하위그룹 본체
                                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // 하위그룹 삭제 버튼

                                f_uiElements[lgContainerName] = newlowGroupContainer;

                                f_lgContainerTransform = newlowGroupContainer.transform;
                                newlowGroupContainer.name = $"Container_LowGroup_{hgid}_{lgid}";
                                newObjLowGroup.name = $"LowGroup_{hgid}_{lgid}";
                                ipLowGroupName.placeholder.enabled = false;
                                ipLowGroupName.text = lgName;
                                AddLowGroupButtonListener(newObjLowGroup, hgid, lgid, lgName);
                            }
                        }
                    }
                }

                foreach (var uiElement in f_uiElements)
                {
                    // 'Container_HighGroup_'로 시작하지 않는 모든 오브젝트를 비활성화
                    if (!uiElement.Key.StartsWith("Container_HighGroup_"))
                    {
                        GameObject obj = uiElement.Value;
                        if (obj != null) // 오브젝트가 null이 아닌지 확인
                        {
                            obj.SetActive(false);
                        }
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(f_hgContainerTransform.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
        }
        else
        {
            DataTable tableHighGroup = ClientDatabase.FetchHighGroupData().Tables[0];
            DataTable tableLowGroup = ClientDatabase.FetchLowGroupData().Tables[0];
            DataTable tableController = ClientDatabase.FetchControllerData().Tables[0];

            settingGroup.SetActive(true);

            if (tableHighGroup.Rows.Count > 0)
            {
                foreach (DataRow highGroupRow in tableHighGroup.Rows)
                {
                    string hgid = highGroupRow["FLD_HGID"].ToString();
                    string hgName = highGroupRow["FLD_NAME"].ToString();
                    string fpImgPath = highGroupRow["FLD_IMG_PATH"].ToString();
                    string fpImgWidth = highGroupRow["FLD_IMG_WIDTH"].ToString();
                    string fpImgHeight = highGroupRow["FLD_IMG_HEIGHT"].ToString();
                    string hgContainerName = $"Container_HighGroup_{hgid}";

                    if (uiElements.TryGetValue(hgContainerName, out var highGroupContainer))
                    {
                        // 기존 상위그룹 요소가 있으면 내용만 업데이트
                        GameObject btnHighGroup = SideMenuManager.FindChildStartingWithName(highGroupContainer.transform, "HighGroup_");
                        TextMeshProUGUI txtHighGroupName = SideMenuManager.FindChildStartingWithName(btnHighGroup.transform, "btn_GroupSetting/txt_GroupName").GetComponent<TextMeshProUGUI>();

                        GameObject activeImage = highGroupContainer.transform.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Open").gameObject;
                        GameObject deactiveImage = highGroupContainer.transform.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Close").gameObject;
                        deactiveImage.SetActive(true);
                        activeImage.SetActive(false);

                        hgContainerTransform = highGroupContainer.transform;
                        txtHighGroupName.text = hgName;
                    }
                    else
                    {
                        GameObject newhighGroupContainer = ObjectPool.Instance.GetGroupSettingHGContainerObject(); // 새로운 상위그룹 컨테이너
                        GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // 컨테이너 속 상위그룹 본체                
                        Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // 상위그룹 설정 버튼
                        TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // 상위그룹명
                        Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // 상위그룹 삭제 버튼
                        Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // 하위그룹 추가 버튼
                        uiElements[hgContainerName] = newhighGroupContainer;

                        hgContainerTransform = newhighGroupContainer.transform;
                        newhighGroupContainer.name = $"Container_HighGroup_{hgid}";
                        newObjHighGroup.name = $"HighGroup_{hgid}";
                        newTxtHighGroupName.text = hgName;
                        AddHighGroupButtonListener(newObjHighGroup, hgContainerTransform, hgid, hgName, fpImgPath, fpImgWidth, fpImgHeight); // 이벤트 리스너 추가(화살표 변경 및 하위그룹 숨김 핸들러)
                    }

                    // 상위그룹에 해당하는 하위그룹만 필터링하여 추가            
                    var filteredLowGroups = tableLowGroup.AsEnumerable().Where(row => row["FLD_HGID"] != DBNull.Value && row.Field<short>("FLD_HGID").ToString() == hgid);

                    if (tableLowGroup.Rows.Count > 0)
                    {
                        // 상위그룹의 LowGroupContainer에 하위그룹 요소 생성
                        foreach (DataRow lowGroupRow in filteredLowGroups)
                        {
                            string lgid = lowGroupRow["FLD_LGID"].ToString();
                            string lgName = lowGroupRow["FLD_NAME"].ToString();
                            string lgContainerName = $"Container_LowGroup_{hgid}_{lgid}";

                            if (uiElements.TryGetValue(lgContainerName, out var lowGroupContainer))
                            {
                                // 기존 하위그룹 요소가 있으면 내용만 업데이트
                                GameObject btnLowGroup = SideMenuManager.FindChildStartingWithName(lowGroupContainer.transform, "LowGroup_");
                                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(btnLowGroup.transform, "txt_LowGroupName").GetComponent<TMP_InputField>();
                                TextMeshProUGUI txtLowGroupName = SideMenuManager.FindChildStartingWithName(btnLowGroup.transform, "txt_LowGroupName").GetComponent<TextMeshProUGUI>();
                                GameObject activeImage = lowGroupContainer.transform.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Active").gameObject;
                                GameObject deactiveImage = lowGroupContainer.transform.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Deactive").gameObject;
                                deactiveImage.SetActive(true);
                                activeImage.SetActive(false);
                                lgContainerTransform = lowGroupContainer.transform;
                                txtLowGroupName.text = lgName;
                            }
                            else
                            {
                                // 하위그룹 요소 생성
                                GameObject newlowGroupContainer = ObjectPool.Instance.GetGroupSettingLGContainerObject(); // 새로운 하위그룹 컨테이너
                                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // 컨테이너 속 하위그룹 본체
                                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // 하위그룹 삭제 버튼

                                uiElements[lgContainerName] = newlowGroupContainer;

                                lgContainerTransform = newlowGroupContainer.transform;
                                newlowGroupContainer.name = $"Container_LowGroup_{hgid}_{lgid}";
                                newObjLowGroup.name = $"LowGroup_{hgid}_{lgid}";
                                ipLowGroupName.placeholder.enabled = false;
                                ipLowGroupName.text = lgName;
                                AddLowGroupButtonListener(newObjLowGroup, hgid, lgid, lgName);
                            }
                        }
                    }
                }

                foreach (var uiElement in uiElements)
                {
                    // 'Container_HighGroup_'로 시작하지 않는 모든 오브젝트를 비활성화
                    if (!uiElement.Key.StartsWith("Container_HighGroup_"))
                    {
                        GameObject obj = uiElement.Value;
                        if (obj != null) // 오브젝트가 null이 아닌지 확인
                        {
                            obj.SetActive(false);
                        }
                    }
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(hgContainerTransform.GetComponent<RectTransform>());
                //LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
        }       
    }

    // 상위그룹 설정 핸들러(상위그룹명, 상위그룹 평면도 이미지)
    private void HandleHighGroup(string hgid, string hgName, string fpImgpath, string fpImgWidth, string fpImgHeight)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_settingHighGroup.SetActive(true);

            Image fpImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // 평면도 이미지 파일 선택 버튼
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();
            Button btnPrev = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Prev").GetComponent<Button>(); // 뒤로가기(=취소) 버튼

            btnChooseFPImage.onClick.AddListener(() => OpenFileChooser()); // 이미지 선택에 대한 리스너
            btnPrev.onClick.AddListener(() =>
            {
                fpImage.sprite = null;

                f_hgElement_Id = string.Empty;
                f_hgElement_Name = string.Empty;
                f_hgElement_ImgPath = string.Empty;
                f_settingHighGroup.SetActive(false);
            }); // 상위그룹 설정 취소 및 돌아가기 리스너


            if (f_inputFieldHGName.placeholder is TMP_Text placeholderText)
                placeholderText.text = "상위그룹명 입력";
            f_inputFieldHGName.text = string.Empty;

            if (hgName == string.Empty || fpImgpath == string.Empty) // 기존에 그룹 정보가 DB에 없는 경우
            {
                f_inputFieldHGName.placeholder.enabled = true;
                f_inputFieldHGName.text = "";
                txtChooseFPImage.text = "이미지 선택";
            }
            else // 해당 그룹 정보가 존재하는 경우
            {
                f_hgElement_Id = hgid;
                f_hgElement_Name = hgName;
                f_hgElement_ImgPath = fpImgpath;

                f_inputFieldHGName.placeholder.enabled = false;
                f_inputFieldHGName.text = hgName;

                txtChooseFPImage.text = "이미지 다시 선택";
                StartCoroutine(LoadImageCoroutine(fpImgpath));

                Button btnConfirm = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // 확인 -> 그룹설정 화면으로 설정정보 이관
                btnConfirm.onClick.RemoveAllListeners();
                btnConfirm.onClick.AddListener(() =>
                {
                    FinishHighGroupSetting(fpImgpath);
                }); // 확인 및 그룹설정으로 설정정보 이관 리스너
            }
        }
        else
        {
            settingHighGroup.SetActive(true);

            Image fpImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // 평면도 이미지 파일 선택 버튼
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();
            Button btnPrev = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Prev").GetComponent<Button>(); // 뒤로가기(=취소) 버튼

            btnChooseFPImage.onClick.AddListener(() => OpenFileChooser()); // 이미지 선택에 대한 리스너
            btnPrev.onClick.AddListener(() =>
            {
                fpImage.sprite = null;

                hgElement_Id = string.Empty;
                hgElement_Name = string.Empty;
                hgElement_ImgPath = string.Empty;
                settingHighGroup.SetActive(false);
            }); // 상위그룹 설정 취소 및 돌아가기 리스너


            if (inputFieldHGName.placeholder is TMP_Text placeholderText)
                placeholderText.text = "상위그룹명 입력";
            inputFieldHGName.text = string.Empty;

            if (hgName == string.Empty || fpImgpath == string.Empty) // 기존에 그룹 정보가 DB에 없는 경우
            {
                inputFieldHGName.placeholder.enabled = true;
                inputFieldHGName.text = "";
                txtChooseFPImage.text = "이미지 선택";
            }
            else // 해당 그룹 정보가 존재하는 경우
            {
                hgElement_Id = hgid;
                hgElement_Name = hgName;
                hgElement_ImgPath = fpImgpath;

                inputFieldHGName.placeholder.enabled = false;
                inputFieldHGName.text = hgName;

                txtChooseFPImage.text = "이미지 다시 선택";
                StartCoroutine(LoadImageCoroutine(fpImgpath));

                Button btnConfirm = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // 확인 -> 그룹설정 화면으로 설정정보 이관
                btnConfirm.onClick.RemoveAllListeners();
                btnConfirm.onClick.AddListener(() =>
                {
                    FinishHighGroupSetting(fpImgpath);
                }); // 확인 및 그룹설정으로 설정정보 이관 리스너
            }
        }        
    }

    // 상위그룹 설정 화면 확인 버튼
    private void FinishHighGroupSetting(string selectedImgPath)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // inputFieldHGName의 text 값이 비었는지 확인
            if (f_inputFieldHGName.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "상위 그룹명이 입력되지 않았습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
            else
            {
                // FLD_HGID로 이미지 파일명이 성공적으로 수정되면 FLD_IMG_PATH 경로 UPDATE
                if (selectedImgPath == "this group have to img path define")
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "이미지가 선택되지 않았습니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                    //Debug.LogError("상위 그룹 설정 실패");
                }
                else
                {
                    f_hgElement_Name = f_inputFieldHGName.text;
                    string query = $"UPDATE TBL_HIGH_GROUP SET FLD_NAME = '{f_hgElement_Name}', FLD_IMG_PATH = '{selectedImgPath}', FLD_IMG_WIDTH = {f_selectedImgWidth}, FLD_IMG_HEIGHT = {f_selectedImgHeight} WHERE FLD_HGID = {f_hgElement_Id}";
                    ClientDatabase.OnUpdateRequest(query);

                    // 새롭게 추가된 행의 FLD_HGID 데이터 추출
                    DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
                    var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == f_hgElement_Name && r.Field<string>("FLD_IMG_PATH") == selectedImgPath);
                    if (row != null)
                    {
                        f_hgElement_Id = row["FLD_HGID"].ToString();

                        if (RenameImageFile(selectedImgPath, f_hgElement_Id))
                        {
                            StartCoroutine(RefreshUIAfterCleanup());
                            f_settingHighGroup.SetActive(false);
                            //Debug.Log("상위 그룹 설정 완료");
                        }
                        else
                        {
                            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            ScreenManager.Instance.txt_PopUpMsg.text = "파일을 찾을 수 없습니다.";
                            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                            //Debug.LogError("상위 그룹 설정 실패");
                        }
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
        }
        else
        {
            // inputFieldHGName의 text 값이 비었는지 확인
            if (inputFieldHGName.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "상위 그룹명이 입력되지 않았습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
            else
            {
                // FLD_HGID로 이미지 파일명이 성공적으로 수정되면 FLD_IMG_PATH 경로 UPDATE
                if (selectedImgPath == "this group have to img path define")
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "이미지가 선택되지 않았습니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                    //Debug.LogError("상위 그룹 설정 실패");
                }
                else
                {
                    hgElement_Name = inputFieldHGName.text;
                    string query = $"UPDATE TBL_HIGH_GROUP SET FLD_NAME = '{hgElement_Name}', FLD_IMG_PATH = '{selectedImgPath}', FLD_IMG_WIDTH = {selectedImgWidth}, FLD_IMG_HEIGHT = {selectedImgHeight} WHERE FLD_HGID = {hgElement_Id}";
                    ClientDatabase.OnUpdateRequest(query);

                    // 새롭게 추가된 행의 FLD_HGID 데이터 추출
                    DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
                    var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == hgElement_Name && r.Field<string>("FLD_IMG_PATH") == selectedImgPath);
                    if (row != null)
                    {
                        hgElement_Id = row["FLD_HGID"].ToString();

                        if (RenameImageFile(selectedImgPath, hgElement_Id))
                        {
                            //StartCoroutine(RefreshUIAfterCleanup());
                            StartCoroutine(RefreshUIAfterCleanup());
                            settingHighGroup.SetActive(false);
                            //Debug.Log("상위 그룹 설정 완료");
                        }
                        else
                        {
                            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            ScreenManager.Instance.txt_PopUpMsg.text = "파일을 찾을 수 없습니다.";
                            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                            //Debug.LogError("상위 그룹 설정 실패");
                        }
                    }
                }



                LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
        }
    }

    // 설정 변경 후 새로고침
    private IEnumerator RefreshUIAfterCleanup()
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            for (int i = f_groupListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(f_groupListContent.transform.GetChild(i).gameObject);
                yield return new WaitForEndOfFrame();
            }
            ObjectPool.Instance.f_CloseGroupSetting();
            f_uiElements.Clear();

            // 다음 프레임까지 기다립니다.
            yield return new WaitForEndOfFrame();

            // 이제 모든 오브젝트가 제거되었으므로 UI를 새로고침합니다.
            LoadGroupAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            for (int i = groupListContent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(groupListContent.transform.GetChild(i).gameObject);
                yield return new WaitForEndOfFrame();
            }
            ObjectPool.Instance.CloseGroupSetting();
            uiElements.Clear();

            // 다음 프레임까지 기다립니다.
            yield return new WaitForEndOfFrame();

            // 이제 모든 오브젝트가 제거되었으므로 UI를 새로고침합니다.
            LoadGroupAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }

    // 상위그룹 열고 닫기 버튼 핸들러
    private void HandleLowGroupVisibility(string hgid)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            Transform highGroupContainer = f_uiElements[$"Container_HighGroup_{hgid}"].transform;
            bool isActive = false; // 초기 상태 설정

            // btn_HighGroup 하위의 활성화/비활성화 이미지 토글
            GameObject activeImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Open").gameObject;
            GameObject deactiveImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Close").gameObject;

            isActive = !activeImage.activeSelf; // 현재 상태에 따라 반전

            activeImage.SetActive(isActive);
            deactiveImage.SetActive(!isActive);

            // Container_LowGroup로 시작하는 모든 자식 오브젝트의 활성화 상태 토글
            foreach (Transform child in highGroupContainer)
            {
                if (child.name.StartsWith("Container_LowGroup"))
                {
                    child.gameObject.SetActive(isActive);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(highGroupContainer.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Transform highGroupContainer = uiElements[$"Container_HighGroup_{hgid}"].transform;
            bool isActive = false; // 초기 상태 설정

            // btn_HighGroup 하위의 활성화/비활성화 이미지 토글
            GameObject activeImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Open").gameObject;
            GameObject deactiveImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Close").gameObject;

            isActive = !activeImage.activeSelf; // 현재 상태에 따라 반전

            activeImage.SetActive(isActive);
            deactiveImage.SetActive(!isActive);

            // Container_LowGroup로 시작하는 모든 자식 오브젝트의 활성화 상태 토글
            foreach (Transform child in highGroupContainer)
            {
                if (child.name.StartsWith("Container_LowGroup"))
                {
                    child.gameObject.SetActive(isActive);
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(highGroupContainer.GetComponent<RectTransform>());
            //LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }

    // Android 파일 선택 모듈 호출
    void OpenFileChooser()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", new AndroidJavaObject("android.content.Intent", currentActivity, new AndroidJavaClass("com.systronics.plugin.FileChooserActivity")));
    }

    // 파일 경로를 수신하는 함수
    public void ReceiveFilePath(string selectedFilePath)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_hgElement_ImgPath = selectedFilePath;
            f_selectedImgPath = selectedFilePath;
            StartCoroutine(LoadImageCoroutine(selectedFilePath));

            Button btnConfirm = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // 확인 -> 그룹설정 화면으로 설정정보 이관
            btnConfirm.onClick.RemoveAllListeners();
            btnConfirm.onClick.AddListener(() => FinishHighGroupSetting(f_selectedImgPath)); // 확인 및 그룹설정으로 설정정보 이관 리스너
        }
        else
        {
            hgElement_ImgPath = selectedFilePath;
            selectedImgPath = selectedFilePath;
            StartCoroutine(LoadImageCoroutine(selectedFilePath));

            Button btnConfirm = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // 확인 -> 그룹설정 화면으로 설정정보 이관
            btnConfirm.onClick.RemoveAllListeners();
            btnConfirm.onClick.AddListener(() => FinishHighGroupSetting(selectedImgPath)); // 확인 및 그룹설정으로 설정정보 이관 리스너
        }        
    }

    // 안드로이드 디바이스의 특정 경로에 저장된 이미지 파일을 불러옴
    private IEnumerator LoadImageCoroutine(string imagePath)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            Image fpImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // 평면도 이미지 파일 선택 버튼
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();

            if (imagePath == "this group have to img path define")
            {
                txtChooseFPImage.text = "이미지 선택";
            }
            else
            {
                txtChooseFPImage.text = "이미지 다시 선택";
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + imagePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                        fpImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                        // 이미지 Width와 Height 저장
                        f_selectedImgWidth = texture.width;
                        f_selectedImgHeight = texture.height;
                    }
                    else
                    {
                        //Debug.LogError("Image load failed. Error: " + uwr.error);
                    }
                }
            }
        }
        else
        {
            Image fpImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // 평면도 이미지 파일 선택 버튼
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();

            if (imagePath == "this group have to img path define")
            {
                txtChooseFPImage.text = "이미지 선택";
            }
            else
            {
                txtChooseFPImage.text = "이미지 다시 선택";
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + imagePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                        fpImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                        // 이미지 Width와 Height 저장
                        selectedImgWidth = texture.width;
                        selectedImgHeight = texture.height;
                    }
                    else
                    {
                        //Debug.LogError("Image load failed. Error: " + uwr.error);
                    }
                }
            }
        }           
    }

    // 안드로이드 디바이스의 특정 경로에 저장된 이미지 파일을 삭제
    public void DeleteImageFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log("GroupSettingManager : Delete FPImage following cancellation of high group settings: " + filePath);
        }
        else
        {
            //Debug.Log("GroupSettingManager : Attempted to delete FPImage due to cancellation of parent group setting, but failed to find image: " + filePath);
        }
    }

    public bool RenameImageFile(string originalFilePath, string newHGID)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // 원본 파일이 실제로 존재하는지 확인
            if (!File.Exists(originalFilePath))
            {
                //Debug.LogError("GroupSettingManager : Original FPImage does not exist: " + originalFilePath);
                return false;
            }

            // 파일 확장자 추출
            string fileExtension = Path.GetExtension(originalFilePath);

            // 새 파일명의 전체 경로를 확장자와 함께 구성
            string newFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath), newHGID + fileExtension);
            f_hgElement_ImgPath = newFilePath;

            try
            {
                // 대상 파일이 이미 존재할 경우 덮어쓰기 허용하여 파일 복사
                File.Copy(originalFilePath, newFilePath, true); // true 파라미터는 덮어쓰기를 허용함
                //Debug.Log("GroupSettingManager : File renamed from " + originalFilePath + " to " + newFilePath);

                // 원본 파일 삭제
                File.Delete(originalFilePath);

                // UPDATE 쿼리를 실행하여 FLD_IMG_PATH 경로를 업데이트
                string updateQuery = $"UPDATE TBL_HIGH_GROUP SET FLD_IMG_PATH = '{newFilePath.Replace("'", "''")}' WHERE FLD_HGID = '{newHGID}'";
                ClientDatabase.OnUpdateRequest(updateQuery);
                return true;
            }
            catch (IOException ex)
            {
                //Debug.LogError("GroupSettingManager : Error renaming file: " + ex.Message);
                return false;
            }
        }
        else
        {
            // 원본 파일이 실제로 존재하는지 확인
            if (!File.Exists(originalFilePath))
            {
                //Debug.LogError("GroupSettingManager : Original FPImage does not exist: " + originalFilePath);
                return false;
            }

            // 파일 확장자 추출
            string fileExtension = Path.GetExtension(originalFilePath);

            // 새 파일명의 전체 경로를 확장자와 함께 구성
            string newFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath), newHGID + fileExtension);
            hgElement_ImgPath = newFilePath;

            try
            {
                // 대상 파일이 이미 존재할 경우 덮어쓰기 허용하여 파일 복사
                File.Copy(originalFilePath, newFilePath, true); // true 파라미터는 덮어쓰기를 허용함
                //Debug.Log("GroupSettingManager : File renamed from " + originalFilePath + " to " + newFilePath);

                // 원본 파일 삭제
                File.Delete(originalFilePath);

                // UPDATE 쿼리를 실행하여 FLD_IMG_PATH 경로를 업데이트
                string updateQuery = $"UPDATE TBL_HIGH_GROUP SET FLD_IMG_PATH = '{newFilePath.Replace("'", "''")}' WHERE FLD_HGID = '{newHGID}'";
                ClientDatabase.OnUpdateRequest(updateQuery);
                return true;
            }
            catch (IOException ex)
            {
                //Debug.LogError("GroupSettingManager : Error renaming file: " + ex.Message);
                return false;
            }
        }        
    }
}