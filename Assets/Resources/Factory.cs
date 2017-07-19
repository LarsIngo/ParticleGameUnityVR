using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Factory {

    public static GameObject CreateMichaelBayEffect(Mesh mesh, Transform t, Color meshColor)
    {
        GameObject michael = new GameObject();
        GameObject blackHole = new GameObject();
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

    public static GameObject CreateStageScreen(Stage stage)
    {

        GameObject stageScreen = new GameObject("Stage_" + stage.name + "Screen");
        stageScreen.AddComponent<StageScreen>();

        return stageScreen;

    }

}
