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


    private bool mActive = false;
    private float mCurrentOffset = 0.0f;

    private float mExplosionSpeed = 1.0f;
    /// <summary>
    /// Speed of with which the explosion expands... :)
    /// Default: 1.0f -->gg
    /// </summary>
    public float ExplosionSpeed { get { return mExplosionSpeed; } set { mExplosionSpeed = value; } }

    public Mesh Mesh { set { this.gameObject.GetComponent<MeshFilter>().mesh = value; } }

    public Color ExplosionColor { set
        {
            mRenderMaterial.SetFloat("gAmbientRed", value.r);
            mRenderMaterial.SetFloat("gAmbientGreen", value.g);
            mRenderMaterial.SetFloat("gAmbientBlue", value.b);
        }
    }
    
    // MONOBEHAVIOUR.
    private void Awake()
    {
        this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = this.gameObject.AddComponent<MeshRenderer>();
        
        mRenderMaterial = new Material(Resources.Load<Shader>("Effects/GeometryExplosion"));
        Debug.Assert(mRenderMaterial != null);
        

        renderer.material = mRenderMaterial;
        

        mCurrentOffset = 0.0f;
        
        mRenderMaterial.SetFloat("gOffset", mCurrentOffset);
        mRenderMaterial.SetFloat("gAmbientRed", 1.0f);
        mRenderMaterial.SetFloat("gAmbientGreen", 1.0f);
        mRenderMaterial.SetFloat("gAmbientBlue", 1.0f);
    }

    public void Explode()
    {
        mActive = true;
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        if (mActive)
        {
            mCurrentOffset += mExplosionSpeed * Hub.Instance.DeltaTime;
            
            mRenderMaterial.SetFloat("gOffset", mCurrentOffset);
            
        }
    }
    
}
