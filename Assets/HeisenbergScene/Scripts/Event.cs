using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventLog
{

    public enum Type
    {
        Position,
        TriggerPressed,
        TriggerPressedFirst,
        ClickEvent,
        Clicked,
        ClickedFirst,
        Released,
        PadClick,
        PadRelease,
        PadTouch,
        PadUntouch
    }

    private long Timestamp;
    private Type type;
    private float PressedValue;
    private Vector3 ControllerPosition;
    private Vector3 ControllerRotation;
    private Vector3 PointerPosition;

    public EventLog(float PressedValue, Vector3 ControllerPosition, Vector3 ControllerRotation, Vector3 PointerPosition)
    {
        this.Timestamp = GetNow();
        this.type = Type.Position;
        this.PressedValue = PressedValue;
        this.ControllerPosition = ControllerPosition;
        this.ControllerRotation = ControllerRotation;
        this.PointerPosition = PointerPosition;
    }

    public float GetPressedValue()
    {
        return this.PressedValue;
    }

    public void SetType(Type t)
    {
        this.type = t;
    }

    public new Type GetType()
    {
        return this.type;
    }

    public string PrintType()
    {
        return Enum.GetName(typeof(Type), this.type);
    }

    private long GetNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }



}
