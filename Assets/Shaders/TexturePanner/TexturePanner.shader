Shader "6991/TexturePanner"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Direction ("Direction", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass {
            HLSLPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _Color;
            float4 _Direction;

            float2 panner(float2 uv, float2 tiling, float2 offset)
            {
                return uv * tiling + offset;
            }

            v2f vertex_shader(appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.texcoord;

                return o;
            }
            
            half4 fragment_shader(v2f i) : SV_Target
            {
                float2 offset = float2(0,.03) * _Time.y;
                float2 scrolling_uv = panner(i.uv, float2(1,1), offset);

                half4 color = tex2D(_MainTex, scrolling_uv);
                
                return color;
            }

            ENDHLSL
        }
    }
}
