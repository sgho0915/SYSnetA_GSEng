using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Data;
using DG.Tweening;
using System.Security.Cryptography;

public class AlarmPopUpManager : MonoBehaviour
{
    public Transform alarmPanelScrollViewContent;
    public GameObject alarmPanel;
    public Transform specificAlarmPanelScrollViewContent;
    public GameObject specificAlarmPanel;
    public Transform entireAlarmPanelScrollViewContent;
    public GameObject entireAlarmPanel;
    public GameObject alarmElementPrefab;
    private Button btnClose; // 알람 팝업 닫기 버튼
    private Button btnSpecificClose; // 알람 팝업 닫기 버튼
    private Button btnEntireClose; // 알람 팝업 닫기 버튼    
    private bool isColorChangeAnimationRunning = false; // 애니메이션 실행 상태를 추적하는 플래그
    private Coroutine alarmSoundCoroutine = null; // 현재 실행 중인 코루틴을 추적

    // 이전에 활성화된 알람 데이터의 고유 식별자를 저장
    private HashSet<string> processedAlarmIdentifiers = new HashSet<string>();
    public static Dictionary<string, GameObject> alarmElementInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> specificAlarmElementInstances = new Dictionary<string, GameObject>();
    public static Dictionary<string, GameObject> entireAlarmElementInstances = new Dictionary<string, GameObject>();

    private static AlarmPopUpManager _instance;
    public static AlarmPopUpManager Instance { get { return _instance; } }

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

        btnClose = alarmPanel.transform.Find("AlarmParent/Top/btn_Close").GetComponent<Button>();
        btnClose.onClick.AddListener(() => CloseAlarmPanel());
        btnSpecificClose = specificAlarmPanel.transform.Find("AlarmParent/Top/btn_Close").GetComponent<Button>();
        btnSpecificClose.onClick.AddListener(() => CloseSpecificAlarmPanel());
        btnEntireClose = entireAlarmPanel.transform.Find("AlarmParent/Top/btn_Close").GetComponent<Button>();
        btnEntireClose.onClick.AddListener(() => CloseEntireAlarmPanel());
    }

    public void CloseAlarmPanel()
    {
        // 알람 요소를 제거하기 전에 현재 활성화된 알람의 식별자를 저장
        foreach (Transform child in alarmPanelScrollViewContent)
        {
            processedAlarmIdentifiers.Add(child.gameObject.name);
        }

        // 알람 요소 제거
        foreach (Transform child in alarmPanelScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        alarmElementInstances.Clear();
        alarmPanel.SetActive(false);
        StopChangeBGColor();

        if (alarmSoundCoroutine != null)
        {
            StopCoroutine(alarmSoundCoroutine);
            alarmSoundCoroutine = null;
        }
    }

    public void CloseSpecificAlarmPanel()
    {
        // 알람 요소 제거
        foreach (Transform child in specificAlarmPanelScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        specificAlarmElementInstances.Clear();
        specificAlarmPanel.SetActive(false);
    }
    public void CloseEntireAlarmPanel()
    {
        // 알람 요소 제거
        foreach (Transform child in entireAlarmPanelScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        entireAlarmElementInstances.Clear();
        entireAlarmPanel.SetActive(false);
    }

    public void ShowEntireControllerAlarm()
    {
        DataTable realTimeWarningTable = ClientDatabase.realTimeWarningData.Tables[0];
        HashSet<string> currentAlarms = new HashSet<string>();

        foreach (DataRow row in realTimeWarningTable.Rows)
        {
            string occuredTime = row["WTIME"].ToString();
            string iid = row["ID"].ToString();
            string cid = row["CID"].ToString();
            string addr = row["ADDR"].ToString();
            string mask = row["MASK"].ToString();
            string targetName = $"AlarmElement_{iid}_{cid}_{addr}_{mask}_{occuredTime}";

            currentAlarms.Add(targetName);

            if (!entireAlarmElementInstances.ContainsKey(targetName))
            {
                GameObject alarmElementInstance = Instantiate(alarmElementPrefab, entireAlarmPanelScrollViewContent);
                alarmElementInstance.name = targetName;
                entireAlarmElementInstances[targetName] = alarmElementInstance;
                alarmElementInstance.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>().text = row["CNAME"].ToString();
                alarmElementInstance.transform.Find("txt_AlarmOccurTime").GetComponent<TextMeshProUGUI>().text = occuredTime;
                alarmElementInstance.transform.Find("txt_AlarmDescription").GetComponent<TextMeshProUGUI>().text = row["DESC"].ToString();
            }
        }
        
        // 적어도 하나의 새로운 알람이 있으면 패널을 활성화
        if (entireAlarmPanelScrollViewContent.childCount > 0)
        {
            entireAlarmPanel.SetActive(true);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(entireAlarmPanelScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    public void ShowSpecificControllerAlarm(DataSet realTimeWarningDataSet, string currentIID, string currentCID)
    {
        DataTable realTimeWarningTable = realTimeWarningDataSet.Tables[0];
        HashSet<string> currentAlarms = new HashSet<string>();

        foreach (DataRow row in realTimeWarningTable.Rows)
        {
            string occuredTime = row["WTIME"].ToString();
            string iid = row["ID"].ToString();
            string cid = row["CID"].ToString();
            string addr = row["ADDR"].ToString();
            string mask = row["MASK"].ToString();
            string targetName = $"AlarmElement_{iid}_{cid}_{addr}_{mask}_{occuredTime}";

            currentAlarms.Add(targetName);

            if (!specificAlarmElementInstances.ContainsKey(targetName))
            {
                GameObject alarmElementInstance = Instantiate(alarmElementPrefab, specificAlarmPanelScrollViewContent);
                alarmElementInstance.name = targetName;
                specificAlarmElementInstances[targetName] = alarmElementInstance;
                alarmElementInstance.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>().text = row["CNAME"].ToString();
                alarmElementInstance.transform.Find("txt_AlarmOccurTime").GetComponent<TextMeshProUGUI>().text = occuredTime;
                alarmElementInstance.transform.Find("txt_AlarmDescription").GetComponent<TextMeshProUGUI>().text = row["DESC"].ToString();
            }
        }
        // 디테일뷰 상태에서 알람 버튼 누르면 해당 컨트롤러의 해제 안된 알람 정보 표시
        foreach (var entry in specificAlarmElementInstances)
        {
            string[] alarmArr = entry.Value.name.Split('_');
            string alarmIID = alarmArr[1];
            string alarmCID = alarmArr[2];

            if (alarmIID == currentIID && alarmCID == currentCID)
            {
                entry.Value.SetActive(true);
            }
            else
            {
                entry.Value.SetActive(false);
            }
        }
        // 적어도 하나의 새로운 알람이 있으면 패널을 활성화
        if (specificAlarmPanelScrollViewContent.childCount > 0)
        {
            specificAlarmPanel.SetActive(true);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(specificAlarmPanelScrollViewContent.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
    }

    public void AlarmTracking(DataSet realTimeWarningDataSet)
    {
        DataTable realTimeWarningTable = realTimeWarningDataSet.Tables[0];
        string occuredTime = string.Empty;
        string cname = string.Empty;
        string desc = string.Empty;
        string iid =  string.Empty;
        string cid = string.Empty;
        string targetName = string.Empty;
        if (realTimeWarningTable.Rows.Count > 0)
        {
            foreach (DataRow row in realTimeWarningTable.Rows)
            {
                occuredTime = row["WTIME"].ToString();
                cname = row["CNAME"].ToString();
                desc = row["DESC"].ToString();
                iid = row["ID"].ToString();
                cid = row["CID"].ToString();
                targetName = $"AlarmElement_{iid}_{cid}_{occuredTime}";

                // 이미 처리된 알람이면 추가 처리를 건너뜀
                if (processedAlarmIdentifiers.Contains(targetName))
                {
                    continue;
                }

                if (!alarmElementInstances.ContainsKey(targetName))
                {
                    GameObject alarmElementInstance = Instantiate(alarmElementPrefab, alarmPanelScrollViewContent);
                    alarmElementInstance.name = targetName;
                    alarmElementInstances[targetName] = alarmElementInstance;
                    alarmElementInstance.transform.Find("txt_ControllerName").GetComponent<TextMeshProUGUI>().text = cname;
                    alarmElementInstance.transform.Find("txt_AlarmOccurTime").GetComponent<TextMeshProUGUI>().text = occuredTime;
                    alarmElementInstance.transform.Find("txt_AlarmDescription").GetComponent<TextMeshProUGUI>().text = desc;
                }
            }

            // 적어도 하나의 새로운 알람이 있으면 패널을 활성화
            if (alarmPanelScrollViewContent.childCount > 0)
            {
                alarmPanel.SetActive(true);
                StartChangeBGColor();
                
                if (alarmSoundCoroutine == null)
                {
                    if(SettingManager.alarmSound == 4)
                    {
                        alarmSoundCoroutine = StartCoroutine(SoundManager.Instance.PlayTTSAlarmSound(realTimeWarningTable));
                    }
                    else
                    {
                        alarmSoundCoroutine = StartCoroutine(SoundManager.Instance.PlayAlarmSound());
                    }
                }                    
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(alarmPanelScrollViewContent.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
        }        
    }

    void StartChangeBGColor()
    {
        if (isColorChangeAnimationRunning) return; // 애니메이션이 이미 실행 중이라면, 함수 실행을 중지

        Image image = alarmPanel.GetComponent<Image>();
        if (image != null)
        {
            Color startColor = new Color(0f / 255f, 0f / 255f, 0f / 255f, 165f / 255f);
            Color endColor = new Color(240f / 255f, 94f / 255f, 51f / 255f, 165f / 255f);
            image.color = startColor;
            var tween = image.DOColor(endColor, 1f)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetEase(Ease.Linear)
                             .OnStart(() => isColorChangeAnimationRunning = true)
                             .OnComplete(() => isColorChangeAnimationRunning = false);
        }
    }

    void StopChangeBGColor()
    {
        Image image = alarmPanel.GetComponent<Image>();
        if (image != null)
        {
            image.DOKill(true); // 애니메이션 중지 및 제거
            image.color = new Color(0f / 255f, 0f / 255f, 0f / 255f, 165f / 255f);
            isColorChangeAnimationRunning = false; // 애니메이션 상태를 중지로 설정
        }
    }
}
