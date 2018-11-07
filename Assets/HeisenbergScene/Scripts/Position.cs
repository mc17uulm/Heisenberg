using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointerEvent
{
    TriggerPressed,
    ClickEvent,
    Clicked,
    Released,
    None
}

public class Position {

    private long timestamp;
    private PointerEvent ev;
    private float pressStrength;
    private Vector3 ControllerPos;
    private Vector3 ControllerRot;
    private Vector3 TargetPos;
    private Vector3 PointerPos;

    public Position(long timestamp, PointerEvent ev, float pressStrength, Vector3 controllerPos, Vector3 controllerRot, Vector3 targetPos, Vector3 pointerPos)
    {
        this.timestamp = timestamp;
        this.ev = ev;
        this.pressStrength = pressStrength;
        this. ControllerPos = controllerPos;
        this.ControllerRot = controllerRot;
        this.TargetPos = targetPos;
        this.PointerPos = pointerPos;

    }

    public long GetTimestamp()
    {
        return this.timestamp;
    }

    public void SetEvent(PointerEvent ev)
    {
        this.ev = ev;
    }

    public PointerEvent GetEvent()
    {
        return this.ev;
    }

    public string PrintEvent()
    {
        switch(this.ev)
        {
            case PointerEvent.TriggerPressed:
                return "TriggerPressed";

            case PointerEvent.Clicked:
                return "Clicked";

            case PointerEvent.Released:
                return "Released";

            case PointerEvent.None:
                return "None";

            default:
                return "None";

        }
    }

    public float GetPressStrength()
    {
        return this.pressStrength;
    }

    public Vector3 GetControllerPos()
    {
        return this.ControllerPos;
    }

    public Vector3 GetControllerRot()
    {
        return this.ControllerRot;
    }

    public Vector3 GetTargetPos()
    {
        return this.TargetPos;
    }

    public Vector3 GetPointerPos()
    {
        return this.PointerPos;
    }
}
