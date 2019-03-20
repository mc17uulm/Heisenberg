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
    private State state;
    private float PressedValue;
    private bool Ballistic;
    private Vector3 ControllerPosition;
    private Vector3 ControllerRotation;
    private Vector3 PointerPosition;

    public EventLog(float PressedValue, bool Ballistic, Vector3 ControllerPosition, Vector3 ControllerRotation, Vector3 PointerPosition, State state)
    {
        this.Timestamp = GetNow();
        this.type = Type.Position;
        this.PressedValue = PressedValue;
        this.Ballistic = Ballistic;
        this.ControllerPosition = ControllerPosition;
        this.ControllerRotation = ControllerRotation;
        this.PointerPosition = PointerPosition;
        this.state = state;
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

    public new State GetState()
    {
        return this.state;
    }

    public long GetTimestamp()
    {
        return this.Timestamp;
    }

    public bool GetBallistic()
    {
        return this.Ballistic;
    }

    public Vector3 GetControllerPos()
    {
        return this.ControllerPosition;
    }

    public Vector3 GetControllerRot()
    {
        return this.ControllerRotation;
    }

    public Vector3 GetPointerPos()
    {
        return this.PointerPosition;
    }

    public string PrintType()
    {
        return Enum.GetName(typeof(Type), this.type);
    }

    public string PrintState()
    {
        return Enum.GetName(typeof(State), this.state);
    }

    private long GetNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }



}
