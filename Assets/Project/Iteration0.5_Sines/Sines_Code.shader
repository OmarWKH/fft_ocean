Shader "Unlit/Sines"
{
    Properties
    {
        _xz ("xz", float) = 1.
        _y ("y", float) = 1.
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            struct Wave {
                float2 K; // wavevector, direction, xz
                float a; // amplitude
                float w; // frequency
                float p; // phase
            };

            float _xz; // toggles the shader;s effect on XZ
            float _y; // toggles the shader's effect on Y
            StructuredBuffer<Wave> _Waves;
            int _WavesCount;

            v2f vert (appdata v)
            {
                float2 xz_sum;
                float y_sum;
                for (float i=0; i<_WavesCount; i++) {
                    Wave wave = _Waves[i];
                    float2 k = wave.K;
                    float2 xz = (k/length(k)) * wave.a * sin(dot(k, v.vertex.xz) - wave.w * _Time + wave.p);
                    xz_sum += xz;
                    float y = wave.a * cos(dot(k, v.vertex.xz) - (wave.w * _Time) + wave.p);
                    y_sum += y;
                }
                v.vertex.xz = lerp(v.vertex.xz, v.vertex.xz - xz_sum, _xz);
                v.vertex.y = lerp(v.vertex.y, y_sum, _y);
                
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = Vector(1,1,1,1);
                return col;
            }
            ENDCG
        }
    }
}
