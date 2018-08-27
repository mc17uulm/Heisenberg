using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Store {

    private Stack<Vector3> Elements;
	
    public Store() {
        this.Elements = new Stack<Vector3>();
    }

    public void Add(Vector3 vector) {
        this.Elements.Push(vector);
    }

}
