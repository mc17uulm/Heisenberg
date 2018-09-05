using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class Session
{

    private string id;
    private List<Try> tries;

    public Session()
    {
        this.id = string.Concat(Enumerable.Range(0, 12).Select(_ => Processing.rand.Next(16).ToString("X")));
        Debug.Log("Session id: " + this.id);
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

    public string GetId()
    {
        return this.id;
    }

    public void SaveToFile(string file)
    {
        if(!File.Exists(file))
        {
            using(StreamWriter w = File.CreateText(file))
            {
                w.WriteLine("id;target;hit;\r\n");
                foreach (Hit h in this.GetAllHits())
                {
                    w.WriteLine(string.Format("{0};{1};{2};\r\n", this.id, h.GetTarget(), h.GetPositions()));
                }
            }
        }
        else
        {
            using (StreamWriter w = File.AppendText(file))
            {
                foreach (Hit h in this.GetAllHits())
                {
                    w.WriteLine(string.Format("{0};{1};{2};\r\n", this.id, h.GetTarget(), h.GetPositions()));
                }
            }
        }
    }
}
