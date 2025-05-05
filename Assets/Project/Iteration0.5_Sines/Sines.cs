using System;
using UnityEngine;

[Serializable]
public struct Wave {
    public Vector2 K; // wavevector, direction, xz
    public float A; // amplitude
    public float w; // frequency
    public float p; // phase
}

public class Sines : MonoBehaviour {
    public Material material;
    public Renderer aRenderer;
    public Wave[] waves;

    private ComputeBuffer _buffer;

    public void Start()
    {
        _buffer = new ComputeBuffer(waves.Length, sizeof(float) * 5);
        _buffer.SetData(waves);
        aRenderer.material.SetInt(Shader.PropertyToID("_WavesCount"), waves.Length);
        aRenderer.material.SetBuffer(Shader.PropertyToID("_Waves"), _buffer);
    }

    [ContextMenu(nameof(UpdateParameters))]
    public void UpdateParameters() {
        aRenderer.enabled = false;
        if (_buffer.count != waves.Length) {
            aRenderer.material.SetInt(Shader.PropertyToID("_WavesCount"), 0);
            _buffer.Dispose();
            _buffer = new ComputeBuffer(waves.Length, sizeof(float) * 5);
            aRenderer.material.SetInt(Shader.PropertyToID("_WavesCount"), waves.Length);
            aRenderer.material.SetBuffer(Shader.PropertyToID("_Waves"), _buffer);
        }
        _buffer.SetData(waves);
        aRenderer.enabled = true;
    }

    void OnDestroy()
    {
        _buffer.Dispose();
    }
}