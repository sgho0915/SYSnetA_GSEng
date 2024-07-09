using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;

public class SetScreenPW : MonoBehaviour
{
    public static SetScreenPW Instance { get; private set; }

    public TextMeshProUGUI txtPW;
    public TextMeshProUGUI txtConfirmPW;
    public TextMeshProUGUI txtAnnounce;

    public GameObject setRepeatValueContainer;

    public Button btnCancel; //  창 닫기 버튼
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button btn5;
    public Button btn6;
    public Button btn7;
    public Button btn8;
    public Button btn9;
    public Button btn0;
    public Button btnDel; // 지우기
    public Button btnClear; // 클리어

    string inputPW = string.Empty;
    string inputConfirmPW = string.Empty;

    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        txtPW = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/SetValueContainer/obj_CurrentValue/txt_Value").GetComponent<TextMeshProUGUI>();
        txtConfirmPW = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/SetRepeatValueContainer/obj_CurrentValue/txt_Value").GetComponent<TextMeshProUGUI>();
        txtAnnounce = this.gameObject.transform.Find("NumPadParent/Center/ValueParent/txt_Announce").GetComponent<TextMeshProUGUI>();

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
        btnDel = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_Del").GetComponent<Button>();
        btnClear = this.gameObject.transform.Find("NumPadParent/Bottom/KeyPadParent/Numpad/btn_Clear").GetComponent<Button>();

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
        btnDel.onClick.AddListener(() => { Push_Del(); });
        btnClear.onClick.AddListener(() => { Push_Clear(); });
    }

    public void InitPassword()
    {
        txtPW.text = string.Empty;
        inputPW = string.Empty;
        txtConfirmPW.text = string.Empty;
        inputConfirmPW = string.Empty;

        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            setRepeatValueContainer.SetActive(false);
            txtAnnounce.text = "기존 비밀번호 네 자리를 입력해주세요.";
        }

        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            setRepeatValueContainer.SetActive(false);
            txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
        }
    }

    public void CloseNumpad()
    {
        InitPassword();
        ScreenLockManager.Instance.isSetPWMode = true;
        ScreenLockManager.Instance.isUnlockMode = false;
        this.gameObject.SetActive(false);
    }

    public void ComparePW(string input, string inputConfirm)
    {
        //Debug.Log($"input:{input}, inputConfirm:{inputConfirm}, config:{ConfigManager.Instance.GetSetting("LOCK_PW")}");
        if(ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (input == ConfigManager.Instance.GetSetting("LOCK_PW"))
                {
                    setRepeatValueContainer.SetActive(true);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(this.gameObject.transform.Find("NumPadParent/Center/ValueParent").GetComponent<RectTransform>());
                    txtPW.text = string.Empty;
                    inputPW = string.Empty;
                    txtConfirmPW.text = string.Empty;
                    inputConfirmPW = string.Empty;
                    ScreenLockManager.Instance.isModifyMode = true;
                    txtAnnounce.text = "새로운 비밀번호 네 자리를 입력해주세요.";
                }
                else
                {
                    txtAnnounce.text = "입력한 비밀번호가 일치하지 않습니다.";
                    txtPW.text = string.Empty;
                    inputPW = string.Empty;
                    txtConfirmPW.text = string.Empty;
                    inputConfirmPW = string.Empty;

                    // 색상 변경
                    txtAnnounce.DOColor(new Color32(240, 94, 51, 255), 0.5f);

                    // 흔들림 효과
                    txtAnnounce.transform.DOShakePosition(0.5f, strength: new Vector3(10, 0, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);

                }
            }
            else
            {
                if (input == inputConfirm)
                {
                    ConfigManager.Instance.SetSetting("LOCK_PW", inputConfirm); // 비밀번호 설정
                    this.gameObject.SetActive(false);
                    ScreenLockManager.Instance.isModifyMode = false;

                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                    ScreenManager.Instance.txt_PopUpMsg.text = "비밀번호가 설정되었습니다.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
                else
                {
                    txtAnnounce.text = "입력한 비밀번호가 일치하지 않습니다.";
                    txtPW.text = string.Empty;
                    inputPW = string.Empty;
                    txtConfirmPW.text = string.Empty;
                    inputConfirmPW = string.Empty;

                    // 색상 변경
                    txtAnnounce.DOColor(new Color32(240, 94, 51, 255), 0.5f);

                    // 흔들림 효과
                    txtAnnounce.transform.DOShakePosition(0.5f, strength: new Vector3(10, 0, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);
                }
            }             
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if(input == ConfigManager.Instance.GetSetting("LOCK_PW"))
            {
                ConfigManager.Instance.SetSetting("LOCK_STATE", "false");
                ScreenLockManager.Instance.btnLock.transform.Find("Lock").gameObject.SetActive(false);
                ScreenLockManager.Instance.btnLock.transform.Find("Unlock").gameObject.SetActive(true);
                ScreenLockManager.Instance.lockScreen.SetActive(false);
                this.gameObject.SetActive(false);

                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                ScreenManager.Instance.txt_PopUpMsg.text = "화면 잠금이 해제되었습니다.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                    
                });
            }
            else
            {
                txtAnnounce.text = "입력한 비밀번호가 일치하지 않습니다.";
                txtPW.text = string.Empty;
                inputPW = string.Empty;

                // 색상 변경
                txtAnnounce.DOColor(new Color32(240, 94, 51, 255), 0.5f);

                // 흔들림 효과
                txtAnnounce.transform.DOShakePosition(0.5f, strength: new Vector3(10, 0, 0), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);

            }
        }
    }

    public void Push_Del()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length > 0)
            {
                if (txtConfirmPW.text.Length > 0)
                {
                    txtConfirmPW.text = txtConfirmPW.text.Substring(0, txtConfirmPW.text.Length - 1);
                    inputConfirmPW = inputConfirmPW.Substring(0, inputConfirmPW.Length - 1);
                    txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    txtPW.text = txtPW.text.Substring(0, txtPW.text.Length - 1);
                    inputPW = inputPW.Substring(0, inputPW.Length - 1);
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                }
            }
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length > 0)
            {
                txtPW.text = txtPW.text.Substring(0, txtPW.text.Length - 1);
                inputPW = inputPW.Substring(0, inputPW.Length - 1);
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
            }
        }
    }

    public void Push_Clear()
    {
        txtAnnounce.color = Color.black;
        txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
        txtPW.text = string.Empty;
        inputPW = string.Empty;
        txtConfirmPW.text = string.Empty;
        inputConfirmPW = string.Empty;
    }

    public void Push_0()
    {
        txtAnnounce.color = Color.black;

        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "0";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "0";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "0";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "0";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "0";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_1()
    {
        txtAnnounce.color = Color.black;

        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "1";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "1";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "1";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "1";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "1";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_2()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "2";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "2";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "2";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "2";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "2";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_3()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "3";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "3";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "3";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "3";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "3";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_4()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "4";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "4";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "4";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "4";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "4";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_5()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "5";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "5";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "5";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "5";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "5";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_6()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "6";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "6";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "6";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "6";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }             
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "6";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_7()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "7";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "7";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "7";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "7";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "7";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_8()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "8";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "8";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "8";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "8";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "8";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }

    public void Push_9()
    {
        txtAnnounce.color = Color.black;
        if (ScreenLockManager.Instance.isSetPWMode && !ScreenLockManager.Instance.isUnlockMode)
        {
            if (!ScreenLockManager.Instance.isModifyMode)
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "9";
                    if (txtPW.text.Length == 4)
                        ComparePW(inputPW, inputConfirmPW);
                }
            }
            else
            {
                if (txtPW.text.Length <= 3)
                {
                    txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                    txtPW.text += "●";
                    inputPW += "9";
                    if (txtPW.text.Length == 4)
                        txtAnnounce.text = "비밀번호 네 자리를 다시 입력해주세요.";
                }
                else
                {
                    if (txtConfirmPW.text.Length < 3)
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "9";
                    }
                    else
                    {
                        txtConfirmPW.text += "●";
                        inputConfirmPW += "9";
                        ComparePW(inputPW, inputConfirmPW);
                    }
                }
            }            
        }
        if (!ScreenLockManager.Instance.isSetPWMode && ScreenLockManager.Instance.isUnlockMode)
        {
            if (txtPW.text.Length <= 3)
            {
                txtAnnounce.text = "비밀번호 네 자리를 입력해주세요.";
                txtPW.text += "●";
                inputPW += "9";
                if (txtPW.text.Length == 4)
                    ComparePW(inputPW, inputConfirmPW);
            }
        }
    }
}
