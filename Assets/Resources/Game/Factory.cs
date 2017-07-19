using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class contatning functions to create constructed game objects.
/// </summary>
public static class Factory
{

    /// +++ FUNCTIONS +++ ///

    public static GameObject CreateMichaelBayEffect(Level level, Mesh mesh, Transform t, Color meshColor)
    {
        GameObject michael = level.CreateGameObject("michael" + Time.time);
        GameObject blackHole = level.CreateGameObject("blackhole" + Time.time);
        michael.transform.position = t.position;
        michael.transform.rotation = t.rotation;
        blackHole.transform.position = t.position;
        blackHole.transform.rotation = t.rotation;

        GeometryExplosion exp = michael.AddComponent<GeometryExplosion>();
        exp.Mesh = mesh;
        exp.ExplosionColor = meshColor;
        exp.ExplosionSpeed = 5;

        LifeTimer michaelLifetimer = michael.AddComponent<LifeTimer>();
        michaelLifetimer.LifeTime = 4.0f;

        GPUParticleAttractor attractor = blackHole.AddComponent<GPUParticleAttractor>();
        attractor.Power = 10.0f;

        LifeTimer blackHoleLifetimer = blackHole.AddComponent<LifeTimer>();
        blackHoleLifetimer.LifeTime = 4.0f;

        exp.Explode();

        return michael;
    }

    /// --- FUNCTIONS --- ///
}
