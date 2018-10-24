using System.Collections;
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
        this.name = "Max Mustermann";
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
                w.WriteLine("name;try;hit;first;target;position.x;position.y;position.z");
                foreach(Try tr in this.tries)
                {
                    foreach (Hit h in tr.GetHits())
                    {
                        foreach (Vector3 pos in h.GetPositions()) {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};", this.name, tr.GetIndex(), h.GetIndex(), h.GetFirst(), h.GetTarget().ToString(), pos.x, pos.y, pos.z));
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
                        foreach (Vector3 pos in h.GetPositions())
                        {
                            w.WriteLine(string.Format("{0};{1};{2};{3};{4};{5};{6};", this.name, tr.GetIndex(), h.GetIndex(), h.GetFirst(), h.GetTarget().ToString(), pos.x, pos.y, pos.z));
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
