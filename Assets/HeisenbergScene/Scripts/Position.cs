using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PointerEvent
{
    TriggerPressed,
    TriggerPressedFirst,
    ClickEvent,
    Clicked,
    ClickedFirst,
    Released,
    PadClick,
    PadRelease,
    PadTouch,
    PadUntouch,
    None
}

public class Position {

    private long Timestamp;
    private PointerEvent Event;
    private float pressStrength;
    private Vector3 ControllerPos;
    private Vector3 ControllerRot;
    private Vector3 targetPos;
    private int targetId;
    private Vector3 PointerPos;

    public Position(float pressStrength, Vector3 controllerPos, Vector3 controllerRot, Vector3 targetPos, int targetId, Vector3 pointerPos)
    {
        this.Timestamp = this.GetNow();
        this.Event = PointerEvent.None;
        this.pressStrength = pressStrength;
        this.ControllerPos = controllerPos;
        this.ControllerRot = controllerRot;
        this.targetPos = targetPos;
        this.targetId = targetId;
        this.PointerPos = pointerPos;
    }

    public long GetTimestamp()
    {
        return this.Timestamp;
    }

    public void SetEvent(PointerEvent ev)
    {
        this.Event = ev;
    }

    public PointerEvent GetEvent()
    {
        return this.Event;
    }

    public string PrintEvent()
    {
        switch(this.ev)
        {
            case PointerEvent.TriggerPressed:
                return "TriggerPressed";

            case PointerEvent.TriggerPressedFirst:
                return "TriggerPressedFirst";

            case PointerEvent.Clicked:
                return "Clicked";

            case PointerEvent.ClickedFirst:
                return "ClickedFirst";

            case PointerEvent.ClickEvent:
                return "ClickEvent";

            case PointerEvent.Released:
                return "Released";

            case PointerEvent.PadClick:
                return "PadClicked";

            case PointerEvent.PadRelease:
                return "PadReleased";

            case PointerEvent.PadTouch:
                return "PadTouched";

            case PointerEvent.PadUntouch:
                return "PadUntouched";

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
        return this.targetPos;
    }

    public int GetTargetId()
    {
        return this.targetId;
    }

    public Vector3 GetPointerPos()
    {
        return this.PointerPos;
    }
    private long GetNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }
}
