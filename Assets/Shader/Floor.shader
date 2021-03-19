Shader "MyShader/Floor"
{
    Properties
    {
        //testFloat ("testFloat", range(0.1, 1)) = 1
        flowDuration ("flowDuration", range(1, 2)) = 1
        groundTex ("groundTex", 2D) = "white"
        magmaTex ("magmaTex", 2D) = "white"
        maskTex ("maskTex", 2D) = "white"
        flowMap ("flowMap", 2D) = "white"
        matcapTex ("matcapTex", 2D) = "white"
    }
    SubShader
    {
        Name "my_floor"
        Tags { "RenderType"="Opaque" }
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vs_main
            #pragma fragment ps_main
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 vnormal : NORMAL;
                float3 vpos : TEXCOORD4;
            };

            sampler2D groundTex;
            sampler2D magmaTex;
            sampler2D maskTex;
            sampler2D flowMap;
            sampler2D matcapTex;
            float4 groundTex_ST;
            float testFloat;
            float flowDuration;

            v2f vs_main (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = v.uv;
                o.vnormal = mul((float3x3)UNITY_MATRIX_MV, v.normal);
                o.vpos = UnityObjectToViewPos(v.pos);
                return o;
            }

            float4 flow_map(sampler2D base, float2 uv, float2 tiling, float t) {
                float4 f = tex2D(flowMap, uv) * 2 - 1;
                float4 o = tex2D(base, uv * tiling + f.xy * t);
                return o;
            }

            float4 matcap(float3 vpos, float3 vnormal) {
                float3 N = normalize(vnormal);
                float3 V = normalize(vpos);
                N -= V * dot(N, V);
                float4 o = tex2D(matcapTex, N.xy * 0.99 * 0.5 - 0.5);
                return o;
            }

            float4 ps_main (v2f i) : SV_Target
            {
                float2 tiling = groundTex_ST.xy;
                fixed4 c1 = tex2D(groundTex, i.uv * tiling);
                float T = _Time.y / flowDuration;
                float t1 = T % 2;
                float t2 = (T + 1) % 2;
                float4 flow1 = flow_map(magmaTex, i.uv, tiling, t1);
                float4 flow2 = flow_map(magmaTex, i.uv, tiling, t2);
                float wFlow = abs(T % 2 - 1);
                float4 c2 = lerp(flow1, flow2, wFlow) * matcap(i.vpos, i.vnormal);
                float4 w = tex2D(maskTex, (i.uv - 0.5) * tiling * max(1 / tiling, testFloat) + 0.5);
                float4 o = lerp(c1, c2, w);
                return o;
            }
            ENDCG
        }
    }
}
