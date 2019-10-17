Shader "Unlit/S_OnlyRenderFocus"
{
    Properties
    {
        _Color ("Color", Color) = (0.6,0.8235,1,1)
		_FocusPosition ("Focus Position", Vector) = (-0.2,0.1,-0.2,0)
		_FocusRadius ("Focus Radius", Float) = 0.3
		_Transparency ("Transparency", Range(0.0,1.0)) = 0.2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
        LOD 100

		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
				half3 worldNormal : TEXCOORD0;
                //float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Transparency;
			float3 _FocusPosition;
			float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
				o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;// UnityObjectToClipPos(vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				fixed4 c = _Color;
				//c.rgb = i.worldNormal*0.5 + 0.5;
				//c.a = 1 - distance(i.worldPos, _FocusPosition);
				return c;
            }
            ENDCG
        }
    }
}
