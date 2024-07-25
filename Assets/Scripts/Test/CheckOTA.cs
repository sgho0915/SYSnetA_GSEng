using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MySqlConnector;
using System.Data;
using System;
using UnityEngine.Networking;
using System.IO;


public class CheckOTA : MonoBehaviour
{
    private string downloadedApkPath = string.Empty; // 다운로드한 APK 파일의 경로 저장
    public Coroutine chdAutoUpdateCoroutine = null; // 현재 실행 중인 코루틴을 추적

    public string chkUpdateUrl = string.Empty;
    public string downloadUpdateUrl = string.Empty;

    private string bcode = "GSENG";

    public static CheckOTA Instance { get; private set; }

    void Awake()
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

    private void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!ClientDatabase.isTestMode)
            {
                chkUpdateUrl = $"http://cloud.systronics.co.kr/app/?BCODE={bcode}";
                downloadUpdateUrl = $"http://cloud.systronics.co.kr/app/?BCODE={bcode}&DOWNLOAD=Y";
            }
            else
            {
                chkUpdateUrl = $"http://cloud.systronics.co.kr/app/?BCODE=NY_BURGER_TEST";
                downloadUpdateUrl = $"http://cloud.systronics.co.kr/app/?BCODE=NY_BURGER_TEST&DOWNLOAD=Y";
            }
        }
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            chkUpdateUrl = $"http://cloud.systronics.co.kr/app/?BCODE={bcode}";
            downloadUpdateUrl = $"http://cloud.systronics.co.kr/app/?BCODE={bcode}&DOWNLOAD=Y";
        }
        if (chdAutoUpdateCoroutine == null && ConfigManager.Instance.GetSetting("AUTO_UPDATE_CHECK") == "true")
            chdAutoUpdateCoroutine = StartCoroutine(AutoUpdateCheck(chkUpdateUrl));
    }

    public void LoadUpdateInfo(bool isAutoUpdateClicked)
    {
        if (isAutoUpdateClicked)
        {
            ScreenManager.Instance.GotoSettings();
            SettingManager.Instance.ChangeUpdateSetting();
            StartCoroutine(GetUpdateInfoJson(chkUpdateUrl));
        }
        else
        {
            StartCoroutine(GetUpdateInfoJson(chkUpdateUrl));
        }
    }

    public IEnumerator GetUpdateInfoJson(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 요청을 보냅니다.
            yield return webRequest.SendWebRequest();

            // 응답으로부터 JSON 데이터를 받습니다.
            string json = webRequest.downloadHandler.text;
            Debug.Log("업데이트 정보 Received: " + json);

            // 오류 검사
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                //Debug.LogError("업데이트 확인 실패 : " + webRequest.error);
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = $"업데이트 확인에 실패했습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
            }
            else
            {
                // JSON 응답을 객체로 변환합니다.
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);
                
                if (!CheckLatestVer(responseData.ver, Application.version))
                {
                    // 현재 버전이 최신 버전인 경우
                    SettingManager.Instance.btnUpdate.gameObject.SetActive(false);
                    CurretVersionIsLatest();
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                    ScreenManager.Instance.txt_PopUpMsg.text = $"최신 소프트웨어(Ver.{Application.version}) 입니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
                else
                {
                    // 최신 버전이 존재하는 경우
                    SettingManager.Instance.btnUpdate.gameObject.SetActive(true);
                    CurrentVersionIsOld(responseData);
                }
            }
        }
    }


    public IEnumerator AutoUpdateCheck(string url)
    {
        while (ConfigManager.Instance.GetSetting("AUTO_UPDATE_CHECK") == "true" ? true : false)
        {
            //Debug.Log($"자동 업데이트 체크 : {ConfigManager.Instance.GetSetting("AUTO_UPDATE_CHECK")}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // 현재 시간을 확인
                DateTime now = DateTime.Now;
                DateTime nextUpdateTime = now.AddHours(6 - now.Hour % 6).AddMinutes(-now.Minute).AddSeconds(-now.Second);

                // 요청을 보냅니다.
                yield return webRequest.SendWebRequest();

                // 응답으로부터 JSON 데이터를 받습니다.
                string json = webRequest.downloadHandler.text;
                Debug.Log("업데이트 정보 Received: " + json);

                // 오류 검사
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    //Debug.LogError("업데이트 확인 실패 : " + webRequest.error);
                }
                else
                {
                    // JSON 응답을 객체로 변환합니다.
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);

                    if (CheckLatestVer(responseData.ver, Application.version))
                    {
                        //Debug.Log($"최신 소프트웨어(Ver.{responseData.ver})가 확인되었습니다.");
                        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                        ScreenManager.Instance.txt_PopUpMsg.text = $"최신 소프트웨어(Ver.{responseData.ver})가 확인되었습니다.\n업데이트를 진행하시겠습니까?";
                        ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(true);
                        ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                        ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
                        ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() =>
                        {
                            ScreenManager.Instance.ClosePopUpMessage();
                            ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(false);
                        });
                        ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                        {
                            ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(false);
                            ScreenManager.Instance.ClosePopUpMessage();

                            LoadUpdateInfo(true);
                        });
                    }
                }

                // 다음 업데이트 시간까지 대기
                yield return new WaitForSeconds((float)(nextUpdateTime - now).TotalSeconds);
            }
        }        
    }

    public bool CheckLatestVer(string serverVer, string appVer)
    {
        string[] arrServer = serverVer.Split('.');
        int serverHighVer = int.Parse(arrServer[0]);
        int serverMidVer = int.Parse(arrServer[1]);
        int serverLowVer = int.Parse(arrServer[2]);
        string[] arrApp = appVer.Split('.');
        int appHighVer = int.Parse(arrApp[0]);
        int appMidVer = int.Parse(arrApp[1]);
        int appLowVer = int.Parse(arrApp[2]);

        if (serverHighVer > appHighVer)
        {
            return true;
        }
        else if (serverHighVer == appHighVer)
        {
            if(serverMidVer > appMidVer)
            {
                return true;
            }
            else if (serverMidVer == appMidVer)
            {
                if(serverLowVer > appLowVer)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }

    // 최신 버전이 존재하는 경우
    public void CurrentVersionIsOld(ResponseData resp)
    {
        // 업데이트에 필요한 항목 활성화
        SettingManager.Instance.item_ChangeLog.SetActive(true);
        SettingManager.Instance.item_DownloadInfo.SetActive(false);
        SettingManager.Instance.item_UpdateInfo.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SettingManager.Instance.updateInfoTransform.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        // 타이틀
        TextMeshProUGUI txtTitle = SettingManager.Instance.item_ChangeLog.transform.Find("Title/UpTitle").GetComponent<TextMeshProUGUI>();
        txtTitle.text = $"새로운 기능 (Ver.{resp.ver})";

        // 체인지 로그
        TextMeshProUGUI txtChangeLog = SettingManager.Instance.item_ChangeLog.transform.Find("Content/txtContent").GetComponent<TextMeshProUGUI>();
        txtChangeLog.text = ConvertEnterToPlainText(resp.content); // HTML을 일반 텍스트로 변환하여 적용

        // 업데이트 정보
        TextMeshProUGUI txtUpdateVer = SettingManager.Instance.item_UpdateInfo.transform.Find("Version/txtValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtUpdateSize = SettingManager.Instance.item_UpdateInfo.transform.Find("Size/txtValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtUploadDate = SettingManager.Instance.item_UpdateInfo.transform.Find("Date/txtValue").GetComponent<TextMeshProUGUI>();
        txtUpdateVer.text = $"Ver.{resp.ver}";
        int bytes = int.Parse(resp.filesize);
        float megabytes = bytes / (1024f * 1024f);
        string formattedMb = megabytes.ToString("F2");
        txtUpdateSize.text = $"{formattedMb}MB";
        txtUploadDate.text = resp.upload_time;

        // 다운로드 및 설치 버튼
        Button btnDownloadUpdate = SettingManager.Instance.btnUpdate.GetComponent<Button>();
        string newFile = Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk");

        if (!File.Exists(newFile))
        {
            //Debug.Log($"DownloadedUpdate(Ver.{resp.ver}).apk 파일이 없음");
            SettingManager.Instance.btnUpdate.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "다운로드 및 설치";
            btnDownloadUpdate.onClick.RemoveAllListeners();
            btnDownloadUpdate.onClick.AddListener(() =>
            {
                StartCoroutine(DownloadAPKCoroutine(downloadUpdateUrl, resp));
            });
        }
        else
        {
            //Debug.Log($"DownloadedUpdate(Ver.{resp.ver}).apk 파일이 있음");
            SettingManager.Instance.btnUpdate.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "업데이트 설치";
            btnDownloadUpdate.onClick.RemoveAllListeners();
            btnDownloadUpdate.onClick.AddListener(() =>
            {
                InstallAPK(Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk"));
            });
        }

        #region 자동 업데이트 체크 저장
        // 자동 업데이트 체크 저장
        Button btnAutoChkUse = SettingManager.Instance.item_AutoUpdateCheck.transform.Find("AutoCheckUse/btnUse").GetComponent<Button>();
        GameObject objAutoChkUse_Selected = btnAutoChkUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objAutoChkUse_NotSelected = btnAutoChkUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnAutoChkNotUse = SettingManager.Instance.item_AutoUpdateCheck.transform.Find("AutoCheckUse/btnNotUse").GetComponent<Button>();
        GameObject objAutoChkNotUse_Selected = btnAutoChkNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objAutoChkNotUse_NotSelected = btnAutoChkNotUse.gameObject.transform.Find("NotSelected").gameObject;

        string isAutoChk = ConfigManager.Instance.GetSetting("AUTO_UPDATE_CHECK");

        if (isAutoChk == "true")
        {
            isAutoChk = "true";
            objAutoChkUse_Selected.SetActive(true);
            objAutoChkUse_NotSelected.SetActive(false);
            objAutoChkNotUse_Selected.SetActive(false);
            objAutoChkNotUse_NotSelected.SetActive(true);
        }
        else
        {
            isAutoChk = "false";
            objAutoChkUse_Selected.SetActive(false);
            objAutoChkUse_NotSelected.SetActive(true);
            objAutoChkNotUse_Selected.SetActive(true);
            objAutoChkNotUse_NotSelected.SetActive(false);
        }

        btnAutoChkUse.onClick.RemoveAllListeners();
        btnAutoChkNotUse.onClick.RemoveAllListeners();
        btnAutoChkUse.onClick.AddListener(() =>
        {
            isAutoChk = "true";
            objAutoChkUse_Selected.SetActive(true);
            objAutoChkUse_NotSelected.SetActive(false);
            objAutoChkNotUse_Selected.SetActive(false);
            objAutoChkNotUse_NotSelected.SetActive(true);
        });
        btnAutoChkNotUse.onClick.AddListener(() =>
        {
            isAutoChk = "false";
            objAutoChkUse_Selected.SetActive(false);
            objAutoChkUse_NotSelected.SetActive(true);
            objAutoChkNotUse_Selected.SetActive(true);
            objAutoChkNotUse_NotSelected.SetActive(false);
        });

        // 하단 저장 버튼
        Button btnSave = SettingManager.Instance.btnSave.GetComponent<Button>();
        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            ConfigManager.Instance.SetSetting("AUTO_UPDATE_CHECK", $"{isAutoChk}"); // 자동 업데이트 체크 여부 저장
            switch (isAutoChk)
            {
                case "true":
                    if(chdAutoUpdateCoroutine == null)
                        chdAutoUpdateCoroutine = StartCoroutine(AutoUpdateCheck(chkUpdateUrl));
                    break;
                case "false":
                    if(chdAutoUpdateCoroutine != null)
                    {
                        if (chdAutoUpdateCoroutine != null)
                        {
                            StopCoroutine(chdAutoUpdateCoroutine);
                            chdAutoUpdateCoroutine = null;
                        }
                    }
                    break;
            }
            
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = "설정이 저장되었습니다.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
            });
        });
#endregion
    }

    // 현재 버전이 최신 버전인 경우
    public void CurretVersionIsLatest()
    {
        // 업데이트에 필요한 항목 숨기기
        SettingManager.Instance.item_ChangeLog.SetActive(false);
        SettingManager.Instance.item_DownloadInfo.SetActive(false);
        SettingManager.Instance.item_UpdateInfo.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SettingManager.Instance.updateInfoTransform.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        // 자동 업데이트 체크 저장
        Button btnAutoChkUse = SettingManager.Instance.item_AutoUpdateCheck.transform.Find("AutoCheckUse/btnUse").GetComponent<Button>();
        GameObject objAutoChkUse_Selected = btnAutoChkUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objAutoChkUse_NotSelected = btnAutoChkUse.gameObject.transform.Find("NotSelected").gameObject;
        Button btnAutoChkNotUse = SettingManager.Instance.item_AutoUpdateCheck.transform.Find("AutoCheckUse/btnNotUse").GetComponent<Button>();
        GameObject objAutoChkNotUse_Selected = btnAutoChkNotUse.gameObject.transform.Find("Selected").gameObject;
        GameObject objAutoChkNotUse_NotSelected = btnAutoChkNotUse.gameObject.transform.Find("NotSelected").gameObject;

        string isAutoChk = ConfigManager.Instance.GetSetting("AUTO_UPDATE_CHECK");

        if (isAutoChk == "true")
        {
            isAutoChk = "true";
            objAutoChkUse_Selected.SetActive(true);
            objAutoChkUse_NotSelected.SetActive(false);
            objAutoChkNotUse_Selected.SetActive(false);
            objAutoChkNotUse_NotSelected.SetActive(true);
        }
        else
        {
            isAutoChk = "false";
            objAutoChkUse_Selected.SetActive(false);
            objAutoChkUse_NotSelected.SetActive(true);
            objAutoChkNotUse_Selected.SetActive(true);
            objAutoChkNotUse_NotSelected.SetActive(false);
        }

        btnAutoChkUse.onClick.RemoveAllListeners();
        btnAutoChkNotUse.onClick.RemoveAllListeners();
        btnAutoChkUse.onClick.AddListener(() =>
        {
            isAutoChk = "true";
            objAutoChkUse_Selected.SetActive(true);
            objAutoChkUse_NotSelected.SetActive(false);
            objAutoChkNotUse_Selected.SetActive(false);
            objAutoChkNotUse_NotSelected.SetActive(true);
        });
        btnAutoChkNotUse.onClick.AddListener(() =>
        {
            isAutoChk = "false";
            objAutoChkUse_Selected.SetActive(false);
            objAutoChkUse_NotSelected.SetActive(true);
            objAutoChkNotUse_Selected.SetActive(true);
            objAutoChkNotUse_NotSelected.SetActive(false);
        });

        // 하단 저장 버튼
        Button btnSave = SettingManager.Instance.btnSave.GetComponent<Button>();
        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            ConfigManager.Instance.SetSetting("AUTO_UPDATE_CHECK", $"{isAutoChk}"); // 자동 업데이트 체크 여부 저장            
            switch (isAutoChk)
            {
                case "true":
                    if (chdAutoUpdateCoroutine == null)
                        chdAutoUpdateCoroutine = StartCoroutine(AutoUpdateCheck(chkUpdateUrl));
                    break;
                case "false":
                    if (chdAutoUpdateCoroutine != null)
                    {
                        if (chdAutoUpdateCoroutine != null)
                        {
                            StopCoroutine(chdAutoUpdateCoroutine);
                            chdAutoUpdateCoroutine = null;
                        }
                    }
                    break;
            }
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = "설정이 저장되었습니다.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
            });
        });
    }
        
    private IEnumerator DownloadAPKCoroutine(string url, ResponseData resp)
    {
        SettingManager.Instance.item_DownloadInfo.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SettingManager.Instance.updateInfoTransform.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        // 다운로드 정보
        Slider downloadSlider = SettingManager.Instance.item_DownloadInfo.transform.Find("Download/Slider").GetComponent<Slider>();
        TextMeshProUGUI txtDownloadValue = SettingManager.Instance.item_DownloadInfo.transform.Find("Download/txtValue").GetComponent<TextMeshProUGUI>();
        
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SendWebRequest();

        while (!request.isDone)
        {
            txtDownloadValue.text = $"다운로드 진행 중 ({(request.downloadProgress * 100).ToString("F1")}%)";
            downloadSlider.value = request.downloadProgress * 100;
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            downloadedApkPath = Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk");
            // 오류 처리
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = $"다운로드에 실패했습니다.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
                downloadSlider.value = 0;
                txtDownloadValue.text = string.Empty;
                SettingManager.Instance.item_DownloadInfo.SetActive(false);
                DeleteDownloadFile(downloadedApkPath);
            });
            //Debug.LogError("다운로드 실패: " + request.error);
        }
        else
        {
            // APK 파일 저장
            downloadedApkPath = Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk");
            File.WriteAllBytes(downloadedApkPath, request.downloadHandler.data);

            // 다운로드 완료 메시지
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = $"소프트웨어가 다운로드 되었습니다.\n설치를 진행하시겠습니까?";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(true);
            ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
                SettingManager.Instance.btnUpdate.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "업데이트 설치";
                SettingManager.Instance.btnUpdate.onClick.RemoveAllListeners();
                SettingManager.Instance.btnUpdate.onClick.AddListener(() =>
                {
                    InstallAPK(Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk"));
                });
                ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(false);
            });
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(false);
                ScreenManager.Instance.ClosePopUpMessage();
                InstallAPK(downloadedApkPath);
            });
        }
    }

    public void DeleteDownloadFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log("파일 삭제 완료 : " + filePath);
        }
        else
        {
            //Debug.Log("파일을 찾을 수 없음 : " + filePath);
        }
    }

    private void InstallAPK(string apkPath)
    {
        try
        {
            AndroidJavaClass unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityClass.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject unityContext = unityActivity.Call<AndroidJavaObject>("getApplicationContext");

            AndroidJavaClass plugin = new AndroidJavaClass("com.systronics.plugin.UnityPlugin");
            string result = plugin.CallStatic<string>("InstallApp", unityContext, apkPath);
        }
        catch (Exception e)
        {
            //Debug.LogError("APK 설치 오류: " + e.Message);
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = $"소프트웨어 설치에 실패했습니다.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
            });
        }
    }

    private string ConvertHtmlToPlainText(string html)
    {
        if (string.IsNullOrEmpty(html)) return "";

        // <br>, <p> 태그를 줄바꿈으로 변환
        string withLineBreaks = html.Replace("<br>", "\n").Replace("<p>", "").Replace("</p>", "\n");

        // HTML 태그 제거
        return System.Text.RegularExpressions.Regex.Replace(withLineBreaks, "<.*?>", string.Empty).Trim();
    }

    private string ConvertEnterToPlainText(string html)
    {
        if (string.IsNullOrEmpty(html))
            return "";

        // '|' 문자를 줄바꿈으로 변환
        return html.Replace("|", "\n");
    }


}
