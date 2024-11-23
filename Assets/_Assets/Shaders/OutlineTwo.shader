Shader "Custom/OutlineTwo"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,1,1)
        _OutlineWidth ("Outline Width", Range(0.01, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }
        Pass
        {
            Name "Outline"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 viewDir : TEXCOORD0;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata_t v)
            {
                v2f o;

                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                float3 worldViewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                float3 offset = worldNormal * _OutlineWidth * dot(worldViewDir, worldNormal);

                o.pos = UnityObjectToClipPos(v.vertex + float4(offset, 0));
                o.viewDir = worldViewDir;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Name "Base"
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _BaseColor;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _BaseColor;
            }
            ENDCG
        }
    }
}
