using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    GameObject mGO;

    private void Start ()
    {
        mGO = new GameObject();
        mGO.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = mGO.AddComponent<GPUParticleSystem>();
        //system.SetEmittFrequency(120.0f);

        //system.SetParticles(new GPUParticleSystem.GPUParticle[] { new GPUParticleSystem.GPUParticle(0,0,5) });
    }

    private void Update()
    {
        //GetComponent<GPUParticleSystem>().EmittParticle();
    }

    private void OnDestroy()
    {

    }

}
