using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BodyPosition
{
    SITTING,
    STANDING
}

public enum ArmPosition
{
    STRECHED,
    APPLIED
}

public enum DOF
{
    SIX,
    THREE
}

public enum InputType
{
    TRIGGER,
    PAD
}

public class Task
{

    private int Id;
    private int Round;
    private BodyPosition BodyPos;
    private ArmPosition ArmPos;
    private DOF DegreeOfFreedom;
    private InputType Input;
    private List<Circle> Circles;
    private List<Position> Stack;


    public Task(int Id, BodyPosition Body, ArmPosition Arm, DOF dof, InputType Input = InputType.TRIGGER)
    {
        this.Id = Id;
        this.Round = 0;
        this.BodyPos = Body;
        this.ArmPos = Arm;
        this.DegreeOfFreedom = dof;
        this.Input = Input;
        this.Circles = new List<Circle>();
        this.Stack = new List<Position>();
    }

    public void Reset()
    {
        this.Round = 0;
        foreach(Circle c in this.Circles)
        {
            c.Reset();
        }
    }

    public int GetId()
    {
        return this.Id;
    }

    public string PrintCommand(int i = 0)
    {
        string o = "Runde " + i + "\r\n";
        o += "Position: " + (this.BodyPos.Equals(BodyPosition.SITTING) ? "STEHEND" : "SITZEND") + "\r\n";
        o += "Arm: " + (this.ArmPos.Equals(ArmPosition.STRECHED) ? "AUSGESTRECKT" : "ANGELEGT") + "\r\n";
        o += "Click: " + (this.Input.Equals(InputType.TRIGGER) ? "TRIGGER" : "PAD") + "\r\n";
        o += "DOF: " + (this.DegreeOfFreedom.Equals(DOF.SIX) ? 6 : 3) + "\r\n";
        o += "\r\n\r\n\r\n";

        return o;
    }

    public void CreateCircles(LatinSquare LTT, int c)
    {
        int[] column = LTT.GetColumn(c);
        for (int j = 0; j < column.Length; j++)
        {
            int Amp = 1;
            int Size = 1;
            if (column[j] <= 3)
            {
                Amp = Config.TargetAmplitudes[0];
                Size = Config.TargetWidths[column[j] - 1];
            }
            else
            {
                Amp = Config.TargetAmplitudes[1];
                Size = Config.TargetWidths[column[j] - 4];
            }
            List<Target> Targets = new List<Target>();
            int n = Config.CircleSize;
            for (int k = 0; k < n; k++)
            {
                Vector3 v = new Vector3(Convert.ToSingle(((double) Amp / 2) * Math.Sin(k * ((double) 360 / n) * (Math.PI / 180))), Convert.ToSingle(((double) Amp / 2) * Math.Cos(k * ((double) 360 / n) * (Math.PI / 180))), 0);
                Targets.Add(new Target(k, v));
            }
            this.Circles.Add(new Circle(j, Amp, Size, this.SortCircle(Targets)));
        }
    }

    public Circle GetCircle()
    {
        return this.Circles[this.Round];
    }

    public List<Circle> GetCircles()
    {
        return this.Circles;
    }

    public InputType GetInput()
    {
        return this.Input;
    }

    public ArmPosition GetArmPosition()
    {
        return this.ArmPos;
    }

    public string PrintArmPosition()
    {
        return Enum.GetName(typeof(ArmPosition), this.ArmPos);
    }

    public BodyPosition GetBodyPosition()
    {
        return this.BodyPos;
    }

    public string PrintBodyPosition()
    {
        return Enum.GetName(typeof(BodyPosition), this.BodyPos);
    }

    public DOF GetDegreeOfFreedom()
    {
        return this.DegreeOfFreedom;
    }

    public string PrintDOF()
    {
        return Enum.GetName(typeof(DOF), this.DegreeOfFreedom);
    }

    public bool HasNewRound()
    {

        if(this.Round == this.Circles.Count-1)
        {
            return false;
        }
        else
        {
            this.Round++;
            return true;
        }
    }

    public void AddToStack(Position pos)
    {
        this.Stack.Add(pos);
    }

    public List<Target> SortCircle(List<Target> Targets)
    {
        List<Target> o = new List<Target>();
        o.Add(Targets[0]);
        int m = Targets.Count - 1;
        for(int i = 1; i < Targets.Count; i++)
        {
            int x = (int) Math.Ceiling((double) i / 2) - 1;
            if((i%2) == 0)
            {
                o.Add(Targets[m - x]);
            }
            else
            {
                o.Add(Targets[(int)Math.Floor((float)m / 2) - x]);
            }
        }
        return o;
    }



}
