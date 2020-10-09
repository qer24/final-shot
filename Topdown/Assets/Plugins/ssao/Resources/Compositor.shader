Shader "Hidden/Custom/AO/Compositor"
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

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 colour;
            float multiplier;
            float power;

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
            sampler2D AOTexture;
            sampler2D faceDepthNormalTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex =  UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            float3 frag(v2f i) : SV_Target
            {
            //    return tex2D(faceDepthNormalTexture, i.uv);
                float3 screenColour = tex2D(_MainTex, i.uv);
                float AOValue = tex2D(AOTexture, i.uv).r;
                AOValue = pow(multiplier * AOValue ,power);
                return lerp(screenColour,colour ,AOValue);
            }
            ENDCG
        }
    }
}

