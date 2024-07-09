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
    public GameObject obj_SettingFilter; // ���� ���� �˾�â
    public GameObject filterListScrollViewContent; // ���� ���� ����� ��ũ�Ѻ� Content
    public GameObject floorScrollView; // floor ����Ʈ ��ũ�Ѻ�
    public GameObject groupScrollView; // group ����Ʈ ��ũ�Ѻ�
    public GameObject equipmentScrollView; // equipment ����Ʈ ��ũ�Ѻ�
    public Toggle floorViewAllToggle; // floor View All ���
    public Toggle groupViewAllToggle; // group View All ���
    public Toggle equipmentViewAllToggle; // equipment View All ���
    public Toggle RunStateToggle; // �������� ��Ʈ�ѷ� View ���
    public Toggle AlarmStateToggle; // �溸���� ��Ʈ�ѷ� View ���
        
    public GameObject filterList_ScrollView; // ���õ� ���� ��� ��� ��ũ�Ѻ�

    // �� ����Ʈ ��ư
    public GameObject btn_Floor;
    public GameObject btn_Group;
    public GameObject btn_Equipment;

    // �� ����Ʈ�� ��ũ�Ѻ� Content
    public static Transform floorContent;
    public static Transform groupContent;
    public static Transform equipmentContent;

    // ���� ���� �� ���õ� ���� ����Ʈ���� ����� ��ũ�Ѻ� Content
    public static Transform selectedFilterContent;

    // �� ����Ʈ DataSet
    private DataSet floorDataSet;
    private DataSet groupDataSet;
    private DataSet equipmentDataSet;    

    // ����Ʈ ���� ���� ����
    private bool isFloorListOpen = false;
    private bool isGroupListOpen = false;
    private bool isEquipmentListOpen = false;

    // �� ����Ʈ Ȱ��ȭ ��Ȱ��ȭ ǥ�� ���
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

    // ���� üũ�� ��۵��� �̸��� HashSet�� ����
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

    // ���� ��ư Ŭ�� �ڵ鷯
    public void ShowSettingFilter()
    {        
        LoadFilterAssets();
    }

    // ���� �˾� �ݱ� �ڵ鷯
    public void HideSettingFilter()
    {
        obj_SettingFilter.SetActive(false);
    }

    // �� ���� ��ҿ� toggle ����
    private void LoadFilterAssets()
    {
        // ���� ��ųʸ��� �����ͺ��̽��� �� ������ �� �� ����
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
                // ���� ��� ��Ұ� ������ ���븸 ������Ʈ
                TextMeshProUGUI floorName = floorToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                floorName.text = hgName;
            }
            else
            {
                // ���ο� ��� ��� ����
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
                // ���� ��� ��Ұ� ������ ���븸 ������Ʈ
                TextMeshProUGUI groupName = groupToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                groupName.text = lgName;                
            }
            else
            {
                // ���ο� ��� ��� ����
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
                // ���� ��� ��Ұ� ������ ���븸 ������Ʈ
                TextMeshProUGUI equipmentName = equipmentToggleInstance.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                equipmentName.text = cName;
            }
            else
            {
                // ���ο� ��� ��� ����
                GameObject newEquipmentToggle = ObjectPool.Instance.GetEquipmentFilterToggleObject();
                newEquipmentToggle.name = targetName;
                equipmentToggleInstances[targetName] = newEquipmentToggle;

                TextMeshProUGUI equipmentName = newEquipmentToggle.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
                equipmentName.text = cName;
            }
        }



        // StateToggle �ʱ�ȭ �� ���� ����
        if (!stateToggleInstances.ContainsKey(RunStateToggle.gameObject.name))
        {
            stateToggleInstances[RunStateToggle.gameObject.name] = RunStateToggle.gameObject;
        }
        else
        {
            // �̹� �����ϸ� ���� ���¸� ����
            RunStateToggle.isOn = stateToggleInstances[RunStateToggle.gameObject.name].GetComponent<Toggle>().isOn;
        }

        if (!stateToggleInstances.ContainsKey(AlarmStateToggle.gameObject.name))
        {
            stateToggleInstances[AlarmStateToggle.gameObject.name] = AlarmStateToggle.gameObject;
        }
        else
        {
            // �̹� �����ϸ� ���� ���¸� ����
            AlarmStateToggle.isOn = stateToggleInstances[AlarmStateToggle.gameObject.name].GetComponent<Toggle>().isOn;
        }




        LayoutRebuilder.ForceRebuildLayoutImmediate(filterListScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        obj_SettingFilter.SetActive(true);
    }

    // ����Ʈ ����, ���� ���� ��ȯ
    private void SetButtonActive(GameObject activeButton, GameObject deactiveButton)
    {
        activeButton.SetActive(true);
        deactiveButton.SetActive(false);
    }

    // Floor ����Ʈ ���� �ݱ� ��ư �ڵ鷯
    public void FloorList()
    {
        if (!isFloorListOpen)
        {
            // ��� ����
            SetButtonActive(floorActive, floorDeactive);
            floorScrollView.SetActive(true);
        }
        else
        {
            // ��� ����
            SetButtonActive(floorDeactive, floorActive);
            floorScrollView.SetActive(false);
        }
        isFloorListOpen = !isFloorListOpen;
        ScreenUpdate();
    }

    // Floor View All ��� üũ
    public void FloorViewAll(bool isOn)
    {
        isFloorListOpen = true;
        SetButtonActive(floorActive, floorDeactive);
        floorScrollView.SetActive(true);

        isOn = floorViewAllToggle.isOn;

        // ��� floorToggleInstance�� üũ ���¸� ����
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

    // Group ����Ʈ ���� �ݱ� ��ư �ڵ鷯
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

    // Group View All ��� üũ
    public void GroupViewAll(bool isOn)
    {
        isGroupListOpen = true;
        SetButtonActive(groupActive, groupDeactive);
        groupScrollView.SetActive(true);

        isOn = groupViewAllToggle.isOn;

        // ��� groupToggleInstances�� üũ ���¸� ����
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

    // Equipment ����Ʈ ���� �ݱ� ��ư �ڵ鷯
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

    // Equipment View All ��� üũ
    public void EquipmentViewAll(bool isOn)
    {
        isEquipmentListOpen = true;
        SetButtonActive(equipmentActive, equipmentDeactive);
        equipmentScrollView.SetActive(true);

        isOn = equipmentViewAllToggle.isOn;

        // ��� equipmentToggleInstances�� üũ ���¸� ����
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

    // �ʱ�ȭ ��ư�� ������ ��� üũ �׸��� isOn false ��
    public void ResetAllCheck()
    {
        floorViewAllToggle.isOn = false;
        groupViewAllToggle.isOn = false;
        equipmentViewAllToggle.isOn = false;

        FloorViewAll(false);
        GroupViewAll(false);
        EquipmentViewAll(false);
    }

    // ��� �ν��Ͻ� ������Ʈ
    private void UpdateToggleInstances(Dictionary<string, GameObject> toggleInstances)
    {
        // ���� ��ųʸ��� �ִ� Ű�� �����Ͽ� �� ��� ����
        HashSet<string> existingKeys = new HashSet<string>(toggleInstances.Keys);               

        // �� �̻� �������� �ʴ� �ν��Ͻ� ��Ȱ��ȭ
        foreach (var key in existingKeys)
        {
            toggleInstances[key].SetActive(false);            
            toggleInstances.Remove(key);
        }
    }

    // ���� ��ũ�Ѻ� Ȱ�� ��Ȱ�� �� Content Rect Transform ������Ʈ
    private void ScreenUpdate()
    {
        obj_SettingFilter.SetActive(false);
        obj_SettingFilter.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(filterListScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // ���� ����
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

            // Grid �ν��Ͻ� ó��
            if (ClientDatabase.controllerGridInstances.TryGetValue(controllerKey, out var gridControllerObject))
            {
                gridControllerObject.SetActive(shouldActivate);
            }

            // List �ν��Ͻ� ó��
            if (ClientDatabase.controllerListInstances.TryGetValue(controllerKey, out var listControllerObject))
            {
                listControllerObject.SetActive(shouldActivate);
            }
        }

        // ����ȭ�� ���� ��ܿ� ���õ� ���� ����Ʈ���� ������
        HideSettingFilter();        
        filterList_ScrollView.SetActive(true);
        isUpdatingFilters = false;
    }

    // ������ ���Ϳ� ���� Ȱ�� ��Ȱ�� ���� ��ȯ
    private bool DetermineActivationBasedOnFilters(string hgid, string lgid, string id, string cid)
    {
        isUpdatingFilters = true;

        // ���� ���õ� ���� �׸���� �̸� ��������
        var checkedFloorToggles = GetCheckedToggleInfo(floorToggleInstances, "FloorToggle_");
        var checkedGroupToggles = GetCheckedToggleInfo(groupToggleInstances, "GroupToggle_");
        var checkedEquipmentToggles = GetCheckedToggleInfo(equipmentToggleInstances, "EquipmentToggle_");
        var checkedStateToggles = GetCheckedToggleInfo(stateToggleInstances, "StateToggle_");

        // ���� �̼��õ� ���� �׸���� �̸� ��������
        var uncheckedFloorToggles = GetUncheckedToggleInfo(floorToggleInstances, "FloorToggle_");
        var uncheckedGroupToggles = GetUncheckedToggleInfo(groupToggleInstances, "GroupToggle_");
        var uncheckedEquipmentToggles = GetUncheckedToggleInfo(equipmentToggleInstances, "EquipmentToggle_");
        var uncheckedStateToggles = GetUncheckedToggleInfo(stateToggleInstances, "StateToggle_");

        // üũ ������ �׸���� ����ȭ�� ���� ����Ʈ ����
        HideUncheckedFilters(uncheckedFloorToggles);
        HideUncheckedFilters(uncheckedGroupToggles);
        HideUncheckedFilters(uncheckedEquipmentToggles);
        HideUncheckedFilters(uncheckedStateToggles);

        // üũ�� ���� �׸���� ����ȭ�� ���� ����Ʈ ǥ��
        ShowCheckedFilters(checkedFloorToggles);
        ShowCheckedFilters(checkedGroupToggles);
        ShowCheckedFilters(checkedEquipmentToggles);
        ShowCheckedFilters(checkedStateToggles);

        List<string> checkedFloorIds = GetCheckedToggleIds(floorToggleInstances, "FloorToggle_");
        List<string> checkedGroupIds = GetCheckedToggleIds(groupToggleInstances, "GroupToggle_");
        List<string> checkedEquipmentIds = GetCheckedToggleIds(equipmentToggleInstances, "EquipmentToggle_");

        // üũ�� ����� ������ ��� ���� ������ ������ ����
        bool allFloorsSelected = checkedFloorIds.Count == 0;
        bool allGroupsSelected = checkedGroupIds.Count == 0;
        bool allEquipmentsSelected = checkedEquipmentIds.Count == 0;

        // State ��� ���� Ȯ��
        bool runStateSelected = RunStateToggle.isOn;
        bool alarmStateSelected = AlarmStateToggle.isOn;

        
        DataSet realTimeDataSet = ClientDatabase.realTimeData;

        // realTimeData���� �ش� ��Ʈ�ѷ��� ���� ã��
        DataRow realTimeRow = realTimeDataSet.Tables[0].Select($"ID = '{id}' AND CID = '{cid}'").FirstOrDefault();

        // State ���� Ȯ��
        bool runStateCheck = !runStateSelected || (runStateSelected && Convert.ToInt32(realTimeRow?["RUN"]) == 1);
        bool alarmStateCheck = !alarmStateSelected || (alarmStateSelected && Convert.ToInt32(realTimeRow?["ALARM"]) == 1);

        // ���ǿ� ���� ��Ʈ�ѷ� Ȱ��ȭ/��Ȱ��ȭ
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

    // üũ�� ��۵��� Ȯ��
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

    // üũ�� ����� ID�� �̸��� �������� �Լ�
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
                checkedInfo[id] = textComponent.text; // ID�� �ؽ�Ʈ ��� ����
            }
        }
        return checkedInfo;
    }

    // üũ ������ ����� ID�� �̸��� �������� �Լ�
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
                checkedInfo[id] = textComponent.text; // ID�� �ؽ�Ʈ ��� ����
            }
        }
        return checkedInfo;
    }

    // üũ ������ ����� ���ӿ�����Ʈ�� �������� �Լ�
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
                uncheckedToggles[id] = toggleObject; // ID�� GameObject ����
            }
        }
        return uncheckedToggles;
    }


    // ����ȭ�� �����׸� ������ư Ŭ�� �� ��Ʈ�ѷ� �ν��Ͻ� ������
    public void UpdateFiltersAndRefreshControllers()
    {
        // ���� ��� ������Ʈ ����
        UpdateFilterList();

        // ��Ʈ�ѷ� �ν��Ͻ� ������Ʈ ����
        //RefreshControllerInstances();

        ApplyFilterAndSave();
    }

    // ���� ��� ������Ʈ ����
    private void UpdateFilterList()
    {
        // Floor ��� ���� ������Ʈ
        foreach (var entry in floorToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // ���� ���� ��Ͽ� �����ϴ��� Ȯ��
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }

        // Group ��� ���� ������Ʈ
        foreach (var entry in groupToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // ���� ���� ��Ͽ� �����ϴ��� Ȯ��
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }

        // Equipment ��� ���� ������Ʈ
        foreach (var entry in equipmentToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // ���� ���� ��Ͽ� �����ϴ��� Ȯ��
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }

        // state ��� ���� ������Ʈ
        foreach (var entry in stateToggleInstances)
        {
            string toggleName = entry.Key;
            GameObject toggleObject = entry.Value;
            TextMeshProUGUI textComponent = toggleObject.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            Toggle toggle = toggleObject.GetComponent<Toggle>();

            // ���� ���� ��Ͽ� �����ϴ��� Ȯ��
            toggle.isOn = currentCheckedNames.Contains(textComponent.text);
        }
    }

    
}