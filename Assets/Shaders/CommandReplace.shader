// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Sprites/Command" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Center ("ObjectCenter", Vector) = (1, 1, 0, 0)
	_Color ("ReplaceWith", Color) = (1, 0, 0, 1)
	_LightColor ("PinkReplaceWith", Color) = (1, 0, 0, 1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100

	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct v2f {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Center;
			fixed4 _Color;
			fixed4 _LightColor;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord1 = o.vertex;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				float2 distance = i.texcoord1.xy;
				float magnitude = sqrt(dot(distance.xy, distance.xy));
				float theta = atan2(distance.x, distance.y);
				float thetaSelector = (_Time * 128) % (3.14159 * 2);
				float actualSelector = lerp(-3.14159, 3.14159, thetaSelector / (3.14159 * 2));
				float thetaDifference;
				if (theta < 0) {
					if (thetaSelector < 0) {
						thetaDifference = abs(theta - actualSelector);
					} else {
						thetaDifference = abs(theta + actualSelector);
					}
				} else {
					if (thetaSelector < 0) {
						thetaDifference = abs(theta - actualSelector);
					} else {
						thetaDifference = abs(theta + actualSelector);
					}
				}
				if (col.r == 1 && col.g == 1 && col.b == 1) {
					if (thetaDifference <= 128) {
						col = _Color;
					} else {
						col = float4(0, 0, 0, 1);
					}
				}
				float lightBrightness = cos(_Time * 256);
				if (col.r == 1 && col.g == 0.4117647058823529 && col.b == 0.7058823529411765) {
					if (lightBrightness >= 0.8) {
						col = _LightColor;
					} else {
						col = float4(0, 0, 0, 1);
					}
				}
				return col;
			}
		ENDCG
	}
}

}
