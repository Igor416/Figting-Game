Shader "Toon/Toon with Cubemap - Swords" {
	Properties {
		_MainTex ("Base (RGB) Refl. strength (A)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_Cube ("Cube Reflection (Cube)", CUBE) = "" {}
		_CubeStr ("Cubemap Strength", Range (0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf ToonRamp

		sampler2D _Ramp;
		
		//Toon lighting function.
		inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
		{
			#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(lightDir);
			#endif
			
			half d = dot (s.Normal, lightDir) * 0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
			
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
			c.a = 0;
			return c;
		}

		sampler2D _MainTex;
		samplerCUBE _Cube;
		float _CubeStr;

		struct Input {
			float2 uv_MainTex;
			float3 worldRefl;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Emission = (texCUBE (_Cube, IN.worldRefl).rgb * c.a) * _CubeStr;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Specular"
}
