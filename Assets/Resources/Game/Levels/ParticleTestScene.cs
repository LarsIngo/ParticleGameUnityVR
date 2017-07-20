using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTestScene : Level
{
    /// +++ MEMBERS +++ ///

    /// --- MEMBERS --- ///


    /// +++ FUNCTIONS +++ ///

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name">Name of level, must be unique.</param>
    public ParticleTestScene(string name) : base(name)
    {

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

        GameObject test = CreateGameObject("test");
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.parent = test.transform;

        descriptor.EmittMesh = sphere.GetComponent<MeshFilter>().mesh;

        GPUParticleSystem system = sphere.AddComponent<GPUParticleSystem>();
        system.ParticleDescriptor = descriptor;

    }


    /// <summary>
    /// Override awake function.
    /// </summary>
    public override void Awake()
    {



    }

    /// <summary>
    /// Override update function.
    /// </summary>
    public override void Update()
    {



    }

    /// <summary>
    /// Override sleep function.
    /// </summary>
    public override void Sleep()
    {


    }

    /// --- FUNCTIONS --- ///
}