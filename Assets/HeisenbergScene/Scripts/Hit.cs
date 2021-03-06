﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit {

    private int index;
    private Vector3 target;
    private List<Position> positions;
    private bool first;

    public Hit(int index, Vector3 target, List<Position> positions, bool first = false)
    {
        this.index = index;
        this.target = target;
        this.positions = positions;
        this.first = first;
    }

    public override string ToString()
    {
        return "First: " + this.first + " | LastPos: " + this.GetLastPosition();
    }

    public int GetIndex()
    {
        return this.index;
    }

    public Vector3 GetTarget()
    {
        return this.target;
    }

    public List<Position> GetPositions()
    {
        return this.positions;
    }

    public bool GetFirst()
    {
        return this.first;
    }

    public Vector3 GetLastPosition()
    {
        if(this.positions.Count == 0)
        {
            return this.target;
        }
        else
        {
            return this.positions[this.positions.Count - 1].GetPointerPos();
        }
    }

    public Vector3 GetAverage(bool only_none = false)
    {
        int volume = this.positions.Count;

        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;

        for (int i = 0; i < volume; i++)
        {
            if(only_none)
            {
                if(!this.positions[i].GetEvent().Equals(PointerEvent.None))
                {
                    continue;
                }
            }
            Vector3 t = this.positions[i].GetPointerPos();
            x += t.x;
            y += t.y;
            z += t.z;
        }
        
        return new Vector3(x / volume, y / volume, z / volume);
    }
	
}
