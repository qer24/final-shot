Shader "Hidden/Custom/AO/BoxBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex VertexToFragment 
            #pragma fragment BlurFragment


            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float2 sampleDirection;
            float4 RenderTextureParams;


            
            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f VertexToFragment (appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //Simple five-tap box blur.
            half BlurFragment (v2f i) : SV_Target{
                float2 sampleVector = sampleDirection / RenderTextureParams.xy;
              
             
                float value = tex2D(_MainTex, i.uv).r;

                float sample0 = tex2D(_MainTex, i.uv + sampleVector).r;
                float sample1 = tex2D(_MainTex, i.uv + 2 * sampleVector).r;
                float sample2 = tex2D(_MainTex, i.uv - sampleVector).r;
                float sample3 = tex2D(_MainTex, i.uv - 2 * sampleVector).r;

                value += sample0, sample1, sample2, sample3;

                value /= 5;

                return value;
            }
            ENDCG
        }
  
    }
}
