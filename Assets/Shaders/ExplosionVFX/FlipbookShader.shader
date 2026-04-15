Shader "6991/ExplosionVFX/FlipbookShader"
{
    Properties
    {
        [NoScaleOffset] _Flipbook ("Flipbook", 2D) = "" {}
        _FlipbookDimensions ("FlipbookDimensions", Vector) = (0, 0, 0, 0)
        _FrameIndex ("FrameIndex", Float) = 0
        [Toggle] _LUT ("LUT", Integer) = 0
        [Toggle] _Darken ("Darken", Integer) = 0
        _DarkenMultiplyScaler ("DarkenScaler", Range(0, 1)) = 0.8
        _LUTColorChoice ("LookUpTextureChoice", Integer) = 0
        [NoScaleOffset] _LookUpTexture ("LookUpTexture", 2D) = "" {}
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
            };

            sampler2D _Flipbook;
            float2 _FlipbookDimensions;
            float _FrameIndex;
            int _LUT;
            int _Darken;
            float _DarkenMultiplyScaler;
            int _LUTColorChoice;
            sampler2D _LookUpTexture;

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
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.texcoord;

                return o;
            }

            float2 get_lut_color(int lut_choice) {
                float2 uv_sample_point = float2(0,0);
                
                switch(lut_choice){
                    // white
                    case 0:
                        uv_sample_point = float2(.1,.1);
                        break;
                    // lightgrey
                    case 1:
                        uv_sample_point = float2(.4,1);
                        break;
                    // darkgrey
                    case 2:
                        uv_sample_point = float2(.65,1);
                        break;
                    // black
                    case 3:
                        uv_sample_point = float2(.85,1);
                        break;
                    
                    // pink
                    case 4:
                        uv_sample_point = float2(.1,.4);
                        break;
                    // teal
                    case 5:
                        uv_sample_point = float2(.4,.4);
                        break;
                    // leaf green
                    case 6:
                        uv_sample_point = float2(.65,.4);
                        break;
                    // gold
                    case 7:
                        uv_sample_point = float2(.8,.4);
                        break;

                    // orange
                    case 8:
                        uv_sample_point = float2(.1,.65);
                        break;
                    // purple
                    case 9:
                        uv_sample_point = float2(.4,.65);
                        break;
                    // deep green
                    case 10:
                        uv_sample_point = float2(.65,.65);
                        break;
                    // light brown
                    case 11:
                        uv_sample_point = float2(.8,.65);
                        break;
                    
                    // red
                    case 12:
                        uv_sample_point = float2(.1,.9);
                        break;
                    // blue
                    case 13:
                        uv_sample_point = float2(.4,.9);
                        break;
                    // green
                    case 14:
                        uv_sample_point = float2(.65,.9);
                        break;
                    // dark brown
                    case 15:
                        uv_sample_point = float2(.8,.9);
                        break;
                }

                return uv_sample_point;
            }

            half4 fragment_shader (v2f i) : SV_Target
            {
                float2 flipbook_uv = flipbook_uv_remap(i.uv, _FlipbookDimensions.x, _FlipbookDimensions.y, _FrameIndex, float2(0,1));

                half4 color = tex2D(_Flipbook, flipbook_uv);

                float3 color_a = color.rgb * color.a;
                
                if (_LUT){
                    float2 sample_point = get_lut_color(_LUTColorChoice);
                    float3 lut_color = tex2D(_LookUpTexture, sample_point);
                    if (_Darken) {
                        lut_color = lut_color * _DarkenMultiplyScaler;
                    }
                    color_a = color.rgb * lut_color;
                }

                color.rgb = color_a;
                
                return color;
            }
            ENDHLSL
        }
    }
}
