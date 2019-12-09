Shader "Hidden/Custom/RadarPing"
{
	Properties
	{
		_Position("Position", Float) = 0.3
		_PulseWidth("Pulse Width", Float) = 0.1
		_Color("Pulse Color", Color) = (0, 1, 0.95, 1)
	}

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			//#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
			#include "UnityCG.cginc"


			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _Position)
				UNITY_DEFINE_INSTANCED_PROP(float, _PulseWidth)
			UNITY_INSTANCING_BUFFER_END(Props)


			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	UNITY_DECLARE_TEX2DARRAY(_MainTex);

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
				

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float depth = tex2D(_CameraDepthTexture, i.uv);
				depth = pow(Linear01Depth(depth), 0.1);
				float4 red = float4(0, 1, 0.95, 1);
				float strength = saturate(_PulseWidth - abs(depth - _Position)) / _PulseWidth;

				return (1 - strength) * color + (strength)*red;
			}

			ENDHLSL
		}
	}
}