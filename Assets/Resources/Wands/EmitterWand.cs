using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterWand : MonoBehaviour {

    GameObject mEmitDish;

    GPUParticleSystem system;

    public bool rightHand;
    public float power;

    // Use this for initialization
    void Awake()
    {

        if(power == 0)
            power = 500;

        //We create the various parts.
        InitGameObjects();

        //We add the emitter to the tip.
        system = mEmitDish.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mEmitDish.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 5.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialAmbient = new Vector3(1.0f, 1.0f, 1.0f);
        system.EmittInheritVelocity = false;

        Vector4[] colorControlpoints = { new Vector4(0, 1, 0, 0), new Vector4(1, 1, 0, 0.1f), new Vector4(0, 1, 0, 0.2f), new Vector4(1, 0, 0, 0.3f), new Vector4(0, 1, 0, 0.4f),
            new Vector4(0, 0, 1, 0.5f), new Vector4(1, 0, 1, 0.6f), new Vector4(0, 1, 1, 0.7f), new Vector4(0, 1, 0, 0.8f), new Vector4(1, 1, 1, 0.9f), new Vector4(1, 1, 0, 1) };

        system.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0.333f), new Vector4(0, 0, 1, 0.666f), new Vector4(0.5f, 0, 0.5f, 1) };
        system.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.01f, 0.01f, 0, 0), new Vector4(0.01f, 0.01f, 0, 1) };
        system.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 0.8f), new Vector4(0.0f, 0, 0, 1.0f) };
        system.TransparencyLifetimePoints = transparencyControlpoints;

    }

    void InitGameObjects()
    {

        mEmitDish = new GameObject("EmitDish");
        mEmitDish.transform.parent = transform;
        mEmitDish.transform.localScale -= Vector3.up * 0.9f;
        TempVisuals(mEmitDish, PrimitiveType.Sphere, Color.black);

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

            if(trigger < 0.1f)
                system.Active = true;
            else system.Active = false;

            system.EmittFrequency = 1 + trigger * power;

        }
        else
        {

            system.Active = true;

        }

    }
}
