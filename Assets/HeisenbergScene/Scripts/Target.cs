using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Target{

    private int Id;
    private Vector3 Position;
    private Vector3 PositionWorld;
    private List<EventLog> Events;
    private Vector2 ActualMovement;
    private EventLog[] FAL;
    
    public Target(int Id, Vector3 Position)
    {
        this.Id = Id;
        this.Position = Position;
        this.Events = new List<EventLog>();
        this.FAL = new EventLog[2];
    }

    public int GetId()
    {
        return this.Id;
    }

    public Vector3 GetPosition()
    {
        return this.Position;
    }

    public Vector3 GetWorldPosition()
    {
        return this.PositionWorld;
    }

    public void SetWorldPosition(Vector3 WorldPosition)
    {
        this.PositionWorld = WorldPosition;
    }

    public List<EventLog> GetEvents()
    {
        return this.Events;
    }

    public EventLog.Type AddEvent(EventLog ev)
    {
        if (this.Events.Count > 0)
        {
            EventLog last = this.Events[this.Events.Count - 1];
            float TriggerPress = ev.GetPressedValue();
            if (TriggerPress > 0.1f && TriggerPress < 1.0f)
            {
                switch(last.GetType())
                {
                    case EventLog.Type.TriggerPressed:
                    case EventLog.Type.TriggerPressedFirst:
                        ev.SetType(EventLog.Type.TriggerPressed);
                        break;
                    default:
                        ev.SetType(EventLog.Type.TriggerPressedFirst);
                        break;
                }
            }
            else if(TriggerPress >= 1.0f)
            {
                switch(last.GetType())
                {
                    case EventLog.Type.Clicked:
                    case EventLog.Type.ClickedFirst:
                        ev.SetType(EventLog.Type.Clicked);
                        break;
                    default:
                        ev.SetType(EventLog.Type.ClickedFirst);
                        break;
                }
            } 
            else
            {
                ev.SetType(EventLog.Type.Position);
            }
        }
        
        this.Events.Add(ev);
        return ev.GetType();
    }

    public List<EventLog> GetPressedPosition()
    {
        return this.Events.Where(el => el.GetType().Equals(EventLog.Type.TriggerPressedFirst)).ToList();
    }

    public List<EventLog> GetClickedPosition()
    {
        return this.Events.Where(el => el.GetType().Equals(EventLog.Type.ClickedFirst)).ToList();
    }

    public List<EventLog> GetSum()
    {
        List<EventLog> o = new List<EventLog>();
        if (this.Events.Count > 0)
        {
            EventLog Pressed = this.Events[0];
            EventLog Clicked = Pressed;
            foreach (EventLog Log in this.Events)
            {
                switch (Log.GetType())
                {
                    case EventLog.Type.TriggerPressedFirst:
                        Pressed = Log;
                        break;
                    case EventLog.Type.ClickedFirst:
                        Clicked = Log;
                        break;
                    case EventLog.Type.Clicked:
                        o.Add(Pressed);
                        o.Add(Clicked);
                        break;
                    default:
                        break;
                }
            }
        }
        return o;
    }

    public List<EventLog> GetFirsts()
    {
        return this.Events.Where(el => el.GetType().Equals(EventLog.Type.ClickedFirst) || el.GetType().Equals(EventLog.Type.TriggerPressedFirst)).ToList();
    }

    private List<EventLog> GetBallisticEvents()
    {
        return this.Events.Where(el => el.GetBallistic()).ToList();
    }

    public void GetFirstAndLast()
    {
        List<EventLog> ballistic = this.GetBallisticEvents();
        this.FAL = new EventLog[] { ballistic[0], ballistic[ballistic.Count - 1] };
    }

    public float CalculateMovementTime()
    {
        this.GetFirstAndLast();
        long time = this.FAL[1].GetTimestamp() - this.FAL[0].GetTimestamp();
        return time;
    }

    public float CalculateDistance()
    {
        Debug.Log("TargetID: " + this.Id);
        Vector3 first = this.FAL[0].GetPointerPos();
        Vector3 last = this.FAL[1].GetPointerPos();
        Debug.Log("First: " + first.ToString());
        Debug.Log("Last: " + last.ToString());
        this.ActualMovement = new Vector2(last.x - first.x, last.y - first.y);
        Debug.Log("ActualMovement: " + this.ActualMovement.ToString() + " | Magn: " + this.ActualMovement.magnitude);
        return this.ActualMovement.magnitude;
    }

    public float ClaculateDeviation()
    {
        Debug.Log("TargetID: " + this.Id);
        Vector3 first = this.FAL[0].GetPointerPos();
        Debug.Log("Target: " + this.PositionWorld.ToString());
        Vector2 intendedVector = new Vector2(this.PositionWorld.x - first.x, this.PositionWorld.y - first.y);
        Debug.Log("IntendedVec: " + intendedVector.ToString());
        Vector2 projectedActualMovement = Vector3.Project(this.ActualMovement, intendedVector);
        Debug.Log("ProjectedMove: " + projectedActualMovement.ToString());
        float o = Mathf.Abs(projectedActualMovement.magnitude - intendedVector.magnitude);
        Debug.Log("ProjectedMag: " + projectedActualMovement.magnitude + " | IntendedVector: " + intendedVector.magnitude + " => " + o);
        return o;
    }



}
