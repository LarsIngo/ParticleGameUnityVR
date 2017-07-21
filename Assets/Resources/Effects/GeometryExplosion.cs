using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float mTimer = 0;

    private bool mActive = false;
    private float mCurrentOffset = 0.0f;

    private float mExplosionSpeed = 1.0f;
    /// <summary>
    /// Speed of with which the explosion expands... :)
    /// Default: 1.0f -->gg
    /// </summary>
    public float ExplosionSpeed { get { return mExplosionSpeed; } set { mExplosionSpeed = value; } }

    private float mShrinkSpeed = 1.0f;
    /// <summary>
    /// Speed of with which the mesh shrinks.
    /// Default: 1.0f
    /// </summary>
    public float ShrinkSpeed { get { return mShrinkSpeed; } set { mShrinkSpeed = value; } }

    private float mShrinkTime = 1.0f;
    /// <summary>
    /// How long the mesh shrinks before exploding.
    /// Default: 0.5f
    /// </summary>
    public float ShrinkTime { get { return mShrinkTime; } set { mShrinkTime = value; } }

    public Mesh Mesh { set { gameObject.GetComponent<MeshFilter>().mesh = value; } }

    

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
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        
        mRenderMaterial = new Material(Resources.Load<Shader>("Effects/GeometryExplosion"));
        Debug.Assert(mRenderMaterial != null);
        
        renderer.material = mRenderMaterial;
        
        mRenderMaterial.SetFloat("gOffset", mCurrentOffset);
        mRenderMaterial.SetFloat("gAmbientRed", 1.0f);
        mRenderMaterial.SetFloat("gAmbientGreen", 1.0f);
        mRenderMaterial.SetFloat("gAmbientBlue", 1.0f);
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        mTimer += Time.deltaTime;

        if (mTimer < mShrinkTime)
        {
            mCurrentOffset -= mShrinkSpeed * Time.deltaTime;
            mRenderMaterial.SetFloat("gOffset", mCurrentOffset);
        }
        else
        {
            mCurrentOffset += mExplosionSpeed * Time.deltaTime;
            mRenderMaterial.SetFloat("gOffset", mCurrentOffset);
        }

    }
    
}
