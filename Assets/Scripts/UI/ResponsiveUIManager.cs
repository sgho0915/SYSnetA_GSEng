using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsiveUIManager : MonoBehaviour
{
    public static ResponsiveUIManager Instance { get; private set;}

    public List<GameObject> upperMenu_List = new List<GameObject>();    

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
        InitUpperMenu();
    }

    public void InitUpperMenu()
    {
        if(ScreenManager.isTablet)
        {
            foreach (GameObject obj in upperMenu_List)
            {
                obj.SetActive(true);
            }
            upperMenu_List[3].SetActive(false);
        }
        else
        {
            foreach (GameObject obj in upperMenu_List)
            {
                obj.SetActive(false);
            }

            upperMenu_List[2].SetActive(true);
            upperMenu_List[3].SetActive(true);
            upperMenu_List[2].transform.Find("txt_time").gameObject.SetActive(false);
            upperMenu_List[2].transform.Find("Status_Internet").gameObject.SetActive(false);
        }
    }
}
