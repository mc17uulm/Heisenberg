using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataSet {

    private int num;
    private Stack<Vector3> stack;

	public DataSet(int num, Stack<Vector3> stack)
    {
        this.num = num;
        this.stack = stack;
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

    public Stack<Vector3> GetStack()
    {
        return this.stack;
    }
    
    public Vector3 GetLastPosition() {
        return this.stack.Peek();
    }

    public Vector3 GetAverage(int volume) {

        volume = volume > this.stack.Count ? this.stack.Count : volume;

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        for(int i = 0; i < volume; i++) {
            Vector3 t = this.stack.Peek();
            x += t.x;
            y += t.y;
            z += t.z;
        }

        return new Vector3(x / volume, y / volume, z / volume);

    }
}
