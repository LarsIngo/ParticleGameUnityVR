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

        LifeTimer michaelLifetimer = michael.AddComponent<LifeTimer>();
        michaelLifetimer.LifeTime = 1.2f;

        GPUParticleAttractor attractor = blackHole.AddComponent<GPUParticleAttractor>();
        attractor.Power = 10.0f;

        LifeTimer blackHoleLifetimer = blackHole.AddComponent<LifeTimer>();
        blackHoleLifetimer.LifeTime = 4.0f;

        GPUParticleSystem particles = blackHole.AddComponent<GPUParticleSystem>();
        particles.EmittParticleLifeTime = 7.0f;
        particles.EmittFrequency = 50.0f;

        Vector4[] colorLife = new Vector4[2];
        colorLife[0] = meshColor;
        colorLife[1] = new Vector4(1.0f, 1.0f, 1.0f, 0.0f);
        particles.ColorLifetimePoints = colorLife;

        Vector4[] scaleLife = new Vector4[2];
        scaleLife[0] = new Vector4(0.01f, 0.01f, 0.0f, 0.0f);
        scaleLife[1] = new Vector4(0.001f, 0.001f, 0.0f, 1.0f);
        particles.ScaleLifetimePoints = scaleLife;

        exp.Explode();

        return michael;
    }

    /// --- FUNCTIONS --- ///
}
