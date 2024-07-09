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

    // ��ǲ �ʵ� ���� �Ϸ� �� ȣ��
    private void OnEndEditInputField()
    {
        OnSearchButtonClicked(); // �˻�
    }


    // �˻� ��ư �̺�Ʈ �ڵ鷯
    public void OnSearchButtonClicked()
    {
        string searchQuery = searchInputField.text;

        // ��Ʈ�ѷ� �׸����
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            // �˻�� ��� ������ ��� ��Ʈ�ѷ� Ȱ��ȭ
            foreach (var controller in ClientDatabase.controllerGridInstances.Values)
            {
                controller.SetActive(true);
            }
        }
        else
        {
            // �˻�� ������ �����ͺ��̽����� �˻�
            string sqlQuery = $"SELECT ID, CID FROM TBL_CONTROLLER WHERE CNAME LIKE '%{searchQuery}%'";
            var searchResults = ClientDatabase.OnSelectRequest(sqlQuery, "TBL_CONTROLLER");

            // ��� ��Ʈ�ѷ� ��Ȱ��ȭ
            foreach (var controller in ClientDatabase.controllerGridInstances.Values)
            {
                controller.SetActive(false);
            }

            // �˻� ����� ���� �ش� ��Ʈ�ѷ��� Ȱ��ȭ
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

        // ��Ʈ�ѷ� ����Ʈ��
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            // �˻�� ��� ������ ��� ��Ʈ�ѷ� Ȱ��ȭ
            foreach (var controller in ClientDatabase.controllerListInstances.Values)
            {
                controller.SetActive(true);
            }
        }
        else
        {
            // �˻�� ������ �����ͺ��̽����� �˻�
            string sqlQuery = $"SELECT ID, CID FROM TBL_CONTROLLER WHERE CNAME LIKE '%{searchQuery}%'";
            var searchResults = ClientDatabase.OnSelectRequest(sqlQuery, "TBL_CONTROLLER");

            // ��� ��Ʈ�ѷ� ��Ȱ��ȭ
            foreach (var controller in ClientDatabase.controllerListInstances.Values)
            {
                controller.SetActive(false);
            }

            // �˻� ����� ���� �ش� ��Ʈ�ѷ��� Ȱ��ȭ
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
