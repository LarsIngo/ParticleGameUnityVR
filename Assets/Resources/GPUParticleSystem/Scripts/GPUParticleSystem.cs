using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleSystem : MonoBehaviour
{
    /// +++ STRUCTS +++ ///

    private class SwapBuffer
    {
        ComputeBuffer[] mBufferArray;
        int mBufferIndex;

        public SwapBuffer(int bufferCount, int count, int stride)
        {
            mBufferIndex = 0;
            mBufferArray = new ComputeBuffer[bufferCount];
            for (int i = 0; i < mBufferArray.GetLength(0); ++i)
                mBufferArray[i] = new ComputeBuffer(count, stride, ComputeBufferType.Default);
        }

        public void Release() { for (int i = 0; i < mBufferArray.GetLength(0); ++i) mBufferArray[i].Release(); }
        public ComputeBuffer GetInputBuffer() { return mBufferArray[mBufferIndex]; }
        public ComputeBuffer GetOutputBuffer() { return mBufferArray[(mBufferIndex + 1) % mBufferArray.GetLength(0)]; }
        public void Swap() { mBufferIndex = (mBufferIndex + 1) % mBufferArray.GetLength(0); }
    }

    /// --- STRUCTS --- ///


    /// +++ STATIC +++ ///

    static Material sRenderMaterial = null;
    static ComputeShader sComputeShader = null;
    static int sKernelUpdate = -1;
    static int sKernelEmitt = -1;
    static Dictionary<Mesh, ComputeBuffer> sMeshDictionary = null;

    // STARTUP.
    public static void StartUp()
    {
        sRenderMaterial = new Material(Resources.Load<Shader>("GPUParticleSystem/Shaders/GPUParticleRenderShader"));
        sComputeShader = Resources.Load<ComputeShader>("GPUParticleSystem/Shaders/GPUParticleComputeShader");
        sKernelUpdate = sComputeShader.FindKernel("UPDATE");
        sKernelEmitt = sComputeShader.FindKernel("EMITT");
        sMeshDictionary = new Dictionary<Mesh, ComputeBuffer>();
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sRenderMaterial = null;
        sComputeShader = null;
        sKernelUpdate = -1;
        sKernelEmitt = -1;
        foreach (KeyValuePair<Mesh, ComputeBuffer> it in sMeshDictionary) it.Value.Release(); // Release compute buffers.
        sMeshDictionary.Clear();
    }

    /// MEMBER

    /// --- STATIC --- ///

    /// +++ MEMBERS +++ ///

    // Particle.
    private SwapBuffer mPositionBuffer;
    private SwapBuffer mVelocityBuffer;
    private SwapBuffer mScaleBuffer;
    private SwapBuffer mColorBuffer;
    private SwapBuffer mLifetimeBuffer;

    private int mMaxParticleCount;
    private int mEmittIndex = 0;

    // Emitter.
    private float mEmittTimer = 0.0f;

    private float mEmittFrequency = 10.0f; private float mNewEmittFrequency = 10.0f;
    /// <summary>
    /// How many partices to emitt per second.
    /// Default: 10
    /// </summary>
    public float EmittFrequency{ get { return mEmittFrequency; } set { mNewEmittFrequency = value; mApply = true; } }

    private float mEmittParticleLifetime = 6.0f; private float mNewmParticleLifetime = 6.0f;
    /// <summary>
    /// How long a particle live in seconds.
    /// Default: 6
    /// </summary>
    public float EmittParticleLifeTime { get { return mEmittParticleLifetime; } set { mNewmParticleLifetime = value; mApply = true; } }

    private bool mActive = true;
    /// <summary>
    /// Whether emitter should emitt particles.
    /// Default: true
    /// </summary>
    public bool Active { get { return mActive; } set { mActive = value; } }

    private bool mApply = true;

    // APPLY.
    private void Apply() { DeInitSystem(); InitSystem(); mApply = false; }

    // MESH.
    private Mesh mEmittMesh = null; private Mesh mNewEmittMesh = null;
    /// <summary>
    /// Type of mesh to emitt from.
    /// If set to null, particles are emitted at emitter position.
    /// Default: null.
    /// </summary>
    public Mesh EmittMesh { get { return mEmittMesh; } set { mNewEmittMesh = value; mApply = true; } }

    private void UpdateMesh()
    {
        // Return early if mesh is null.
        if (mEmittMesh == null) return;

        // Return early if mesh is loaded.
        if (sMeshDictionary.ContainsKey(mEmittMesh)) return;

        int[] indicesIN = mEmittMesh.triangles;
        Vector3[] verticesIN = mEmittMesh.vertices;
        Vector3[] verticesOUT = new Vector3[indicesIN.GetLength(0)];

        // Create new emitt position buffer from mesh.
        ComputeBuffer emittPostionBuffer = new ComputeBuffer(indicesIN.GetLength(0), sizeof(float) * 3);
        for (int i = 0; i < indicesIN.GetLength(0); ++i)
        {
            int vID = indicesIN[i];
            verticesOUT[i] = verticesIN[vID];
        }
        
        // Update buffer.
        emittPostionBuffer.SetData(verticesOUT);

        // Add to dictionary.
        sMeshDictionary[mEmittMesh] = emittPostionBuffer;
    }

    // INIT.
    private void InitSystem()
    {
        mEmittFrequency = mNewEmittFrequency;
        mEmittParticleLifetime = mNewmParticleLifetime;
        mMaxParticleCount = (int)Mathf.Ceil(mEmittFrequency * mEmittParticleLifetime);

        // BUFFERS.
        mPositionBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);
        mVelocityBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);
        mScaleBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);
        mColorBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);
        mLifetimeBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);

        {   // Set lifetime default (negative)value.
            float[] arr = new float[mMaxParticleCount * 4];
            for (int i = 0; i < mMaxParticleCount * 4; ++i)
            {
                arr[i] = -0.01f;
            }
            mLifetimeBuffer.GetInputBuffer().SetData(arr);
            mLifetimeBuffer.GetOutputBuffer().SetData(arr);
        }

        // MESH.
        mEmittMesh = mNewEmittMesh;
        UpdateMesh();

    }

    // DEINIT.
    private void DeInitSystem()
    {
        mPositionBuffer.Release();
        mVelocityBuffer.Release();
        mScaleBuffer.Release();
        mColorBuffer.Release();
        mLifetimeBuffer.Release();
    }

    // EMITT UPDATE.
    private void EmittUppdate()
    {
        // Update timer.
        mEmittTimer += Time.deltaTime;

        int emittCount = (int)(mEmittFrequency * mEmittTimer);

        if (emittCount == 0) return;

        mEmittTimer -= emittCount * 1.0f / mEmittFrequency;

        for (int i = 0; i < emittCount; ++i)
        {
            // BIND PARTICLE BUFFERS.
            sComputeShader.SetBuffer(sKernelEmitt, "gPositionBuffer", mPositionBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gVelocityBuffer", mVelocityBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gScaleBuffer", mScaleBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gColorBuffer", mColorBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gLifetimeBuffer", mLifetimeBuffer.GetOutputBuffer());

            // EMITT INFO.
            sComputeShader.SetInt("gEmittIndex", mEmittIndex);
            sComputeShader.SetFloats("gPosition", new float[] { transform.position.x, transform.position.y, transform.position.z });
            sComputeShader.SetFloats("gVelocity", new float[] { 0, 1, 0 });
            sComputeShader.SetFloats("gScale", new float[] { 1, 1 });
            sComputeShader.SetFloats("gColor", new float[] { 0, 1, 0 });
            sComputeShader.SetFloats("gLifetime", new float[] { mEmittParticleLifetime });

            // EMITT MESH.

            // DISPATCH.
            sComputeShader.Dispatch(sKernelEmitt, 1, 1, 1);

            // Increment emitt index.
            mEmittIndex = (mEmittIndex + 1) % mMaxParticleCount;
        }
    }

    // UPDATE.
    private void UpdateSystem()
    {
        // SWAP INPUT/OUTPUT.
        mPositionBuffer.Swap();
        mVelocityBuffer.Swap();
        mScaleBuffer.Swap();
        mColorBuffer.Swap();
        mLifetimeBuffer.Swap();

        // BIND INPUT BUFFERS.
        sComputeShader.SetBuffer(sKernelUpdate, "gPositionIN", mPositionBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gVelocityIN", mVelocityBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gScaleIN", mScaleBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gColorIN", mColorBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gLifetimeIN", mLifetimeBuffer.GetInputBuffer());

        // BIND OUTPUT BUFFERS.
        sComputeShader.SetBuffer(sKernelUpdate, "gPositionOUT", mPositionBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gVelocityOUT", mVelocityBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gScaleOUT", mScaleBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gColorOUT", mColorBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gLifetimeOUT", mLifetimeBuffer.GetOutputBuffer());

        // SET META DATA.
        sComputeShader.SetInt("gMaxParticleCount", mMaxParticleCount);
        sComputeShader.SetFloat("gDeltaTime", Time.deltaTime);

        sComputeShader.SetFloats("gForce", new float[] { 0, 0, 0 });
        sComputeShader.SetFloat("gDrag", 0);

        // DISPATCH.
        sComputeShader.Dispatch(sKernelUpdate, (int)Mathf.Ceil(mMaxParticleCount / 64.0f), 1, 1);
    }

    // RENDER.
    private void RenderSystem()
    {
        sRenderMaterial.SetPass(0);

        // BIND BUFFERS.
        sRenderMaterial.SetBuffer("gPosition", mPositionBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gVelocity", mVelocityBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gScale", mScaleBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gColor", mColorBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gLifetime", mLifetimeBuffer.GetOutputBuffer());

        // DRAW.
        Graphics.DrawProcedural(MeshTopology.Points, mMaxParticleCount, 1);
    }

    public int Count { get { return mMaxParticleCount; } }

    // MONOBEHAVIOUR.
    private void Awake()
    {
        if (sRenderMaterial == null) StartUp();
        InitSystem();
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        // Update buffers if needed (updated).
        if (mApply)
            Apply();

        // Emitt new particles this frame if active.
        if (mActive)
            EmittUppdate();

        // Update particles this frame.
        UpdateSystem();
    }

    // MONOBEHAVIOUR.
    private void OnRenderObject()
    {
        // Render this frame.
        RenderSystem();
    }

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitSystem();
        if (sRenderMaterial != null) Shutdown();
    }

    /// --- MEMBERS --- ///

}
