using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Target{

    private int Id;
    private Vector3 Position;
    private List<EventLog> Events;
    
    public Target(int Id, Vector3 Position)
    {
        this.Id = Id;
        this.Position = Position;
        this.Events = new List<EventLog>();
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

    public Vector3 GetPressedPosition()
    {
        List<EventLog> filtered = this.Events.Where(el => el.GetType().Equals(EventLog.Type.TriggerPressedFirst)).ToList();
        return filtered[filtered.Count - 1].GetPointerPos();
    }

    public Vector3 GetClickedPosition()
    {
        List<EventLog> filtered = this.Events.Where(el => el.GetType().Equals(EventLog.Type.ClickedFirst)).ToList();
        return filtered[filtered.Count - 1].GetPointerPos();
    }

}
