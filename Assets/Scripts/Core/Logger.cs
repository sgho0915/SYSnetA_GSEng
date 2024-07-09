using System;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class Logger : MonoBehaviour
{
    private static string logFileName = DateTime.Now.ToString("yyyyMM") + "_log.txt";
    private static string fullPath;

    void Awake()
    {
        //if (Application.platform == RuntimePlatform.Android && !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        //{
        //    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        //}

        //// �α� ������ ������ ��θ� ���� ���� ���丮 ���� Logs ������ ����
        //string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");

        //// Logs ������ ������ ����
        //if (!Directory.Exists(logDirectory))
        //{
        //    Directory.CreateDirectory(logDirectory);
        //}

        //fullPath = Path.Combine(logDirectory, logFileName);

        //// �α� �޽��� �̺�Ʈ�� �ڵ鷯 ���
        //Application.logMessageReceived += LogMessageReceived;
        ////Debug.Log("Logger : Start saving logs in Logs folder");
    }

    void OnDestroy()
    {
        //Debug.Log("Logger : Stop saving logs");
        // �̺�Ʈ �ڵ鷯 ��� ����
        Application.logMessageReceived -= LogMessageReceived;
    }

    private void LogMessageReceived(string condition, string stackTrace, LogType type)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {condition}";
        if (type == LogType.Error || type == LogType.Exception)
        {
            logEntry += $"\nStackTrace: {stackTrace}";
        }

        // �Ŵ� �α� ���� �̸� ������Ʈ
        string currentFileName = DateTime.Now.ToString("yyyyMM") + "_log.txt";
        if (currentFileName != logFileName)
        {
            logFileName = currentFileName;
            fullPath = Path.Combine(Path.Combine(Application.persistentDataPath, "Logs"), logFileName); // ��� ������Ʈ
        }

        // �α� ���Ͽ� �α� �޽��� �߰�
        try
        {
            File.AppendAllText(fullPath, logEntry + "\n");
        }
        catch (Exception ex)
        {
            //Debug.LogError($"Logger : Failed to write log to file: {ex.Message}");
        }
    }
}
