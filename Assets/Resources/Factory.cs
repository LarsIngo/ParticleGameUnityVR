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
