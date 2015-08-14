Shader "Sprites/DiffuseReplace"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("ReplaceWith", Color) = (1,1,1,1)
		_Center ("Center", Vector) = (1,1,0,0)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog keepalpha
		#pragma multi_compile _ PIXELSNAP_ON

		sampler2D _MainTex;
		fixed4 _Color;
		float4 _Center;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};

		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON)
			//v.vertex = UnityPixelSnap(v.vertex);
			#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;

			float4 distance = _Center + v.vertex;
			float magnitude = sqrt(dot(distance.xy, distance.xy));
			float theta = atan2(distance.y, distance.x);
			float thetaSelector = sin(_Time * 128) * (3.14159265359 * 2);
			float xSelector = 1;//((cos(_Time * 128) + 1) / 2) * 64;
			/**if (sqrt(theta * theta - thetaSelector * thetaSelector) <= 1) {
				o.color.a = 1;
			} else {
				o.color.a = 0;
			}**/
			if (v.vertex.x >= 0) {
				o.color = _Color;
				o.color.a = 1;
			} else {
				o.color.a = 0;
			}
			//o.color.a = lerp(0, 1, theta / 3.14159265359);
			//o.color.a = lerp(0, 1, xSelector / 64);
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			/**fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			if (c[0] == 1 && c[1] == 1 && c[2] == 1 && c[3] > 0 && IN.color.a > 0) {
				//o.Albedo = c.rgb * _Color * c.a;
				o.Albedo = IN.color.rgb;
				o.Alpha = IN.color.a;
			} else {
				o.Albedo = c.rgb * c.a;
				o.Alpha = c.a;
			}**/

			/**float3 to_replace = float3(1.0, 1.0, 1.0);
			float4 result = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			float dist = distance(result.rgb, to_replace);
			float factor = saturate(1.0 - dist / 0.01);
			result.rgb = lerp(result.rgb, _Color, factor);
			o.Albedo = result.rgb * result.a;

			o.Alpha = result.a;**/

			float threshold = 2.985; // Threshold of 0.995 per channel (just under 255/256); added three times, once for each color channel
      float4 result = tex2D(_MainTex, IN.uv_MainTex);
      float luminance = dot(result.rgb, float3(1,1,1)); // Add up the color channels in the texture; dot products are fast
      float factor = step(threshold, luminance); // Step instead of distance is a bit faster; selects all white pixels
      result.rgb = lerp(result.rgb, _Color, factor);
      o.Albedo = result.rgb * IN.color * result.a;  // Multiply the vertex color *after* the replacement is done
			if (factor == 1 && result.rgb.r > 0) {
	      o.Alpha = IN.color.a;
			} else {
				o.Alpha = result.a;
			}
		}
		ENDCG
	}

Fallback "Transparent/VertexLit"
}
