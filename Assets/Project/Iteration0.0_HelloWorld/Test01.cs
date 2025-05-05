using UnityEngine;

public class Test01 : MonoBehaviour {
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    
    private int mainKernel;

    void Start()
    {
        InitializeShaders();
    }

    void Update() {
        DispatchShaders();
    }

    void InitializeShaders() {
        mainKernel = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(mainKernel, Shader.PropertyToID("Result"), renderTexture);
    }

    void DispatchShaders() {
        uint threadsX, threadsY, threadsZ;

        computeShader.GetKernelThreadGroupSizes(mainKernel, out threadsX, out threadsY, out threadsZ);
        computeShader.Dispatch(mainKernel, (int)threadsX, (int)threadsY, (int)threadsZ);
    }
}