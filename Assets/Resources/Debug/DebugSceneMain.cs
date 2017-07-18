using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSceneMain : MonoBehaviour
{

    GameObject emitter;

    void Start ()
    {
        emitter = new GameObject("emitter");
        emitter.transform.position = new Vector3(0,0,5);
        GPUParticleSystem system = emitter.AddComponent<GPUParticleSystem>();
        system.EmittFrequency = 10.0f;
        system.EmittParticleLifeTime = 5.0f;

        GameObject attractor = new GameObject("attractor");
        attractor.transform.position = new Vector3(0, 0, 0);
        GPUParticleAttractor a = attractor.AddComponent<GPUParticleAttractor>();

        GameObject vf = new GameObject("vector field");
        vf.transform.position = new Vector3(5, 0, 5);
        GPUParticleVectorField b = vf.AddComponent<GPUParticleVectorField>();

        GameObject collider0 = new GameObject("collider0");
        collider0.transform.position = new Vector3(5, 0, 0);
        collider0.AddComponent<GPUParticleSphereCollider>();

        GameObject collider1 = new GameObject("collider1");
        collider1.transform.position = new Vector3(-5, 0, 0);
        collider1.AddComponent<GPUParticleSphereCollider>();

    }
	
	void Update ()
    {
        if (Time.time > 5.0f)
            emitter.GetComponent<GPUParticleSystem>().Active = false;

    }

}
