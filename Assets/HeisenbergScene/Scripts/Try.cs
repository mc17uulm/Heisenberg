using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Try {

    private int index;
    private List<Hit> hits;

    public Try(int index)
    {
        this.index = index;
        this.hits = new List<Hit>();
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
	
}
