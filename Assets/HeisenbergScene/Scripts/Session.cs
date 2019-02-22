using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Session
{

    private List<Try> tries;
    private List<Task> Tasks;
    private string SaveFile;
    private string SumFile;
    private string TroughputFile;

    public Session(List<Task> Tasks)
    {
        this.Tasks = Tasks;
        this.tries = new List<Try>();
        this.SaveFile = Path.Combine(Application.streamingAssetsPath, "data/FullData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
        this.SumFile = Path.Combine(Application.streamingAssetsPath, "data/SumData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
        this.TroughputFile = Path.Combine(Application.streamingAssetsPath, "data/TroughputData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
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

    public void Save()
    {
        SaveToFile(SaveFile);
        SaveSum(SumFile);
        SaveTroughput(TroughputFile);
        Config.SaveToConfig(SaveFile, SumFile, TroughputFile);
    }

    public void SaveToFile(string file)
    {
        if (!File.Exists(file))
        {
            using (StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("UserId;TaskNo;CircleNo;TargetNo;ArmPos;BodyPos;DOF;ballistic;timestamp;event;targetDistance;targetWidth;targetID;TriggerValue;ControllerPos.X;ControllerPos.Y;ControllerPos.Z;ControllerRot.X;ControllerRot.Y;ControllerRot.Z;TargetPos.X;TargetPos.Y;TargetPos.Z;PointerPos.X;PointerPos.Y");
            }
        }

        using (StreamWriter w = File.AppendText(file))
        { 

            foreach (Task task in this.Tasks)
            {
                foreach (Circle circle in task.GetCircles())
                {
                    foreach (Target target in circle.GetTargets())
                    {
                        foreach (EventLog log in target.GetEvents())
                        {
                            Vector3 controllerPos = log.GetControllerPos();
                            Vector3 controllerRot = log.GetControllerRot();
                            Vector3 pointerPos = log.GetPointerPos();
                            Vector3 targetPos = target.GetPosition();
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};{23};{24}",
                                Config.UserId,
                                task.GetId(),
                                circle.GetId(),
                                target.GetId(),
                                task.PrintArmPosition(),
                                task.PrintBodyPosition(),
                                task.PrintDOF(),
                                log.GetBallistic(),
                                log.GetTimestamp(),
                                log.PrintType(),
                                circle.GetAmplitude(),
                                circle.GetSize(),
                                target.GetId(),
                                log.GetPressedValue(),
                                controllerPos.x,
                                controllerPos.y,
                                controllerPos.z,
                                controllerRot.x,
                                controllerRot.y,
                                controllerRot.z,
                                targetPos.x,
                                targetPos.y,
                                targetPos.z,
                                pointerPos.x,
                                pointerPos.y
                            ));
                        }
                    }
                }
            }
        }
    }

    public void SaveSum(string file)
    {
        if (!File.Exists(file))
        {
            using (StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("id;ArmPos;BodyPos;DOF;ballistic;targetDistance,targetWidth;targetID;target.x;target.y;pressed.x;pressed.y;click.x;click.y;difference.x;difference.y");
            }
        }

        using (StreamWriter w = File.AppendText(file))
        {
            foreach (Task task in this.Tasks)
            {
                foreach (Circle circle in task.GetCircles())
                {
                    foreach (Target target in circle.GetTargets())
                    {
                        List<EventLog> Pressed = target.GetPressedPosition();
                        List<EventLog> Clicked = target.GetClickedPosition();
                        Vector3 Position = target.GetPosition();
                        for (int i = 0; i < Clicked.Count; i++)
                        { 
                            EventLog press = Pressed[i];
                            EventLog click = Clicked[i];    
                            Vector3 p = press.GetPointerPos();
                            Vector3 c = click.GetPointerPos();
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15}",
                                Config.UserId,
                                task.PrintArmPosition(),
                                task.PrintBodyPosition(),
                                task.PrintDOF(),
                                press.GetBallistic(),
                                circle.GetAmplitude(),
                                circle.GetSize(),
                                target.GetId(),
                                Position.x,
                                Position.y,
                                p.x,
                                p.y,
                                c.x,
                                c.y,
                                p.x - c.x,
                                p.y - c.y
                             ));
                        }
                    }
                }
            }
        }
    }

    public void SaveTroughput(string file)
    {
        if (!File.Exists(file))
        {
            using (StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("UserId;ArmPos;BodyPos;DOF;targetID;targetDistance;targetWidth;MeanMT;TroughputRegular;EffectiveWidth;EffectiveDistance;EffectiveID;EffectiveTroughput");
            }
        }

        using (StreamWriter w = File.AppendText(file))
        {

            foreach (Task task in this.Tasks)
            {
                foreach (Circle circle in task.GetCircles())
                {
                    w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5}",
                        Config.UserId,
                        task.PrintArmPosition(),
                        task.PrintBodyPosition(),
                        task.PrintDOF(),
                        circle.GetId(),
                        circle.CalculateTroughput()
                    ));
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
            //num = Processing.rand.Next(0, int.MaxValue);
            num = 1;
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
