Shader "Skybox/Starfield Procedural" {

  SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      struct v2f {
        float4  pos :    SV_POSITION;
        half3  uv  :    TEXCOORD0;
      };

      half random3(half3 n){
        return frac(sin(dot(n.xyz, half3(12.9898,78.233,1)))*43758.5453);
      }

      float frandom3(float3 n){
        return frac(sin(dot(n.xyz, half3(12.9898,78.233,1)))*43758.5453);
      }

      v2f vert(appdata_base v)
      {
        v2f o;
        float scale = 256;
        half rnd = random3(round(_Time));
        float4 rndFloat = clamp(rnd.x, -1, 1);
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        o.pos.x = o.pos.x + _WorldSpaceCameraPos.x * -1 + (rndFloat * 6 * sin(_Time * (16 + o.pos.x)));
        o.pos.y = o.pos.y + _WorldSpaceCameraPos.y * -1 + (rndFloat * 6 * sin(_Time * (16 + o.pos.y)));
        o.uv = v.texcoord.xyz * scale;

        return o;
      }

      half4 frag(v2f i) : COLOR
      {
        half rnd = random3(round(i.uv));

        //half2 col = saturate(half2(rnd, rnd));
        half stars = clamp(rnd - 0.19, 0, 255);

        if (stars.x >= 0.8) {
          return half4(stars.xxx, 1);
        } else {
          return half4(0, 0, 0, 1);
        }
      }
      ENDCG
    }
  }
}
