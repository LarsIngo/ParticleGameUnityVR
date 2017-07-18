using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Factory {

    public static GameObject CreateMichaelBayEffect(Mesh mesh, Transform t)
    {
        GameObject michael = new GameObject();
        michael.transform.position = t.position;
        michael.transform.rotation = t.rotation;

        GeometryExplosion exp = michael.AddComponent<GeometryExplosion>();
        exp.Mesh = mesh;

        GPUParticleAttractor attractor = michael.AddComponent<GPUParticleAttractor>();
        attractor.Power = 10.0f;

        LifeTimer lifetimer = michael.AddComponent<LifeTimer>();
        lifetimer.LifeTime = 4.0f;


        exp.Explode();

        return michael;
    }

}
