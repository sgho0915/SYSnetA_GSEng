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
    // ���� ù ���� �� ���
    private Dictionary<string, GameObject> f_uiElements = new Dictionary<string, GameObject>();
    [Header("ù ���� �׷� �������")]
    public GameObject f_settingGroup;
    public GameObject f_groupListScrollView; // �׷� ����Ʈ ��ũ�Ѻ�
    public static Transform f_groupListContent; // �׷� ����Ʈ ��ũ�Ѻ� Content
    public static Transform f_hgContainerTransform; // Container_HighGroup�� Transform
    public static Transform f_lgContainerTransform; // Container_LowGroup�� Transform
    private Button f_btnClose;
    private Button f_btnAddHighGroup;
    private Button f_btnSave;

    [Header("ù ���� �����׷� �������")]
    public GameObject f_settingHighGroup; // �����׷� ����ȭ��
    private TMP_InputField f_inputFieldHGName; // �����׷�� ���� �� ���� ��ǲ�ʵ�

    [Header("ù ���� ������ ������� ������ ���� ���ڿ� �迭")]
    public string[] f_arrSettingQuery;
    public string f_hgElement_Id = string.Empty;
    public string f_hgElement_Name = string.Empty;
    public string f_hgElement_ImgPath = string.Empty;
    public string f_selectedImgPath = string.Empty;

    private int f_selectedImgWidth;
    private int f_selectedImgHeight;

    // ���� ù ���� ���� ��� �� ���
    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();
    [Header("�׷� �������")]
    public GameObject settingGroup;
    public GameObject groupListScrollView; // �׷� ����Ʈ ��ũ�Ѻ�
    public static Transform groupListContent; // �׷� ����Ʈ ��ũ�Ѻ� Content
    public static Transform hgContainerTransform; // Container_HighGroup�� Transform
    public static Transform lgContainerTransform; // Container_LowGroup�� Transform
    private Button btnClose;
    private Button btnAddHighGroup;
    private Button btnSave;

    [Header("�����׷� �������")]
    public GameObject settingHighGroup; // �����׷� ����ȭ��
    private TMP_InputField inputFieldHGName; // �����׷�� ���� �� ���� ��ǲ�ʵ�

    [Header("������ ������� ������ ���� ���ڿ� �迭")]
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

        // ù ���� ��
        f_groupListContent = f_groupListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();
        f_btnAddHighGroup = f_settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/AddGroup/btn_AddGroup").GetComponent<Button>();
        f_btnAddHighGroup.onClick.AddListener(() => AddHighGroupContainer());
        f_btnClose = f_settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Close").GetComponent<Button>();
        f_btnClose.onClick.AddListener(() => CloseGroupSetting());
        f_btnSave = f_settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Save").GetComponent<Button>();
        f_btnSave.onClick.AddListener(() => {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = "�׷� ������ �Ϸ� �Ǿ����ϴ�.";
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

        // ù ���� ����
        groupListContent = groupListScrollView.transform.Find("Viewport/Content").GetComponent<Transform>();
        btnAddHighGroup = settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/AddGroup/btn_AddGroup").GetComponent<Button>();
        btnAddHighGroup.onClick.AddListener(() => AddHighGroupContainer());
        btnClose = settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(() => CloseGroupSetting());
        btnSave = settingGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Save").GetComponent<Button>();
        btnSave.onClick.AddListener(() => {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = "�׷� ������ �Ϸ� �Ǿ����ϴ�.";
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

    // ���� �׷� �����̳� �߰�
    private void AddHighGroupContainer()
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // �����׷쿡 ���� �̹� �����ϴ� �����׷� ID�� �ִ밪 +1�� ���� ���ο� �����׷� INSERT
            DataTable hgTableMaxId = ClientDatabase.FetchHighGroupData().Tables[0];
            var maxHgIdRow = hgTableMaxId.AsEnumerable()
                .Where(r => r.Field<string>("FLD_IMG_PATH") == "this group have to img path define")
                .Select(r => r.Field<short>("FLD_HGID"))
                .DefaultIfEmpty((short)0) // �⺻�� ������ ���� ���� �׷��� ���� ��츦 ó��
                .Max();
            int newHgId = maxHgIdRow + 1; // ���ο� FLD_LGID ���� ����

            // �����׷� ��� ����        
            string query = $"INSERT INTO TBL_HIGH_GROUP (FLD_NAME, FLD_IMG_PATH, FLD_IMG_WIDTH, FLD_IMG_HEIGHT) VALUES ('�� �����׷�{newHgId}', 'this group have to img path define', '0', '0')";
            ClientDatabase.OnInsertRequest(query);

            // ���Ӱ� �߰��� ���� FLD_HGID ������ ����
            DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
            var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"�� �����׷�{newHgId}" && r.Field<string>("FLD_IMG_PATH") == "this group have to img path define");

            // �׷� ���� ȭ�鿡 ���ο� �����׷� �����̳� �ν��Ͻ� ����
            GameObject newhighGroupContainer = ObjectPool.Instance.f_GetGroupSettingHGContainerObject(); // ���ο� �����׷� �����̳�
            string hgContainerName = $"Container_HighGroup_{row["FLD_HGID"].ToString()}";
            newhighGroupContainer.name = hgContainerName;

            GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // �����̳� �� �����׷� ��ü        
            Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // �����׷� ���� ��ư
            TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // �����׷��
            Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // �����׷� ���� ��ư
            Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // �����׷� �߰� ��ư
            f_uiElements[hgContainerName] = newhighGroupContainer;

            f_hgContainerTransform = newhighGroupContainer.transform;
            Transform hgTrs = f_hgContainerTransform;

            newObjHighGroup.name = $"HighGroup_{row["FLD_HGID"].ToString()}";
            newTxtHighGroupName.text = row["FLD_NAME"].ToString();
            AddHighGroupButtonListener(newObjHighGroup, hgTrs, row["FLD_HGID"].ToString(), row["FLD_NAME"].ToString(), row["FLD_IMG_PATH"].ToString(), row["FLD_IMG_WIDTH"].ToString(), row["FLD_IMG_HEIGHT"].ToString()); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)


            LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(f_hgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            // �����׷쿡 ���� �̹� �����ϴ� �����׷� ID�� �ִ밪 +1�� ���� ���ο� �����׷� INSERT
            DataTable hgTableMaxId = ClientDatabase.FetchHighGroupData().Tables[0];
            var maxHgIdRow = hgTableMaxId.AsEnumerable()
                .Where(r => r.Field<string>("FLD_IMG_PATH") == "this group have to img path define")
                .Select(r => r.Field<short>("FLD_HGID"))
                .DefaultIfEmpty((short)0) // �⺻�� ������ ���� ���� �׷��� ���� ��츦 ó��
                .Max();
            int newHgId = maxHgIdRow + 1; // ���ο� FLD_LGID ���� ����

            // �����׷� ��� ����        
            string query = $"INSERT INTO TBL_HIGH_GROUP (FLD_NAME, FLD_IMG_PATH, FLD_IMG_WIDTH, FLD_IMG_HEIGHT) VALUES ('�� �����׷�{newHgId}', 'this group have to img path define', '0', '0')";
            ClientDatabase.OnInsertRequest(query);

            // ���Ӱ� �߰��� ���� FLD_HGID ������ ����
            DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
            var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"�� �����׷�{newHgId}" && r.Field<string>("FLD_IMG_PATH") == "this group have to img path define");

            // �׷� ���� ȭ�鿡 ���ο� �����׷� �����̳� �ν��Ͻ� ����
            GameObject newhighGroupContainer = ObjectPool.Instance.GetGroupSettingHGContainerObject(); // ���ο� �����׷� �����̳�
            string hgContainerName = $"Container_HighGroup_{row["FLD_HGID"].ToString()}";
            newhighGroupContainer.name = hgContainerName;

            GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // �����̳� �� �����׷� ��ü        
            Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // �����׷� ���� ��ư
            TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // �����׷��
            Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // �����׷� ���� ��ư
            Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // �����׷� �߰� ��ư
            uiElements[hgContainerName] = newhighGroupContainer;

            hgContainerTransform = newhighGroupContainer.transform;
            Transform hgTrs = hgContainerTransform;

            newObjHighGroup.name = $"HighGroup_{row["FLD_HGID"].ToString()}";
            newTxtHighGroupName.text = row["FLD_NAME"].ToString();
            AddHighGroupButtonListener(newObjHighGroup, hgTrs, row["FLD_HGID"].ToString(), row["FLD_NAME"].ToString(), row["FLD_IMG_PATH"].ToString(), row["FLD_IMG_WIDTH"].ToString(), row["FLD_IMG_HEIGHT"].ToString()); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)


            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
    }

    // ���� �׷� �����̳� �߰�
    private void AddLowGroupContainer(string hgid, Transform hgTransform)
    {
        if(ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // �����׷쿡 ���� �̹� �����ϴ� �����׷� ID�� �ִ밪 +1�� ���� ���ο� �����׷� INSERT
            DataTable lgTableMaxId = ClientDatabase.FetchLowGroupData().Tables[0];
            var maxLgIdRow = lgTableMaxId.AsEnumerable()
                .Where(r => r.Field<short>("FLD_HGID") == short.Parse(hgid))
                .Select(r => r.Field<short>("FLD_LGID"))
                .DefaultIfEmpty((short)0) // �⺻�� ������ ���� ���� �׷��� ���� ��츦 ó��
                .Max();
            int newLgId = maxLgIdRow + 1; // ���ο� FLD_LGID ���� ����
            string query = $"INSERT INTO TBL_LOW_GROUP (FLD_HGID, FLD_LGID, FLD_NAME) VALUES ('{hgid}', '{newLgId}', '�� �����׷�{newLgId}')";
            ClientDatabase.OnInsertRequest(query);

            // ���Ӱ� �߰��� ���� FLD_LGID ����
            DataTable lgTable = ClientDatabase.FetchLowGroupData().Tables[0];

            // FLD_HGID�� hgid�̰� FLD_LGID�� newLgId�� �� ã��
            DataRow row = lgTable.AsEnumerable()
                .FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"�� �����׷�{newLgId}");

            // ���ǿ� �´� ���� �ִ��� Ȯ���ϰ�, �ش� ���� ������ ó��
            if (row != null)
            {
                // ���ǿ� �´� ���� �ִٸ�, �ش� ���� �����͸� �̿��� ó���� ����
                string lgContainerName = $"Container_LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";

                // �׷� ���� ȭ�鿡 ���ο� �����׷� �����̳� �ν��Ͻ� ����
                GameObject newlowGroupContainer = Instantiate(ObjectPool.Instance.f_groupSettingLGContainerPrefab, hgTransform);
                ObjectPool.Instance.f_GroupSettingHGContainerObjects.Add(newlowGroupContainer);

                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // �����̳� �� �����׷� ��ü
                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // �����׷� ���� ��ư

                f_uiElements[lgContainerName] = newlowGroupContainer;

                f_lgContainerTransform = newlowGroupContainer.transform;
                newlowGroupContainer.name = lgContainerName;
                newObjLowGroup.name = $"LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";
                ipLowGroupName.placeholder.enabled = false;
                ipLowGroupName.text = row["FLD_NAME"].ToString();
                AddLowGroupButtonListener(newObjLowGroup, hgid, row["FLD_LGID"].ToString(), row["FLD_NAME"].ToString()); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
            }
            else
            {
                // ���ǿ� �´� ���� ���� ����� ó��
                //Debug.Log("No matching row found.");
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgTransform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(f_lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            // �����׷쿡 ���� �̹� �����ϴ� �����׷� ID�� �ִ밪 +1�� ���� ���ο� �����׷� INSERT
            DataTable lgTableMaxId = ClientDatabase.FetchLowGroupData().Tables[0];
            var maxLgIdRow = lgTableMaxId.AsEnumerable()
                .Where(r => r.Field<short>("FLD_HGID") == short.Parse(hgid))
                .Select(r => r.Field<short>("FLD_LGID"))
                .DefaultIfEmpty((short)0) // �⺻�� ������ ���� ���� �׷��� ���� ��츦 ó��
                .Max();
            int newLgId = maxLgIdRow + 1; // ���ο� FLD_LGID ���� ����
            string query = $"INSERT INTO TBL_LOW_GROUP (FLD_HGID, FLD_LGID, FLD_NAME) VALUES ('{hgid}', '{newLgId}', '�� �����׷�{newLgId}')";
            ClientDatabase.OnInsertRequest(query);

            // ���Ӱ� �߰��� ���� FLD_LGID ����
            DataTable lgTable = ClientDatabase.FetchLowGroupData().Tables[0];

            // FLD_HGID�� hgid�̰� FLD_LGID�� newLgId�� �� ã��
            DataRow row = lgTable.AsEnumerable()
                .FirstOrDefault(r => r.Field<string>("FLD_NAME") == $"�� �����׷�{newLgId}");

            // ���ǿ� �´� ���� �ִ��� Ȯ���ϰ�, �ش� ���� ������ ó��
            if (row != null)
            {
                // ���ǿ� �´� ���� �ִٸ�, �ش� ���� �����͸� �̿��� ó���� ����
                string lgContainerName = $"Container_LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";

                // �׷� ���� ȭ�鿡 ���ο� �����׷� �����̳� �ν��Ͻ� ����
                GameObject newlowGroupContainer = Instantiate(ObjectPool.Instance.groupSettingLGContainerPrefab, hgTransform);
                ObjectPool.Instance.GroupSettingHGContainerObjects.Add(newlowGroupContainer);

                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // �����̳� �� �����׷� ��ü
                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // �����׷� ���� ��ư

                uiElements[lgContainerName] = newlowGroupContainer;

                lgContainerTransform = newlowGroupContainer.transform;
                newlowGroupContainer.name = lgContainerName;
                newObjLowGroup.name = $"LowGroup_{hgid}_{row["FLD_LGID"].ToString()}";
                ipLowGroupName.placeholder.enabled = false;
                ipLowGroupName.text = row["FLD_NAME"].ToString();
                AddLowGroupButtonListener(newObjLowGroup, hgid, row["FLD_LGID"].ToString(), row["FLD_NAME"].ToString()); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
            }
            else
            {
                // ���ǿ� �´� ���� ���� ����� ó��
                //Debug.Log("No matching row found.");
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgTransform.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        
    }

    // �����׷� ��ư ������
    private void AddLowGroupButtonListener(GameObject objLowGroup, string hgid, string lgid, string lgName)
    {
        TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(objLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>(); // �����׷� �̸� ���� ��ǲ�ʵ�
        Button btnDeleteLowGroup = objLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // �����׷� ���� ��ư

        btnDeleteLowGroup.GetComponent<Button>().onClick.RemoveAllListeners();
        btnDeleteLowGroup.GetComponent<Button>().onClick.AddListener(() => DeleteLowGroup(objLowGroup, hgid, lgid, lgName));

        // onValueChanged ������ �߰�
        ipLowGroupName.onEndEdit.RemoveAllListeners(); // ���� ������ ����
        ipLowGroupName.onEndEdit.AddListener(delegate
        {
            if (ipLowGroupName.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "���� �׷���� �Էµ��� �ʾҽ��ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
            else
            {
                UpdateLowGroupName(hgid, lgid, lgName, ipLowGroupName.text);

                // �����׷� ���� �����ʵ� ����� �̸����� ������Ʈ
                btnDeleteLowGroup.GetComponent<Button>().onClick.RemoveAllListeners();
                btnDeleteLowGroup.GetComponent<Button>().onClick.AddListener(() => DeleteLowGroup(objLowGroup, hgid, lgid, ipLowGroupName.text));
            }
        }); // ���ο� ������ DB ������Ʈ
    }

    // �����׷� ��ǲ�ʵ� �̸����� �Ϸ� ������
    private void UpdateLowGroupName(string hgid, string lgid, string beforeValue, string changeValue)
    {
        // ���� ���� ����: FLD_NAME�� ���� �ùٸ� changeValue�� ������Ʈ�ϰ�, ���ڿ� ���� ���� ���� ����ǥ(') �߰�
        string query = $"UPDATE TBL_LOW_GROUP SET FLD_NAME = '{changeValue}' WHERE FLD_HGID = '{hgid}' AND FLD_LGID = '{lgid}'";
        if (ClientDatabase.OnUpdateRequest(query))
        {
            //Debug.Log($"�����׷���� ���� {beforeValue}���� {changeValue}�� ���� �Ϸ�");
        }
        else
        {
            //Debug.Log($"�����׷�� ���� ����");
        }
    }

    // �����׷� ��ư ������
    private void AddHighGroupButtonListener(GameObject objHighGroup, Transform hgTransform, string hgid, string hgName, string fpImgpath, string fpImgWidth, string fpImgHeight)
    {
        Button btnOpenGroupList = objHighGroup.transform.Find("btn_OpenGroupList").GetComponent<Button>(); // �����׷� ����Ʈ ���� ��ư
        Button btnHighGroupSetting = objHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // �����׷� ���� ��ư
        Button btnDeleteHighGroup = objHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // �����׷� ���� ��ư
        Button btnAddLowGroup = objHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // �����׷� �߰� ��ư

        btnOpenGroupList.GetComponent<Button>().onClick.AddListener(() => HandleLowGroupVisibility(hgid));
        btnHighGroupSetting.GetComponent<Button>().onClick.AddListener(() => HandleHighGroup(hgid, hgName, fpImgpath, fpImgWidth, fpImgHeight));
        btnDeleteHighGroup.GetComponent<Button>().onClick.AddListener(() => DeleteHighGroup(objHighGroup, hgTransform, hgid, hgName, fpImgpath, fpImgWidth, fpImgHeight));
        btnAddLowGroup.GetComponent<Button>().onClick.AddListener(() => AddLowGroupContainer(hgid, hgTransform));
    }

    // ���� �׷� ����
    public void DeleteHighGroup(GameObject objHighGroup, Transform hgTransform, string hgid, string hgName, string fpImgpath, string fpImgWidth, string fpImgHeight)
    {
        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
        ScreenManager.Instance.txt_PopUpMsg.text = $"{hgName} �׷��� �����Ͻðڽ��ϱ�?\n����� ��鵵, �����׷�, ��Ʈ�ѷ���\n�׷������� �Բ� �����˴ϴ�.";
        
        ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
        ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
            // TBL_CONTROLLER�� ����� �׷����� �ʱ�ȭ
            string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET HGID = 0, LGID = 0, GROUP_ORDER = 0 WHERE HGID = {hgid};";
            // TBL_LOW_GROUP�� ����� �׷����� ����
            string tblLowGroupQuery = $"DELETE FROM TBL_LOW_GROUP WHERE FLD_HGID = {hgid};";
            // TBL_HIGH_GROUP�� �ش��ϴ� �׷����� ����
            string tblHighGroupQuery = $"DELETE FROM TBL_HIGH_GROUP WHERE FLD_HGID = {hgid};";

            bool controllerUpdated = ClientDatabase.OnDeleteRequest(tblControllerQuery);
            bool lowGroupDeleted = ClientDatabase.OnDeleteRequest(tblLowGroupQuery);
            bool highGroupDeleted = ClientDatabase.OnDeleteRequest(tblHighGroupQuery);

            if (controllerUpdated && lowGroupDeleted && highGroupDeleted)
            {
                StartCoroutine(RefreshUIAfterCleanup());
                ScreenManager.Instance.ClosePopUpMessage();
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                //Debug.Log($"�����׷� ���� ���� �Ϸ� : {hgName}, {hgid}");
            }
            else
            {
                //Debug.Log($"�����׷� ���� ���� ���� : {hgName}, {hgid}");
            }
        });
    }

    // ���� �׷� ����
    public void DeleteLowGroup(GameObject objLowGroup, string hgid, string lgid, string lgName)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
            ScreenManager.Instance.txt_PopUpMsg.text = $"{lgName} �׷��� �����Ͻðڽ��ϱ�?\n����� ��Ʈ�ѷ��� �׷�������\n�Բ� �����˴ϴ�.";

            ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
                // TBL_CONTROLLER�� ����� �׷����� �ʱ�ȭ
                string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET HGID = 0, LGID = 0, GROUP_ORDER = 0 WHERE HGID = {hgid} AND LGID = {lgid};";
                // TBL_LOW_GROUP�� ����� �׷����� ����
                string tblLowGroupQuery = $"DELETE FROM TBL_LOW_GROUP WHERE FLD_HGID = {hgid} AND FLD_LGID = {lgid};";

                bool controllerUpdated = ClientDatabase.OnDeleteRequest(tblControllerQuery);
                bool lowGroupDeleted = ClientDatabase.OnDeleteRequest(tblLowGroupQuery);

                if (controllerUpdated && lowGroupDeleted)
                {
                    //Debug.Log($"�����׷� ���� ���� �Ϸ� : {lgName}, {lgid}");
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
                    //Debug.Log($"�����׷� ���� ���� ���� : {lgName}, {lgid}");
                }
            });
        }
        else
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
            ScreenManager.Instance.txt_PopUpMsg.text = $"{lgName} �׷��� �����Ͻðڽ��ϱ�?\n����� ��Ʈ�ѷ��� �׷�������\n�Բ� �����˴ϴ�.";

            ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => {
                // TBL_CONTROLLER�� ����� �׷����� �ʱ�ȭ
                string tblControllerQuery = $"UPDATE TBL_CONTROLLER SET HGID = 0, LGID = 0, GROUP_ORDER = 0 WHERE HGID = {hgid} AND LGID = {lgid};";
                // TBL_LOW_GROUP�� ����� �׷����� ����
                string tblLowGroupQuery = $"DELETE FROM TBL_LOW_GROUP WHERE FLD_HGID = {hgid} AND FLD_LGID = {lgid};";

                bool controllerUpdated = ClientDatabase.OnDeleteRequest(tblControllerQuery);
                bool lowGroupDeleted = ClientDatabase.OnDeleteRequest(tblLowGroupQuery);

                if (controllerUpdated && lowGroupDeleted)
                {
                    //Debug.Log($"�����׷� ���� ���� �Ϸ� : {lgName}, {lgid}");
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
                    //Debug.Log($"�����׷� ���� ���� ���� : {lgName}, {lgid}");
                }
            });
        }        
    }

    public void OpenGroupSetting()
    {        
        SideMenuManager.Instance.SideMenuStateChange();
        LoadGroupAssets();
    }

    // �� �׷� ��ҿ� Button ����
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
                        // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
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
                        GameObject newhighGroupContainer = ObjectPool.Instance.f_GetGroupSettingHGContainerObject(); // ���ο� �����׷� �����̳�
                        GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // �����̳� �� �����׷� ��ü                
                        Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // �����׷� ���� ��ư
                        TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // �����׷��
                        Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // �����׷� ���� ��ư
                        Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // �����׷� �߰� ��ư
                        f_uiElements[hgContainerName] = newhighGroupContainer;

                        f_hgContainerTransform = newhighGroupContainer.transform;
                        newhighGroupContainer.name = $"Container_HighGroup_{hgid}";
                        newObjHighGroup.name = $"HighGroup_{hgid}";
                        newTxtHighGroupName.text = hgName;
                        AddHighGroupButtonListener(newObjHighGroup, f_hgContainerTransform, hgid, hgName, fpImgPath, fpImgWidth, fpImgHeight); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                    }

                    // �����׷쿡 �ش��ϴ� �����׷츸 ���͸��Ͽ� �߰�            
                    var filteredLowGroups = tableLowGroup.AsEnumerable().Where(row => row["FLD_HGID"] != DBNull.Value && row.Field<short>("FLD_HGID").ToString() == hgid);

                    if (tableLowGroup.Rows.Count > 0)
                    {
                        // �����׷��� LowGroupContainer�� �����׷� ��� ����
                        foreach (DataRow lowGroupRow in filteredLowGroups)
                        {
                            string lgid = lowGroupRow["FLD_LGID"].ToString();
                            string lgName = lowGroupRow["FLD_NAME"].ToString();
                            string lgContainerName = $"Container_LowGroup_{hgid}_{lgid}";

                            if (f_uiElements.TryGetValue(lgContainerName, out var lowGroupContainer))
                            {
                                // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
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
                                // �����׷� ��� ����
                                GameObject newlowGroupContainer = ObjectPool.Instance.f_GetGroupSettingLGContainerObject(); // ���ο� �����׷� �����̳�
                                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // �����̳� �� �����׷� ��ü
                                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // �����׷� ���� ��ư

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
                    // 'Container_HighGroup_'�� �������� �ʴ� ��� ������Ʈ�� ��Ȱ��ȭ
                    if (!uiElement.Key.StartsWith("Container_HighGroup_"))
                    {
                        GameObject obj = uiElement.Value;
                        if (obj != null) // ������Ʈ�� null�� �ƴ��� Ȯ��
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
                        // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
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
                        GameObject newhighGroupContainer = ObjectPool.Instance.GetGroupSettingHGContainerObject(); // ���ο� �����׷� �����̳�
                        GameObject newObjHighGroup = newhighGroupContainer.transform.Find("obj_HighGroup").gameObject; // �����̳� �� �����׷� ��ü                
                        Button newBtnHighGroupSetting = newObjHighGroup.transform.Find("btn_GroupSetting").GetComponent<Button>(); // �����׷� ���� ��ư
                        TextMeshProUGUI newTxtHighGroupName = newBtnHighGroupSetting.transform.Find("txt_GroupName").GetComponent<TextMeshProUGUI>(); // �����׷��
                        Button newBtnDeleteHighGroup = newObjHighGroup.transform.Find("btn_DeleteHighGroup").GetComponent<Button>(); // �����׷� ���� ��ư
                        Button newBtnAddGroup = newObjHighGroup.transform.Find("btn_AddLowGroup").GetComponent<Button>(); // �����׷� �߰� ��ư
                        uiElements[hgContainerName] = newhighGroupContainer;

                        hgContainerTransform = newhighGroupContainer.transform;
                        newhighGroupContainer.name = $"Container_HighGroup_{hgid}";
                        newObjHighGroup.name = $"HighGroup_{hgid}";
                        newTxtHighGroupName.text = hgName;
                        AddHighGroupButtonListener(newObjHighGroup, hgContainerTransform, hgid, hgName, fpImgPath, fpImgWidth, fpImgHeight); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                    }

                    // �����׷쿡 �ش��ϴ� �����׷츸 ���͸��Ͽ� �߰�            
                    var filteredLowGroups = tableLowGroup.AsEnumerable().Where(row => row["FLD_HGID"] != DBNull.Value && row.Field<short>("FLD_HGID").ToString() == hgid);

                    if (tableLowGroup.Rows.Count > 0)
                    {
                        // �����׷��� LowGroupContainer�� �����׷� ��� ����
                        foreach (DataRow lowGroupRow in filteredLowGroups)
                        {
                            string lgid = lowGroupRow["FLD_LGID"].ToString();
                            string lgName = lowGroupRow["FLD_NAME"].ToString();
                            string lgContainerName = $"Container_LowGroup_{hgid}_{lgid}";

                            if (uiElements.TryGetValue(lgContainerName, out var lowGroupContainer))
                            {
                                // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
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
                                // �����׷� ��� ����
                                GameObject newlowGroupContainer = ObjectPool.Instance.GetGroupSettingLGContainerObject(); // ���ο� �����׷� �����̳�
                                GameObject newObjLowGroup = newlowGroupContainer.transform.Find("obj_LowGroup").gameObject; // �����̳� �� �����׷� ��ü
                                TMP_InputField ipLowGroupName = SideMenuManager.FindChildStartingWithName(newObjLowGroup.transform, "InputField_LowGroupSetting").GetComponent<TMP_InputField>();
                                Button newBtnDeleteLowGroup = newObjLowGroup.transform.Find("btn_DeleteLowGroup").GetComponent<Button>(); // �����׷� ���� ��ư

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
                    // 'Container_HighGroup_'�� �������� �ʴ� ��� ������Ʈ�� ��Ȱ��ȭ
                    if (!uiElement.Key.StartsWith("Container_HighGroup_"))
                    {
                        GameObject obj = uiElement.Value;
                        if (obj != null) // ������Ʈ�� null�� �ƴ��� Ȯ��
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

    // �����׷� ���� �ڵ鷯(�����׷��, �����׷� ��鵵 �̹���)
    private void HandleHighGroup(string hgid, string hgName, string fpImgpath, string fpImgWidth, string fpImgHeight)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_settingHighGroup.SetActive(true);

            Image fpImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // ��鵵 �̹��� ���� ���� ��ư
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();
            Button btnPrev = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Prev").GetComponent<Button>(); // �ڷΰ���(=���) ��ư

            btnChooseFPImage.onClick.AddListener(() => OpenFileChooser()); // �̹��� ���ÿ� ���� ������
            btnPrev.onClick.AddListener(() =>
            {
                fpImage.sprite = null;

                f_hgElement_Id = string.Empty;
                f_hgElement_Name = string.Empty;
                f_hgElement_ImgPath = string.Empty;
                f_settingHighGroup.SetActive(false);
            }); // �����׷� ���� ��� �� ���ư��� ������


            if (f_inputFieldHGName.placeholder is TMP_Text placeholderText)
                placeholderText.text = "�����׷�� �Է�";
            f_inputFieldHGName.text = string.Empty;

            if (hgName == string.Empty || fpImgpath == string.Empty) // ������ �׷� ������ DB�� ���� ���
            {
                f_inputFieldHGName.placeholder.enabled = true;
                f_inputFieldHGName.text = "";
                txtChooseFPImage.text = "�̹��� ����";
            }
            else // �ش� �׷� ������ �����ϴ� ���
            {
                f_hgElement_Id = hgid;
                f_hgElement_Name = hgName;
                f_hgElement_ImgPath = fpImgpath;

                f_inputFieldHGName.placeholder.enabled = false;
                f_inputFieldHGName.text = hgName;

                txtChooseFPImage.text = "�̹��� �ٽ� ����";
                StartCoroutine(LoadImageCoroutine(fpImgpath));

                Button btnConfirm = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // Ȯ�� -> �׷켳�� ȭ������ �������� �̰�
                btnConfirm.onClick.RemoveAllListeners();
                btnConfirm.onClick.AddListener(() =>
                {
                    FinishHighGroupSetting(fpImgpath);
                }); // Ȯ�� �� �׷켳������ �������� �̰� ������
            }
        }
        else
        {
            settingHighGroup.SetActive(true);

            Image fpImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // ��鵵 �̹��� ���� ���� ��ư
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();
            Button btnPrev = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Title/btn_Prev").GetComponent<Button>(); // �ڷΰ���(=���) ��ư

            btnChooseFPImage.onClick.AddListener(() => OpenFileChooser()); // �̹��� ���ÿ� ���� ������
            btnPrev.onClick.AddListener(() =>
            {
                fpImage.sprite = null;

                hgElement_Id = string.Empty;
                hgElement_Name = string.Empty;
                hgElement_ImgPath = string.Empty;
                settingHighGroup.SetActive(false);
            }); // �����׷� ���� ��� �� ���ư��� ������


            if (inputFieldHGName.placeholder is TMP_Text placeholderText)
                placeholderText.text = "�����׷�� �Է�";
            inputFieldHGName.text = string.Empty;

            if (hgName == string.Empty || fpImgpath == string.Empty) // ������ �׷� ������ DB�� ���� ���
            {
                inputFieldHGName.placeholder.enabled = true;
                inputFieldHGName.text = "";
                txtChooseFPImage.text = "�̹��� ����";
            }
            else // �ش� �׷� ������ �����ϴ� ���
            {
                hgElement_Id = hgid;
                hgElement_Name = hgName;
                hgElement_ImgPath = fpImgpath;

                inputFieldHGName.placeholder.enabled = false;
                inputFieldHGName.text = hgName;

                txtChooseFPImage.text = "�̹��� �ٽ� ����";
                StartCoroutine(LoadImageCoroutine(fpImgpath));

                Button btnConfirm = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // Ȯ�� -> �׷켳�� ȭ������ �������� �̰�
                btnConfirm.onClick.RemoveAllListeners();
                btnConfirm.onClick.AddListener(() =>
                {
                    FinishHighGroupSetting(fpImgpath);
                }); // Ȯ�� �� �׷켳������ �������� �̰� ������
            }
        }        
    }

    // �����׷� ���� ȭ�� Ȯ�� ��ư
    private void FinishHighGroupSetting(string selectedImgPath)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            // inputFieldHGName�� text ���� ������� Ȯ��
            if (f_inputFieldHGName.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "���� �׷���� �Էµ��� �ʾҽ��ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
            else
            {
                // FLD_HGID�� �̹��� ���ϸ��� ���������� �����Ǹ� FLD_IMG_PATH ��� UPDATE
                if (selectedImgPath == "this group have to img path define")
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�̹����� ���õ��� �ʾҽ��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                    //Debug.LogError("���� �׷� ���� ����");
                }
                else
                {
                    f_hgElement_Name = f_inputFieldHGName.text;
                    string query = $"UPDATE TBL_HIGH_GROUP SET FLD_NAME = '{f_hgElement_Name}', FLD_IMG_PATH = '{selectedImgPath}', FLD_IMG_WIDTH = {f_selectedImgWidth}, FLD_IMG_HEIGHT = {f_selectedImgHeight} WHERE FLD_HGID = {f_hgElement_Id}";
                    ClientDatabase.OnUpdateRequest(query);

                    // ���Ӱ� �߰��� ���� FLD_HGID ������ ����
                    DataTable hgTable = ClientDatabase.FetchHighGroupData().Tables[0];
                    var row = hgTable.AsEnumerable().FirstOrDefault(r => r.Field<string>("FLD_NAME") == f_hgElement_Name && r.Field<string>("FLD_IMG_PATH") == selectedImgPath);
                    if (row != null)
                    {
                        f_hgElement_Id = row["FLD_HGID"].ToString();

                        if (RenameImageFile(selectedImgPath, f_hgElement_Id))
                        {
                            StartCoroutine(RefreshUIAfterCleanup());
                            f_settingHighGroup.SetActive(false);
                            //Debug.Log("���� �׷� ���� �Ϸ�");
                        }
                        else
                        {
                            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            ScreenManager.Instance.txt_PopUpMsg.text = "������ ã�� �� �����ϴ�.";
                            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                            //Debug.LogError("���� �׷� ���� ����");
                        }
                    }
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(f_groupListContent.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
        }
        else
        {
            // inputFieldHGName�� text ���� ������� Ȯ��
            if (inputFieldHGName.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "���� �׷���� �Էµ��� �ʾҽ��ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
            }
            else
            {
                // FLD_HGID�� �̹��� ���ϸ��� ���������� �����Ǹ� FLD_IMG_PATH ��� UPDATE
                if (selectedImgPath == "this group have to img path define")
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�̹����� ���õ��� �ʾҽ��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                    //Debug.LogError("���� �׷� ���� ����");
                }
                else
                {
                    hgElement_Name = inputFieldHGName.text;
                    string query = $"UPDATE TBL_HIGH_GROUP SET FLD_NAME = '{hgElement_Name}', FLD_IMG_PATH = '{selectedImgPath}', FLD_IMG_WIDTH = {selectedImgWidth}, FLD_IMG_HEIGHT = {selectedImgHeight} WHERE FLD_HGID = {hgElement_Id}";
                    ClientDatabase.OnUpdateRequest(query);

                    // ���Ӱ� �߰��� ���� FLD_HGID ������ ����
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
                            //Debug.Log("���� �׷� ���� �Ϸ�");
                        }
                        else
                        {
                            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            ScreenManager.Instance.txt_PopUpMsg.text = "������ ã�� �� �����ϴ�.";
                            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                            //Debug.LogError("���� �׷� ���� ����");
                        }
                    }
                }



                LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
                Canvas.ForceUpdateCanvases();
            }
        }
    }

    // ���� ���� �� ���ΰ�ħ
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

            // ���� �����ӱ��� ��ٸ��ϴ�.
            yield return new WaitForEndOfFrame();

            // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
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

            // ���� �����ӱ��� ��ٸ��ϴ�.
            yield return new WaitForEndOfFrame();

            // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
            LoadGroupAssets();

            LayoutRebuilder.ForceRebuildLayoutImmediate(groupListContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }

    // �����׷� ���� �ݱ� ��ư �ڵ鷯
    private void HandleLowGroupVisibility(string hgid)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            Transform highGroupContainer = f_uiElements[$"Container_HighGroup_{hgid}"].transform;
            bool isActive = false; // �ʱ� ���� ����

            // btn_HighGroup ������ Ȱ��ȭ/��Ȱ��ȭ �̹��� ���
            GameObject activeImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Open").gameObject;
            GameObject deactiveImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Close").gameObject;

            isActive = !activeImage.activeSelf; // ���� ���¿� ���� ����

            activeImage.SetActive(isActive);
            deactiveImage.SetActive(!isActive);

            // Container_LowGroup�� �����ϴ� ��� �ڽ� ������Ʈ�� Ȱ��ȭ ���� ���
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
            bool isActive = false; // �ʱ� ���� ����

            // btn_HighGroup ������ Ȱ��ȭ/��Ȱ��ȭ �̹��� ���
            GameObject activeImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Open").gameObject;
            GameObject deactiveImage = highGroupContainer.Find($"HighGroup_{hgid}/btn_OpenGroupList/State_Close").gameObject;

            isActive = !activeImage.activeSelf; // ���� ���¿� ���� ����

            activeImage.SetActive(isActive);
            deactiveImage.SetActive(!isActive);

            // Container_LowGroup�� �����ϴ� ��� �ڽ� ������Ʈ�� Ȱ��ȭ ���� ���
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

    // Android ���� ���� ��� ȣ��
    void OpenFileChooser()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", new AndroidJavaObject("android.content.Intent", currentActivity, new AndroidJavaClass("com.systronics.plugin.FileChooserActivity")));
    }

    // ���� ��θ� �����ϴ� �Լ�
    public void ReceiveFilePath(string selectedFilePath)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            f_hgElement_ImgPath = selectedFilePath;
            f_selectedImgPath = selectedFilePath;
            StartCoroutine(LoadImageCoroutine(selectedFilePath));

            Button btnConfirm = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // Ȯ�� -> �׷켳�� ȭ������ �������� �̰�
            btnConfirm.onClick.RemoveAllListeners();
            btnConfirm.onClick.AddListener(() => FinishHighGroupSetting(f_selectedImgPath)); // Ȯ�� �� �׷켳������ �������� �̰� ������
        }
        else
        {
            hgElement_ImgPath = selectedFilePath;
            selectedImgPath = selectedFilePath;
            StartCoroutine(LoadImageCoroutine(selectedFilePath));

            Button btnConfirm = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/Bottom/btn_Confirm").GetComponent<Button>(); // Ȯ�� -> �׷켳�� ȭ������ �������� �̰�
            btnConfirm.onClick.RemoveAllListeners();
            btnConfirm.onClick.AddListener(() => FinishHighGroupSetting(selectedImgPath)); // Ȯ�� �� �׷켳������ �������� �̰� ������
        }        
    }

    // �ȵ���̵� ����̽��� Ư�� ��ο� ����� �̹��� ������ �ҷ���
    private IEnumerator LoadImageCoroutine(string imagePath)
    {
        if (ScreenManager.Instance.CurrentScreenState == ScreenManager.ScreenState.None)
        {
            Image fpImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectedImageParent/SelectedFPImage").GetComponent<Image>();
            Button btnChooseFPImage = f_settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // ��鵵 �̹��� ���� ���� ��ư
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();

            if (imagePath == "this group have to img path define")
            {
                txtChooseFPImage.text = "�̹��� ����";
            }
            else
            {
                txtChooseFPImage.text = "�̹��� �ٽ� ����";
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + imagePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                        fpImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                        // �̹��� Width�� Height ����
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
            Button btnChooseFPImage = settingHighGroup.transform.Find("SettingGroupParent/obj_Setting_Group/SelectImage/btn_AddGroup").GetComponent<Button>(); // ��鵵 �̹��� ���� ���� ��ư
            TextMeshProUGUI txtChooseFPImage = btnChooseFPImage.transform.Find("AddGroupParent/txt_SelectImage").GetComponent<TextMeshProUGUI>();

            if (imagePath == "this group have to img path define")
            {
                txtChooseFPImage.text = "�̹��� ����";
            }
            else
            {
                txtChooseFPImage.text = "�̹��� �ٽ� ����";
                using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://" + imagePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                        fpImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                        // �̹��� Width�� Height ����
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

    // �ȵ���̵� ����̽��� Ư�� ��ο� ����� �̹��� ������ ����
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
            // ���� ������ ������ �����ϴ��� Ȯ��
            if (!File.Exists(originalFilePath))
            {
                //Debug.LogError("GroupSettingManager : Original FPImage does not exist: " + originalFilePath);
                return false;
            }

            // ���� Ȯ���� ����
            string fileExtension = Path.GetExtension(originalFilePath);

            // �� ���ϸ��� ��ü ��θ� Ȯ���ڿ� �Բ� ����
            string newFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath), newHGID + fileExtension);
            f_hgElement_ImgPath = newFilePath;

            try
            {
                // ��� ������ �̹� ������ ��� ����� ����Ͽ� ���� ����
                File.Copy(originalFilePath, newFilePath, true); // true �Ķ���ʹ� ����⸦ �����
                //Debug.Log("GroupSettingManager : File renamed from " + originalFilePath + " to " + newFilePath);

                // ���� ���� ����
                File.Delete(originalFilePath);

                // UPDATE ������ �����Ͽ� FLD_IMG_PATH ��θ� ������Ʈ
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
            // ���� ������ ������ �����ϴ��� Ȯ��
            if (!File.Exists(originalFilePath))
            {
                //Debug.LogError("GroupSettingManager : Original FPImage does not exist: " + originalFilePath);
                return false;
            }

            // ���� Ȯ���� ����
            string fileExtension = Path.GetExtension(originalFilePath);

            // �� ���ϸ��� ��ü ��θ� Ȯ���ڿ� �Բ� ����
            string newFilePath = Path.Combine(Path.GetDirectoryName(originalFilePath), newHGID + fileExtension);
            hgElement_ImgPath = newFilePath;

            try
            {
                // ��� ������ �̹� ������ ��� ����� ����Ͽ� ���� ����
                File.Copy(originalFilePath, newFilePath, true); // true �Ķ���ʹ� ����⸦ �����
                //Debug.Log("GroupSettingManager : File renamed from " + originalFilePath + " to " + newFilePath);

                // ���� ���� ����
                File.Delete(originalFilePath);

                // UPDATE ������ �����Ͽ� FLD_IMG_PATH ��θ� ������Ʈ
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