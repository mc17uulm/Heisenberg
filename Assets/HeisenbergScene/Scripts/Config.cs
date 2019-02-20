using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Config {

    private static string ConfigFile = Path.Combine(Application.streamingAssetsPath, "config.json");
    private static ConfigObject config;
    private static DataObject active = null;

    public static void init()
    {
        /**
        if(File.Exists(ConfigFile))
        {
            string data = File.ReadAllText(ConfigFile);
            ConfigObject obj = JsonUtility.FromJson<ConfigObject>(data);
            config = obj;

            Debug = obj.debug;
            Timespan = obj.timespan;

            bool start = false;
            foreach(DataObject d in obj.data)
            {
                if(d.state.Equals("added"))
                {
                    start = true;
                    active = d;
                }
            }

            UnityEngine.Debug.Log("Start: " + start);
            start = true;

            if(!start)
            {
                UnityEngine.Debug.Log("No active DataObject");
                Application.Quit();
            }
            else
            {
                UserId = active.id;
                SaveFiles = active.files;
            }

        } else
        {
            UnityEngine.Debug.Log("Config File not found");
            Application.Quit();
        }*/
    }

    public static void SaveToConfig(string saveFile, string sumFile, string TroughputFile)
    {
        List<string> files = new List<string>()
        {
            saveFile,
            sumFile,
            TroughputFile
        };

        foreach(DataObject d in config.data)
        {
            if(d.state.Equals("added"))
            {
                d.files = files;
                d.state = "finished";
            }
        }

        string json = JsonUtility.ToJson(config);
        File.WriteAllText(ConfigFile, json);

    }
    
    public static int UserId = 1;

    // Adds debug information to screen
    public static bool Debug = false;

    // User has to be Timespan ms in target to successfully click
    public static int Timespan = 500;

    public static bool Pad = false;

    public static int TargetId = 1;

    public static int[] TargetAmplitudes = new int[]
    {
        350, 150
    };

    /**public static int[] TargetWidths = new int[]
    {
        15, 30, 50
    };*/

    public static int[] TargetWidths = new int[]
{
        15, 30, 50
};

    //public static int CircleSize = 13;
    public static int CircleSize = 1;

    // Record clickes based on PAD or TRIGGER
    //public static ClickMode clickMode = ClickMode.TRIGGER;
    //public static GridMode grid = GridMode.GRID;

    // Start position of 3DOF
    public static Vector3 Start = new Vector3(0, 40, 0);

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
    
    public static int Tries = 16;
    public static bool Random = true;

    // Size of target
    public static int Dimension = 15;

    // Distance from user to target
    public static int Distance = 8;

    public static List<string> SaveFiles = null;

    public static string SaveFile = @"C:\Users\Lizenznehmer\Desktop\Heisenberg\savefile.csv";
    public static string SaveFileTwo = @"C:\Users\Lizenznehmer\Desktop\Heisenberg\savefile_2.csv";
    public static LatinSquare LatinSquare = new LatinSquare(8);
  
}
