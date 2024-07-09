using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ScreenManager;

public class SideMenuManager : MonoBehaviour
{
    public GameObject sideMenu;
    private Animator sideMenuAnim;
    private bool isSideMenuOpen = false;
    private Coroutine currentCoroutine = null; // ���� ���� ���� �ڷ�ƾ�� ����
    private static readonly WaitForSeconds waitSec = new WaitForSeconds(0.6f);

    //UI ��� ������ ���� ��ųʸ�
    public Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();

    public GameObject sideMenuScrollView; // SideMenu_ScrollView
    public Transform sideMenuContent; // SideMenu_ScrollView�� Content
    public Transform hgContainerTransform; // Container_HighGroup�� Transform
    public Transform lgContainerTransform; // Container_LowGroup�� Transform

    private string selectedIid = null;
    private string selectedCid = null;


    public static SideMenuManager Instance { get; private set; }

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

        sideMenuContent = sideMenuScrollView.transform.Find("Viewport/Content");
    }

    private void Start()
    {
        sideMenuAnim = sideMenu.GetComponent<Animator>();
    }

    public void SideMenuStateChange()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(IStateChange());
    }

    IEnumerator IStateChange()
    {
        if (!isSideMenuOpen)
        {
            LoadGroupAssets();
            ClientDatabase.isPolling = false;
            sideMenuAnim.SetBool("SideMenu_Idle", false);
            sideMenuAnim.SetBool("SideMenu_Open", true);
            sideMenu.GetComponent<DropShadow>().enabled = true;
            DecisionShowFPPScreen();
        }
        else
        {
            ClientDatabase.isPolling = false;
            sideMenuAnim.SetBool("SideMenu_Open", false);
            sideMenuAnim.SetBool("SideMenu_Close", true);
            sideMenu.GetComponent<DropShadow>().enabled = false;

            ObjectPool.Instance.CloseSideMenu();
            uiElements.Clear();
            for (int i = sideMenuContent.transform.childCount - 1; i >= 0; i--)
            {                
                Destroy(sideMenuContent.transform.GetChild(i).gameObject);
            }

            hgContainerTransform = null;
            lgContainerTransform = null;
        }
        isSideMenuOpen = !isSideMenuOpen;

        yield return waitSec;
        ClientDatabase.isPolling = true;
        currentCoroutine = null;
    }

    // ���� ���� �� ���ΰ�ħ
    public IEnumerator RefreshUIAfterCleanup()
    {
        for (int i = sideMenuContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(sideMenuContent.transform.GetChild(i).gameObject);
        }
        ObjectPool.Instance.CloseSideMenu();
        uiElements.Clear();

        // ���� �����ӱ��� ��ٸ��ϴ�.
        yield return new WaitForSeconds(0.45f);

        // ���� ��� ������Ʈ�� ���ŵǾ����Ƿ� UI�� ���ΰ�ħ�մϴ�.
        LoadGroupAssets();
    }

    public static GameObject FindChildStartingWithName(Transform parent, string nameStart)
    {
        foreach (Transform child in parent)
        {
            if (child.name.StartsWith(nameStart))
            {
                return child.gameObject;
            }
        }
        return null; // ã�� ���� ��� null ��ȯ
    }

    // �� �׷� ��ҿ� Button ����
    public void LoadGroupAssets()
    {
        DataTable tableHighGroup = ClientDatabase.highGroupData.Tables[0];
        DataTable tableLowGroup = ClientDatabase.lowGroupData.Tables[0];
        DataTable tableController = ClientDatabase.controllerData.Tables[0];

        if(tableHighGroup.Rows.Count > 0)
        {
            foreach (DataRow highGroupRow in tableHighGroup.Rows)
            {
                string hgid = highGroupRow["FLD_HGID"].ToString();
                string hgName = highGroupRow["FLD_NAME"].ToString();
                string hgContainerName = $"Container_HighGroup_{hgid}";

                if (uiElements.TryGetValue(hgContainerName, out var highGroupContainer))
                {
                    // ���� �����׷� ��Ұ� ������ ���븸 ������Ʈ
                    GameObject btnHighGroup = FindChildStartingWithName(highGroupContainer.transform, "btn_HighGroup");
                    TextMeshProUGUI txtHighGroupName = FindChildStartingWithName(btnHighGroup.transform, "txt_HighGroupName").GetComponent<TextMeshProUGUI>();

                    GameObject activeImage = highGroupContainer.transform.Find($"btn_HighGroup_{hgid}/Img_Active").gameObject;
                    GameObject deactiveImage = highGroupContainer.transform.Find($"btn_HighGroup_{hgid}/Img_Deactive").gameObject;
                    deactiveImage.SetActive(true);
                    activeImage.SetActive(false);

                    hgContainerTransform = highGroupContainer.transform;
                    txtHighGroupName.text = hgName;
                }
                else
                {
                    // �����׷� ��� ����
                    GameObject newhighGroupContainer = ObjectPool.Instance.GetHighGroupContainerObject();
                    GameObject newbtnHighGroup = newhighGroupContainer.transform.Find("btn_HighGroup").gameObject;
                    TextMeshProUGUI newtxtHighGroupName = newbtnHighGroup.transform.Find("txt_HighGroupName").GetComponent<TextMeshProUGUI>();
                    AddHighGroupButtonListener(newbtnHighGroup, hgid); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� �����׷� ���� �ڵ鷯)
                    uiElements[hgContainerName] = newhighGroupContainer;

                    hgContainerTransform = newhighGroupContainer.transform;
                    newhighGroupContainer.name = $"Container_HighGroup_{hgid}";
                    newbtnHighGroup.name = $"btn_HighGroup_{hgid}";
                    newtxtHighGroupName.text = hgName;
                }

                // �����׷쿡 �ش��ϴ� �����׷츸 ���͸��Ͽ� �߰�            
                var filteredLowGroups = tableLowGroup.AsEnumerable().Where(row => row["FLD_HGID"] != DBNull.Value && row.Field<short>("FLD_HGID").ToString() == hgid);

                if(tableLowGroup.Rows.Count > 0)
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
                            GameObject btnLowGroup = FindChildStartingWithName(lowGroupContainer.transform, "btn_LowGroup");
                            TextMeshProUGUI txtLowGroupName = FindChildStartingWithName(btnLowGroup.transform, "txt_LowGroupName").GetComponent<TextMeshProUGUI>();
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
                            GameObject newlowGroupContainer = ObjectPool.Instance.GetLowGroupContainerObject();
                            GameObject newbtnLowGroup = newlowGroupContainer.transform.Find("btn_LowGroup").gameObject;
                            TextMeshProUGUI newtxtLowGroupName = newbtnLowGroup.transform.Find("txt_LowGroupName").GetComponent<TextMeshProUGUI>();
                            AddLowGroupButtonListener(newbtnLowGroup, hgid, lgid); // �̺�Ʈ ������ �߰�(ȭ��ǥ ���� �� ��Ʈ�ѷ� ���� �ڵ鷯)
                            uiElements[lgContainerName] = newlowGroupContainer;

                            lgContainerTransform = newlowGroupContainer.transform;
                            newlowGroupContainer.name = $"Container_LowGroup_{hgid}_{lgid}";
                            newbtnLowGroup.name = $"btn_LowGroup_{hgid}_{lgid}";
                            newtxtLowGroupName.text = lgName;
                        }

                        // �����׷쿡 �ش��ϴ� ��Ʈ�ѷ��� ���͸��Ͽ� �߰�
                        var filteredControllers = tableController.AsEnumerable().Where(row => row["HGID"] != DBNull.Value && row["LGID"] != DBNull.Value &&
                                                                                        row.Field<int>("HGID").ToString() == hgid && row.Field<int>("LGID").ToString() == lgid);

                        if (tableController.Rows.Count > 0)
                        {
                            // �����׷쿡 ��Ʈ�ѷ� ��� ����
                            foreach (DataRow controllerRow in filteredControllers)
                            {
                                string iid = controllerRow["ID"].ToString();
                                string cid = controllerRow["CID"].ToString();
                                string cName = controllerRow["CNAME"].ToString();
                                string controllerContainerName = $"Container_Controller_{iid}_{cid}";

                                if (uiElements.TryGetValue(controllerContainerName, out var controllerContainer))
                                {
                                    // ���� ��Ʈ�ѷ� ��Ұ� ������ ���븸 ������Ʈ
                                    GameObject btnController = FindChildStartingWithName(controllerContainer.transform, "btn_Controller");
                                    TextMeshProUGUI txtControllerName = FindChildStartingWithName(btnController.transform, "txt_ControllerName").GetComponent<TextMeshProUGUI>();

                                    txtControllerName.text = cName;
                                }
                                else
                                {
                                    // ��Ʈ�ѷ� ��� ����
                                    GameObject newcontrollerContainer = ObjectPool.Instance.GetControllerContainerObject();
                                    GameObject newbtnController = newcontrollerContainer.transform.Find("btn_Controller").gameObject;
                                    TextMeshProUGUI newtxtControllerName = newbtnController.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>();
                                    AddControllerButtonListener(newcontrollerContainer, iid, cid); // �̺�Ʈ ������ �߰�(��Ʈ�ѷ� Ŭ�� �� �ڵ鷯)
                                    uiElements[controllerContainerName] = newcontrollerContainer;

                                    newcontrollerContainer.name = $"Container_Controller_{iid}_{cid}";
                                    newbtnController.name = $"btn_Controller_{iid}_{cid}";
                                    newtxtControllerName.text = cName;
                                }
                            }
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

            LayoutRebuilder.ForceRebuildLayoutImmediate(sideMenuContent.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(hgContainerTransform.GetComponent<RectTransform>());
            //LayoutRebuilder.ForceRebuildLayoutImmediate(lgContainerTransform.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }
        
    }

    // �����׷� ��ư ������
    private void AddHighGroupButtonListener(GameObject btnHighGroup, string hgid)
    {
        btnHighGroup.GetComponent<Button>().onClick.AddListener(() => ToggleLowGroupVisibility(hgid));
    }

    // �����׷� ��ư ������
    private void AddLowGroupButtonListener(GameObject btnLowGroup, string hgid, string lgid)
    {
        btnLowGroup.GetComponent<Button>().onClick.AddListener(() => ToggleControllerVisibility(hgid, lgid));
    }

    // ��Ʈ�ѷ� ��ư ������
    private void AddControllerButtonListener(GameObject newcontrollerContainer, string iid, string cid)
    {
        GameObject newbtnController = newcontrollerContainer.transform.Find("btn_Controller").gameObject;
        GameObject newbtnControllerStyle = newcontrollerContainer.transform.Find("btn_ControllerStyle").gameObject;

        newbtnController.GetComponent<Button>().onClick.RemoveAllListeners();
        newbtnController.GetComponent<Button>().onClick.AddListener(() => {
            // ���� Ŭ���� ��Ʈ�ѷ��� ���õ� ���·� ������Ʈ
            selectedIid = iid;
            selectedCid = cid;

            // ��� ��Ʈ�ѷ��� ���� ���� ������Ʈ
            UpdateAllControllerStates();
            DetailView.Instance.CloseDetailView();
            //DetailView.Instance.OpenDetailView(newbtnController, iid, cid);
            DetailView.Instance.OpenDetailView(iid, cid);
        });
        newbtnControllerStyle.GetComponent<Button>().onClick.RemoveAllListeners();
        newbtnControllerStyle.GetComponent<Button>().onClick.AddListener(() => {
            // ���� Ŭ���� ��Ʈ�ѷ��� ���õ� ���·� ������Ʈ
            selectedIid = iid;
            selectedCid = cid;

            // ��� ��Ʈ�ѷ��� ���� ���� ������Ʈ
            //UpdateAllControllerStates();

            if (ScreenManager.Instance.CurrentScreenState == ScreenState.FloorPlan)
            {
                DataTable tblController = ClientDatabase.FetchControllerData(iid, cid).Tables[0];
                bool isfppGen = false;
                // �����׷�, ��Ʈ�ѷ��� �Ҵ�
                foreach (DataRow row in tblController.Rows)
                {
                    if (iid == row["ID"].ToString() && cid == row["CID"].ToString())
                    {
                        isfppGen = row["FPP_GEN"].ToString() == "1" ? true : false;
                    }
                }

                if(isfppGen)
                    FloorPlanManager.Instance.ModifyFPPSetting(iid, cid);
                else
                    FloorPlanManager.Instance.OpenFPPSetting(iid, cid);
            }
        });
    }

    // ȭ�� ���¿� ���� ��Ʈ�ѷ� ��鵵 �߰�, ���� ��ư Ȱ��, ��Ȱ��
    public void DecisionShowFPPScreen()
    {
        foreach(var controllerContainer in ObjectPool.Instance.ControllerContainerObjects)
        {
            if(ScreenManager.Instance.CurrentScreenState == ScreenState.FloorPlan)
                controllerContainer.transform.Find("btn_ControllerStyle").GetComponent<Button>().gameObject.SetActive(true);
            else
                controllerContainer.transform.Find("btn_ControllerStyle").GetComponent<Button>().gameObject.SetActive(false);

        }
    }

    // �����׷� ��ư �ڵ鷯
    private void ToggleLowGroupVisibility(string hgid)
    {
        Transform highGroupContainer = uiElements[$"Container_HighGroup_{hgid}"].transform;
        bool isActive = false; // �ʱ� ���� ����

        // btn_HighGroup ������ Ȱ��ȭ/��Ȱ��ȭ �̹��� ���
        GameObject activeImage = highGroupContainer.Find($"btn_HighGroup_{hgid}/Img_Active").gameObject;
        GameObject deactiveImage = highGroupContainer.Find($"btn_HighGroup_{hgid}/Img_Deactive").gameObject;

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(sideMenuContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(highGroupContainer.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // �����׷� ��ư �ڵ鷯
    private void ToggleControllerVisibility(string hgid, string lgid)
    {
        Transform lowGroupContainer = uiElements[$"Container_LowGroup_{hgid}_{lgid}"].transform;
        bool isActive = false; // �ʱ� ���� ����

        // btn_LowGroup ������ Ȱ��ȭ/��Ȱ��ȭ �̹��� ���
        GameObject activeImage = lowGroupContainer.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Active").gameObject;
        GameObject deactiveImage = lowGroupContainer.Find($"btn_LowGroup_{hgid}_{lgid}/Img_Deactive").gameObject;

        isActive = !activeImage.activeSelf; // ���� ���¿� ���� ����

        activeImage.SetActive(isActive);
        deactiveImage.SetActive(!isActive);

        // Container_Controller�� �����ϴ� ��� �ڽ� ������Ʈ�� Ȱ��ȭ ���� ���
        foreach (Transform child in lowGroupContainer)
        {
            if (child.name.StartsWith("Container_Controller"))
            {
                child.gameObject.SetActive(isActive);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(sideMenuContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(lowGroupContainer.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(lowGroupContainer.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    // ��� ��Ʈ�ѷ��� ���� ������Ʈ�� ���� �Լ�
    private void UpdateAllControllerStates()
    {
        foreach (var item in uiElements)
        {
            if (item.Key.StartsWith("Container_Controller_"))
            {
                var parts = item.Key.Split('_');
                if (parts.Length >= 4) // Key ������ ���������� Ȯ��
                {
                    string currentIid = parts[2];
                    string currentCid = parts[3];
                    UpdateControllerState(currentIid, currentCid);
                }
            }
        }
    }

    // ���� ��Ʈ�ѷ� ���� ������Ʈ
    private void UpdateControllerState(string iid, string cid)
    {
        Transform controllerContainer = uiElements[$"Container_Controller_{iid}_{cid}"].transform;
        TextMeshProUGUI controllerName = controllerContainer.Find($"btn_Controller_{iid}_{cid}/txt_ControllerName").GetComponent<TextMeshProUGUI>();
        Image controllerStyle = controllerContainer.Find($"btn_ControllerStyle/Img_ControllerStyle").GetComponent<Image>();

        // ���õ� ��Ʈ�ѷ����� Ȯ��
        bool isSelected = (iid == selectedIid && cid == selectedCid);

        // ���� ���¿� ���� ���� ����
        controllerName.color = isSelected ? Color.white : ColorUtility.TryParseHtmlString("#2D2D2D", out var color) ? color : Color.gray;
        controllerStyle.color = isSelected ? Color.white : ColorUtility.TryParseHtmlString("#999999", out var color2) ? color2 : Color.gray;
    }

    public void ResetControllerTextColors()
    {
        if (uiElements.Count > 0)
        {
            foreach (var item in uiElements)
            {
                // Container_Controller_�� �����ϴ� ��� �������� ������� ��
                if (item.Key.StartsWith("Container_Controller_"))
                {
                    // ���õ� ��Ʈ�ѷ��� �����ϰ� ���� ����
                    if (!item.Key.Equals($"Container_Controller_{selectedIid}_{selectedCid}"))
                    {
                        Transform controllerContainer = item.Value.transform;
                        TextMeshProUGUI controllerName = controllerContainer.Find($"btn_Controller_{controllerContainer.name.Split('_')[2]}_{controllerContainer.name.Split('_')[3]}/txt_ControllerName").GetComponent<TextMeshProUGUI>();
                        Color color;
                        if (ColorUtility.TryParseHtmlString("#2D2D2D", out color))
                        {
                            controllerName.color = color;
                        }
                    }
                }
            }
            // ���� ���� �ʱ�ȭ
            selectedIid = null;
            selectedCid = null;
        }
    }
}