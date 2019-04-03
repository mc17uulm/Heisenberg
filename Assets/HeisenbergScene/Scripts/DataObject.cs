using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataObject
{
    public int id;
    public string date;
    public List<string> files;

    public DataObject (int id, string date, List<string> files)
    {
        this.id = id;
        this.date = date;
        this.files = files;
    }
}
