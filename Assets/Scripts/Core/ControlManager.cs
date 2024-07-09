
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlManager : MonoBehaviour
{
    ControllerStatus status;

    private static ControlManager _instance;
    public static ControlManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;            
        }
    }

    public bool SendControlCommand(string iid, string cid, string jsonString)
    {
        string insertQuery = $"INSERT INTO TBL_REALTIME_SETUP (ID, CID, JSON) VALUES ('{iid}', '{cid}', '{jsonString}')";

        bool insertSuccessful = ClientDatabase.OnInsertRequest(insertQuery); // insert 수행 후 true or false 리턴

        if (insertSuccessful)
        {
            //Debug.Log($"ControlManager : IID-{iid}, CID-{cid}에 대한 설정값 변경 완료");
        }
        else
        {
            //Debug.Log($"ControlManager : IID-{iid}, CID-{cid}에 대한 설정값 변경 실패");
        }

        return insertSuccessful;
    }
}
