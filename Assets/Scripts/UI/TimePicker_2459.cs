using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class TimePicker_2459 : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public TMP_InputField hourInputField;
    public TMP_InputField minuteInputField;

    private int hour;
    private int minute;

    private bool isPressingHourUp = false;
    private bool isPressingHourDown = false;
    private bool isPressingMinuteUp = false;
    private bool isPressingMinuteDown = false;

    private float acceleration = 0.9f; // 값이 작을수록 빠른 가속

    private void Start()
    {
        SetHour(0);
        SetMinute(0);
    }

    public void OnHourUp()
    {
        SetHour(hour + 1);
    }

    public void OnHourDown()
    {
        SetHour(hour - 1);
    }

    public void OnMinuteUp()
    {
        SetMinute(minute + 1);
    }

    public void OnMinuteDown()
    {
        SetMinute(minute - 1);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerEnter.name == "btn_HourUp")
        {
            isPressingHourUp = true;
            StartCoroutine(AccelerateHour(true));
        }
        else if (eventData.pointerEnter.name == "btn_HourDown")
        {
            isPressingHourDown = true;
            StartCoroutine(AccelerateHour(false));
        }
        else if (eventData.pointerEnter.name == "btn_MinuteUp")
        {
            isPressingMinuteUp = true;
            StartCoroutine(AccelerateMinute(true));
        }
        else if (eventData.pointerEnter.name == "btn_MinuteDown")
        {
            isPressingMinuteDown = true;
            StartCoroutine(AccelerateMinute(false));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressingHourUp = false;
        isPressingHourDown = false;
        isPressingMinuteUp = false;
        isPressingMinuteDown = false;
    }

    IEnumerator AccelerateHour(bool isUp)
    {
        float waitTime = 0.5f;
        while ((isUp && isPressingHourUp) || (!isUp && isPressingHourDown))
        {
            if (isUp) OnHourUp();
            else OnHourDown();

            waitTime *= acceleration;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator AccelerateMinute(bool isUp)
    {
        float waitTime = 0.5f;
        while ((isUp && isPressingMinuteUp) || (!isUp && isPressingMinuteDown))
        {
            if (isUp) OnMinuteUp();
            else OnMinuteDown();

            waitTime *= acceleration;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void SetHour(int value)
    {
        hour = Mathf.Clamp(value, 0, 23);
        hourInputField.text = hour.ToString("00");
    }

    private void SetMinute(int value)
    {
        minute = Mathf.Clamp(value, 0, 59);
        minuteInputField.text = minute.ToString("00");
    }
}
