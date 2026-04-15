Shader "6991/EbertVolumeRendering/VolumeSolidSpace"
{
    Properties 
    {
        _StepSize ("StepSize", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader

            #include "UnityCG.cginc" // could do my reusable graphic methods like this.

            struct v2f 
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 world_pos : TEXCOORD1;
            };

            Texture3D _Random3D;
            sampler3D sampler_Random3D;
            float4x4 _WorldToVolume;
            float _StepSize;

            v2f vertex_shader(appdata_base v)
            {
                v2f output;
                
                output.world_pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                output.vertex = UnityObjectToClipPos(v.vertex);
                output.uv = v.texcoord;

                return output;
            }

            bool RayBoxIntersection(float3 rayOrigin, float3 rayDir, float3 boxMin, float3 boxMax, out float tEnter, out float tExit)
            {
                float3 t0 = (boxMin - rayOrigin) / rayDir;
                float3 t1 = (boxMax - rayOrigin) / rayDir;

                float3 tmin = min(t0, t1);
                float3 tmax = max(t0, t1);

                tEnter = max(max(tmin.x, tmin.y), tmin.z);
                tExit  = min(min(tmax.x, tmax.y), tmax.z);

                return tExit >= tEnter;
            }

            float calc_noise(float3 pnt, float size)
            {
                // Convert to integer lattice coordinates
                int px = (int)pnt.x;
                int py = (int)pnt.y;
                int pz = (int)pnt.z;

                int x = px & ((int)size - 1);
                int y = py & ((int)size - 1);
                int z = pz & ((int)size - 1);

                // Fractional offsets
                float tx = pnt.x - px;
                float ty = pnt.y - py;
                float tz = pnt.z - pz;

                // Normalize to [0,1] for texture sampling
                float3 uv000 = float3(x, y, z) / size;
                float3 uv001 = float3(x, y, z+1) / size;
                float3 uv010 = float3(x, y+1, z) / size;
                float3 uv011 = float3(x, y+1, z+1) / size;
                float3 uv100 = float3(x+1, y, z) / size;
                float3 uv101 = float3(x+1, y, z+1) / size;
                float3 uv110 = float3(x+1, y+1, z) / size;
                float3 uv111 = float3(x+1, y+1, z+1) / size;

                // Sample corners
                float c000 = tex3D(sampler_Random3D, uv000).r;
                float c001 = tex3D(sampler_Random3D, uv001).r;
                float c010 = tex3D(sampler_Random3D, uv010).r;
                float c011 = tex3D(sampler_Random3D, uv011).r;
                float c100 = tex3D(sampler_Random3D, uv100).r;
                float c101 = tex3D(sampler_Random3D, uv101).r;
                float c110 = tex3D(sampler_Random3D, uv110).r;
                float c111 = tex3D(sampler_Random3D, uv111).r;

                // Interpolate along Y
                float p_l2 = lerp(c000, c010, ty);
                float p_r2 = lerp(c100, c110, ty);
                float p_l  = lerp(c001, c011, ty);
                float p_r  = lerp(c101, c111, ty);

                // Interpolate along X
                float p_face2 = lerp(p_l2, p_r2, tx);
                float p_face1 = lerp(p_l,  p_r,  tx);

                // Interpolate along Z
                float p_final = lerp(p_face2, p_face1, tz);

                return p_final;
            }

            float turbulence(float3 pnt, float pixel_size, float size)
            {
                float t = 0.0;
                float scale = 1.0;

                // Ebert: accumulate noise at decreasing scales
                // while scale > pixel_size
                while (scale > pixel_size)
                {
                    float3 p = pnt / scale;   // Ebert divides coordinates by scale
                    t += calc_noise(p, size) * scale;
                    scale *= 0.5;             // divide scale by 2.0 each iteration
                }

                return t;
            }

            void TransferFunction(float density, out float3 color, out float alpha)
            {
                /* 1. Normalize or remap density into a usable shading range */
                float d = saturate(density);

                /* 2. Compute opacity from density (absorption/emission strength) */
                alpha = d; // placeholder

                /* 3. Compute color from density (color ramp, tint, emission) */
                color = float3(d, d, d); // placeholder

                /* 4. Apply artistic shaping (contrast, curves, ramps) */
                color = pow(color, 1.2); // example
                alpha = smoothstep(0.1, 0.8, alpha);

                /* 5. Final clamp */
                alpha = saturate(alpha);
                color = saturate(color);
            }

            float EvaluateDensity(float3 p)
            {
                /* 1. Sample base noise (your calc_noise) */
                float baseNoise = calc_noise(p, 64);

                /* 2. Sample turbulence (your turbulence function) */
                float turb = turbulence(p, 1, 64);

                /* 3. Combine noise sources into a raw density */
                float rawDensity = baseNoise + turb;

                /* 4. Apply shaping masks (falloff, radial, height, etc.) */
                float shapedDensity = rawDensity; // placeholder

                /* 5. Remap, clamp, or bias the density */
                float finalDensity = saturate(shapedDensity);

                return finalDensity;
            }


            float4 fragment_shader(v2f i) : SV_Target
            {
                /* 1. Compute ray origin and direction in the volume’s space */
                float3 rayOriginWS = _WorldSpaceCameraPos;
                float3 rayDirWS = normalize(i.world_pos - _WorldSpaceCameraPos);

                // Transform ray into the volume’s local/object space
                float3 rayOrigin = mul(_WorldToVolume, float4(rayOriginWS, 1.0)).xyz;
                float3 rayDir = normalize(mul(_WorldToVolume, float4(rayDirWS, 0.0)).xyz);

                // return float4(rayOrigin, 1.0);
         
                /* 2. Intersect the ray with the volume bounds to find entry and exit */
                float tEnter, tExit;
                bool hit = RayBoxIntersection(rayOrigin, rayDir, float3(-0.5,-0.5,-0.5), float3(0.5,0.5,0.5), tEnter, tExit);

                // float3 debug = mul(_WorldToVolume, float4(i.world_pos, 1)).xyz;
                // float4 color = (0,0,0,1);
                // color.xyz = debug;
                // return color;

                if (!hit || tExit < 0.0)
                    return float4(0,0,0,0); // Ray misses the volume entirely

                // Clamp entry to avoid stepping behind the camera
                tEnter = max(tEnter, 0.0);
                
                /* 3. Initialize accumulators for color, opacity, and ray position */
                /* 3. Initialize accumulators for color, opacity, and ray position */
                float3 accumColor = float3(0,0,0);
                float accumAlpha = 0.0;
                
                float t = tEnter;          // Current ray distance
                float tEnd = tExit;        // End of ray segment inside the volume
                // tEnd = min(tExit, 1.0);
                float stepSize = _StepSize; // User-defined or auto-tuned step length

                #define MAX_STEPS 128

                /* 4. Raymarch loop */
                [loop]
                for (int step = 0; step < MAX_STEPS && t < tEnd; step++)
                {
                    /* 4.1 Compute current sample position along the ray */
                    float3 samplePos = rayOrigin + rayDir * t;

                    /* 4.2 Sample the density field at this position */
                    float density = EvaluateDensity(samplePos);

                    /* 4.3 Convert density to color and opacity */
                    float3 sampleColor;
                    float sampleAlpha;
                    TransferFunction(density, sampleColor, sampleAlpha);

                    /* 4.4 Composite sample using emission and absorption */
                    sampleAlpha *= (1.0 - accumAlpha);
                    accumColor += sampleColor * sampleAlpha;
                    accumAlpha += sampleAlpha;

                    // if (any(samplePos < 0.0) || any(samplePos > 1.0))
                    //     break;

                    /* 4.5 Early exit if opacity is saturated */
                    if (accumAlpha >= 0.99)
                        break;
                    
                    t += stepSize;
                }

                /* 5. Return the final accumulated color */
                return float4(accumColor, accumAlpha);
            }

            ENDHLSL
        }

    }
}