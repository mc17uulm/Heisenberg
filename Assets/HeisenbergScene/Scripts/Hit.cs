using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit {

    private int index;
    private Vector3 target;
    private List<Vector3> positions;

    public Hit(int index, Vector3 target, List<Vector3> positions)
    {
        this.index = index;
        this.target = target;
        this.positions = positions;
    }

    public int GetIndex()
    {
        return this.index;
    }

    public Vector3 GetTarget()
    {
        return this.target;
    }

    public List<Vector3> GetPositions()
    {
        return this.positions;
    }

    public Vector3 GetLastPosition()
    {
        return this.positions[this.positions.Count - 1];
    }

    public Vector3 GetAverage()
    {
        int volume = this.positions.Count;

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        for (int i = 0; i < volume; i++)
        {
            Vector3 t = this.positions[this.positions.Count - 1];
            x += t.x;
            y += t.y;
            z += t.z;
        }

        return new Vector3(x / volume, y / volume, z / volume);
    }
	
}
