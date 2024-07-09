using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ObjectPool : MonoBehaviour
{
    [Header("메인화면 컨트롤러 인스턴스")]
    public Transform obj_ControllerScrollViewGridContent; // 컨트롤러 인스턴스 스크롤뷰의 Content
    public GameObject controllerPrefab_Grid; // 컨트롤러 객체 프리팹 (Grid)
    public GameObject controllerPrefab_List; // 컨트롤러 객체 프리팹 (List)
    public GameObject trendItemPrefab_Grid; // 컨트롤러 인스턴스의 트렌드 프리팹 (Grid)
    public GameObject trendItemPrefab_List; // 컨트롤러 인스턴스의 트렌드 프리팹 (List)

    [Header("필터 토글 인스턴스")]
    public GameObject filterTogglePrefab; // 필터 기능의 toggle 프리팹
    public GameObject filterSelectedPrefab; // 선택된 필터 표시 프리팹

    [Header("사이드 메뉴 인스턴스")]
    public GameObject hgContainerPrefab; // 사이드 메뉴 상위그룹 프리팹
    public GameObject lgContainerPrefab; // 사이드 메뉴 하위그룹 프리팹
    public GameObject controllerContainerPrefab; // 사이드 메뉴 하위그룹 프리팹

    [Header("메인 상태표시")]
    public GameObject mainStyleSetControllerPrefab; // 메인 상태 표시 컨트롤러 프리팹
    public Transform obj_MainStyleSetControllerScrollViewContent; // 메인 상태 표시 컨트롤러 인스턴스 스크롤뷰의 Content

    [Header("디테일 뷰 인스턴스")]
    public GameObject dvTrendNormalWidget; // 디테일뷰 Center 트렌드 일반 위젯    
    public GameObject dvTrendAOWidget; // 디테일뷰 Center 트렌드 AO 위젯    
    public GameObject dvGraphElement; // 디테일뷰 그래프 요소
    public GameObject dvGroupElement; // 디테일뷰 group 태그 요소
    public GameObject dvSetElement_Numpad; // 디테일뷰 group 태그 설정 요소(tag 태그)
    public GameObject dvSetElement_Dropdown; // 디테일뷰 group 태그 설정 요소(tag 태그)
    public GameObject dvSetElement_Toggle; // 디테일뷰 group 태그 설정 요소(tag 태그)
    public GameObject dvSetElement_Timepicker; // 디테일뷰 group 태그 설정 요소(tag 태그)

    [Header("그룹설정 인스턴스")]
    public GameObject f_groupSettingHGContainerPrefab; // 첫 설정 시 그룹설정 상위그룹 프리팹
    public GameObject f_groupSettingLGContainerPrefab; // 첫 설정 시 그룹설정 하위그룹 프리팹
    public GameObject groupSettingHGContainerPrefab; // 그룹설정 상위그룹 프리팹
    public GameObject groupSettingLGContainerPrefab; // 그룹설정 하위그룹 프리팹

    [Header("인터페이스 설정 인스턴스")]
    public GameObject f_interfaceContainerPrefab; // 첫 설정 시 인터페이스 프리팹
    public GameObject interfaceContainerPrefab; // 인터페이스 프리팹

    [Header("컨트롤러 설정 인스턴스")]
    public GameObject f_controllerSetContainerPrefab; // 첫 설정 시 컨트롤러 프리팹
    public GameObject f_protocolTogglePrefab; // 첫 설정 시 프로토콜 토글 프리팹
    public GameObject controllerSetContainerPrefab; // 컨트롤러 프리팹
    public GameObject protocolTogglePrefab; // 프로토콜 토글 프리팹

    [Header("평면도 패널 설정 인스턴스")]
    public GameObject fppTrendTogglePrefab; // 평면도 패널 프리팹
    public GameObject fppImagePrefab; // 평면도 이미지 프리팹

    // 메인화면 컨트롤러 인스턴스
    public List<GameObject> controllerObjects = new List<GameObject>();
    public List<GameObject> trendObjects = new List<GameObject>();

    //지워야됨
    private List<GameObject> settingItemObjects = new List<GameObject>(); // 지워야됨
    private List<GameObject> settingDetailItemObjects = new List<GameObject>(); // 지워야됨

    // 필터
    public List<GameObject> FloorFilterToggleObjects = new List<GameObject>();
    public List<GameObject> GroupFilterToggleObjects = new List<GameObject>();
    public List<GameObject> EquipmentFilterToggleObjects = new List<GameObject>();
    public  List<GameObject> SelectedFilterObjects = new List<GameObject>();

    // 사이드 메뉴 그룹
    public List<GameObject> HighGroupContainerObjects = new List<GameObject>();
    public List<GameObject> LowGroupContainerObjects = new List<GameObject>();
    public List<GameObject> ControllerContainerObjects = new List<GameObject>();

    // 디테일 뷰
    public List<GameObject> DVTrendNormalWidgetObjects = new List<GameObject>();
    public List<GameObject> DVTrendAOWidgetObjects = new List<GameObject>();
    public List<GameObject> DVGraphElementObjects = new List<GameObject>();
    public List<GameObject> DVGroupElementObjects = new List<GameObject>();
    public List<GameObject> DVSetElementObjects = new List<GameObject>();

    // 그룹설정 그룹
    public List<GameObject> f_GroupSettingHGContainerObjects = new List<GameObject>();
    public List<GameObject> f_GroupSettingLGContainerObjects = new List<GameObject>();
    public List<GameObject> GroupSettingHGContainerObjects = new List<GameObject>();
    public List<GameObject> GroupSettingLGContainerObjects = new List<GameObject>();

    // 인터페이스 설정 인스턴스
    public List<GameObject> f_InterfaceContainerObjects = new List<GameObject>();
    public List<GameObject> InterfaceContainerObjects = new List<GameObject>();

    // 컨트롤러 설정 인스턴스
    public List<GameObject> f_ControllerSetContainerObjects = new List<GameObject>();
    public List<GameObject> f_ProtocolToggleObjects = new List<GameObject>();
    public List<GameObject> ControllerSetContainerObjects = new List<GameObject>();
    public List<GameObject> ProtocolToggleObjects = new List<GameObject>();

    // 평면도 트렌드 토글 인스턴스
    public List<GameObject> fppTrendToggleObjects = new List<GameObject>();

    // 평면도 이미지 인스턴스
    public List<GameObject> fppImageObjects = new List<GameObject>();

    // 메인 상태 표시(뷰, 컨트롤러 스타일 설정)
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

    // 디테일 뷰 인스턴스 초기화
    public void CloseDV()
    {
        DVTrendNormalWidgetObjects.Clear();
        DVTrendAOWidgetObjects.Clear();
        DVGraphElementObjects.Clear();
        DVGroupElementObjects.Clear();
        DVSetElementObjects.Clear();
    }

    // 사이드 메뉴 인스턴스 초기화
    public void CloseSideMenu()
    {
        HighGroupContainerObjects.Clear();
        LowGroupContainerObjects.Clear();
        ControllerContainerObjects.Clear();
    }

    // 그룹설정 인스턴스 초기화
    public void CloseGroupSetting()
    {
        GroupSettingHGContainerObjects.Clear();
        GroupSettingLGContainerObjects.Clear();
    }

    // 인터페이스 설정 인스턴스 초기화
    public void CloseInterfaceSetting()
    {
        InterfaceContainerObjects.Clear();        
    }

    // 컨트롤러 설정 인스턴스 초기화
    public void CloseControllerSetting()
    {
        ControllerSetContainerObjects.Clear();
    }

    // 컨트롤러 추가시 프로토콜 토글 인스턴스 초기화
    public void CloseControllerAddSetting()
    {
        ProtocolToggleObjects.Clear();
    }

    // 첫 실행 시 그룹설정 인스턴스 초기화
    public void f_CloseGroupSetting()
    {
        f_GroupSettingHGContainerObjects.Clear();
        f_GroupSettingLGContainerObjects.Clear();
    }

    // 첫 실행 시 인터페이스 설정 인스턴스 초기화
    public void f_CloseInterfaceSetting()
    {
        f_InterfaceContainerObjects.Clear();
    }

    // 첫 실행 시 컨트롤러 설정 인스턴스 초기화
    public void f_CloseControllerSetting()
    {
        f_ControllerSetContainerObjects.Clear();
    }

    // 첫 실행 시 컨트롤러 추가시 프로토콜 토글 인스턴스 초기화
    public void f_CloseControllerAddSetting()
    {
        f_ProtocolToggleObjects.Clear();
    }

    // FPP 트렌드 토글 인스턴스 초기화
    public void CloseFPPTrendToggleSetting()
    {
        fppTrendToggleObjects.Clear();        
    }

    // 평면도 이미지 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(fppImagePrefab, FloorPlanManager.Instance.fpContent);
        fppImageObjects.Add(newObj);
        return newObj;
    }


    // 첫 실행 시 프로토콜 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(f_protocolTogglePrefab, ControllerSettingManager.f_protocolListContent);
        f_ProtocolToggleObjects.Add(newObj);
        return newObj;
    }

    // 첫 실행 시 컨트롤러 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(f_controllerSetContainerPrefab, ControllerSettingManager.f_controllerListContent);
        f_ControllerSetContainerObjects.Add(newObj);
        return newObj;
    }

    // 첫 실행 시 인터페이스 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(f_interfaceContainerPrefab, InterfaceSettingManager.f_interfaceListContent);
        f_InterfaceContainerObjects.Add(newObj);
        return newObj;
    }

    // 첫 실행 시 그룹설정 상위그룹 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(f_groupSettingHGContainerPrefab, GroupSettingManager.f_groupListContent);
        f_GroupSettingHGContainerObjects.Add(newObj);
        return newObj;
    }

    // 첫 실행 시 그룹설정 하위그룹 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(f_groupSettingLGContainerPrefab, GroupSettingManager.f_hgContainerTransform);
        f_GroupSettingLGContainerObjects.Add(newObj);
        return newObj;
    }

    // 프로토콜 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(fppTrendTogglePrefab, FloorPlanManager.Instance.trendListContent);
        fppTrendToggleObjects.Add(newObj);
        return newObj;
    }

    // 프로토콜 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(protocolTogglePrefab, ControllerSettingManager.protocolListContent);
        ProtocolToggleObjects.Add(newObj);
        return newObj;
    }

    // 컨트롤러 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(controllerSetContainerPrefab, ControllerSettingManager.controllerListContent);
        ControllerSetContainerObjects.Add(newObj);
        return newObj;
    }

    // 인터페이스 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(interfaceContainerPrefab, InterfaceSettingManager.interfaceListContent);
        InterfaceContainerObjects.Add(newObj);
        return newObj;
    }

    // 그룹설정 상위그룹 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(groupSettingHGContainerPrefab, GroupSettingManager.groupListContent);
        GroupSettingHGContainerObjects.Add(newObj);
        return newObj;
    }

    // 그룹설정 하위그룹 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(groupSettingLGContainerPrefab, GroupSettingManager.hgContainerTransform);
        GroupSettingLGContainerObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 트렌드 일반 위젯 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvTrendNormalWidget, DetailView.Instance.mainWidgetScrollViewContent);
        DVTrendNormalWidgetObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 트렌드 AO 위젯 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvTrendAOWidget, DetailView.Instance.aoWidgetScrollViewContent);
        DVTrendAOWidgetObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 그래프 요소 버튼 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvGraphElement, DetailView.Instance.graphElementScrollViewContent);
        DVGraphElementObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 하단메뉴 group 태그 버튼 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvGroupElement, DetailView.Instance.groupElementScrollViewContent);
        DVGroupElementObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 하단메뉴 group 태그 버튼 인스턴스 생성(Numpad타입)
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvSetElement_Numpad, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 하단메뉴 group 태그 버튼 인스턴스 생성(Dropdown타입)
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvSetElement_Dropdown, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 하단메뉴 group 태그 버튼 인스턴스 생성(Toggle타입)
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvSetElement_Toggle, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // 디테일뷰 하단메뉴 group 태그 버튼 인스턴스 생성(TimePicker타입)
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(dvSetElement_Timepicker, DetailView.Instance.setElementScrollViewContent);
        DVSetElementObjects.Add(newObj);
        return newObj;
    }

    // 사이드메뉴 상위그룹 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(hgContainerPrefab, SideMenuManager.Instance.sideMenuContent);
        HighGroupContainerObjects.Add(newObj);
        return newObj;
    }

    // 사이드메뉴 하위그룹 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(lgContainerPrefab, SideMenuManager.Instance.hgContainerTransform);
        LowGroupContainerObjects.Add(newObj);
        return newObj;
    }

    // 사이드메뉴 컨트롤러 컨테이너 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(controllerContainerPrefab, SideMenuManager.Instance.lgContainerTransform);
        ControllerContainerObjects.Add(newObj);
        return newObj;
    }

    // 메인 스타일 설정 컨트롤러 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(mainStyleSetControllerPrefab, obj_MainStyleSetControllerScrollViewContent);
        MainStyleSetControllerObjects.Add(newObj);
        return newObj;
    }

    // 선택된 필터 항목에 대한 프리팹 오브젝트 풀링
    public GameObject GetSelectedFilterObject(string filterId, string filterName)
    {
        if (FilterManager.Instance.currentCheckedNames.Contains(filterName))
        {
            return null; // 이미 존재하는 경우 새 객체 생성을 방지
        }

        foreach (var obj in SelectedFilterObjects)
        {
            if (!obj.activeSelf)
            {
                obj.SetActive(true);
                FilterManager.Instance.currentCheckedNames.Add(filterName); // 필터 이름을 HashSet에 추가

                // 필터 삭제 버튼에 이벤트 리스너 추가
                Button deleteButton = obj.transform.Find("btn_ExceptionFilter").GetComponent<Button>();
                deleteButton.onClick.AddListener(() => RemoveFilter(filterName, obj));

                return obj;
            }
        }

        GameObject newObj = Instantiate(filterSelectedPrefab, FilterManager.selectedFilterContent);
        newObj.name = $"Filter_Selected_{filterId}_{filterName}";
        SelectedFilterObjects.Add(newObj);
        FilterManager.Instance.currentCheckedNames.Add(filterName); // 새로 생성된 경우 필터 이름을 HashSet에 추가

        // 새 객체에 대한 필터 삭제 버튼 이벤트 리스너 추가
        Button newDeleteButton = newObj.transform.Find("btn_ExceptionFilter").GetComponent<Button>();
        newDeleteButton.onClick.AddListener(() => RemoveFilter(filterName, newObj));

        return newObj;
    }

    // 선택된 필터 항목 제거
    public void RemoveFilter(string filterName, GameObject filterObject)
    {
        filterObject.SetActive(false); // 필터 객체 비활성화
        FilterManager.Instance.currentCheckedNames.Remove(filterName); // 필터 이름 HashSet에서 제거
        //SelectedFilterObjects.Remove(filterObject);

        // 필터 목록 및 컨트롤러 인스턴스 업데이트
        if (!FilterManager.Instance.isUpdatingFilters)
        {
            FilterManager.Instance.UpdateFiltersAndRefreshControllers();
        }
    }

    // 필터 상위그룹(층) 토글 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(filterTogglePrefab, FilterManager.floorContent);
        FloorFilterToggleObjects.Add(newObj);
        return newObj;
    }

    // 필터 하위그룹(그룹) 토글 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(filterTogglePrefab, FilterManager.groupContent);
        GroupFilterToggleObjects.Add(newObj);
        return newObj;
    }

    // 필터 장비(컨트롤러) 토글 인스턴스 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(filterTogglePrefab, FilterManager.equipmentContent);
        EquipmentFilterToggleObjects.Add(newObj);
        return newObj;
    }

    // 컨트롤러 인스턴스 트렌드 Grid 프리팹 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(trendItemPrefab_Grid, ClientDatabase.controllerTrendContentGrid);
        trendObjects.Add(newObj);
        return newObj;
    }

    // 컨트롤러 인스턴스 트렌드 List 프리팹 생성
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

        // 필요하면 추가 오브젝트 생성
        GameObject newObj = Instantiate(trendItemPrefab_List, ClientDatabase.controllerTrendContentList);
        trendObjects.Add(newObj);
        return newObj;
    }

    // 컨트롤러 Grid 인스턴스 생성
    public GameObject GetControllerGridObject()
    {
        // 실행중인 디바이스에 따라 적합한 프리팹이 Instantiate됨
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

            // 필요하면 추가 오브젝트 생성
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

            // 필요하면 추가 오브젝트 생성
            GameObject newObj = Instantiate(controllerPrefab_Grid, ClientDatabase.controllerContentGrid);
            controllerObjects.Add(newObj);
            return newObj;
        }
    }

    // 컨트롤러 List 인스턴스 생성
    public GameObject GetControllerListObject()
    {
        // 실행중인 디바이스에 따라 적합한 프리팹이 Instantiate됨
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

            // 필요하면 추가 오브젝트 생성
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

            // 필요하면 추가 오브젝트 생성
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

        // 필요하면 추가 오브젝트 생성
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
                // 초기 이름 할당 또는 기존 이름 유지
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

        // 필요하면 추가 오브젝트 생성
        //GameObject newObj = Instantiate(settingDetailItemPrefab, obj_ContentSettingList);
        //settingDetailItemObjects.Add(newObj);
        //newObj.name = "btn_SettingDetailItem_"; // 새 객체에 초기 이름 할당        
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
                item.name = "btn_SettingDetailItem_"; // 이름 초기화
            }
        }
    }
}

