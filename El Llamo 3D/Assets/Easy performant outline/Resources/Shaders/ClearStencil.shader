Shader "ClearStencil" 
{    
    SubShader {
        ColorMask 0
        ZWrite Off

        Stencil 
        {
            Ref 0
            Comp Always
            Pass Replace
            ZFail Replace
        }

        CGINCLUDE
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            float4 frag(v2f i) : SV_Target {
                return half4(0,0,0,1);
            }
        ENDCG

        Pass {
            Cull Back
            ZTest Greater
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    } 
}