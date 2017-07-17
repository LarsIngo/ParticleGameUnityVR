using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInput : MonoBehaviour {

    public GameObject left;
    public GameObject right;

    static SteamVR_TrackedObject leftController;
    static SteamVR_TrackedObject rightController;
    public static bool controllersFound { get { return leftController.isActiveAndEnabled && rightController.isActiveAndEnabled; } }

    // Use this for initialization
    void Start () {

        leftController = left.GetComponent<SteamVR_TrackedObject>();
        rightController = right.GetComponent<SteamVR_TrackedObject>();

    }

    public static float LeftTrigger()
    {

        if (!leftController.isActiveAndEnabled)
            return -1;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)leftController.index);
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

    }
    public static float RightTrigger()
    {

        if (!rightController.isActiveAndEnabled)
            return -1;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)rightController.index);
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

    }
    public static bool LeftGrip()
    {

        if (!leftController.isActiveAndEnabled)
            return false;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)leftController.index);
        return controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip);

    }
    public static bool RightGrip()
    {

        if (!rightController.isActiveAndEnabled)
            return false;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)rightController.index);
        return controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip);

    }


}
