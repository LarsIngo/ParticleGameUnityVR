using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInput : MonoBehaviour {

    public GameObject left;
    public GameObject right;

    static SteamVR_TrackedObject leftController;
    static SteamVR_TrackedObject rightController;
    public static bool controllersFound;

    // Use this for initialization
    void Start () {

        leftController = left.GetComponent<SteamVR_TrackedObject>();
        rightController = right.GetComponent<SteamVR_TrackedObject>();



        controllersFound = leftController && rightController;

    }

    public static float LeftTrigger()
    {

        if (!controllersFound)
            return -1;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)leftController.index);
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

    }

}
