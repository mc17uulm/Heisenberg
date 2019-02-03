using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target{

    private int Id;
    private Vector3 Position;
    
    public Target(int Id, Vector3 Position)
    {
        this.Id = Id;
        this.Position = Position;
    }

    public int GetId()
    {
        return this.Id;
    }

    public Vector3 GetPosition()
    {
        return this.Position;
    }

}
