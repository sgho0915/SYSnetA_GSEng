using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ObjectPool : MonoBehaviour
{
    [Header("����ȭ�� ��Ʈ�ѷ� �ν��Ͻ�")]
    public Transform obj_ControllerScrollViewGridContent; // ��Ʈ�ѷ� �ν��Ͻ� ��ũ�Ѻ��� Content
    public GameObject controllerPrefab_Grid; // ��Ʈ�ѷ� ��ü ������ (Grid)
    public GameObject controllerPrefab_List; // ��Ʈ�ѷ� ��ü ������ (List)
    public GameObject trendItemPrefab_Grid; // ��Ʈ�ѷ� �ν��Ͻ��� Ʈ���� ������ (Grid)
    public GameObject trendItemPrefab_List; // ��Ʈ�ѷ� �ν��Ͻ��� Ʈ���� ������ (List)

    [Header("���� ��� �ν��Ͻ�")]
    public GameObject filterTogglePrefab; // ���� ����� toggle ������
    public GameObject filterSelectedPrefab; // ���õ� ���� ǥ�� ������

    [Header("���̵� �޴� �ν��Ͻ�")]
    public GameObject hgContainerPrefab; // ���̵� �޴� �����׷� ������
    public GameObject lgContainerPrefab; // ���̵� �޴� �����׷� ������
    public GameObject controllerContainerPrefab; // ���̵� �޴� �����׷� ������

    [Header("���� ����ǥ��")]
    public GameObject mainStyleSetControllerPrefab; // ���� ���� ǥ�� ��Ʈ�ѷ� ������
    public Transform obj_MainStyleSetControllerScrollViewContent; // ���� ���� ǥ�� ��Ʈ�ѷ� �ν��Ͻ� ��ũ�Ѻ��� Content

    [Header("������ �� �ν��Ͻ�")]
    public GameObject dvTrendNormalWidget; // �����Ϻ� Center Ʈ���� �Ϲ� ����    
    public GameObject dvTrendAOWidget; // �����Ϻ� Center Ʈ���� AO ����    
    public GameObject dvGraphElement; // �����Ϻ� �׷��� ���
    public GameObject dvGroupElement; // �����Ϻ� group �±� ���
    public GameObject dvSetElement_Numpad; // �����Ϻ� group �±� ���� ���(tag �±�)
    public GameObject dvSetElement_Dropdown; // �����Ϻ� group �±� ���� ���(tag �±�)
    public GameObject dvSetElement_Toggle; // �����Ϻ� group �±� ���� ���(tag �±�)
    public GameObject dvSetElement_Timepicker; // �����Ϻ� group �±� ���� ���(tag �±�)

    [Header("�׷켳�� �ν��Ͻ�")]
    public GameObject f_groupSettingHGContainerPrefab; // ù ���� �� �׷켳�� �����׷� ������
    public GameObject f_groupSettingLGContainerPrefab; // ù ���� �� �׷켳�� �����׷� ������
    public GameObject groupSettingHGContainerPrefab; // �׷켳�� �����׷� ������
    public GameObject groupSettingLGContainerPrefab; // �׷켳�� �����׷� ������

    [Header("�������̽� ���� �ν��Ͻ�")]
    public GameObject f_interfaceContainerPrefab; // ù ���� �� �������̽� ������
    public GameObject interfaceContainerPrefab; // �������̽� ������

    [Header("��Ʈ�ѷ� ���� �ν��Ͻ�")]
    public GameObject f_controllerSetContainerPrefab; // ù ���� �� ��Ʈ�ѷ� ������
    public GameObject f_protocolTogglePrefab; // ù ���� �� �������� ��� ������
    public GameObject controllerSetContainerPrefab; // ��Ʈ�ѷ� ������
    public GameObject protocolTogglePrefab; // �������� ��� ������

    [Header("��鵵 �г� ���� �ν��Ͻ�")]
    public GameObject fppTrendTogglePrefab; // ��鵵 �г� ������
    public GameObject fppImagePrefab; // ��鵵 �̹��� ������

    // ����ȭ�� ��Ʈ�ѷ� �ν��Ͻ�
    public List<GameObject> controllerObjects = new List<GameObject>();
    public List<GameObject> trendObjects = new List<GameObject>();

    //�����ߵ�
    private List<GameObject> settingItemObjects = new List<GameObject>(); // �����ߵ�
    private List<GameObject> settingDetailItemObjects = new List<GameObject>(); // �����ߵ�

    // ����
    public List<GameObject> FloorFilterToggleObjects = new List<GameObject>();
    public List<GameObject> GroupFilterToggleObjects = new List<GameObject>();
    public List<GameObject> EquipmentFilterToggleObjects = new List<GameObject>();
    public  List<GameObject> SelectedFilterObjects = new List<GameObject>();

    // ���̵� �޴� �׷�
    public List<GameObject> HighGroupContainerObjects = new List<GameObject>();
    public List<GameObject> LowGroupContainerObjects = new List<GameObject>();
    public List<GameObject> ControllerContainerObjects = new List<GameObject>();

    // ������ ��
    public List<GameObject> DVTrendNormalWidgetObjects = new List<GameObject>();
    public List<GameObject> DVTrendAOWidgetObjects = new List<GameObject>();
    public List<GameObject> DVGraphElementObjects = new List<GameObject>();
    public List<GameObject> DVGroupElementObjects = new List<GameObject>();
    public List<GameObject> DVSetElementObjects = new List<GameObject>();

    // �׷켳�� �׷�
    public List<GameObject> f_GroupSettingHGContainerObjects = new List<GameObject>();
    public List<GameObject> f_GroupSettingLGContainerObjects = new List<GameObject>();
    public List<GameObject> GroupSettingHGContainerObjects = new List<GameObject>();
    public List<GameObject> GroupSettingLGContainerObjects = new List<GameObject>();

    // �������̽� ���� �ν��Ͻ�
    public List<GameObject> f_InterfaceContainerObjects = new List<GameObject>();
    public List<GameObject> InterfaceContainerObjects = new List<GameObject>();

    // ��Ʈ�ѷ� ���� �ν��Ͻ�
    public List<GameObject> f_ControllerSetContainerObjects = new List<GameObject>();
    public List<GameObject> f_ProtocolToggleObjects = new List<GameObject>();
    public List<GameObject> ControllerSetContainerObjects = new List<GameObject>();
    public List<GameObject> ProtocolToggleObjects = new List<GameObject>();

    // ��鵵 Ʈ���� ��� �ν��Ͻ�
    public List<GameObject> fppTrendToggleObjects = new List<GameObject>();

    // ��鵵 �̹��� �ν��Ͻ�
    public List<GameObject> fppImageObjects = new List<GameObject>();

    // ���� ���� ǥ��(��, ��Ʈ�ѷ� ��Ÿ�� ����)
    private List<GameObject> MainStyleSetControllerObjects = new List<GameObject>();

    public static ObjectPool Instance { get; private set; }

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
    }

    // ������ �� �ν��Ͻ� �ʱ�ȭ
    public void CloseDV()
    {
        DVTrendNormalWidgetObjects.Clear();
        DVTrendAOWidgetObjects.Clear();
        DVGraphElementObjects.Clear();
        DVGroupElementObjects.Clear();
        DVSetElementObjects.Clear();
    }

    // ���̵� �޴� �ν��Ͻ� �ʱ�ȭ
    public void CloseSideMenu()
    {
        HighGroupContainerObjects.Clear();
        LowGroupContainerObjects.Clear();
        ControllerContainerObjects.Clear();
    }

    // �׷켳�� �ν��Ͻ� �ʱ�ȭ
    public void CloseGroupSetting()
    {
        GroupSettingHGContainerObjects.Clear();
        GroupSettingLGContainerObjects.Clear();
    }

    // �������̽� ���� �ν��Ͻ� �ʱ�ȭ
    public void CloseInterfaceSetting()
    {
        InterfaceContainerObjects.Clear();        
    }

    // ��Ʈ�ѷ� ���� �ν��Ͻ� �ʱ�ȭ
    public void CloseControllerSetting()
    {
        ControllerSetContainerObjects.Clear();
    }

    // ��Ʈ�ѷ� �߰��� �������� ��� �ν��Ͻ� �ʱ�ȭ
    public void CloseControllerAddSetting()
    {
        ProtocolToggleObjects.Clear();
    }

    // ù ���� �� �׷켳�� �ν��Ͻ� �ʱ�ȭ
    public void f_CloseGroupSetting()
    {
        f_GroupSettingHGContainerObjects.Clear();
        f_GroupSettingLGContainerObjects.Clear();
    }

    // ù ���� �� �������̽� ���� �ν��Ͻ� �ʱ�ȭ
    public void f_CloseInterfaceSetting()
    {
        f_InterfaceContainerObjects.Clear();
    }

    // ù ���� �� ��Ʈ�ѷ� ���� �ν��Ͻ� �ʱ�ȭ
    public void f_CloseControllerSetting()
    {
        f_ControllerSetContainerObjects.Clear();
    }

    // ù ���� �� ��Ʈ�ѷ� �߰��� �������� ��� �ν��Ͻ� �ʱ�ȭ
    public void f_CloseControllerAddSetting()
    {
        f_ProtocolToggleObjects.Clear();
    }

    // FPP Ʈ���� ��� �ν��Ͻ� �ʱ�ȭ
    public void CloseFPPTrendToggleSetting()
    {
        fppTrendToggleObjects.Clear();        
    }

    // ��鵵 �̹��� �ν��Ͻ� ����
    public GameObject GetFPPImageObject()
    {
        foreach (var obj in fppImageObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(fppImagePrefab, FloorPlanManager.Instance.fpContent);
        fppImageObjects.Add(newObj);
        return newObj;
    }


    // ù ���� �� �������� �����̳� ����
    public GameObject f_GetProtocolToggleObject()
    {
        foreach (var obj in f_ProtocolToggleObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(f_protocolTogglePrefab, ControllerSettingManager.f_protocolListContent);
        f_ProtocolToggleObjects.Add(newObj);
        return newObj;
    }

    // ù ���� �� ��Ʈ�ѷ� �����̳� ����
    public GameObject f_GetControllerSetContainerObject()
    {
        foreach (var obj in f_ControllerSetContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(f_controllerSetContainerPrefab, ControllerSettingManager.f_controllerListContent);
        f_ControllerSetContainerObjects.Add(newObj);
        return newObj;
    }

    // ù ���� �� �������̽� �����̳� ����
    public GameObject f_GetInterfaceContainerObject()
    {
        foreach (var obj in f_InterfaceContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(f_interfaceContainerPrefab, InterfaceSettingManager.f_interfaceListContent);
        f_InterfaceContainerObjects.Add(newObj);
        return newObj;
    }

    // ù ���� �� �׷켳�� �����׷� �����̳� ����
    public GameObject f_GetGroupSettingHGContainerObject()
    {
        foreach (var obj in f_GroupSettingHGContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(f_groupSettingHGContainerPrefab, GroupSettingManager.f_groupListContent);
        f_GroupSettingHGContainerObjects.Add(newObj);
        return newObj;
    }

    // ù ���� �� �׷켳�� �����׷� �����̳� ����
    public GameObject f_GetGroupSettingLGContainerObject()
    {
        foreach (var obj in f_GroupSettingLGContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(f_groupSettingLGContainerPrefab, GroupSettingManager.f_hgContainerTransform);
        f_GroupSettingLGContainerObjects.Add(newObj);
        return newObj;
    }

    // �������� �����̳� ����
    public GameObject GetFPPTrendToggleObject()
    {
        foreach (var obj in fppTrendToggleObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(fppTrendTogglePrefab, FloorPlanManager.Instance.trendListContent);
        fppTrendToggleObjects.Add(newObj);
        return newObj;
    }

    // �������� �����̳� ����
    public GameObject GetProtocolToggleObject()
    {
        foreach (var obj in ProtocolToggleObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(protocolTogglePrefab, ControllerSettingManager.protocolListContent);
        ProtocolToggleObjects.Add(newObj);
        return newObj;
    }

    // ��Ʈ�ѷ� �����̳� ����
    public GameObject GetControllerSetContainerObject()
    {
        foreach (var obj in ControllerSetContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(controllerSetContainerPrefab, ControllerSettingManager.controllerListContent);
        ControllerSetContainerObjects.Add(newObj);
        return newObj;
    }

    // �������̽� �����̳� ����
    public GameObject GetInterfaceContainerObject()
    {
        foreach (var obj in InterfaceContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(interfaceContainerPrefab, InterfaceSettingManager.interfaceListContent);
        InterfaceContainerObjects.Add(newObj);
        return newObj;
    }

    // �׷켳�� �����׷� �����̳� ����
    public GameObject GetGroupSettingHGContainerObject()
    {
        foreach (var obj in GroupSettingHGContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(groupSettingHGContainerPrefab, GroupSettingManager.groupListContent);
        GroupSettingHGContainerObjects.Add(newObj);
        return newObj;
    }

    // �׷켳�� �����׷� �����̳� ����
    public GameObject GetGroupSettingLGContainerObject()
    {
        foreach (var obj in GroupSettingLGContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(groupSettingLGContainerPrefab, GroupSettingManager.hgContainerTransform);
        GroupSettingLGContainerObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� Ʈ���� �Ϲ� ���� �ν��Ͻ� ����
    public GameObject GetDVTrendNormalWidgetObject()
    {
        foreach (var obj in DVTrendNormalWidgetObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvTrendNormalWidget, DetailView.Instance.mainWidgetScrollViewContent);
        DVTrendNormalWidgetObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� Ʈ���� AO ���� �ν��Ͻ� ����
    public GameObject GetDVTrendAOWidgetObject()
    {
        foreach (var obj in DVTrendAOWidgetObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvTrendAOWidget, DetailView.Instance.aoWidgetScrollViewContent);
        DVTrendAOWidgetObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� �׷��� ��� ��ư �ν��Ͻ� ����
    public GameObject GetDVGraphElementObject()
    {
        foreach (var obj in DVGraphElementObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvGraphElement, DetailView.Instance.graphElementScrollViewContent);
        DVGraphElementObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� �ϴܸ޴� group �±� ��ư �ν��Ͻ� ����
    public GameObject GetDVGroupElementObject()
    {
        foreach (var obj in DVGroupElementObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvGroupElement, DetailView.Instance.groupElementScrollViewContent);
        DVGroupElementObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� �ϴܸ޴� group �±� ��ư �ν��Ͻ� ����(NumpadŸ��)
    public GameObject GetDVSetElementObject_Numpad()
    {
        foreach (var obj in DVSetElementObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvSetElement_Numpad, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� �ϴܸ޴� group �±� ��ư �ν��Ͻ� ����(DropdownŸ��)
    public GameObject GetDVSetElementObject_Dropdown()
    {
        foreach (var obj in DVSetElementObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvSetElement_Dropdown, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� �ϴܸ޴� group �±� ��ư �ν��Ͻ� ����(ToggleŸ��)
    public GameObject GetDVSetElementObject_Toggle()
    {
        foreach (var obj in DVSetElementObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvSetElement_Toggle, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // �����Ϻ� �ϴܸ޴� group �±� ��ư �ν��Ͻ� ����(TimePickerŸ��)
    public GameObject GetDVSetElementObject_TimePicker()
    {
        foreach (var obj in DVSetElementObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(dvSetElement_Timepicker, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // ���̵�޴� �����׷� �����̳� ����
    public GameObject GetHighGroupContainerObject()
    {
        foreach (var obj in HighGroupContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(hgContainerPrefab, SideMenuManager.Instance.sideMenuContent);
        HighGroupContainerObjects.Add(newObj);
        return newObj;
    }

    // ���̵�޴� �����׷� �����̳� ����
    public GameObject GetLowGroupContainerObject()
    {
        foreach (var obj in LowGroupContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(lgContainerPrefab, SideMenuManager.Instance.hgContainerTransform);
        LowGroupContainerObjects.Add(newObj);
        return newObj;
    }

    // ���̵�޴� ��Ʈ�ѷ� �����̳� ����
    public GameObject GetControllerContainerObject()
    {
        foreach (var obj in ControllerContainerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(controllerContainerPrefab, SideMenuManager.Instance.lgContainerTransform);
        ControllerContainerObjects.Add(newObj);
        return newObj;
    }

    // ���� ��Ÿ�� ���� ��Ʈ�ѷ� �ν��Ͻ� ����
    public GameObject GetMainStyleSetControllerObject()
    {
        foreach (var obj in MainStyleSetControllerObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(mainStyleSetControllerPrefab, obj_MainStyleSetControllerScrollViewContent);
        MainStyleSetControllerObjects.Add(newObj);
        return newObj;
    }

    // ���õ� ���� �׸� ���� ������ ������Ʈ Ǯ��
    public GameObject GetSelectedFilterObject(string filterId, string filterName)
    {
        if (FilterManager.Instance.currentCheckedNames.Contains(filterName))
        {
            return null; // �̹� �����ϴ� ��� �� ��ü ������ ����
        }

        foreach (var obj in SelectedFilterObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                FilterManager.Instance.currentCheckedNames.Add(filterName); // ���� �̸��� HashSet�� �߰�

                // ���� ���� ��ư�� �̺�Ʈ ������ �߰�
                Button deleteButton = obj.transform.Find("btn_ExceptionFilter").GetComponent<Button>();
                deleteButton.onClick.AddListener(() => RemoveFilter(filterName, obj));

                return obj;
            }
        }

        GameObject newObj = Instantiate(filterSelectedPrefab, FilterManager.selectedFilterContent);
        newObj.name = $"Filter_Selected_{filterId}_{filterName}";
        SelectedFilterObjects.Add(newObj);
        FilterManager.Instance.currentCheckedNames.Add(filterName); // ���� ������ ��� ���� �̸��� HashSet�� �߰�

        // �� ��ü�� ���� ���� ���� ��ư �̺�Ʈ ������ �߰�
        Button newDeleteButton = newObj.transform.Find("btn_ExceptionFilter").GetComponent<Button>();
        newDeleteButton.onClick.AddListener(() => RemoveFilter(filterName, newObj));

        return newObj;
    }

    // ���õ� ���� �׸� ����
    public void RemoveFilter(string filterName, GameObject filterObject)
    {
        filterObject.SetActive(false); // ���� ��ü ��Ȱ��ȭ
        FilterManager.Instance.currentCheckedNames.Remove(filterName); // ���� �̸� HashSet���� ����
        //SelectedFilterObjects.Remove(filterObject);

        // ���� ��� �� ��Ʈ�ѷ� �ν��Ͻ� ������Ʈ
        if (!FilterManager.Instance.isUpdatingFilters)
        {
            FilterManager.Instance.UpdateFiltersAndRefreshControllers();
        }
    }

    // ���� �����׷�(��) ��� �ν��Ͻ� ����
    public GameObject GetFloorFilterToggleObject()
    {
        foreach (var obj in FloorFilterToggleObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(filterTogglePrefab, FilterManager.floorContent);
        FloorFilterToggleObjects.Add(newObj);
        return newObj;
    }

    // ���� �����׷�(�׷�) ��� �ν��Ͻ� ����
    public GameObject GetGroupFilterToggleObject()
    {
        foreach (var obj in GroupFilterToggleObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(filterTogglePrefab, FilterManager.groupContent);
        GroupFilterToggleObjects.Add(newObj);
        return newObj;
    }

    // ���� ���(��Ʈ�ѷ�) ��� �ν��Ͻ� ����
    public GameObject GetEquipmentFilterToggleObject()
    {
        foreach (var obj in EquipmentFilterToggleObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(filterTogglePrefab, FilterManager.equipmentContent);
        EquipmentFilterToggleObjects.Add(newObj);
        return newObj;
    }

    // ��Ʈ�ѷ� �ν��Ͻ� Ʈ���� Grid ������ ����
    public GameObject GetTrendGridObject()
    {
        foreach (var obj in trendObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(trendItemPrefab_Grid, ClientDatabase.controllerTrendContentGrid);
        trendObjects.Add(newObj);
        return newObj;
    }

    // ��Ʈ�ѷ� �ν��Ͻ� Ʈ���� List ������ ����
    public GameObject GetTrendListObject()
    {
        foreach (var obj in trendObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        GameObject newObj = Instantiate(trendItemPrefab_List, ClientDatabase.controllerTrendContentList);
        trendObjects.Add(newObj);
        return newObj;
    }

    // ��Ʈ�ѷ� Grid �ν��Ͻ� ����
    public GameObject GetControllerGridObject()
    {
        // �������� ����̽��� ���� ������ �������� Instantiate��
        if (ScreenManager.isTablet)
        {
            foreach (var obj in controllerObjects)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // �ʿ��ϸ� �߰� ������Ʈ ����
            GameObject newObj = Instantiate(controllerPrefab_Grid, ClientDatabase.controllerContentGrid);
            controllerObjects.Add(newObj);
            return newObj;
        }
        else
        {
            foreach (var obj in controllerObjects)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // �ʿ��ϸ� �߰� ������Ʈ ����
            GameObject newObj = Instantiate(controllerPrefab_Grid, ClientDatabase.controllerContentGrid);
            controllerObjects.Add(newObj);
            return newObj;
        }
    }

    // ��Ʈ�ѷ� List �ν��Ͻ� ����
    public GameObject GetControllerListObject()
    {
        // �������� ����̽��� ���� ������ �������� Instantiate��
        if (ScreenManager.isTablet)
        {
            foreach (var obj in controllerObjects)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // �ʿ��ϸ� �߰� ������Ʈ ����
            GameObject newObj = Instantiate(controllerPrefab_List, ClientDatabase.controllerContentList);
            controllerObjects.Add(newObj);
            return newObj;
        }
        else
        {
            foreach (var obj in controllerObjects)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            // �ʿ��ϸ� �߰� ������Ʈ ����
            GameObject newObj = Instantiate(controllerPrefab_List, ClientDatabase.controllerContentList);
            controllerObjects.Add(newObj);
            return newObj;
        }
    }

    public GameObject GetSettingItemObject()
    {
        foreach (var obj in settingItemObjects)
        {
            if (!obj.activeSelf && obj.name.StartsWith("btn_SettingItem"))
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        //GameObject newObj = Instantiate(settingItemPrefab, obj_ContentSettingList);
        //settingItemObjects.Add(newObj);        
        //return newObj;
        return null;
    }

    public GameObject GetSettingDetailItemObject()
    {
        ReturnSettingItemObject();
        foreach (var obj in settingDetailItemObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                // �ʱ� �̸� �Ҵ� �Ǵ� ���� �̸� ����
                if (!obj.name.StartsWith("btn_SettingDetailItem_"))
                {
                    obj.name = "btn_SettingDetailItem_";
                }
                return obj;
            }
            else
            {

            }
        }

        // �ʿ��ϸ� �߰� ������Ʈ ����
        //GameObject newObj = Instantiate(settingDetailItemPrefab, obj_ContentSettingList);
        //settingDetailItemObjects.Add(newObj);
        //newObj.name = "btn_SettingDetailItem_"; // �� ��ü�� �ʱ� �̸� �Ҵ�        
        //newObj.SetActive(true);
        //return newObj;
        return null;
    }

    public void ReturnConverterObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ReturnControllerObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ReturnSettingItemObject()
    {
        foreach (var item in settingItemObjects)
        {
            if (item.activeSelf && item.name.StartsWith("btn_SettingItem_"))
            {                
                item.SetActive(false);
            }
        }        
    }

    public void ReactivationSettingItemObject()
    {
        foreach (var item in settingItemObjects)
        {
            if (!item.activeSelf && item.name.StartsWith("btn_SettingItem_"))
            {
                item.SetActive(true);
            }
        }
    }

    public void ReturnSettingDetailItemObject()
    {
        foreach (var item in settingDetailItemObjects)
        {
            if (item.activeSelf && item.name.StartsWith("btn_SettingDetailItem_"))
            {
                item.SetActive(false);
                item.name = "btn_SettingDetailItem_"; // �̸� �ʱ�ȭ
            }
        }
    }
}

