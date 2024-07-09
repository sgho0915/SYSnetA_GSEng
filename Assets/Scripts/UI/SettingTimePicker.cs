using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;

public class SettingTimePicker : MonoBehaviour
{
    public static TextMeshProUGUI txtElementName; // group 태그에 속하는 addr 태그의 name 값
    public static TextMeshProUGUI txtCurrentValue; // 현재값. $"현재값 : {시간 또는 월 값}{시 또는 월 단위}{분 또는 일 값}{분 또는 일 단위}"
    public static TextMeshProUGUI txtRangeValue; // 범위값. $"범위 : {상위바이트(시) 최소값}{단위}{하위바이트(분)최소값}{단위} ~ {상위바이트(시)최대값}{단위}{하위바이트(분)최대값}{단위}"
    public static TextMeshProUGUI txtSetHourValue; // 새로 설정하고자 하는 시 또는 월 단위 설정 값
    public static TextMeshProUGUI txtSetMinValue; // 새로 설정하고자 하는 분 또는 일 단위 설정 값
    public static TextMeshProUGUI txtSetHourUnit; // 새로 설정하고자 하는 시 또는 월 단위 설정 범위
    public static TextMeshProUGUI txtSetMinUnit; // 새로 설정하고자 하는 분 또는 일 단위 설정 범위

    public static Button btnHourValueUp; // 설정 시간 값 증가 버튼
    public static Button btnHourValueDown; // 설정 시간 값 감소 버튼
    public static Button btnMinValueUp; // 설정 시간 값 증가 버튼
    public static Button btnMinValueDown; // 설정 시간 값 감소 버튼

    public static Button btnOK;
    public static Button btnCancel;

    private void Awake()
    {
        txtElementName = this.gameObject.transform.Find("TimePickerParent/Center/TextObjParent/txt_Title").GetComponent<TextMeshProUGUI>();
        txtCurrentValue = this.gameObject.transform.Find("TimePickerParent/Center/TextObjParent/txt_CurrentValue").GetComponent<TextMeshProUGUI>();
        txtRangeValue = this.gameObject.transform.Find("TimePickerParent/Center/TextObjParent/txt_RangeValue").GetComponent<TextMeshProUGUI>();
        txtSetHourValue = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetMonthOrHourParent/ValueParent/ValueContainer/txt_Value").GetComponent<TextMeshProUGUI>();
        txtSetHourUnit = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetMonthOrHourParent/ValueParent/txt_Unit").GetComponent<TextMeshProUGUI>();
        txtSetMinValue = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetDayOrMinuteParent/ValueParent/ValueContainer/txt_Value").GetComponent<TextMeshProUGUI>();
        txtSetMinUnit = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetDayOrMinuteParent/ValueParent/txt_Unit").GetComponent<TextMeshProUGUI>();

        btnHourValueUp = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetMonthOrHourParent/ValueUpParent/btn_ValueUp").GetComponent<Button>();
        btnHourValueDown = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetMonthOrHourParent/ValueDownParent/btn_ValueDown").GetComponent<Button>();
        btnMinValueUp = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetDayOrMinuteParent/ValueUpParent/btn_ValueUp").GetComponent<Button>();
        btnMinValueDown = this.gameObject.transform.Find("TimePickerParent/Center/SettingObjParent/SetDayOrMinuteParent/ValueDownParent/btn_ValueDown").GetComponent<Button>();

        btnOK = this.gameObject.transform.Find("TimePickerParent/Bottom/btn_Save").GetComponent<Button>();
        btnCancel = this.gameObject.transform.Find("TimePickerParent/Bottom/btn_Cancel").GetComponent<Button>();

        btnCancel.onClick.AddListener(() => { InitTimePicker(); this.gameObject.SetActive(false); });
    }

    public static void InitTimePicker()
    {
        txtElementName.text = string.Empty;
        txtCurrentValue.text = string.Empty;
        txtRangeValue.text = string.Empty;
        txtSetHourValue.text = string.Empty;
        txtSetHourUnit.text = string.Empty;
        txtSetMinValue.text = string.Empty;
        txtSetMinUnit.text = string.Empty;

        btnHourValueUp.onClick.RemoveAllListeners();
        btnHourValueDown.onClick.RemoveAllListeners();
        btnMinValueUp.onClick.RemoveAllListeners();
        btnMinValueDown.onClick.RemoveAllListeners();
    }
}
