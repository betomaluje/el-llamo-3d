Shader "OutlinePostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                float4x4 mvp = UNITY_MATRIX_MVP;
                o.vertex = mul(mvp, v.vertex);
				
                o.uv = v.uv;

                return o;
            }

            sampler2D _MainTex;
            half4 _MainTex_ST;

            float4 _OutlinePostProcessDirection;

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                half4 first = tex2D(_MainTex, uv + float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y));
                half4 second = tex2D(_MainTex, uv - float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y));
                half4 center = tex2D(_MainTex, uv);    
                half4 maxResult = max(max(first, second), center); 
                maxResult.a = (first.a + second.a + center.a) / 3.0f;

                half4 avgResult = (first + second + center) / 3.0f;

                return lerp(maxResult, avgResult, first.a * second.a * center.a);
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4x4 mvp = UNITY_MATRIX_MVP;
                o.vertex = mul(mvp, v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            half4 _MainTex_ST;

            float4 _OutlinePostProcessDirection;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                half4 first = tex2D(_MainTex, uv + float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y));
                half4 second = tex2D(_MainTex, uv - float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y));
                half4 center = tex2D(_MainTex, uv);

                return max(max(first, second), center);
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4x4 mvp = UNITY_MATRIX_MVP;
                o.vertex = mul(mvp, v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            half4 _MainTex_ST;

            sampler2D _InfoBuffer;

            float4 _OutlinePostProcessDirection;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                half4 info = tex2D(_InfoBuffer, uv);
                half4 first = tex2D(_MainTex, uv + float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y) * info.r);
                half4 second = tex2D(_MainTex, uv - float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y)* info.r);
                half4 center = tex2D(_MainTex, uv);    
                half4 maxResult = max(max(first, second), center); 
                maxResult.a = (first.a + second.a + center.a) / 3.0f;

                half4 avgResult = (first + second + center) / 3.0f;

                return lerp(maxResult, avgResult, first.a * second.a * center.a);
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4x4 mvp = UNITY_MATRIX_MVP;
                o.vertex = mul(mvp, v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            half4 _MainTex_ST;
            
            sampler2D _InfoBuffer;

            float4 _OutlinePostProcessDirection;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST);
                half4 info = tex2D(_InfoBuffer, uv);
                half4 first = tex2D(_MainTex, uv + float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y) * info.g);
                half4 second = tex2D(_MainTex, uv - float2(_OutlinePostProcessDirection.x, _OutlinePostProcessDirection.y)* info.g);
                half4 center = tex2D(_MainTex, uv);

                return max(max(first, second), center);
            }
            ENDCG
        }
    }
}