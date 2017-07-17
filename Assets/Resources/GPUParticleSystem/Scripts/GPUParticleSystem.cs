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

    /// --- STRUCTS --- ///


    /// +++ STATIC +++ ///

    static Dictionary<GPUParticleSystem, GPUParticleSystem> sGPUParticleSystemDictionary = null;

    static ComputeShader sComputeShader = null;
    static int sKernelUpdate = -1;
    static int sKernelEmitt = -1;
    static int sKernelResult = -1;
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
        Dictionary<GPUParticleSphereCollider, GPUParticleSphereCollider> sphereColliderDictionary = GPUParticleSphereCollider.GetGPUParticleSphereColliderDictionary();

        // Return early if null or zero GPUParticleSphereCollider.
        if (sphereColliderDictionary == null) return;
        if (sphereColliderDictionary.Count == 0) return;

        Debug.Assert(sphereColliderDictionary.Count < sMaxSphereColliderCount);

        bool initZero = true;
        foreach (KeyValuePair<GPUParticleSystem, GPUParticleSystem> it in sGPUParticleSystemDictionary)
        {
            GPUParticleSystem system = it.Value;

            sGPUColliderResultSwapBuffer.Swap();
            sComputeShader.SetBuffer(sKernelResult, "gGPUColliderResultBufferIN", sGPUColliderResultSwapBuffer.GetInputBuffer());
            sComputeShader.SetBuffer(sKernelResult, "gGPUColliderResultBufferOUT", sGPUColliderResultSwapBuffer.GetOutputBuffer());

            sComputeShader.SetInt("gGPUColliderCount", sphereColliderDictionary.Count);
            sComputeShader.SetBuffer(sKernelResult, "gSphereColliderResultBufferREAD", system.GetSphereColliderResultBuffer());

            sComputeShader.SetBool("gInitZero", initZero);
            initZero = false;

            // DISPATCH.
            sComputeShader.Dispatch(sKernelResult, (int)Mathf.Ceil(sMaxSphereColliderCount / 64.0f), 1, 1);
        }

        // GET DATA FRPM GPU TO CPU.
        int[] collisionData = new int[sphereColliderDictionary.Count];
        sGPUColliderResultSwapBuffer.GetOutputBuffer().GetData(collisionData);

        // UPDATE COLLIDERS.
        foreach (KeyValuePair<GPUParticleSphereCollider, GPUParticleSphereCollider> it in GPUParticleSphereCollider.GetGPUParticleSphereColliderDictionary())
        {
            GPUParticleSphereCollider collider = it.Value;
            collider.SetCollisionsThisFrame(collisionData[collider.GetIndex]);
        }
        Debug.Log(collisionData[0]);
    }

    /// --- STATIC --- ///

    /// +++ MEMBERS +++ ///

    // Material.
    private Material mRenderMaterial = null;

    // Particle.
    private SwapBuffer mPositionBuffer;
    private SwapBuffer mVelocityBuffer;
    private SwapBuffer mScaleBuffer;
    private SwapBuffer mColorBuffer;
    private SwapBuffer mLifetimeBuffer;

    private int mMaxParticleCount;
    private int mEmittIndex = 0;

    // Collisons.
    private ComputeBuffer mSphereColliderResultBuffer = null;
    public ComputeBuffer GetSphereColliderResultBuffer() { return mSphereColliderResultBuffer; }

    // Emitter.
    private Vector3 mLastPosition = Vector3.zero;

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

    private bool mEmittInheritVelocity = true;
    /// <summary>
    /// Whether emitted particles inherit velocity from emitter.
    /// Default: true
    /// </summary>
    public bool EmittInheritVelocity { get { return mEmittInheritVelocity; } set { mEmittInheritVelocity = value;} }

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

    private Vector2 mEmittInitialScale = Vector2.one;
    /// <summary>
    /// Initial scale of emitted particle.
    /// Default: 1,1
    /// </summary>
    public Vector2 EmittInitialScale { get { return mEmittInitialScale; } set { mEmittInitialScale = value; } }

    private Vector3 mEmittInitialColor = Vector3.one;
    /// <summary>
    /// Initial color of emitted particle.
    /// Default: 1,1,1
    /// </summary>
    public Vector3 EmittInitialColor { get { return mEmittInitialColor; } set { mEmittInitialColor = value; } }

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
        mMaxParticleCount = (int)Mathf.Ceil(mEmittFrequency * mEmittParticleLifetime);
        mLastPosition = transform.position;

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

        // MATERIAL.
        mRenderMaterial = new Material(Resources.Load<Shader>("GPUParticleSystem/Shaders/GPUParticleRenderShader"));

        // COLLISION.
        mSphereColliderResultBuffer = new ComputeBuffer(1, sizeof(int));

    }

    // DEINIT.
    private void DeInitSystem()
    {
        mPositionBuffer.Release();
        mVelocityBuffer.Release();
        mScaleBuffer.Release();
        mColorBuffer.Release();
        mLifetimeBuffer.Release();

        mRenderMaterial = null;

        mSphereColliderResultBuffer.Release();
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

            // Inherit velocity from emitter if true.
            Vector3 velocity = ((transform.position - mLastPosition) / Time.deltaTime) * (mEmittInheritVelocity ? 1 : 0) + mEmittInitialVelocity;

            // EMITT INFO.
            sComputeShader.SetInt("gEmittIndex", mEmittIndex);
            sComputeShader.SetFloats("gPosition", new float[] { transform.position.x, transform.position.y, transform.position.z });
            sComputeShader.SetFloats("gVelocity", new float[] { velocity.x, velocity.y, velocity.z });
            sComputeShader.SetFloats("gScale", new float[] { mEmittInitialScale.x, mEmittInitialScale.y });
            sComputeShader.SetFloats("gColor", new float[] { mEmittInitialColor.x, mEmittInitialColor.y, mEmittInitialColor.z });
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
                float scale = Mathf.Max(Mathf.Max(vectorField.transform.localScale.x, vectorField.transform.localScale.y), vectorField.transform.localScale.z);
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
        Dictionary<GPUParticleSphereCollider, GPUParticleSphereCollider> sphereColliderDictionary = GPUParticleSphereCollider.GetGPUParticleSphereColliderDictionary();
        if (sphereColliderDictionary == null)
        {
            sComputeShader.SetInt("gSphereColliderCount", 0);
        }
        else
        {
            Debug.Assert(sphereColliderDictionary.Count < sMaxSphereColliderCount);

            float[] sphereColliderArray = new float[sphereColliderDictionary.Count * 4];
            int i = 0;
            foreach (KeyValuePair<GPUParticleSphereCollider, GPUParticleSphereCollider> it in sphereColliderDictionary)
            {
                GPUParticleSphereCollider sphereCollider = it.Value;
                float scale = Mathf.Max(Mathf.Max(sphereCollider.transform.localScale.x, sphereCollider.transform.localScale.y), sphereCollider.transform.localScale.z);

                sphereColliderArray[i++] = sphereCollider.transform.position.x;
                sphereColliderArray[i++] = sphereCollider.transform.position.y;
                sphereColliderArray[i++] = sphereCollider.transform.position.z;
                sphereColliderArray[i++] = scale;
            }
            sGPUParticleSphereColliderBuffer.SetData(sphereColliderArray);

            sComputeShader.SetInt("gSphereColliderCount", sphereColliderDictionary.Count);
            sComputeShader.SetBuffer(sKernelUpdate, "gSphereColliderBuffer", sGPUParticleSphereColliderBuffer);
            sComputeShader.SetBuffer(sKernelUpdate, "gSphereColliderResultBufferWRITE", mSphereColliderResultBuffer);
        }

        // DISPATCH.
        sComputeShader.Dispatch(sKernelUpdate, (int)Mathf.Ceil(mMaxParticleCount / 64.0f), 1, 1);
    }

    // RENDER.
    private void RenderSystem()
    {
        mRenderMaterial.SetPass(0);

        // BIND BUFFERS.
        mRenderMaterial.SetBuffer("gPosition", mPositionBuffer.GetOutputBuffer());
        mRenderMaterial.SetBuffer("gVelocity", mVelocityBuffer.GetOutputBuffer());
        mRenderMaterial.SetBuffer("gScale", mScaleBuffer.GetOutputBuffer());
        mRenderMaterial.SetBuffer("gColor", mColorBuffer.GetOutputBuffer());
        mRenderMaterial.SetBuffer("gLifetime", mLifetimeBuffer.GetOutputBuffer());

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
