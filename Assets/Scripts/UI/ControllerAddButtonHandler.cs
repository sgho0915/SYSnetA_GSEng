using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerAddButtonHandler : MonoBehaviour
{
    public Button btn_ControllerAdd;      
    private ControllerManager controllerManager;
    public UnityEvent<Transform> onButtonClicked;

    private void Start()
    {
        btn_ControllerAdd.onClick.AddListener(HandleClick);
    }

    private void Awake()
    {     
        ControllerManager[] controllers = Resources.FindObjectsOfTypeAll<ControllerManager>();
             
        if (controllers.Length > 0)
        {
            controllerManager = controllers[0];
        }
        else
        {
            //Debug.LogError("No ControllerManager found!");
        }
    }

    private void HandleClick()
    {        
        controllerManager.OnControllerAddButtonClick(transform);
    }
}
