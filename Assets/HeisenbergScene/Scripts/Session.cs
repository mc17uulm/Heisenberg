﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Session
{

    private List<Try> tries;
    private string SaveFile;
    private string SumFile;

    public Session()
    {
        this.tries = new List<Try>();
        this.SaveFile = Path.Combine(Application.streamingAssetsPath, "data/FullData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
        this.SumFile = Path.Combine(Application.streamingAssetsPath, "data/SumData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
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

    public void Save(Transform trans)
    {
        this.SaveToFile(SaveFile);
        this.SaveSum(SumFile, trans);
        Config.SaveToConfig(SaveFile, SumFile);
    }

    public void SaveToFile(string file)
    {
        if(!File.Exists(file))
        {
            using(StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("name;round;trial;ArmPos;BodyPos;DOF;ballistic;timestamp;event;TriggerValue;ControllerPos.X;ControllerPos.Y;ControllerPos.Z;ControllerRot.X;ControllerRot.Y;ControllerRot.Z;TargetID;TargetPos.X;TargetPos.Y;TargetPos.Z;PointerPos.X;PointerPos.Y");
                foreach(Try tr in this.tries)
                {
                    bool[] states = tr.GetStates();
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Position pos in h.GetPositions()) {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};",
                                Config.UserId,
                                tr.GetIndex(),
                                h.GetIndex(),
                                states[1] ? "ausgestreckt" : "angelegt",
                                states[0] ? "sitzend" : "stehend",
                                states[2] ? "6" : "3",
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
                                pos.GetTargetId(),
                                pos.GetTargetPos().x,
                                pos.GetTargetPos().y,
                                pos.GetTargetPos().z,
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
                    bool[] states = tr.GetStates();
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Position pos in h.GetPositions())
                        {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};",
                                Config.UserId,
                                tr.GetIndex(),
                                h.GetIndex(),
                                states[1] ? "ausgestreckt" : "angelegt",
                                states[0] ? "sitzend" : "stehend",
                                states[2] ? "6" : "3",
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
                                pos.GetTargetId(),
                                pos.GetTargetPos().x,
                                pos.GetTargetPos().y,
                                pos.GetTargetPos().z,
                                pos.GetPointerPos().x,
                                pos.GetPointerPos().y
                             ));
                        }
                    }
                }
            }
        }
    }

    public void SaveSum(string file, Transform trans)
    {
        if(!File.Exists(file))
        {
            using (StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("name;round;trial;ArmPos;BodyPos;DOF;ballistic;targetID;target.x;target.y;pressed.x;pressed.y;click.x;click.y;difference.x;difference.y;");
                foreach (Try tr in this.tries)
                {
                    bool[] states = tr.GetStates();
                    foreach (Hit h in tr.GetHits())
                    {
                        Vector3 pressed = new Vector3(0,0,0);
                        Vector3 clicked = new Vector3(0,0,0);
                        List<Vector3> p = new List<Vector3>();
                        int targetId = h.GetPositions()[0].GetTargetId();
                        Vector3 target = h.GetPositions()[0].GetTargetPos();
                        foreach (Position pos in h.GetPositions())
                        {
                            switch (pos.GetEvent())
                            {

                                case PointerEvent.TriggerPressedFirst:
                                    pressed = pos.GetPointerPos();
                                    break;

                                case PointerEvent.ClickedFirst:
                                    clicked = pos.GetPointerPos();
                                    break;

                                default:
                                    break;

                            }

                            // true if not ballistic
                            if (!h.GetFirst())
                            {
                                p.Add(pos.GetPointerPos());
                            }

                        }

                        if(p.Count > 0)
                        {
                            target = GetAverage(p);
                        }

                        w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};",
                            Config.UserId,
                            tr.GetIndex(),
                            h.GetIndex(),
                            states[1] ? "ausgestreckt" : "angelegt",
                            states[0] ? "sitzend" : "stehend",
                            states[2] ? "6" : "3",
                            h.GetFirst(),
                            targetId,
                            target.x,
                            target.y,
                            pressed.x,
                            pressed.y,
                            clicked.x,
                            clicked.y,
                            pressed.x - clicked.x,
                            pressed.y - clicked.y
                         ));
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
                    bool[] states = tr.GetStates();
                    foreach (Hit h in tr.GetHits())
                    {
                        Vector3 pressed = new Vector3(0, 0, 0);
                        Vector3 clicked = new Vector3(0, 0, 0);
                        List<Vector3> p = new List<Vector3>();
                        int targetId = h.GetPositions()[0].GetTargetId();
                        Vector3 target = h.GetPositions()[0].GetTargetPos();
                        foreach (Position pos in h.GetPositions())
                        {
                            switch (pos.GetEvent())
                            {

                                case PointerEvent.TriggerPressedFirst:
                                    pressed = pos.GetPointerPos();
                                    break;

                                case PointerEvent.ClickedFirst:
                                    clicked = pos.GetPointerPos();
                                    break;

                                default:
                                    break;

                            }

                            // true if not ballistic
                            if (!h.GetFirst())
                            {
                                p.Add(pos.GetPointerPos());
                            }

                        }

                        if (p.Count > 0)
                        {
                            target = GetAverage(p);
                        }

                        w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};",
                            Config.UserId,
                            tr.GetIndex(),
                            h.GetIndex(),
                            states[1] ? "ausgestreckt" : "angelegt",
                            states[0] ? "sitzend" : "stehend",
                            states[2] ? "6" : "3",
                            h.GetFirst(),
                            targetId,
                            target.x,
                            target.y,
                            pressed.x,
                            pressed.y,
                            clicked.x,
                            clicked.y,
                            pressed.x - clicked.x,
                            pressed.y - clicked.y
                         ));
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

    public Vector3 GetAverage(List<Vector3> stack)
    {
        int volume = stack.Count;

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        for (int i = 0; i < volume; i++)
        {
            Vector3 t = stack[i];
            x += t.x;
            y += t.y;
            z += t.z;
        }

        Vector3 tmp = new Vector3(x / volume, y / volume, z / volume);

        return tmp;
    }
}
