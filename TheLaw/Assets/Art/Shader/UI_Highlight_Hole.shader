Shader "UI/HighlightHole"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _BgColor("BgColor", Color) = (0,0,0,0.85)
        _HoleCenter("HoleCenter", Vector) = (0,0,0,0)
        _HoleSize("HoleSize", Vector) = (300,200,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BgColor;
            float2 _HoleCenter;
            float2 _HoleSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pos = i.worldPos.xy;
                float2 halfSize = _HoleSize * 0.5f;
                float2 min = _HoleCenter - halfSize;
                float2 max = _HoleCenter + halfSize;

                // 在洞里 → 完全透明
                if (pos.x > min.x && pos.x < max.x && pos.y > min.y && pos.y < max.y)
                {
                    discard;
                }

                return _BgColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}