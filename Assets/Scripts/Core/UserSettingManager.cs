using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Renci.SshNet.Security;
using System.Security.Cryptography;
using System;
using System.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.SceneManagement;

public class UserSettingManager : MonoBehaviour
{
    public static UserSettingManager Instance { get; private set; }
    private Dictionary<string, GameObject> userElements = new Dictionary<string, GameObject>();

    [Header("���� ���� ���� ȭ��")]
    public GameObject userElementScrollView;
    public GameObject userPrefab;
    public static Transform userContent;
    public Button btnAdd;
    public Button btnModify;
    public Button btnRemove;
    public Button btnSendTestPush;

    [Header("���� �߰�/����")]
    public GameObject settingUserScreen;
    public TMP_InputField inputfield_ID;
    public TMP_InputField inputfield_PW;
    public TMP_InputField inputfield_PWCheck;
    public TMP_InputField inputfield_PWHint;
    public TMP_InputField inputfield_Name;
    public TMP_InputField inputfield_Email;
    public TMP_InputField inputfield_Phone;
    public TMP_Dropdown dropdown_Auth;
    public Toggle toggle_sms;
    public Toggle toggle_push;
    public Button btnSave;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        userContent = userElementScrollView.transform.Find("Viewport/Content");
    }

    public void LoadUserManagement()
    {
        DataTable tableUser = ClientDatabase.FetchUserData().Tables[0];
        btnAdd.onClick.RemoveAllListeners();
        btnAdd.onClick.AddListener(() => AddUser());
        btnModify.onClick.RemoveAllListeners();
        btnModify.onClick.AddListener(() => ModifyUser());
        btnRemove.onClick.RemoveAllListeners();
        btnRemove.onClick.AddListener(() => RemoveUser());
        btnSendTestPush.onClick.RemoveAllListeners();
        btnSendTestPush.onClick.AddListener(() => SendTestPush());

        foreach (DataRow userRow in tableUser.Rows)
        {
            string userID = userRow["FLD_ID"].ToString();
            string userName = userRow["FLD_NAME"].ToString();
            string userPW = userRow["FLD_PW"].ToString();
            string userEmail = userRow["FLD_EMAIL"].ToString();
            string userPhone = userRow["FLD_SMS_PHONE"].ToString();
            string userRecvSMS = userRow["FLD_RECV_SMS"].ToString();
            string userRecvPush = userRow["FLD_RECV_PUSH"].ToString();
            string userAuth = userRow["FLD_AUTH"].ToString();
            string userAccess = userRow["FLD_ACCESS"].ToString();
            string userLanguage = userRow["FLD_LANGUAGE"].ToString();
            string userFCMToken = userRow["FLD_FCM_TOKEN"].ToString();
            string userDesc = userRow["FLD_DESC"].ToString();

            string userElementName = $"user_{userID}";

            if (userElements.TryGetValue(userElementName, out var userElement))
            {
                // ���� ���� ��Ұ� ������ ���븸 ������Ʈ
                Toggle toggleSelect = userElement.transform.Find("obj_Select/Toggle").GetComponent<Toggle>();
                TextMeshProUGUI txtID = userElement.transform.Find("obj_ID/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtName = userElement.transform.Find("obj_Name/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtEmail = userElement.transform.Find("obj_Email/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtPhoneNum = userElement.transform.Find("obj_PhoneNum/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtSMSRecv = userElement.transform.Find("obj_SMS_Recv/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtPushRecv = userElement.transform.Find("obj_Push_Recv/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtAuth = userElement.transform.Find("obj_Auth/text").GetComponent<TextMeshProUGUI>();

                toggleSelect.group = userContent.GetComponent<ToggleGroup>();

                userElement.name = userElementName;
                txtID.text = userID;
                txtName.text = userName;
                txtEmail.text = userEmail;
                txtPhoneNum.text = userPhone;
                txtSMSRecv.text = userRecvSMS == "1" ? "Y" : "N";
                txtPushRecv.text = userRecvPush == "1" ? "Y" : "N";
                switch (userAuth)
                {
                    case "0":
                        txtAuth.text = "�ְ� ������";
                        break;
                    case "1":
                        txtAuth.text = "������";
                        break;
                    case "2":
                        txtAuth.text = "�Ϲ� �����";
                        break;
                }
            }
            else
            {
                GameObject newUserElement = ObjectPool.Instance.GetUserElement();

                userElements[userElementName] = newUserElement;

                Toggle toggleSelect = newUserElement.transform.Find("obj_Select/Toggle").GetComponent<Toggle>();
                TextMeshProUGUI txtID = newUserElement.transform.Find("obj_ID/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtName = newUserElement.transform.Find("obj_Name/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtEmail = newUserElement.transform.Find("obj_Email/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtPhoneNum = newUserElement.transform.Find("obj_PhoneNum/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtSMSRecv = newUserElement.transform.Find("obj_SMS_Recv/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtPushRecv = newUserElement.transform.Find("obj_Push_Recv/text").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI txtAuth = newUserElement.transform.Find("obj_Auth/text").GetComponent<TextMeshProUGUI>();

                toggleSelect.group = userContent.GetComponent<ToggleGroup>();

                newUserElement.name = userElementName;
                txtID.text = userID;
                txtName.text = userName;
                txtEmail.text = userEmail;
                txtPhoneNum.text = userPhone;
                txtSMSRecv.text = userRecvSMS == "1" ? "Y" : "N";
                txtPushRecv.text = userRecvPush == "1" ? "Y" : "N";
                switch (userAuth)
                {
                    case "0":
                        txtAuth.text = "�ְ� ������";
                        break;
                    case "1":
                        txtAuth.text = "������";
                        break;
                    case "2":
                        txtAuth.text = "�Ϲ� �����";
                        break;
                }
            }
        }        
    }

    public void InitUserScreen()
    {
        inputfield_ID.text = string.Empty;
        inputfield_PW.text = string.Empty;
        inputfield_PWCheck.text = string.Empty;
        inputfield_PWHint.text = string.Empty;
        inputfield_Name.text = string.Empty;
        inputfield_Email.text = string.Empty;
        inputfield_Phone.text = string.Empty;
        dropdown_Auth.ClearOptions();
        toggle_sms.isOn = false;
        toggle_push.isOn = false;
        inputfield_ID.interactable = true;
    }

    public void AddUser()
    {
        settingUserScreen.SetActive(true);
        Button btnClose = settingUserScreen.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            InitUserScreen();
            settingUserScreen.SetActive(false);
        });

        TextMeshProUGUI txtTitle = settingUserScreen.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
        txtTitle.text = "���� �߰�";
        TextMeshProUGUI txtBtnText = settingUserScreen.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
        txtBtnText.text = "�߰�";

        InitUserScreen();

        int iSMS = -1;
        int iPush = -1;
        int iAuth = -1;

        dropdown_Auth.ClearOptions();
        List<string> auths = new List<string> { "������", "�Ϲ� �����" };
        List<TMP_Dropdown.OptionData> options_auths = new List<TMP_Dropdown.OptionData>();
        foreach (string auth in auths)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(auth);
            options_auths.Add(option);
        }
        dropdown_Auth.AddOptions(options_auths);
        if (dropdown_Auth.options.Count > 0)
            dropdown_Auth.value = 0;


        toggle_sms.onValueChanged.AddListener((value) =>
        {
            if (value)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Notification;
                ScreenManager.Instance.txt_PopUpMsg.text = "SMS �߼� ����� �����ڵ�\n(https://www.icodekorea.com/)���� ����,\n��� ���� �� ��� �����մϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
            }
        });

        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            #region ��������
            // ��й�ȣ�� ���� �ٸ� ��� ����
            DataTable tableUser = ClientDatabase.FetchUserData().Tables[0];
            foreach (DataRow userRow in tableUser.Rows)
            {
                string userID = userRow["FLD_ID"].ToString();

                if(inputfield_ID.text == userID)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "������ ���̵� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                    return;
                }
            }

            if (inputfield_PW.text != inputfield_PWCheck.text)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "�Է��� ��й�ȣ�� ���� ��ġ���� �ʽ��ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }

            if (inputfield_ID.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "���̵� �Է����ּ���.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }

            if (inputfield_PW.text == string.Empty || inputfield_PWCheck.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "��й�ȣ�� �Է����ּ���.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();                    
                });
                return;
            }

            if (toggle_sms.isOn == true && inputfield_Phone.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "�޴��� ��ȣ�� �Է����ּ���.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }
            #endregion

            if (toggle_sms.isOn) iSMS = 1;
            else iSMS = 0;

            if (toggle_push.isOn) iPush = 1;
            else iPush = 0;

            iAuth = dropdown_Auth.value + 1;

            string tblUserQuery = $"INSERT INTO TBL_USER (FLD_ID, FLD_NAME, FLD_PW, FLD_EMAIL, FLD_SMS_PHONE, FLD_RECV_SMS, FLD_RECV_PUSH, FLD_AUTH, FLD_DESC) VALUES ('{inputfield_ID.text}', '{inputfield_Name.text}', '{inputfield_PW.text}', '{inputfield_Email.text}', '{inputfield_Phone.text}', {iSMS}, {iPush}, {iAuth}, '{inputfield_PWHint.text}')";

            try
            {
                if (ClientDatabase.OnInsertRequest(tblUserQuery))
                {
                    LoadUserManagement();
                    InitUserScreen();
                    settingUserScreen.SetActive(false);
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "���� �߰��� �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }            
        });
    }

    public void ModifyUser()
    {
        var selectedUser = userContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ����
        if (selectedUser == null)
        {
            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            ScreenManager.Instance.txt_PopUpMsg.text = "������ ���� �������ּ���.";
            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
            {
                ScreenManager.Instance.ClosePopUpMessage();
            });
            return; // ���õ� ����� ������ ����
        }
        Transform parent = selectedUser.transform.parent; // ù ��° �θ�
        Transform grandParent = parent != null ? parent.parent : null; // �� ��° �θ� (�θ��� �θ�)

        string selectedUserID = string.Empty;
        // �θ��� �θ� ������Ʈ�� null�� �ƴ��� Ȯ���� �� �̸��� ����
        if (grandParent != null)
        {
            selectedUserID = grandParent.gameObject.name.Replace("user_", "");            
        }

        //string selectedUserID = selectedUser.gameObject.name.Replace("user_", "");
        string userID = string.Empty;
        string userName = string.Empty;
        string userPW = string.Empty;
        string userEmail = string.Empty;
        string userPhone = string.Empty;
        string userRecvSMS = string.Empty;
        string userRecvPush = string.Empty;
        int userAuth = 0;
        string userAccess = string.Empty;
        string userLanguage = string.Empty;
        string userFCMToken = string.Empty;
        string userDesc = string.Empty;

        DataTable tableUser = ClientDatabase.FetchUserData().Tables[0];
        foreach (DataRow userRow in tableUser.Rows)
        {
            if (selectedUserID == userRow["FLD_ID"].ToString())
            {
                userID = userRow["FLD_ID"].ToString();
                userName = userRow["FLD_NAME"].ToString();
                userPW = userRow["FLD_PW"].ToString();
                userEmail = userRow["FLD_EMAIL"].ToString();
                userPhone = userRow["FLD_SMS_PHONE"].ToString();
                userRecvSMS = userRow["FLD_RECV_SMS"].ToString();
                userRecvPush = userRow["FLD_RECV_PUSH"].ToString();
                userAuth = int.Parse(userRow["FLD_AUTH"].ToString());
                userAccess = userRow["FLD_ACCESS"].ToString();
                userLanguage = userRow["FLD_LANGUAGE"].ToString();
                userFCMToken = userRow["FLD_FCM_TOKEN"].ToString();
                userDesc = userRow["FLD_DESC"].ToString();
            }
        }


        settingUserScreen.SetActive(true);
        Button btnClose = settingUserScreen.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/btn_Close").GetComponent<Button>();
        btnClose.onClick.RemoveAllListeners();
        btnClose.onClick.AddListener(() =>
        {
            InitUserScreen();
            settingUserScreen.SetActive(false);
        });

        TextMeshProUGUI txtTitle = settingUserScreen.transform.Find("SettingControllerParent/obj_Setting_Controller/Title/txt_BottomTitle").GetComponent<TextMeshProUGUI>();
        txtTitle.text = "���� ����";
        TextMeshProUGUI txtBtnText = settingUserScreen.transform.Find("SettingControllerParent/obj_Setting_Controller/Bottom/btn_Save/Text (TMP)").GetComponent<TextMeshProUGUI>();
        txtBtnText.text = "����";

        InitUserScreen();

        int iSMS = -1;
        int iPush = -1;
        int iAuth = -1;

        dropdown_Auth.ClearOptions();
        List<string> auths = new List<string> { "������", "�Ϲ� �����" };
        List<TMP_Dropdown.OptionData> options_auths = new List<TMP_Dropdown.OptionData>();
        foreach (string auth in auths)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(auth);
            options_auths.Add(option);
        }
        dropdown_Auth.AddOptions(options_auths);
        if (dropdown_Auth.options.Count > 0)
            dropdown_Auth.value = userAuth - 1;

        inputfield_ID.text = userID;
        inputfield_ID.interactable = false;
        inputfield_PW.text = userPW;
        inputfield_PWCheck.text = string.Empty;
        inputfield_PWHint.text = userDesc;
        inputfield_Name.text = userName;
        inputfield_Email.text = userEmail;
        inputfield_Phone.text = userPhone;
        toggle_sms.isOn = userRecvSMS == "1" ? true : false;
        toggle_push.isOn = userRecvPush == "1" ? true : false;

        if (!toggle_sms.isOn)
        {
            toggle_sms.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Notification;
                    ScreenManager.Instance.txt_PopUpMsg.text = "SMS �߼� ����� �����ڵ�\n(https://www.icodekorea.com/)���� ����,\n��� ���� �� ��� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
            });
        }
        else
        { 
            ScreenManager.Instance.ClosePopUpMessage();
            toggle_sms.onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Notification;
                    ScreenManager.Instance.txt_PopUpMsg.text = "SMS �߼� ����� �����ڵ�\n(https://www.icodekorea.com/)���� ����,\n��� ���� �� ��� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
            });
        }

        

        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            #region ��������
            // ��й�ȣ�� ���� �ٸ� ��� ����
            if (inputfield_PW.text != inputfield_PWCheck.text)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "�Է��� ��й�ȣ�� ���� ��ġ���� �ʽ��ϴ�.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }

            if (inputfield_ID.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "���̵� �Է����ּ���.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }

            if (inputfield_PW.text == string.Empty || inputfield_PWCheck.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "��й�ȣ�� �Է����ּ���.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }

            if (toggle_sms.isOn == true && inputfield_Phone.text == string.Empty)
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                ScreenManager.Instance.txt_PopUpMsg.text = "�޴��� ��ȣ�� �Է����ּ���.";
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                {
                    ScreenManager.Instance.ClosePopUpMessage();
                });
                return;
            }
            #endregion

            if (toggle_sms.isOn) iSMS = 1;
            else iSMS = 0;

            if (toggle_push.isOn) iPush = 1;
            else iPush = 0;

            iAuth = dropdown_Auth.value + 1;

            string tblUserQuery = $"UPDATE TBL_USER SET FLD_ID = '{inputfield_ID.text}', FLD_NAME = '{inputfield_Name.text}', FLD_PW = '{inputfield_PW.text}', FLD_EMAIL = '{inputfield_Email.text}', FLD_SMS_PHONE = '{inputfield_Phone.text}', FLD_RECV_SMS = {iSMS}, FLD_RECV_PUSH = {iPush}, FLD_AUTH = {iAuth}, FLD_DESC = '{inputfield_PWHint.text}' WHERE FLD_ID = '{selectedUserID}'";
           
            try
            {
                if (ClientDatabase.OnUpdateRequest(tblUserQuery))
                {
                    LoadUserManagement();
                    InitUserScreen();
                    settingUserScreen.SetActive(false);
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "���� ������ �����߽��ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        });
    }

    public void RemoveUser()
    {
        var selectedUser = userContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ����
        Transform parent = selectedUser.transform.parent; // ù ��° �θ�
        Transform grandParent = parent != null ? parent.parent : null; // �� ��° �θ� (�θ��� �θ�)

        string selectedUserID = string.Empty;
        // �θ��� �θ� ������Ʈ�� null�� �ƴ��� Ȯ���� �� �̸��� ����
        if (grandParent != null)
        {
            selectedUserID = grandParent.gameObject.name.Replace("user_", "");
        }

        string userID = string.Empty;

        DataTable tableUser = ClientDatabase.FetchUserData().Tables[0];
        foreach (DataRow userRow in tableUser.Rows)
        {
            if (selectedUserID == userRow["FLD_ID"].ToString())
            {
                ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Delete;
                ScreenManager.Instance.txt_PopUpMsg.text = $"������ ������ �����Ͻðڽ��ϱ�?";
                ScreenManager.Instance.btnPopUpCancel.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpCancel.onClick.AddListener(() => ScreenManager.Instance.ClosePopUpMessage());
                ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() => 
                {
                    //string tblUserQuery = $"UPDATE TBL_USER SET FLD_ID = '{inputfield_ID.text}', FLD_NAME = '{inputfield_Name.text}', FLD_PW = '{inputfield_PW.text}', FLD_EMAIL = '{inputfield_Email.text}', FLD_SMS_PHONE = '{inputfield_Phone.text}', FLD_RECV_SMS = {iSMS}, FLD_RECV_PUSH = {iPush}, FLD_AUTH = {iAuth}, FLD_DESC = '{inputfield_PWHint.text}' WHERE FLD_ID = '{selectedUserID}'";
                    string tblUserQuery = $"DELETE FROM TBL_USER WHERE `FLD_ID` = '{selectedUserID}';";
                    try
                    {
                        if (ClientDatabase.OnDeleteRequest(tblUserQuery))
                        {
                            for (int i = ObjectPool.Instance.UserObjects.Count - 1; i >= 0; i--)
                            {
                                if (ObjectPool.Instance.UserObjects[i] == grandParent.gameObject)
                                {
                                    ObjectPool.Instance.UserObjects.RemoveAt(i);
                                    Destroy(grandParent.gameObject);
                                    break;
                                }
                            }


                            LoadUserManagement();
                            InitUserScreen();
                            settingUserScreen.SetActive(false);
                            ScreenManager.Instance.ClosePopUpMessage();
                        }
                        else
                        {
                            ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                            ScreenManager.Instance.txt_PopUpMsg.text = "���� ������ �����߽��ϴ�.";
                            ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                            ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                            {
                                ScreenManager.Instance.ClosePopUpMessage();
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                });
            }
        }

            
    }

    public void SendTestPush()
    {
        var selectedUser = userContent.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault(); // ���� ���õ� ����
        Transform parent = selectedUser.transform.parent; // ù ��° �θ�
        Transform grandParent = parent != null ? parent.parent : null; // �� ��° �θ� (�θ��� �θ�)

        string selectedUserID = string.Empty;
        // �θ��� �θ� ������Ʈ�� null�� �ƴ��� Ȯ���� �� �̸��� ����
        if (grandParent != null)
        {
            selectedUserID = grandParent.gameObject.name.Replace("user_", "");
        }

        string userID = string.Empty;

        DataTable tableUser = ClientDatabase.FetchUserData().Tables[0];
        foreach (DataRow userRow in tableUser.Rows)
        {            
            string userFCMToken = userRow["FLD_FCM_TOKEN"].ToString();

            if (selectedUserID == userRow["FLD_ID"].ToString())
            {
                if (userRow["FLD_FCM_TOKEN"].ToString() != string.Empty)
                {
                    StartCoroutine(FCMNotifier.Instance.GetAccessTokenAndSendMessage(userFCMToken, $"�׽�Ʈ Push", $"�׽�Ʈ Push �˸��Դϴ�."));

                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.Confirm;
                    ScreenManager.Instance.txt_PopUpMsg.text = "�׽�Ʈ Push �˸��� ���۵Ǿ����ϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }
                else
                {
                    ScreenManager.Instance.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
                    ScreenManager.Instance.txt_PopUpMsg.text = "Push �˸��� �ش� ��������\n����� �ۿ��� �α��� �� ���� �����մϴ�.";
                    ScreenManager.Instance.btnPopUpConfirm.onClick.RemoveAllListeners();
                    ScreenManager.Instance.btnPopUpConfirm.onClick.AddListener(() =>
                    {
                        ScreenManager.Instance.ClosePopUpMessage();
                    });
                }                          
            }
        }
    }
}