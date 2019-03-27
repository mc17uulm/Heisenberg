using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Session
{
    
    private static string SaveFile;
    private static string SumFile;
    private static string TroughputFile;

    public static void Initalize()
    {
        SaveFile = Path.Combine(Application.streamingAssetsPath, "data/FullData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
        SumFile = Path.Combine(Application.streamingAssetsPath, "data/SumData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
        TroughputFile = Path.Combine(Application.streamingAssetsPath, "data/TroughputData_" + Config.UserId + "_" + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");
    }

    public static void Save(List<Task> Tasks)
    {
        SaveToFile(Tasks);
        SaveSum(Tasks);
        SaveTroughput(Tasks);
        Config.SaveToConfig(SaveFile, SumFile, TroughputFile);
    }

    public static void SaveToFile(List<Task> Tasks)
    {
        if (!File.Exists(SaveFile))
        {
            using (StreamWriter w = File.CreateText(SaveFile))
            {
                w.WriteLine("UserId;ConditionHash;TaskNo;CircleNo;TargetNo;ArmPos;BodyPos;DOF;ballistic;timestamp;event;state;targetDistanceLocal;targetWidthLocal;targetDistance;targetWidth;TriggerValue;ControllerPos.X;ControllerPos.Y;ControllerPos.Z;ControllerRot.X;ControllerRot.Y;ControllerRot.Z;TargetPos.X;TargetPos.Y;TargetPos.Z;PointerPos.X;PointerPos.Y");
            }
        }

        using (StreamWriter w = File.AppendText(SaveFile))
        { 

            foreach (Task task in Tasks)
            {
                foreach (Circle circle in task.GetCircles())
                {
                    foreach (Target target in circle.GetTargets())
                    {
                        string hash = ComputeHash(string.Format("{0}{1}{2}{3}{4}{5}", task.GetId(), circle.GetId(), target.GetId(), task.PrintArmPosition(), task.PrintBodyPosition(), task.PrintDOF()));
                        foreach (EventLog log in target.GetEvents())
                        {
                            Vector3 controllerPos = log.GetControllerPos();
                            Vector3 controllerRot = log.GetControllerRot();
                            Vector3 pointerPos = log.GetPointerPos();
                            Vector3 targetPos = target.GetWorldPosition();
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};{23};{24};{25};{26};{27}",
                                Config.UserId,
                                hash,
                                task.GetId(),
                                circle.GetId(),
                                target.GetId(),
                                task.PrintArmPosition(),
                                task.PrintBodyPosition(),
                                task.PrintDOF(),
                                log.GetBallistic(),
                                log.GetTimestamp(),
                                log.PrintType(),
                                log.PrintState(),
                                circle.GetAmplitude(),
                                circle.GetSize(),
                                circle.GetDistance(),
                                circle.GetWidth(),
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

    public static void SaveSum(List<Task> Tasks)
    {
        if (!File.Exists(SumFile))
        {
            using (StreamWriter w = File.CreateText(SumFile))
            {
                w.WriteLine("id;ConditionHash;TaskNo;CircleNo;TargetNo;ArmPos;BodyPos;DOF;ballistic;targetDistanceLocal;targetWidthLocal;targetDistance;targetWidth;targetID;target.x;target.y;pressed.x;pressed.y;click.x;click.y;difference.x;difference.y");
            }
        }

        using (StreamWriter w = File.AppendText(SumFile))
        {
            foreach (Task task in Tasks)
            {
                foreach (Circle circle in task.GetCircles())
                {
                    foreach (Target target in circle.GetTargets())
                    {
                        string hash = ComputeHash(string.Format("{0}{1}{2}{3}{4}{5}", task.GetId(), circle.GetId(), target.GetId(), task.PrintArmPosition(), task.PrintBodyPosition(), task.PrintDOF()));
                        List<Vector3 []> Sum = target.GetSum();
                        Vector3 Position = target.GetWorldPosition();
                        for (int i = 0; i < Sum.Count; i++)
                        {
                            bool ballistic = i == 0;
                            Vector3[] vecs = Sum[i];
                            Vector3 p = vecs[0];
                            Vector3 c = vecs[1];
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21}",
                                Config.UserId,
                                hash,
                                task.GetId(),
                                circle.GetId(),
                                target.GetId(),
                                task.PrintArmPosition(),
                                task.PrintBodyPosition(),
                                task.PrintDOF(),
                                ballistic,
                                circle.GetAmplitude(),
                                circle.GetSize(),
                                circle.GetDistance(),
                                circle.GetWidth(),
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

    public static string ComputeHash(string input) {
        // Adler32 hashing algo
        const int mod = 65521;
        uint a = 1, b = 0;
        foreach (char c in input) {
            a = (a + c) % mod;
            b = (b + a) % mod;
        }
        return "" + ((b << 16) | a);
    }

    public static void SaveTroughput(List<Task> Tasks)
    {
        if (!File.Exists(TroughputFile))
        {
            using (StreamWriter w = File.CreateText(TroughputFile))
            {
                w.WriteLine("UserId;ArmPos;BodyPos;DOF;circleID;targetDistanceLocal;targetWidthLocal;targetDistance;targetWidth;MeanMT;TroughputRegular;EffectiveWidth;EffectiveDistance;EffectiveID;EffectiveTroughput;SumOfDeviations");
            }
        }

        using (StreamWriter w = File.AppendText(TroughputFile))
        {

            foreach (Task task in Tasks)
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
