using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour {

    private static SteamVR_TrackedController controller;
    public GameObject newController;
    public Vector3 position;
    private static SteamVR_Controller.Device device = null;
    private static string mode;

    private void OnEnable()
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

        if(position == null)
        {
            position = new Vector3(0, 0, 0);
        }

        mode = "full";

    }

    // Update is called once per frame
    void Update () {
        newController.transform.rotation = controller.transform.rotation;
        if(mode.Equals("full"))
        {
            newController.transform.position = controller.transform.position;
        }
        else
        {
            newController.transform.position = position;
        }
	}

    public static void changeMode(string m)
    {
        mode = m;
    }

    public static float getTriggerPressed()
    {
        if (device == null)
        {
            device = SteamVR_Controller.Input((int)controller.controllerIndex);
            Debug.Log("failed");
            return 0;
        }
        else
        {
            Debug.Log("Success");
            Vector3 vec = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
            Debug.Log(vec.ToString());
            return vec.x;
        }
    }
}
