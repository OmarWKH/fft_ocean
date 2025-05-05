using System;
using UnityEngine;

public class OceanSimulationNaive : MonoBehaviour
{
    [Header("Resources")]
    public ComputeShader computeShader;
    public RenderTexture displacement;
    public RenderTexture ht;
    public RenderTexture iDFTHt;
    public RenderTexture debug;
    public Texture2D noiseR0;
    public Texture2D noiseI0;
    public Texture2D noiseR1;
    public Texture2D noiseI1;
    [Header("Static Parameters")]
    public int N;
    [Header("Dynamic Parameters")]
    public int L;
    public int A;
    public Vector2 w;
    public int V;
    public bool overrideTime;
    public float time;
    [Header("Testing")]
    public bool playInEditMode;

    private int idftOneShotHtKernel;
    private int evolve;
    private bool initialized;

    void Start()
    {
        InitializeShaders();
    }

    void Update()
    {
        if (!Application.isPlaying) return;
        UpdateSafely();
    }

    void UpdateSafely() {
        try {
            if (!initialized) InitializeShaders();
            UpdateShaders();
        }
        catch (Exception) {
            this.enabled = false;
            throw;
        }
    }

    void UpdateShaders() {
        Vector4 shaderTime = Shader.GetGlobalVector("_Time");
        if (overrideTime) {
            shaderTime = new Vector4(time/20, time, time*2, time*3);
        }
        computeShader.SetVector("Time", shaderTime);
        computeShader.SetInt("L", L);
        computeShader.SetInt("A", A);
        computeShader.SetVector("w", w);
        computeShader.SetInt("V", V);
        DispatchUpdateShaders();
    }

    void InitializeShaders() {
        initialized = true;

        computeShader.SetInt("N", N);
        
        evolve = computeShader.FindKernel("Evolve");
        computeShader.SetTexture(evolve, Shader.PropertyToID("RT_ht"), ht);
        computeShader.SetTexture(evolve, "noise_r0", noiseR0);
        computeShader.SetTexture(evolve, "noise_i0", noiseI0);
        computeShader.SetTexture(evolve, "noise_r1", noiseR1);
        computeShader.SetTexture(evolve, "noise_i1", noiseI1);
        computeShader.SetTexture(evolve, Shader.PropertyToID("RT_Debug"), debug);

        idftOneShotHtKernel = computeShader.FindKernel("iDFTHTOneShot");
        computeShader.SetTexture(idftOneShotHtKernel, Shader.PropertyToID("RT_ht"), ht);
        computeShader.SetTexture(idftOneShotHtKernel, Shader.PropertyToID("RT_iDFTHt"), iDFTHt);
        computeShader.SetTexture(idftOneShotHtKernel, Shader.PropertyToID("RT_VertexDisplacement"), displacement);
        computeShader.SetTexture(idftOneShotHtKernel, Shader.PropertyToID("RT_Debug"), debug);
    }

    void DispatchUpdateShaders() {
        uint threadsX, threadsY, threadsZ;

        computeShader.GetKernelThreadGroupSizes(evolve, out threadsX, out threadsY, out threadsZ);
        computeShader.Dispatch(evolve, (int)threadsX, (int)threadsY, (int)threadsZ);

        computeShader.GetKernelThreadGroupSizes(idftOneShotHtKernel, out threadsX, out threadsY, out threadsZ);
        computeShader.Dispatch(idftOneShotHtKernel, (int)threadsX, (int)threadsY, (int)threadsZ);
    }

    [ContextMenu("Generate")]
    void Generate() {
        InitializeShaders();
        UpdateShaders();
    }

    void OnGUI()
    {
        
    }

    void OnRenderObject()
    {
        if (runInEditMode && !Application.isPlaying) {
            UpdateSafely();
        }
    }

    void OnValidate()
    {
        this.runInEditMode = playInEditMode;
    }
}
