Shader "Hidden/TextureStacking"
{
    Properties
    {
        _MainTex ("Accumulated Canvas", 2D) = "white" {}
        _ApplicableTex ("Applicable", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ApplicableTex;
            float4 _ApplicableTex_ST;
            fixed4 _Tint;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 baseCanvas = tex2D(_MainTex, i.uv);

                float2 applicableUV = TRANSFORM_TEX(i.uv, _ApplicableTex);

                if (applicableUV.x < 0.0 || applicableUV.x > 1.0 || applicableUV.y < 0.0 || applicableUV.y > 1.0) {
                    return baseCanvas;
                }

                fixed4 tint = tex2D(_ApplicableTex, applicableUV);

                fixed3 incomingRgb = tint.rgb * _Tint.rgb;
                float incomingAlpha = tint.a * _Tint.a;

                // we have to do alpha pre-multiplication, because the canvas buffer (rtA) is pre-cleared with transparent black
                // doing straight alpha approach will result in an incorrect alpha-blending between the canvas and the tattoo
                float finalAlpha = incomingAlpha + baseCanvas.a * (1.0 - incomingAlpha);

                fixed3 finalRgb = baseCanvas.rgb;
                if (finalAlpha > 0.0) {
                    finalRgb = (incomingRgb * incomingAlpha + baseCanvas.rgb * baseCanvas.a * (1.0 - incomingAlpha)) / finalAlpha;
                }

                return fixed4(finalRgb, finalAlpha);
            }
            ENDCG
        }
    }
}
