using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target{

    private int Id;
    private Vector3 Position;
    private List<Event> Events;
    
    public Target(int Id, Vector3 Position)
    {
        this.Id = Id;
        this.Position = Position;
        this.Events = new List<Event>();
    }

    public int GetId()
    {
        return this.Id;
    }

    public Vector3 GetPosition()
    {
        return this.Position;
    }

    public List<Event> GetEvents()
    {
        return this.Events;
    }

    public Event.Type AddEvent(Event ev)
    {
        Event last = this.Events[this.Events.Count - 1];
        float TriggerPress = ev.GetPressedValue();
        if (this.Events.Count > 0)
        {
            if(TriggerPress > 0.1f && TriggerPress < 1.0f)
            {
                switch(last.GetType())
                {
                    case Event.Type.TriggerPressed:
                    case Event.Type.TriggerPressedFirst:
                        ev.SetType(Event.Type.TriggerPressed);
                        break;
                    default:
                        ev.SetType(Event.Type.TriggerPressedFirst);
                        break;
                }
            }
            else if(TriggerPress >= 1.0f)
            {
                switch(last.GetType())
                {
                    case Event.Type.Clicked:
                    case Event.Type.ClickedFirst:
                        ev.SetType(Event.Type.Clicked);
                        break;
                    default:
                        ev.SetType(Event.Type.ClickedFirst);
                        break;
                }
            } 
            else
            {
                ev.SetType(Event.Type.Position);
            }
        }
        
        this.Events.Add(ev);
        return ev.GetType();
    }

}
