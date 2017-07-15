using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleSystem : MonoBehaviour
{
    /// +++ STRUCTS +++ ///

    private struct GPUParticle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector2 scale;
        public Vector3 color;
        //private float lifetime;
    }

    private struct Constants
    {
        public float drag;
        public Vector3 force;
    }

    private struct EmittInfo
    {
        public int emittIndex;
        public Vector3 postition;
        public Vector3 velocity;
        public Vector2 scale;
        public Vector3 color;
        public float lifetime;
    }

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

    // STARTUP.
    public static void StartUp()
    {
        sRenderMaterial = new Material(Resources.Load<Shader>("GPUParticleSystem/Shaders/GPUParticleRenderShader"));
        sComputeShader = Resources.Load<ComputeShader>("GPUParticleSystem/Shaders/GPUParticleComputeShader");
        sKernelUpdate = sComputeShader.FindKernel("UPDATE");
        sKernelEmitt = sComputeShader.FindKernel("EMITT");
    }

    // SHUTDOWN.
    public static void Shutdown()
    {
        sRenderMaterial = null;
        sComputeShader = null;
        sKernelUpdate = -1;
        sKernelEmitt = -1;
    }

    /// MEMBER

    /// --- STATIC --- ///

    /// +++ MEMBERS +++ ///

    // Particle.
    private SwapBuffer mParticleBuffer;
    private const int mMaxParticleCount = 1000;
    private int mParticleCount = 0;
    private int mNextFrameParticleCount = 0;
    private ComputeBuffer mEmittInfoBuffer;
    private ComputeBuffer mConstantsBuffer;

    // Emitter.
    /// <summary>
    /// Time since last emitt.
    /// </summary>
    private float mEmittTimer = 0.0f;

    /// <summary>
    /// Particles to emitt per seconds(hertz).
    /// Default: 2
    /// </summary>
    private float mEmittFrequency = 2.0f;

    // INIT.
    private void InitSystem()
    {
        mParticleBuffer = new SwapBuffer(2, mMaxParticleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(GPUParticle)));
        mEmittInfoBuffer = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(EmittInfo)));
        mConstantsBuffer = new ComputeBuffer(1, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Constants)));
    }

    // DEINIT.
    private void DeInitSystem()
    {
        mParticleBuffer.Release();
        mEmittInfoBuffer.Release();
        mConstantsBuffer.Release();
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
            // BIND PARTICLE BUFFER.
            sComputeShader.SetBuffer(sKernelEmitt, "gParticleBufferIN", mParticleBuffer.GetInputBuffer());

            // EMITT INFO.
            EmittInfo emittInfo = new EmittInfo();
            emittInfo.emittIndex = mParticleCount + i;
            emittInfo.postition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            emittInfo.velocity = new Vector3(0.0f, 1.0f, 0.0f);
            emittInfo.scale = new Vector3(0.4f, 0.4f);
            emittInfo.color = new Vector3(1.0f, 0.0f, 1.0f);
            mEmittInfoBuffer.SetData(new EmittInfo[] { emittInfo });
            sComputeShader.SetBuffer(sKernelEmitt, "gEmittInfoBuffer", mEmittInfoBuffer);

            // DISPATCH.
            sComputeShader.Dispatch(sKernelEmitt, 1, 1, 1);
        }

        mNextFrameParticleCount = mParticleCount + emittCount;
    }

    // UPDATE.
    private void UpdateSystem()
    {
        if (mParticleCount == 0) return;

        // CONSTANTS.
        Constants constants = new Constants();
        constants.drag = 1.0f / 5.0f;
        constants.force = new Vector3(0, 0, 0);
        mConstantsBuffer.SetData(new Constants[] { constants });
        sComputeShader.SetBuffer(sKernelUpdate, "gConstantsBuffer", mConstantsBuffer);

        mParticleBuffer.Swap();
        sComputeShader.SetBuffer(sKernelUpdate, "gParticleBufferIN", mParticleBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gParticleBufferOUT", mParticleBuffer.GetOutputBuffer());
        sComputeShader.SetInt("gParticleCount", mParticleCount);
        sComputeShader.SetFloat("gDeltaTime", Time.deltaTime);
        sComputeShader.Dispatch(sKernelUpdate, (int)Mathf.Ceil(mParticleCount / 64.0f), 1, 1);
    }

    // RENDER.
    private void RenderSystem()
    {
        if (mParticleCount == 0) return;

        sRenderMaterial.SetPass(0);
        sRenderMaterial.SetBuffer("gParticleBuffer", mParticleBuffer.GetOutputBuffer());
        sRenderMaterial.SetInt("gParticleCount", mParticleCount);
        Graphics.DrawProcedural(MeshTopology.Triangles, 6, mParticleCount);
    }

    public int Count { get { return mParticleCount; } }

    // MONOBEHAVIOUR.
    private void Awake()
    {
        if (sRenderMaterial == null) StartUp();
        InitSystem();
    }

    // MONOBEHAVIOUR.
    private void Update()
    {
        // Update emitter this frame.
        EmittUppdate();

        // Update particles this frame.
        UpdateSystem();
    }

    // MONOBEHAVIOUR.
    private void OnRenderObject()
    {
        // Render this frame.
        RenderSystem();

        // Update particle count for next frame.
        mParticleCount = mNextFrameParticleCount;
    }

    // MONOBEHAVIOUR.
    private void OnDestroy()
    {
        DeInitSystem();
        if (sRenderMaterial != null) Shutdown();
    }

    /// --- MEMBERS --- ///

}
