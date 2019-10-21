Shader "Lit/S_OnlyRenderFocus"
{
    Properties
    {
        _Color ("Color", Color) = (0.6,0.8235,1,1)
		_FocusPosition ("FocusPosition", Vector) = (-0.2,0.1,-0.2,0)
		_FocusRadius ("FocusRadius", Float) = 0.3
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
			Tags { "LightMode" = "LightWeightForward" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			//#include "UnityLightingCommon.cginc" // for _LightColor0


            struct v2f
            {
				//Necessary for every shader
				float4 vertex : SV_POSITION;

                float2 uv : TEXCOORD0;

				float3 worldPos : TEXCOORD1;
            };

           // sampler2D _MainTex;
			float3 _FocusPosition;
			float _FocusRadius;
			float4 _Color;


            v2f vert (appdata_base v)
            {

                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;

				o.worldPos = mul(UNITY_MATRIX_M,v.vertex).xyz;
				
				//half3 worldNormal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				fixed4 col = _Color;

				//col *= i.diff;	
				float dist = distance(_FocusPosition, i.worldPos);
				dist = step(_FocusRadius, dist);
				col.a = 1-dist;
				//c.a = 1 - distance(i.worldPos, _FocusPosition);
				return col;
            }
            ENDCG
        }
    }
	FallBack "Hidden/InternalErrorShader"
}
