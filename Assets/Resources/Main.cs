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
        mParticleSystem.transform.position = new Vector3(0, 1, 0);
        mParticleSystem.transform.localScale *= 0.1f;
        GPUParticleSystem system = mParticleSystem.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mParticleSystem.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 30.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialScale = new Vector2(0.01f, 0.01f);
        system.EmittInitialColor = new Vector3(0.0f, 1.0f, 0.0f);
        system.EmittInheritVelocity = true;

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
