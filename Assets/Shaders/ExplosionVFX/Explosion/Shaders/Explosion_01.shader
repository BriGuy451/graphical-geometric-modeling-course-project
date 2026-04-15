Shader "6991/ExplosionVFX/Explosion_01Shader"
{
    Properties
    {
        [NoScaleOffset] _ExplosionFlipbook ("ExplosionFlipbook", 2D) = "" {}
        [NoScaleOffset] _ExplosionFlipbook3D ("ExplosionFlipbook3D", 3D) = "" {}
        [Toggle] _is3D ("3D", Integer) = 0
        _FlipbookDimensions ("FlipbookDimensions", Vector) = (0, 0, 0, 0)
        _FrameIndex ("FrameIndex", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
                float3 uvw: TEXCOORD1;
            };
            
            int _is3D;
            sampler2D _ExplosionFlipbook;
            sampler3D _ExplosionFlipbook3D;
            float2 _FlipbookDimensions;
            float _FrameIndex;

            float2 flipbook_uv_remap(float2 uv, float width, float height, float frame, float2 invert) {
                frame = floor(fmod(frame + float(0.00001), width*height));

                float2 tileCount = float2(1.0, 1.0) / float2(width, height);

                float base = floor((frame + float(0.5)) * tileCount.x);

                float tileX = (frame - width * base);

                float tileY = (invert.y * height - (base + invert.y * 1));

                float2 remapped_uv = (uv + float2(tileX, tileY)) * tileCount;

                return remapped_uv;
            }

            v2f vertex_shader (appdata_base v)
            {
                v2f o;
                
                o.uvw = (v.vertex + 0.5);

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.texcoord;

                return o;
            }

            half4 fragment_shader (v2f i) : SV_Target
            {
                float2 flipbook_uv = flipbook_uv_remap(i.uv, _FlipbookDimensions.x, _FlipbookDimensions.y, _FrameIndex, float2(0,1));
                
                half4 color = half4(0,0,0,0);

                if (_is3D > 0){
                    color = tex3D(_ExplosionFlipbook3D, i.uvw);
                } else {
                    color = tex2D(_ExplosionFlipbook, flipbook_uv);
                }

                float3 color_a = color.rgb * color.a;
                color.rgb = color_a;
                
                return color;
            }
            ENDHLSL
        }
    }
}
