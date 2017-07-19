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

    public static void CreateMichaelBayEffect(Level level, Mesh mesh, Transform t, Color meshColor)
    {
        GameObject michael = level.CreateGameObject("michael" + count++);
        GameObject blackHole = level.CreateGameObject("blackhole" + count++);
        michael.transform.position = t.position;
        michael.transform.rotation = t.rotation;
        blackHole.transform.position = t.position;
        blackHole.transform.rotation = t.rotation;

        GeometryExplosion exp = michael.AddComponent<GeometryExplosion>();
        exp.Mesh = mesh;
        exp.ExplosionColor = meshColor;
        exp.ExplosionSpeed = 8;
        exp.ShrinkSpeed = 1.0f;
        exp.ShrinkTime = 0.25f;

        TimerStretch timeStrech = michael.AddComponent<TimerStretch>();
        timeStrech.TimePrePhase = 0.5f;
        timeStrech.TimeMainPhase = 0.2f;
        timeStrech.TimePostPhase = 0.5f;
        timeStrech.TargetTimeScale = 0.1f;

        LifeTimer michaelLifetimer = michael.AddComponent<LifeTimer>();
        michaelLifetimer.LifeTime = 4.0f;

        GPUParticleAttractor attractor = blackHole.AddComponent<GPUParticleAttractor>();
        attractor.Power = 250.0f;
        LifeTimer blackHoleLifetimer = blackHole.AddComponent<LifeTimer>();
        blackHoleLifetimer.LifeTime = 0.2f;

        AudioSource ceramicSound = michael.AddComponent<AudioSource>();
        ceramicSound.clip = Resources.Load<AudioClip>("Samples/Explosion/Ceramic");
        ceramicSound.Play();

        AudioSource artillerySound = michael.AddComponent<AudioSource>();
        artillerySound.clip = Resources.Load<AudioClip>("Samples/Explosion/Artillery");
        artillerySound.time = 0.25f;
        artillerySound.Play();

    }

    public static GameObject CreateStageScreen(Level level, StageInfo stageInfo)
    {

        GameObject screen = CreateWorldImage(level, stageInfo.mThumbnail);
        screen.AddComponent<MeshCollider>();

        if (stageInfo.mLocked || stageInfo.mStarRequirement > Hub.Instance.stars)
        {

            GameObject lockImage = CreateWorldImage(level, "Textures/Locked", true);
            lockImage.transform.position -= Vector3.forward * 0.1f;
            lockImage.transform.parent = screen.transform;

            GameObject requirement = CreateWorldText(level, stageInfo.mStarRequirement.ToString(), Color.red * 0.9f);
            requirement.transform.position -= Vector3.forward * 0.15f;
            requirement.transform.localScale *= 2;
            requirement.transform.parent = screen.transform;

        }
        else
        {

            GameObject name = CreateWorldText(level, stageInfo.mName, Color.white);
            name.transform.position -= Vector3.up * 0.6f;
            name.transform.localScale *= 0.3f;
            name.transform.parent = screen.transform;

            GameObject stars = level.CreateGameObject("STARS" + count++);

            if (stageInfo.Score < stageInfo.mGold)
            {

                GameObject gold = CreateWorldImage(level, "Textures/Star");
                gold.transform.position += Vector3.right * 1.1f;
                gold.transform.parent = stars.transform;

            }
            if (stageInfo.Score < stageInfo.mSilver)
            {

                GameObject silver = CreateWorldImage(level, "Textures/Star");
                silver.transform.parent = stars.transform;

            }
            if (stageInfo.Score < stageInfo.mBronze)
            {

                GameObject bronze = CreateWorldImage(level, "Textures/Star");
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

    public static GameObject CreateWorldText(Level level, string text, Color color)
    {

        GameObject canvasGO = level.CreateGameObject("CANVAS" + count++);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.GetComponent<RectTransform>().position = Vector3.zero;
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000);
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1000);

        GameObject textGO = level.CreateGameObject("TEXT" + count++);
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

    public static GameObject CreateWorldImage(Level level, string image, bool transparent = false)
    {

        GameObject imageGO = level.CreateGameObject("image" + count++);
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

    public static GameObject CreateAttractorWand(Level level, float power, bool rightHand)
    {

        //The wand is the parent object to all the parts.
        GameObject WandGO = level.CreateGameObject("AttractorWand" + count++);

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
        GameObject TipGO = new GameObject("Tip" + count++);
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

        return WandGO;

    }

    public static GameObject CreateMenuWand(Level level, bool rightHand)
    {

        //The wand is the parent object to all the parts.
        GameObject WandGO = level.CreateGameObject("MenuWand" + count++);

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

        return WandGO;

    }


    public static GameObject CreateVatsugWand(Level level, float powerEndAttractor, float powerNormalAttractors, float pendulumSpeed, float reboundDistance, bool rightHand)
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
        
        GameObject endAttractor;

        GameObject emitter = new GameObject("VatsugEmitter");


        //We add the emitter to the tip.
        GPUParticleSystem particleEmitter = emitter.AddComponent<GPUParticleSystem>();
        GPUParticleAttractor attractor = emitter.AddComponent<GPUParticleAttractor>();
        TempVisuals(emitter, PrimitiveType.Sphere, Color.blue);
        emitter.GetComponent<Renderer>().enabled = false;
        emitter.transform.parent = TipGO.transform;
        emitter.transform.localScale = Vector3.one * 0.7f;
        emitter.transform.localPosition = new Vector3(1, 0, 0) * reboundDistance;

        particleEmitter.EmittMesh = TipGO.GetComponent<MeshFilter>().mesh;
        particleEmitter.EmittParticleLifeTime = 5.0f;
        particleEmitter.EmittFrequency = 500.0f;
        particleEmitter.EmittInheritVelocity = false;

        particleEmitter.Active = false;

        Vector4[] colorControlpoints = { new Vector4(1, 0, 0, 0), new Vector4(1, 1, 0, 0.3f), new Vector4(0, 1, 0, 1.0f) };
        particleEmitter.ColorLifetimePoints = colorControlpoints;

        Vector4[] haloControlpoints = { new Vector4(0, 0, 1, 0), new Vector4(1, 0, 1, 1.0f) };
        particleEmitter.HaloLifetimePoints = haloControlpoints;

        Vector4[] scaleControlpoints = { new Vector4(0.04f, 0.04f, 0, 0), new Vector4(0.01f, 0.01f, 0, 0.02f), new Vector4(0.01f, 0.01f, 0, 1) };
        particleEmitter.ScaleLifetimePoints = scaleControlpoints;

        Vector4[] transparencyControlpoints = { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 0.8f), new Vector4(0.0f, 0, 0, 1.0f) };
        particleEmitter.TransparencyLifetimePoints = transparencyControlpoints;
        
        endAttractor = new GameObject();
        endAttractor.AddComponent<GPUParticleAttractor>();
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

        count++;

        return WandGO;
    }

    public static GameObject CreateBasicEnemy(Level level, Vector3 position)
    {
        GameObject enemy = level.CreateGameObject("enemy" + count);
        enemy.AddComponent<BasicEnemy>();
        enemy.transform.position = position;

        count++;

        return enemy;
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
