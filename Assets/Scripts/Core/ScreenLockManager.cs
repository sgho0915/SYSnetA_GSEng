using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static Unity.Collections.AllocatorManager;

public class ScreenLockManager : MonoBehaviour
{
    public static ScreenLockManager Instance { get; private set; }

    public GameObject lockScreen;
    public Button btnLock;
    public Button btnUnlock;
    public GameObject settingPasswd;
    public CanvasGroup unlockCanvasGroup;

    public bool isSetPWMode = false;
    public bool isUnlockMode = false;
    public bool isModifyMode = false;

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
    }
    private void Start()
    {
        InitLock();
    }

    public void InitLock()
    {
        btnLock.onClick.RemoveAllListeners();
        btnLock.onClick.AddListener(() =>
        {
            LockScreen();
        });
        if (ConfigManager.Instance.GetSetting("LOCK_USE") == "true")
        {
            if (ConfigManager.Instance.GetSetting("LOCK_STATE") == "true")
            {
                LockScreen();
            }
            else
            {
                btnLock.gameObject.SetActive(true);
                btnLock.transform.Find("Lock").gameObject.SetActive(false);
                btnLock.transform.Find("Unlock").gameObject.SetActive(true);                
            }
        }
        else
        {
            btnLock.gameObject.SetActive(false);
            btnLock.transform.Find("Lock").gameObject.SetActive(false);
            btnLock.transform.Find("Unlock").gameObject.SetActive(true);
        }
    }

    public void LockScreen()
    {
        ConfigManager.Instance.SetSetting("LOCK_STATE", "true");
        lockScreen.SetActive(true);
        btnLock.gameObject.SetActive(true);
        btnLock.transform.Find("Lock").gameObject.SetActive(true);
        btnLock.transform.Find("Unlock").gameObject.SetActive(false);

        Button btnLockScreen = lockScreen.GetComponent<Button>();
        btnLockScreen.onClick.RemoveAllListeners();
        btnLockScreen.onClick.AddListener(() =>
        {
            unlockCanvasGroup.DOFade(1, 0.5f) // 0.5초 동안 불투명하게
            .OnComplete(() =>
            {
                StartCoroutine(FadeOutUnlockButton());
            });
        });

        btnUnlock.onClick.RemoveAllListeners();
        btnUnlock.onClick.AddListener(() => 
        {
            isUnlockMode = true;
            isSetPWMode = false;            
            settingPasswd.SetActive(true);
            SetScreenPW.Instance.InitPassword();
        });

        // btnUnlock을 투명에서 불투명으로 변경 후 다시 투명하게 만듭니다.
        unlockCanvasGroup.DOFade(1, 0.5f) // 0.5초 동안 불투명하게
            .OnComplete(() =>
            {
                StartCoroutine(FadeOutUnlockButton());
            });
    }

    private IEnumerator FadeOutUnlockButton()
    {
        yield return new WaitForSeconds(2); // 2초 기다림
        unlockCanvasGroup.DOFade(0, 0.5f); // 다시 0.5초 동안 투명하게
    }
}
