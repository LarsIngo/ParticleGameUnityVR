using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorWand : MonoBehaviour {

    GameObject mWandGO;

    GameObject mRodGO;
    Mesh mRodMesh;

    GameObject mTipGO;
    Mesh mTipMesh;
    GPUParticleAttractor attractor;
    GPUParticleSystem system;

    string mRodMaterialPath;
    string mTipMaterialPath;

    public bool rightHand;
    public float power;

    // Use this for initialization
    void Start () {

        rightHand = true;
        power = 20;

        //We create the various parts.
        InitGameObjects();

        //We add the emitter to the tip.
        system = mTipGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mTipGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 5.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialAmbient = new Vector3(1.0f, 1.0f, 1.0f);

        Vector4[] colorControlpoints = { new Vector4(0, 1, 0, 0), new Vector4(1, 1, 0, 0.1f), new Vector4(0, 1, 0, 0.2f), new Vector4(1, 0, 0, 0.3f), new Vector4(0, 1, 0, 0.4f),
            new Vector4(0, 0, 1, 0.5f), new Vector4(1, 0, 1, 0.6f), new Vector4(0, 1, 1, 0.7f), new Vector4(0, 1, 0, 0.8f), new Vector4(1, 1, 1, 0.9f), new Vector4(1, 1, 0, 1) };

        system.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0.333f), new Vector4(0, 0, 1, 0.666f), new Vector4(0.5f, 0, 0.5f, 1) };
        system.HaloLifetimePoints = haloControlpoints;
        /*
        Vector4[] scaleControlpoints = { new Vector4(0.1f, 0.1f, 0, 0), new Vector4(0.1f, 0.1f, 0, 0.5f), new Vector4(0.1f, 0, 0.1f, 1) };
        system.ScaleLifetimePoints = scaleControlpoints;*/

        Vector4[] transparencyControlpoints = { new Vector4(0.0f, 0, 0, 0), new Vector4(0.0f, 0, 0, 0.5f), new Vector4(1.0f, 0, 0, 1.0f) };
        system.TransparencyLifetimePoints = transparencyControlpoints;

        system.EmittInheritVelocity = true;

        //We add an attractor to the tip.
        attractor = mTipGO.AddComponent<GPUParticleAttractor>();
        attractor.Power = 1;

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
    void Update () {

        if (VrInput.controllersFound)
        {

            float trigger = 0;
            if(rightHand)
                trigger = VrInput.RightTrigger();
            else trigger = VrInput.RightTrigger();

            attractor.Power = power * trigger;

            if(trigger == 1.0f)
                system.Active = true;
            else system.Active = false;

        }
        else
        {

            if (Input.GetKey(KeyCode.Space))
            {

                //attractor.Power = 20;
                system.Active = false;

            }
            else
            {

                //attractor.Power = 0;
                system.Active = true;

            }

        }


    }
}
