using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    private static ConfigManager _instance;
    public static ConfigManager Instance { get { return _instance; } }

    private Dictionary<string, string> settings = new Dictionary<string, string>();
    private string settingsFilePath;

    // �̱��� �ν��Ͻ� ���� �� �ʱ�ȭ
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
        LoadSettings(); // ���� �ε�
    }

    // ���� ���� �ε� �Ǵ� �⺻ ���� ����
    public void LoadSettings()
    {
        settingsFilePath = Path.Combine(Application.persistentDataPath, "Settings/config.ini");

        bool settingsUpdated = false; // ������ ������Ʈ �Ǿ����� �����ϴ� �÷���

        // ���� ������ �������� ������ �⺻ ���� ����
        if (!File.Exists(settingsFilePath))
        {
            CreateDefaultSettings(); // �⺻ �������� ����
        }
        else
        {
            // ���� ���Ͽ��� ���� �б�
            string[] lines = File.ReadAllLines(settingsFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    settings[parts[0].Trim()] = parts[1].Trim(); // ���� �� ����
                }
            }

            // ���ο� ���� �׸� Ȯ�� �� ������Ʈ
            settingsUpdated = UpdateSettingsWithNewDefaults();
        }

        if (settingsUpdated)
        {
            // ����� ������ ���Ͽ� ����
            SaveSettings();
        }
    }

    // ���ο� ���� �׸��� ���� ������ �߰��ϰ�, �ʿ��� ��� �⺻������ ����
    private bool UpdateSettingsWithNewDefaults()
    {
        Dictionary<string, string> defaultSettings = new Dictionary<string, string>
    {
        // ���⿡ ���ο� ���� �׸�� �⺻�� �߰�
            {"LOCK_USE", "false"},
            {"LOCK_STATE", "false"},
            {"LOCK_PW", "0000"},
            {"VOLUME_LEVEL", "5"},
            {"TOUCH_SOUND", "1"},
            {"ALARM_SOUND", "1"},
            {"COLOR_THEME", "1"},
            {"WEATHER_USE", "0"}            
        // ������ �������� �߰��ϰ� ���� �⺻ ���� �߰�
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

    // �⺻ ���� ���� �� ���Ͽ� ����
    private void CreateDefaultSettings()
    {
        // Settings ������ ������ ����
        string directoryPath = Path.GetDirectoryName(settingsFilePath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // �⺻ ���� �� ����
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

        // ���Ͽ� �⺻ ���� ����
        SaveSettings(defaultSettings);
    }

    // Ư�� ���� �� ��������
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
        return defaultValue; // �⺻�� ��ȯ
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
        return defaultValue; // �⺻�� ��ȯ
    }


    // ���� �� ���� �� ����
    public void SetSetting(string key, string value)
    {
        settings[key] = value;

        SaveSettings();
    }

    public void SetSetting(string key, bool value)
    {
        settings[key] = value.ToString(); // bool ���� ���ڿ��� ��ȯ�Ͽ� ����
        SaveSettings();
    }

    public void SetSetting(string key, int value)
    {
        settings[key] = value.ToString(); // int ���� ���ڿ��� ��ȯ�Ͽ� ����
        SaveSettings();
    }

    // ���� ���Ͽ� ����
    private void SaveSettings(Dictionary<string, string> settingsToSave = null)
    {
        List<string> lines = new List<string>();

        if (settingsToSave == null)
        {
            settingsToSave = settings; // �Ű������� ������ ���� ���� ���
        }

        // ���� ���� �������� ��ȯ
        foreach (var setting in settingsToSave)
        {
            lines.Add($"{setting.Key} = {setting.Value}");
        }

        // ���� ���Ͽ� ����
        File.WriteAllLines(settingsFilePath, lines.ToArray());
    }
}
