using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{

    private int Round;
    private int Amplitude;
    private int Size;
    private List<Target> Targets;

    public Circle(int Amplitude, int Size, List<Target> Targets)
    {
        this.Round = 0;
        this.Amplitude = Amplitude;
        this.Size = Size;
        this.Targets = Targets;
    }

    public Target GetTarget()
    {
        return this.Targets[this.Round];
    }

    public void NewRound()
    {
        this.Round++;
    }

    public List<Target> GetTargets()
    {
        return this.Targets;
    }
}
