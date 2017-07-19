using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugWand : MonoBehaviour
{

    GameObject mWandGO;

    GameObject mRodGO;

    GameObject mTipGO;

    private const uint mNrOfAttractors = 5;

    GameObject[] mAttractors;

    GameObject mEndAttractor;
    private float mPowerEndAttractor;
    private const float mTimerEndAttractor = 0.1f;
    private float mTimerCurrentEndAttractor = mTimerEndAttractor;

    private GPUParticleSystem mParticles = null;

    public bool rightHand;
    public float mAttractorsPower;

    // Use this for initialization
    void Awake()
    {
        mPowerEndAttractor = 30.0f;
        mAttractorsPower = 7.0f;

        //We create the various parts.
        InitGameObjects();

        //We add the emitter to the tip.
        mParticles = mTipGO.AddComponent<GPUParticleSystem>();
        mParticles.EmittMesh = mTipGO.GetComponent<MeshFilter>().mesh;
        mParticles.EmittParticleLifeTime = 5.0f;
        mParticles.EmittFrequency = 500.0f;
        mParticles.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        mParticles.EmittInheritVelocity = false;

        mParticles.Active = false;

        Vector4[] colorControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(1, 1, 0, 0.5f), new Vector4(0, 1, 0, 1.0f) };
        mParticles.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(1, 1, 0, 0), new Vector4(1, 1, 0, 1.0f) };
        mParticles.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.05f, 0.05f, 0, 0), new Vector4(0.01f, 0.01f, 0, 1) };
        mParticles.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 0.8f), new Vector4(0.0f, 0, 0, 1.0f) };
        mParticles.TransparencyLifetimePoints = transparencyControlpoints;

        mAttractors = new GameObject[mNrOfAttractors];

        mEndAttractor = new GameObject();
        mEndAttractor.AddComponent<GPUParticleAttractor>();
        mEndAttractor.transform.parent = mTipGO.transform;
        mEndAttractor.transform.localPosition = Vector3.up * 10f;

       
        float TwoPIdivNrAttractors = Mathf.PI * 2 / mNrOfAttractors;
        for (int i = 0; i < 5; ++i)
        {
            mAttractors[i] = new GameObject("attractor" + i.ToString());
            mAttractors[i].transform.parent = mTipGO.transform;
            mAttractors[i].transform.localPosition = new Vector3(Mathf.Cos(TwoPIdivNrAttractors * i), 0.0f, Mathf.Sin(TwoPIdivNrAttractors * i)).normalized * 5 * Mathf.Sin(Time.time);

            mAttractors[i].AddComponent<GPUParticleAttractor>().Power = mAttractorsPower;
        }

        mWandGO.transform.Rotate(90, 0, 0);
        mWandGO.transform.position += Vector3.forward * 0.2f;

    }

    void InitGameObjects()
    {

        //The wand is the parent object to all the parts.
        mWandGO = new GameObject("Wand");
        mWandGO.transform.parent = gameObject.transform;

        //The rod
        //We set its transform.
        mRodGO = new GameObject("Rod");
        mRodGO.transform.parent = mWandGO.transform;
        mRodGO.transform.localScale += Vector3.up * 8;
        mRodGO.transform.localScale *= 0.2f;
        TempVisuals(mRodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        mTipGO = new GameObject("Tip");
        mTipGO.transform.parent = mWandGO.transform;
        mTipGO.transform.position += Vector3.up * 2;
        mTipGO.transform.localScale *= 0.5f;
        TempVisuals(mTipGO, PrimitiveType.Sphere, Color.red);

        mWandGO.transform.localScale *= 0.1f;

    }

    void TempVisuals(GameObject target, PrimitiveType primitive, Color color)
    {

        GameObject tmp = GameObject.CreatePrimitive(primitive);
        MeshRenderer renderer = target.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = color;
        renderer.material = mat;
        MeshFilter filter = target.AddComponent<MeshFilter>();
        filter.mesh = tmp.GetComponent<MeshFilter>().mesh;
        Destroy(tmp);

    }

    // Update is called once per frame
    void Update()
    {
        if (VrInput.controllersFound)
        {

            float trigger = 0;
            if (rightHand)
                trigger = VrInput.RightTrigger();
            else trigger = VrInput.LeftTrigger();

            

            if (trigger == 1.0f)
            {
                for (int i = 0; i < mNrOfAttractors; ++i)
                  mAttractors[i].GetComponent<GPUParticleAttractor>().Power = mAttractorsPower;

                mParticles.Active = true;
                mTimerCurrentEndAttractor = mTimerEndAttractor;

                mEndAttractor.GetComponent<GPUParticleAttractor>().Power = 0.0f;
            }
            else
            {
                for (int i = 0; i < mNrOfAttractors; ++i)
                  mAttractors[i].GetComponent<GPUParticleAttractor>().Power = 0.0f;

                mParticles.Active = false;
                if (mTimerCurrentEndAttractor > 0.0f)
                {
                    mTimerCurrentEndAttractor -= Time.deltaTime;
                    mEndAttractor.GetComponent<GPUParticleAttractor>().Power = mPowerEndAttractor;

                }
                else
                {
                    mEndAttractor.GetComponent<GPUParticleAttractor>().Power = 0.0f;
                }
            }
        }
        else
        {
            /*
            if (Input.GetKey(KeyCode.Space))
            {

                attractor.Power = 20;
                system.Active = false;

            }
            else
            {

                attractor.Power = 0;
                system.Active = true;

            }
            */
        }

    }
}
