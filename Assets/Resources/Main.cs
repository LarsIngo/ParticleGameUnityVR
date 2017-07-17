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

        GPUParticleVectorField leftVectorField = mLeftController.AddComponent<GPUParticleVectorField>();
        leftVectorField.Radius = 2.0f;
        leftVectorField.Vector = Vector3.up * 20.0f;
        leftVectorField.RelativeVectorField = true;

        GPUParticleAttractor rightAttractor = mRightController.AddComponent<GPUParticleAttractor>();
        rightAttractor.Power = 100;

        mParticleSystem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mParticleSystem.name = "Emitter";
        mParticleSystem.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mParticleSystem.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mParticleSystem.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 5.0f;
        system.EmittFrequency = 2.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialScale = new Vector2(0.1f, 0.1f);
        system.EmittInitialAmbient = new Vector3(0.5f, 0.0f, 0.5f);

        Vector4 [] lerpetylerplerplerp = { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0.333f), new Vector4(0, 0, 1, 0.666f), new Vector4(0.5f, 0, 0.5f, 1) };
        system.HaloLifetimePoints = lerpetylerplerplerp;

        system.EmittInheritVelocity = true;

    }

    private void Update()
    {
        SteamVR_TrackedController left = mLeftController.GetComponent<SteamVR_TrackedController>();
        SteamVR_TrackedController right = mRightController.GetComponent<SteamVR_TrackedController>();

        if (left != null)
        {
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)left.controllerIndex);

            left.GetComponent<GPUParticleVectorField>().Vector = Vector3.up * 20.0f * (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ? 1 : 0); 
        }
        if(right != null)
        {
            SteamVR_Controller.Device device = SteamVR_Controller.Input((int)right.controllerIndex);

            right.GetComponent<GPUParticleAttractor>().Power = 100.0f * (device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ? 1 : 0);
        }
    }

    private void OnDestroy()
    {

    }

}
