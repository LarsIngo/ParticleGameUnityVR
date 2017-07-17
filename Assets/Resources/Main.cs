using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mParticleSystem;

    GameObject mHMD;
    GameObject mLeftController;
    GameObject mRightController;

    private void Start ()
    {
        mHMD = GameObject.Find("Camera (head)");
        mLeftController = GameObject.Find("Controller (left)");
        mRightController = GameObject.Find("Controller (right)");

        if (mLeftController == null)
        {
            mLeftController = new GameObject("STATIC LEFT CONTROLLER");
            mLeftController.transform.position = new Vector3(-3,0,5);
        }
        if (mRightController == null)
        {
            mRightController = new GameObject("STATIC RIGHT CONTROLLER");
            mRightController.transform.position = new Vector3(3, 0, 5);
        }

        mRightController.AddComponent<AttractorWand>();

    }

    private void Update()
    {
        SteamVR_TrackedController left = mLeftController.GetComponent<SteamVR_TrackedController>();
        SteamVR_TrackedController right = mRightController.GetComponent<SteamVR_TrackedController>();

        if (left != null)
        {
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)left.controllerIndex);
            
            if (left.triggerPressed)
                left.GetComponent<GPUParticleVectorField>().Vector = Vector3.up * 20.0f;
            else left.GetComponent<GPUParticleVectorField>().Vector = Vector3.up * 0.0f;

        }
        if(right != null)
        {
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)right.controllerIndex);

            if(right.triggerPressed)
                right.GetComponent<GPUParticleAttractor>().Power = 20.0f;
            else right.GetComponent<GPUParticleAttractor>().Power = 0.0f;

        }
    }

    private void OnDestroy()
    {

    }

}
