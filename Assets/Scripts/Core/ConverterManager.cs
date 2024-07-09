using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using MySqlConnector;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class ConverterManager : MonoBehaviour
{
    ScreenManager screenManager; // ScreenManager 인스턴스 할당을 위한 변수

    [Header("기타")]
    public TextMeshProUGUI txt_Title;                   // 상단 타이틀
    public GameObject obj_converterAddListScrollView;   // 통신 프로토콜 리스트

    [Header("SERIAL 관련 오브젝트")]
    // Serial
    public GameObject obj_settingSerial;                // serial 설정 화면
    public TMP_Dropdown dropdown_COMPort;               // dropdown_COMPort에 컨버터 목록을 보이기 위한 드롭다운 오브젝트
    public TMP_Dropdown dropdown_Baudrate;              // Baudrate 드롭다운 오브젝트
    public TMP_Dropdown dropdown_ByteSize;              // ByteSize 드롭다운 오브젝트
    public TMP_Dropdown dropdown_StopBits;              // StopBits 드롭다운 오브젝트
    public TMP_Dropdown dropdown_Parity;                // Parity 드롭다운 오브젝트
    public Toggle toggle_RTS;                           // RTS 토글 오브젝트
    public Toggle toggle_DTR;                           // DTR 토글 오브젝트
    public TMP_InputField inputField_ConverterName;     // 컨버터명 인풋필드 오브젝트
    public TMP_InputField inputField_Remark;            // 비고 인풋필드 오브젝트

    private void Awake()
    {
        screenManager = ScreenManager.Instance; // ScreenManager 싱글톤 인스턴스 적용
    }

    void Start()
    {
        
    }   

    #region Serial 컨버터 DB에 직접 추가
    public void OpenSerialSetting()
    {
        txt_Title.text = "Serial 설정";
        obj_converterAddListScrollView.SetActive(false);
        obj_settingSerial.SetActive(true);
        LoadSerialList();
    }

    /// <summary>
    /// DB에 Serial 컨버터 추가
    /// </summary>
    public void AddSerialConverter()
    {
        // 컨버터 이름이 null인지 체크, null일 경우 에러 리턴, 에러 팝업 출력
        if (string.IsNullOrWhiteSpace(inputField_ConverterName.text)) 
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "컨버터명을 입력해주세요.";
            //Debug.LogError("컨버터명은 필수 입력사항입니다.");
            return; // 컨버터명 값이 존재하지 않으므로 여기서 함수 종료
        }

        // comport, baudrate 등 각각의 항목의 값을 변수에 할당함
        string fld_id_comport = dropdown_COMPort.options[dropdown_COMPort.value].text; // comport 값을 변수에 할당함
        string comport = $"/dev/{dropdown_COMPort.options[dropdown_COMPort.value].text}"; // comport의 전체 경로 값을 변수에 할당함
        int baudrate = int.Parse(dropdown_Baudrate.options[dropdown_Baudrate.value].text); // baudrate 값을 변수에 할당함
        int stopbits = int.Parse(dropdown_StopBits.options[dropdown_StopBits.value].text); // stopbits 값을 변수에 할당함
        int bytesize = int.Parse(dropdown_ByteSize.options[dropdown_ByteSize.value].text); // bytesize 값을 변수에 할당함
        string parity = ""; // 아래의 switch 문에서 parity 값의 첫 문자를 따서 변수에 할당함
        switch (dropdown_Parity.options[dropdown_Parity.value].text)
        {
            case "None":
                parity = "N";
                break;
            case "Even":
                parity = "E";
                break;
            case "Odd":
                parity = "O";
                break;
        }
        int rts = toggle_RTS.isOn ? 1 : 0; // rts 값을 변수에 할당함
        int dtr = toggle_DTR.isOn ? 1 : 0; // dtr 값을 변수에 할당함

        // fld_config에 insert할 값을 변수에 할당함
        string fld_config = $"{{\"comport\":\"{comport}\",\"baudrate\":{baudrate},\"stopbits\":{stopbits},\"bytesize\":{bytesize},\"parity\":\"{parity}\",\"RTS\":{rts},\"DTR\":{dtr}}}";

        string converterName = inputField_ConverterName.text; // 컨버터명 값을 변수에 할당함
        string remark = string.IsNullOrWhiteSpace(inputField_Remark.text) ? "comment" : inputField_Remark.text; // 비고 값을 변수에 할당함

        // 추가하려는 SerialPort가 실제로 존재하는지 검증(TBL_SERIAL_LIST 테이블에서 해당 FLD_ID의 존재 여부 확인)
        string checkExistenceQuery = $"SELECT FLD_NAME FROM TBL_SERIAL_LIST WHERE FLD_NAME = '{fld_id_comport}'"; // 컨버터 id(tty...)의 존재 무결성 체크를 위한 쿼리
        bool doesExist = ClientDatabase.DoesExistInTableCheck(checkExistenceQuery); // 체크 수행 후 true or false 리턴
        if (!doesExist) // 에러 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "Serial 컨버터 연결상태를 확인해주세요.";
            //Debug.LogError($"TBL_SERIAL_LIST 테이블에 {fld_id_comport}가 존재하지 않습니다.");
            return; // 존재하지 않으므로 여기서 함수 종료
        }

        // 추가하려는 SerialPort의 FLD_ID 값이 TBL_INTERFACE 테이블에 이미 존재하는지 검증
        string checkExistenceInInterfaceQuery = $"SELECT FLD_ID FROM TBL_INTERFACE WHERE FLD_ID = '{fld_id_comport}'";
        bool doesExistInInterface = ClientDatabase.DoesExistInTableCheck(checkExistenceInInterfaceQuery);
        if (doesExistInInterface) // 에러 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "이미 추가된 Serial Port입니다.";
            //Debug.LogError($"TBL_INTERFACE 테이블에 {fld_id_comport}가 이미 존재합니다.");
            return; // 이미 존재하므로 여기서 함수 종료
        }

        // TBL_SERIAL_LIST 테이블에서 해당 FLD_ID의 값이 존재할 경우 TBL_INTERFACE에 insert할 쿼리
        string insertQuery = $"INSERT INTO TBL_INTERFACE (FLD_TYPE, FLD_ID, FLD_NAME, FLD_CONFIG, FLD_REMARK) VALUES ('SERIAL', '{fld_id_comport}', '{converterName}', '{fld_config}', '{remark}')";

        bool insertSuccessful = ClientDatabase.OnInsertRequest(insertQuery); // insert 수행 후 true or false 리턴

        if (insertSuccessful) // insert 성공 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Notification;
            screenManager.txt_PopUpMsg.text = "Serial 컨버터가 추가되었습니다.";
            //Debug.Log($"TBL_INTERFACE 테이블에 Serial 타입의 컨버터({converterName})가 성공적으로 행이 삽입되었습니다.");
        }
        else // insert 실패 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "Serial 컨버터 추가에 실패했습니다.";
            //Debug.LogError($"TBL_INTERFACE 테이블에 Serial 타입의 컨버터({converterName}) 추가 실패");
        }
    }

    #region 드롭다운 항목 관리
    /// <summary>
    /// TBL_SERIAL_LIST 테이블의 모든 행을 읽어 Dropdown_COMPort 오브젝트의 목록에 추가함
    /// </summary>
    public void LoadSerialList()
    {
        string loadSerialListQeurry = "SELECT FLD_NAME FROM TBL_SERIAL_LIST"; // TBL_SERIAL_LIST 테이블의 모든 행을 읽어오기 위한 쿼리

        // static 메서드 OnSelectRequest 호출
        DataSet ds = ClientDatabase.OnSelectRequest(loadSerialListQeurry, "TBL_SERIAL_LIST");

        if (ds != null)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dropdown_COMPort.options.Clear();  // 기존의 Dropdown 목록을 초기화합니다.

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string serial = row["FLD_NAME"].ToString(); // Port Name
                    dropdown_COMPort.options.Add(new TMP_Dropdown.OptionData(serial));  // Dropdown에 Port Name을 추가                    
                }
            }

            AddBaudrateOptions();
            AddByteSizeOptions();
            AddStopBitsOptions();
            AddParityOptions();
        }
        else
        {
            //Debug.LogError("TBL_SERIAL_LIST DataSet is null");
        }
    }

    // Baudrate 드롭다운 오브젝트에 항목 추가
    private void AddBaudrateOptions()
    {
        List<TMP_Dropdown.OptionData> baudrateOptions = new List<TMP_Dropdown.OptionData>
    {
        new TMP_Dropdown.OptionData("9600"),
        new TMP_Dropdown.OptionData("19200"),
        new TMP_Dropdown.OptionData("38400"),
        new TMP_Dropdown.OptionData("57600"),
        new TMP_Dropdown.OptionData("115200")
    };
        dropdown_Baudrate.options.Clear();
        dropdown_Baudrate.options.AddRange(baudrateOptions);
        dropdown_Baudrate.RefreshShownValue();
    }

    // ByteSize 드롭다운 오브젝트에 항목 추가
    private void AddByteSizeOptions()
    {
        List<TMP_Dropdown.OptionData> byteSizeOptions = new List<TMP_Dropdown.OptionData>
    {
        new TMP_Dropdown.OptionData("8"),
        new TMP_Dropdown.OptionData("7")
    };
        dropdown_ByteSize.options.Clear();
        dropdown_ByteSize.options.AddRange(byteSizeOptions);
        dropdown_ByteSize.RefreshShownValue();
    }

    // StopBits 드롭다운 오브젝트에 항목 추가
    private void AddStopBitsOptions()
    {
        List<TMP_Dropdown.OptionData> stopBitsOptions = new List<TMP_Dropdown.OptionData>
    {
        new TMP_Dropdown.OptionData("1"),
        new TMP_Dropdown.OptionData("2")
    };
        dropdown_StopBits.options.Clear();
        dropdown_StopBits.options.AddRange(stopBitsOptions);
        dropdown_StopBits.RefreshShownValue();
    }

    // Parity 드롭다운 오브젝트에 항목 추가
    private void AddParityOptions()
    {
        List<TMP_Dropdown.OptionData> parityOptions = new List<TMP_Dropdown.OptionData>
    {
        new TMP_Dropdown.OptionData("None"),
        new TMP_Dropdown.OptionData("Even"),
        new TMP_Dropdown.OptionData("Odd")
    };
        dropdown_Parity.options.Clear();
        dropdown_Parity.options.AddRange(parityOptions);
        dropdown_Parity.RefreshShownValue();
    }
    #endregion
    #endregion

    #region Serial 컨버터 수정
    /// <summary>
    /// DB에 Serial 컨버터 내용 수정
    /// </summary>
    public void ModifySerialConverter()
    {
        // 컨버터 이름이 null인지 체크, null일 경우 에러 리턴, 에러 팝업 출력
        if (string.IsNullOrWhiteSpace(inputField_ConverterName.text))
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "컨버터명을 입력해주세요.";
            //Debug.LogError("컨버터명은 필수 입력사항입니다.");
            return; // 컨버터명 값이 존재하지 않으므로 여기서 함수 종료
        }

        // comport, baudrate 등 각각의 항목의 값을 변수에 할당함
        string fld_id_comport = dropdown_COMPort.options[dropdown_COMPort.value].text; // comport 값을 변수에 할당함
        string comport = $"/dev/{dropdown_COMPort.options[dropdown_COMPort.value].text}"; // comport의 전체 경로 값을 변수에 할당함
        int baudrate = int.Parse(dropdown_Baudrate.options[dropdown_Baudrate.value].text); // baudrate 값을 변수에 할당함
        int stopbits = int.Parse(dropdown_StopBits.options[dropdown_StopBits.value].text); // stopbits 값을 변수에 할당함
        int bytesize = int.Parse(dropdown_ByteSize.options[dropdown_ByteSize.value].text); // bytesize 값을 변수에 할당함
        string parity = ""; // 아래의 switch 문에서 parity 값의 첫 문자를 따서 변수에 할당함
        switch (dropdown_Parity.options[dropdown_Parity.value].text)
        {
            case "None":
                parity = "N";
                break;
            case "Even":
                parity = "E";
                break;
            case "Odd":
                parity = "O";
                break;
        }
        int rts = toggle_RTS.isOn ? 1 : 0; // rts 값을 변수에 할당함
        int dtr = toggle_DTR.isOn ? 1 : 0; // dtr 값을 변수에 할당함

        // fld_config에 insert할 값을 변수에 할당함
        string fld_config = $"{{\"comport\":\"{comport}\",\"baudrate\":{baudrate},\"stopbits\":{stopbits},\"bytesize\":{bytesize},\"parity\":\"{parity}\",\"RTS\":{rts},\"DTR\":{dtr}}}";

        string converterName = inputField_ConverterName.text; // 컨버터명 값을 변수에 할당함
        string remark = string.IsNullOrWhiteSpace(inputField_Remark.text) ? "comment" : inputField_Remark.text; // 비고 값을 변수에 할당함

        // 추가하려는 SerialPort가 실제로 존재하는지 검증(TBL_SERIAL_LIST 테이블에서 해당 FLD_ID의 존재 여부 확인)
        string checkExistenceQuery = $"SELECT FLD_NAME FROM TBL_SERIAL_LIST WHERE FLD_NAME = '{fld_id_comport}'"; // 컨버터 id(tty...)의 존재 무결성 체크를 위한 쿼리
        bool doesExist = ClientDatabase.DoesExistInTableCheck(checkExistenceQuery); // 체크 수행 후 true or false 리턴
        if (!doesExist) // 에러 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "Serial 컨버터 연결상태를 확인해주세요.";
            //Debug.LogError($"TBL_SERIAL_LIST 테이블에 {fld_id_comport}가 존재하지 않습니다.");
            return; // 존재하지 않으므로 여기서 함수 종료
        }

        // 추가하려는 SerialPort의 FLD_ID 값이 TBL_INTERFACE 테이블에 이미 존재하는지 검증
        string checkExistenceInInterfaceQuery = $"SELECT FLD_ID FROM TBL_INTERFACE WHERE FLD_ID = '{fld_id_comport}'";
        bool doesExistInInterface = ClientDatabase.DoesExistInTableCheck(checkExistenceInInterfaceQuery);
        if (doesExistInInterface) // 에러 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "이미 추가된 Serial Port입니다.";
            //Debug.LogError($"TBL_INTERFACE 테이블에 {fld_id_comport}가 이미 존재합니다.");
            return; // 이미 존재하므로 여기서 함수 종료
        }

        // TBL_SERIAL_LIST 테이블에서 해당 FLD_ID의 값이 존재할 경우 TBL_INTERFACE에 insert할 쿼리
        string insertQuery = $"INSERT INTO TBL_INTERFACE (FLD_TYPE, FLD_ID, FLD_NAME, FLD_CONFIG, FLD_REMARK) VALUES ('SERIAL', '{fld_id_comport}', '{converterName}', '{fld_config}', '{remark}')";

        bool insertSuccessful = ClientDatabase.OnInsertRequest(insertQuery); // insert 수행 후 true or false 리턴

        if (insertSuccessful) // insert 성공 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Notification;
            screenManager.txt_PopUpMsg.text = "Serial 컨버터가 추가되었습니다.";
            //Debug.Log($"TBL_INTERFACE 테이블에 Serial 타입의 컨버터({converterName})가 성공적으로 행이 삽입되었습니다.");
        }
        else // insert 실패 팝업 출력
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "Serial 컨버터 추가에 실패했습니다.";
            //Debug.LogError($"TBL_INTERFACE 테이블에 Serial 타입의 컨버터({converterName}) 추가 실패");
        }
    }
    #endregion

    /// <summary>
    /// DB 쿼리 돌려서 나온 DataSet 내용 로그 남기는 함수
    /// </summary>
    /// <param name="ds"></param>
    public void LogDataSet(DataSet ds)
    {
        if (ds == null)
        {
            //Debug.LogError("DataSet is null.");
            return;
        }

        foreach (DataTable table in ds.Tables)
        {
            //Debug.Log($"Table Name: {table.TableName}");
            foreach (DataRow row in table.Rows)
            {
                string rowLog = "Row: ";
                foreach (DataColumn column in table.Columns)
                {
                    rowLog += $"{column.ColumnName}: {row[column]} | ";
                }
                //Debug.Log(rowLog);
            }
        }
    }

    /// <summary>
    /// 컨버터 추가 화면 닫기
    /// </summary>
    public void CloseConverterAddScreen()
    {
        BackToConProtocolList();
        screenManager.CurrentScreenState = ScreenManager.ScreenState.Main; // ScreenState를 다시 main으로 전환
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 컨버터 추가 화면 메인으로 돌아가기
    /// </summary>
    public void BackToConProtocolList()
    {
        txt_Title.text = "컨버터 추가";
        obj_converterAddListScrollView.SetActive(true);
        obj_settingSerial.SetActive(false);
    }
}
