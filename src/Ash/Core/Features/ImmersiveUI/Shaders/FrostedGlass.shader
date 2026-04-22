// shader code from this repo
// https://github.com/andydbc/unity-frosted-glass/

Shader "Effects/FrostedGlass"
{
    Properties
    {
        _FrostTex ("Fross Texture", 2D) = "white" {}
        _FrostIntensity ("Frost Intensity", Range(0.0, 4.0)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue" = "Transparent"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uvfrost : TEXCOORD0;
                float4 uvgrab : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FrostTex;
            float4 _FrostTex_ST;

            float _FrostIntensity;

            sampler2D _GrabSharpTexture;
            sampler2D _GrabBlurTexture_0;
            sampler2D _GrabBlurTexture_1;
            sampler2D _GrabBlurTexture_2;
            sampler2D _GrabBlurTexture_3;
            sampler2D _GrabBlurTexture_4;
            sampler2D _Main_Tex;

            float4 _MainTex_TexelSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvfrost = TRANSFORM_TEX(v.uv, _FrostTex);
                o.uvgrab = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noise = tex2D(_FrostTex, i.uvfrost).a;
                float surfSmooth = 1.0 - (noise * _FrostIntensity);

                surfSmooth = clamp(0, 1, surfSmooth);

                float2 screenUV = i.uvgrab.xy / i.uvgrab.w;

                half4 sharp = tex2D(_GrabSharpTexture, screenUV);
                half4 ref00 = tex2D(_GrabBlurTexture_0, screenUV);
                half4 ref01 = tex2D(_GrabBlurTexture_1, screenUV);
                half4 ref02 = tex2D(_GrabBlurTexture_2, screenUV);
                half4 ref03 = tex2D(_GrabBlurTexture_3, screenUV);
                half4 ref04 = tex2D(_GrabBlurTexture_4, screenUV);

                float stepSharp = smoothstep(0.8, 1.0, surfSmooth);
                float step00    = smoothstep(0.6, 0.8, surfSmooth);
                float step01    = smoothstep(0.4, 0.6, surfSmooth);
                float step02    = smoothstep(0.2, 0.4, surfSmooth);
                float step03    = smoothstep(0.0, 0.2, surfSmooth);

                half4 refraction = lerp(ref04, ref03, step03);
                refraction = lerp(refraction, ref02, step02);
                refraction = lerp(refraction, ref01, step01);
                refraction = lerp(refraction, ref00, step00);
                refraction = lerp(refraction, sharp, stepSharp);

                return refraction;
            }
            ENDCG
        }
    }
}
