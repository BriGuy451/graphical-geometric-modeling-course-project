Shader "6991/LightningVFX/LightningShader"
{
    Properties
    {
        [NoScaleOffset] _LightningTexArr ("LightningTextures", 2DArray) = "" {}
        _SliceRange ("SliceRange", Range(0, 3)) = 0
        [NoScaleOffset] _AnimationGradientTex ("AnimationGradient", 2D) = "white" {}
        _AnimationRange ("AnimationRange", Range(0, 1)) = 0.5
        [Toggle] _LUT ("LUT", Float) = 0
        [NoScaleOffset] _LUTex ("LUTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vertex_shader
            #pragma fragment fragment_shader

            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            UNITY_DECLARE_TEX2DARRAY(_LightningTexArr);
            
            float _SliceRange;

            sampler2D _AnimationGradientTex;
            float _AnimationRange;

            float _LUT;
            sampler2D _LUTex;

            v2f vertex_shader (appdata_base v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            half4 fragment_shader (v2f i) : SV_Target
            {
                half4 color = half4(0,0,0,0);
                half4 gradient = tex2D(_AnimationGradientTex, i.uv);

                if (step(_AnimationRange, gradient.r) == 0)
                    clip(-1);
                
                color = UNITY_SAMPLE_TEX2DARRAY(_LightningTexArr, float3(i.uv, _SliceRange));

                if (_LUT) {
                    // half3 lutColor = tex2D(_LUTex, float2(0.1, .9)).rgb;
                    half3 lutColor = float3(0.184, .35, .95) * 4;
                    color.rgb = color.rgb + lutColor;
                }
                
                return color;
            }

            ENDHLSL
        }
    }
}
