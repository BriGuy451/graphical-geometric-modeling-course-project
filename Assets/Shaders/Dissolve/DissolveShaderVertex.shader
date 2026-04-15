Shader "6991/Dissolve/DissolveShaderVertex"
{
    Properties
    {
        [MainTexture] [NoScaleOffset] _MainTex ("MainTexture", 2D) = "white" {}
        [NoScaleOffset] _NoiseTex ("NoiseTexture", 2D) = "red" {}
        _Threshold ("EffectThreshold", Range(0.0, 1.0)) = 0
        _MinY ("MinimumY", float) = 0
        _MaxY ("MaximumY", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCast"= "true" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vertexBreakdown
            #pragma fragment passthroughFragment
            
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
            
            float rand(float2 uv) { return frac(sin(dot(uv, float2(12.9898,78.233))) * 43758.5453); }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float _Threshold;
            float _MinY;
            float _MaxY;
            
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            v2f vertexBreakdown(appdata v)
            {
                // compute mask to determine if this vertex should be collapsed or not.
                float vertex_position_mask = saturate((v.vertex.y - _MinY) / (_MaxY - _MinY));
                
                float4 texture_coordinates = float4(v.uv.x, v.uv.y, 0, 0);
                
                float4 noise_mask = tex2Dlod(_NoiseTex, texture_coordinates);
                
                // if (noise_mask.x < _Threshold)
                //     v.vertex = float4(0, 0, 0, 1);
                
                if (vertex_position_mask < _Threshold)
                    v.vertex = float4(0, 0, 0, 1);

                v2f vertexPassedToFragment;
                vertexPassedToFragment.vertex = UnityObjectToClipPos(v.vertex);
                vertexPassedToFragment.uv = v.uv;
                
                return vertexPassedToFragment;
            }

            fixed4 passthroughFragment(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            
            ENDHLSL
        }
    }
}