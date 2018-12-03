using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour {

    private static SteamVR_TrackedController controller;
    public GameObject newController;
    public Vector3 position;
    private static SteamVR_Controller.Device device = null;
    private static string mode;

    private void Awake()
    {
        Debug.Log("ON ENABLE");
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

        mode = "fulldsgsd";

    }

    // Update is called once per frame
    void Update () {
        newController.transform.rotation = controller.transform.rotation;
        if(mode.Equals("full"))
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

    public static void changeMode(string m)
    {
        mode = m;
    }

    public static float getTriggerPressed()
    {
        if (device == null)
        {
            device = SteamVR_Controller.Input((int)controller.controllerIndex);
            return 0;
        }
        else
        {
            Vector3 vec = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            return vec.x;
        }
    }
}
