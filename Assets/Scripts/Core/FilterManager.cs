using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FilterManager : MonoBehaviour
{
    public GameObject obj_SettingFilter; // 필터 설정 팝업창
    public GameObject filterListScrollViewContent; // 필터 설정 대단위 스크롤뷰 Content
    public GameObject floorScrollView; // floor 리스트 스크롤뷰
    public GameObject groupScrollView; // group 리스트 스크롤뷰
    public GameObject equipmentScrollView; // equipment 리스트 스크롤뷰
    public Toggle floorViewAllToggle; // floor View All 토글
    public Toggle groupViewAllToggle; // group View All 토글
    public Toggle equipmentViewAllToggle; // equipment View All 토글
    public Toggle RunStateToggle; // 실행중인 컨트롤러 View 토글
    public Toggle AlarmStateToggle; // 경보중인 컨트롤러 View 토글
        
    public GameObject filterList_ScrollView; // 선택된 필터 목록 담길 스크롤뷰

    // 각 리스트 버튼
    public GameObject btn_Floor;
    public GameObject btn_Group;
    public GameObject btn_Equipment;

    // 각 리스트의 스크롤뷰 Content
    public static Transform floorContent;
    public static Transform groupContent;
    public static Transform equipmentContent;

    // 필터 저장 시 선택된 필터 리스트들이 저장될 스크롤뷰 Content
    public static Transform selectedFilterContent;

    // 각 리스트 DataSet
    private DataSet floorDataSet;
    private DataSet groupDataSet;
    private DataSet equipmentDataSet;    

    // 리스트 열림 닫힘 상태
    private bool isFloorListOpen = false;
    private bool isGroupListOpen = false;
    private bool isEquipmentListOpen = false;

    // 각 리스트 활성화 비활성화 표시 요소
    private GameObject floorActive;
    private GameObject floorDeactive;
    private GameObject groupActive;
    private GameObject groupDeactive;
    private GameObject equipmentActive;
    private GameObject equipmentDeactive;

    public bool isUpdatingFilters = false;

    public static Dictionary<string, GameObject> floorToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> groupToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> equipmentToggleInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> stateToggleInstances = new Dictionary<string, GameObject>();

    // 현재 체크된 토글들의 이름을 HashSet에 저장
    public HashSet<string> currentCheckedNames = new HashSet<string>();

    public static FilterManager Instance { get; private set; }


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

        floorContent = floorScrollView.transform.Find("Viewport/Content");
        groupContent = groupScrollView.transform.Find("Viewport/Content");
        equipmentContent = equipmentScrollView.transform.Find("Viewport/Content");
        selectedFilterContent = filterList_ScrollView.transform.Find("Viewport/Content");

        floorActive = btn_Floor.transform.Find("Active").gameObject;
        floorDeactive = btn_Floor.transform.Find("Deactive").gameObject;
        groupActive = btn_Group.transform.Find("Active").gameObject;
        groupDeactive = btn_Group.transform.Find("Deactive").gameObject;
        equipmentActive = btn_Equipment.transform.Find("Active").gameObject;
        equipmentDeactive = btn_Equipment.transform.Find("Deactive").gameObject;
    }

    // 필터 버튼 클릭 핸들러
    public void ShowSettingFilter()
    {        
        LoadFilterAssets();
    }

    // 필터 팝업 닫기 핸들러
    public void HideSettingFilter()
    {
        obj_SettingFilter.SetActive(false);
    }

    // 각 필터 요소에 toggle 생성
    private void LoadFilterAssets()
    {
        // 기존 딕셔너리와 데이터베이스의 새 데이터 비교 및 갱신
        if (floorToggleInstances.Count != 0 && groupToggleInstances.Count != 0 && equipmentToggleInstances.Count != 0 && stateToggleInstances.Count != 0)
        {
            UpdateToggleInstances(floorToggleInstances);
            UpdateToggleInstances(groupToggleInstances);
            UpdateToggleInstances(equipmentToggleInstances);            

            UpdateFilterList();
        }

        isFloorListOpen = false;
        isGroupListOpen = false;
        isEquipmentListOpen = false;

        floorDataSet = ClientDatabase.highGroupData;
        groupDataSet = ClientDatabase.lowGroupData;
        equipmentDataSet = ClientDatabase.controllerData;        

        DataTable floorTable = floorDataSet.Tables[0];
        DataTable groupTable = groupDataSet.Tables[0];
        DataTable equipmentTable = equipmentDataSet.Tables[0];        

        foreach (DataRow floorRow in floorTable.Rows)
        {
            string hgid = floorRow["FLD_HGID"].ToString(); 
            string hgName = floorRow["FLD_NAME"].ToString();
            string targetName = $"FloorToggle_{hgid}";

            if (floorToggleInstances.TryGetValue(targetName, out var floorToggleInstance))
            {
                // 기존 토글 요소가 있으면 내용만 업데이트
                TextMeshProUGUI floorName = floorToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                floorName.text = hgName;
            }
            else
            {
                // 새로운 토글 요소 생성
                GameObject newFloorToggle = ObjectPool.Instance.GetFloorFilterToggleObject();
                newFloorToggle.name = targetName;
                floorToggleInstances[targetName] = newFloorToggle;

                TextMeshProUGUI floorName = newFloorToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                floorName.text = hgName;
            }
        }

        foreach (DataRow groupRow in groupTable.Rows)
        {
            string hgid = groupRow["FLD_HGID"].ToString();
            string lgid = groupRow["FLD_LGID"].ToString();
            string lgName = groupRow["FLD_NAME"].ToString();
            string targetName = $"GroupToggle_{hgid}_{lgid}";

            if (groupToggleInstances.TryGetValue(targetName, out var groupToggleInstance))
            {
                // 기존 토글 요소가 있으면 내용만 업데이트
                TextMeshProUGUI groupName = groupToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                groupName.text = lgName;                
            }
            else
            {
                // 새로운 토글 요소 생성
                GameObject newGroupToggle = ObjectPool.Instance.GetGroupFilterToggleObject();
                newGroupToggle.name = targetName;
                groupToggleInstances[targetName] = newGroupToggle;

                TextMeshProUGUI groupName = newGroupToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                groupName.text = lgName;
            }
        }

        foreach (DataRow equipmentRow in equipmentTable.Rows)
        {
            string iid = equipmentRow["ID"].ToString();
            string cid = equipmentRow["CID"].ToString();
            string cName = equipmentRow["CNAME"].ToString();
            string targetName = $"EquipmentToggle_{iid}_{cid}";

            if (equipmentToggleInstances.TryGetValue(targetName, out var equipmentToggleInstance))
            {
                // 기존 토글 요소가 있으면 내용만 업데이트
                TextMeshProUGUI equipmentName = equipmentToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                equipmentName.text = cName;
            }
            else
            {
                // 새로운 토글 요소 생성
                GameObject newEquipmentToggle = ObjectPool.Instance.GetEquipmentFilterToggleObject();
                newEquipmentToggle.name = targetName;
                equipmentToggleInstances[targetName] = newEquipmentToggle;

                TextMeshProUGUI equipmentName = newEquipmentToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                equipmentName.text = cName;
            }
        }



        // StateToggle 초기화 및 상태 유지
        if (!stateToggleInstances.ContainsKey(RunStateToggle.gameObject.name))
        {
            stateToggleInstances[RunStateToggle.gameObject.name] = RunStateToggle.gameObject;
        }
        else
        {
            // 이미 존재하면 기존 상태를 유지
            RunStateToggle.isOn = stateToggleInstances[RunStateToggle.gameObject.name].GetComponent<Toggle>().isOn;
        }

        if (!stateToggleInstances.ContainsKey(AlarmStateToggle.gameObject.name))
        {
            stateToggleInstances[AlarmStateToggle.gameObject.name] = AlarmStateToggle.gameObject;
        }
        else
        {
            // 이미 존재하면 기존 상태를 유지
            AlarmStateToggle.isOn = stateToggleInstances[AlarmStateToggle.gameObject.name].GetComponent<Toggle>().isOn;
        }




        LayoutRebuilder.ForceRebuildLayoutImmediate(filterListScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        obj_SettingFilter.SetActive(true);
    }

    // 리스트 열림, 닫힘 상태 전환
    private void SetButtonActive(GameObject activeButton, GameObject deactiveButton)
    {
        activeButton.SetActive(true);
        deactiveButton.SetActive(false);
    }

    // Floor 리스트 열고 닫기 버튼 핸들러
    public void FloorList()
    {
        if (!isFloorListOpen)
        {
            // 목록 열림
            SetButtonActive(floorActive, floorDeactive);
            floorScrollView.SetActive(true);
        }
        else
        {
            // 목록 닫힘
            SetButtonActive(floorDeactive, floorActive);
            floorScrollView.SetActive(false);
        }
        isFloorListOpen = !isFloorListOpen;
        ScreenUpdate();
    }

    // Floor View All 토글 체크
    public void FloorViewAll(bool isOn)
    {
        isFloorListOpen = true;
        SetButtonActive(floorActive, floorDeactive);
        floorScrollView.SetActive(true);

        isOn = floorViewAllToggle.isOn;

        // 모든 floorToggleInstance의 체크 상태를 설정
        foreach (var entry in floorToggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = isOn;
            }
        }

        ScreenUpdate();
    }

    // Group 리스트 열고 닫기 버튼 핸들러
    public void GroupList()
    {
        if (!isGroupListOpen)
        {
            SetButtonActive(groupActive, groupDeactive);
            groupScrollView.SetActive(true);
        }
        else
        {
            SetButtonActive(groupDeactive, groupActive);
            groupScrollView.SetActive(false);
        }
        isGroupListOpen = !isGroupListOpen;
        ScreenUpdate();
    }

    // Group View All 토글 체크
    public void GroupViewAll(bool isOn)
    {
        isGroupListOpen = true;
        SetButtonActive(groupActive, groupDeactive);
        groupScrollView.SetActive(true);

        isOn = groupViewAllToggle.isOn;

        // 모든 groupToggleInstances의 체크 상태를 설정
        foreach (var entry in groupToggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = isOn;
            }
        }

        ScreenUpdate();
    }

    // Equipment 리스트 열고 닫기 버튼 핸들러
    public void EquipmentList()
    {
        if (!isEquipmentListOpen)
        {
            SetButtonActive(equipmentActive, equipmentDeactive);
            equipmentScrollView.SetActive(true);
        }
        else
        {
            SetButtonActive(equipmentDeactive, equipmentActive);
            equipmentScrollView.SetActive(false);
        }
        isEquipmentListOpen = !isEquipmentListOpen;
        ScreenUpdate();
    }

    // Equipment View All 토글 체크
    public void EquipmentViewAll(bool isOn)
    {
        isEquipmentListOpen = true;
        SetButtonActive(equipmentActive, equipmentDeactive);
        equipmentScrollView.SetActive(true);

        isOn = equipmentViewAllToggle.isOn;

        // 모든 equipmentToggleInstances의 체크 상태를 설정
        foreach (var entry in equipmentToggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null)
            {
                toggle.isOn = isOn;
            }
        }

        ScreenUpdate();
    }

    // 초기화 버튼을 누르면 모든 체크 항목이 isOn false 됨
    public void ResetAllCheck()
    {
        floorViewAllToggle.isOn = false;
        groupViewAllToggle.isOn = false;
        equipmentViewAllToggle.isOn = false;

        FloorViewAll(false);
        GroupViewAll(false);
        EquipmentViewAll(false);
    }

    // 토글 인스턴스 업데이트
    private void UpdateToggleInstances(Dictionary<string, GameObject> toggleInstances)
    {
        // 현재 딕셔너리에 있는 키를 복사하여 비교 목록 생성
        HashSet<string> existingKeys = new HashSet<string>(toggleInstances.Keys);               

        // 더 이상 존재하지 않는 인스턴스 비활성화
        foreach (var key in existingKeys)
        {
            toggleInstances[key].SetActive(false);            
            toggleInstances.Remove(key);
        }
    }

    // 하위 스크롤뷰 활성 비활성 시 Content Rect Transform 업데이트
    private void ScreenUpdate()
    {
        obj_SettingFilter.SetActive(false);
        obj_SettingFilter.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(filterListScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // 필터 저장
    public void ApplyFilterAndSave()
    {
        DataSet controllerDataSet = ClientDatabase.controllerData;

        foreach (DataRow row in controllerDataSet.Tables[0].Rows)
        {
            string hgid = row["HGID"].ToString();
            string lgid = row["LGID"].ToString();
            string id = row["ID"].ToString();
            string cid = row["CID"].ToString();
            string controllerKey = $"Controller_{id}_{cid}";

            bool shouldActivate = DetermineActivationBasedOnFilters(hgid, lgid, id, cid);

            // Grid 인스턴스 처리
            if (ClientDatabase.controllerGridInstances.TryGetValue(controllerKey, out var gridControllerObject))
            {
                gridControllerObject.SetActive(shouldActivate);
            }

            // List 인스턴스 처리
            if (ClientDatabase.controllerListInstances.TryGetValue(controllerKey, out var listControllerObject))
            {
                listControllerObject.SetActive(shouldActivate);
            }
        }

        // 메인화면 좌측 상단에 선택된 필터 리스트들이 생성됨
        HideSettingFilter();        
        filterList_ScrollView.SetActive(true);
        isUpdatingFilters = false;
    }

    // 선택한 필터에 따른 활성 비활성 여부 반환
    private bool DetermineActivationBasedOnFilters(string hgid, string lgid, string id, string cid)
    {
        isUpdatingFilters = true;

        // 현재 선택된 필터 항목들의 이름 가져오기
        var checkedFloorToggles = GetCheckedToggleInfo(floorToggleInstances, "FloorToggle_");
        var checkedGroupToggles = GetCheckedToggleInfo(groupToggleInstances, "GroupToggle_");
        var checkedEquipmentToggles = GetCheckedToggleInfo(equipmentToggleInstances, "EquipmentToggle_");
        var checkedStateToggles = GetCheckedToggleInfo(stateToggleInstances, "StateToggle_");

        // 현재 미선택된 필터 항목들의 이름 가져오기
        var uncheckedFloorToggles = GetUncheckedToggleInfo(floorToggleInstances, "FloorToggle_");
        var uncheckedGroupToggles = GetUncheckedToggleInfo(groupToggleInstances, "GroupToggle_");
        var uncheckedEquipmentToggles = GetUncheckedToggleInfo(equipmentToggleInstances, "EquipmentToggle_");
        var uncheckedStateToggles = GetUncheckedToggleInfo(stateToggleInstances, "StateToggle_");

        // 체크 해제된 항목들의 메인화면 필터 리스트 숨김
        HideUncheckedFilters(uncheckedFloorToggles);
        HideUncheckedFilters(uncheckedGroupToggles);
        HideUncheckedFilters(uncheckedEquipmentToggles);
        HideUncheckedFilters(uncheckedStateToggles);

        // 체크된 필터 항목들의 메인화면 필터 리스트 표시
        ShowCheckedFilters(checkedFloorToggles);
        ShowCheckedFilters(checkedGroupToggles);
        ShowCheckedFilters(checkedEquipmentToggles);
        ShowCheckedFilters(checkedStateToggles);

        List<string> checkedFloorIds = GetCheckedToggleIds(floorToggleInstances, "FloorToggle_");
        List<string> checkedGroupIds = GetCheckedToggleIds(groupToggleInstances, "GroupToggle_");
        List<string> checkedEquipmentIds = GetCheckedToggleIds(equipmentToggleInstances, "EquipmentToggle_");

        // 체크된 토글이 없으면 모든 값을 선택한 것으로 간주
        bool allFloorsSelected = checkedFloorIds.Count == 0;
        bool allGroupsSelected = checkedGroupIds.Count == 0;
        bool allEquipmentsSelected = checkedEquipmentIds.Count == 0;

        // State 토글 상태 확인
        bool runStateSelected = RunStateToggle.isOn;
        bool alarmStateSelected = AlarmStateToggle.isOn;

        
        DataSet realTimeDataSet = ClientDatabase.realTimeData;

        // realTimeData에서 해당 컨트롤러의 상태 찾기
        DataRow realTimeRow = realTimeDataSet.Tables[0].Select($"ID = '{id}' AND CID = '{cid}'").FirstOrDefault();

        // State 조건 확인
        bool runStateCheck = !runStateSelected || (runStateSelected && Convert.ToInt32(realTimeRow?["RUN"]) == 1);
        bool alarmStateCheck = !alarmStateSelected || (alarmStateSelected && Convert.ToInt32(realTimeRow?["ALARM"]) == 1);

        // 조건에 따라 컨트롤러 활성화/비활성화
        bool floorCheck = allFloorsSelected || checkedFloorIds.Contains(hgid);
        bool groupCheck = allGroupsSelected || checkedGroupIds.Contains($"{hgid}_{lgid}");
        bool equipmentCheck = allEquipmentsSelected || checkedEquipmentIds.Contains($"{id}_{cid}");

        return floorCheck && groupCheck && equipmentCheck && runStateCheck && alarmStateCheck;
    }

    private void HideUncheckedFilters(Dictionary<string, string> uncheckedToggles)
    {
        foreach (var info in uncheckedToggles)
        {
            currentCheckedNames.Remove(info.Value);
            var filterObject = GameObject.Find($"Filter_Selected_{info.Key}_{info.Value}");
            if (filterObject != null)
            {
                //Debug.Log($"remove {filterObject.name}");
                ObjectPool.Instance.RemoveFilter(info.Value, filterObject);
            }
        }
    }

    private void ShowCheckedFilters(Dictionary<string, string> checkedToggles)
    {
        foreach (var info in checkedToggles)
        {
            var filterObject = ObjectPool.Instance.GetSelectedFilterObject(info.Key, info.Value);
            if (filterObject != null)
            {
                var textComponent = filterObject.transform.Find("txt_Filter").GetComponent<TextMeshProUGUI>();
                textComponent.text = info.Value;
            }
        }
    }

    // 체크된 토글들을 확인
    private List<string> GetCheckedToggleIds(Dictionary<string, GameObject> toggleInstances, string prefix)
    {
        List<string> checkedIds = new List<string>();
        foreach (var entry in toggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                string id = entry.Key.Replace(prefix, "");
                checkedIds.Add(id);
            }
        }
        return checkedIds;
    }

    // 체크된 토글의 ID와 이름을 가져오는 함수
    private Dictionary<string, string> GetCheckedToggleInfo(Dictionary<string, GameObject> toggleInstances, string prefix)
    {
        Dictionary<string, string> checkedInfo = new Dictionary<string, string>();
        foreach (var entry in toggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                string id = entry.Key.Replace(prefix, "");
                TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                checkedInfo[id] = textComponent.text; // ID와 텍스트 모두 저장
            }
        }
        return checkedInfo;
    }

    // 체크 해제된 토글의 ID와 이름을 가져오는 함수
    private Dictionary<string, string> GetUncheckedToggleInfo(Dictionary<string, GameObject> toggleInstances, string prefix)
    {
        Dictionary<string, string> checkedInfo = new Dictionary<string, string>();
        foreach (var entry in toggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null && !toggle.isOn)
            {
                string id = entry.Key.Replace(prefix, "");
                TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                checkedInfo[id] = textComponent.text; // ID와 텍스트 모두 저장
            }
        }
        return checkedInfo;
    }

    // 체크 해제된 토글의 게임오브젝트를 가져오는 함수
    private Dictionary<string, GameObject> GetUncheckedToggleGameObject(Dictionary<string, GameObject> toggleInstances, string prefix)
    {
        Dictionary<string, GameObject> uncheckedToggles = new Dictionary<string, GameObject>();
        foreach (var entry in toggleInstances)
        {
            GameObject toggleObject = entry.Value;
            Toggle toggle = toggleObject.GetComponent<Toggle>();
            if (toggle != null && !toggle.isOn)
            {
                string id = entry.Key.Replace(prefix, "");
                uncheckedToggles[id] = toggleObject; // ID와 GameObject 저장
            }
        }
        return uncheckedToggles;
    }


    // 메인화면 필터항목 삭제버튼 클릭 시 컨트롤러 인스턴스 재정렬
    public void UpdateFiltersAndRefreshControllers()
    {
        // 필터 목록 업데이트 로직
        UpdateFilterList();

        // 컨트롤러 인스턴스 업데이트 로직
        //RefreshControllerInstances();

        ApplyFilterAndSave();
    }

    // 필터 목록 업데이트 로직
    private void UpdateFilterList()
    {
        // Floor 토글 상태 업데이트
        foreach (var entry in floorToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // 현재 필터 목록에 존재하는지 확인
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }

        // Group 토글 상태 업데이트
        foreach (var entry in groupToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // 현재 필터 목록에 존재하는지 확인
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }

        // Equipment 토글 상태 업데이트
        foreach (var entry in equipmentToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // 현재 필터 목록에 존재하는지 확인
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }

        // state 토글 상태 업데이트
        foreach (var entry in stateToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // 현재 필터 목록에 존재하는지 확인
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }
    }

    
}