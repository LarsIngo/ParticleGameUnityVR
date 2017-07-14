using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleSystem : MonoBehaviour
{
    /// +++ STRUCTS +++ ///

    public struct GPUParticle
    {
        public float x,y,z;
        float pad;

        public GPUParticle(float x = 0, float y = 0, float z = 0) { this.x = x; this.y = y; this.z = z; pad = 0; }
        public GPUParticle(Vector3 position) { x = position.x; y = position.y; z = position.z; pad = 0; }
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
        //sKernelEmitt = sComputeShader.FindKernel("EMITT");
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

    private SwapBuffer mParticleBuffer;
    private const int mMaxParticleCount = 1000;
    private int mParticleCount = 0;
    private int mNextFrameParticleCount = 0;

    // INIT.
    private void InitSystem()
    {
        mParticleBuffer = new SwapBuffer(2, mMaxParticleCount, System.Runtime.InteropServices.Marshal.SizeOf(typeof(GPUParticle)));
    }

    // DEINIT.
    private void DeInitSystem()
    {
        mParticleBuffer.Release();
    }

    // UPDATE.
    private void UpdateSystem()
    {
        if (mParticleCount == 0) return;

        mParticleBuffer.Swap();
        sComputeShader.SetBuffer(sKernelUpdate, "gParticleBufferIN", mParticleBuffer.GetInputBuffer());
        sComputeShader.SetBuffer(sKernelUpdate, "gParticleBufferOUT", mParticleBuffer.GetOutputBuffer());
        sComputeShader.SetInt("gParticleCount", mParticleCount);
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

    // SET PARTICLES.
    public void SetParticles(GPUParticle[] particleArray)
    {
        mParticleBuffer.GetInputBuffer().SetData(particleArray); // TODO, remove one
        mParticleBuffer.GetOutputBuffer().SetData(particleArray);
        mNextFrameParticleCount = particleArray.GetLength(0);
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
        // Update this frame.
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
