using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mSystemGO;
    GameObject mAttractorGO;

    private void Start ()
    {
        mSystemGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mSystemGO.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mSystemGO.AddComponent<GPUParticleSystem>();
        //system.EmittMesh = mSystemGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 30.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialScale = new Vector2(0.1f, 0.1f);
        system.EmittInitialColor = new Vector3(0.0f, 1.0f, 0.0f);

        mAttractorGO = new GameObject();
        mAttractorGO.AddComponent<GPUParticleAttractor>();
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
