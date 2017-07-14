using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Main : MonoBehaviour
{

    private void Start ()
    {
        GameObject go = new GameObject();

        GPUParticleSystem system = go.AddComponent<GPUParticleSystem>();

        system.SetParticles(new GPUParticleSystem.GPUParticle[] { new GPUParticleSystem.GPUParticle(0,0,5) });
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {

    }

}
