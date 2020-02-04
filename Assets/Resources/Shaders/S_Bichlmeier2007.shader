Shader "Unlit/S_Bichlmeier2007"
{
	Properties
	{
		[IntRange] _StencilRef("Stencil Reference Value", Range(0,255)) = 10
		_WeightCurvature("Curvature Weight", Float) = 1.0
		_WeightAngleofIncidence("Angle of Incidence Weight", Float) = 1.0
		_WeightDistanceFalloff("Distance Falloff Weight", Float) = 1.0
		_Alpha("alpha value scaling the curvature", Float) = 1.0
		_Beta("beta value scaling the angle of incidence", Float) = 1.0
		_Gamma("gamma value scaling the distance falloff", Float) = 1.0
		_FocusPosition("FocusPosition", Vector) = (-0.2,0.1,0.2,0)
		_FocusRadius("Focus Radius", Float) = 0.3
		_BorderSize("Thickness of colored border", Float) = 0.001
		_BorderColor("Color of border", Color) = (1,0,0,1)
		_CurvatureMap("Curvature Map", 2D) = "black"
		
		
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent"}
        LOD 100

		ZWrite On
		ZTest LEqual

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			Tags { "LightMode" = "ForwardBase" }

			
			Stencil{
				Ref [_StencilRef]
				Comp Always
				Fail Keep
				Pass Replace
				ZFail Keep
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"

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

				float2 screenPos : TEXCOORD4;

				//Setup for Single Pass Instancing
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float3, _FocusPosition)
				UNITY_DEFINE_INSTANCED_PROP(float, _FocusRadius)
				UNITY_DEFINE_INSTANCED_PROP(float, _WeightCurvature)
				UNITY_DEFINE_INSTANCED_PROP(float, _WeightAngleofIncidence)
				UNITY_DEFINE_INSTANCED_PROP(float, _WeightDistanceFalloff)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(float, _Beta)
				UNITY_DEFINE_INSTANCED_PROP(float, _Gamma)
				UNITY_DEFINE_INSTANCED_PROP(float, _BorderSize)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _BorderColor)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _CurvatureMap)
				UNITY_DEFINE_INSTANCED_PROP(sampler2D, _ZedImage)
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

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                // sample the curvature - for this map 0.5 means no curvature, 0 extreme inward curvature and 1 extreme outward curvature
				fixed4 curvMap = tex2D(_CurvatureMap, i.uv);
				//Translates the curvature map to a simple scalar 0=no curvature; 1=maximum curvature
				float curv =1 -pow(1 - saturate(abs((0.5 - curvMap.x) * 2)),_Alpha);

				//Angle of Incidence factor
				float angInc = saturate(1 - pow(dot(i.worldNrm, i.viewVec), _Beta));

				//Distance factor
				float dist = distance(_FocusPosition, i.worldPos);

				float distFalloff = pow(saturate(dist / _FocusRadius),_Gamma);

				

				

				//Base Color for the skin
				fixed4 col = fixed4(0, 0, 0, 1);

				//Final opacity calculation
				float opacity = saturate(max(max(_WeightCurvature*curv,_WeightDistanceFalloff*distFalloff),_WeightAngleofIncidence*angInc));

				//Calculate the border position
				float outsideFocus = step(_FocusRadius, dist);
				float borderValue = (outsideFocus - step(_BorderSize + _FocusRadius, dist));
				//If the fragment is the border then make it red 
#ifdef _ZEDMINI_BLENDING
				float3 zed = tex2D(_ZedImage, i.screenPos).xyz;
				col.xyz = borderValue * _BorderColor.xyz + (1 - borderValue) * zed;
#else
				col.xyz = borderValue * _BorderColor.xyz + (1 - borderValue) * float3(0, 0, 0);
#endif
				col.a = outsideFocus * 1 + (1 - outsideFocus) * opacity;
				
				

				return UNITY_ACCESS_INSTANCED_PROP(Props, col);
            }
            ENDCG
        }
    }
	FallBack "Hidden/InternalErrorShader"
}
