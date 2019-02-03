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

public enum Input
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
    private Input Input;
    private List<Circle> Circles;
    private List<Position> Stack;


    public Task(int Id, BodyPosition Body, ArmPosition Arm, DOF dof, Input input = Input.TRIGGER)
    {
        this.Id = Id;
        this.Round = 0;
        this.BodyPos = Body;
        this.ArmPos = Arm;
        this.DegreeOfFreedom = dof;
        this.Input = input;
        this.Circles = new List<Circle>();
        this.Stack = new List<Position>();
    }

    public string PrintCommand(int i = 0)
    {
        string o = "Runde " + i + "\r\n";
        o += "Position: " + (this.BodyPos.Equals(BodyPosition.SITTING) ? "STEHEND" : "SITZEND") + "\r\n";
        o += "Arm: " + (this.ArmPos.Equals(ArmPosition.STRECHED) ? "AUSGESTRECKT" : "ANGELEGT") + "\r\n";
        o += "Click: " + (this.Input.Equals(Input.TRIGGER) ? "TRIGGER" : "PAD") + "\r\n";

        return o;
    }

    public void CreateCircles(LatinSquare LTT)
    {
        int[] column = LTT.GetColumn(this.Id);
        for(int j = 0; j < column.Length; j++)
        {
            int Amp = 1;
            int Size = 1;
            if(column[j] <= 3)
            {
                Amp = Config.TargetAmplitudes[0];
                Size = Config.TargetWidths[column[j] - 1];
            } else
            {
                Amp = Config.TargetAmplitudes[1];
                Size = Config.TargetWidths[column[j] - 4];
            }
            List<Target> Targets = new List<Target>();
            for(int k = 0; k < Config.CircleSize; k++)
            {
                Targets.Add(new Target(k, new Vector3(Convert.ToSingle((Amp / 2) * Math.Sin((k) * (k / 360))), Convert.ToSingle((Amp / 2) * Math.Sin((k) * (k / 360))), 0)));
            }
            this.Circles.Add(new Circle(Amp, Size, Targets));
        }
    }

    public Circle GetCircle()
    {
        return this.Circles[this.Round];
    }

    public void NewRound()
    {
        this.Round++;
    }

    public void AddToStack(Position pos)
    {
        this.Stack.Add(pos);
    }



}
