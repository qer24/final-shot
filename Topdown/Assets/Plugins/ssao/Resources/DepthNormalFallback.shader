// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Custom/Deferred/DepthNormalFallback"
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
	
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			

		
            struct appdata
            {
                float4 vertex : POSITION0;
                float2 uv : TEXCOORD1;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float depth : POSITION1;
            };



            v2f vert (appdata v)
            {
                v2f o;

				float3 vertexPosition = v.vertex;
				float3 worldPosition = mul(unity_ObjectToWorld, vertexPosition).xyz;

				o.normal = mul(unity_ObjectToWorld,float4(v.normal,0));
				o.depth = - UnityObjectToViewPos(v.vertex).z;
				o.vertex = UnityObjectToClipPos(vertexPosition);
				
                return o;
            }



			float4 frag(v2f i) : SV_Target
			{
				return float4(i.normal,i.depth);
            }
            ENDCG
        }
    }
}
