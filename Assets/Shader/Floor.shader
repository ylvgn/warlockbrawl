Shader "MyShader/Floor"
{
    Properties
    {
        //testFloat ("testFloat", range(0.1, 1)) = 1
        groundTex ("groundTex", 2D) = "white"
        magmaTex ("magmaTex", 2D) = "white"
        flowMap ("flowMap", 2D) = "white"
        maskTex ("maskTex", 2D) = "white"
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D groundTex;
            sampler2D magmaTex;
            sampler2D maskTex;
            sampler2D flowMap;
            float4 groundTex_ST;
            float testFloat;

            v2f vs_main (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = v.uv;
                return o;
            }

            float4 ps_main (v2f i) : SV_Target
            {
                float2 tiling = groundTex_ST.xy;
                float4 w = tex2D(maskTex, (i.uv - 0.5) * tiling * max(1 / tiling, testFloat) + 0.5);
                float4 f = tex2D(flowMap, i.uv) * 2 - 1;
                float2 offset = lerp(0, f, w.r) * _Time.y / 2;
                fixed4 c1 = tex2D(groundTex, i.uv * tiling + offset);
                float4 c2 = tex2D(magmaTex, i.uv * tiling + offset);
                float4 o = lerp(c1, c2, w);
                return o;
            }
            ENDCG
        }
    }
}
