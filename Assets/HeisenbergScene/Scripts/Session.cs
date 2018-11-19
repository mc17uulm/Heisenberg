using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Session
{

    private string name;
    private string type;
    private List<Try> tries;

    public Session()
    {
        this.name = "Marco";
        this.type = "Angelegt";
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
                w.WriteLine("name;position;round;trial;ballistic;timestamp;event;TriggerValue;ControllerPos.X;ControllerPos.Y;ControllerPos.Z;ControllerRot.X;ControllerRot.Y;ControllerRot.Z;TargetID;TargetPos.X;TargetPos.Y;TargetPos.Z;PointerPos.X;PointerPos.Y");
                foreach(Try tr in this.tries)
                {
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Position pos in h.GetPositions()) {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};",
                                this.name,
                                this.type,
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
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Position pos in h.GetPositions())
                        {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};",
                                this.name,
                                this.type,
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
                w.WriteLine("name;round;trial;ballistic;targetID;target.x;target.y;pressed.x;pressed.y;click.x;click.y;difference.x;difference.y;");
                foreach (Try tr in this.tries)
                {
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

                        w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};",
                            this.name,
                            tr.GetIndex(),
                            h.GetIndex(),
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

                        w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};",
                            this.name,
                            tr.GetIndex(),
                            h.GetIndex(),
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
