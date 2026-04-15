Shader "6991/LightningVFX/CloudShader"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [Space(10)]
        [NoScaleOffset] [Toggle] _LUT ("LUT", Float) = 0
        [NoScaleOffset] _ColorLookupTexture ("Color LUT Texture", 2D) = "white" {}
        [Header(Emission Properties)]
        [Space(10)]
        [Toggle] _Emission ("Emission", Integer) = 0
        [NoScaleOffset] _EmissiveTex ("Emissive", 2D) = "white" {}
        _EmissiveIntensity ("EmissiveIntensity", Range(0,10)) = 1
        _EmissiveColor ("EmissiveColor", Color) = (0,0,1,0)
        [Header(Erosion Mask Properties)]
        [Space(10)]
        [NoScaleOffset] _ErosionMask ("ErosionMask", 2D) = "white" {}
        _ErosionThreshold ("ErosionThreshold", Range(0.01, 1)) = 0.5
        // [Space(10)]
        // [Header(Attribute Testing)]
        // [KeywordEnum(Add, Multiply, Test)] _AttributeTest ("Test Attribute", Integer) = 0 // KeywordEnum is shaderkeyword related
        // [Enum(Left,0, Middle,1, Right,2)] _AttributeTest2 ("Test Attribute 2", Integer) = 0  // value is follow by the enum property name
        // [PowerSlider(3.0)] _AttributeTest3 ("Test Attribute 3", Range(0.01, 1)) = 0.08 // A slider with 3.0 response curve
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

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

            float _LUT;
            sampler2D _ColorLookupTexture;
            
            int _Emission;
            sampler2D _EmissiveTex;
            float _EmissiveIntensity;
            half4 _EmissiveColor;
            
            sampler2D _ErosionMask;
            float _ErosionThreshold;
            
            
            v2f vertex_shader (appdata_base v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            half4 fragment_shader (v2f i) : SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                half4 erosion_alpha = tex2D(_ErosionMask, i.uv).a;
                
                color.a = step(_ErosionThreshold, erosion_alpha);
                
                if (_Emission > 0){
                    half4 emissive_tex = tex2D(_EmissiveTex, i.uv).r;
                    half4 litColor = color.rgba; // baseCol.rgb * LightColor * NdotL;
                    half4 emissive = emissive_tex * _EmissiveColor * _EmissiveIntensity; // emissiveMask * _EmissiveColor.rgb * _EmissiveIntensity;
                    color = litColor + emissive;
                }

                if (_LUT) {
                    half3 lutColor = tex2D(_ColorLookupTexture, float2(0.9, .9)).rgb;
                    color.rgb = color.rgb * lutColor;
                }

                return color;
            }

            void procedural_gas_noise() {

            }
            void procedural_gas_turbulence() {

            }
            void procedural_gas_volume_density_function() {

            }
            
            ENDHLSL
        }
    }
}
