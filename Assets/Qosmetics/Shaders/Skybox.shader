Shader "Qosmetics/Skybox"
{
    Properties
    {
        _Color ("Main Color", Color) = (0,0,0,0)
        _RimColor ("Rim Color", Color) = (0,0,0,0)
        _Intensity ("Intensity", float) = 0
    }
    SubShader
    {
        // No culling or depth

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
                float4 screenPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            sampler2D _MainTex;
            float _Intensity;
            float4 _Color;
            float4 _RimColor;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;

                //vignette
                uv -= 0.5;
                
                float vig = length(uv);

                // coloring shit
                fixed4 col = (_Color * vig) + ((1 - vig) * _RimColor) * _Intensity;
                col.a = 0;
                return col;
            }
            ENDCG
        }
    }
}
