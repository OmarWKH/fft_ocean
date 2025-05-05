using UnityEngine;

public class Test01 : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public RenderTexture displacement;
    public RenderTexture h0;
    public RenderTexture philips;
    public RenderTexture iDFTRow;
    public RenderTexture iDFTColumnFinal;
    public RenderTexture debug;
    public int N;
    public int L;
    public int A;
    public Vector2 w;
    public int V;
    public Texture2D noiseR0;
    public Texture2D noiseI0;
    public Texture2D noiseR1;
    public Texture2D noiseI1;

    private int mainKernel;
    private int initializeKernel;
    private int idftRowKernel;
    private int idftColumnKernel;
    private int idftOneShotKernel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeShaders();
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetVector("Time", Shader.GetGlobalVector("_Time"));
        computeShader.SetInt("N", N);
        computeShader.SetInt("L", L);
        computeShader.SetInt("A", A);
        computeShader.SetVector("w", w);
        computeShader.SetInt("V", V);
        DispatchShaders();
        this.enabled = false;
    }

    void InitializeShaders() {
        mainKernel = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(mainKernel, Shader.PropertyToID("Result"), renderTexture);

        initializeKernel = computeShader.FindKernel("Initialize");
        computeShader.SetTexture(initializeKernel, Shader.PropertyToID("RT_h0"), h0);
        computeShader.SetTexture(initializeKernel, Shader.PropertyToID("RT_Ph"), philips);
        computeShader.SetTexture(initializeKernel, "noise_r0", noiseR0);
        computeShader.SetTexture(initializeKernel, "noise_i0", noiseI0);
        computeShader.SetTexture(initializeKernel, "noise_r1", noiseR1);
        computeShader.SetTexture(initializeKernel, "noise_i1", noiseI1);

        idftRowKernel = computeShader.FindKernel("iDFTRow");
        computeShader.SetTexture(idftRowKernel, Shader.PropertyToID("RT_h0"), h0);
        computeShader.SetTexture(idftRowKernel, Shader.PropertyToID("RT_iDFTRow"), iDFTRow);

        idftColumnKernel = computeShader.FindKernel("iDFTColumn");
        computeShader.SetTexture(idftColumnKernel, Shader.PropertyToID("RT_iDFTRow"), iDFTRow);
        computeShader.SetTexture(idftColumnKernel, Shader.PropertyToID("RT_iDFTColumnFinal"), iDFTColumnFinal);
        computeShader.SetTexture(idftColumnKernel, Shader.PropertyToID("RT_VertexDisplacement"), displacement);

        idftOneShotKernel = computeShader.FindKernel("iDFTOneShot");
        computeShader.SetTexture(idftOneShotKernel, Shader.PropertyToID("RT_h0"), h0);
        computeShader.SetTexture(idftOneShotKernel, Shader.PropertyToID("RT_iDFTColumnFinal"), iDFTColumnFinal);
        computeShader.SetTexture(idftOneShotKernel, Shader.PropertyToID("RT_VertexDisplacement"), displacement);
        computeShader.SetTexture(idftOneShotKernel, "noise_r0", noiseR0);
        computeShader.SetTexture(idftOneShotKernel, "noise_i0", noiseI0);
        computeShader.SetTexture(idftOneShotKernel, "noise_r1", noiseR1);
        computeShader.SetTexture(idftOneShotKernel, "noise_i1", noiseI1);
        computeShader.SetTexture(idftOneShotKernel, Shader.PropertyToID("RT_Debug"), debug);
    }

    void DispatchShaders() {
        computeShader.GetKernelThreadGroupSizes(mainKernel, out uint threadsX, out uint threadsY, out uint threadsZ);
        computeShader.Dispatch(mainKernel, (int)threadsX, (int)threadsY, (int)threadsZ);

        computeShader.GetKernelThreadGroupSizes(initializeKernel, out threadsX, out threadsY, out threadsZ);
        computeShader.Dispatch(initializeKernel, (int)threadsX, (int)threadsY, (int)threadsZ);

        // computeShader.GetKernelThreadGroupSizes(idftRowKernel, out threadsX, out threadsY, out threadsZ);
        // computeShader.Dispatch(idftRowKernel, (int)threadsX, (int)threadsY, (int)threadsZ);

        // computeShader.GetKernelThreadGroupSizes(idftColumnKernel, out threadsX, out threadsY, out threadsZ);
        // computeShader.Dispatch(idftColumnKernel, (int)threadsX, (int)threadsY, (int)threadsZ);

        computeShader.GetKernelThreadGroupSizes(idftOneShotKernel, out threadsX, out threadsY, out threadsZ);
        computeShader.Dispatch(idftOneShotKernel, (int)threadsX, (int)threadsY, (int)threadsZ);
    }
}
