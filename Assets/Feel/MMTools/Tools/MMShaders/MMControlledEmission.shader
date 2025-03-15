Shader "Universal Render Pipeline/Custom/MMControlledEmission_URP"
{
    Properties
    {
        _TextureSample0("Texture Sample 0", 2D) = "white" {}
        _DiffuseColor("DiffuseColor", Color) = (1,1,1,1)
        _Opacity("Opacity", Range( 0 , 1)) = 1
        [HDR]_EmissionColor("EmissionColor", Color) = (1,1,1,1)
        _EmissionForce("EmissionForce", Float) = 0
        _EmissionFresnelBias("EmissionFresnelBias", Float) = 1
        _EmissionFresnelScale("EmissionFresnelScale", Float) = 1
        _EmissionFresnelPower("EmissionFresnelPower", Float) = 1
        _OpacityFresnelBias("OpacityFresnelBias", Float) = 1
        _OpacityFresnelScale("OpacityFresnelScale", Float) = 1
        _OpacityFresnelPower("OpacityFresnelPower", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            TEXTURE2D(_TextureSample0);
            SAMPLER(sampler_TextureSample0);
            half4 _DiffuseColor;
            half _Opacity;
            half4 _EmissionColor;
            half _EmissionForce;
            half _EmissionFresnelBias;
            half _EmissionFresnelScale;
            half _EmissionFresnelPower;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half3 normalWS = normalize(IN.normalWS);
                half3 viewDirWS = normalize(IN.viewDirWS);
                half fresnel = _EmissionFresnelBias + _EmissionFresnelScale * pow(1.0 - dot(normalWS, viewDirWS), _EmissionFresnelPower);
                
                half4 texColor = SAMPLE_TEXTURE2D(_TextureSample0, sampler_TextureSample0, IN.uv);
                half3 albedo = texColor.rgb * _DiffuseColor.rgb;
                half alpha = texColor.a * _Opacity;
                half3 emission = _EmissionForce * _EmissionColor.rgb * fresnel;
                
                return half4(albedo + emission, alpha);
            }
            ENDHLSL
        }
    }
}
