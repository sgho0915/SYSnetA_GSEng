using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingNumPad : MonoBehaviour
{
    public static TextMeshProUGUI txtCategoryName; // group 태그의 title 값
    public static TextMeshProUGUI txtElementName; // group 태그에 속하는 addr 태그의 name 값
    public static TextMeshProUGUI txtCurrentValue; // 현재값. 단위와 함께 할당됨    
    public static TextMeshProUGUI txtSetValue; // 설정값
    public static TextMeshProUGUI txtSetUnit; // 설정값 단위
    public static TextMeshProUGUI txtRangeStr; // 범위. 기본내용은 $"범위 : {min}~{max}{unit}" 으로 구성됨
    public static Toggle toggleNoUse; // 사용 안함에 대한 토글
    public static TextMeshProUGUI txtNoUseStr; // 사용 안함 값에 대한 문자열
    public static Toggle toggleSetValStr; // 사용 안함에 대한 토글
    public static TextMeshProUGUI txtSetValStr; // 사용 안함 값에 대한 문자열
    public static Button btnOK; // 제어 명령 버튼(OK)

    public static Button btnCancel; //  창 닫기 버튼
    public static Button btn1;
    public static Button btn2;
    public static Button btn3;
    public static Button btn4;
    public static Button btn5;
    public static Button btn6;
    public static Button btn7;
    public static Button btn8;
    public static Button btn9;
    public static Button btn0;
    public static Button btnSub; // 마이너스
    public static Button btnDot; // 점
    public static Button btnDel; // 지우기
    public static Button btnClear; // 클리어

    public static GameObject setValContainer;

    public static bool isNumpadInit = true;

    private void Awake()
    {
        setValContainer = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ValueContainer/SetValueContainer/obj_CurrentValue/obj_Value").gameObject;
        txtCategoryName = this.gameObject.transform.Find("NumPadParent/Top/txt_CategoryName").GetComponent<TextMeshProUGUI>();
        txtElementName = this.gameObject.transform.Find("NumPadParent/Top/txt_ElementName").GetComponent<TextMeshProUGUI>();
        txtCurrentValue = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ValueContainer/CurrentValueContainer/obj_CurrentValue/txt_CurrentValue").GetComponent<TextMeshProUGUI>();
        txtSetValue = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ValueContainer/SetValueContainer/obj_CurrentValue/obj_Value/txt_Value").GetComponent<TextMeshProUGUI>();
        txtSetUnit = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ValueContainer/SetValueContainer/obj_CurrentValue/obj_Value/txt_Unit").GetComponent<TextMeshProUGUI>();
        txtRangeStr = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/txt_RangeValue").GetComponent<TextMeshProUGUI>();
        toggleNoUse = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ToggleParent/Toggle_NoUse").GetComponent<Toggle>();
        txtNoUseStr = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ToggleParent/Toggle_NoUse/txt_NoUseStr").GetComponent<TextMeshProUGUI>();
        toggleSetValStr = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ToggleParent/Toggle_SetValStr").GetComponent<Toggle>();
        txtSetValStr = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/ToggleParent/Toggle_SetValStr/txt_SetValStr").GetComponent<TextMeshProUGUI>();
        btnOK = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/FuncPad/btn_OK").GetComponent<Button>();

        btnCancel = this.gameObject.transform.Find("NumPadParent/Top/btn_Close").GetComponent<Button>();
        btn1 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_1").GetComponent<Button>();
        btn2 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_2").GetComponent<Button>();
        btn3 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_3").GetComponent<Button>();
        btn4 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_4").GetComponent<Button>();
        btn5 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_5").GetComponent<Button>();
        btn6 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_6").GetComponent<Button>();
        btn7 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_7").GetComponent<Button>();
        btn8 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_8").GetComponent<Button>();
        btn9 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_9").GetComponent<Button>();
        btn0 = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_0").GetComponent<Button>();
        btnSub = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_sub").GetComponent<Button>();
        btnDot = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_dot").GetComponent<Button>();
        btnDel = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/FuncPad/btn_Del").GetComponent<Button>();
        btnClear = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/FuncPad/btn_Clear").GetComponent<Button>();

        btnCancel.onClick.AddListener(() => { CloseNumpad(); });
        btn1.onClick.AddListener(() => { Push_1(); });
        btn2.onClick.AddListener(() => { Push_2(); });
        btn3.onClick.AddListener(() => { Push_3(); });
        btn4.onClick.AddListener(() => { Push_4(); });
        btn5.onClick.AddListener(() => { Push_5(); });
        btn6.onClick.AddListener(() => { Push_6(); });
        btn7.onClick.AddListener(() => { Push_7(); });
        btn8.onClick.AddListener(() => { Push_8(); });
        btn9.onClick.AddListener(() => { Push_9(); });
        btn0.onClick.AddListener(() => { Push_0(); });
        btnSub.onClick.AddListener(() => { Push_Sub(); });
        btnDot.onClick.AddListener(() => { Push_Dot(); });
        btnDel.onClick.AddListener(() => { Push_Del(); });
        btnClear.onClick.AddListener(() => { Push_Clear(); });
    }

    // tag 태그의 style 속성값이 numpad인 설정 요소 버튼을 클릭할 경우 초기화를 먼저 진행함
    public static void InitNumpad()
    {
        txtCategoryName.text = string.Empty;
        txtElementName.text = string.Empty;
        txtSetValue.text = string.Empty;
        txtCurrentValue.text = string.Empty;
        txtSetUnit.text = string.Empty;        
        txtRangeStr.text = string.Empty;
        txtNoUseStr.text = string.Empty;
        txtSetValStr.text = string.Empty;
        toggleNoUse.isOn = false;
        toggleSetValStr.isOn = false;
        isNumpadInit = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());        
    }

    public void CloseNumpad()
    {
        InitNumpad();
        this.gameObject.SetActive(false);
    }

    public void Push_Del()
    {
        if (!toggleNoUse.isOn && !toggleSetValStr.isOn)
        {
            if (!string.IsNullOrEmpty(txtSetValue.text))
            {
                if (isNumpadInit || txtSetValue.text == "0" || txtSetValue.text.Length == 1)
                {
                    txtSetValue.text = "0";
                }
                else
                {
                    txtSetValue.text = txtSetValue.text.Substring(0, txtSetValue.text.Length - 1);
                }
            }
            isNumpadInit = false;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_Clear()
    {
        if (!toggleNoUse.isOn && !toggleSetValStr.isOn)
        {
            if (isNumpadInit || txtSetValue.text == "0")
            {
                txtSetValue.text = "0";
            }
            else
            {
                txtSetValue.text = "0";
            }
            isNumpadInit=false;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_Dot()
    {
        if (!toggleNoUse.isOn && !toggleSetValStr.isOn)
        {
            if (txtSetValue.text.Contains("."))
            {
                // 이미 '.'이 문자열에 존재할 경우 아무런 동작을 수행하지 않도록 상태 유지
            }
            else
            {
                txtSetValue.text += ".";
            }
            isNumpadInit = false;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_Sub()
    {
        if (!toggleNoUse.isOn && !toggleSetValStr.isOn)
        {
            if (txtSetValue.text == "0")
            {
                txtSetValue.text = txtSetValue.text.Substring(1);
                txtSetValue.text += "-";
            }
            else
            {
                // txtSetValue.text 값이 음수인지 확인
                if (txtSetValue.text.StartsWith("-"))
                {
                    // 이미 '-' 기호가 있다면 제거
                    txtSetValue.text = txtSetValue.text.Substring(1);
                }
                else
                {
                    // '-' 기호가 없다면 맨 앞에 추가
                    txtSetValue.text = "-" + txtSetValue.text;
                }
            }
            isNumpadInit = false;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_0()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "0";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "0";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_1()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "1";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "1";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_2()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "2";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "2";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_3()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "3";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "3";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_4()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "4";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "4";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_5()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "5";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "5";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_6()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "6";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "6";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_7()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "7";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "7";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_8()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "8";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "8";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }

    public void Push_9()
    {
        if (toggleNoUse.isOn || toggleSetValStr.isOn)
        {
            toggleNoUse.isOn = false;
            toggleSetValStr.isOn = false;
        }

        if (isNumpadInit || txtSetValue.text == "0")
        {
            txtSetValue.text = "9";
        }
        else
        {
            if (!(txtSetValue.text.Length >= 8))
                txtSetValue.text += "9";
        }
        isNumpadInit = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(setValContainer.GetComponent<RectTransform>());
    }
}
