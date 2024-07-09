using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class ControllerStyleToggle : MonoBehaviour
{
    GameObject imgSelected;

    private void Awake()
    {
        imgSelected = this.gameObject.transform.Find("Img_Selected").gameObject;
        imgSelected.SetActive(false);
        this.gameObject.GetComponent<Toggle>().onValueChanged.AddListener(delegate
        {
            ShowSelectedImage();
        });
    }

    public void ShowSelectedImage()
    {
        if (this.gameObject.GetComponent<Toggle>().isOn)
            imgSelected.SetActive(true);
        else
            imgSelected.SetActive(false);
    }
}
