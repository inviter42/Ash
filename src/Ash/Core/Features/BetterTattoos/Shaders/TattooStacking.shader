Shader "Hidden/TattooStacking"
{
    Properties
    {
        _MainTex ("Accumulated Canvas", 2D) = "white" {}
        _TattooTex ("Tattoo", 2D) = "white" {}
        _Color ("Tattoo Color", Color) = (1,1,1,1)
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
            sampler2D _TattooTex;
            float4 _TattooTex_ST;
            fixed4 _Color;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 baseCanvas = tex2D(_MainTex, i.uv);

                float2 tattooUV = TRANSFORM_TEX(i.uv, _TattooTex);

                if (tattooUV.x < 0.0 || tattooUV.x > 1.0 || tattooUV.y < 0.0 || tattooUV.y > 1.0) {
                    return baseCanvas;
                }

                fixed4 tattooCol = tex2D(_TattooTex, tattooUV);

                fixed3 incomingRGB = tattooCol.rgb * _Color.rgb;
                float incomingAlpha = tattooCol.a * _Color.a;

                float finalAlpha = incomingAlpha + baseCanvas.a * (1.0 - incomingAlpha);

                fixed3 finalRGB = baseCanvas.rgb;
                if (finalAlpha > 0.0) {
                    finalRGB = (incomingRGB * incomingAlpha + baseCanvas.rgb * baseCanvas.a * (1.0 - incomingAlpha)) / finalAlpha;
                }

                return fixed4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}
