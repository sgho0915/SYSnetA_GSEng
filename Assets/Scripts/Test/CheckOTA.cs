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
    private string downloadedApkPath = string.Empty; // �ٿ�ε��� APK ������ ��� ����
    public Coroutine chdAutoUpdateCoroutine = null; // ���� ���� ���� �ڷ�ƾ�� ����

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
            // ��û�� �����ϴ�.
            yield return webRequest.SendWebRequest();

            // �������κ��� JSON �����͸� �޽��ϴ�.
            string json = webRequest.downloadHandler.text;
            Debug.Log("������Ʈ ���� Received: " + json);

            // ���� �˻�
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                //Debug.LogError("������Ʈ Ȯ�� ���� : " + webRequest.error);
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = $"������Ʈ Ȯ�ο� �����߽��ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
            }
            else
            {
                // JSON ������ ��ü�� ��ȯ�մϴ�.
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);
                
                if (!CheckLatestVer(responseData.ver, Application.version))
                {
                    // ���� ������ �ֽ� ������ ���
                    SettingManager.Instance.btnUpdate.gameObject.SetActive(false);
                    CurretVersionIsLatest();
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                    ScreenManager.Instance.txt_PopUpMsg.text = $"�ֽ� ����Ʈ����(Ver.{Application.version}) �Դϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
                else
                {
                    // �ֽ� ������ �����ϴ� ���
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
            //Debug.Log($"�ڵ� ������Ʈ üũ : {ConfigManager.Instance.GetSetting("AUTO_UPDATE_CHECK")}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // ���� �ð��� Ȯ��
                DateTime now = DateTime.Now;
                DateTime nextUpdateTime = now.AddHours(6 - now.Hour % 6).AddMinutes(-now.Minute).AddSeconds(-now.Second);

                // ��û�� �����ϴ�.
                yield return webRequest.SendWebRequest();

                // �������κ��� JSON �����͸� �޽��ϴ�.
                string json = webRequest.downloadHandler.text;
                Debug.Log("������Ʈ ���� Received: " + json);

                // ���� �˻�
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    //Debug.LogError("������Ʈ Ȯ�� ���� : " + webRequest.error);
                }
                else
                {
                    // JSON ������ ��ü�� ��ȯ�մϴ�.
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);

                    if (CheckLatestVer(responseData.ver, Application.version))
                    {
                        //Debug.Log($"�ֽ� ����Ʈ����(Ver.{responseData.ver})�� Ȯ�εǾ����ϴ�.");
                        ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                        ScreenManager.Instance.txt_PopUpMsg.text = $"�ֽ� ����Ʈ����(Ver.{responseData.ver})�� Ȯ�εǾ����ϴ�.\n������Ʈ�� �����Ͻðڽ��ϱ�?";
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

                // ���� ������Ʈ �ð����� ���
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

    // �ֽ� ������ �����ϴ� ���
    public void CurrentVersionIsOld(ResponseData resp)
    {
        // ������Ʈ�� �ʿ��� �׸� Ȱ��ȭ
        SettingManager.Instance.item_ChangeLog.SetActive(true);
        SettingManager.Instance.item_DownloadInfo.SetActive(false);
        SettingManager.Instance.item_UpdateInfo.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SettingManager.Instance.updateInfoTransform.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        // Ÿ��Ʋ
        TextMeshProUGUI txtTitle = SettingManager.Instance.item_ChangeLog.transform.Find("Title/UpTitle").GetComponent<TextMeshProUGUI>();
        txtTitle.text = $"���ο� ��� (Ver.{resp.ver})";

        // ü���� �α�
        TextMeshProUGUI txtChangeLog = SettingManager.Instance.item_ChangeLog.transform.Find("Content/txtContent").GetComponent<TextMeshProUGUI>();
        txtChangeLog.text = ConvertEnterToPlainText(resp.content); // HTML�� �Ϲ� �ؽ�Ʈ�� ��ȯ�Ͽ� ����

        // ������Ʈ ����
        TextMeshProUGUI txtUpdateVer = SettingManager.Instance.item_UpdateInfo.transform.Find("Version/txtValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtUpdateSize = SettingManager.Instance.item_UpdateInfo.transform.Find("Size/txtValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtUploadDate = SettingManager.Instance.item_UpdateInfo.transform.Find("Date/txtValue").GetComponent<TextMeshProUGUI>();
        txtUpdateVer.text = $"Ver.{resp.ver}";
        int bytes = int.Parse(resp.filesize);
        float megabytes = bytes / (1024f * 1024f);
        string formattedMb = megabytes.ToString("F2");
        txtUpdateSize.text = $"{formattedMb}MB";
        txtUploadDate.text = resp.upload_time;

        // �ٿ�ε� �� ��ġ ��ư
        Button btnDownloadUpdate = SettingManager.Instance.btnUpdate.GetComponent<Button>();
        string newFile = Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk");

        if (!File.Exists(newFile))
        {
            //Debug.Log($"DownloadedUpdate(Ver.{resp.ver}).apk ������ ����");
            SettingManager.Instance.btnUpdate.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "�ٿ�ε� �� ��ġ";
            btnDownloadUpdate.onClick.RemoveAllListeners();
            btnDownloadUpdate.onClick.AddListener(() =>
            {
                StartCoroutine(DownloadAPKCoroutine(downloadUpdateUrl, resp));
            });
        }
        else
        {
            //Debug.Log($"DownloadedUpdate(Ver.{resp.ver}).apk ������ ����");
            SettingManager.Instance.btnUpdate.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "������Ʈ ��ġ";
            btnDownloadUpdate.onClick.RemoveAllListeners();
            btnDownloadUpdate.onClick.AddListener(() =>
            {
                InstallAPK(Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk"));
            });
        }

        #region �ڵ� ������Ʈ üũ ����
        // �ڵ� ������Ʈ üũ ����
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

        // �ϴ� ���� ��ư
        Button btnSave = SettingManager.Instance.btnSave.GetComponent<Button>();
        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            ConfigManager.Instance.SetSetting("AUTO_UPDATE_CHECK", $"{isAutoChk}"); // �ڵ� ������Ʈ üũ ���� ����
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
            ScreenManager.Instance.txt_PopUpMsg.text = "������ ����Ǿ����ϴ�.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
            });
        });
#endregion
    }

    // ���� ������ �ֽ� ������ ���
    public void CurretVersionIsLatest()
    {
        // ������Ʈ�� �ʿ��� �׸� �����
        SettingManager.Instance.item_ChangeLog.SetActive(false);
        SettingManager.Instance.item_DownloadInfo.SetActive(false);
        SettingManager.Instance.item_UpdateInfo.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SettingManager.Instance.updateInfoTransform.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();

        // �ڵ� ������Ʈ üũ ����
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

        // �ϴ� ���� ��ư
        Button btnSave = SettingManager.Instance.btnSave.GetComponent<Button>();
        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            ConfigManager.Instance.SetSetting("AUTO_UPDATE_CHECK", $"{isAutoChk}"); // �ڵ� ������Ʈ üũ ���� ����            
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
            ScreenManager.Instance.txt_PopUpMsg.text = "������ ����Ǿ����ϴ�.";
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

        // �ٿ�ε� ����
        Slider downloadSlider = SettingManager.Instance.item_DownloadInfo.transform.Find("Download/Slider").GetComponent<Slider>();
        TextMeshProUGUI txtDownloadValue = SettingManager.Instance.item_DownloadInfo.transform.Find("Download/txtValue").GetComponent<TextMeshProUGUI>();
        
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SendWebRequest();

        while (!request.isDone)
        {
            txtDownloadValue.text = $"�ٿ�ε� ���� �� ({(request.downloadProgress * 100).ToString("F1")}%)";
            downloadSlider.value = request.downloadProgress * 100;
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            downloadedApkPath = Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk");
            // ���� ó��
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = $"�ٿ�ε忡 �����߽��ϴ�.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
                downloadSlider.value = 0;
                txtDownloadValue.text = string.Empty;
                SettingManager.Instance.item_DownloadInfo.SetActive(false);
                DeleteDownloadFile(downloadedApkPath);
            });
            //Debug.LogError("�ٿ�ε� ����: " + request.error);
        }
        else
        {
            // APK ���� ����
            downloadedApkPath = Path.Combine(Application.persistentDataPath, $"DownloadedUpdate(Ver.{resp.ver}).apk");
            File.WriteAllBytes(downloadedApkPath, request.downloadHandler.data);

            // �ٿ�ε� �Ϸ� �޽���
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
            ScreenManager.Instance.txt_PopUpMsg.text = $"����Ʈ��� �ٿ�ε� �Ǿ����ϴ�.\n��ġ�� �����Ͻðڽ��ϱ�?";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.gameObject.SetActive(true);
            ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
                SettingManager.Instance.btnUpdate.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "������Ʈ ��ġ";
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
            //Debug.Log("���� ���� �Ϸ� : " + filePath);
        }
        else
        {
            //Debug.Log("������ ã�� �� ���� : " + filePath);
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
            //Debug.LogError("APK ��ġ ����: " + e.Message);
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = $"����Ʈ���� ��ġ�� �����߽��ϴ�.";
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

        // <br>, <p> �±׸� �ٹٲ����� ��ȯ
        string withLineBreaks = html.Replace("<br>", "\n").Replace("<p>", "").Replace("</p>", "\n");

        // HTML �±� ����
        return System.Text.RegularExpressions.Regex.Replace(withLineBreaks, "<.*?>", string.Empty).Trim();
    }

    private string ConvertEnterToPlainText(string html)
    {
        if (string.IsNullOrEmpty(html))
            return "";

        // '|' ���ڸ� �ٹٲ����� ��ȯ
        return html.Replace("|", "\n");
    }


}
