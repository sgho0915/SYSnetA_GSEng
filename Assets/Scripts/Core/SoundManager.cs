using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> touchSoundList = new List<AudioClip>(); // ��ġ ȿ����
    public List<AudioClip> alarmSoundList = new List<AudioClip>(); // ��ġ ȿ����
    public AudioSource touchSndAudioSource;
    public AudioSource alarmSndAudioSource;
    public AudioSource ExampleAudioSource;
    private WaitForSeconds playInterval = new(3); // �溸�� ��� ����
    private WaitForSeconds playInterval2 = new(0.5f); // �溸�� ��� ����

    public static SoundManager Instance { get; private set; }

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

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        touchSndAudioSource.volume = (float)(SettingManager.volLevel * 0.1f);
        alarmSndAudioSource.volume = (float)(SettingManager.volLevel * 0.1f);

        if (SettingManager.touchSound != 0)
            touchSndAudioSource.clip = touchSoundList[SettingManager.touchSound - 1];
        else
            touchSndAudioSource.clip = null;

        if (SettingManager.alarmSound != 0)
            alarmSndAudioSource.clip = alarmSoundList[SettingManager.alarmSound - 1];
        else
            alarmSndAudioSource.clip = null;

        StartCoroutine(PlayTouchSound());
    }

    public void SaveSoundsData(int p_volLevel, int p_touchSnd, int p_alarmSound)
    {
        // ���� ũ�� ����
        touchSndAudioSource.volume = (float)(p_volLevel * 0.1f);
        alarmSndAudioSource.volume = (float)(p_volLevel * 0.1f);

        // ��ġ�� ����
        if(p_touchSnd != 0)
            touchSndAudioSource.clip = touchSoundList[p_touchSnd - 1];
        else
            touchSndAudioSource.clip = null;

        // �溸�� ����
        if (p_alarmSound != 0)
            alarmSndAudioSource.clip = alarmSoundList[p_alarmSound - 1];
        else
            alarmSndAudioSource.clip = null;
    }


    private IEnumerator PlayTouchSound()
    {
        while (true)
        {
            yield return new WaitUntil(() => SettingManager.touchSound != 0);
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));

            touchSndAudioSource.Play(); // ��ġ ȿ���� ���
        }
    }

    public IEnumerator PlayAlarmSound()
    {
        while (true)
        {
            alarmSndAudioSource.Play(); // �溸 ȿ���� ���           

            yield return playInterval;
        }
    }

    public IEnumerator PlayTTSAlarmSound(DataTable realTimeWarningTable)
    {
        string cname = string.Empty;
        string desc = string.Empty;        

        while (true)
        {
            foreach (DataRow row in realTimeWarningTable.Rows)
            {
                cname = row["CNAME"].ToString();
                desc = row["DESC"].ToString();
                                
                TTSManager.instance.RunTTS($"{cname}���� {desc}�Ͽ����ϴ�.");
                yield return new WaitUntil(() => TTSManager.instance.mAudio.clip != null);
                
                yield return new WaitForSeconds(TTSManager.instance.mAudio.clip.length);

                yield return playInterval;
            }            
        }
    }
}
