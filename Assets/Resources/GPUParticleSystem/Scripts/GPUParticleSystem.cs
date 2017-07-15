using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleSystem : MonoBehaviour
{
    /// +++ STRUCTS +++ ///

    private struct GPUParticle
    {
        private float px, py, pz; // Position.
        private float vx, vy, vz; // Velocity
        private float sx, sy; // Scale.
        private float cx, cy, cz; // Color.
        //private float lifetime; // Lifetime.
    }

    private struct Constants
    {
        public float drag; // Constant Drag.
        public Vector3 force; // Constant Force.
    }

    private struct EmittInfo
    {
        private int emittIndex; // Index in particle array.
        private float px, py, pz; // Initial Position.
        private float vx, vy, vz; // Initial Velocity.
        private float sx, sy; // Initial Scale.
        private float cx, cy, cz; // Initial Color.
        //private float lifetime; // Initial Lifetime.

        public EmittInfo(
            int emittIndex,
            float px = 0, float py = 0, float pz = 0,
            float vx = 0, float vy = 1, float vz = 0,
            float sx = 1, float sy = 1,
            float cx = 1, float cy = 0, float cz = 0
            //float lifetime = 10
            )
        {
            this.emittIndex = emittIndex;
            this.px = px; this.py = py; this.pz = pz;
            this.vx = vx; this.vy = vy; this.vz = vz;
            this.sx = sx; this.sy = sy;
            this.cx = cx; this.cy = cy; this.cz = cz;
            //this.lifetime = lifetime;
        }
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

            //sComputeShader.SetInt("gEmittIndex", mParticleCount + i);

            // EMITT INFO.
            EmittInfo emittInfo = new EmittInfo(
                mParticleCount + i,
                transform.position.x, transform.position.y, transform.position.z, // Position.
                0.0f, 1.0f, 0.0f, // Velocity.
                0.4f, 0.4f, // Scale.
                1.0f, 0.0f, 1.0f // Color.
                //1.0f // Lifetime.
                );
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
