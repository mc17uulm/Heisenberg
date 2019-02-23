using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
*   Tunnel records position and events of the controller
*/

public class Tunnel : MonoBehaviour {
    
    private static SteamVR_TrackedController controller;
    public GameObject newController;
    public Vector3 StaticPosition;
    private static SteamVR_Controller.Device device = null;
    private static int ControllerId;
    private static EventLog.Type EventType;
    private static Task ActualTask;

    private static float[] PressValues;
    private static float PressValue = 0.0f;
    private static bool Initalized = false;

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
    void Update()
    {

        if (Initalized)
        {
            UpdatePressedValue();

            newController.transform.rotation = controller.transform.rotation;
            
            // If mode is on 3DOF (DOF.THREE), the controller can only change rotation, not position
            switch (Processing.GetActualTask().GetDegreeOfFreedom())
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
	}

    private void ControllerOnRelease(object sender, ClickedEventArgs e)
    {
        EventType = EventLog.Type.Released;
        EventLogger.AddEvent(EventLog.Type.Released);
    }

    private void ControllerOnClick(object sender, ClickedEventArgs e)
    {
        EventType = EventLog.Type.ClickEvent;
        EventLogger.AddEvent(EventLog.Type.ClickEvent);
    }

    private void ControllerPadClick(object sender, ClickedEventArgs e)
    {
        EventType = EventLog.Type.PadClick;
        EventLogger.AddEvent(EventLog.Type.PadClick);
    }

    private void ControllerPadRelease(object sender, ClickedEventArgs e)
    {
        EventType = EventLog.Type.PadRelease;
        EventLogger.AddEvent(EventLog.Type.PadRelease);
    }

    private void ControllerPadTouch(object sender, ClickedEventArgs e)
    {
        EventType = EventLog.Type.PadTouch;
        EventLogger.AddEvent(EventLog.Type.PadTouch);
    }

    private void ControllerPadUntouch(object sender, ClickedEventArgs e)
    {
        EventType = EventLog.Type.PadUntouch;
        EventLogger.AddEvent(EventLog.Type.PadUntouch);
    }

    public static void IsInitalized()
    {
        Initalized = true;
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
        float Before = PressValue;
        float Now;
        switch (Processing.GetActualTask().GetInput())
        {
            case InputType.PAD:
                Now = GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
                break;
            case InputType.TRIGGER:
            default:
                Now = GetDevice().GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;
                break;
        }

        EventLogger.SetPressValue(Now);
        if (Now > 0.1f && Now < 1.0f)
        {
            if(Before <= 0.1f)
            {
                EventLogger.AddEvent(EventLog.Type.TriggerPressedFirst);
            }
            else if(Before >= 1.0f)
            {
                EventLogger.AddEvent(EventLog.Type.Position);
            }
            else
            {
                EventLogger.AddEvent(EventLog.Type.TriggerPressed);
            }
        }
        else if(Now >= 1.0f)
        {
            if (Before <= 0.1f)
            {
                Processing.HandleClick();
                EventLogger.AddEvent(EventLog.Type.ClickedFirst);
            }
            else if (Before >= 1.0f)
            {
                EventLogger.AddEvent(EventLog.Type.Clicked);
            }
            else
            {
                EventLogger.AddEvent(EventLog.Type.Position);
            }
        }
        else
        {
            EventLogger.AddEvent(EventLog.Type.Position);
        }

        PressValue = Now;
    }

    public static float[] GetPressValues()
    {
        return PressValues;
    }
    
}
