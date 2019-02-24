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

    public static void AddHit(bool Ballistic, Vector3 Position, Vector3 Rotation, Vector3 Point, Task Task)
    {
        if(Logs.Count > 0)
        {
            Session.AddToFile(Logs[Logs.Count - 1]);
        }
        Logs.Add(new EventLog(
            PressValue,
            Ballistic,
            Position,
            Rotation,
            Point,
            Task,
            Task.GetCircle(),
            Task.GetCircle().GetTarget()
        ));
    }

    public static void AddEvent(EventLog.Type Event)
    {
        if(Logs.Count > 0)
        {
            Logs[Logs.Count - 1].SetType(Event);
        }
    }

    public static List<EventLog> GetFullStack()
    {
        return Logs;
    }

    public static List<EventLog> GetFirsts()
    {
        List<EventLog> List = new List<EventLog>();
        foreach(EventLog Log in Logs)
        {
            switch(Log.GetType())
            {
                case EventLog.Type.TriggerPressedFirst:
                    if (List.Count > 0 && List[List.Count-1].GetType().Equals(Log.GetType()))
                    {
                        List[List.Count - 1] = Log;
                    }
                    else
                    {
                        List.Add(Log);
                    }
                    break;
                case EventLog.Type.ClickedFirst:
                    List.Add(Log);
                    break;
                default:
                    break;
            }
        }
        return List;
    }

    public static void GetTroughput()
    {
        int CIDB = -1;
        List<EventLog> Circle = new List<EventLog>();
        foreach(EventLog Log in Logs)
        {
            int CIDN = Log.GetCircle().GetId();
            if(CIDB == -1)
            {
                CIDB = CIDN;
            }
            else if(CIDB != CIDN)
            {
                CalculcateTroughput(Circle);
                Circle = new List<EventLog>();
            }
            else
            {
                Circle.Add(Log);
            }
        }
    }

    private static void CalculcateTroughput(List<EventLog> Circle)
    {

    }


}
