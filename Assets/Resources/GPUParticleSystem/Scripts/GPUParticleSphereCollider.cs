using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Particle sphere collider to interact with particles on the GPU.
/// </summary>
public class GPUParticleSphereCollider : MonoBehaviour
{
    /// +++ STATIC +++ ///

    private static Dictionary<GPUParticleSphereCollider, GPUParticleSphereCollider> sGPUParticleSphereColliderDictionary = null;
    public static Dictionary<GPUParticleSphereCollider, GPUParticleSphereCollider> GetGPUParticleSphereColliderDictionary() { return sGPUParticleSphereColliderDictionary; }


    // STARTUP.
    public static void StartUp()
    {
        sGPUParticleSphereColliderDictionary = new Dictionary<GPUParticleSphereCollider, GPUParticleSphereCollider>();
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sGPUParticleSphereColliderDictionary.Clear();
        sGPUParticleSphereColliderDictionary = null;
    }

    /// --- STATIC --- ///


    /// +++ MEMBERS +++ ///

    private float mRadius = 1.0f;
    /// <summary>
    /// Radius of sphere.
    /// Default: 1
    /// </summary>
    public float Radius { get { return mRadius; } set { mRadius = value; } }

    private void InitAttractor()
    {

    }

    private void DeInitAttractor()
    {

    }

    /// --- MEMBERS --- ///

    // MONOBEHAVIOUR.
    private void Awake()
    {
        if (sGPUParticleSphereColliderDictionary == null) StartUp();
        InitAttractor();
        sGPUParticleSphereColliderDictionary[this] = this;
    }

    // MONOBEHAVIOUR.
    private void Update()
    {

    }

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitAttractor();
        sGPUParticleSphereColliderDictionary.Remove(this);
        if (sGPUParticleSphereColliderDictionary.Count == 0) Shutdown();
    }

    // MONOBEHAVIOUR.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, Mathf.Max(Mathf.Max(transform.localScale.x, transform.localScale.y), transform.localScale.z) * mRadius);
    }

}
