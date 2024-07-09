using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{
    public static TextMeshProUGUI txtCurrentValueStr; // $"{������}��(��)" �� ���� ������ ���ڿ�
    public static TextMeshProUGUI txtChangeValueStr; // $"{���氪}(��)�� �����Ͻðڽ��ϱ�?" �� ���� ������ ���ڿ�
    public static Button btnOK;
    public static Button btnCancel;

    private void Awake()
    {
        txtCurrentValueStr = this.gameObject.transform.Find("ToggleParent/Center/TextObjParent/txt_StrElement").GetComponent<TextMeshProUGUI>();
        txtChangeValueStr = this.gameObject.transform.Find("ToggleParent/Center/TextObjParent/txt_StrValue").GetComponent<TextMeshProUGUI>();
        btnOK = this.gameObject.transform.Find("ToggleParent/Bottom/btn_Confirm").GetComponent<Button>();
        btnCancel = this.gameObject.transform.Find("ToggleParent/Bottom/btn_Cancel").GetComponent<Button>();
        btnCancel.onClick.AddListener(() => { InitToggle(); this.gameObject.SetActive(false); });
    }

    public static void InitToggle()
    {
        txtCurrentValueStr.text = string.Empty;
        txtChangeValueStr.text = string.Empty;
    }
}