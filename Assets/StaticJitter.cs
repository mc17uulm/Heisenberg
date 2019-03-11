using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class StaticJitter : MonoBehaviour
{

    private static SteamVR_TrackedController controller;
    private static List<string> Stack;
    private static bool Start = false;
    private static long EndTime = -2;
    private static string SaveFile;

    // Start is called before the first frame update
    void OnEnable()
    {
        controller = GetComponent<SteamVR_TrackedController>();

        if (controller == null)
        {
            controller = GetComponentInParent<SteamVR_TrackedController>();
        }
        if (controller == null)
        {
            Debug.Log("TrackedContoroller still null");
            Application.Quit();
        }

        SaveFile = Path.Combine(Application.streamingAssetsPath, "Jitter_Log_"  + System.DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv");

        Stack = new List<string>();
        Stack.Add("Position.x;Position.y;Position.z;Rotation.x;Rotation.y;Rotation.z");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Start = true;
            Debug.Log("Start");
            EndTime = GetNow() + (10000);
        }

        if(Start)
        {
            if(GetNow() < EndTime)
            {
                Vector3 pos = controller.transform.position;
                Vector3 rot = controller.transform.rotation.eulerAngles;

                string line = string.Format("{0};{1};{2};{3};{4};{5}", pos.x, pos.y, pos.z, rot.x, rot.y, rot.z);
                Debug.Log(line);
                Stack.Add(line);
            }
            else
            {
                if (!File.Exists(SaveFile))
                {
                    using (StreamWriter w = File.CreateText(SaveFile))
                    {
                        foreach(string line in Stack)
                        {
                            w.WriteLine(line);
                        }
                    }
                }
                Start = false;
                Debug.Log("Finished");
                Application.Quit();
            }
            
        }
    }

    private long GetNow()
    {
        return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
    }
}
