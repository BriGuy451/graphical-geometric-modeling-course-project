Shader "6991/Implosion/ImplosionShaderSimple"
{
    Properties
    {
        [MainTexture] [NoScaleOffset] _MainTex ("MainTexture", 2D) = "white" {}
        _CurrentChannel ("CurrentChannel", Integer) = 1
        _ConvergencePoint ("ConvergencePoint", Vector) = (0,0,0,0)
        _ChannelOneAnimation ("ChannelOneAnimation", Range(0.0, 1.0)) = 0
        _ChannelTwoAnimation ("ChannelTwoAnimation", Range(0.0, 1.0)) = 0
        _ChannelThreeAnimation ("ChannelThreeAnimation", Range(0.0, 1.0)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader
            
            #include "UnityCG.cginc"

            struct special_channel_vert {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
                float4 texcoord2 : TEXCOORD2;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                int texcoordval : TEXCOORD1;
                
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            int _CurrentChannel;
            float _ChannelOneAnimation;
            float _ChannelTwoAnimation;
            float _ChannelThreeAnimation;
            float3 _ConvergencePoint;

            v2f vertex_shader (special_channel_vert v)
            {
                v2f o;
                o.texcoordval = 0;

                float3 object_final = v.vertex.xyz;
                if (v.texcoord2.x == 1) {
                    o.texcoordval = v.texcoord2.x;
                    
                    float3 object_pos = object_final;
                    object_final = lerp(object_pos, _ConvergencePoint, _ChannelOneAnimation);
                }

                if (v.texcoord2.x == 2){
                    o.texcoordval = v.texcoord2.x;
                    
                    float3 object_pos = object_final;
                    object_final = lerp(object_pos, _ConvergencePoint, _ChannelTwoAnimation);
                }

                if (v.texcoord2.x == 3){
                    o.texcoordval = v.texcoord2.x;
                    
                    float3 object_pos = object_final;
                    object_final = lerp(object_pos, _ConvergencePoint, _ChannelThreeAnimation);
                }

                v.vertex.xyzw = float4(object_final, 1);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            half4 fragment_shader(v2f i) :SV_Target
            {
                half4 color = tex2D(_MainTex, i.uv);
                return color;
            }

            ENDHLSL
        }
    }
}