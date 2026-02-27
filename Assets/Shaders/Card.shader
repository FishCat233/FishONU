Shader "Unlit/Card"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _DyeEnable ("Dye Enable", Range(0,1)) = 0
        _DyeColor ("Dye Color", Color) = (1,0,0,1)

        _HighlightEnable ("Highlight Enable", Range(0,1)) = 0
        _HighlightColor ("Highlight Color", Color) = (0,0,0,0.77)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            Name "Main"

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                UNITY_FOG_COORDS(1)
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _DyeColor;
            float _DyeEnable;

            float _HighlightEnable;
            float4 _HighlightColor;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {

                // ===== Card Color =====
                fixed4 col = tex2D(_MainTex, i.uv);

                // 计算亮度
                fixed gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // 插值
                fixed3 result = lerp(_DyeColor.rgb, fixed3(1,1,1), gray);
                
                if (_DyeEnable > 0.5f)
                    col = fixed4(result, col.a);

                UNITY_APPLY_FOG(i.fogCoord, col);

                // ===== Highlight Color =====
                if (_HighlightEnable > 0.5f) {
                    col.rgb = lerp(col.rgb, _HighlightColor.rgb, _HighlightColor.a);
                }
                
                return col;
            }
            ENDCG
        }

        Pass {
            Name "CardOutline"
            Tags { "LightMode" = "CardOutline"}
        }
    }
}