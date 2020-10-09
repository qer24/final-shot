Shader "Hidden/Custom/AO/Calculate"
{
    Properties
    {
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma multi_compile_local _ FLIPAONORMAL
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define PI 3.141596 
            #define BIAS 0.002
            #define EPSILON 0.00001

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



            sampler2D _MainTex;
            sampler2D faceDepthNormalTexture;

            float4x4 viewMatrix;
            float4x4 inverseViewMatrix;
            float4x4 projectionMatrix;
            float4x4 inverseProjectionMatrix;

            int sampleCount;
            float sampleRange;
            
            float Random(float u, float v)
            {
                float f = dot(float2(12.9898, 78.233), float2(u, v));
                return frac(43758.5453 * sin(f));
            }

            // Uniformaly distributed points on a unit sphere
            // http://mathworld.wolfram.com/SpherePointPicking.html
            float3 GetPointOnSphere(float a, float b){


                float u = a * 2.0 - 1.0;
                float theta = b * 2 * PI;
                float sinTheta, cosTheta;
                sincos(theta, sinTheta, cosTheta);

                float3 v = float3(float2(cosTheta,sinTheta) * sqrt(1.0 - u * u), u);
                
                return v;
                
            }

            float3 GetPointOnHemisphere(float a, float b, float3 direction) {
                float3 sphereDirection = GetPointOnSphere(a, b);
                sphereDirection = faceforward(sphereDirection, -direction, sphereDirection);
                return sphereDirection;
            }
 
            float3 GetSampleVector(float2 uv, float time, float index, float3 direction)
            {
                return GetPointOnHemisphere(Random(uv.x + time, uv.y + index), Random(-uv.x - time, uv.y + index), direction);
            }

            float2 ViewToUVPosition(float3 view) {
                float orthoDistance = lerp(-view.z, 1.0, unity_OrthoParams.w);
                float3 screenSpacePosition = mul(projectionMatrix, float4(view, 1));
                return ((screenSpacePosition.xy / orthoDistance) + 1.0) / 2.0;
            }

            float3 UVToViewPosition(float2 uv, float depth)
            {
                float orthoDistance = lerp(depth, 1.0, unity_OrthoParams.w);
                float3 viewPosition = mul(inverseProjectionMatrix, float4((uv * 2.0 - 1.0) * orthoDistance, -depth, 1)).xyz;
                viewPosition.z = -depth;
                return viewPosition;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex =  UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            // Calculate AO using the method described in "Alchemy screen-space ambient obscurance algorithm".
            //https://research.nvidia.com/publication/alchemy-screen-space-ambient-obscurance-algorithm
            half frag(v2f i) : SV_Target
            {

                float4 screenColour = tex2D(_MainTex, i.uv);
                float4 rawDepthNormal = tex2D(faceDepthNormalTexture, i.uv);

                float3 normal = rawDepthNormal.xyz;
                float depth = rawDepthNormal.w;

                float3 viewSpacePosition = UVToViewPosition(i.uv, depth);
                float3 viewSpaceNormal = mul(viewMatrix, float4(normal, 0));
                


                #if defined(FLIPAONORMAL)
                    viewSpaceNormal *= -1;
                #endif

                float AOValue = 0;
                for (int s = 0; s < sampleCount; s++)
                {
                    //Get a random point within a hemisphere.
                    float3 sampleVector = GetSampleVector(i.uv,_Time.x, s, viewSpaceNormal);             
                    sampleVector *= sampleRange;

                    //Get depth at sample point.
                    float3 viewSpaceSample = viewSpacePosition + sampleVector;
                    float2 UVSpaceSample = ViewToUVPosition(viewSpaceSample);
                    //TODO(Bryn) define out-of-bounds value. 
                    float sampleDepth = tex2D(faceDepthNormalTexture, UVSpaceSample).w;
                 
                    //Get collision with world. 
                    float3 viewSpaceIntersection = UVToViewPosition(UVSpaceSample, sampleDepth);
                    float3 toSample = viewSpaceIntersection - viewSpacePosition;
                      
                    float a = max(dot(toSample, viewSpaceNormal) - BIAS * depth, 0);
                    float b = dot(toSample, toSample) + EPSILON;

                    AOValue += a / b;
                }
                AOValue /= sampleCount;

                AOValue *= sampleRange;

                AOValue = min(AOValue,1);

                return AOValue;
            }
            ENDCG
        }
    }
}
