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

    public static void Save()
    {
        SaveSum();
        SaveTroughput();
        Config.SaveToConfig(SaveFile, SumFile, TroughputFile);
    }

    public static void AddToFile(EventLog Log)
    {
        if (!File.Exists(SaveFile))
        {
            using (StreamWriter w = File.CreateText(SaveFile))
            {
                w.WriteLine("UserId;TaskNo;CircleNo;TargetNo;ArmPos;BodyPos;DOF;ballistic;timestamp;event;targetDistance;targetWidth;TriggerValue;ControllerPos.X;ControllerPos.Y;ControllerPos.Z;ControllerRot.X;ControllerRot.Y;ControllerRot.Z;TargetPos.X;TargetPos.Y;TargetPos.Z;PointerPos.X;PointerPos.Y");
            }
        }

        using (StreamWriter w = File.AppendText(SaveFile))
        {
                w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                    Config.UserId,
                    Log.PrintIds(),
                    Log.PrintTask(),
                    Log.GetBallistic(),
                    Log.GetTimestamp(),
                    Log.PrintType(),
                    Log.PrintCircle(),
                    Log.GetPressedValue(),
                    Log.PrintPositions()
                ));
        }
    }

    public static void SaveSum()
    {
        if (!File.Exists(SumFile))
        {
            using (StreamWriter w = File.CreateText(SumFile))
            {
                w.WriteLine("id;TaskNo;CircleNo;TargetNo;ArmPos;BodyPos;DOF;ballistic;targetDistance,targetWidth;target.x;target.y;target.z;pressed.x;pressed.y;click.x;click.y;difference.x;difference.y");
            }
        }

        using (StreamWriter w = File.AppendText(SumFile))
        {
            List<EventLog> Firsts = EventLogger.GetFirsts();
            for (int i = 0; i < Firsts.Count; i=i+2)
            {
                EventLog Press = Firsts[i];
                EventLog Click = Firsts[i + 1];
                if(!Press.GetType().Equals(EventLog.Type.TriggerPressedFirst) || !Click.GetType().Equals(EventLog.Type.ClickedFirst))
                {
                    Debug.Log("ERROR: Save Sum is not working");
                }
                else
                {
                    w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}",
                       Config.UserId,
                       Click.PrintIds(),
                       Click.PrintTask(),
                       Click.GetBallistic(),
                       Click.PrintCircle(),
                       Click.PrintTarget(),
                       Press.GetPointerPos().x,
                       Press.GetPointerPos().y,
                       Click.GetPointerPos().x, 
                       Click.GetPointerPos().y,
                       Press.GetPointerPos().x - Click.GetPointerPos().x,
                       Press.GetPointerPos().y - Click.GetPointerPos().y
                    ));
                }
            }
        }
    }

    public static void SaveTroughput()
    {
        if (!File.Exists(TroughputFile))
        {
            using (StreamWriter w = File.CreateText(TroughputFile))
            {
                w.WriteLine("UserId;ArmPos;BodyPos;DOF;targetID;targetDistance;targetWidth;MeanMT;TroughputRegular;EffectiveWidth;EffectiveDistance;EffectiveID;EffectiveTroughput");
            }
        }

        using (StreamWriter w = File.AppendText(TroughputFile))
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

    private static string BuildHexString()
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

    public static Vector3 GetAverage(List<Vector3> stack)
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
