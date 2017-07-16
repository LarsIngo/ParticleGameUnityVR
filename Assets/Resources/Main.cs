using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mGO;

    private void Start ()
    {
        mGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mGO.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 30.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialScale = new Vector2(0.1f, 0.1f);
        system.EmittInitialColor = new Vector3(0.5f, 0.0f, 0.5f);
        system.EmittInitialHaloColor = new Vector3(0.15f, 0.0f, 0.15f);
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
