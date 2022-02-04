Shader "Qosmetics/Mirrorable"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        [MaterialToggle]_CustomColors("CustomColors", Float) = 0
        _StencilRefID("Stencil ID Reference", Float) = 0

        // Variables used by qosmetics internally to make sure mirroring works, you should basically never edit these manually unless you know what you are doing (that's why they are hidden)
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _BlendSrcFactor("Blend Src", Float) = 1
        [HideInInspector] _BlendDstFactor("Blend Dst", Float) = 0
        [HideInInspector] _BlendSrcFactorA("Blend Src Factor A", Float) = 0
        [HideInInspector] _BlendDstFactorA("Blend Dst Factor A", Float) = 0
        [HideInInspector] _Alpha("Alpha", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        // Used by the game to make the notes visible in the mirror
        Stencil
        {
            Ref[_StencilRefID]
            Comp[_StencilComp]
            Pass[_StencilOp]
        }
        // Blend is mostly handled by Qosmetics for solid objects. If you are using transparent materials for your cyoob you might want to make these not use the variables
        // IF YOU ARE PUTTING IN VALUES MANUALLY HERE, MAKE SURE THAT YOU TEST THE CYOOBS BEFORE RELEASING THEM
        Blend [_BlendSrcFactor] [_BlendDstFactor], [_BlendSrcFactorA] [_BlendDstFactorA]

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            // the usual, declare a variable to use it
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;

                // the multiplication by the _Alpha variable is neccesary for the mirrorred version to properly be less visible
                return float4 (col.rgb, col.a * _Alpha);
            }
            ENDCG
        }
    }
}
