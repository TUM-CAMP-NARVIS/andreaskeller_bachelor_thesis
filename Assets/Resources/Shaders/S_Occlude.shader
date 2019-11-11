Shader "Unlit/S_Occlude"
{
	Properties
	{
		_FocusPosition("FocusPosition", Vector) = (-0.2,0.1,0.2,0)
		_FocusRadius("Focus Radius", Float) = 0.3
		
    }
    SubShader
    {
        Tags { "Queue"="Geometry-1" "RenderType" = "Opaque"}
        LOD 100
		ColorMask 0
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

				float3 worldPos : TEXCOORD1;

				float depth : DEPTH;
				//float3 worldViewDir : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
            };

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float3, _FocusPosition)
				UNITY_DEFINE_INSTANCED_PROP(float, _FocusRadius)
			UNITY_INSTANCING_BUFFER_END(Props)
			

            v2f vert (appdata v)
            {

                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o)
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(UNITY_MATRIX_M,v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

				float dist = distance(_FocusPosition, i.worldPos);
				if (dist > _FocusRadius+0.001f) {
					i.depth = 0;
				}
				else
				{
					i.depth = 0.99f;
				}

				//Colored Border
				
				

				return 0;
            }
            ENDCG
        }
    }
	FallBack "Hidden/InternalErrorShader"
}
