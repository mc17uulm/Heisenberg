using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target{

    private Vector3 position;
    private int id;
    
    public Target(Vector3 position, int id)
    {
        this.position = position;
        this.id = id;
    }

    public Vector3 GetPosition()
    {
        return this.position;
    }

    public int GetId()
    {
        return this.id;
    }

}
