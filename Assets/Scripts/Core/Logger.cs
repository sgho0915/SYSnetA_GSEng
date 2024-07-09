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

        //// 로그 파일을 저장할 경로를 앱의 전용 디렉토리 내의 Logs 폴더로 설정
        //string logDirectory = Path.Combine(Application.persistentDataPath, "Logs");

        //// Logs 폴더가 없으면 생성
        //if (!Directory.Exists(logDirectory))
        //{
        //    Directory.CreateDirectory(logDirectory);
        //}

        //fullPath = Path.Combine(logDirectory, logFileName);

        //// 로그 메시지 이벤트에 핸들러 등록
        //Application.logMessageReceived += LogMessageReceived;
        ////Debug.Log("Logger : Start saving logs in Logs folder");
    }

    void OnDestroy()
    {
        //Debug.Log("Logger : Stop saving logs");
        // 이벤트 핸들러 등록 해제
        Application.logMessageReceived -= LogMessageReceived;
    }

    private void LogMessageReceived(string condition, string stackTrace, LogType type)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {condition}";
        if (type == LogType.Error || type == LogType.Exception)
        {
            logEntry += $"\nStackTrace: {stackTrace}";
        }

        // 매달 로그 파일 이름 업데이트
        string currentFileName = DateTime.Now.ToString("yyyyMM") + "_log.txt";
        if (currentFileName != logFileName)
        {
            logFileName = currentFileName;
            fullPath = Path.Combine(Path.Combine(Application.persistentDataPath, "Logs"), logFileName); // 경로 업데이트
        }

        // 로그 파일에 로그 메시지 추가
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
