Shader "Lit/S_SphereInside"
{
    Properties
    {
        _Color ("Color", Color) = (0.6,0.8235,1,1)

		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 5
    }
    SubShader
    {
        Tags { "Queue"="Transparent-1" "RenderType" = "Opaque"}
        LOD 100

		

		ZWrite Off
		Cull Front
		//ZTest LEqual

		Blend SrcAlpha OneMinusSrcAlpha
		
        Pass
        {
			Tags { "LightMode" = "LightWeightForward" }

			Stencil{
				Ref[_StencilRef]
				Comp Equal
				Fail Keep
				Pass Replace
				ZFail Keep
			}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

            //#include "UnityCG.cginc"
			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/ShaderGraphFunctions.hlsl"
			//#include "UnityLightingCommon.cginc" // for _LightColor0

			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct v2f
            {
				//Necessary for every shader
				float4 vertex : SV_POSITION;

				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 0);

				float2 uv : TEXCOORD1;

				float3 worldPos : TEXCOORD2;
				float3 worldNrm : TEXCOORD3;
				float3 viewVec : TEXCOORD4;

				float3 worldTang : TEXCOORD5;
				float3 worldBiTang : TEXCOORD6;
				float3 worldViewDir : TEXCOORD7;

				float3 vertexLight : TEXCOORD8;

				//float3 worldViewDir : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
            };

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)


            v2f vert (appdata v)
            {

                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				o.uv = v.texcoord;

				o.worldPos = mul(UNITY_MATRIX_M,v.vertex).xyz;
				o.worldNrm = normalize(mul(v.normal, (float3x3)UNITY_MATRIX_I_M));
				o.worldTang = normalize(mul((float3x3)UNITY_MATRIX_M, v.tangent.xyz));
				o.worldBiTang = cross(o.worldNrm, o.worldTang.xyz) * v.tangent.w;
				o.worldViewDir = _WorldSpaceCameraPos.xyz - mul(GetObjectToWorldMatrix(), float4(v.vertex.xyz, 1.0)).xyz;


				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				float3 lwWNormal = TransformObjectToWorldNormal(v.normal);

				o.vertexLight = VertexLighting(vertexInput.positionWS, lwWNormal);

				o.vertex = vertexInput.positionCS;
				
				//half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {

				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				InputData inputData;
				inputData.positionWS = i.worldPos;
				inputData.normalWS = normalize(i.worldNrm);
				inputData.viewDirectionWS = normalize(i.worldViewDir);
				inputData.shadowCoord = 0;
				inputData.fogCoord = 0;
				inputData.vertexLighting = i.vertexLight;
				inputData.bakedGI = SAMPLE_GI(i.lightmapUV, i.vertexSH, inputData.normalWS);


				float3 Albedo = _Color;
				float3 Specular = float3(0, 0, 0);
				float Metallic = 0;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = 0;
				float Smoothness = 0.5;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0;

				half4 color = LightweightFragmentPBR(
					inputData,
					Albedo,
					Metallic,
					Specular,
					Smoothness,
					Occlusion,
					Emission,
					Alpha);

				color = half4(1, 0.5, 0.5, 1);
				return color;
            }
            ENDHLSL
        }
    }
	FallBack "Hidden/InternalErrorShader"
}
