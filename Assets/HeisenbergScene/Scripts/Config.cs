using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config {

    public static Vector3 Start = new Vector3(0, 0, 0);
    public static Vector3[] Positions = new Vector3[] {
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
    public static string Name = "Marco";
    public static int Id = 1;
    public static int Repeat = 1;
    public static int Tries = 8;
    public static bool Random = true;
    public static int Dimension = 15;
    public static int Distance = 8;
    public static bool LastPosition = false;
    public static string SaveFile = @"C:\Users\Lizenznehmer\Desktop\Heisenberg\savefile.csv";
    public static string SaveFileTwo = @"C:\Users\Lizenznehmer\Desktop\Heisenberg\savefile_2.csv";
    public static LatinSquare LatinSquare = new LatinSquare(8);

  
}
