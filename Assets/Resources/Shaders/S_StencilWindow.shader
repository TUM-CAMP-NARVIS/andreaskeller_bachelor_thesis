﻿Shader "Unlit/S_StencilWindow"
{
    Properties
    {
		[IntRange] _StencilValue("Stencil Reference Value", Range(0,255)) = 12
    }
    SubShader
    {
        Tags { "Queue" = "Geometry-1" "RenderType"="Opaque" }
        LOD 100
		

		Stencil{
			Ref[_StencilValue]
			Comp Less
			Pass Replace
		}

        Pass
        {
			Cull Off

			Blend One One
			ZWrite Off
            
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {

                v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o)
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}
