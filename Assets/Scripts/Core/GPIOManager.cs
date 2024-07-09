using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.UI;

public class GPIOManager : MonoBehaviour
{
#if UNITY_ANDROID
    AndroidJavaObject gpioControl;
    public TMP_Dropdown dropdown_GPIOPorts;
    private int selectedGPIOPort = 0;

    public GameObject gpioScreen;
    public TextMeshProUGUI txtResult;
    public Button btnGPIOOutOpen;
    public Button btnGPIOOutClose;
    public Button btnGPIORead;
    public Button btnGPIOScreenClose;

    public static GPIOManager Instance { get; private set; }

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

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass pluginClass = new AndroidJavaClass("com.systronics.plugin.GPIOControl");
                gpioControl = new AndroidJavaObject("com.systronics.plugin.GPIOControl", currentActivity);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to initialize GPIOControl: {ex.Message}");
            }

            OpenGPIOScreen();
            btnGPIOScreenClose.onClick.AddListener(() =>
            {
                CloseGPIOScreen();
            });
        }
        
    }

    public int ReadGPIO(int port)
    {
        if (gpioControl != null) // gpioControl이 null이 아닌지 확인
        {
            return gpioControl.Call<int>("readGPIO", port);
        }
        else
        {
            Debug.LogError("ReadGPIO : gpioControl is not initialized.");
            return -1; // 적절한 오류 코드 반환
        }
    }

    public int WriteGPIO(int port, bool value)
    {
        // Note: `writeGPIO` 메소드는 `boolean` 타입을 두 번째 인자로 사용
        // `int value`를 `boolean`으로 변환. 0이면 `false`, 그 외에는 `true`로 변환.
               
        if (gpioControl != null) // gpioControl이 null이 아닌지 확인
        {
            return gpioControl.Call<int>("writeGPIO", port, value);
        }
        else
        {
            Debug.LogError("WriteGPIO : gpioControl is not initialized.");
            return -1; // 적절한 오류 코드 반환
        }
    }

    public void CloseGPIOScreen()
    {
        gpioScreen.SetActive(false);
    }

    public void OpenGPIOScreen()
    {
        dropdown_GPIOPorts.ClearOptions();

        List<TMP_Dropdown.OptionData> options_GPIOPorts = new List<TMP_Dropdown.OptionData>();
        string portName = string.Empty;
        for (int i = 0; i <= 3; i++)
        {
            switch (i)
            {
                case 0:
                    portName = $"GPIO ({i}) FAN1";
                    break;
                case 1:
                    portName = $"GPIO ({i}) FAN2";
                    break;
                case 2:
                    portName = $"GPIO ({i}) TCP/Rx";
                    break;
                case 3:
                    portName = $"GPIO ({i}) TCP/Tx";
                    break;
            }
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(portName);
            options_GPIOPorts.Add(option);
        }

        dropdown_GPIOPorts.AddOptions(options_GPIOPorts);

        dropdown_GPIOPorts.onValueChanged.RemoveAllListeners();
        dropdown_GPIOPorts.onValueChanged.AddListener((value) =>
        {
            selectedGPIOPort = value;
            Debug.Log($"selectedGPIOPort : {selectedGPIOPort}");
        });

        btnGPIOOutOpen.onClick.RemoveAllListeners();
        btnGPIOOutOpen.onClick.AddListener(() =>
        {
            WriteGPIO(selectedGPIOPort, true);
            txtResult.text = $"{dropdown_GPIOPorts.options[selectedGPIOPort].text} = Open";
        });

        btnGPIOOutClose.onClick.RemoveAllListeners();
        btnGPIOOutClose.onClick.AddListener(() =>
        {
            WriteGPIO(selectedGPIOPort, false);
            txtResult.text = $"{dropdown_GPIOPorts.options[selectedGPIOPort].text} = Close";
        });

        btnGPIORead.onClick.RemoveAllListeners();
        btnGPIORead.onClick.AddListener(() =>
        {
            txtResult.text = $"{dropdown_GPIOPorts.options[selectedGPIOPort].text} = Return:{ReadGPIO(selectedGPIOPort)} Complete";            
        });
    }
#endif
}