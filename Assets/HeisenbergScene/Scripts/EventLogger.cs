using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLogger
{

    private static List<EventLog> Logs = new List<EventLog>();
    private static float PressValue = 0.0f;

    public static void Add(EventLog log)
    {
        Logs.Add(log);
    }

    public static void SetPressValue(float Value)
    {
        PressValue = Value;
    }

    public static void AddHit(bool Ballistic, Vector3 Position, Vector3 Rotation, Vector3 Point)
    {
        Logs.Add(new EventLog(
            PressValue,
            Ballistic,
            Position,
            Rotation,
            Point
        ));
    }

    public static void AddEvent(EventLog.Type Event)
    {
        if(Logs.Count > 0)
        {
            Logs[Logs.Count - 1].SetType(Event);
        }
    }


}
