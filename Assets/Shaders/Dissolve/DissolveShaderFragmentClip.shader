Shader "6991/Dissolve/DissolveShaderFragmentClip"
{
    Properties
    {
        [MainTexture][NoScaleOffset] _MainTex ("MainTexture", 2D) = "white" {}
        _Threshold ("EffectThreshold", Range(0.0, 1.0)) = 0
        _MinY ("MinimumY", float) = 0
        _MaxY ("MaximumY", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCast"= "true" }
        // Cull Off
        LOD 100

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vertex_passthrough
            #pragma fragment fragment_breakdown
            
            // make fog work
            // I want to play with the fog
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float mask_pos: TEXCOORD1;
                UNITY_FOG_COORDS(1)
            };
            
            float rand(float2 uv) { return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453); }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float _Threshold;
            float _MinY;
            float _MaxY;
            
            v2f vertex_passthrough(appdata v)
            {
                // compute mask to determine if this vertex should be collapsed or not.
                float vertex_position_mask = saturate((v.vertex.y - _MinY) / (_MaxY - _MinY));
                
                v2f vertex_to_fragment;
                vertex_to_fragment.vertex = UnityObjectToClipPos(v.vertex);
                vertex_to_fragment.mask_pos = vertex_position_mask;
                
                vertex_to_fragment.uv = v.uv;
                
                return vertex_to_fragment;
            }

            fixed4 fragment_breakdown(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                if (i.mask_pos < _Threshold)
                    clip(-1);

                return col;
            }
            
            ENDHLSL
        }
    }
}