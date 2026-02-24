Shader "Unlit/CardOutlineFeature"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _EnableOutline ("Enable Outline", float) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _EnableOutline;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;

                if (_EnableOutline > 0.5) {
                    // 顶点外扩
                //    float2 center = float2(0,0);
                //    float2 dir = normalize(v.vertex.xy - center.xy);
                //    v.vertex.xy += dir * _OutlineWidth;
                }

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor * (sin(_Time.y * 5) * 0.5 + 0.8);
            }
            ENDCG
        }
    }
}
