using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataSet : MonoBehaviour{

    private int num;
    private List<Vector3> stack;
    private GameObject point;

	public DataSet(int num, List<Vector3> stack, GameObject point)
    {
        this.num = num;
        this.stack = stack;
        this.point = Instantiate(point, point.transform.position, Quaternion.identity) as GameObject;
        this.point.SetActive(false);
    }

    public override string ToString()
    {
        return "";
       // return string.Format("Versuch {0}:\nTarget: x={1} | y={2} | z={3}\nActual: x={4} | y={5} | z={6}\nDistance: {7}",
         //   this.num, this.selected.x, this.selected.y, this.selected.z, this.clicked.x, this.clicked.y, this.clicked.z, this.distance);
    }

    
    public int GetNum()
    {
        return this.num;
    }

    public List<Vector3> GetStack()
    {
        return this.stack;
    }

    public GameObject GetPoint()
    {
        return this.point;
    }

    public Vector3 GetPosition(Mode mode)
    {
        switch (mode)
        {
            case Mode.AVG:
                return this.GetAverage();

            case Mode.LASTPOS:
                return this.GetLastPosition();

            default:
                return new Vector3();
        }
    }
    
    public Vector3 GetLastPosition() {
        return this.stack[this.stack.Count-1];
    }

    public Vector3 GetAverage() {

        //volume = volume > this.stack.Count ? this.stack.Count : volume;
        int volume = this.stack.Count;

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        for(int i = 0; i < volume; i++) {
            Vector3 t = this.stack[this.stack.Count - 1];
            x += t.x;
            y += t.y;
            z += t.z;
        }

        return new Vector3(x / volume, y / volume, z / volume);

    }
}
