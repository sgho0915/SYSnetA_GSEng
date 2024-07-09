using System;
using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class ResponseData
{
    public string brandCode;
    public string fileNo;
    public string history;
    public string content;
    public string upload_time;
    public string filesize;
    public string ver;
}

public class InformationManager : MonoBehaviour
{
    public string swVer = string.Empty; // 소프트웨어 버전
    public string swDate = string.Empty; // 소프트웨어 버전 제작일    
    public string cloudID = string.Empty; // 클라우드 id
    public string rsID = string.Empty; // 원격지원 id
    public string totalStorage = string.Empty; // 디바이스 총 용량
    public string usedStorage = string.Empty; // 디바이스 사용 용량
    public string freeStorage = string.Empty; // 디바이스 여유 공간

    public static InformationManager Instance { get; private set; }

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


    public void LoadSystemInfo()
    {
        LoadStorageInfo(); 
        LoadOthersInfo();
        LoadSWInfo();
    }

    // 소프트웨어 버전 로드
    public void LoadSWInfo()
    {
        StartCoroutine(GetJsonDataCoroutine(CheckOTA.Instance.chkUpdateUrl));        
    }

    // 이더넷, 무선랜, 클라우드 ID, rustdesk ID 로드
    public void LoadOthersInfo()
    {        
        TextMeshProUGUI txtEthIP = SettingManager.Instance.item_EthInfo.transform.Find("InfoParent/IPParent/txtValueIP").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtEthMac = SettingManager.Instance.item_EthInfo.transform.Find("InfoParent/MACParent/txtValueMAC").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtWlanIP = SettingManager.Instance.item_WlanInfo.transform.Find("InfoParent/IPParent/txtValueIP").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtWlanMac = SettingManager.Instance.item_WlanInfo.transform.Find("InfoParent/MACParent/txtValueMAC").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtCloudID = SettingManager.Instance.item_CloudInfo.transform.Find("txtValue").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI txtRustDeskID = SettingManager.Instance.item_RuskDeskInfo.transform.Find("txtValue").GetComponent<TextMeshProUGUI>();

        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        //    AndroidJavaClass pluginClass = new AndroidJavaClass("com.systronics.plugin.EthernetStatusPlugin");

        //    txtEthIP.text = pluginClass.CallStatic<string>("getEthernetLocalIpAddress", currentActivity);
        //    txtEthMac.text = pluginClass.CallStatic<string>("getEthernetMacAddress", currentActivity);
        //    txtWlanIP.text = pluginClass.CallStatic<string>("getWifiLocalIpAddress", currentActivity);
        //    txtWlanMac.text = pluginClass.CallStatic<string>("getWifiMacAddress", currentActivity);
        //}

        DataTable tblConfig = ClientDatabase.FetchConfigData().Tables[0];
        foreach (DataRow row in tblConfig.Rows)
        {
            txtCloudID.text = row["CLOUD_ID"].ToString();
            txtRustDeskID.text = row["RUSTDESK_ID"].ToString();
            
            cloudID = row["CLOUD_ID"].ToString();
            rsID = row["RUSTDESK_ID"].ToString();
        }
    }

    public IEnumerator GetJsonDataCoroutine(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 요청을 보냅니다.
            yield return webRequest.SendWebRequest();

            // 응답으로부터 JSON 데이터를 받습니다.
            string json = webRequest.downloadHandler.text;
            //Debug.Log("Received: " + json);
            TextMeshProUGUI txtSWInfo = SettingManager.Instance.item_SwInfo.transform.Find("btnValueHidden/txtValue").GetComponent<TextMeshProUGUI>();
            // 오류 검사
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
               // Debug.LogError("Error: " + webRequest.error);
                txtSWInfo.text = $"Ver.{Application.version} (업데이트 확인 불가)";
            }
            else
            {
                // JSON 응답을 객체로 변환합니다.
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(json);
                txtSWInfo.text = $"Ver.{Application.version}";
                if (!CheckOTA.Instance.CheckLatestVer(responseData.ver, Application.version))
                {
                    txtSWInfo.text = $"Ver.{Application.version}";
                    swDate = responseData.upload_time;
                    swVer = responseData.ver;
                }
                else
                {
                    txtSWInfo.text = $"Ver.{Application.version} (업데이트 확인 필요)";
                    swVer = Application.version;
                }
            }
        }
    }

    // 저장소 용량 로드
    public void LoadStorageInfo()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass storageInfoClass = new AndroidJavaClass("com.systronics.plugin.StorageInfo");
            //storageInfo = storageInfoClass.CallStatic<string>("getExternalStorageInfo");
            //Debug.Log("Storage Info: " + storageInfo);
            totalStorage = storageInfoClass.CallStatic<string>("getTotalExternalStorage");
            usedStorage = storageInfoClass.CallStatic<string>("getUsedExternalStorage");
            freeStorage = storageInfoClass.CallStatic<string>("getFreeExternalStorage");
        }

        TextMeshProUGUI txtStorage = SettingManager.Instance.item_StorageInfo.transform.Find("txtValue").GetComponent<TextMeshProUGUI>();
        txtStorage.text = $"총 {totalStorage}GB 중 {usedStorage}GB 사용중, {freeStorage}GB 남음";
    }
}
