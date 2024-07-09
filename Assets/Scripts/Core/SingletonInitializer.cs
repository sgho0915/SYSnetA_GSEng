using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonInitializer : MonoBehaviour
{
    void Awake()
    {
        //로딩 단계에서 모든 싱글톤 인스턴스가 생성되므로, 게임 중에 추가적인 싱글톤 생성으로 인한 프레임 드랍이나 지연을 방지
        var infoManagerInstance = InformationManager.Instance;
        var settingManagerInstance = SettingManager.Instance;
        var firstStartManagerInstance = FirstStartManager.Instance;
        var configManagerInstance = ConfigManager.Instance;
        var manufacturingDataInstance = ManufacturingData.Instance;
        var xmlParserInstance = XMLParser.Instance;
        var screenManagerInstance = ScreenManager.Instance;
        var objectPoolInstance = ObjectPool.Instance;        
        var filterManagerInstance = FilterManager.Instance;
        var sideMenuManagerInstance = SideMenuManager.Instance;
        var detailViewInstance = DetailView.Instance;
        var groupManagerInstance = GroupSettingManager.Instance;
        var InterfaceManagerInstance = InterfaceSettingManager.Instance;
        var controlManagerInstance = ControlManager.Instance;
        var alarmPopUpManagerInstance = AlarmPopUpManager.Instance;
        var floorPlanManager = FloorPlanManager.Instance;
    }
}