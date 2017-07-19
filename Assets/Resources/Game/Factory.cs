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
        exp.ExplosionSpeed = 5;

        LifeTimer michaelLifetimer = michael.AddComponent<LifeTimer>();
        michaelLifetimer.LifeTime = 4.0f;

        GPUParticleAttractor attractor = blackHole.AddComponent<GPUParticleAttractor>();
        attractor.Power = 1000.0f;
        LifeTimer blackHoleLifetimer = blackHole.AddComponent<LifeTimer>();
        blackHoleLifetimer.LifeTime = 0.5f;

        exp.Explode();

        count++;

        return michael;
    }

    public static GameObject CreateStageScreen(Level level, StageInfo stageInfo)
    {

        GameObject screen = CreateWorldImage(level, stageInfo.thumbnail);
        screen.AddComponent<MeshCollider>();

        if (stageInfo.locked || stageInfo.starRequirement > Hub.Instance.stars)
        {

            GameObject lockImage = CreateWorldImage(level, "Textures/Locked", true);
            lockImage.transform.position -= Vector3.forward * 0.1f;
            lockImage.transform.parent = screen.transform;

        }
        else
        {

            GameObject name = CreateWorldText(level, stageInfo.name, Color.white);
            name.transform.position -= Vector3.up * 0.6f;
            name.transform.localScale *= 0.3f;
            name.transform.parent = screen.transform;



            string medalText = "";
            if (stageInfo.Score < stageInfo.gold)
                medalText = "Gold";
            else if (stageInfo.Score < stageInfo.silver)
                medalText = "Silver";
            else if (stageInfo.Score < stageInfo.bronze)
                medalText = "Bronze";

            GameObject medal = CreateWorldText(level, medalText, Color.black);
            medal.transform.position -= Vector3.up * 0.8f;
            medal.transform.localScale *= 0.4f;
            medal.transform.SetParent(screen.transform);

            screen.AddComponent<StageScreen>().stageInfo = stageInfo;

            count++;

        }

        return screen;

    }

    public static GameObject CreateWorldText(Level level, string text, Color color)
    {

        GameObject canvasGO = level.CreateGameObject("CANVAS" + count);
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.GetComponent<RectTransform>().position = Vector3.zero;
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1000);
        canvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1000);

        GameObject textGO = level.CreateGameObject("TEXT" + count);
        textGO.transform.SetParent(canvasGO.transform);
        UnityEngine.UI.Text textUI = textGO.AddComponent<UnityEngine.UI.Text>();
        textUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 5000);
        textUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 5000);
        textUI.transform.localScale *= 0.001f;
        textUI.text = text;
        textUI.fontSize = 299;
        textUI.color = color;
        textUI.alignment = TextAnchor.MiddleCenter;
        textUI.font = Resources.Load<Font>("Fonts/unispace bd");

        count++;

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
