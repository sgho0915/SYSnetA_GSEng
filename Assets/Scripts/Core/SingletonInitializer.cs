using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonInitializer : MonoBehaviour
{
    void Awake()
    {
        //�ε� �ܰ迡�� ��� �̱��� �ν��Ͻ��� �����ǹǷ�, ���� �߿� �߰����� �̱��� �������� ���� ������ ����̳� ������ ����
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