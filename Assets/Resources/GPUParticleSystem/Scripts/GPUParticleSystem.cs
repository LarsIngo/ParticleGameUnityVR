using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

    private struct EmittMeshInfo
    {
        public ComputeBuffer mVertexBuffer;
        public int mVertexCount;
        public ComputeBuffer mIndexBuffer;
        public int mIndexCount;
    }

    /// --- STRUCTS --- ///


    /// +++ STATIC +++ ///

    static Material sRenderMaterial = null;
    static ComputeShader sComputeShader = null;
    static int sKernelUpdate = -1;
    static int sKernelEmitt = -1;
    static Dictionary<Mesh, EmittMeshInfo> sEmittMeshInfoDictionary = null;

    // STARTUP.
    public static void StartUp()
    {
        sRenderMaterial = new Material(Resources.Load<Shader>("GPUParticleSystem/Shaders/GPUParticleRenderShader"));
        sComputeShader = Resources.Load<ComputeShader>("GPUParticleSystem/Shaders/GPUParticleComputeShader");
        sKernelUpdate = sComputeShader.FindKernel("UPDATE");
        sKernelEmitt = sComputeShader.FindKernel("EMITT");
        sEmittMeshInfoDictionary = new Dictionary<Mesh, EmittMeshInfo>();
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sRenderMaterial = null;
        sComputeShader = null;
        sKernelUpdate = -1;
        sKernelEmitt = -1;
        foreach (KeyValuePair<Mesh, EmittMeshInfo> it in sEmittMeshInfoDictionary)
        {   // Release compute buffers.
            it.Value.mVertexBuffer.Release();
            it.Value.mIndexBuffer.Release();
        }
        sEmittMeshInfoDictionary.Clear();
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
    public float EmittFrequency{ get { return mEmittFrequency; } set { mNewEmittFrequency = value; /*mApply = true;*/ } }

    private float mEmittParticleLifetime = 6.0f; private float mNewEmittParticleLifetime = 6.0f;
    /// <summary>
    /// How long a particle live in seconds.
    /// Default: 6
    /// </summary>
    public float EmittParticleLifeTime { get { return mEmittParticleLifetime; } set { mNewEmittParticleLifetime = value; /*mApply = true;*/ } }

    private bool mActive = true;
    /// <summary>
    /// Whether emitter should emitt particles.
    /// Default: true
    /// </summary>
    public bool Active { get { return mActive; } set { mActive = value; } }

    //private bool mApply = true;

    // APPLY.
    private void Apply() { DeInitSystem(); InitSystem(); /*mApply = false;*/ }

    // MESH.
    private Mesh mEmittMesh = null; private Mesh mNewEmittMesh = null;
    /// <summary>
    /// Type of mesh to emitt from.
    /// If set to null, particles are emitted at emitter position.
    /// Default: null.
    /// </summary>
    public Mesh EmittMesh { get { return mEmittMesh; } set { mNewEmittMesh = value; /*mApply = true;*/ } }

    private void UpdateMesh()
    {
        // Return early if mesh is null.
        if (mEmittMesh == null) return;

        // Return early if mesh is loaded.
        if (sEmittMeshInfoDictionary.ContainsKey(mEmittMesh)) return;

        Vector3[] vertices = mEmittMesh.vertices;
        int[] indices = mEmittMesh.triangles;

        // Create new emitt mesh info from mesh.
        EmittMeshInfo emittMeshInfo = new EmittMeshInfo();
        emittMeshInfo.mVertexCount = vertices.GetLength(0);
        emittMeshInfo.mVertexBuffer = new ComputeBuffer(vertices.GetLength(0), sizeof(float) * 3);
        emittMeshInfo.mVertexBuffer.SetData(vertices);
        emittMeshInfo.mIndexCount = indices.GetLength(0);
        emittMeshInfo.mIndexBuffer = new ComputeBuffer(indices.GetLength(0), sizeof(int));
        emittMeshInfo.mIndexBuffer.SetData(indices);

        // Add to dictionary.
        sEmittMeshInfoDictionary[mEmittMesh] = emittMeshInfo;
    }

    // INIT.
    private void InitSystem()
    {
        mEmittFrequency = mNewEmittFrequency;
        mEmittParticleLifetime = mNewEmittParticleLifetime;
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
            sComputeShader.SetFloats("gScale", new float[] { 0.1f, 0.1f });
            sComputeShader.SetFloats("gColor", new float[] { 0, 1, 0 });
            sComputeShader.SetFloats("gLifetime", new float[] { mEmittParticleLifetime });

            // EMITT MESH.
            if (mEmittMesh == null)
            {
                // Set count to 0.
                sComputeShader.SetInt("gEmittMeshVertexCount", 0);
                sComputeShader.SetInt("gEmittMeshIndexCount", 0);
                sComputeShader.SetInt("gEmittMeshRandomIndex", 0);
            }
            else
            {
                Debug.Assert(sEmittMeshInfoDictionary.ContainsKey(mEmittMesh));

                // Set index and vertex buffer.
                EmittMeshInfo emittMeshInfo = sEmittMeshInfoDictionary[mEmittMesh];
                sComputeShader.SetBuffer(sKernelEmitt, "gEmittMeshVertexBuffer", emittMeshInfo.mVertexBuffer);
                sComputeShader.SetBuffer(sKernelEmitt, "gEmittMeshIndexBuffer", emittMeshInfo.mIndexBuffer);
                sComputeShader.SetInt("gEmittMeshVertexCount", emittMeshInfo.mVertexCount);
                sComputeShader.SetInt("gEmittMeshIndexCount", emittMeshInfo.mIndexCount);
                sComputeShader.SetInt("gEmittMeshRandomIndex", Random.Range(0, emittMeshInfo.mIndexCount - 1));
                sComputeShader.SetFloats("gEmittMeshScale", new float[] { transform.localScale.x, transform.localScale.y, transform.localScale.z });
            }
            


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

        sComputeShader.SetFloats("gForce", new float[] { 0, -9.82f, 0 });
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
        // Update GPU particle if needed (updated).
        if (mNewEmittFrequency != mEmittFrequency || mNewEmittParticleLifetime != mEmittParticleLifetime || mNewEmittMesh != mEmittMesh)
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
