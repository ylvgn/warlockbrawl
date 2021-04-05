Shader "MyShader/Barrier"
{
    Properties
    {
        testFloat("testFloat", Range(0.1, 10)) = 2
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vs_main
            #pragma fragment ps_main

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

            sampler2D _MainTex;
            float testFloat;

            v2f vs_main (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.uv = v.uv;
                return o;
            }

            fixed4 ps_main (v2f i) : SV_Target
            {
                float angle = _Time.y * testFloat % 360;
                float r = radians(angle);
                float2x2 rotate_matrix = float2x2(cos(r), -sin(r), sin(r), cos(r));
                float2 uv = mul(i.uv - 0.5, rotate_matrix) + 0.5;
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}
