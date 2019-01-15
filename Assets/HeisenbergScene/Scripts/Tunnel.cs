using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TunnelState
{
    SIX,
    THREE
}

public enum ClickMode
{
    TRIGGER,
    PAD
}

public class Tunnel : MonoBehaviour {


    private static SteamVR_TrackedController controller;
    public GameObject newController;
    public Vector3 position;
    private static SteamVR_Controller.Device device = null;
    private static TunnelState mode;
    private static ClickMode ClickMode;
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
        ClickMode = ClickMode.TRIGGER;

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

    public static void ChangeClickTarget(ClickMode state)
    {
        ClickMode = state;
    }

    private static SteamVR_Controller.Device GetDevice()
    {
        int id = (int)controller.controllerIndex;
        if ((device == null) || (id != ControllerId))
        {
            ControllerId = id;
            return SteamVR_Controller.Input(id);
        }

        return device;
    }

    public static float GetPressedValue()
    {
        switch(ClickMode)
        {
            case ClickMode.PAD:
                return GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            default:
                return GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

        }
        
    }
}
