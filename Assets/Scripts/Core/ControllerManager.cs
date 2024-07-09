using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System;
using MySqlConnector;
using System.Text;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class ControllerManager : MonoBehaviour
{
    ScreenManager screenManager; // ScreenManager 인스턴스 할당을 위한 변수

    [Header("기타")]
    public TextMeshProUGUI txt_Title;                   // 상단 타이틀    

    [Header("컨트롤러 관련 오브젝트")]        
    public TMP_InputField inputField_ControllerName;    // 컨트롤러 인풋필드 오브젝트
    public TMP_Dropdown dropdown_Protocol;              // dropdown_Protocol에 프로토콜 목록을 보이기 위한 드롭다운 오브젝트
    public Toggle toggle_SMS;                           // SMS 토글 오브젝트
    public Toggle toggle_Share;                         // Share 토글 오브젝트    
    public TMP_InputField inputField_Remark;            // 비고 인풋필드 오브젝트

    private string converterObjName = string.Empty;

    private void Awake()
    {
        screenManager = ScreenManager.Instance; // ScreenManager 싱글톤 인스턴스 적용
    }

    void Start()
    {
        
    }
       
    #region 컨트롤러 DB에 직접 추가
    public void OpenControllerAddSetting()
    {
        this.gameObject.SetActive(true);
        txt_Title.text = "컨트롤러 추가";
        LoadProtocolList();
    }

    /// <summary>
    /// "btn_ControllerAdd" 버튼을 클릭했을 때 호출되는 메서드
    /// </summary>
    public void OnControllerAddButtonClick(Transform buttonTransform)
    {
        if (buttonTransform == null)
        {
            //Debug.LogError("buttonTransform is null");
            return;
        }
        else
        {
            converterObjName = buttonTransform.name;
        }

        OpenControllerAddSetting();

    }

    public void AddController()
    {
        // 필드 값 준비
        string fld_InterfaceId = converterObjName.Substring("obj_Converter_".Length);
        string fld_Type = GetConverterType(fld_InterfaceId);
        int fld_ControllerId = GetNextControllerId(fld_InterfaceId);
        string[] protocolData = GetProtocolData();
        string[] controllerAnoterData = GetControllerAnotherData();

        if (protocolData == null)
        {
            //Debug.LogError("Protocol data not found.");
            return;
        }

        // TBL_CONTROLLER에 데이터 삽입
        InsertControllerData(fld_InterfaceId, fld_ControllerId, protocolData, controllerAnoterData);
    }

    /// <summary>
    /// fld_InterfaceId가 TBL_INTERFACE 테이블의 FLD_ID에 존재하는지 확인
    /// </summary>
    /// <param name="fld_InterfaceId"></param>
    /// <returns></returns>
    private string GetConverterType(string fld_InterfaceId)
    {
        string converterType = string.Empty;        
                
        DataSet ds = ClientDatabase.FetchInterfaceData(fld_InterfaceId);

        if (ds != null)
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {               
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    converterType = row["FLD_TYPE"].ToString(); // 컨버터 타입
                    return converterType;
                }
            }
        }
        else
        {
            //Debug.LogError("TBL_SERIAL_LIST DataSet is null");
        }
        return null;
    }

    /// <summary>
    /// 다음 FLD_CONTROLLER_ID 값을 찾습니다.
    /// </summary>
    /// <param name="fldInterfaceId">FLD_INTERFACE_ID 값</param>
    /// <returns>다음 FLD_CONTROLLER_ID 값</returns>
    private int GetNextControllerId(string fldInterfaceId)
    {
        string query = $"SELECT MAX(FLD_CONTROLLER_ID) AS MaxControllerId FROM TBL_CONTROLLER WHERE FLD_INTERFACE_ID = '{fldInterfaceId}'";

        DataSet ds = ClientDatabase.OnSelectRequest(query, "TBL_CONTROLLER");

        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            DataRow row = ds.Tables[0].Rows[0];
            if (row["MaxControllerId"] != DBNull.Value)
            {
                int maxControllerId = Convert.ToInt32(row["MaxControllerId"]);
                return maxControllerId + 1;
            }
        }

        // FLD_INTERFACE_ID 필드 값이 fldInterfaceId인 행이 존재하지 않으므로
        // FLD_CONTROLLER_ID 값으로 1을 반환합니다.
        return 1;
    }

    #region 드롭다운 항목 관리
    /// <summary>
    /// TBL_PROTOCOL_API 테이블의 모든 행 중 프로토콜명과 버전을 읽어 Dropdown_Protocol 오브젝트의 목록에 추가함
    /// </summary>
    public void LoadProtocolList()
    {
        DataRow[] foundRows = ClientDatabase.SelectRowsFromTable(ClientDatabase.FetchProtocolList(), "TBL_PROTOCOL_API", "");
        if (foundRows.Length > 0)
        {
            dropdown_Protocol.options.Clear();  // 기존의 Dropdown 목록을 초기화합니다.
            string protocolName = foundRows[0]["NAME"].ToString(); // 프로토콜명
            string protocolVer = foundRows[0]["VER"].ToString(); // 프로토콜 버전
            dropdown_Protocol.options.Add(new TMP_Dropdown.OptionData($"[{protocolVer}] {protocolName}"));  // Dropdown에 프로토콜 버전과 이름을 조합한 항목을 추가          
        }
        else
        {
            //Debug.LogError("TBL_PROTOCOL_API DataSet is null");
            return;
        }
    }
    #endregion



    /// <summary>
    /// 선택된 프로토콜의 데이터를 가져옵니다.
    /// </summary>
    /// <returns>프로토콜 데이터</returns>
    private string[] GetProtocolData()
    {
        string selectedProtocol = dropdown_Protocol.options[dropdown_Protocol.value].text;
        Regex regex = new Regex(@"\[(.*?)\] (.*)");
        Match match = regex.Match(selectedProtocol);
        if (!match.Success)
        {
            //Debug.LogError("Failed to parse the selected protocol");
            return null;
        }

        string protocolVer = match.Groups[1].Value;
        string protocolName = match.Groups[2].Value;

        // KEY, START, END라는 단어는 MySQL/MariaDB에서 예약어로 사용되고 있어 이를 필드 이름으로 사용할 때는 백틱(``)으로 감싸야 함
        DataRow[] foundRows = ClientDatabase.SelectRowsFromTable(ClientDatabase.FetchProtocolList(), "TBL_PROTOCOL_API", $"VER = '{protocolVer}' AND NAME = '{protocolName}'");
        if (foundRows.Length > 0)
        {            
            string key = foundRows[0]["KEY"].ToString();
            string start = foundRows[0]["START"].ToString();
            string end = foundRows[0]["END"].ToString();
            return new string[] { key, start, end };
        }
        else
        {
            //Debug.LogError("Failed to find the protocol data in the database");
            return null;
        }
    }

    private string[] GetControllerAnotherData()
    {
        string converterName = inputField_ControllerName.text; // 컨트롤러명 값을 변수에 할당함
        string remark = string.IsNullOrWhiteSpace(inputField_Remark.text) ? "comment" : inputField_Remark.text; // 비고 값을 변수에
        int smsSend = toggle_SMS.isOn ? 1 : 0; // SMS 전송여부 값을 변수에 할당함
        int share = toggle_Share.isOn ? 1 : 0; // 공유 값을 변수에 할당함

        return new string[] { converterName, remark, smsSend.ToString(), share.ToString() };
    }

    /// <summary>
    /// TBL_CONTROLLER 테이블에 데이터를 삽입합니다.
    /// </summary>
    /// <param name="fldInterfaceId">FLD_INTERFACE_ID 값</param>
    /// <param name="fldControllerId">FLD_CONTROLLER_ID 값</param>
    /// <param name="protocolData">프로토콜 데이터</param>
    /// <param name="controllerAnotherData">컨트롤러 기타 데이터</param>
    private void InsertControllerData(string fldInterfaceId, int fldControllerId, string[] protocolData, string[] controllerAnotherData)
    {
        // 데이터 배열에서 필요한 값을 추출합니다.
        string protocolKey = protocolData[0];
        string protocolStart = protocolData[1];
        string protocolEnd = protocolData[2];
        string controllerName = controllerAnotherData[0];
        string controllerRemark = controllerAnotherData[1];
        string controllerSMSSend = controllerAnotherData[2];
        string controllerShare = controllerAnotherData[3];

        // SQL 쿼리 문자열을 작성합니다.
        string insertQuery = $@"
        INSERT INTO TBL_CONTROLLER 
        (
            FLD_TYPE, 
            FLD_INTERFACE_ID, 
            FLD_CONTROLLER_ID, 
            FLD_PROTOCOL_KEY, 
            FLD_ADDR_ST, 
            FLD_ADDR_ED, 
            FLD_NAME, 
            FLD_REMARK, 
            FLD_SMS_SEND, 
            FLD_SHARE
        ) 
        VALUES 
        (
            (SELECT FLD_TYPE FROM TBL_INTERFACE WHERE FLD_ID = '{fldInterfaceId}'), 
            '{fldInterfaceId}', 
            {fldControllerId}, 
            '{protocolKey}', 
            '{protocolStart}', 
            '{protocolEnd}', 
            '{controllerName}', 
            '{controllerRemark}', 
            {controllerSMSSend}, 
            {controllerShare}
        );
    ";

        // 쿼리를 실행하여 데이터를 삽입합니다.
        bool isSuccess = ClientDatabase.OnInsertRequest(insertQuery);
        if (isSuccess)
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.Notification;
            screenManager.txt_PopUpMsg.text = "컨트롤러가 추가되었습니다.";
            //Debug.Log($"TBL_CONTROLLER 테이블에 컨트롤러({controllerName})가 성공적으로 행이 삽입되었습니다.");
        }
        else
        {
            screenManager.CurrentPopUpState = ScreenManager.PopUpState.ErrorWarning;
            screenManager.txt_PopUpMsg.text = "컨트롤러 추가에 실패했습니다.";
            //Debug.LogError($"TBL_CONTROLLER 테이블에 컨트롤러({controllerName}) 추가 실패");
        }
    }
    #endregion


    /// <summary>
    /// 컨트롤러 추가 화면 닫기
    /// </summary>
    public void CloseConverterAddScreen()
    {        
        screenManager.CurrentScreenState = ScreenManager.ScreenState.Main; // ScreenState를 다시 main으로 전환
        inputField_ControllerName.text = string.Empty;
        inputField_Remark.text = string.Empty;
        toggle_SMS.isOn = false;
        toggle_Share.isOn = false;
        this.gameObject.SetActive(false);
    }
}
