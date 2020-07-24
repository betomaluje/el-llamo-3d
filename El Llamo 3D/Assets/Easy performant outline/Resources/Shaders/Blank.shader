Shader "Blank" {    
    Properties
    {
        _Stencil ("Stencil", Int) = 6
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1"}
        ZWrite Off

        Stencil 
        {
            Ref [_Stencil]
            Comp Always
            Pass Replace
            ZFail Keep
        }

        Pass 
        {
            Cull Off
            ZTest Always
        
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
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v) 
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target 
            {
                return float4(0,0,0,0.0);
            }

            ENDCG
        }

        Pass
        {
            Cull Off
            ZTest Always

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