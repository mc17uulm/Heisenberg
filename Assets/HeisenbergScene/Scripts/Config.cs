using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config{

    public Vector3 Start { get; }
    public Vector3[] Positions { get; }
    public string Name { get; }
    public int Id { get; }
    public int Repeat { get; }
    public int Tries { get; }
    public bool Random { get; }
    public int Dimension { get; }
    public int Distance { get; }
    public bool LastPosition { get; }
    public string SaveFile { get; }
    public string SaveFileTwo { get; }
    public LatinSquare LatinSquare { get; }

    public Config()
    {
        // Name of user
        this.Name = "Marco";
        this.Id = 1;

        this.Start = new Vector3(0, 0, 0);
        this.Positions = new Vector3[]
        {
            new Vector3(-280,180,0),
            new Vector3(-93.5f,180,0),
            new Vector3(93.5f,180,0),
            /**
            new Vector3(280,180,0),

            new Vector3(-280,0,0),
            new Vector3(-93.5f,0,0),
            new Vector3(93.5f,0,0),
            new Vector3(280,0,0),

            new Vector3(-280,-180,0),
            new Vector3(-93.5f,-180,0),
            new Vector3(93.5f,-180,0),
            new Vector3(280,-180,0)*/
        };

        // How many times is a position repeated
        this.Repeat = 1;
        // Amount of tries;
        this.Tries = 8;
        // Show the positions in random order?
        this.Random = true;
        // Size of the target in px (width = height)
        this.Dimension = 15;
        // Distance between camera and target (z)
        this.Distance = 8;
        // Show last position or average 
        this.LastPosition = false;

        // Savefiles
        this.SaveFile = @"C:\Users\mi-vr\Desktop\Heisenberg\savefile.csv";
        this.SaveFileTwo = @"C:\Users\mi-vr\Desktop\Heisenberg\savefile_2.csv";

        this.LatinSquare = new LatinSquare(8);
    }
  
}
