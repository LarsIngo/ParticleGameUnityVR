using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mGO;
    public GameObject rightHand;
    public GameObject leftHand;

    private void Start ()
    {
        mGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mGO.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 3.0f;
        system.EmittFrequency = 350.0f;

        if (rightHand)
        {

            mGO.transform.position = rightHand.transform.position;
            mGO.transform.parent = rightHand.transform;

        }


    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
