using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target{

    private List<Vector3> Circle;
    private int Amplitude;
    private int Size;
    
    public Target(List<Vector3> Circle, int Amplitude, int Size)
    {
        this.Circle = Circle;
        this.Amplitude = Amplitude;
        this.Size = Size;
    }

    public List<Vector3> GetCircle()
    {
        return this.Circle;
    }

    public int GetAmplitude()
    {
        return this.Amplitude;
    }

    public int GetSize()
    {
        return this.Size;
    }

}
