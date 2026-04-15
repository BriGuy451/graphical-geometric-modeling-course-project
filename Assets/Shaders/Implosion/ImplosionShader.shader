Shader "6991/Implosion/ImplosionShader"
{
    Properties
    {
        _ConvergencePoint ("ConvergencePoint", Vector) = (0,0,0,0)
        _MeshPercentage ("MeshPercentage", Range(0.0, 1.0)) = 0 
        _AnimationThreshold ("_AnimationThreshold", Range(0.0, 1.0)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "ForceNoShadowCast"= "true" }

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
            
            float3 _ConvergencePoint;
            float _MeshPercentage;
            float _AnimationThreshold;
            
            StructuredBuffer<float3> _Data;
            int _VertCount;

            float3 GetTargetVert(float3 source)
            {
                [loop]
                for (int i = 0; i < _VertCount - 1; i++) 
                {
                    if (all(abs(_Data[i] - source) < 1e-5))
                    {
                        return _Data[i + 1];
                    }
                }

                return float3(0, 0, 0);
            }

            v2f vertex_shader (appdata_base v)
            {
                // 0 this vertex is at it's initial position
                // as we move towards 1, the vertex lerps toward the convergence point
                // lerp(init_vertex_position, convergence_point, interpolator) = new vertex position
                v2f o;

                float3 object_pos = v.vertex.xyz;

                float3 world_pos = mul(unity_ObjectToWorld, float4(object_pos, 1.0));

                float3 mapped_vert_world = GetTargetVert(world_pos);
                
                float3 mapped_vert_object = mul(unity_WorldToObject, float4(mapped_vert_world, 1.0));

                // float3 converge_direction_to_init_pos = (object_pos - _ConvergencePoint) * _MeshPercentage;
                float3 object_final = lerp(object_pos, mapped_vert_object, _AnimationThreshold);
                // float3 object_final = lerp(object_pos, GetTargetVert(v.vertex.xyz), _AnimationThreshold);

                v.vertex.xyz = float4(object_final, 1);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;

                return o;
            }

            half4 fragment_shader(v2f i) : SV_Target 
            {
                return half4(.4,.4,.4,1);
            }

            ENDHLSL
        }
    }
}