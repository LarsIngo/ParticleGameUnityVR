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
        int mBufferCount;
        int mStride;

        public SwapBuffer(int bufferCount, int count, int stride)
        {
            mBufferIndex = 0;
            mBufferCount = bufferCount;
            mStride = stride;
            Init(count, stride);
        }
        private void Init(int count, int stride)
        {
            mBufferArray = new ComputeBuffer[mBufferCount];
            for (int i = 0; i < mBufferCount; ++i)
                mBufferArray[i] = new ComputeBuffer(count, stride, ComputeBufferType.Default);
        }
        public void Release() { for (int i = 0; i < mBufferCount; ++i) mBufferArray[i].Release(); }
        public ComputeBuffer GetInputBuffer() { return mBufferArray[mBufferIndex]; }
        public ComputeBuffer GetOutputBuffer() { return mBufferArray[(mBufferIndex + 1) % mBufferCount]; }
        public void Swap() { mBufferIndex = (mBufferIndex + 1) % mBufferCount; }
        public void Resize(int count) { Release(); Init(count, mStride); }
    }

    private struct EmittMeshInfo
    {
        public ComputeBuffer mVertexBuffer;
        public int mVertexCount;
        public ComputeBuffer mIndexBuffer;
        public int mIndexCount;
        public ComputeBuffer mMatrixBuffer;
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
    static int sKernelMergeInitSort = -1;
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

    // Merge Particle Buffers.
    static SwapBuffer sMergedPositionBuffer = null;
    static SwapBuffer sMergedVelocityBuffer = null;
    static SwapBuffer sMergedLifetimeBuffer = null;
    static SwapBuffer sMergedColorBuffer = null;
    static SwapBuffer sMergedHaloBuffer = null;
    static SwapBuffer sMergedScaleBuffer = null;
    static SwapBuffer sMergedTransperancyBuffer = null;
    static int sMergedParticleCount = 0;
    static int sTotalParticleCount = 0;
    static SwapBuffer sSortElementSwapBuffer = null;

    // Material.
    static Material sRenderMaterial = null;

    // Flags to make sure we only do some functins onces per frame.
    static bool sLateUpdate;
    static Dictionary<Camera, Dictionary<Camera.MonoOrStereoscopicEye, Camera.MonoOrStereoscopicEye>> sRenderedCameraDictionary = new Dictionary<Camera, Dictionary<Camera.MonoOrStereoscopicEye, Camera.MonoOrStereoscopicEye>>();

    // STARTUP.
    static void StartUp()
    {

        sGPUParticleSystemDictionary = new Dictionary<GPUParticleSystem, GPUParticleSystem>();

        sComputeShader = Resources.Load<ComputeShader>("GPUParticleSystem/Shaders/GPUParticleComputeShader");
        sKernelUpdate = sComputeShader.FindKernel("UPDATE");
        sKernelEmitt = sComputeShader.FindKernel("EMITT");
        sKernelResult = sComputeShader.FindKernel("RESULT");
        sKernelMergeInitSort = sComputeShader.FindKernel("MERGE_INITSORT");
        sKernelSort = sComputeShader.FindKernel("SORT");
        sEmittMeshInfoDictionary = new Dictionary<Mesh, EmittMeshInfo>();
        sGPUParticleAttractorBuffer = new ComputeBuffer(sMaxAttractorCount, sizeof(float) * 8);
        sGPUParticleVectorFieldBuffer = new ComputeBuffer(sMaxVectorFieldCount, sizeof(float) * 8);
        sGPUParticleSphereColliderBuffer = new ComputeBuffer(sMaxSphereColliderCount, sizeof(float) * 4);
        sGPUColliderResultSwapBuffer = new SwapBuffer(2, sMaxGPUColliderCount, sizeof(int));

        sMergedPositionBuffer = new SwapBuffer(2, 1, sizeof(float) * 4);
        sMergedVelocityBuffer = new SwapBuffer(2, 1, sizeof(float) * 4);
        sMergedLifetimeBuffer = new SwapBuffer(2, 1, sizeof(float) * 4);

        sMergedColorBuffer = new SwapBuffer(1, 1, sizeof(float) * 4);
        sMergedHaloBuffer = new SwapBuffer(1, 1, sizeof(float) * 4);
        sMergedScaleBuffer = new SwapBuffer(1, 1, sizeof(float) * 4);
        sMergedTransperancyBuffer = new SwapBuffer(1, 1, sizeof(float) * 4);

        sSortElementSwapBuffer = new SwapBuffer(2, 1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(SortElement)));

        // MATERIAL.
        sRenderMaterial = new Material(Resources.Load<Shader>("GPUParticleSystem/Shaders/GPUParticleRenderShader"));
    }

    // SHUTDOWN.
    static void Shutdown()
    {
        sGPUParticleSystemDictionary.Clear();
        sGPUParticleSystemDictionary = null;

        sComputeShader = null;
        sKernelUpdate = -1;
        sKernelEmitt = -1;
        sKernelResult = -1;
        sKernelMergeInitSort = -1;
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

        sMergedPositionBuffer.Release();
        sMergedVelocityBuffer.Release();
        sMergedLifetimeBuffer.Release();

        sMergedColorBuffer.Release();
        sMergedHaloBuffer.Release();
        sMergedScaleBuffer.Release();
        sMergedTransperancyBuffer.Release();

        sSortElementSwapBuffer.Release();

        sRenderMaterial = null;
    }

    /// <summary>
    /// Kills all living particles.
    /// </summary>
    public static void KillAllParticles()
    {
        if (sGPUParticleSystemDictionary == null) return;

        foreach (KeyValuePair<GPUParticleSystem, GPUParticleSystem> it in sGPUParticleSystemDictionary)
        {
            GPUParticleSystem system = it.Value;

            float[] systemData = new float[system.mMaxParticleCount * 4];
            for (int i = 0; i < systemData.GetLength(0); ++i)
                systemData[i] = -100.0f;

            system.mLifetimeBuffer.GetInputBuffer().SetData(systemData);
            system.mLifetimeBuffer.GetOutputBuffer().SetData(systemData);

            for (int i = 0; i < systemData.GetLength(0); ++i)
                systemData[i] = float.MaxValue;

            system.mPositionBuffer.GetInputBuffer().SetData(systemData);
            system.mPositionBuffer.GetOutputBuffer().SetData(systemData);

            int[] r = new int[sMaxGPUColliderCount];
            for (int i = 0; i < r.GetLength(0); ++i)
                r[i] = 0;
            system.mSphereColliderResultBuffer.SetData(r);
        }

        // TODO: Might not need to reset all this data!!

        float[] mergedData = new float[sMergedParticleCount * 4];
        for (int i = 0; i < mergedData.GetLength(0); ++i)
            mergedData[i] = -100.0f;

        sMergedLifetimeBuffer.GetInputBuffer().SetData(mergedData);
        sMergedLifetimeBuffer.GetOutputBuffer().SetData(mergedData);

        for (int i = 0; i < mergedData.GetLength(0); ++i)
            mergedData[i] = float.MaxValue;

        sMergedPositionBuffer.GetInputBuffer().SetData(mergedData);
        sMergedPositionBuffer.GetOutputBuffer().SetData(mergedData);

        int[] data = new int[sMaxGPUColliderCount];
        for (int i = 0; i < data.GetLength(0); ++i)
            data[i] = 0;
        sGPUColliderResultSwapBuffer.GetInputBuffer().SetData(data);
        sGPUColliderResultSwapBuffer.GetOutputBuffer().SetData(data);


    }

    /// --- STATIC --- ///


    /// +++ MEMBERS +++ ///

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

    // Emitter.
    private Vector3 mLastPosition = Vector3.zero;

    private float mEmittTimer = 0.0f;

    private GPUParticleDescriptor mDescriptor = null;

    private ComputeBuffer mColorLifetimePointsBuffer = null;

    private ComputeBuffer mHaloLifetimePointsBuffer = null;

    private ComputeBuffer mScaleLifetimePointsBuffer = null;

    private ComputeBuffer mTransparencyLifetimePointsBuffer = null;

    private bool mActive = true;
    /// <summary>
    /// Whether emitter should emitt particles.
    /// Default: true
    /// </summary>
    public bool Active { get { return mActive; } set { mActive = value; } }

    // APPLY.
    private void Apply() { DeInitSystem(); InitSystem(); }

    private void UpdateMesh()
    {
        // Return early if mesh is null.
        if (mDescriptor.EmittMesh == null) return;

        // Return early if mesh is loaded.
        if (sEmittMeshInfoDictionary.ContainsKey(mDescriptor.EmittMesh)) return;

        Vector3[] vertices = mDescriptor.EmittMesh.vertices;

        int[] indices = mDescriptor.EmittMesh.triangles;

        // Create new emitt mesh info from mesh.
        EmittMeshInfo emittMeshInfo = new EmittMeshInfo();
        emittMeshInfo.mVertexCount = vertices.GetLength(0);
        emittMeshInfo.mVertexBuffer = new ComputeBuffer(vertices.GetLength(0), sizeof(float) * 3);
        emittMeshInfo.mVertexBuffer.SetData(vertices);
        emittMeshInfo.mIndexCount = indices.GetLength(0);
        emittMeshInfo.mIndexBuffer = new ComputeBuffer(indices.GetLength(0), sizeof(int));
        emittMeshInfo.mIndexBuffer.SetData(indices);

        // Add to dictionary.
        sEmittMeshInfoDictionary[mDescriptor.EmittMesh] = emittMeshInfo;
    }

    // INIT.
    private void InitSystem()
    {

        mDescriptor.Update();

        mMaxParticleCount = (int)Mathf.Ceil(mDescriptor.EmittFrequency * mDescriptor.Lifetime);
        mLastPosition = transform.position;

        sTotalParticleCount += mMaxParticleCount;

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
        UpdateMesh();

        //LIFETIME POINT BUFFERS
        // ------- Color ------
        UpdateLifetimeBuffer(out mColorLifetimePointsBuffer, mDescriptor.ColorOverLifetime.Get());

        // ------- Halo -------
        UpdateLifetimeBuffer(out mHaloLifetimePointsBuffer, mDescriptor.HaloOverLifetime.Get());

        // ------ Scale ------
        UpdateLifetimeBuffer(out mScaleLifetimePointsBuffer, mDescriptor.ScaleOverLifetime.Get());

        // ------ Transparency -
        UpdateLifetimeBuffer(out mTransparencyLifetimePointsBuffer, mDescriptor.OpacityOverLifetime.Get());

        // COLLISION.
        mSphereColliderResultBuffer = new ComputeBuffer(sMaxSphereColliderCount, sizeof(int));

    }

    // DEINIT.
    private void DeInitSystem()
    {
        sTotalParticleCount -= mMaxParticleCount;

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

        mSphereColliderResultBuffer.Release();
    }

    // EMITT UPDATE.
    private void EmittUpdate()
    {
        // Update timer.
        mEmittTimer += Time.deltaTime;

        int emittCount = (int)(mDescriptor.EmittFrequency * mEmittTimer);

        if (emittCount == 0) return;

        mEmittTimer -= emittCount * 1.0f / mDescriptor.EmittFrequency;

        Vector3 emitterVelocity = transform.position - mLastPosition;

        for (int i = 0; i < emittCount; ++i)
        {
            // BIND PARTICLE BUFFERS.
            sComputeShader.SetBuffer(sKernelEmitt, "gPositionBuffer", mPositionBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gVelocityBuffer", mVelocityBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelEmitt, "gLifetimeBuffer", mLifetimeBuffer.GetOutputBuffer());

            // Inherit velocity from emitter if true.
            Vector3 velocity = (emitterVelocity / Time.deltaTime) * (mDescriptor.InheritVelocity ? 1 : 0) + mDescriptor.InitialVelocity;
            
            Vector3 newInitPos = mLastPosition;
            float delta = i / emittCount;
            newInitPos += emitterVelocity * delta;

            // EMITT INFO.
            sComputeShader.SetInt("gEmittIndex", mEmittIndex);
            sComputeShader.SetFloats("gPosition", new float[] { newInitPos.x, newInitPos.y, newInitPos.z });
            sComputeShader.SetFloats("gVelocity", new float[] { velocity.x, velocity.y, velocity.z });
            sComputeShader.SetFloats("gLifetime", new float[] { mDescriptor.Lifetime });

            // EMITT MESH.
            if (mDescriptor.EmittMesh == null)
            {
                // Set count to 0.
                sComputeShader.SetInt("gEmittMeshVertexCount", 0);
                sComputeShader.SetInt("gEmittMeshIndexCount", 0);
                sComputeShader.SetInt("gEmittMeshRandomIndex", 0);
            }
            else
            {
                Debug.Assert(sEmittMeshInfoDictionary.ContainsKey(mDescriptor.EmittMesh));

                EmittMeshInfo emittMeshInfo = sEmittMeshInfoDictionary[mDescriptor.EmittMesh];

                // Set index and vertex buffer.
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
        sComputeShader.SetInt("gColorLifetimeCount", mDescriptor.ColorOverLifetime.Length());
        sComputeShader.SetBuffer(sKernelUpdate, "gColorLifetimeBuffer", mColorLifetimePointsBuffer);
        sComputeShader.SetInt("gHaloLifetimeCount", mDescriptor.HaloOverLifetime.Length());
        sComputeShader.SetBuffer(sKernelUpdate, "gHaloLifetimeBuffer", mHaloLifetimePointsBuffer);
        sComputeShader.SetInt("gScaleLifetimeCount", mDescriptor.ScaleOverLifetime.Length());
        sComputeShader.SetBuffer(sKernelUpdate, "gScaleLifetimeBuffer", mScaleLifetimePointsBuffer);
        sComputeShader.SetInt("gTransparencyLifetimeCount", mDescriptor.OpacityOverLifetime.Length());
        sComputeShader.SetBuffer(sKernelUpdate, "gTransparencyLifetimeBuffer", mTransparencyLifetimePointsBuffer);

        // SET META DATA.
        sComputeShader.SetInt("gMaxParticleCount", mMaxParticleCount);
        sComputeShader.SetFloat("gDeltaTime", Time.deltaTime);

        sComputeShader.SetFloats("gConstantAcceleration", new float[] {mDescriptor.ConstantAcceleration.x, mDescriptor.ConstantAcceleration.y, mDescriptor.ConstantAcceleration.z });
        sComputeShader.SetFloat("gConstantDrag", mDescriptor.ConstantDrag);

        // ACCELERATOR.
        Dictionary<GPUParticleAttractor, GPUParticleAttractor> attractorDictionary = GPUParticleAttractor.GetGPUParticleAttractorDictionary();
        if (attractorDictionary == null)
        {
            sComputeShader.SetInt("gAttractorCount", 0);
        }
        else
        {
            Debug.Assert(attractorDictionary.Count < sMaxAttractorCount);

            float[] attractorArray = new float[attractorDictionary.Count * 8];
            int i = 0;
            int count = 0;
            foreach (KeyValuePair<GPUParticleAttractor, GPUParticleAttractor> it in attractorDictionary)
            {
                GPUParticleAttractor attractor = it.Value;

                //Skip if inactive.
                if (!attractor.gameObject.activeInHierarchy) continue;

                float scale = Mathf.Max(Mathf.Max(attractor.transform.localScale.x, attractor.transform.localScale.y), attractor.transform.localScale.z);

                attractorArray[i++] = attractor.transform.position.x;
                attractorArray[i++] = attractor.transform.position.y;
                attractorArray[i++] = attractor.transform.position.z;
                attractorArray[i++] = attractor.Power;
                attractorArray[i++] = scale * attractor.Min;
                attractorArray[i++] = scale * attractor.Max;
                attractorArray[i++] = 0.0f; //Padding
                attractorArray[i++] = 0.0f; //Padding

                count++;
            }
            sGPUParticleAttractorBuffer.SetData(attractorArray);

            sComputeShader.SetInt("gAttractorCount", count);
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
            int count = 0;
            foreach (KeyValuePair<GPUParticleVectorField, GPUParticleVectorField> it in vectorFieldDictionary)
            {
                GPUParticleVectorField vectorField = it.Value;
                
                //Skip if inactive.
                if (!vectorField.gameObject.activeInHierarchy) continue;

                float scale = Mathf.Max(Mathf.Max(vectorField.transform.localScale.x, vectorField.transform.localScale.y), vectorField.transform.localScale.z);
                Vector3 vector = vectorField.RelativeVectorField ? vectorField.VectorRelative : vectorField.Vector;

                vectorFieldArray[i++] = vectorField.transform.position.x;
                vectorFieldArray[i++] = vectorField.transform.position.y;
                vectorFieldArray[i++] = vectorField.transform.position.z;
                vectorFieldArray[i++] = vector.x;
                vectorFieldArray[i++] = vector.y;
                vectorFieldArray[i++] = vector.z;
                vectorFieldArray[i++] = scale * vectorField.Min;
                vectorFieldArray[i++] = scale * vectorField.Max;

                count++;
            }
            sGPUParticleVectorFieldBuffer.SetData(vectorFieldArray);

            sComputeShader.SetInt("gVectorFieldCount", count);
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
            int count = 0;
            for (int i = 0, j = 0; i < sphereColliderList.Count; ++i)
            {
                GPUParticleSphereCollider sphereCollider = sphereColliderList[i];

                //Skip if inactive.
                if (!sphereCollider.gameObject.activeInHierarchy) continue;

                float scale = Mathf.Max(Mathf.Max(sphereCollider.transform.localScale.x, sphereCollider.transform.localScale.y), sphereCollider.transform.localScale.z);

                sphereColliderArray[j++] = sphereCollider.transform.position.x;
                sphereColliderArray[j++] = sphereCollider.transform.position.y;
                sphereColliderArray[j++] = sphereCollider.transform.position.z;
                sphereColliderArray[j++] = scale * sphereCollider.Radius;

                count++;
            }

            sGPUParticleSphereColliderBuffer.SetData(sphereColliderArray);

            sComputeShader.SetInt("gSphereColliderCount", count);
            sComputeShader.SetBuffer(sKernelUpdate, "gSphereColliderBuffer", sGPUParticleSphereColliderBuffer);
            sComputeShader.SetBuffer(sKernelUpdate, "gSphereColliderResultBufferWRITE", mSphereColliderResultBuffer);
        }

        // DISPATCH.
        sComputeShader.Dispatch(sKernelUpdate, (int)Mathf.Ceil(mMaxParticleCount / 64.0f), 1, 1);
    }

    private void UpdateVertexBuffers()
    {

        if (mDescriptor.EmittMesh)
        {

            Vector3[] vertices = mDescriptor.EmittMesh.vertices;

            for (int i = 0; i < vertices.Length; i++)
            {

                vertices[i] = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);

            }

            sEmittMeshInfoDictionary[mDescriptor.EmittMesh].mVertexBuffer.SetData(vertices);

        }

    }

    private void UpdateLifetimeBuffer(out ComputeBuffer buffer, Vector4[] lifetimePoints)
    {

        int length = lifetimePoints.Length;
        buffer = new ComputeBuffer(length, sizeof(float) * 4);

        float[] lifetimeData = new float[length * 4];
        for (int i = 0, j = 0; i < length; ++i, j += 4)
        {
            lifetimeData[j] = lifetimePoints[i].x;
            lifetimeData[j + 1] = lifetimePoints[i].y;
            lifetimeData[j + 2] = lifetimePoints[i].z;
            lifetimeData[j + 3] = lifetimePoints[i].w;
        }
        buffer.SetData(lifetimeData);

    }

    // MERGE.
    private static void Merge()
    {
        if (sMergedParticleCount < sTotalParticleCount)
        {
            sMergedParticleCount = Mathf.NextPowerOfTwo(sTotalParticleCount);

            // Resize buffers.
            sMergedPositionBuffer.Resize(sMergedParticleCount);
            sMergedVelocityBuffer.Resize(sMergedParticleCount);
            sMergedLifetimeBuffer.Resize(sMergedParticleCount);

            sMergedColorBuffer.Resize(sMergedParticleCount);
            sMergedHaloBuffer.Resize(sMergedParticleCount);
            sMergedScaleBuffer.Resize(sMergedParticleCount);
            sMergedTransperancyBuffer.Resize(sMergedParticleCount);

            sSortElementSwapBuffer.Resize(sMergedParticleCount);
        }

        // CLEAR OLD VALUES.

        // TODO: Might not need to reset all this data!!
        float[] mergedData = new float[sMergedParticleCount * 4];
        for (int i = 0; i < mergedData.GetLength(0); ++i)
            mergedData[i] = -100.0f;

        sMergedLifetimeBuffer.GetInputBuffer().SetData(mergedData);
        sMergedLifetimeBuffer.GetOutputBuffer().SetData(mergedData);

        for (int i = 0; i < mergedData.GetLength(0); ++i)
            mergedData[i] = float.MaxValue;

        sMergedPositionBuffer.GetInputBuffer().SetData(mergedData);
        sMergedPositionBuffer.GetOutputBuffer().SetData(mergedData);

        // SET BUFFERS.
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergePositionOUT", sMergedPositionBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergeVelocityOUT", sMergedVelocityBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergeLifetimeOUT", sMergedLifetimeBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergeColorOUT", sMergedColorBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergeHaloOUT", sMergedHaloBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergeScaleOUT", sMergedScaleBuffer.GetOutputBuffer());
        sComputeShader.SetBuffer(sKernelMergeInitSort, "mergeTransperancyOUT", sMergedTransperancyBuffer.GetOutputBuffer());

        sComputeShader.SetBuffer(sKernelMergeInitSort, "gSortElementBufferOUT", sSortElementSwapBuffer.GetOutputBuffer());


        Vector3 cForward = Camera.main.transform.forward;
        sComputeShader.SetFloats("gCameraForward", new float[] { cForward.x, cForward.y, cForward.z });

        int offset = 0;
        foreach (KeyValuePair<GPUParticleSystem,GPUParticleSystem> it in sGPUParticleSystemDictionary)
        {
            GPUParticleSystem system = it.Value;

            if (!system.gameObject.activeInHierarchy) continue;

            sComputeShader.SetInt("gLocalParticleCount", system.mMaxParticleCount);
            sComputeShader.SetInt("gOffsetIndex", offset);

            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemPositionIN", system.mPositionBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemVelocityIN", system.mVelocityBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemLifetimeIN", system.mLifetimeBuffer.GetOutputBuffer());
            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemColorIN", system.mColorBuffer);
            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemHaloIN", system.mHaloBuffer);
            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemScaleIN", system.mScaleBuffer);
            sComputeShader.SetBuffer(sKernelMergeInitSort, "gLocalSystemTransperancyIN", system.mTransperancyBuffer);

            sComputeShader.Dispatch(sKernelMergeInitSort, (int)Mathf.Ceil(system.mMaxParticleCount / 64.0f), 1, 1);

            offset += system.mMaxParticleCount;
        }
    }

    // SORT.
    private static void Sort()
    {
        Debug.Assert(sTotalParticleCount <= sMergedParticleCount);
        Debug.Assert(Mathf.IsPowerOfTwo(sMergedParticleCount));

        // DISPATCH SORT BUFFER.
        for (int k = 2; k <= sMergedParticleCount; k <<= 1) // Major steps.
        {
            for (int j = k >> 1; j > 0; j = j >> 1) // Minor steps.
            {
                // SWAP AND BIND BUFFERS.
                sSortElementSwapBuffer.Swap();
                sComputeShader.SetBuffer(sKernelSort, "gSortElementBufferIN", sSortElementSwapBuffer.GetInputBuffer());
                sComputeShader.SetBuffer(sKernelSort, "gSortElementBufferOUT", sSortElementSwapBuffer.GetOutputBuffer());

                sComputeShader.SetInt("gK", k);
                sComputeShader.SetInt("gJ", j);
                sComputeShader.SetInt("gMaxParticleCount", sMergedParticleCount);

                // DISPATCH.
                sComputeShader.Dispatch(sKernelSort, (int)Mathf.Ceil(sMergedParticleCount / 64.0f), 1, 1);
            }
        }
    }

    // RENDER.
    private void RenderSystem()
    {
        Debug.Assert(sTotalParticleCount <= sMergedParticleCount);

        sRenderMaterial.SetPass(0);

        // BIND BUFFERS.
        sRenderMaterial.SetBuffer("gPosition", sMergedPositionBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gVelocity", sMergedVelocityBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gLifetime", sMergedLifetimeBuffer.GetOutputBuffer());

        sRenderMaterial.SetBuffer("gColor", sMergedColorBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gHalo", sMergedHaloBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gScale", sMergedScaleBuffer.GetOutputBuffer());
        sRenderMaterial.SetBuffer("gTransparency", sMergedTransperancyBuffer.GetOutputBuffer());

        // BIND SORTED BUFFER AND USE AS INDEX BUFFER TO RENDER BACK TO FRONT.
        sRenderMaterial.SetBuffer("gSortedParticleIndexBuffer", sSortElementSwapBuffer.GetOutputBuffer());

        // DRAW.
        Graphics.DrawProcedural(MeshTopology.Points, sTotalParticleCount, 1);
    }

    public int Count { get { return mMaxParticleCount; } }

    private bool mInitiated = false;

    // MONOBEHAVIOUR.
    public GPUParticleDescriptor ParticleDescriptor {
        set
        {

            mDescriptor = value;

            if (!mInitiated)
            {

                if (sGPUParticleSystemDictionary == null) StartUp();
                InitSystem();
                sGPUParticleSystemDictionary[this] = this;
                mInitiated = true;

            }
            else Apply();

        }
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        // Used to make static functions get called once.
        sLateUpdate = true;
        sRenderedCameraDictionary.Clear();

        //Update the vertex buffers to match the current transform.
        UpdateVertexBuffers();

        // Update buffers if needed (updated).
        if (mDescriptor.OutOfDate)
            Apply();

        // Emitt new particles this frame if active.
        if (mActive)
            EmittUpdate();

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

    // FETCH COLLISION RESULTS.
    private static void FetchCollisionResults()
    {
        List<GPUParticleSphereCollider> sphereColliderList = GPUParticleSphereCollider.GetGPUParticleSphereColliderList();

        // Return early if null or zero GPUParticleSphereCollider.
        if (sphereColliderList == null) return;
        if (sphereColliderList.Count == 0) return;

        Debug.Assert(sphereColliderList.Count < sMaxSphereColliderCount);

        // Reset data.

        // TODO: Might not need to reset all this data!!
        int[] data = new int[sMaxGPUColliderCount];
        for (int i = 0; i < data.GetLength(0); ++i)
            data[i] = 0;
        sGPUColliderResultSwapBuffer.GetInputBuffer().SetData(data);
        sGPUColliderResultSwapBuffer.GetOutputBuffer().SetData(data);

        int count = 0;
        bool initZero = true;
        foreach (KeyValuePair<GPUParticleSystem, GPUParticleSystem> it in sGPUParticleSystemDictionary)
        {
            GPUParticleSystem system = it.Value;

            if (!system.gameObject.activeInHierarchy) continue;

            sGPUColliderResultSwapBuffer.Swap();
            sComputeShader.SetBuffer(sKernelResult, "gGPUColliderResultBufferIN", sGPUColliderResultSwapBuffer.GetInputBuffer());
            sComputeShader.SetBuffer(sKernelResult, "gGPUColliderResultBufferOUT", sGPUColliderResultSwapBuffer.GetOutputBuffer());

            sComputeShader.SetInt("gGPUColliderCount", sphereColliderList.Count);
            sComputeShader.SetBuffer(sKernelResult, "gSphereColliderResultBufferREAD", system.GetSphereColliderResultBuffer());

            sComputeShader.SetBool("gInitZero", initZero);
            initZero = false;

            // DISPATCH.
            sComputeShader.Dispatch(sKernelResult, (int)Mathf.Ceil(sMaxSphereColliderCount / 64.0f), 1, 1);

            count++;
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

    // MONOBEHAVIOUR.
    private void OnRenderObject()
    {
        // Check and make sure to only render particles once per camera/eye.

        Camera.MonoOrStereoscopicEye activeEye = Camera.current.stereoActiveEye;

        if (!sRenderedCameraDictionary.ContainsKey(Camera.current))
        {
            Dictionary<Camera.MonoOrStereoscopicEye, Camera.MonoOrStereoscopicEye> activeEyeDictionary = new Dictionary<Camera.MonoOrStereoscopicEye, Camera.MonoOrStereoscopicEye>();
            sRenderedCameraDictionary[Camera.current] = activeEyeDictionary;
        }

        if (!sRenderedCameraDictionary[Camera.current].ContainsKey(activeEye))
        {
            // Add camera to dictionary so we only render system once per camera.
            sRenderedCameraDictionary[Camera.current][activeEye] = activeEye;

            // Merge particle buffers.
            Merge();

            // Sort particles.
            Sort();

            // Render this frame.
            RenderSystem();
        }
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
