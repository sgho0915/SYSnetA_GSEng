using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPPSingleBottomView : MonoBehaviour
{
    GameObject objTop;
    GameObject objCenter;
    GameObject objBottom;

    private float lastTapTimeTop = 0f;
    private float lastTapTimeCenter = 0f;
    private float doubleTapThreshold = 0.5f; // ������ ���� �ð� ����

    void Start()
    {
        objTop = this.gameObject.transform.Find("Top").gameObject;
        objCenter = this.gameObject.transform.Find("Center").gameObject;
        objBottom = this.gameObject.transform.Find("Bottom").gameObject;
    }

    void Update()
    {
        // ��ġ �Է� ó��
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                Vector2 touchPosition = touch.position;
                ProcessTap(touchPosition);
            }
        }

        // ���콺 Ŭ�� �Է� ó��
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            ProcessTap(mousePosition);
        }
    }

    private void ProcessTap(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f)) // 100f�� ����ĳ��Ʈ�� �ִ� �Ÿ�
        {
            if (hit.transform.gameObject == objTop)
            {
                //objBottom.SetActive(!objBottom.activeSelf);
                HandleDoubleTap(ref lastTapTimeTop);
            }
            else if (hit.transform.gameObject == objCenter)
            {
                //objBottom.SetActive(!objBottom.activeSelf);
                HandleDoubleTap(ref lastTapTimeCenter);
            }
        }
    }

    private void HandleDoubleTap(ref float lastTapTime)
    {
        float currentTime = Time.time;

        if (currentTime - lastTapTime < doubleTapThreshold)
        {
            // ������ ����
            objBottom.SetActive(!objBottom.activeSelf);
        }

        lastTapTime = currentTime;
    }
}
