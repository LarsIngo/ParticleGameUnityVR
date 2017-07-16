using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Particle attractors to interact with particles on the GPU.
/// </summary>
public class GPUParticleAttractor : MonoBehaviour
{
    /// +++ STATIC +++ ///

    private static Dictionary<GPUParticleAttractor, GPUParticleAttractor> sGPUParticleAttractorDictionary = null;
    public static Dictionary<GPUParticleAttractor, GPUParticleAttractor> GetGPUParticleAttractorDictionary() { return sGPUParticleAttractorDictionary; }

    // STARTUP.
    public static void StartUp()
    {
        sGPUParticleAttractorDictionary = new Dictionary<GPUParticleAttractor, GPUParticleAttractor>();
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sGPUParticleAttractorDictionary.Clear();
        sGPUParticleAttractorDictionary = null;
    }

    /// --- STATIC --- ///


    /// +++ MEMBERS +++ ///

    private float mPower = 100.0f;
    /// <summary>
    /// Power of attractor.
    /// Default: 100
    /// </summary>
    public float Power { get { return mPower; } set { mPower = value; } }

    private void InitAttractor()
    {

    }

    private void DeInitAttractor()
    {

    }

    /// --- MEMBERS --- ///

    // MONOBEHAVIOUR.
    private void Awake ()
    {
        if (sGPUParticleAttractorDictionary == null) StartUp();
        InitAttractor();
        sGPUParticleAttractorDictionary[this] = this;
    }

    // MONOBEHAVIOUR.
    private void Update ()
    {
		
	}

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitAttractor();
        sGPUParticleAttractorDictionary.Remove(this);
        if (sGPUParticleAttractorDictionary.Count == 0) Shutdown();
    }
}
