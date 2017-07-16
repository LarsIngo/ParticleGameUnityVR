using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mGO;
    public GameObject attractor;
    public GameObject emitter;

    private void Start ()
    {
        mGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mGO.transform.position = new Vector3(0,0,0);
        mGO.transform.localScale *= 0.1f;
        GPUParticleSystem system = mGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = mGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 30.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInitialScale = new Vector2(0.01f, 0.01f);
        system.EmittInitialColor = new Vector3(0.0f, 1.0f, 0.0f);
        system.TMPAcceleratorPower = 10f;

        mGO.transform.parent = emitter.transform;

    }

    private void Update()
    {
        mGO.GetComponent<GPUParticleSystem>().TMPAcceleratorPosition = attractor.transform.position;
    }

    private void OnDestroy()
    {

    }

}
