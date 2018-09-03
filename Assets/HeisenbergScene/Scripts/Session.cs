using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session
{

    private string id;
    private List<List<DataSet>> tries;

    public Session(string id)
    {
        this.id = id;
        this.tries = new List<List<DataSet>>();
    }

    public void AddTry(List<DataSet> t)
    {
        this.tries.Add(t);
    }

    public List<DataSet> GetTry(int index)
    {
        return this.tries[index];
    }

    public string GetId()
    {
        return this.id;
    }
}
