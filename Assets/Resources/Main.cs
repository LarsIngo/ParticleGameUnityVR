using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mGO;

    private void Start ()
    {
        mGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mGO.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mGO.GetComponent<MeshFilter>().mesh;
        //system.EmittParticleLifeTime = 1.0f;

    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
