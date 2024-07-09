using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingDetailItemButtonDataSender : MonoBehaviour
{    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => SettingPanelManager.ReceiveSettingDetailItemButtonData(this.gameObject.name, this.transform.Find("txt_Title").GetComponent<TextMeshProUGUI>().text, this.transform.Find("txt_CurValue").GetComponent<TextMeshProUGUI>().text, this.transform.Find("txt_BitValue").GetComponent<TextMeshProUGUI>().text));
    }
}
