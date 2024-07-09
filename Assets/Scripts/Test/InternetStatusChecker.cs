using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEditor;

public class InternetStatusChecker : MonoBehaviour
{
    private bool isEthernetConnected = false;
    private bool isWifiOn = false;
    private string ssid = string.Empty;
    private int rssi = 0;
    private int level = 0;
    private WaitForSeconds sec3 = new WaitForSeconds(3f);
    private Coroutine rxCoroutine = null;
    private Coroutine txCoroutine = null;

    public Image imgInternet;
    public Sprite imgEthernetOn;
    public Sprite imgNoInternet;
    public Sprite imgWifi_1;
    public Sprite imgWifi_2;
    public Sprite imgWifi_3;
    public Sprite imgWifi_4;
    public Sprite imgWifi_5;

    private bool togglingActive = false;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => StartCoroutine(CheckInternetStatus()));
            StartRandomToggleGPIO();
        }
    }

    void StartRandomToggleGPIO()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!togglingActive)
            {
                if (rxCoroutine == null)
                    rxCoroutine = StartCoroutine(ToggleGPIORxRandomly());
                if (txCoroutine == null)
                    txCoroutine = StartCoroutine(ToggleGPIOTxRandomly());
            }
        }        
    }

    IEnumerator ToggleGPIOTxRandomly()
    {
        togglingActive = true;
        while (togglingActive)
        {
            if (isEthernetConnected || rssi > -115)
            {
                GPIOManager.Instance.WriteGPIO(2, true);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.15f)); // 0.1초에서 1초 사이의 랜덤한 시간 동안 대기

                GPIOManager.Instance.WriteGPIO(2, false);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.15f)); // 0.1초에서 1초 사이의 랜덤한 시간 동안 대기
            }
            else
            {
                GPIOManager.Instance.WriteGPIO(2, true);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ToggleGPIORxRandomly()
    {
        togglingActive = true;
        while (togglingActive)
        {
            if (isEthernetConnected || rssi > -115)
            {
                GPIOManager.Instance.WriteGPIO(3, true);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.15f)); // 0.1초에서 1초 사이의 랜덤한 시간 동안 대기

                GPIOManager.Instance.WriteGPIO(3, false);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.15f)); // 0.1초에서 1초 사이의 랜덤한 시간 동안 대기
            }
            else
            {
                GPIOManager.Instance.WriteGPIO(3, true);
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    void StopRandomToggleGPIO()
    {
        if (togglingActive)
        {
            if (txCoroutine != null)
                StopCoroutine(ToggleGPIOTxRandomly());
            txCoroutine = null;
            if (rxCoroutine != null)
                StopCoroutine(ToggleGPIORxRandomly());
            rxCoroutine = null;
            GPIOManager.Instance.WriteGPIO(2, true); // 종료 시 핀 상태를 false로 설정
            GPIOManager.Instance.WriteGPIO(3, true); // 종료 시 핀 상태를 false로 설정
            togglingActive = false;
        }
    }

    IEnumerator CheckInternetStatus()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Input.location.Start();
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaClass pluginClass = new AndroidJavaClass("com.systronics.plugin.EthernetStatusPlugin");
            AndroidJavaClass wifiStatusClass = new AndroidJavaClass("com.systronics.plugin.WiFiStatus");

            while (true)
            {
                isEthernetConnected = pluginClass.CallStatic<bool>("isEthernetConnected", currentActivity);
                isWifiOn = wifiStatusClass.CallStatic<bool>("getWiFiOn", currentActivity);
                ssid = wifiStatusClass.CallStatic<string>("getWiFiSSID", currentActivity);
                rssi = wifiStatusClass.CallStatic<int>("getWiFiRSSI", currentActivity);
                level = GetSignalLevel(rssi);

                //Debug.Log($"isEthernetConnected:{isEthernetConnected}, isWifiOn: {isWifiOn}, ssid:{ssid}, rssi:{rssi}, level:{level}");

                if (isEthernetConnected)
                {
                    if (rssi < -115)
                    {
                        imgInternet.sprite = imgEthernetOn;
                    }
                    else
                    {
                        imgInternet.sprite = imgEthernetOn;
                    }
                }
                else
                {
                    if (rssi < -115)
                    {
                        imgInternet.sprite = imgNoInternet;
                    }
                    else
                    {
                        switch (level)
                        {
                            case 1:
                                imgInternet.sprite = imgWifi_1;
                                break;
                            case 2:
                                imgInternet.sprite = imgWifi_2;
                                break;
                            case 3:
                                imgInternet.sprite = imgWifi_3;
                                break;
                            case 4:
                                imgInternet.sprite = imgWifi_4;
                                break;
                            case 5:
                                imgInternet.sprite = imgWifi_5;
                                break;
                        }
                    }
                }

                // 1초마다 체크. 필요에 따라 시간 조절 가능
                yield return sec3;
            }
        }
    }

    private int GetSignalLevel(int rssi)
    {
        // 리턴값이 높을 수록 수신감도가 나쁨
        if (rssi >= -40) return 1;
        if (rssi >= -60) return 2;
        if (rssi >= -70) return 3;
        if (rssi >= -100) return 4;
        return 5;
    }

    private void OnApplicationQuit()
    {
        StopRandomToggleGPIO();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopRandomToggleGPIO();  // 앱이 백그라운드로 이동할 때 LED 끄기
        }
        else
        {
            StartRandomToggleGPIO();  // 앱이 포커스를 다시 얻을 때 LED 켜기
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            StartRandomToggleGPIO();  // 앱이 포커스를 다시 얻을 때 LED 켜기
        }
        else
        {
            StopRandomToggleGPIO();  // 앱이 포커스를 잃을 때 LED 끄기
        }
    }
}