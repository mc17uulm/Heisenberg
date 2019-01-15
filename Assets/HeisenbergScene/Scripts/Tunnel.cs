using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TunnelState
{
    SIX,
    THREE
}

public class Tunnel : MonoBehaviour {


    private static SteamVR_TrackedController controller;
    public GameObject newController;
    public Vector3 position;
    private static SteamVR_Controller.Device device = null;
    private static TunnelState mode;
    private static int ControllerId;


    private void Awake()
    {
        controller = GetComponent<SteamVR_TrackedController>();

        if (controller == null)
        {
            controller = GetComponentInParent<SteamVR_TrackedController>();
        }
        if (controller == null)
        {
            Debug.Log("TrackedContoroller still null");
            Application.Quit();
        }

        controller.TriggerUnclicked -= ControllerOnRelease;
        controller.TriggerUnclicked += ControllerOnRelease;
        controller.TriggerClicked -= ControllerOnClick;
        controller.TriggerClicked += ControllerOnClick;
        controller.PadClicked -= ControllerOnTab;
        controller.PadClicked += ControllerOnTab;

        if (position == null)
        {
            position = new Vector3(0, 0, 0);
        }

        mode = TunnelState.SIX;

    }

    // Update is called once per frame
    void Update () {
        newController.transform.rotation = controller.transform.rotation;
        switch(mode)
        {
            case TunnelState.SIX:
                newController.transform.position = controller.transform.position;
                break;

            case TunnelState.THREE:
                newController.transform.position = position;
                break;

            default:
                Debug.Log("No valid mode");
                newController.transform.position = controller.transform.position;
                break;
        }
	}

    private void ControllerOnRelease(object sender, ClickedEventArgs e)
    {
        Processing.addEvent(PointerEvent.Released);
    }

    private void ControllerOnClick(object sender, ClickedEventArgs e)
    {
        Processing.addEvent(PointerEvent.ClickEvent);
    }

    private void ControllerOnTab(object sender, ClickedEventArgs e)
    {
        Debug.Log("TabClick");
        //Processing.addEvent(PointerEvent.TabClick);
    }

    public static void ChangeMode(TunnelState state)
    {
        mode = state;
    }

    public static float getTriggerPressed()
    {
        if (device == null)
        {
            ControllerId = (int)controller.controllerIndex;
            Debug.Log("Init id: " + ControllerId);
            device = SteamVR_Controller.Input(ControllerId);
            return 0;
        }
        else
        {
            int id = (int)controller.controllerIndex;
            if(id != ControllerId)
            {
                ControllerId = id;
                device = SteamVR_Controller.Input(ControllerId);
                Debug.Log("Changed id: " + id);
            }
            Vector3 vec = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            Debug.Log(vec);
            return vec.x;
        }
    }
}
