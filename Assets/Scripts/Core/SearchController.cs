using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;

public class SearchController : MonoBehaviour
{
    public TMP_InputField searchInputField;

    private void Start()
    {
        searchInputField.onEndEdit.AddListener(delegate { OnEndEditInputField(); });
    }

    // 인풋 필드 편집 완료 시 호출
    private void OnEndEditInputField()
    {
        OnSearchButtonClicked(); // 검색
    }


    // 검색 버튼 이벤트 핸들러
    public void OnSearchButtonClicked()
    {
        string searchQuery = searchInputField.text;

        // 컨트롤러 그리드뷰
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            // 검색어가 비어 있으면 모든 컨트롤러 활성화
            foreach (var controller in ClientDatabase.controllerGridInstances.Values)
            {
                controller.SetActive(true);
            }
        }
        else
        {
            // 검색어가 있으면 데이터베이스에서 검색
            string sqlQuery = $"SELECT ID, CID FROM TBL_CONTROLLER WHERE CNAME LIKE '%{searchQuery}%'";
            var searchResults = ClientDatabase.OnSelectRequest(sqlQuery, "TBL_CONTROLLER");

            // 모든 컨트롤러 비활성화
            foreach (var controller in ClientDatabase.controllerGridInstances.Values)
            {
                controller.SetActive(false);
            }

            // 검색 결과에 따라 해당 컨트롤러만 활성화
            foreach (DataRow row in searchResults.Tables[0].Rows)
            {
                string id = row["ID"].ToString();
                string cid = row["CID"].ToString();
                string controllerKey = $"Controller_{id}_{cid}";

                if (ClientDatabase.controllerGridInstances.TryGetValue(controllerKey, out var controllerObject))
                {
                    controllerObject.SetActive(true);
                }
            }
        }

        // 컨트롤러 리스트뷰
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            // 검색어가 비어 있으면 모든 컨트롤러 활성화
            foreach (var controller in ClientDatabase.controllerListInstances.Values)
            {
                controller.SetActive(true);
            }
        }
        else
        {
            // 검색어가 있으면 데이터베이스에서 검색
            string sqlQuery = $"SELECT ID, CID FROM TBL_CONTROLLER WHERE CNAME LIKE '%{searchQuery}%'";
            var searchResults = ClientDatabase.OnSelectRequest(sqlQuery, "TBL_CONTROLLER");

            // 모든 컨트롤러 비활성화
            foreach (var controller in ClientDatabase.controllerListInstances.Values)
            {
                controller.SetActive(false);
            }

            // 검색 결과에 따라 해당 컨트롤러만 활성화
            foreach (DataRow row in searchResults.Tables[0].Rows)
            {
                string id = row["ID"].ToString();
                string cid = row["CID"].ToString();
                string controllerKey = $"Controller_{id}_{cid}";

                if (ClientDatabase.controllerListInstances.TryGetValue(controllerKey, out var controllerObject))
                {
                    controllerObject.SetActive(true);
                }
            }
        }
    }

}
