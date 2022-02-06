Shader "Qosmetics/Debris"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [MaterialToggle] _CustomColors("Custom Colors", Float) = 1

        // Variables used by qosmetics internally to make sure the slice works, you should basically never edit these manually unless you know what you are doing (that's why they are hidden)
        [HideInInspector] _SlicePos("Slice Local Position", Vector) = (0,0,0)
        [HideInInspector] _CutPlane("Slice Local Normal", Vector) = (0,0,0)
        [HideInInspector] _TransformOffset("Transform Offset", Vector) = (0,0,0)
        _SliceEdge("Slice Glow Edge", Float) = 0.025

    }
        SubShader
        {
        Cull Off
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Qosmetics/CGINC/slice.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 localPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 _SlicePos;
            half3 _CutPlane;
            float3 _TransformOffset;
            float _SliceEdge;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (slice(_CutPlane, _SlicePos, i.localPos, _TransformOffset, _SliceEdge)) {
                    discard;
                }
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                
                return col;
            }
            ENDCG
        }
    }
}
