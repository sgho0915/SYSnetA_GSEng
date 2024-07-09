using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingDropdown : MonoBehaviour
{
    public static TextMeshProUGUI txtElementName; // group �±׿� ���ϴ� tag �±��� name ��
    public static TMP_Dropdown dropdownElements; // tag �±��� items ��Ұ� ��� dropdown
    public static Button btnOK; // ���� ��� ��ư
    public static Button btnCancel; // â �ݱ�
    
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
