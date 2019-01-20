using UnityEngine;

public class Config {

    // Name & id of user
    public static string Name = "Marco";
    public static int Id = 1;

    // Adds debug information to screen
    public static bool Debug = true;

    // User has to be Timespan ms in target to successfully click
    public static int Timespan = 500;

    // Record clickes based on PAD or TRIGGER
    public static ClickMode clickMode = ClickMode.TRIGGER;
    public static GridMode grid = GridMode.GRID;

    // Start position of 3DOF
    public static Vector3 Start = new Vector3(0, 0, 0);

    // Positions of targets
    public static Vector3[] Positions = new Vector3[] {
        new Vector3(-280,180,0),
        new Vector3(-93.5f,180,0),
        new Vector3(93.5f,180,0),
        new Vector3(280,180,0),

            new Vector3(-280,0,0),
            new Vector3(-93.5f,0,0),
            new Vector3(93.5f,0,0),
            new Vector3(280,0,0),

            new Vector3(-280,-180,0),
            new Vector3(-93.5f,-180,0),
            new Vector3(93.5f,-180,0),
            new Vector3(280,-180,0)
    };

    public static Vector3[] Circle = new Vector3[]
    {
         new Vector3(-280,180,0)
    };
    
    public static int Tries = 8;
    public static bool Random = true;

    // Size of target
    public static int Dimension = 15;

    // Distance from user to target
    public static int Distance = 8;

    public static string SaveFile = @"C:\Users\Lizenznehmer\Desktop\Heisenberg\savefile.csv";
    public static string SaveFileTwo = @"C:\Users\Lizenznehmer\Desktop\Heisenberg\savefile_2.csv";
    public static LatinSquare LatinSquare = new LatinSquare(8);
  
}
