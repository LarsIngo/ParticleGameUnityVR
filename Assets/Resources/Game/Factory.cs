using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class contatning functions to create constructed game objects.
/// </summary>
public static class Factory
{

    /// --- MEMBERS --- ///

    /// <summary>
    /// The amount of items created by the factory.
    /// </summary>
    private static int count = 0;

    /// +++ MEMBERS +++ ///

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



        exp.Explode();

        count++;

        return michael;
    }

    public static GameObject CreateStageScreen(Level level, StageInfo stageInfo)
    {

        GameObject screen = level.CreateGameObject(stageInfo.name + "_SCREEN");
        MeshFilter meshFilter = screen.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = screen.AddComponent<MeshRenderer>();

        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshFilter.mesh = tmp.GetComponent<MeshFilter>().mesh;
        Object.Destroy(tmp);

        screen.AddComponent<MeshCollider>();

        Material mat = new Material(Shader.Find("Unlit/Texture"));
        mat.mainTexture = Resources.Load(stageInfo.thumbnail) as Texture2D;
        meshRenderer.material = mat;

        screen.AddComponent<StageScreen>().stageInfo = stageInfo;

        count++;

        return screen;

    }

    public static GameObject CreateAttractorWand(Level level, float power, bool rightHand)
    {

        //The wand is the parent object to all the parts.
        GameObject WandGO = level.CreateGameObject("AttractorWand" + count);

        //The rod
        //We set its transform.
        GameObject RodGO = level.CreateGameObject("Rod" + count);
        RodGO.transform.parent = WandGO.transform;
        RodGO.transform.localScale += Vector3.up * 8;
        RodGO.transform.localScale *= 0.2f;
        RodGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(RodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        GameObject TipGO = new GameObject("Tip" + count);
        TipGO.transform.parent = WandGO.transform;
        TipGO.transform.position += Vector3.up * 2;
        TipGO.transform.localScale *= 0.5f;
        TipGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(TipGO, PrimitiveType.Sphere, Color.red);

        WandGO.transform.localScale *= 0.1f;

        //We add the emitter to the tip.
        GPUParticleSystem system = TipGO.AddComponent<GPUParticleSystem>();
        system.EmittMesh = TipGO.GetComponent<MeshFilter>().mesh;
        system.EmittParticleLifeTime = 5.0f;
        system.EmittFrequency = 500.0f;
        system.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        system.EmittInheritVelocity = false;

        Vector4[] colorControlpoints = { new Vector4(0, 1, 0, 0), new Vector4(1, 1, 0, 0.1f), new Vector4(0, 1, 0, 0.2f), new Vector4(1, 0, 0, 0.3f), new Vector4(0, 1, 0, 0.4f),
            new Vector4(0, 0, 1, 0.5f), new Vector4(1, 0, 1, 0.6f), new Vector4(0, 1, 1, 0.7f), new Vector4(0, 1, 0, 0.8f), new Vector4(1, 1, 1, 0.9f), new Vector4(1, 1, 0, 1) };

        system.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0.333f), new Vector4(0, 0, 1, 0.666f), new Vector4(0.5f, 0, 0.5f, 1) };
        system.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.01f, 0.01f, 0, 0), new Vector4(0.01f, 0.01f, 0, 1) };
        system.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 0.8f), new Vector4(0.0f, 0, 0, 1.0f) };
        system.TransparencyLifetimePoints = transparencyControlpoints;

        //We add an attractor to the tip.
        GPUParticleAttractor attractor = TipGO.AddComponent<GPUParticleAttractor>();
        attractor.Power = 1;

        WandGO.transform.Rotate(90, 0, 0);

        AttractorWand wand = WandGO.AddComponent<AttractorWand>();
        wand.power = power;
        wand.rightHand = rightHand;

        wand.system = system;
        wand.attractor = attractor;

        count++;

        return WandGO;

    }

    public static GameObject CreateMenuWand(Level level, bool rightHand)
    {

        //The wand is the parent object to all the parts.
        GameObject WandGO = level.CreateGameObject("MenuWand" + count);

        //The rod
        //We set its transform.
        GameObject RodGO = level.CreateGameObject("Rod" + count);
        RodGO.transform.parent = WandGO.transform;
        RodGO.transform.localScale += Vector3.up * 8;
        RodGO.transform.localScale *= 0.2f;
        RodGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(RodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        GameObject TipGO = new GameObject("Tip" + count);
        TipGO.transform.parent = WandGO.transform;
        TipGO.transform.position += Vector3.up * 2;
        TipGO.transform.localScale *= 0.5f;
        TipGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(TipGO, PrimitiveType.Sphere, Color.red);

        LineRenderer lineRenderer = TipGO.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = 2;
        WandGO.transform.localScale *= 0.1f;

        WandGO.transform.Rotate(90, 0, 0);


        MenuWand wand = WandGO.AddComponent<MenuWand>();
        wand.lineRenderer = lineRenderer;
        wand.rightHand = rightHand;

        count++;

        return WandGO;

    }


    public static GameObject CreateVatsugWand(Level level, float powerEndAttractor, float powerNormalAttractors, bool rightHand)
    {
        //The wand is the parent object to all the parts.
        GameObject WandGO = level.CreateGameObject("VatsugWand" + count);

        //The rod
        //We set its transform.
        GameObject RodGO = level.CreateGameObject("Rod" + count);
        RodGO.transform.parent = WandGO.transform;
        RodGO.transform.localScale += Vector3.up * 8;
        RodGO.transform.localScale *= 0.2f;
        RodGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(RodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        GameObject TipGO = new GameObject("Tip" + count);
        TipGO.transform.parent = WandGO.transform;
        TipGO.transform.position += Vector3.up * 2;
        TipGO.transform.localScale *= 0.5f;
        TipGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(TipGO, PrimitiveType.Sphere, Color.red);

        WandGO.transform.localScale *= 0.1f;

        //++++++++++ WAND ++++++++++
        const uint nrOfAttractors = 3;

        GameObject[] attractors;

        GameObject endAttractor;

        //We add the emitter to the tip.
        GPUParticleSystem particleEmitter = TipGO.AddComponent<GPUParticleSystem>();
        particleEmitter.EmittMesh = TipGO.GetComponent<MeshFilter>().mesh;
        particleEmitter.EmittParticleLifeTime = 3.0f;
        particleEmitter.EmittFrequency = 750.0f;
        particleEmitter.EmittInitialVelocity = new Vector3(0.0f, 0.0f, 0.0f);
        particleEmitter.EmittInheritVelocity = false;

        particleEmitter.Active = false;

        Vector4[] colorControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(1, 1, 0, 0.3f), new Vector4(0, 1, 0, 1.0f) };
        particleEmitter.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(1, 1, 0, 0), new Vector4(1, 1, 0, 1.0f) };
        particleEmitter.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.04f, 0.04f, 0, 0), new Vector4(0.01f, 0.01f, 0, 0.02f), new Vector4(0.01f, 0.01f, 0, 1) };
        particleEmitter.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 0.8f), new Vector4(0.0f, 0, 0, 1.0f) };
        particleEmitter.TransparencyLifetimePoints = transparencyControlpoints;

        attractors = new GameObject[nrOfAttractors];

        endAttractor = new GameObject();
        endAttractor.AddComponent<GPUParticleAttractor>();
        endAttractor.transform.parent = TipGO.transform;
        endAttractor.transform.localPosition = Vector3.up * 12.0f;

        //the distance to wich the normal attractors can reach when periodically sinusweaving their way to victory...
        float normalAttractorReboundDistance = 5.0f;

        float TwoPIdivNrAttractors = Mathf.PI * 2 / nrOfAttractors;
        for (int i = 0; i < nrOfAttractors; ++i)
        {
            attractors[i] = new GameObject("attractorVatsug" + i.ToString());
            attractors[i].transform.parent = TipGO.transform;
            attractors[i].transform.localPosition = new Vector3(Mathf.Cos(TwoPIdivNrAttractors * i), 0.0f, Mathf.Sin(TwoPIdivNrAttractors * i)).normalized * normalAttractorReboundDistance;

            attractors[i].AddComponent<GPUParticleAttractor>().Power = powerNormalAttractors;
        }

        WandGO.transform.Rotate(90, 0, 0);
        WandGO.transform.position += Vector3.forward * 0.2f;


        VatsugWand wand = TipGO.AddComponent<VatsugWand>();
        wand.mAttractors = attractors;
        wand.mEndAttractor = endAttractor;
        wand.mNrOfAttractors = nrOfAttractors;
        wand.mPowerEndAttractor = powerEndAttractor;
        wand.mPowerAttractors = powerNormalAttractors;
        wand.mNormalAttractorReboundDistance = normalAttractorReboundDistance;
        wand.mParticles = particleEmitter;
        wand.rightHand = rightHand;

        count++;

        return WandGO;
    }


    private static void TempVisuals(GameObject target, PrimitiveType primitive, Color color)
    {

        GameObject tmp = GameObject.CreatePrimitive(primitive);
        MeshRenderer renderer = target.AddComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Unlit/Color"));
        mat.color = color;
        renderer.material = mat;
        MeshFilter filter = target.AddComponent<MeshFilter>();
        filter.mesh = tmp.GetComponent<MeshFilter>().mesh;
        Object.Destroy(tmp);

    }

    /// --- FUNCTIONS --- ///
}
