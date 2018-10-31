﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Session
{

    private string name;
    private List<Try> tries;

    public Session()
    {
        this.name = "Marco";
        this.tries = new List<Try>();
    }

    public void AddTry(Try t)
    {
        this.tries.Add(t);
    }

    public Try GetTry(int index)
    {
        return this.tries[index];
    }

    public List<Try> GetTries()
    {
        return this.tries;
    }

    public List<Hit> GetAllHits()
    {
        List<Hit> o = new List<Hit>();
        foreach(Try t in this.tries)
        {
            o.Concat(t.GetHits());
        }

        return o;
    }

    public string GetName()
    {
        return this.name;
    }

    public void SaveToFile(string file)
    {
        if(!File.Exists(file))
        {
            using(StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("name;round;trial;ballistic;timestamp;event;TriggerValue;ControllerPos.X;ControllerPos.Y;ControllerPos.Z;ControllerRot.X;ControllerRot.Y;ControllerRot.Z;TargetPos.X;TargetPos.Y;TargetPos.Z;PointerPos.X;PointerPos.Y");
                foreach(Try tr in this.tries)
                {
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Position pos in h.GetPositions()) {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};",
                                this.name, 
                                tr.GetIndex(), 
                                h.GetIndex(), 
                                h.GetFirst(), 
                                pos.GetTimestamp(),
                                pos.PrintEvent(),
                                pos.GetPressStrength(),
                                pos.GetControllerPos().x,
                                pos.GetControllerPos().y,
                                pos.GetControllerPos().z,
                                pos.GetControllerRot().x,
                                pos.GetControllerRot().y,
                                pos.GetControllerRot().z,
                                h.GetTarget().x,
                                h.GetTarget().y,
                                h.GetTarget().z,
                                pos.GetPointerPos().x,
                                pos.GetPointerPos().y
                             ));
                        }
                    }
                }
            }
        }
        else
        {
            using (StreamWriter w = File.AppendText(file))
            {
                foreach (Try tr in this.tries)
                {
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Position pos in h.GetPositions())
                        {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};",
                                this.name,
                                tr.GetIndex(),
                                h.GetIndex(),
                                h.GetFirst(),
                                pos.GetTimestamp(),
                                pos.PrintEvent(),
                                pos.GetPressStrength(),
                                pos.GetControllerPos().x,
                                pos.GetControllerPos().y,
                                pos.GetControllerPos().z,
                                pos.GetControllerRot().x,
                                pos.GetControllerRot().y,
                                pos.GetControllerRot().z,
                                h.GetTarget().x,
                                h.GetTarget().y,
                                h.GetTarget().z,
                                pos.GetPointerPos().x,
                                pos.GetPointerPos().y
                             ));
                        }
                    }
                }
            }
        }
    }

    private string BuildHexString()
    {
        string hexValue = string.Empty;
        int num;

        for (int i = 0; i < 8; i++)
        {
            num = Processing.rand.Next(0, int.MaxValue);
            hexValue += num.ToString("X8");
        }

        return hexValue;
    }
}
