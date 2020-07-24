Shader "Unlit/Outline"
{
    Properties
    {
        _Stencil ("Mask stencil", Int) = 6
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Stencil 
        {
            Ref [_Stencil]
            Comp NotEqual
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 depth : TEXCOORD0;
            };

            float4 _Color;
            sampler2D _Depth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_DEPTH(o);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                return _Color + float4(0.01f, 0.01f, 0.01f, 0);
            }
            ENDCG
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
            };

            sampler2D _AlphaTexture;
            float _Cutout;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 texColor = tex2D(_AlphaTexture, i.uv);
                clip(texColor.a - _Cutout);
                return _Color + float4(0.01f, 0.01f, 0.01f, 0);
            }
            ENDCG
        }
    }
}
