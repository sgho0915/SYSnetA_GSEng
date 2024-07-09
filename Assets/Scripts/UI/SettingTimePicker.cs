using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Serialization;

public class SettingTimePicker : MonoBehaviour
{
    public static TextMeshProUGUI txtElementName; // group �±׿� ���ϴ� addr �±��� name ��
    public static TextMeshProUGUI txtCurrentValue; // ���簪. $"���簪 : {�ð� �Ǵ� �� ��}{�� �Ǵ� �� ����}{�� �Ǵ� �� ��}{�� �Ǵ� �� ����}"
    public static TextMeshProUGUI txtRangeValue; // ������. $"���� : {��������Ʈ(��) �ּҰ�}{����}{��������Ʈ(��)�ּҰ�}{����} ~ {��������Ʈ(��)�ִ밪}{����}{��������Ʈ(��)�ִ밪}{����}"
    public static TextMeshProUGUI txtSetHourValue; // ���� �����ϰ��� �ϴ� �� �Ǵ� �� ���� ���� ��
    public static TextMeshProUGUI txtSetMinValue; // ���� �����ϰ��� �ϴ� �� �Ǵ� �� ���� ���� ��
    public static TextMeshProUGUI txtSetHourUnit; // ���� �����ϰ��� �ϴ� �� �Ǵ� �� ���� ���� ����
    public static TextMeshProUGUI txtSetMinUnit; // ���� �����ϰ��� �ϴ� �� �Ǵ� �� ���� ���� ����

    public static Button btnHourValueUp; // ���� �ð� �� ���� ��ư
    public static Button btnHourValueDown; // ���� �ð� �� ���� ��ư
    public static Button btnMinValueUp; // ���� �ð� �� ���� ��ư
    public static Button btnMinValueDown; // ���� �ð� �� ���� ��ư

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
