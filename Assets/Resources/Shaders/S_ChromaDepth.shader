// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/S_ChromaDepth"
{
	Properties
	{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 10

		_CloseColor("Close Color", Color) = (0.5, 0.5, 0.5, 1)
		_FarColor("Far Color", Color) = (0.5, 0.5, 0.5, 1)
		_FocusPosition("FocusPosition", Vector) = (-0.2,0.1,0.2,0)
		_FocusNormal("Focus Normal", Vector) = (0,1,0,0)

		_DepthDistance("Distance for color lerp", Float) = 0.1
		_BaseColor("Base Color", Color) = (0.5, 0.5, 0.5, 1)
	}

		SubShader
		{
			Pass
			{
				Tags {"LightMode" = "ForwardBase"}

				Stencil{
						Ref[_StencilRef]
						Comp Equal
						Fail Keep
						Pass Replace
						ZFail Keep
					}

				CGPROGRAM

			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0;
				float4 vertex : SV_POSITION;

				float distance : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _FocusPosition)
				UNITY_DEFINE_INSTANCED_PROP(float4, _FocusNormal)
				UNITY_DEFINE_INSTANCED_PROP(half4, _CloseColor)
				UNITY_DEFINE_INSTANCED_PROP(half4, _FarColor)
				UNITY_DEFINE_INSTANCED_PROP(float, _DepthDistance)
			UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert(appdata_base v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.diff = nl * _LightColor0;

				o.diff.rgb += ShadeSH9(half4(worldNormal,1));

				float3 direction = normalize((_FocusNormal * -1).xyz);
				float3 vecToFocus = mul(unity_ObjectToWorld, v.vertex).xyz - _FocusPosition.xyz;
				float3 component = dot(vecToFocus, direction) * direction;
				o.distance = length(component);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float shading = i.diff.r;

				float4 color = float4(lerp(_CloseColor, _FarColor, saturate(i.distance / _DepthDistance)).xyz,1);
				color *= shading;

				return UNITY_ACCESS_INSTANCED_PROP(Props, color);
			}
			ENDCG
		}
		}
}
