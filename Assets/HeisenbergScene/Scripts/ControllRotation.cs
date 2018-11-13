using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class ControllRotation : MonoBehaviour {

    public Vector3 position = new Vector3(0, 1.75f, 0);
    private SteamVR_TrackedController trackedController;

    private void OnEnable()
    {
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        if (trackedController == null)
        {
            Debug.Log("TrackedContoroller still null");
            Application.Quit();
        }

    }

    void Update()
    {
        Debug.Log(trackedController.transform.position);
    }

    private void LateUpdate()
    {
        Vector3 pos = trackedController.transform.localPosition;
        trackedController.transform.position = position - pos;
    }
}
