Shader "Ash/BPExtendedTessellationOffsetCutout"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _BumpMap ("BumpMap", 2D) = "bump" {}
        _OcclusionMap ("OcclusionMap", 2D) = "white" {}

        _Color ("Main Color", Color) = (1,1,1,1)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)

        _BumpScale ("BumpScale", Float) = 1.0
        _Cutoff ("Cutoff", Float) = 0.5
        _Fresnel ("Fresnel", Float) = 1.0
        _Glossiness ("Glossiness", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" "LightMode"="ForwardBase" }

        Pass
        {
            ZWrite On
            ZTest LEqual
            Offset -1, -1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                SHADOW_COORDS(2)
                float3 normal : TEXCOORD3;
                float3 tangent : TEXCOORD4;
                float3 bitangent : TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _BumpMap;
            sampler2D _OcclusionMap;

            fixed4 _Color;
            fixed4 _SpecularColor;

            half _BumpScale;
            half _Cutoff;
            half _Fresnel;
            half _Glossiness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                o.tangent = normalize(UnityObjectToWorldDir(v.tangent.xyz));
                o.bitangent = normalize(cross(o.normal, o.tangent) * v.tangent.w);

                TRANSFER_SHADOW(o);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 albedoSample = tex2D(_MainTex, i.uv);

                clip(albedoSample.a - _Cutoff);

                fixed4 bumpSample = tex2D(_BumpMap, i.uv);
                fixed4 occSample = tex2D(_OcclusionMap, i.uv);

                half3 normalTex = UnpackScaleNormal(tex2D(_BumpMap, i.uv), _BumpScale);

                float3 worldNormalTex = normalize(
                    i.tangent * normalTex.x +
                    i.bitangent * normalTex.y +
                    i.normal * normalTex.z
                );

                float3 N = worldNormalTex;

                // parse alloy area lights data
                fixed3 activeLightColor = _LightColor0.rgb;
                float packedData = _LightColor0.a;

                float specularMask = step(0.0, packedData);
                packedData = abs(packedData);

                float isPositionalLight = _WorldSpaceLightPos0.w;
                float lightRadius = 0.0;
                float lightLength = 0.0;
                float lightRange = 1.0;
                float3 baseLightDir = _WorldSpaceLightPos0.xyz;

                if (isPositionalLight > 0.5)
                {
                    lightRange = 1.0 / sqrt(max(0.0001, _LightPositionRange.w));

                    float lightLengthRatio = floor(packedData) / 1000.0;
                    float lightRadiusRatio = frac(packedData);

                    lightRadius = lightRadiusRatio * lightRange;
                    lightLength = lightLengthRatio * (lightRange * 2.0);

                    baseLightDir = baseLightDir - i.worldPos;
                }
                else
                {
                    lightRadius = packedData / 10.0;
                    lightLength = 0.0;
                }

                float3 V = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float3 R = reflect(-V, N);

                float3 adjustedLightDir = baseLightDir;
                if (isPositionalLight > 0.5 && (lightRadius > 0.0 || lightLength > 0.0))
                {
                    float3 lightPosWorld = _WorldSpaceLightPos0.xyz;
                    float3 centerToRay = dot(lightPosWorld - i.worldPos, R) * R - (lightPosWorld - i.worldPos);
                    float3 shiftDir = normalize(centerToRay + 0.0001);
                    float maxExtent = max(lightRadius, lightLength * 0.5);

                    adjustedLightDir += shiftDir * clamp(length(centerToRay), 0.0, maxExtent);
                }

                float3 L = normalize(adjustedLightDir);
                float3 H = normalize(L + V);

                float NdotL = max(0.0, dot(N, L));
                float NdotH = max(0.0, dot(N, H));
                float VdotH = max(0.0, dot(V, H));

                // calculate pbr components
                float3 baseAlbedo = albedoSample.rgb * _Color.rgb;
                float3 ambientLighting = ShadeSH9(float4(N, 1.0)) * occSample.rgb * baseAlbedo;

                float3 diffuseLighting = baseAlbedo * activeLightColor * NdotL;

                float shininessExponent = 1.0 + (_Glossiness * 127.0);
                float specPower = pow(NdotH, shininessExponent);

                float blinnPhongNormalize = (shininessExponent + 2.0) / 8.0;

                float3 specularLighting = _SpecularColor.rgb * (specPower * blinnPhongNormalize) * _Glossiness * activeLightColor * NdotL * specularMask;

                float NdotV = max(0.0, dot(N, V));
                float rimFresnel = pow(1.0 - NdotV, 5.0) * saturate(_Fresnel);
                float3 fresnelLighting = rimFresnel * activeLightColor * NdotL;

                // attenuation
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                float3 energyConservationFactor = 1.0 - _SpecularColor.rgb;

                fixed4 finalColor = fixed4(0, 0, 0, 0);

                finalColor.rgb = ambientLighting + ((diffuseLighting * energyConservationFactor) + specularLighting + fresnelLighting) * atten;

                return finalColor;
            }
            ENDCG
        }
    }
}
