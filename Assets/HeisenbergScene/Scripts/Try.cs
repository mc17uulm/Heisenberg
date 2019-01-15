using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Try {

    private int index;
    private List<Hit> hits;
    private bool[] states;

    public Try(int index, bool[] states)
    {

        this.index = index;
        this.hits = new List<Hit>();
        this.states = states;
    }

    public void AddHit(Hit hit)
    {
        this.hits.Add(hit);
    }

    public int GetIndex()
    {
        return this.index;
    }

    public List<Hit> GetHits()
    {
        return this.hits;
    }

    public bool[] GetStates()
    {
        return this.states;
    }
	
}
