Shader "6991/Suction/SuctionShaderFrag"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset] _NoiseTex ("Noise Texture", 2D) = "white" {}
        [NoScaleOffset] _RadialTex ("Radial Texture", 2D) = "white" {}
        _FlowSpeed ("Flow Speed", float) = 5
        _Threshold ("Time Thresh", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCast" = "true" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader

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
                float3 world_pos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _NoiseTex;
            sampler2D _RadialTex;
            float _Threshold;
            float _FlowSpeed;

            v2f vertex_shader (appdata vert)
            {
                v2f vert_to_frag;

                vert_to_frag.world_pos = UnityObjectToWorldDir(vert.vertex);
                vert_to_frag.vertex = UnityObjectToClipPos(vert.vertex);
                vert_to_frag.uv = vert.uv;

                return vert_to_frag;
            }

            float radial_mask(float2 uv){
                float2 center = float2(0.5, 0.5);
                float dist = length(uv - center);
                return saturate(1.0 - dist * 2.0);
            }

            fixed4 fragment_shader(v2f i) : SV_Target {
                float2 uvPlanar = i.world_pos.xz * 1;

                float2 uvPlanarAnim = uvPlanar + float2(.1, .1) * _Time.y;

                float noise = tex2D(_NoiseTex, uvPlanarAnim);
                float radial = tex2D(_RadialTex, uvPlanarAnim);

                float mask = noise + radial;

                float2 distorted = i.uv + (mask - 0.5) * .025;

                fixed4 col = tex2D(_MainTex, distorted);

                return col;
            }
            
            ENDHLSL
        }
    }
}
