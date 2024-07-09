using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateManager : MonoBehaviour
{
    private void Awake()
    {
        //QualitySettings.vSyncCount = 0; // V-Sync를 비활성화
        Application.targetFrameRate = 120; // 프레임 제한 해제
        //Application.targetFrameRate = 60;
    }
}
