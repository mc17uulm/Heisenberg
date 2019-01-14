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

        if (position == null)
        {
            position = new Vector3(0, 0, 0);
        }

        mode = TunnelState.SIX;

    }

    // Update is called once per frame
    void Update () {
        newController.transform.rotation = controller.transform.rotation;
        if(mode.Equals(TunnelState.SIX))
        {
            newController.transform.position = controller.transform.position;
        }
        else
        {
            newController.transform.position = position;
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

    public static void ChangeMode(TunnelState state)
    {
        mode = state;
    }

    public static float getTriggerPressed()
    {
        if (device == null)
        {
            // (int)controller.controllerIndex
            device = SteamVR_Controller.Input(3);
            Debug.Log(device);
            return 0;
        }
        else
        {
            Vector3 vec = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            Debug.Log(vec);
            return vec.x;
        }
    }
}
