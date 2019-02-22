using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Target{

    private int Id;
    private Vector3 Position;
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
        Debug.Log("TargetId: " + this.Id + " | Count Ballistic: " + ballistic.Count);
        this.FAL = new EventLog[] { ballistic[0], ballistic[ballistic.Count - 1] };
    }

    public float CalculateMovementTime()
    {
        this.GetFirstAndLast();
        return (float) this.FAL[1].GetTimestamp() - this.FAL[0].GetTimestamp();
    }

    public float CalculateDistance()
    {
        Vector3 first = this.FAL[0].GetPointerPos();
        Vector3 last = this.FAL[1].GetPointerPos();
        this.ActualMovement = new Vector2(last.x - first.x, last.y - first.y);
        return this.ActualMovement.magnitude;
    }

    public float ClaculateDeviation()
    {
        Vector3 first = this.FAL[0].GetPointerPos();
        Vector2 intendedVector = new Vector2(this.Position.x - first.x, this.Position.y - first.y);
        Vector2 projectedActualMovement = Vector3.Project(this.ActualMovement, intendedVector);
        return projectedActualMovement.magnitude - intendedVector.magnitude;
    }



}
