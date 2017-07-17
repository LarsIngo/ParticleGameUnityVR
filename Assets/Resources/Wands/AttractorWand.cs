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

    // Use this for initialization
    void Start () {

        //We create the various parts.
        InitGameObjects();

        //We add the emitter to the tip.
        system = mTipGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mTipGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 10.0f;
        system.EmittFrequency = 1500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialScale = new Vector2(0.01f, 0.01f);
        system.EmittInitialColor = new Vector3(0.0f, 1.0f, 0.0f);
        system.EmittInheritVelocity = false;

        //We add an attractor to the tip.
        attractor = mTipGO.AddComponent<GPUParticleAttractor>();
        attractor.Power = 0;

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

            if (VrInput.rightController.triggerPressed)
            {

                attractor.Power = 20;
                system.EmittFrequency = 1500f;

            }
            else
            {

                attractor.Power = 0;
                system.EmittFrequency = 0;

            }

        }
        else
        {

            if (Input.GetKey(KeyCode.Space))
            {

                attractor.Power = 20;
                system.Active = true;

            }
            else
            {

                attractor.Power = 0;
                system.Active = false;

            }

        }

		
	}
}
