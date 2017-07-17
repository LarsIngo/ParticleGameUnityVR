using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrInput : MonoBehaviour {

    public GameObject left;
    public GameObject right;

    public static SteamVR_TrackedController leftController;
    public static SteamVR_TrackedController rightController;
    public static bool controllersFound;
    // Use this for initialization
    void Start () {
        leftController = left.GetComponent<SteamVR_TrackedController>();
        rightController = right.GetComponent<SteamVR_TrackedController>();

        controllersFound = leftController && rightController;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
