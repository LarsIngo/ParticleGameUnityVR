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

        GameObject test = CreateGameObject("test");
        GPUParticleDescriptor descriptor = new GPUParticleDescriptor();

        GPUParticleSystem system = test.AddComponent<GPUParticleSystem>();
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