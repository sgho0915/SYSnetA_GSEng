using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{
    public static TextMeshProUGUI txtCurrentValueStr; // $"{기존값}을(를)" 와 같은 형태의 문자열
    public static TextMeshProUGUI txtChangeValueStr; // $"{변경값}(으)로 변경하시겠습니까?" 와 같은 형태의 문자열
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