Shader "Unlit/S_Bichlmeier2007"
{
	Properties
	{
		_WeightCurvature("Curvature Weight", Float) = 1.0
		_WeightAngleofIncidence("Angle of Incidence Weight", Float) = 1.0
		_WeightDistanceFalloff("Distance Falloff Weight", Float) = 1.0
		_FocusPosition("FocusPosition", Vector) = (-0.2,0.1,0.2,0)
		_FocusRadius("Focus Radius", Float) = 0.3
		_BorderSize("Thickness of colored border", Float) = 0.02
		_BorderColor("Color of border", Color) = (1,0,0,1)
		_CurvatureMap("Curvature Map", 2D) = "black"
		
    }
		SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
		LOD 100

		ZWrite On
		ZTest LEqual

		//Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"
		//#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"

		struct appdata
		{
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

			float2 uv : TEXCOORD0;

			float3 worldPos : TEXCOORD1;
			float3 worldNrm : TEXCOORD2;
			float3 viewVec : TEXCOORD3;
			float4 screenPos : TEXCOORD4;
			float eyeDepth : TEXCOORD5;
			float worldDirection : TEXCOORD6;
			//float3 worldViewDir : TEXCOORD5;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
		};

		UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float3, _FocusPosition)
			UNITY_DEFINE_INSTANCED_PROP(float, _FocusRadius)
			UNITY_DEFINE_INSTANCED_PROP(float, _WeightCurvature)
			UNITY_DEFINE_INSTANCED_PROP(float, _WeightAngleofIncidence)
			UNITY_DEFINE_INSTANCED_PROP(float, _WeightDistanceFalloff)
			UNITY_DEFINE_INSTANCED_PROP(float, _BorderSize)
			UNITY_DEFINE_INSTANCED_PROP(fixed4, _BorderColor)
			UNITY_DEFINE_INSTANCED_PROP(matrix, clipToWorld)
			UNITY_DEFINE_INSTANCED_PROP(sampler2D, _CurvatureMap)
			UNITY_DEFINE_INSTANCED_PROP(sampler2D_float, _CameraDepthTexture)
		UNITY_INSTANCING_BUFFER_END(Props)


		v2f vert(appdata v)
		{

			v2f o;

			UNITY_SETUP_INSTANCE_ID(v);
			UNITY_INITIALIZE_OUTPUT(v2f, o)
			UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

			UNITY_TRANSFER_INSTANCE_ID(v, o);

			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;

			o.worldPos = mul(UNITY_MATRIX_M,v.vertex).xyz;

			o.worldNrm = UnityObjectToWorldNormal(v.normal);

			o.viewVec = normalize(_WorldSpaceCameraPos.xyz - o.worldPos);

			o.screenPos = ComputeScreenPos(o.vertex);

			float4 clip = float4(o.vertex.xy, 0.0, 1.0);
			o.worldDirection = mul(clipToWorld, clip) - _WorldSpaceCameraPos;


			COMPUTE_EYEDEPTH(o.eyeDepth);

			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			UNITY_SETUP_INSTANCE_ID(i);
			UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
			// sample the curvature
			fixed4 curvMap = tex2D(_CurvatureMap, i.uv);

			float dist = distance(_FocusPosition, i.worldPos);
			float distFalloff = saturate(dist / _FocusRadius);
			float angInc = saturate(1 - dot(i.worldNrm, i.viewVec));
			float curv = saturate(abs((0.5 - curvMap.x) * 2));
			fixed4 col = fixed4(0, 0, 0, 1);

			float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy);
			depth = LinearEyeDepth(depth);
			float3 worldSpace = i.worldDirection * depth + _WorldSpaceCameraPos;

			//dist = step(_WeightDistanceFalloff, dist);
			/*
			float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
			float sceneZ = LinearEyeDepth(rawZ);
			float partZ = i.eyeDepth;
			*/

			col.a = saturate(max(max(_WeightCurvature * curv,_WeightDistanceFalloff * dist),_WeightAngleofIncidence * angInc));

			float borderValue = (step(_FocusRadius, dist) - step(_BorderSize + _FocusRadius, dist));
			//Colored Border
			col.xyz = borderValue * _BorderColor.xyz + (1 - borderValue) * float3(1, 1, 1);

			if (dist > _FocusRadius + _BorderSize) {
				col.a = 1;

			}
			else {
				col.a = 1;
				col.xyz = worldSpace;//(sceneZ - partZ) * _WeightCurvature;
			}


			return UNITY_ACCESS_INSTANCED_PROP(Props, col);
		}
		ENDCG
	}
	}

	/*
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent"}
        LOD 100

		ZWrite On
		ZTest LEqual

		//Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			Tags { "LightMode" = "LightWeightForward" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"
			//#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"

			struct appdata
			{
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

                float2 uv : TEXCOORD0;

				float3 worldPos : TEXCOORD1;
				float3 worldNrm : TEXCOORD2;
				float3 viewVec : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
				float eyeDepth : TEXCOORD5;
				//float3 worldViewDir : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float3, _FocusPosition)
				UNITY_DEFINE_INSTANCED_PROP(float, _FocusRadius)
				UNITY_DEFINE_INSTANCED_PROP(float, _WeightCurvature)
				UNITY_DEFINE_INSTANCED_PROP(float, _WeightAngleofIncidence)
				UNITY_DEFINE_INSTANCED_PROP(float, _WeightDistanceFalloff)
				UNITY_DEFINE_INSTANCED_PROP(float, _BorderSize)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _BorderColor)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _CurvatureMap)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D_float, _CameraDepthTexture)
			UNITY_INSTANCING_BUFFER_END(Props)
			

            v2f vert (appdata v)
            {

                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o)
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				o.worldPos = mul(UNITY_MATRIX_M,v.vertex).xyz;
				
				o.worldNrm = UnityObjectToWorldNormal(v.normal);

				o.viewVec = normalize(_WorldSpaceCameraPos.xyz - o.worldPos);

				o.screenPos = ComputeScreenPos(o.vertex);

				COMPUTE_EYEDEPTH(o.eyeDepth);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                // sample the curvature
				fixed4 curvMap = tex2D(_CurvatureMap, i.uv);

				float dist = distance(_FocusPosition, i.worldPos);
				float distFalloff = saturate(dist / _FocusRadius);
				float angInc = saturate(1-dot(i.worldNrm, i.viewVec));
				float curv = saturate(abs((0.5 - curvMap.x) * 2));
				fixed4 col = fixed4(0, 0, 0, 1);
				//dist = step(_WeightDistanceFalloff, dist);
				
				float rawZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				float sceneZ = LinearEyeDepth(rawZ);
				float partZ = i.eyeDepth;

				col.a = saturate(max(max(_WeightCurvature*curv,_WeightDistanceFalloff*dist),_WeightAngleofIncidence*angInc));

				float borderValue = (step(_FocusRadius, dist) - step(_BorderSize + _FocusRadius, dist));
				//Colored Border
				col.xyz = borderValue * _BorderColor.xyz + (1 - borderValue) * float3(1, 1, 1);

				if (dist > _FocusRadius+_BorderSize) {
					col.a = 1;
					
				}
				else {
					col.a = 1;
					col.xyz = (sceneZ-partZ) * _WeightCurvature;
				}
				

				return UNITY_ACCESS_INSTANCED_PROP(Props, col);
            }
            ENDCG
        }
		
    }*/
	FallBack "Standard"
}
