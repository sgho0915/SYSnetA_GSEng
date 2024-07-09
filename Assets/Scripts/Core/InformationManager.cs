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
    public string swVer = string.Empty; // ����Ʈ���� ����
    public string swDate = string.Empty; // ����Ʈ���� ���� ������    
    public string cloudID = string.Empty; // Ŭ���� id
    public string rsID = string.Empty; // �������� id
    public string totalStorage = string.Empty; // ����̽� �� �뷮
    public string usedStorage = string.Empty; // ����̽� ��� �뷮
    public string freeStorage = string.Empty; // ����̽� ���� ����

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

    // ����Ʈ���� ���� �ε�
    public void LoadSWInfo()
    {
        StartCoroutine(GetJsonDataCoroutine(CheckOTA.Instance.chkUpdateUrl));        
    }

    // �̴���, ������, Ŭ���� ID, rustdesk ID �ε�
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
            // ��û�� �����ϴ�.
            yield return webRequest.SendWebRequest();

            // �������κ��� JSON �����͸� �޽��ϴ�.
            string json = webRequest.downloadHandler.text;
            //Debug.Log("Received: " + json);
            TextMeshProUGUI txtSWInfo = SettingManager.Instance.item_SwInfo.transform.Find("btnValueHidden/txtValue").GetComponent<TextMeshProUGUI>();
            // ���� �˻�
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
               // Debug.LogError("Error: " + webRequest.error);
                txtSWInfo.text = $"Ver.{Application.version} (������Ʈ Ȯ�� �Ұ�)";
            }
            else
            {
                // JSON ������ ��ü�� ��ȯ�մϴ�.
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
                    txtSWInfo.text = $"Ver.{Application.version} (������Ʈ Ȯ�� �ʿ�)";
                    swVer = Application.version;
                }
            }
        }
    }

    // ����� �뷮 �ε�
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
        txtStorage.text = $"�� {totalStorage}GB �� {usedStorage}GB �����, {freeStorage}GB ����";
    }
}
