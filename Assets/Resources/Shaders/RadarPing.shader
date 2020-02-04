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
			#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"


			#pragma vertex VertDefault
			#pragma fragment frag


			float _Position;
			float _PulseWidth;

			TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);


			TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);


				

			float4 frag(VaryingsDefault i) : SV_Target
			{
				float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
				float depth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord).r;
				depth = Linear01Depth(depth);
				float4 red = float4(0, 1, 0.95, 1);
				float strength = saturate(_PulseWidth - abs(depth - _Position)) / _PulseWidth;
				return (1 - strength) * color + (strength)*red;
			}

			ENDHLSL
		}
	}
}