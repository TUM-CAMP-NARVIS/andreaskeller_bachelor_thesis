Shader "Custom/S_Hatching"
{
	Properties
	{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 10
		_HatchingSmall("Hatching Texture Small", 2D) = "white" {}
		_HatchingBig("Hatching Texture Big", 2D) = "white" {}
		_UVScale("UV Scale", Float) = 6.0
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
			#pragma multi_compile _ _INVERTHATCHING
			#pragma multi_compile _ _TRIPLANAR

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
				float2 uvSmall : TEXCOORD1;
				float2 uvBig : TEXCOORD2;
				

#ifdef _TRIPLANAR
				float3 coords : TEXCOORD3;
				float3 objNormal : TEXCOORD4;
#endif

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _HatchingSmall)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _HatchingBig)
				UNITY_DEFINE_INSTANCED_PROP(float4, _HatchingSmall_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _HatchingBig_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _UVScale)
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

				o.uvSmall = _UVScale * TRANSFORM_TEX(v.texcoord, _HatchingSmall);
				o.uvBig = _UVScale * TRANSFORM_TEX(v.texcoord, _HatchingBig);

#ifdef _TRIPLANAR
				o.coords = v.vertex.xyz * _UVScale;
				o.objNormal = v.normal;
#endif
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float shading = i.diff.r;

#ifdef _TRIPLANAR
				half3 blend = abs(i.objNormal);
				blend /= dot(blend, 1.0);
				fixed4 cx = tex2D(_HatchingSmall, i.coords.yz);
				fixed4 cy = tex2D(_HatchingSmall, i.coords.xz);
				fixed4 cz = tex2D(_HatchingSmall, i.coords.xy);

				fixed4 bcx = tex2D(_HatchingBig, i.coords.yz);
				fixed4 bcy = tex2D(_HatchingBig, i.coords.xz);
				fixed4 bcz = tex2D(_HatchingBig, i.coords.xy);

				float4 hatchSmall = cx * blend.x + cy * blend.y + cz * blend.z;
				float4 hatchBig = bcx * blend.x + bcy * blend.y + bcz * blend.z;
#else
				float4 hatchSmall = tex2D(_HatchingSmall, i.uvSmall);
				float4 hatchBig = tex2D(_HatchingBig, i.uvBig);
#endif
				float step = 1.0f / 6.0f;
				float4 col = float4(0, 0, 0, 1);
#ifdef _INVERTHATCHING
				if (shading <= step) {
					col = lerp(hatchSmall.r, hatchSmall.g, 6.0f * shading);
				}
				if (shading > step&& shading <= 2.0f * step) {
					col = lerp(hatchSmall.g, hatchSmall.b, 6.0f * (shading - step));
				}
				if (shading > 2.0f * step && shading <= 3.0f * step) {
					col = lerp(hatchSmall.b, hatchBig.r, 6.0f * (shading - 2.0f * step));
				}
				if (shading > 3.0f * step && shading <= 4.0f * step) {
					col = lerp(hatchBig.r, hatchBig.g, 6.0f * (shading - 3.0f * step));
				}
				if (shading > 4.0f * step && shading <= 5.0f * step) {
					col = lerp(hatchBig.g, hatchBig.b, 6.0f * (shading - 4.0f * step));
				}
				if (shading > 5.0f * step) {
					col = lerp(hatchBig.b, float4(0, 0, 0, 1), 6.0f * (shading - 5.0f * step));
				}
#else
				if (shading <= step) {
					col = lerp(hatchBig.b, hatchBig.g, 6.0f * shading);
				}
				if (shading > step&& shading <= 2.0f * step) {
					col = lerp(hatchBig.g, hatchBig.r, 6.0f * (shading - step));
				}
				if (shading > 2.0f * step && shading <= 3.0f * step) {
					col = lerp(hatchBig.r, hatchSmall.b, 6.0f * (shading - 2.0f * step));
				}
				if (shading > 3.0f * step && shading <= 4.0f * step) {
					col = lerp(hatchSmall.b, hatchSmall.g, 6.0f * (shading - 3.0f * step));
				}
				if (shading > 4.0f * step && shading <= 5.0f * step) {
					col = lerp(hatchSmall.g, hatchSmall.r, 6.0f * (shading - 4.0f * step));
				}
				if (shading > 5.0f * step) {
					col = lerp(hatchSmall.r, float4(1, 1, 1, 1), 6.0f * (shading - 5.0f * step));
				}
#endif
				float4 color = float4(1 - col.x, 1 - col.x, 1 - col.x, 1 - col.x);
				color *= float4(_BaseColor.r, _BaseColor.g, _BaseColor.b, 1);
				return UNITY_ACCESS_INSTANCED_PROP(Props, color);
			}
			ENDCG
		}
	}
}