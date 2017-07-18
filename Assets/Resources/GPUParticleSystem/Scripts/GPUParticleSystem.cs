using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Particle System on the GPU.
/// Each system acts like an emitter.
/// Config Emitt values to change the behaviour of the particles.
/// </summary>
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

    private struct SortElement
    {
        public float mValue;
        public int mIndex;
    }

    /// --- STRUCTS --- ///


    /// +++ STATIC +++ ///

    static Dictionary<GPUParticleSystem, GPUParticleSystem> sGPUParticleSystemDictionary = null;

    static ComputeShader sComputeShader = null;
    static int sKernelUpdate = -1;
    static int sKernelEmitt = -1;
    static int sKernelResult = -1;
    static int sKernelInitSort = -1;
    static int sKernelSort = -1;
    static Dictionary<Mesh, EmittMeshInfo> sEmittMeshInfoDictionary = null;

    static ComputeBuffer sGPUParticleAttractorBuffer = null;
    const int sMaxAttractorCount = 64;

    static ComputeBuffer sGPUParticleVectorFieldBuffer = null;
    const int sMaxVectorFieldCount = 64;

    static ComputeBuffer sGPUParticleSphereColliderBuffer = null;
    const int sMaxSphereColliderCount = 64;

    static SwapBuffer sGPUColliderResultSwapBuffer = null;
    const int sMaxGPUColliderCount = sMaxSphereColliderCount;

    static bool sLateUpdate;

    // STARTUP.
    public static void StartUp()
    {

        sGPUParticleSystemDictionary = new Dictionary<GPUParticleSystem, GPUParticleSystem>();

        sComputeShader = Resources.Load<ComputeShader>("GPUParticleSystem/Shaders/GPUParticleComputeShader");
        sKernelUpdate = sComputeShader.FindKernel("UPDATE");
        sKernelEmitt = sComputeShader.FindKernel("EMITT");
        sKernelResult = sComputeShader.FindKernel("RESULT");
        sKernelInitSort = sComputeShader.FindKernel("INITSORT");
        sKernelSort = sComputeShader.FindKernel("SORT");
        sEmittMeshInfoDictionary = new Dictionary<Mesh, EmittMeshInfo>();
        sGPUParticleAttractorBuffer = new ComputeBuffer(sMaxAttractorCount, sizeof(float) * 4);
        sGPUParticleVectorFieldBuffer = new ComputeBuffer(sMaxVectorFieldCount, sizeof(float) * 8);
        sGPUParticleSphereColliderBuffer = new ComputeBuffer(sMaxSphereColliderCount, sizeof(float) * 4);
        sGPUColliderResultSwapBuffer = new SwapBuffer(2, sMaxGPUColliderCount, sizeof(int));
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sGPUParticleSystemDictionary.Clear();
        sGPUParticleSystemDictionary = null;

        sComputeShader = null;
        sKernelUpdate = -1;
        sKernelEmitt = -1;
        sKernelResult = -1;
        sKernelInitSort = -1;
        sKernelSort = -1;
        foreach (KeyValuePair<Mesh, EmittMeshInfo> it in sEmittMeshInfoDictionary)
        {   // Release compute buffers.
            it.Value.mVertexBuffer.Release();
            it.Value.mIndexBuffer.Release();
        }
        sEmittMeshInfoDictionary.Clear();
        sGPUParticleAttractorBuffer.Release();
        sGPUParticleVectorFieldBuffer.Release();
        sGPUParticleSphereColliderBuffer.Release();
        sGPUColliderResultSwapBuffer.Release();
    }

    // FETCH COLLISION RESULTS.
    private static void FetchCollisionResults()
    {
        List<GPUParticleSphereCollider> sphereColliderList = GPUParticleSphereCollider.GetGPUParticleSphereColliderList();

        // Return early if null or zero GPUParticleSphereCollider.
        if (sphereColliderList == null) return;
        if (sphereColliderList.Count == 0) return;

        Debug.Assert(sphereColliderList.Count < sMaxSphereColliderCount);

        bool initZero = true;
        foreach (KeyValuePair<GPUParticleSystem, GPUParticleSystem> it in sGPUParticleSystemDictionary)
        {
            GPUParticleSystem system = it.Value;

            sGPUColliderResultSwapBuffer.Swap();
            sComputeShader.SetBuffer(sKernelResult, "gGPUColliderResultBufferIN", sGPUColliderResultSwapBuffer.GetInputBuffer());
            sComputeShader.SetBuffer(sKernelResult, "gGPUColliderResultBufferOUT", sGPUColliderResultSwapBuffer.GetOutputBuffer());

            sComputeShader.SetInt("gGPUColliderCount", sphereColliderList.Count);
            sComputeShader.SetBuffer(sKernelResult, "gSphereColliderResultBufferREAD", system.GetSphereColliderResultBuffer());

            sComputeShader.SetBool("gInitZero", initZero);
            initZero = false;

            // DISPATCH.
            sComputeShader.Dispatch(sKernelResult, (int)Mathf.Ceil(sMaxSphereColliderCount / 64.0f), 1, 1);
        }

        // GET DATA FROM GPU TO CPU.
        int[] collisionData = new int[sphereColliderList.Count];
        sGPUColliderResultSwapBuffer.GetOutputBuffer().GetData(collisionData);

        // UPDATE COLLIDERS.
        for (int i = 0; i < sphereColliderList.Count; ++i)
        {
            GPUParticleSphereCollider collider = sphereColliderList[i];
            collider.SetCollisionsThisFrame(collisionData[i]);
        }
    }

    /// --- STATIC --- ///


    /// +++ MEMBERS +++ ///

    // Material.
    private Material mRenderMaterial = null;

    // Particle.
    private SwapBuffer mPositionBuffer;
    private SwapBuffer mVelocityBuffer;
    private SwapBuffer mLifetimeBuffer;
    private ComputeBuffer mColorBuffer;
    private ComputeBuffer mHaloBuffer;
    private ComputeBuffer mScaleBuffer;
    private ComputeBuffer mTransperancyBuffer;

    private int mMaxParticleCount;
    private int mEmittIndex = 0;

    // Collisons.
    private ComputeBuffer mSphereColliderResultBuffer = null;
    public ComputeBuffer GetSphereColliderResultBuffer() { return mSphereColliderResultBuffer; }

    // Sort.
    private SwapBuffer mSortElementSwapBuffer = null;

    // Emitter.
    private Vector3 mLastPosition = Vector3.zero;

    private float mEmittTimer = 0.0f;

    private float mEmittFrequency = 10.0f; private float mNewEmittFrequency = 10.0f;
    /// <summary>
    /// How many partices to emitt per second.
    /// Default: 10
    /// </summary>
    public float EmittFrequency { get { return mEmittFrequency; } set { mNewEmittFrequency = value; mApply = true; } }

    private float mEmittParticleLifetime = 6.0f; private float mNewmParticleLifetime = 6.0f;
    /// <summary>
    /// How long a particle live in seconds.
    /// Default: 6
    /// </summary>
    public float EmittParticleLifeTime { get { return mEmittParticleLifetime; } set { mNewmParticleLifetime = value; mApply = true; } }

    private bool mEmittInheritVelocity = true;
    /// <summary>
    /// Whether emitted particles inherit velocity from emitter.
    /// Default: true
    /// </summary>
    public bool EmittInheritVelocity { get { return mEmittInheritVelocity; } set { mEmittInheritVelocity = value; } }

    private Vector3 mEmittConstantAcceleration = Vector3.zero;
    /// <summary>
    /// Constant acceleration applyed to particles.
    /// Default: 0,0,0
    /// </summary>
    public Vector3 EmittConstantAcceleration { get { return mEmittConstantAcceleration; } set { mEmittConstantAcceleration = value; } }

    private float mEmittConstantDrag = 1.0f;
    /// <summary>
    /// Constant drag applyed to particles.
    /// Default: 1
    /// </summary>
    public float EmittConstantDrag { get { return mEmittConstantDrag; } set { mEmittConstantDrag = value; } }

    private Vector3 mEmittInitialVelocity = Vector3.zero;
    /// <summary>
    /// Initial velocity of emitted particle.
    /// Default: 0,0,0
    /// </summary>
    public Vector3 EmittInitialVelocity { get { return mEmittInitialVelocity; } set { mEmittInitialVelocity = value; } }

    private ComputeBuffer mColorLifetimePointsBuffer = null;
    private Vector4[] mColorLifetimePoints = new Vector4[] { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 1) };
    private Vector4[] mNewColorLifetimePoints = new Vector4[] { new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 1) };
    /// <summary>
    /// colorarray of emitted particle.
    /// Default: 1,1,1,0 to 0,1,0,1
    /// </summary>
    public Vector4[] ColorLifetimePoints
    {
        get
        {
            return mColorLifetimePoints;
        }
        set
        {
            Debug.Assert(value.Length >= 2);
            Debug.Assert(value[0].w == 0);
            Debug.Assert(value[value.Length - 1].w == 1);
            for (int i = 1; i < value.Length; ++i)
            {
                Debug.Assert(value[i - 1].w < value[i].w);
            }
            mNewColorLifetimePoints = value;
            mApply = true;
        }
    }


    private ComputeBuffer mHaloLifetimePointsBuffer = null;
    private Vector4[] mHaloLifetimePoints = new Vector4[] { new Vector4(1, 1, 1, 0), new Vector4(0, 1, 0, 1) };
    private Vector4[] mNewHaloLifetimePoints = new Vector4[] { new Vector4(1, 1, 1, 0), new Vector4(0, 1, 0, 1) };
    /// <summary>
    /// Bordercolorarray of emitted particle.
    /// Default: 1,1,1,0 to 0,1,0,1
    /// </summary>
    public Vector4[] HaloLifetimePoints
    {
        get
        {
            return mHaloLifetimePoints;
        }
        set
        {
            Debug.Assert(value.Length >= 2);
            Debug.Assert(value[0].w == 0);
            Debug.Assert(value[value.Length - 1].w == 1);
            for (int i = 1; i < value.Length; ++i)
            {
                Debug.Assert(value[i - 1].w < value[i].w);
            }
            mNewHaloLifetimePoints = value;
            mApply = true;
        }
    }


    private ComputeBuffer mScaleLifetimePointsBuffer = null;
    private Vector4[] mScaleLifetimePoints = new Vector4[] { new Vector4(0.1f, 0.1f, 0, 0), new Vector4(0.1f, 0.1f, 0, 1) };
    private Vector4[] mNewScaleLifetimePoints = new Vector4[] { new Vector4(0.1f, 0.1f, 0, 0), new Vector4(0.1f, 0.1f, 0, 1) };
    /// <summary>
    /// scale of emitted particle.
    /// Default: 1,1,1,0 to 0,1,0,1
    /// </summary>
    public Vector4[] ScaleLifetimePoints
    {
        get
        {
            return mScaleLifetimePoints;
        }
        set
        {
            Debug.Assert(value.Length >= 2);
            Debug.Assert(value[0].w == 0);
            Debug.Assert(value[value.Length - 1].w == 1);
            for (int i = 1; i < value.Length; ++i)
            {
                Debug.Assert(value[i - 1].w < value[i].w);
            }
            mNewScaleLifetimePoints = value;
            mApply = true;
        }
    }

    private ComputeBuffer mTransparencyLifetimePointsBuffer = null;
    private Vector4[] mTransparencyLifetimePoints = new Vector4[] { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 1) };
    private Vector4[] mNewTransparencyLifetimePoints = new Vector4[] { new Vector4(1.0f, 0, 0, 0), new Vector4(1.0f, 0, 0, 1) };
    /// <summary>
    /// transparency of emitted particle.
    /// Default: opaque
    /// </summary>
    public Vector4[] TransparencyLifetimePoints
    {
        get
        {
            return mTransparencyLifetimePoints;
        }
        set
        {
            Debug.Assert(value.Length >= 2);
            Debug.Assert(value[0].w == 0);
            Debug.Assert(value[value.Length - 1].w == 1);
            for (int i = 1; i < value.Length; ++i)
            {
                Debug.Assert(value[i - 1].w < value[i].w);
            }
            mNewTransparencyLifetimePoints = value;
            mApply = true;
        }
    }

    private bool mActive = true;
    /// <summary>
    /// Whether emitter should emitt particles.
    /// Default: true
    /// </summary>
    public bool Active { get { return mActive; } set { mActive = value; } }

    // APPLY.
    private bool mApply = true;
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
        mEmittParticleLifetime = mNewmParticleLifetime;
        mMaxParticleCount = Mathf.NextPowerOfTwo((int)Mathf.Ceil(mEmittFrequency * mEmittParticleLifetime));
        mLastPosition = transform.position;
        mColorLifetimePoints = mNewColorLifetimePoints;
        mHaloLifetimePoints = mNewHaloLifetimePoints;
        mScaleLifetimePoints = mNewScaleLifetimePoints;
        mTransparencyLifetimePoints = mNewTransparencyLifetimePoints;

        // BUFFERS.
        mPositionBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);
        mVelocityBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);
        mLifetimeBuffer = new SwapBuffer(2, mMaxParticleCount, sizeof(float) * 4);

        mColorBuffer = new ComputeBuffer(mMaxParticleCount, sizeof(float) * 4);
        mHaloBuffer = new ComputeBuffer(mMaxParticleCount, sizeof(float) * 4);
        mScaleBuffer = new ComputeBuffer(mMaxParticleCount, sizeof(float) * 4);
        mTransperancyBuffer = new ComputeBuffer(mMaxParticleCount, sizeof(float) * 4);

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

        // MATERIAL.
        mRenderMaterial = new Material(Resources.Load<Shader>("GPUParticleSystem/Shaders/GPUParticleRenderShader"));


        //LIFETIME POINT BUFFERS
        // ------- Color ------
        mColorLifetimePointsBuffer = new ComputeBuffer(mColorLifetimePoints.Length, sizeof(float) * 4);

        float[] colorLifetimeArr = new float[mColorLifetimePoints.Length * 4];
        for (int i = 0, j = 0; i < mColorLifetimePoints.Length; ++i, j += 4)
        {
            colorLifetimeArr[j] = mColorLifetimePoints[i].x;
            colorLifetimeArr[j + 1] = mColorLifetimePoints[i].y;
            colorLifetimeArr[j + 2] = mColorLifetimePoints[i].z;
            colorLifetimeArr[j + 3] = mColorLifetimePoints[i].w;
        }
        mColorLifetimePointsBuffer.SetData(colorLifetimeArr);

        // ------- Halo -------
        mHaloLifetimePointsBuffer = new ComputeBuffer(mHaloLifetimePoints.Length, sizeof(float) * 4);

        float[] haloLifetimeArr = new float[mHaloLifetimePoints.Length * 4];
        for (int i = 0, j = 0; i < mHaloLifetimePoints.Length; ++i, j += 4)
        {
            haloLifetimeArr[j] = mHaloLifetimePoints[i].x;
            haloLifetimeArr[j + 1] = mHaloLifetimePoints[i].y;
            haloLifetimeArr[j + 2] = mHaloLifetimePoints[i].z;
            haloLifetimeArr[j + 3] = mHaloLifetimePoints[i].w;
        }
        mHaloLifetimePointsBuffer.SetData(haloLifetimeArr);

        // ------ Scale ------
        mScaleLifetimePointsBuffer = new ComputeBuffer(mScaleLifetimePoints.Length, sizeof(float) * 4);

        float[] scaleLifetimeArr = new float[mScaleLifetimePoints.Length * 4];
        for (int i = 0, j = 0; i < mScaleLifetimePoints.Length; ++i, j += 4)
        {
            scaleLifetimeArr[j] = mScaleLifetimePoints[i].x;
            scaleLifetimeArr[j + 1] = mScaleLifetimePoints[i].y;
            scaleLifetimeArr[j + 2] = mScaleLifetimePoints[i].z;
            scaleLifetimeArr[j + 3] = mScaleLifetimePoints[i].w;
        }
        mScaleLifetimePointsBuffer.SetData(scaleLifetimeArr);

        // ------ Transparency -
        mTransparencyLifetimePointsBuffer = new ComputeBuffer(mTransparencyLifetimePoints.Length, sizeof(float) * 4);

        float[] transparencyLifetimeArr = new float[mTransparencyLifetimePoints.Length * 4];
        for (int i = 0, j = 0; i < mTransparencyLifetimePoints.Length; ++i, j += 4)
        {
            transparencyLifetimeArr[j] = mTransparencyLifetimePoints[i].x;
            transparencyLifetimeArr[j + 1] = mTransparencyLifetimePoints[i].y;
            transparencyLifetimeArr[j + 2] = mTransparencyLifetimePoints[i].z;
            transparencyLifetimeArr[j + 3] = mTransparencyLifetimePoints[i].w;
        }
        mTransparencyLifetimePointsBuffer.SetData(transparencyLifetimeArr);

        // COLLISION.
        mSphereColliderResultBuffer = new ComputeBuffer(sMaxSphereColliderCount, sizeof(int));

        // SORTELEMENT.
        mSortElementSwapBuffer = new SwapBuffer(2, mMaxParticleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SortElement)));

    }

    // DEINIT.
    private void DeInitSystem()
    {
        mPositionBuffer.Release();
        mVelocityBuffer.Release();
        mLifetimeBuffer.Release();

        mColorBuffer.Release();
        mHaloBuffer.Release();
        mScaleBuffer.Release();
        mTransperancyBuffer.Release();

        mColorLifetimePointsBuffer.Release();
        mHaloLifetimePointsBuffer.Release();
        mScaleLifetimePointsBuffer.Release();
        mTransparencyLifetimePointsBuffer.Release();

        mRenderMaterial = null;

        mSphereColliderResultBuffer.Release();

        mSortElementSwapBuffer.Release();
    }

    // EMITT UPDATE.
    private void EmittUppdate()
    {
        // Update timer.
        mEmittTimer += Time.deltaTime;

        int emittCount = (int)(mEmittFrequency * mEmittTimer);

        if (emittCount == 0) return;

        mEmittTimer -= emittCount * 1.0f / mEmittFrequency;

        Vector3 emitterVelocity = transform.position - mLastPosition;

        for (int i = 0; i < emittCount; ++i)
        {
            // BIND PARTICLE BUFFERS.
            sComputeShader.SetBuffer(sKernelEmitt, "gPositionBuffer", mPositionBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gVelocityBuffer", mVelocityBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gLifetimeBuffer", mLifetimeBuffer.GetOutputBuffer());

            // Inherit velocity from emitter if true.
            Vector3 velocity = (emitterVelocity / Time.deltaTime) * (mEmittInheritVelocity ? 1 : 0) + mEmittInitialVelocity;

            
            Vector3 newInitPos = mLastPosition;
            float delta = i / emittCount;
            newInitPos += emitterVelocity * delta;

            // EMITT INFO.
            sComputeShader.SetInt("gEmittIndex", mEmittIndex);
            sComputeShader.SetFloats("gPosition", new float[] { newInitPos.x, newInitPos.y, newInitPos.z });
            sComputeShader.SetFloats("gVelocity", new float[] { velocity.x, velocity.y, velocity.z });
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
                sComputeShader.SetFloats("gEmittMeshScale", new float[] { transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z });
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
        mLifetimeBuffer.Swap();

        // BIND INPUT BUFFERS.
        sComputeShader.SetBuffer(sKernelUpdate, "gPositionIN", mPositionBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gVelocityIN", mVelocityBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gLifetimeIN", mLifetimeBuffer.GetInputBuffer());

        // BIND OUTPUT BUFFERS.
        sComputeShader.SetBuffer(sKernelUpdate, "gPositionOUT", mPositionBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gVelocityOUT", mVelocityBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gLifetimeOUT", mLifetimeBuffer.GetOutputBuffer());

        sComputeShader.SetBuffer(sKernelUpdate, "gColorOUT", mColorBuffer);
        sComputeShader.SetBuffer(sKernelUpdate, "gHaloOUT", mHaloBuffer);
        sComputeShader.SetBuffer(sKernelUpdate, "gScaleOUT", mScaleBuffer);
        sComputeShader.SetBuffer(sKernelUpdate, "gTransparencyOUT", mTransperancyBuffer);

        // BIND VALUE OF LIFETIME BUFFERS.
        sComputeShader.SetInt("gColorLifetimeCount", mColorLifetimePoints.Length);
        sComputeShader.SetBuffer(sKernelUpdate, "gColorLifetimeBuffer", mColorLifetimePointsBuffer);
        sComputeShader.SetInt("gHaloLifetimeCount", mHaloLifetimePoints.Length);
        sComputeShader.SetBuffer(sKernelUpdate, "gHaloLifetimeBuffer", mHaloLifetimePointsBuffer);
        sComputeShader.SetInt("gScaleLifetimeCount", mScaleLifetimePoints.Length);
        sComputeShader.SetBuffer(sKernelUpdate, "gScaleLifetimeBuffer", mScaleLifetimePointsBuffer);
        sComputeShader.SetInt("gTransparencyLifetimeCount", mTransparencyLifetimePoints.Length);
        sComputeShader.SetBuffer(sKernelUpdate, "gTransparencyLifetimeBuffer", mTransparencyLifetimePointsBuffer);

        // SET META DATA.
        sComputeShader.SetInt("gMaxParticleCount", mMaxParticleCount);
        sComputeShader.SetFloat("gDeltaTime", Time.deltaTime);

        sComputeShader.SetFloats("gConstantAcceleration", new float[] {mEmittConstantAcceleration.x, mEmittConstantAcceleration.y, mEmittConstantAcceleration.z });
        sComputeShader.SetFloat("gConstantDrag", mEmittConstantDrag);

        // ACCELERATOR.
        Dictionary<GPUParticleAttractor, GPUParticleAttractor> attractorDictionary = GPUParticleAttractor.GetGPUParticleAttractorDictionary();
        if (attractorDictionary == null)
        {
            sComputeShader.SetInt("gAttractorCount", 0);
        }
        else
        {
            Debug.Assert(attractorDictionary.Count < sMaxAttractorCount);

            float[] attractorArray = new float[attractorDictionary.Count * 4];
            int i = 0;
            foreach (KeyValuePair<GPUParticleAttractor, GPUParticleAttractor> it in attractorDictionary)
            {
                GPUParticleAttractor attractor = it.Value;

                attractorArray[i++] = attractor.transform.position.x;
                attractorArray[i++] = attractor.transform.position.y;
                attractorArray[i++] = attractor.transform.position.z;
                attractorArray[i++] = attractor.Power;
            }
            sGPUParticleAttractorBuffer.SetData(attractorArray);

            sComputeShader.SetInt("gAttractorCount", attractorDictionary.Count);
            sComputeShader.SetBuffer(sKernelUpdate, "gAttractorBuffer", sGPUParticleAttractorBuffer);
        }

        // Vector Fields.
        Dictionary<GPUParticleVectorField, GPUParticleVectorField> vectorFieldDictionary = GPUParticleVectorField.GetGPUParticleAttractorDictionary();
        if (vectorFieldDictionary == null)
        {
            sComputeShader.SetInt("gVectorFieldCount", 0);
        }
        else
        {
            Debug.Assert(vectorFieldDictionary.Count < sMaxVectorFieldCount);

            float[] vectorFieldArray = new float[vectorFieldDictionary.Count * 8];
            int i = 0;
            foreach (KeyValuePair<GPUParticleVectorField, GPUParticleVectorField> it in vectorFieldDictionary)
            {
                GPUParticleVectorField vectorField = it.Value;
                float scale = Mathf.Max(Mathf.Max(vectorField.transform.lossyScale.x, vectorField.transform.lossyScale.y), vectorField.transform.lossyScale.z);
                Vector3 vector = vectorField.RelativeVectorField ? vectorField.VectorRelative : vectorField.Vector;

                vectorFieldArray[i++] = vectorField.transform.position.x;
                vectorFieldArray[i++] = vectorField.transform.position.y;
                vectorFieldArray[i++] = vectorField.transform.position.z;
                vectorFieldArray[i++] = scale * vectorField.Radius;
                vectorFieldArray[i++] = vector.x;
                vectorFieldArray[i++] = vector.y;
                vectorFieldArray[i++] = vector.z;
                vectorFieldArray[i++] = 0.0f;

            }
            sGPUParticleVectorFieldBuffer.SetData(vectorFieldArray);

            sComputeShader.SetInt("gVectorFieldCount", vectorFieldDictionary.Count);
            sComputeShader.SetBuffer(sKernelUpdate, "gVectorFieldBuffer", sGPUParticleVectorFieldBuffer);
        }

        // SPHERE COLLIDER.
        List<GPUParticleSphereCollider> sphereColliderList = GPUParticleSphereCollider.GetGPUParticleSphereColliderList();
        if (sphereColliderList == null)
        {
            sComputeShader.SetInt("gSphereColliderCount", 0);
        }
        else
        {
            Debug.Assert(sphereColliderList.Count < sMaxSphereColliderCount);

            // Reset result buffer.
            int[] resetArray = new int[sphereColliderList.Count];
            for (int i = 0; i < sphereColliderList.Count; ++i)
            {
                resetArray[i] = 0;
            }
            mSphereColliderResultBuffer.SetData(resetArray);

            // Update sphere collider buffer.
            float[] sphereColliderArray = new float[sphereColliderList.Count * 4];
            for (int i = 0, j = 0; i < sphereColliderList.Count; ++i)
            {
                GPUParticleSphereCollider sphereCollider = sphereColliderList[i];

                float scale = Mathf.Max(Mathf.Max(sphereCollider.transform.lossyScale.x, sphereCollider.transform.lossyScale.y), sphereCollider.transform.lossyScale.z);

                sphereColliderArray[j++] = sphereCollider.transform.position.x;
                sphereColliderArray[j++] = sphereCollider.transform.position.y;
                sphereColliderArray[j++] = sphereCollider.transform.position.z;
                sphereColliderArray[j++] = scale * sphereCollider.Radius;
            }

            sGPUParticleSphereColliderBuffer.SetData(sphereColliderArray);

            sComputeShader.SetInt("gSphereColliderCount", sphereColliderList.Count);
            sComputeShader.SetBuffer(sKernelUpdate, "gSphereColliderBuffer", sGPUParticleSphereColliderBuffer);
            sComputeShader.SetBuffer(sKernelUpdate, "gSphereColliderResultBufferWRITE", mSphereColliderResultBuffer);
        }

        // DISPATCH.
        sComputeShader.Dispatch(sKernelUpdate, (int)Mathf.Ceil(mMaxParticleCount / 64.0f), 1, 1);
    }


    // SORT.
    private void Sort()
    {
        Debug.Assert(Mathf.IsPowerOfTwo(mMaxParticleCount));

        // INIT/UPDATE SORT BUFFER. TODO COMBINE ALL SYSTEMS.
        Vector3 cForward = Camera.main.transform.forward;
        sComputeShader.SetFloats("gCameraForward", new float[] { cForward.x, cForward.y, cForward.z });
        sComputeShader.SetInt("gMaxParticleCount", mMaxParticleCount);
        sComputeShader.SetInt("gOffsetIndex", 0);
        sComputeShader.SetBuffer(sKernelInitSort, "gSortElementBufferOUT", mSortElementSwapBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelInitSort, "gPositionIN", mPositionBuffer.GetOutputBuffer());
        sComputeShader.Dispatch(sKernelInitSort, (int)Mathf.Ceil(mMaxParticleCount / 64.0f), 1, 1);

        // DISPATCH SORT BUFFER.
        for (int k = 2; k <= mMaxParticleCount; k <<= 1) // Major steps.
        {
            for (int j = k >> 1; j > 0; j = j >> 1) // Minor steps.
            {
                // SWAP AND BIND BUFFERS.
                mSortElementSwapBuffer.Swap();
                sComputeShader.SetBuffer(sKernelSort, "gSortElementBufferIN", mSortElementSwapBuffer.GetInputBuffer());
                sComputeShader.SetBuffer(sKernelSort, "gSortElementBufferOUT", mSortElementSwapBuffer.GetOutputBuffer());

                sComputeShader.SetInt("gK", k);
                sComputeShader.SetInt("gJ", j);
                sComputeShader.SetInt("gMaxParticleCount", mMaxParticleCount);

                // DISPATCH.
                sComputeShader.Dispatch(sKernelSort, (int)Mathf.Ceil(mMaxParticleCount / 64.0f), 1, 1);
            }
        }
    }

    // RENDER.
    private void RenderSystem()
    {
        mRenderMaterial.SetPass(0);

        // BIND BUFFERS.
        mRenderMaterial.SetBuffer("gPosition", mPositionBuffer.GetOutputBuffer());
        mRenderMaterial.SetBuffer("gVelocity", mVelocityBuffer.GetOutputBuffer());
        mRenderMaterial.SetBuffer("gLifetime", mLifetimeBuffer.GetOutputBuffer());

        mRenderMaterial.SetBuffer("gColor", mColorBuffer);
        mRenderMaterial.SetBuffer("gHalo", mHaloBuffer);
        mRenderMaterial.SetBuffer("gScale", mScaleBuffer);
        mRenderMaterial.SetBuffer("gTransparency", mTransperancyBuffer);

        // BIND SORTED BUFFER AND USE AS INDEX BUFFER TO RENDER BACK TO FRONT.
        mRenderMaterial.SetBuffer("gSortedParticleIndexBuffer", mSortElementSwapBuffer.GetOutputBuffer());

        // DRAW.
        Graphics.DrawProcedural(MeshTopology.Points, mMaxParticleCount, 1);
    }

    public int Count { get { return mMaxParticleCount; } }

    // MONOBEHAVIOUR.
    private void Awake()
    {
        if (sGPUParticleSystemDictionary == null) StartUp();
        InitSystem();
        sGPUParticleSystemDictionary[this] = this;
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        // Used to make static function get called once in LateUpdate();
        sLateUpdate = true;

        // Update buffers if needed (updated).
        if (mApply)
            Apply();

        // Emitt new particles this frame if active.
        if (mActive)
            EmittUppdate();

        // Update particles this frame.
        UpdateSystem();

        // Update last position.
        mLastPosition = transform.position;
    }

    private void LateUpdate()
    {
        // Enter once per frame to get data from GPU to CPU.
        if (sLateUpdate)
        {
            sLateUpdate = false;
            // Get collisions from this frame.
            FetchCollisionResults();
        }
    }

    // MONOBEHAVIOUR.
    private void OnRenderObject()
    {
        // Sort particles.
        Sort();

        // Render this frame.
        RenderSystem();
    }

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitSystem();
        sGPUParticleSystemDictionary.Remove(this);
        if (sGPUParticleSystemDictionary.Count == 0) Shutdown();
    }

    /// --- MEMBERS --- ///
    
}
