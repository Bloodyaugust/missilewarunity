// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Sprite/Test" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color ("ReplaceWith", Color) = (1, 0, 0, 1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100

	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	Pass {
		CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members vpos)
#pragma exclude_renderers d3d11 xbox360
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
			fixed4 _Color;

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
				float4 distance = i.texcoord1.xyxy;
				float magnitude = sqrt(dot(distance.xy, distance.xy));
				float theta = atan2(distance.y, distance.x);
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
				if (col.r == 1 && col.g == 1 && col.b == 1 && thetaDifference <= 0.5) {
					col = _Color;
				}
				return col;
			}
		ENDCG
	}
}

}
