using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private static int ControllerId;
    private static Event.Type EventType;
    private static Task ActualTask;

    private static float[] PressValues;

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

        PressValues = new float[] { 0.0f, 0.0f };
        ActualTask = null;

    }

    // Update is called once per frame
    void Update () {

        UpdatePressedValue();

        newController.transform.rotation = controller.transform.rotation;

        // If mode is on 3DOF (DOF.THREE), the controller can only change rotation, not position
        switch(ActualTask.GetDegreeOfFreedom())
        {
            case DOF.SIX:
                newController.transform.position = controller.transform.position;
                break;

            case DOF.THREE:
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
        EventType = Event.Type.Released;
    }

    private void ControllerOnClick(object sender, ClickedEventArgs e)
    {
        EventType = Event.Type.ClickEvent;
    }

    private void ControllerPadClick(object sender, ClickedEventArgs e)
    {
        EventType = Event.Type.PadClick;
    }

    private void ControllerPadRelease(object sender, ClickedEventArgs e)
    {
        EventType = Event.Type.PadRelease;
    }

    private void ControllerPadTouch(object sender, ClickedEventArgs e)
    {
        EventType = Event.Type.PadTouch;
    }

    private void ControllerPadUntouch(object sender, ClickedEventArgs e)
    {
        EventType = Event.Type.PadUntouch;
    }

    public static void UpdateTask(Task task)
    {
        ActualTask = task;
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

    private static void UpdatePressedValue()
    {
        float pre = PressValues[0];
        switch(ActualTask.GetInput())
        {
            case Input.PAD:
                PressValues[0] = GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                break;
            case Input.TRIGGER:
            default:
                PressValues[0] = GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
                break;

        }
        PressValues[1] = pre;

        if(PressValues[1] >= 1 && PressValues[0] < 1)
        {
            HandleClick();
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
            if(Hits.Length > 0)
            {
                Event now = new Event(
                    PressValues[0],
                    controller.transform.position,
                    controller.transform.rotation.eulerAngles,
                    Hits[0].point
                );

                now.SetType(EventType);

                EventType = ActualTask.GetCircle().GetTarget().AddEvent(now);
            }
            
        } 
        else if(Contains(Hits, "TargetPanel", out Obj, out Hit))
        {
            Event now = new Event(
                    PressValues[0],
                    controller.transform.position,
                    controller.transform.rotation.eulerAngles,
                    Hits[0].point
                );

            now.SetType(EventType);

            EventType = ActualTask.GetCircle().GetTarget().AddEvent(now);
        }
    }
    
    private static void HandleClick()
    {
        switch(Processing.GetState())
        {
            case State.WAIT_FOR_BALLISTIC:
                Processing.SetState(State.ACITVATE_TIMER);
                break;
        }
    }

    private void HandleSphereHit()
    {
        switch(Processing.GetState())
        {
            
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

}
