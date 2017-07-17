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
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialAmbient = new Vector3(1.0f, 1.0f, 1.0f);

        Vector4[] colorControlpoints = { new Vector4(0, 1, 0, 0), new Vector4(1, 1, 0, 0.1f), new Vector4(0, 1, 0, 0.2f), new Vector4(1, 0, 0, 0.3f), new Vector4(0, 1, 0, 0.4f),
            new Vector4(0, 0, 1, 0.5f), new Vector4(1, 0, 1, 0.6f), new Vector4(0, 1, 1, 0.7f), new Vector4(0, 1, 0, 0.8f), new Vector4(1, 1, 1, 0.9f), new Vector4(1, 1, 0, 1) };

        system.ColorLifetimePoints = colorControlpoints;

        Vector4 [] haloControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0.333f), new Vector4(0, 0, 1, 0.666f), new Vector4(0.5f, 0, 0.5f, 1) };
        system.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.1f, 0.1f, 0, 0), new Vector4(0.3f, 0.3f, 0, 0.5f), new Vector4(0.1f, 0, 0.1f, 1) };
        system.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(0.0f, 0, 0, 0), new Vector4(0.0f, 0, 0, 0.5f), new Vector4(1.0f, 0, 0, 1.0f) };
        system.TransparencyLifetimePoints = transparencyControlpoints;

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
