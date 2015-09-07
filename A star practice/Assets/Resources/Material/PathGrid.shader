Shader "Custom/PathGrid" {
	Properties {

	}
	SubShader {

		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			
			struct appdata_t
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;

				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 o = i.color;
				o.a = 0.5;
				return o;
			}

			ENDCG
		}

	} 
}
