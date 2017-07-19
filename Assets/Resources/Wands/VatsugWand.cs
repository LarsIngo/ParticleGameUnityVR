using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VatsugWand : MonoBehaviour
{

    GameObject mWandGO;

    GameObject mRodGO;

    GameObject mTipGO;

    GameObject[] mAttractors;
    private const uint mNrOfAttractors = 5;

    private GPUParticleAttractor mEndAttractor = null;
    private float mPowerEndAttractor;
    private const float mTimerEndAttractor = 0.75f;
    private float mTimerCurrentEndAttractor = mTimerEndAttractor;

    private GPUParticleSystem mParticles = null;

    public bool rightHand;
    public float mAttractorsPower;

    // Use this for initialization
    void Awake()
    {
        mPowerEndAttractor = 50.0f;
        mAttractorsPower = 20;

        //We create the various parts.
        InitGameObjects();

        //We add the emitter to the tip.
        mParticles = mTipGO.AddComponent<GPUParticleSystem>();
        mParticles.EmittMesh = mTipGO.GetComponent<MeshFilter>().mesh;
        mParticles.EmittParticleLifeTime = 5.0f;
        mParticles.EmittFrequency = 500.0f;
        mParticles.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        mParticles.EmittInheritVelocity = false;

        Vector4[] colorControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(1, 1, 0, 0.5f), new Vector4(0, 1, 0, 1.0f) };
        mParticles.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(1, 1, 0, 0), new Vector4(1, 1, 0, 1.0f) };
        mParticles.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.05f, 0.05f, 0, 0), new Vector4(0.01f, 0.01f, 0, 1) };
        mParticles.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 0.8f), new Vector4(0.0f, 0, 0, 1.0f) };
        mParticles.TransparencyLifetimePoints = transparencyControlpoints;

        mEndAttractor = mTipGO.AddComponent<GPUParticleAttractor>();
        mEndAttractor.transform.localPosition = Vector3.up * 1.5f;

        mAttractors = new GameObject[mNrOfAttractors];//GPUParticleAttractor[mNrOfAttractors];

        float TwoPIdivNrAttractors = Mathf.PI * 2 / mNrOfAttractors;
        for (int i = 0; i < 5; ++i)
        {
            //mAttractors[i] = mTipGO.AddComponent<GPUParticleAttractor>();
            //mAttractors[i].Power = 1;
            TempVisuals(mAttractors[i], PrimitiveType.Cube, Color.blue);
            mAttractors[i].transform.parent = mTipGO.transform;
            mAttractors[i].transform.localPosition = new Vector3(Mathf.Cos(TwoPIdivNrAttractors * i), 0.0f, Mathf.Sin(TwoPIdivNrAttractors * i)).normalized * 6;
            
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
                //for (int i = 0; i < mNrOfAttractors; ++i)
                //  mAttractors[i].Power = power * trigger;

                mParticles.Active = false;
                mTimerCurrentEndAttractor = mTimerEndAttractor;

                mEndAttractor.Power = 0.0f;
            }
            else
            {
                //for (int i = 0; i < mNrOfAttractors; ++i)
                //  mAttractors[i].Power = 0.0f;

                mParticles.Active = false;
                if (mTimerCurrentEndAttractor > 0.0f)
                {
                    mTimerCurrentEndAttractor -= Time.deltaTime;
                    mEndAttractor.Power = mPowerEndAttractor;

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
