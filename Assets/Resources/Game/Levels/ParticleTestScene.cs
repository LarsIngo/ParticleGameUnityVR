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
        GameObject screen = Factory.CreateWorldImage(this, "Textures/Default");
        screen.transform.position += Vector3.forward;
        descriptor.EmittMesh = screen.GetComponent<MeshFilter>().mesh;
        GPUParticleSystem system = screen.AddComponent<GPUParticleSystem>();
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