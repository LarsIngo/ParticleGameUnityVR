using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInput : MonoBehaviour {

    public GameObject left;
    public GameObject right;

    static public GameObject leftHand;
    static public GameObject rightHand;

    static SteamVR_TrackedObject leftController;
    static SteamVR_TrackedObject rightController;
    public static bool controllersFound { get { return leftController.isActiveAndEnabled && rightController.isActiveAndEnabled && leftController; } }

    // Use this for initialization
    void Start () {

        leftController = left.GetComponent<SteamVR_TrackedObject>();
        rightController = right.GetComponent<SteamVR_TrackedObject>();

        leftHand = left;
        rightHand = right;

    }

    public static float LeftTrigger()
    {

        if (!controllersFound)
            return -1;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)leftController.index);
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

    }
    public static float RightTrigger()
    {

        if (!controllersFound)
            return -1;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)rightController.index);
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x;

    }
    public static bool LeftGrip()
    {

        if (!controllersFound)
            return false;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)leftController.index);
        return controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip);

    }
    public static bool RightGrip()
    {

        if (!controllersFound)
            return false;

        SteamVR_Controller.Device controller = SteamVR_Controller.Input((int)rightController.index);
        return controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip);

    }


}
