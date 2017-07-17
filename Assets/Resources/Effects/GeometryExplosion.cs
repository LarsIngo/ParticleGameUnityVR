using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Particle System on the GPU.
/// Each system acts like an emitter.
/// Config Emitt values to change the behaviour of the particles.
/// </summary>
public class GeometryExplosion : MonoBehaviour
{

    /// +++ MEMBERS +++ ///

    // Material.
    private Material mRenderMaterial = null;

    private float mExplosionSpeed = 1.0f;
    private float mCurrentOffset = 0.0f;
   

    private bool mActive = false;
    /// <summary>
    /// Whether emitter should emitt particles.
    /// Default: true
    /// </summary>
    public bool Active { get { return mActive; } set { mActive = value; } }

    
    // MONOBEHAVIOUR.
    private void Awake()
    {
        mRenderMaterial = new Material(Resources.Load<Shader>("Effects/GeometryExplosion"));
        mCurrentOffset = 0.0f;
        mRenderMaterial.SetFloat("gOffset", 0.2f);
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            this.GetComponent<Renderer>().material = mRenderMaterial;
            mActive = true;
        }
        
        if (mActive)
        {
            mCurrentOffset += mExplosionSpeed * Time.deltaTime;
            mRenderMaterial.SetFloat("gOffset", mCurrentOffset);
        }
    }
    
}
