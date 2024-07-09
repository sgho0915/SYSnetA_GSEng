using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingDropdown : MonoBehaviour
{
    public static TextMeshProUGUI txtElementName; // group 태그에 속하는 tag 태그의 name 값
    public static TMP_Dropdown dropdownElements; // tag 태그의 items 요소가 담길 dropdown
    public static Button btnOK; // 제어 명령 버튼
    public static Button btnCancel; // 창 닫기
    
    private void Awake()
    {
        txtElementName = this.gameObject.transform.Find("DropdownParent/Center/txt_ElementName").GetComponent<TextMeshProUGUI>();
        dropdownElements = this.gameObject.transform.Find("DropdownParent/Center/Dropdown").GetComponent<TMP_Dropdown>();
        btnOK = this.gameObject.transform.Find("DropdownParent/Bottom/btn_Confirm").GetComponent<Button>();
        btnCancel = this.gameObject.transform.Find("DropdownParent/Bottom/btn_Cancel").GetComponent<Button>();
        btnCancel.onClick.AddListener(() => { InitDropdown(); this.gameObject.SetActive(false); });
    }

    public static void InitDropdown()
    {
        txtElementName.text = string.Empty;
        dropdownElements.ClearOptions();        
    }
}
