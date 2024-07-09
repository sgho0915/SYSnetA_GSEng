using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingItemButtonDataSender : MonoBehaviour
{   
    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => SettingPanelManager.ReceiveSettingItemButtonData(this.gameObject.name, this.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text));        
    }
}
