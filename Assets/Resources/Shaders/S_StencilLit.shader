Shader "Custom/S_StencilLit"
{
	Properties
	{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 10
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
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 diff : COLOR0;
				float4 vertex : SV_POSITION;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
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
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				fixed4 col = _BaseColor;
				col *= i.diff;
				return UNITY_ACCESS_INSTANCED_PROP(Props, col);
			}
			ENDCG
		}
	}
}