Shader "6991/LightningVFX/SplashShader"
{
    Properties
    {
        [NoScaleOffset] _SplatterTexArr ("SplatterTexArr", 2DArray) = "" {}
        [NoScaleOffset] [Normal] _SplatterNormalTexArr ("SplatterNormalTexArr", 2DArray) = "" {}
        [NoScaleOffset] _ColorLUT ("ColorLookupTex", 2D) = "" {}
        _Slice ("Slice", Range(0, 48)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _ColorLUT;
            float _AnimationRange;
            float _Slice;

            UNITY_DECLARE_TEX2DARRAY(_SplatterTexArr);
            UNITY_DECLARE_TEX2DARRAY(_SplatterNormalTexArr);

            v2f vertex_shader (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.texcoord;

                return o;
            }

            half4 fragment_shader (v2f i) : SV_Target
            {
                
                half4 normal = UNITY_SAMPLE_TEX2DARRAY(_SplatterNormalTexArr, float3(i.uv, _Slice));
                normal.xyz = normal.xyz * 2 - 1;
                normal.xyz = normal.xzy;
                
                half4 color = UNITY_SAMPLE_TEX2DARRAY(_SplatterTexArr, float3(i.uv, _Slice));
                half4 lut_color = tex2D(_ColorLUT, half2(0.9, 0.7));

                color.rgb = color.rgb * lut_color.rgb;
                // color *= normal;

                return color;
            }
            ENDHLSL
        }
    }
}
