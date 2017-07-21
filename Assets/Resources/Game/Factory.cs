﻿using System.Collections;
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

    public static void CreateMichaelBayEffect(Mesh mesh, Transform t, Color meshColor)
    {
        GameObject michael = new GameObject("michael" + count++);
        //GameObject blackHole = new GameObject("blackhole" + count++);
        michael.transform.position = t.position;
        michael.transform.rotation = t.rotation;
        //blackHole.transform.position = t.position;
        //blackHole.transform.rotation = t.rotation;

        if (mesh != null)
        {
            GeometryExplosion exp = michael.AddComponent<GeometryExplosion>();
            exp.Mesh = mesh;
            exp.ExplosionColor = meshColor;
            exp.ExplosionSpeed = 8;
            exp.ShrinkSpeed = 1.0f;
            exp.ShrinkTime = 0.25f;
        }

        TimerStretch timeStrech = michael.AddComponent<TimerStretch>();
        timeStrech.TimePrePhase = 0.5f;
        timeStrech.TimeMainPhase = 0.2f;
        timeStrech.TimePostPhase = 0.5f;
        timeStrech.TargetTimeScale = 0.1f;

        LifeTimer michaelLifetimer = michael.AddComponent<LifeTimer>();
        michaelLifetimer.LifeTime = 4.0f;

        //GPUParticleAttractor attractor = blackHole.AddComponent<GPUParticleAttractor>();
        //attractor.Power = 250.0f;
        //LifeTimer blackHoleLifetimer = blackHole.AddComponent<LifeTimer>();
        //blackHoleLifetimer.LifeTime = 0.2f;

        AudioSource ceramicSound = michael.AddComponent<AudioSource>();
        ceramicSound.clip = Resources.Load<AudioClip>("Samples/Explosion/Ceramic");
        ceramicSound.Play();

        AudioSource artillerySound = michael.AddComponent<AudioSource>();
        artillerySound.clip = Resources.Load<AudioClip>("Samples/Explosion/Artillery");
        artillerySound.time = 0.25f;
        artillerySound.Play();

    }

    public static GameObject CreateStageScreen(StageInfo stageInfo)
    {

        GameObject screen = CreateWorldImage(stageInfo.mThumbnail);
        screen.AddComponent<MeshCollider>();
        screen.AddComponent<GPUParticleAttractor>();
        screen.GetComponent<GPUParticleAttractor>().Max = 1;
        screen.GetComponent<GPUParticleAttractor>().Power = -0.1f;

        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Mesh planeMesh = tmp.GetComponent<MeshFilter>().mesh;
        Object.Destroy(tmp);

        if (stageInfo.mLocked || stageInfo.mStarRequirement > Hub.Instance.stars)
        {

            GameObject lockImage = CreateWorldImage("Textures/Locked", true);
            lockImage.transform.position -= Vector3.forward * 0.1f;
            lockImage.transform.SetParent(screen.transform);

            GameObject requirement = CreateWorldText(stageInfo.mStarRequirement.ToString(), Color.red * 0.9f);
            requirement.transform.position -= Vector3.forward * 0.15f;
            requirement.transform.localScale *= 2;
            requirement.transform.SetParent(screen.transform);

        }
        else
        {

            GameObject name = CreateWorldText(stageInfo.mName, Color.white);
            name.transform.position -= Vector3.up * 0.6f;
            name.transform.localScale *= 0.3f;
            name.transform.SetParent(screen.transform);

            GameObject stars = new GameObject("STARS" + count++);

            if (stageInfo.Score < stageInfo.mGold)
            {

                GameObject gold = CreateWorldImage("Textures/Star");
                gold.transform.position += Vector3.right * 1.1f;
                gold.transform.parent = stars.transform;

            }
            if (stageInfo.Score < stageInfo.mSilver)
            {

                GameObject silver = CreateWorldImage("Textures/Star");
                silver.transform.parent = stars.transform;

            }
            if (stageInfo.Score < stageInfo.mBronze)
            {

                GameObject bronze = CreateWorldImage("Textures/Star");
                bronze.transform.position -= Vector3.right * 1.1f;
                bronze.transform.parent = stars.transform;

            }

            stars.transform.position -= Vector3.up * 0.8f;
            stars.transform.localScale *= 0.2f;
            stars.transform.SetParent(screen.transform);

            screen.AddComponent<StageScreen>().stageInfo = stageInfo;

            count++;

        }

        return screen;

    }

    public static GameObject CreateWorldText(string text, Color color)
    {

        GameObject canvasGO = new GameObject("CANVAS" + count++);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.GetComponent<RectTransform>().position = Vector3.zero;
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000);
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1000);

        GameObject textGO = new GameObject("TEXT" + count++);
        textGO.transform.SetParent(canvasGO.transform);
        UnityEngine.UI.Text textUI = textGO.AddComponent<UnityEngine.UI.Text>();
        textUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 5000);
        textUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 5000);
        textUI.transform.localScale *= 0.004f;
        textUI.text = text;
        textUI.fontSize = 75;
        textUI.color = color;
        textUI.alignment = TextAnchor.MiddleCenter;
        textUI.font = Resources.Load<Font>("Fonts/unispace bd");

        return canvasGO;

    }

    public static GameObject CreateWorldImage(string image, bool transparent = false)
    {

        GameObject imageGO = new GameObject("image" + count++);
        MeshFilter meshFilter = imageGO.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = imageGO.AddComponent<MeshRenderer>();

        GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Quad);
        meshFilter.mesh = tmp.GetComponent<MeshFilter>().mesh;
        Object.Destroy(tmp);

        Material mat = new Material(Shader.Find("Unlit/Texture"));

        if(transparent)
            mat = new Material(Shader.Find("Unlit/Transparent"));

        mat.mainTexture = Resources.Load(image) as Texture2D;
        meshRenderer.material = mat;

        return imageGO;

    }

    public static GameObject CreateAttractorWand(float power, bool rightHand)
    {

        //The wand is the parent object to all the parts.
        GameObject WandGO = new GameObject("AttractorWand" + count++);

        //The rod
        //We set its transform.
        GameObject RodGO = new GameObject("Rod" + count++);
        RodGO.transform.parent = WandGO.transform;
        RodGO.transform.localScale += Vector3.up * 8;
        RodGO.transform.localScale *= 0.2f;
        RodGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(RodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        GameObject TipGO = new GameObject("Tip" + count++);
        TipGO.transform.parent = WandGO.transform;
        TipGO.transform.position += Vector3.up * 2;
        TipGO.transform.localScale *= 0.5f;
        TipGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(TipGO, PrimitiveType.Sphere, Color.red);

        WandGO.transform.localScale *= 0.1f;

        //We add the emitter to the tip.
        GPUParticleSystem system = TipGO.AddComponent<GPUParticleSystem>();

        GPUParticleDescriptor descriptor = new GPUParticleDescriptor();
        descriptor.EmittFrequency = 500.0f;
        descriptor.Lifetime = 5.0f;
        descriptor.InheritVelocity = false;

        GPUParticleDescriptor.LifetimePoints colorPoints = new GPUParticleDescriptor.LifetimePoints();
        colorPoints.Add(new Vector4(0, 1, 0, 0));
        colorPoints.Add(new Vector4(1, 1, 0, 0.1f));
        colorPoints.Add(new Vector4(0, 1, 0, 0.2f));
        colorPoints.Add(new Vector4(1, 0, 0, 0.3f));
        colorPoints.Add(new Vector4(0, 1, 0, 0.4f));
        colorPoints.Add(new Vector4(0, 0, 1, 0.5f));
        colorPoints.Add(new Vector4(1, 0, 1, 0.6f));
        colorPoints.Add(new Vector4(0, 1, 1, 0.7f));
        colorPoints.Add(new Vector4(0, 1, 0, 0.8f));
        colorPoints.Add(new Vector4(1, 1, 1, 0.9f));
        colorPoints.Add(new Vector4(1, 1, 0, 1));
        descriptor.ColorOverLifetime = colorPoints;

        GPUParticleDescriptor.LifetimePoints haloPoints = new GPUParticleDescriptor.LifetimePoints();
        haloPoints.Add(new Vector4(1, 0, 0, 0));
        haloPoints.Add(new Vector4(0, 1, 0, 0.333f));
        haloPoints.Add(new Vector4(0, 0, 1, 0.666f));
        haloPoints.Add(new Vector4(0.5f, 0, 0.5f, 1));
        descriptor.HaloOverLifetime = haloPoints;

        GPUParticleDescriptor.LifetimePoints scalePoints = new GPUParticleDescriptor.LifetimePoints();
        scalePoints.Add(new Vector4(0.01f, 0.01f, 0, 0));
        scalePoints.Add(new Vector4(0.01f, 0.01f, 0, 1));
        descriptor.ScaleOverLifetime = scalePoints;

        GPUParticleDescriptor.LifetimePoints opacityPoints = new GPUParticleDescriptor.LifetimePoints();
        opacityPoints.Add(new Vector4(1.0f, 0, 0, 0));
        opacityPoints.Add(new Vector4(1.0f, 0, 0, 0.8f));
        opacityPoints.Add(new Vector4(0.0f, 0, 0, 1.0f));
        descriptor.OpacityOverLifetime = opacityPoints;

        descriptor.EmittMesh = TipGO.GetComponent<MeshFilter>().mesh;

        system.ParticleDescriptor = descriptor;

        //We add an attractor to the tip.
        GPUParticleAttractor attractor = TipGO.AddComponent<GPUParticleAttractor>();
        attractor.Power = 1;

        //We add vector field to the tip.
        GPUParticleVectorField vectorField = TipGO.AddComponent<GPUParticleVectorField>();
        vectorField.Max = 0.3f;

        WandGO.transform.Rotate(90, 0, 0);

        AttractorWand wand = WandGO.AddComponent<AttractorWand>();
        wand.power = power;
        wand.rightHand = rightHand;

        wand.system = system;
        wand.attractor = attractor;
        wand.vectorField = vectorField;

        GameObject hand = new GameObject(rightHand ? "Right" : "Left" + "hand");

        WandGO.transform.parent = hand.transform;

        hand.AddComponent<MirrorHandMovement>().rightHand = rightHand;

        return hand;

    }

    public static GameObject CreateMenuWand(bool rightHand)
    {

        //The wand is the parent object to all the parts.
        GameObject WandGO = new GameObject("MenuWand" + count++);

        //The rod
        //We set its transform.
        GameObject RodGO = new GameObject("Rod" + count++);
        RodGO.transform.parent = WandGO.transform;
        RodGO.transform.localScale += Vector3.up * 8;
        RodGO.transform.localScale *= 0.2f;
        RodGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(RodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        GameObject TipGO = new GameObject("Tip" + count++);
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

        GameObject hand = new GameObject(rightHand ? "Right" : "Left" + "hand");

        WandGO.transform.parent = hand.transform;

        hand.AddComponent<MirrorHandMovement>().rightHand = rightHand;

        return hand;

    }

    public static GameObject CreateVatsugWand(Level level, float powerEndAttractor, float powerNormalAttractors, float pendulumSpeed, float reboundDistance, bool rightHand)
    {
        //The wand is the parent object to all the parts.
        GameObject WandGO = level.CreateGameObject("VatsugWand" + count++);

        //The rod
        //We set its transform.
        GameObject RodGO = level.CreateGameObject("Rod" + count++);
        RodGO.transform.parent = WandGO.transform;
        RodGO.transform.localScale += Vector3.up * 8;
        RodGO.transform.localScale *= 0.2f;
        RodGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(RodGO, PrimitiveType.Cylinder, Color.black);

        //The tip
        //We set its transform
        GameObject TipGO = level.CreateGameObject("Tip" + count++);
        TipGO.transform.parent = WandGO.transform;
        TipGO.transform.position += Vector3.up * 2;
        TipGO.transform.localScale *= 0.5f;
        TipGO.transform.position += Vector3.forward * 0.2f;
        TempVisuals(TipGO, PrimitiveType.Sphere, Color.red);

        WandGO.transform.localScale *= 0.1f;

        //++++++++++ WAND ++++++++++
        
        GameObject endAttractor;

        GameObject emitter = level.CreateGameObject("VatsugEmitter" + count++);


        //We add the emitter to the tip.
        GPUParticleSystem particleEmitter = emitter.AddComponent<GPUParticleSystem>();
        emitter.AddComponent<GPUParticleAttractor>().Power = 0.0f;
        TempVisuals(emitter, PrimitiveType.Sphere, Color.blue);
        emitter.GetComponent<Renderer>().enabled = false;
        emitter.transform.parent = TipGO.transform;
        emitter.transform.localScale = Vector3.one * 0.7f;
        emitter.transform.localPosition = new Vector3(1, 0, 0) * reboundDistance;
        

        GPUParticleDescriptor descriptor = new GPUParticleDescriptor();
        descriptor.EmittFrequency = 500.0f;
        descriptor.Lifetime = 5.0f;
        descriptor.InheritVelocity = false;

        GPUParticleDescriptor.LifetimePoints colorPoints = new GPUParticleDescriptor.LifetimePoints();
        colorPoints.Add(new Vector4(1, 0, 0, 0));
        colorPoints.Add(new Vector4(1, 1, 0, 0.3f));
        colorPoints.Add(new Vector4(0, 1, 0, 1.0f));
        descriptor.ColorOverLifetime = colorPoints;

        GPUParticleDescriptor.LifetimePoints haloPoints = new GPUParticleDescriptor.LifetimePoints();
        haloPoints.Add(new Vector4(0, 0, 1, 0));
        haloPoints.Add(new Vector4(1, 0, 1, 1.0f));
        descriptor.HaloOverLifetime = haloPoints;

        GPUParticleDescriptor.LifetimePoints scalePoints = new GPUParticleDescriptor.LifetimePoints();
        scalePoints.Add(new Vector4(0.01f, 0.01f, 0, 0));
        scalePoints.Add(new Vector4(0.01f, 0.01f, 0, 1));
        descriptor.ScaleOverLifetime = scalePoints;

        GPUParticleDescriptor.LifetimePoints opacityPoints = new GPUParticleDescriptor.LifetimePoints();
        opacityPoints.Add(new Vector4(1.0f, 0, 0, 0));
        opacityPoints.Add(new Vector4(1.0f, 0, 0, 0.8f));
        opacityPoints.Add(new Vector4(0.0f, 0, 0, 1.0f));
        descriptor.OpacityOverLifetime = opacityPoints;

        descriptor.EmittMesh = TipGO.GetComponent<MeshFilter>().mesh;

        particleEmitter.ParticleDescriptor = descriptor;

        particleEmitter.Active = false;
        
        endAttractor = new GameObject();
        endAttractor.AddComponent<GPUParticleAttractor>().Power = 0.0f;
        endAttractor.transform.parent = TipGO.transform;
        endAttractor.transform.localPosition = Vector3.up * 17.0f;
        

        WandGO.transform.Rotate(90, 0, 0);
        WandGO.transform.position += Vector3.forward * 0.2f;


        VatsugWand wand = TipGO.AddComponent<VatsugWand>();
        wand.mEndAttractor = endAttractor;
        wand.mPowerEndAttractor = powerEndAttractor;
        wand.mPowerAttractors = powerNormalAttractors;
        wand.mReboundDistance = reboundDistance;
        wand.mParticleEmitter = emitter;// particleEmitter;
        wand.rightHand = rightHand;
        wand.pendulumSpeed = pendulumSpeed;

        WandGO.AddComponent<MirrorHandMovement>();

        return WandGO;
    }

    public static GameObject CreateBasicEnemy()
    {
        GameObject gameObject = new GameObject("Basic Enemy " + count++);

        // MESH.
        gameObject.AddComponent<MeshFilter>().mesh = CreateMesh(PrimitiveType.Sphere);

        // MATERIAL.
        Material material = gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"));
        material.color = Color.green;

        // COLLIDER.
        gameObject.AddComponent<GPUParticleSphereCollider>().Radius = 0.5f;

        // HEALTH.
        gameObject.AddComponent<Health>();

        return gameObject;
    }

    private static Mesh CreateMesh(PrimitiveType primitive)
    {

        GameObject tmp = GameObject.CreatePrimitive(primitive);
        Mesh mesh = tmp.GetComponent<MeshFilter>().mesh;
        Object.Destroy(tmp);

        return mesh;
    }
    

    public static void CreateVatsug(Level level, Transform parent)
    {


        GameObject StraitenOutFishObject = level.CreateGameObject("StraitenOutFishObject" + count++);
        StraitenOutFishObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load("VatsugLevel/nnj3de_crucarp/Materials/cruscarp", typeof(Material));
        StraitenOutFishObject.transform.Rotate(new Vector3(-90, 0, 0));
        StraitenOutFishObject.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("VatsugLevel/nnj3de_crucarp/cruscarp", typeof(Mesh));
        GPUParticleSphereCollider particleColider = StraitenOutFishObject.AddComponent<GPUParticleSphereCollider>();
        particleColider.Radius = 0.001f;
        StraitenOutFishObject.transform.localScale = new Vector3(300, 200, 200);
        StraitenOutFishObject.transform.parent = parent;
        StraitenOutFishObject.transform.localPosition = new Vector3(0, 0, -0.25f);

        StraitenOutFishObject.AddComponent<Health>().HealthStart = 1000;

        
        
        return;
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
