Shader "Hidden/Custom/Deferred/DepthNormal"
{
	Properties
	{

	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Pass
		{
		//	   Tags{"LightMode" = "UniversalForward"}
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#include "UnityCG.cginc"
			

		
            struct appdata
            {
                float4 vertex : POSITION0;
                float2 uv : TEXCOORD1;
				float3 normal : NORMAL;
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
				float4 positionAndDepth : POSITION1;
            };


			struct g2f {
				v2g data;
				float3 faceNormal: NORMAL1;
			};


            v2g vert (appdata v)
            {
                v2g o;

				float3 vertexPosition = v.vertex;
				float3 worldPosition = mul(unity_ObjectToWorld, vertexPosition).xyz;
				float linearDepth = - UnityObjectToViewPos(v.vertex).z;

				float4x4 m = UNITY_MATRIX_V;
				o.positionAndDepth = float4(worldPosition,linearDepth);       
				o.vertex = UnityObjectToClipPos(vertexPosition);
				
                return o;
            }

			[maxvertexcount(3)]
			void geom(triangle v2g i[3], inout TriangleStream<g2f> stream) {

				g2f g0, g1, g2;

				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				
				float3 p0 = i[0].positionAndDepth.xyz;
				float3 p1 = i[1].positionAndDepth.xyz;
				float3 p2 = i[2].positionAndDepth.xyz;

				float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));

				g0.faceNormal = triangleNormal;
				g1.faceNormal = triangleNormal;
				g2.faceNormal = triangleNormal;

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}

			float4 frag(g2f i) : SV_Target
			{
				return float4(i.faceNormal,i.data.positionAndDepth.w);
            }
            ENDCG
        }
    }

	 Fallback "Hidden/Custom/Deferred/DepthNormalFallback"
}
