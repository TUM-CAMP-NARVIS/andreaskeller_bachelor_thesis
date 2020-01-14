Shader "Custom/S_HatchingOld"
{
    Properties
    {
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 10
        _Hatching1 ("Hatching Texture Small", 2D) = "white" {}
		_Hatching2 ("Hatching Texture Big", 2D) = "white" {}
		_UVScale("UV Scale", Float) = 6.0
		_AdditionalLightDir("Extra light source direction", Vector) = (0,0,0,0)
		_AdditionalLightPos("Extra light source position", Vector) = (0,0,0,0)
		[IntRange] _UseTriPlanar("Use Triplanar Mapping", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Geometry" "RenderType" = "Opaque"}
        LOD 100

		ZWrite On
		ZTest LEqual

		//Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
			/*
			Stencil{
				Ref[_StencilRef]
				Comp Equal
				Fail Keep
				Pass Replace
				ZFail Keep
			}*/

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal    : NORMAL;
                float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float dotVP : TEXCOORD1;
				float2 uv2 : TEXCOORD2;

				float3 coords : TEXCOORD3;
				float3 objNormal : TEXCOORD4;
				float2 triUV : TEXCOORD5;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _Hatching1)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _Hatching2)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Hatching1_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _Hatching2_ST)
				UNITY_DEFINE_INSTANCED_PROP(float4, _AdditionalLightPos)
				UNITY_DEFINE_INSTANCED_PROP(float4, _AdditionalLightDir)
				UNITY_DEFINE_INSTANCED_PROP(float, _UVScale)
				UNITY_DEFINE_INSTANCED_PROP(int, _UseTriPlanar)
			UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o)
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = _UVScale * TRANSFORM_TEX(v.uv, _Hatching1);
				o.uv2 = _UVScale * TRANSFORM_TEX(v.uv, _Hatching2);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);

				//direct lighting
				o.dotVP = max(0, dot(worldNormal, normalize(_WorldSpaceLightPos0.xyz)));

				//ambient lighting
				//o.dotVP += ShadeSH9(half4(worldNormal, 1));
				float intensity = min(_AdditionalLightDir.w,1-clamp(distance(_AdditionalLightPos, mul(unity_ObjectToWorld, v.vertex)), 0, 1));

				//Headlight - hardcoded, not fun
				o.dotVP += max(0, dot(worldNormal, normalize(_AdditionalLightDir.xyz))) * (intensity);

				o.coords = v.vertex.xyz * _UVScale;
				o.objNormal = v.normal;
				o.triUV = v.uv;


				UNITY_TRANSFER_INSTANCE_ID(v, o);
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);



				float diffuse = i.dotVP;
				float shading = diffuse;

				float4 hatch1 = tex2D(_Hatching1, i.uv);
				float4 hatch2 = tex2D(_Hatching2, i.uv2);

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

				float4 col = (0,0,0,0);

				float step = 1.0f / 6.0f;
				if (shading <= step) {
					col = lerp(hatch2.b, hatch2.g, 6.0f * shading);
				}
				if (shading > step && shading <= 2.0f*step) {
					col = lerp(hatch2.g, hatch2.r, 6.0f * (shading - step));
				}
				if (shading > 2.0f*step && shading <= 3.0f*step) {
					col = lerp(hatch2.r, hatch1.b, 6.0f * (shading - 2.0f * step));
				}
				if (shading > 3.0f * step && shading <= 4.0f * step) {
					col = lerp(hatch1.b, hatch1.g, 6.0f * (shading - 3.0f * step));
				}
				if (shading > 4.0f * step && shading <= 5.0f * step) {
					col = lerp(hatch1.g, hatch1.r, 6.0f * (shading - 4.0f * step));
				}
				if (shading > 5.0f * step) {
					col = lerp(hatch1.r, float4(1,1,1,1), 6.0f * (shading - 5.0f * step));
				}
				fixed4 color = fixed4(1-col.x,1-col.x,1-col.x, 1-col.x);
				//fixed4 color = col;
				return UNITY_ACCESS_INSTANCED_PROP(Props, color);
            }
            ENDCG
        }
    }
}
