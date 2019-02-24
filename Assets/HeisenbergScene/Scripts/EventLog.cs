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
    private bool Ballistic;
    private Vector3 ControllerPosition;
    private Vector3 ControllerRotation;
    private Vector3 PointerPosition;
    private Task Task;
    private Circle Circle;
    private Target Target;

    public EventLog(float PressedValue, bool Ballistic, Vector3 ControllerPosition, Vector3 ControllerRotation, Vector3 PointerPosition, Task Task, Circle Circle, Target Target)
    {
        this.Timestamp = GetNow();
        this.type = Type.Position;
        this.PressedValue = PressedValue;
        this.Ballistic = Ballistic;
        this.ControllerPosition = ControllerPosition;
        this.ControllerRotation = ControllerRotation;
        this.PointerPosition = PointerPosition;
        this.Task = Task;
        this.Circle = Circle;
        this.Target = Target;
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

    public Task GetTask()
    {
        return this.Task;
    }

    public Circle GetCircle()
    {
        return this.Circle;
    }

    public Target GetTarget()
    {
        return this.Target;
    }

    public string PrintType()
    {
        return Enum.GetName(typeof(Type), this.type);
    }

    private long GetNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }

    public string PrintIds()
    {
        return string.Format("{0};{1};{2}", Task.GetId(), Circle.GetId(), Target.GetId());
    }

    public string PrintTask()
    {
        return string.Format("{0};{1};{2}", Task.PrintArmPosition(), Task.PrintBodyPosition(), Task.PrintDOF());
    }

    public string PrintCircle()
    {
        return string.Format("{0};{1}", Circle.GetAmplitude(), Circle.GetSize());
    }

    public string PrintTarget()
    {
        return string.Format("{0};{1};{2}",
            Target.GetPosition().x,
            Target.GetPosition().y,
            Target.GetPosition().z
        );
    }

    public string PrintPositions()
    {
        return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",
            ControllerPosition.x,
            ControllerPosition.y,
            ControllerPosition.z,
            ControllerRotation.x,
            ControllerRotation.y,
            ControllerRotation.z,
            Target.GetPosition().x,
            Target.GetPosition().y,
            Target.GetPosition().z,
            PointerPosition.x,
            PointerPosition.y
        );
    }





}
