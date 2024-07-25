using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;
    public static ConfigManager Instance { get { return _instance; } }

    private Dictionary<string, string> settings = new Dictionary<string, string>();
    private string settingsFilePath;

    // 싱글톤 인스턴스 생성 및 초기화
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

        }
        LoadSettings(); // 설정 로드
    }

    // 설정 파일 로드 또는 기본 설정 생성
    public void LoadSettings()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, "Settings/config.ini");

        bool settingsUpdated = false; // 설정이 업데이트 되었는지 추적하는 플래그

        // 설정 파일이 존재하지 않으면 기본 설정 생성
        if (!File.Exists(settingsFilePath))
        {
            CreateDefaultSettings(); // 기본 설정으로 생성
        }
        else
        {
            // 설정 파일에서 설정 읽기
            string[] lines = File.ReadAllLines(settingsFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    settings[parts[0].Trim()] = parts[1].Trim(); // 설정 값 저장
                }
            }

            // 새로운 설정 항목 확인 및 업데이트
            settingsUpdated = UpdateSettingsWithNewDefaults();
        }

        if (settingsUpdated)
        {
            // 변경된 설정을 파일에 저장
            SaveSettings();
        }
    }

    // 새로운 설정 항목을 현재 설정에 추가하고, 필요한 경우 기본값으로 설정
    private bool UpdateSettingsWithNewDefaults()
    {
        Dictionary<string, string> defaultSettings = new Dictionary<string, string>
    {
        // 여기에 새로운 설정 항목과 기본값 추가
            {"LOCK_USE", "false"},
            {"LOCK_STATE", "false"},
            {"LOCK_PW", "0000"},
            {"VOLUME_LEVEL", "5"},
            {"TOUCH_SOUND", "1"},
            {"ALARM_SOUND", "1"},
            {"COLOR_THEME", "1"},
            {"WEATHER_USE", "0"}            
        // 기존에 설정에서 추가하고 싶은 기본 설정 추가
    };

        bool updated = false;

        foreach (var item in defaultSettings)
        {
            if (!settings.ContainsKey(item.Key))
            {
                settings[item.Key] = item.Value;
                updated = true;
            }
        }

        return updated;
    }

    // 기본 설정 생성 및 파일에 저장
    private void CreateDefaultSettings()
    {
        // Settings 폴더가 없으면 생성
        string directoryPath = Path.GetDirectoryName(settingsFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 기본 설정 값 정의
        Dictionary<string, string> defaultSettings = new Dictionary<string, string>
        {
            {"MODE", "SERVER"},
            {"SERVER_IP", "127.0.0.1"},
            {"DB_ID", "root"},
            {"DB_PW", "!@#QWE123"},
            {"DB_PORT", "28365"},
            {"DB_NAME", "kiosk"},
            {"DB_MIN_POOL_SIZE", "1"},
            {"DB_MAX_POOL_SIZE", "20"},
            {"DB_POLLING_INTERVAL", "1"},
            {"FIRSTSTART", "true"},
            {"AUTO_UPDATE_CHECK", "true"}            
        };

        // 파일에 기본 설정 저장
        SaveSettings(defaultSettings);
    }

    // 특정 설정 값 가져오기
    public string GetSetting(string key)
    {
        if (settings.ContainsKey(key))
        {
            return settings[key];
        }
        return null;
    }

    public bool GetBoolSetting(string key, bool defaultValue = false)
    {
        if (settings.TryGetValue(key, out string value))
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
        }
        return defaultValue; // 기본값 반환
    }

    public int GetIntSetting(string key, int defaultValue = 0)
    {
        if (settings.TryGetValue(key, out string value))
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
        }
        return defaultValue; // 기본값 반환
    }


    // 설정 값 변경 및 저장
    public void SetSetting(string key, string value)
    {
        settings[key] = value;

        SaveSettings();
    }

    public void SetSetting(string key, bool value)
    {
        settings[key] = value.ToString(); // bool 값을 문자열로 변환하여 저장
        SaveSettings();
    }

    public void SetSetting(string key, int value)
    {
        settings[key] = value.ToString(); // int 값을 문자열로 변환하여 저장
        SaveSettings();
    }

    // 설정 파일에 저장
    private void SaveSettings(Dictionary<string, string> settingsToSave = null)
    {
        List<string> lines = new List<string>();

        if (settingsToSave == null)
        {
            settingsToSave = settings; // 매개변수가 없으면 현재 설정 사용
        }

        // 설정 파일 형식으로 변환
        foreach (var setting in settingsToSave)
        {
            lines.Add($"{setting.Key} = {setting.Value}");
        }

        // 설정 파일에 쓰기
        File.WriteAllLines(settingsFilePath, lines.ToArray());
    }
}
