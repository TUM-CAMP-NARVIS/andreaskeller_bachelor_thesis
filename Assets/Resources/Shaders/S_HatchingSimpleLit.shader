//Hatching shader based on LWRP Simple Lit
// Shader targeted for low end devices. Single Pass Forward Rendering.
Shader "Custom/S_HatchingSimpleLit"
{
	// Keep properties of StandardSpecular shader for upgrade reasons.
	Properties
	{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 10
		[ToggleOff] _UseTriPlanar("Use Triplanar Mapping", Range(0,1)) = 0
		[ToggleOff] _InvertHatching("Invert the Hatching (color bright parts with strokes)", Float) = 0.0

		_Hatching1("Hatching Texture Small", 2D) = "white" {}
		_Hatching2("Hatching Texture Big", 2D) = "white" {}

		_UVScale("UV Scale", Float) = 6.0

		_BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 1)
		_BaseMap("Base Map (RGB) Smoothness / Alpha (A)", 2D) = "white" {}

		_Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.5

		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_SpecGlossMap("Specular Map", 2D) = "white" {}
		
		[Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessSource("Smoothness Source", Float) = 0.0
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0

		[HideInInspector] _BumpScale("Scale", Float) = 1.0
		[NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Emission Color", Color) = (0,0,0)
		[NoScaleOffset]_EmissionMap("Emission Map", 2D) = "white" {}

		// Blending state
		[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0

		[ToogleOff] _ReceiveShadows("Receive Shadows", Float) = 1.0

			// Editmode props
			[HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
			[HideInInspector] _Smoothness("SMoothness", Float) = 0.5

			// ObsoleteProperties
			[HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
			[HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
			[HideInInspector] _Shininess("Smoothness", Float) = 0.0
			[HideInInspector] _GlossinessSource("GlossinessSource", Float) = 0.0
			[HideInInspector] _SpecSource("SpecularHighlights", Float) = 0.0
	}

	SubShader
		{
			Tags { "RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True"}
			
			LOD 300

			Pass
			{
				Name "ForwardLit"
				Tags { "LightMode" = "LightweightForward" }
				/*
			Stencil{
				Ref[_StencilRef]
				Comp Equal
				Fail Keep
				Pass Replace
				ZFail Keep
			}*/
				// Use same blending / depth states as Standard shader
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
			Cull[_Cull]

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			// -------------------------------------
			// Material Keywords
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _ _SPECGLOSSMAP _SPECULAR_COLOR
			#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _EMISSION
			#pragma shader_feature _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Lightweight Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex vert
			#pragma fragment frag
			#define BUMP_SCALE_NOT_SUPPORTED 1

			#include "Assets/Resources/Shaders/Custom_SimpleLitInput.hlsl"
			#include "Assets/Resources/Shaders/Custom_SimpleLitForwardPass.hlsl"


			

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _Hatching1)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _Hatching2)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Hatching1_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Hatching2_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _UVScale)
				UNITY_DEFINE_INSTANCED_PROP(float, _InvertHatching)
				UNITY_DEFINE_INSTANCED_PROP(int, _UseTriPlanar)
			UNITY_INSTANCING_BUFFER_END(Props)


			Varyings vert(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
				VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
				half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
				half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
				half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

				output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
				output.posWS.xyz = vertexInput.positionWS;
				output.positionCS = vertexInput.positionCS;

#ifdef _NORMALMAP
				output.normal = half4(normalInput.normalWS, viewDirWS.x);
				output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
				output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);
#else
				output.normal = NormalizeNormalPerVertex(normalInput.normalWS);
				output.viewDir = viewDirWS;
#endif

				output.uv1 = _UVScale * TRANSFORM_TEX(input.texcoord, _Hatching1);
				output.uv2 = _UVScale * TRANSFORM_TEX(input.texcoord, _Hatching2);

				OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
				OUTPUT_SH(output.normal.xyz, output.vertexSH);

				output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

#if defined(_MAIN_LIGHT_SHADOWS) && !defined(_RECEIVE_SHADOWS_OFF)
				output.shadowCoord = GetShadowCoord(vertexInput);
#endif

				return output;
			}

			half4 frag(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float2 uv = input.uv;
				half4 diffuseAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
				half3 diffuse = diffuseAlpha.rgb * _BaseColor.rgb;

				half alpha = diffuseAlpha.a * _BaseColor.a;
					AlphaDiscard(alpha, _Cutoff);

			#ifdef _ALPHAPREMULTIPLY_ON
				diffuse *= alpha;
			#endif

				half3 normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap));
				half3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
				half4 specular = SampleSpecularSmoothness(uv, alpha, _SpecColor, TEXTURE2D_ARGS(_SpecGlossMap, sampler_SpecGlossMap));
				half smoothness = specular.a;

				InputData inputData;
				InitializeInputData(input, normalTS, inputData);

				half3 white = half3(1, 1, 1);

				half4 lwCol = LightweightFragmentBlinnPhong(inputData, white, specular, smoothness, emission, alpha);

				float4 hatch1 = tex2D(_Hatching1, input.uv1);
				float4 hatch2 = tex2D(_Hatching2, input.uv2);

				/*
				if (_UseTriPlanar > 0) {
					half3 blend = abs(i.objNormal);
					blend /= dot(blend, 1.0);
					fixed4 cx = tex2D(_Hatching1, i.coords.yz);
					fixed4 cy = tex2D(_Hatching1, i.coords.xz);
					fixed4 cz = tex2D(_Hatching1, i.coords.xy);

					fixed4 bcx = tex2D(_Hatching2, i.coords.yz);
					fixed4 bcy = tex2D(_Hatching2, i.coords.xz);
					fixed4 bcz = tex2D(_Hatching2, i.coords.xy);

					hatch1 = cx * blend.x + cy * blend.y + cz * blend.z;
					hatch2 = bcx * blend.x + bcy * blend.y + bcz * blend.z;

				}
				*/

				float shading = lwCol.r;

				float4 col = float4(0, 0, 0, 0);

				if (_InvertHatching > 0) {
					float step = 1.0f / 6.0f;
					if (shading <= step) {
						col = lerp(hatch1.r, hatch1.g, 6.0f * shading);
					}
					if (shading > step&& shading <= 2.0f * step) {
						col = lerp(hatch1.g, hatch1.b, 6.0f * (shading - step));
					}
					if (shading > 2.0f * step && shading <= 3.0f * step) {
						col = lerp(hatch1.b, hatch2.r, 6.0f * (shading - 2.0f * step));
					}
					if (shading > 3.0f * step && shading <= 4.0f * step) {
						col = lerp(hatch2.r, hatch2.g, 6.0f * (shading - 3.0f * step));
					}
					if (shading > 4.0f * step && shading <= 5.0f * step) {
						col = lerp(hatch2.g, hatch2.b, 6.0f * (shading - 4.0f * step));
					}
					if (shading > 5.0f * step) {
						col = lerp(hatch2.b, float4(0, 0, 0, 1), 6.0f * (shading - 5.0f * step));
					}
				}
				else
				{
					float step = 1.0f / 6.0f;
					if (shading <= step) {
						col = lerp(hatch2.b, hatch2.g, 6.0f * shading);
					}
					if (shading > step&& shading <= 2.0f * step) {
						col = lerp(hatch2.g, hatch2.r, 6.0f * (shading - step));
					}
					if (shading > 2.0f * step && shading <= 3.0f * step) {
						col = lerp(hatch2.r, hatch1.b, 6.0f * (shading - 2.0f * step));
					}
					if (shading > 3.0f * step && shading <= 4.0f * step) {
						col = lerp(hatch1.b, hatch1.g, 6.0f * (shading - 3.0f * step));
					}
					if (shading > 4.0f * step && shading <= 5.0f * step) {
						col = lerp(hatch1.g, hatch1.r, 6.0f * (shading - 4.0f * step));
					}
					if (shading > 5.0f * step) {
						col = lerp(hatch1.r, float4(1, 1, 1, 1), 6.0f * (shading - 5.0f * step));
					}
				}
				
				float4 color = float4(1 - col.x, 1 - col.x, 1 - col.x, 1 - col.x);

				color *= float4(diffuse.r,diffuse.g,diffuse.b,1);
				//fixed4 color = col;
				return UNITY_ACCESS_INSTANCED_PROP(Props, color);


				color.rgb = MixFog(color.rgb, inputData.fogCoord);
				return color;
				}

				ENDHLSL
			}

			Pass
			{
				Name "ShadowCaster"
				Tags{"LightMode" = "ShadowCaster"}

				ZWrite On
				ZTest LEqual
				Cull[_Cull]

				HLSLPROGRAM
				// Required to compile gles 2.0 with standard srp library
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0

				// -------------------------------------
				// Material Keywords
				#pragma shader_feature _ALPHATEST_ON
				#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

				//--------------------------------------
				// GPU Instancing
				#pragma multi_compile_instancing

				#pragma vertex ShadowPassVertex
				#pragma fragment ShadowPassFragment

				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/ShadowCasterPass.hlsl"
				ENDHLSL
			}

			Pass
			{
				Name "DepthOnly"
				Tags{"LightMode" = "DepthOnly"}

				ZWrite On
				ColorMask 0
				Cull[_Cull]

				HLSLPROGRAM
				// Required to compile gles 2.0 with standard srp library
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 2.0

				#pragma vertex DepthOnlyVertex
				#pragma fragment DepthOnlyFragment

				// -------------------------------------
				// Material Keywords
				#pragma shader_feature _ALPHATEST_ON
				#pragma shader_feature _GLOSSINESS_FROM_BASE_ALPHA

				//--------------------------------------
				// GPU Instancing
				#pragma multi_compile_instancing

				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/DepthOnlyPass.hlsl"
				ENDHLSL
			}

				// This pass it not used during regular rendering, only for lightmap baking.
				Pass
				{
					Name "Meta"
					Tags{ "LightMode" = "Meta" }

					Cull Off

					HLSLPROGRAM
				// Required to compile gles 2.0 with standard srp library
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x

				#pragma vertex LightweightVertexMeta
				#pragma fragment LightweightFragmentMetaSimple

				#pragma shader_feature _EMISSION
				#pragma shader_feature _SPECGLOSSMAP

				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitMetaPass.hlsl"

				ENDHLSL
			}
			Pass
			{
				Name "Lightweight2D"
				Tags{ "LightMode" = "Lightweight2D" }
				Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

				HLSLPROGRAM
				// Required to compile gles 2.0 with standard srp library
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x

				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature _ALPHATEST_ON
				#pragma shader_feature _ALPHAPREMULTIPLY_ON

				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/SimpleLitInput.hlsl"
				#include "Packages/com.unity.render-pipelines.lightweight/Shaders/Utils/Lightweight2D.hlsl"
				ENDHLSL
			}
			}
				Fallback "Hidden/InternalErrorShader"
				//CustomEditor "UnityEditor.Rendering.LWRP.ShaderGUI.SimpleLitShader"
}
