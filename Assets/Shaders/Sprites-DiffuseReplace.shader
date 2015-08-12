Shader "Sprites/DiffuseReplace"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("ReplaceWith", Color) = (1,1,1,1)
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

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
		};

		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif

			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			/**fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			if (c[0] == 1 && c[1] == 1 && c[2] == 1) {
				o.Albedo = c.rgb * _Color * c.a;
			} else {
				o.Albedo = c.rgb * c.a;
			}**/

			float3 to_replace = float3(1.0, 1.0, 1.0);
			float4 result = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			float dist = distance(result.rgb, to_replace);
			float factor = saturate(1.0 - dist / 0.01);
			result.rgb = lerp(result.rgb, _Color, factor);
			o.Albedo = result.rgb * result.a;

			o.Alpha = result.a;
		}
		ENDCG
	}

Fallback "Transparent/VertexLit"
}
