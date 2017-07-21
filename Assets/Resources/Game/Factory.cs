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

    public static void CreateMichaelBayEffect(Mesh mesh, Transform t, Color meshColor, bool timestretch = true)
    {
        GameObject michael = new GameObject("michael" + count++);
        //GameObject blackHole = new GameObject("blackhole" + count++);
        michael.transform.position = t.position;
        michael.transform.rotation = t.rotation;
        michael.transform.localScale = t.localScale;
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

        if (timestretch)
        {
            TimerStretch timeStrech = michael.AddComponent<TimerStretch>();
            timeStrech.TimePrePhase = 0.5f;
            timeStrech.TimeMainPhase = 0.2f;
            timeStrech.TimePostPhase = 0.5f;
            timeStrech.TargetTimeScale = 0.1f;
        }

        LifeTimer michaelLifetimer = michael.AddComponent<LifeTimer>();
        michaelLifetimer.LifeTime = 4.0f;

        //GPUParticleAttractor attractor = blackHole.AddComponent<GPUParticleAttractor>();
        //attractor.Power = 250.0f;
        //LifeTimer blackHoleLifetimer = blackHole.AddComponent<LifeTimer>();
        //blackHoleLifetimer.LifeTime = 0.2f;

        AudioSource ceramicSound = michael.AddComponent<AudioSource>();
        ceramicSound.clip = Resources.Load<AudioClip>("Samples/Explosion/Ceramic");
        ceramicSound.time = 0.2f;
        ceramicSound.spatialBlend = 1.0f;
        ceramicSound.Play();

        AudioSource artillerySound = michael.AddComponent<AudioSource>();
        artillerySound.clip = Resources.Load<AudioClip>("Samples/Explosion/Artillery");
        artillerySound.time = 0.25f;
        artillerySound.spatialBlend = 1.0f;
        artillerySound.Play();

    }

    public static void CreateCelebration()
    {
        float lifetime = 25.0f;//6.8f;

        // Celebration
        GameObject celebration = new GameObject("Celebration " + Time.time);

        // LFEITIME.
        celebration.AddComponent<LifeTimer>().LifeTime = lifetime;

        // AUDIO.
        AudioSource cheeringSound = celebration.AddComponent<AudioSource>();
        cheeringSound.clip = Resources.Load<AudioClip>("Samples/Cheering/cheering");
        cheeringSound.Play();

        AudioSource encouragementSound = celebration.AddComponent<AudioSource>();
        encouragementSound.clip = Resources.Load<AudioClip>("Samples/Cheering/encouragement");
        encouragementSound.Play();

        AudioSource fireworkSound = celebration.AddComponent<AudioSource>();
        fireworkSound.clip = Resources.Load<AudioClip>("Samples/Firework/firework1");
        fireworkSound.Play();

        // FireworkSpawn
        GameObject fireworkSpawn = new GameObject("FireworkSpawn " + Time.time);

        fireworkSpawn.AddComponent<FireworkSpawn>();

        fireworkSpawn.AddComponent<LifeTimer>().LifeTime = lifetime / 2.0f;
    }

    public static void CreateFireworkHead(Vector3 position)
    {
        GameObject gameObject = new GameObject("Firework " + Time.time);
        gameObject.transform.position = position;

        // PARTICLESYSTEM.
        GPUParticleSystem system = gameObject.AddComponent<GPUParticleSystem>();

        GPUParticleDescriptor descriptor = new GPUParticleDescriptor();
        descriptor.EmittFrequency = 500.0f;
        descriptor.Lifetime = 1.0f;
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

        descriptor.EmittMesh = CreateMesh(PrimitiveType.Sphere);

        system.ParticleDescriptor = descriptor;

        // ATTRACTOR.
        gameObject.AddComponent<GPUParticleAttractor>().Power = -10.0f;

        // LIFETIME.
        gameObject.AddComponent<LifeTimer>().LifeTime = 5;
    }

    public static void CreateFeedbackText(string text, Color color, Vector3 origin, Vector3 velocity)
    {
        GameObject gameObject = CreateWorldText(text, color);

        gameObject.transform.position = origin;

        gameObject.transform.LookAt((origin - Vector3.zero) * 2);

        gameObject.AddComponent<LifeTimer>().LifeTime = 0.5f;

        gameObject.AddComponent<Rigidbody>().velocity = velocity;

        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = Resources.Load<AudioClip>("Samples/Ping/ping");
        audio.Play();
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

            if (stageInfo.Score >= stageInfo.mGold)
            {

                GameObject gold = CreateWorldImage("Textures/Star", true);
                gold.transform.position += Vector3.right * 1.1f;
                gold.transform.parent = stars.transform;

            }
            if (stageInfo.Score >= stageInfo.mSilver)
            {

                GameObject silver = CreateWorldImage("Textures/Star", true);
                silver.transform.parent = stars.transform;

            }
            if (stageInfo.Score >= stageInfo.mBronze)
            {

                GameObject bronze = CreateWorldImage("Textures/Star", true);
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

        Health.HEALTH_FACTOR = 2.0f;
        GPUParticleDescriptor descriptor = new GPUParticleDescriptor();
        descriptor.EmittFrequency = 1000.0f;
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
        scalePoints.Add(new Vector4(0.005f, 0.005f, 0, 0));
        scalePoints.Add(new Vector4(0.005f, 0.005f, 0, 1));
        descriptor.ScaleOverLifetime = scalePoints;

        GPUParticleDescriptor.LifetimePoints opacityPoints = new GPUParticleDescriptor.LifetimePoints();
        opacityPoints.Add(new Vector4(0.0f, 0, 0, 0));
        opacityPoints.Add(new Vector4(1.0f, 0, 0, 0.1f));
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

    public static GameObject CreateVatsugWand(float powerEndAttractor, float powerNormalAttractors, float pendulumSpeed, float reboundDistance, bool rightHand)
    {
        //The wand is the parent object to all the parts.
        GameObject WandGO = new GameObject("VatsugWand" + count++);

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

        //++++++++++ WAND ++++++++++

        GameObject endAttractor;

        GameObject emitter = new GameObject("VatsugEmitter" + count++);


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

        GameObject hand = new GameObject(rightHand ? "Right" : "Left" + "hand");

        WandGO.transform.parent = hand.transform;
        hand.AddComponent<MirrorHandMovement>();

        return WandGO;
    }

    public static GameObject CreateBasicEnemy(Vector3 position, int health)
    {
        GameObject gameObject = new GameObject("Basic Enemy " + count++);
        gameObject.transform.position = position;

        // MESH.
        gameObject.AddComponent<MeshFilter>().mesh = CreateMesh(PrimitiveType.Sphere);

        // MATERIAL.
        Material material = gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Unlit/Color"));
        material.color = Color.green;

        // COLLIDER.
        gameObject.AddComponent<GPUParticleSphereCollider>().Radius = 0.5f;

        // HEALTH.
        gameObject.AddComponent<Health>().HealthStart = health;

        // FEEDBACK.
        gameObject.AddComponent<FeedBack>();

        return gameObject;
    }

    private static Mesh CreateMesh(PrimitiveType primitive)
    {

        GameObject tmp = GameObject.CreatePrimitive(primitive);
        Mesh mesh = tmp.GetComponent<MeshFilter>().mesh;
        Object.Destroy(tmp);

        return mesh;
    }
    

    public static void CreateVatsug(Transform parent)
    {


        GameObject StraitenOutFishObject = new GameObject("StraitenOutFishObject" + count++);
        StraitenOutFishObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load("VatsugLevel/nnj3de_crucarp/Materials/cruscarp", typeof(Material));
        StraitenOutFishObject.transform.Rotate(new Vector3(-90, 0, 0));
        StraitenOutFishObject.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("VatsugLevel/nnj3de_crucarp/cruscarp", typeof(Mesh));
        GPUParticleSphereCollider particleColider = StraitenOutFishObject.AddComponent<GPUParticleSphereCollider>();
        particleColider.Radius = 0.001f;
        StraitenOutFishObject.transform.localScale = new Vector3(300, 200, 200);
        StraitenOutFishObject.transform.parent = parent;
        StraitenOutFishObject.transform.localPosition = new Vector3(0, 0, -0.25f);

        Health hp = StraitenOutFishObject.AddComponent<Health>();
        hp.HealthStart = 1000;
        hp.slowmotionEffect = false;

        AudioSource sound = StraitenOutFishObject.AddComponent<AudioSource>();
        sound.volume = 0.1f;
        sound.maxDistance = 10.0f;
        sound.spatialBlend = 1.0f;

        

        return;
    }

    public static void CreateVatsug2(Transform parent)
    {
        GameObject StraitenOutBirdObject = new GameObject("StraitenOutBirdObject" + count++);

        //---------------------------------------------------------------------------//
        /*Low Poly Bird (Animated) by Charlie Tinley is licensed under CC Attribution*/
        //---------------------------------------------------------------------------//
        StraitenOutBirdObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load("VatsugLevel/Fagel/source/Materials/BirdUVTexture", typeof(Material));
        StraitenOutBirdObject.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("VatsugLevel/Fagel/source/Bird_Asset", typeof(Mesh));

        GPUParticleSphereCollider particleColider = StraitenOutBirdObject.AddComponent<GPUParticleSphereCollider>();
        particleColider.Radius = 0.045f;
        StraitenOutBirdObject.transform.localScale = new Vector3(4, 4, 4);
        StraitenOutBirdObject.transform.parent = parent;
        StraitenOutBirdObject.transform.localPosition = new Vector3(0, 0, -0.1f);

        Health hp = StraitenOutBirdObject.AddComponent<Health>();
        hp.HealthStart = 1000;
        hp.slowmotionEffect = false;

        AudioSource sound = StraitenOutBirdObject.AddComponent<AudioSource>();
        sound.volume = 0.1f;
        sound.maxDistance = 15.0f;
        sound.spatialBlend = 1.0f;
        sound.clip = Resources.Load<AudioClip>("Samples/Vatsug/gull");


        return;
    }
    

    public static GameObject CreateBoat()
    {
        GameObject boat = new GameObject("dasBoot" + count++);

        boat.AddComponent<MeshRenderer>().material = (Material)Resources.Load("VatsugLevel/Boat/Meshes/Materials/Boat_MAT", typeof(Material));
        boat.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("VatsugLevel/Boat/Meshes/Boat_Mesh", typeof(Mesh));
        boat.transform.Rotate(new Vector3(-90, 90, 0));
        
        return boat;
    }

    public static GameObject CreateMoon()
    {
        GameObject moon = new GameObject("moon" + count++);
        Material mat = (Material)Resources.Load("VatsugLevel/Moon/Moon_mat");
        
        moon.AddComponent<MeshRenderer>().material = mat;
        moon.AddComponent<MeshFilter>().mesh = CreateMesh(PrimitiveType.Sphere);

        Light moonLight = moon.AddComponent<Light>();
        moonLight.type = LightType.Directional;
        moonLight.shadows = LightShadows.Soft;
        moonLight.color = new Color(0.25f, 0.25f, 0.25f, 1.0f);

        moon.transform.localScale = new Vector3(5, 5, 5);
        moon.transform.position = new Vector3(-20, 20, 20);


        moonLight.transform.LookAt(new Vector3(0, 0, 0));

        return moon;
    }

    public static GameObject CreateWater()
    {
        GameObject waterGO = new GameObject("water" + count++);
        //waterGO.transform.localScale = new Vector3(5, 1, 5);

        Material mat = (Material)Resources.Load("Water/Materials/WaterBasicNighttime");

        //Material mat = (Material)Resources.Load("WaterOther/Materials/WaterProDaytime");
        //mat.shader = Shader.Find("FX/Water");

        waterGO.AddComponent<MeshRenderer>().material = mat;
        waterGO.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("WaterOther/Models/WaterPlane", typeof(Mesh));// CreateMesh(PrimitiveType.Plane);
        waterGO.transform.localScale = new Vector3(32, 1, 32);

        UnityStandardAssets.Water.WaterBasic vann = waterGO.AddComponent<UnityStandardAssets.Water.WaterBasic>();
        

        return waterGO;
    }

    public static void CreateIsland()
    {
        GameObject island = new GameObject("Island" + count++);

        island.AddComponent<MeshRenderer>().material = (Material)Resources.Load("VatsugLevel/Moon/sand_mat");
        island.AddComponent<MeshFilter>().mesh = CreateMesh(PrimitiveType.Sphere);
        island.transform.localScale = new Vector3(5, 0.5f, 5);
        island.transform.position = new Vector3(0, -0.1f, 0);

        string name = "Palmtree" + count++;
        Debug.Assert(!GameObject.Find(name));

        GameObject go = GameObject.Instantiate(Resources.Load("VatsugLevel/Palm/Prefabs/PalmTree_dual_ 2sided")) as GameObject;
        go.name = name;
        go.transform.position = new Vector3(0.3f, 0.2f, 1.8f);
        go.transform.eulerAngles = new Vector3(10, 20, 10);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        //level.AddChildGO(go);



        //PalmTree_dual_leafs_1sided


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
