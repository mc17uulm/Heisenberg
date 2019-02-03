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

public enum GridMode
{
    GRID,
    CIRCLE
}
/**
*   Tunnel records position and events of the controller
*/

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class Tunnel : MonoBehaviour {

    private static SteamVR_LaserPointer LaserPointer;
    private static SteamVR_TrackedController controller;
    public GameObject newController;
    public Vector3 StaticPosition;
    private static SteamVR_Controller.Device device = null;
    private static TunnelState mode;
    private static int ControllerId;
    private static Event Event;

    private void Awake()
    {
        controller = GetComponent<SteamVR_TrackedController>();
        LaserPointer = GetComponent<SteamVR_LaserPointer>();

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
        controller.PadClicked -= ControllerPadClick;
        controller.PadClicked += ControllerPadClick;
        controller.PadUnclicked -= ControllerPadRelease;
        controller.PadUnclicked += ControllerPadRelease;
        controller.PadTouched -= ControllerPadTouch;
        controller.PadTouched += ControllerPadTouch;
        controller.PadUntouched -= ControllerPadUntouch;
        controller.PadUntouched += ControllerPadUntouch;

        if (StaticPosition == null)
        {
            StaticPosition = Config.Start;
        }

        mode = TunnelState.SIX;

    }

    // Update is called once per frame
    void Update () {
        newController.transform.rotation = controller.transform.rotation;

        // If mode is on 3DOF (TunnelState.THREE), the controller can only change rotation, not position
        switch(mode)
        {
            case TunnelState.SIX:
                newController.transform.position = controller.transform.position;
                break;

            case TunnelState.THREE:
                newController.transform.position = StaticPosition;
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

    private void ControllerPadClick(object sender, ClickedEventArgs e)
    {
        Processing.addEvent(PointerEvent.PadClick);
    }

    private void ControllerPadRelease(object sender, ClickedEventArgs e)
    {
        Processing.addEvent(PointerEvent.PadRelease);
    }

    private void ControllerPadTouch(object sender, ClickedEventArgs e)
    {
        Processing.addEvent(PointerEvent.PadTouch);
    }

    private void ControllerPadUntouch(object sender, ClickedEventArgs e)
    {
        Processing.addEvent(PointerEvent.PadUntouch);
    }

    public static void ChangeMode(TunnelState state)
    {
        mode = state;
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
        switch(Config.clickMode)
        {
            case ClickMode.PAD:
                return GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            default:
                return GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

        }
        
    }

    public void ProcessHits()
    {
        RaycastHit[] Hits;
        Hits = Physics.RaycastAll(LaserPointer.transform.position, transform.forward, 100.0f);

        GameObject Obj;
        RaycastHit Hit;

        if(Contains(Hits, "Sphere", out Obj, out Hit))
        {
            Processing.AddData(new Position(
                GetNow(),
                Event,

            ));
        }
    }

    private Boolean Contains(RaycastHit[] hits, string name, out GameObject x, out RaycastHit hit)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.name.Equals(name))
            {
                x = hits[i].collider.gameObject;
                hit = hits[i];
                return true;
            }
        }

        x = null;
        hit = new RaycastHit();
        return false;
    }

    private long GetNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }
}
