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

    private float mPower = 20.0f;
    /// <summary>
    /// Power of attractor.
    /// Default: 20
    /// </summary>
    public float Power { get { return mPower; } set { mPower = value; } }

    private float mMin = 0;
    /// <summary>
    /// Minimum distance to attractor.
    /// Default: 0
    /// </summary>
    public float Min { get { return mMin; } set { mMin = Mathf.Max(value, 0); } }

    private float mMax = float.MaxValue;
    /// <summary>
    /// Maximum distance to attractor.
    /// Default: float.MaxValue
    /// </summary>
    public float Max { get { return mMax; } set { mMax = Mathf.Max(value, 0); } }

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
