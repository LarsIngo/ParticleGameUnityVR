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
        GPUParticleDescriptor.LifetimePoints colorPoints = new GPUParticleDescriptor.LifetimePoints();
        colorPoints.Add(new Vector4(1, 1, 1, 0));
        colorPoints.Add(new Vector4(0, 0, 0, 1));
        descriptor.ColorOverLifetime = colorPoints;
        GPUParticleDescriptor.LifetimePoints sizePoints = new GPUParticleDescriptor.LifetimePoints();
        sizePoints.Add(new Vector4(0, 0, 0, 0));
        sizePoints.Add(new Vector4(1, 1, 1, 1));
        descriptor.ScaleOverLifetime = sizePoints;

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