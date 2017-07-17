using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Particle vector fields to interact with particles on the GPU.
/// </summary>
public class GPUParticleVectorField : MonoBehaviour
{
    /// +++ STATIC +++ ///

    private static Dictionary<GPUParticleVectorField, GPUParticleVectorField> sGPUParticleVectorFieldDictionary = null;
    public static Dictionary<GPUParticleVectorField, GPUParticleVectorField> GetGPUParticleAttractorDictionary() { return sGPUParticleVectorFieldDictionary; }

    // STARTUP.
    public static void StartUp()
    {
        sGPUParticleVectorFieldDictionary = new Dictionary<GPUParticleVectorField, GPUParticleVectorField>();
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sGPUParticleVectorFieldDictionary.Clear();
        sGPUParticleVectorFieldDictionary = null;
    }

    /// --- STATIC --- ///


    /// +++ MEMBERS +++ ///

    private float mRadius = 1.0f;
    /// <summary>
    /// The radius if the field.
    /// Default: 1
    /// </summary>
    public float Radius { get { return mRadius; } set { mRadius = value; } }

    private Vector3 mVector = Vector3.up;
    /// <summary>
    /// The vector of the field.
    /// Not relative to transform rotation.
    /// Default: Up
    /// </summary>
    public Vector3 Vector { get { return mVector; } set { mVector = value; } }

    private bool mRelativeVectorField = false;
    /// <summary>
    /// Toggle the vector to cnage with rotation.
    /// Default: false
    /// </summary>
    public bool RelativeVectorField { get { return mRelativeVectorField; } set { mRelativeVectorField = value; } }

    /// <summary>
    /// The vector of the field relative to transfrom rotation.
    /// </summary>
    public Vector3 VectorRelative { get { return transform.rotation * mVector; } }

    private void InitVectorField()
    {

    }

    private void DeInitVectorField()
    {

    }

    /// --- MEMBERS --- ///

    // MONOBEHAVIOUR.
    private void Awake ()
    {
        if (sGPUParticleVectorFieldDictionary == null) StartUp();
        InitVectorField();
        sGPUParticleVectorFieldDictionary[this] = this;
    }

    // MONOBEHAVIOUR.
    private void Update ()
    {
		
	}

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitVectorField();
        sGPUParticleVectorFieldDictionary.Remove(this);
        if (sGPUParticleVectorFieldDictionary.Count == 0) Shutdown();
    }

    // MONOBEHAVIOUR.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, Mathf.Max(Mathf.Max(transform.lossyScale.x, transform.lossyScale.y), transform.lossyScale.z) * mRadius);
    }
}
