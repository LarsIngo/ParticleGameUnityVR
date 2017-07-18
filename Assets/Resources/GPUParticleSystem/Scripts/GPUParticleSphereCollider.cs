using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Particle sphere collider to interact with particles on the GPU.
/// </summary>
public class GPUParticleSphereCollider : MonoBehaviour
{
    /// +++ STATIC +++ ///

    private static List<GPUParticleSphereCollider> sGPUParticleSphereColliderList = null;
    public static List<GPUParticleSphereCollider> GetGPUParticleSphereColliderList() { return sGPUParticleSphereColliderList; }

    // STARTUP.
    public static void StartUp()
    {
        sGPUParticleSphereColliderList = new List<GPUParticleSphereCollider>();
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sGPUParticleSphereColliderList.Clear();
        sGPUParticleSphereColliderList = null;
    }

    /// --- STATIC --- ///


    /// +++ MEMBERS +++ ///

    private int mCollisionsThisFrame = 0;
    /// <summary>
    /// Number of particles that collieded with this collider during this frame.
    /// </summary>
    public int CollisionsThisFrame { get { return mCollisionsThisFrame; } }

    /// <summary>
    /// Used by GPUParticleSystem to set collisions this frame.
    /// </summary>
    public void SetCollisionsThisFrame(int collisions)
    {
        mCollisionsThisFrame = collisions;
    }

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
        if (sGPUParticleSphereColliderList == null) StartUp();
        InitAttractor();
        sGPUParticleSphereColliderList.Add(this);
    }

    // MONOBEHAVIOUR.
    private void Update()
    {

    }

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitAttractor();
        sGPUParticleSphereColliderList.Remove(this);
        if (sGPUParticleSphereColliderList.Count == 0) Shutdown();
    }

    // MONOBEHAVIOUR.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, Mathf.Max(Mathf.Max(transform.lossyScale.x, transform.lossyScale.y), transform.lossyScale.z) * mRadius);
    }

}
