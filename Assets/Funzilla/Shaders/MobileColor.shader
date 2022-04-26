Shader "Mobile/Color" {
	Properties{
		_Color("Main Color", COLOR) = (1,1,1,1)
	}

	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd

		UNITY_INSTANCING_BUFFER_START(Props)
		float4 _Color;
		UNITY_INSTANCING_BUFFER_END(Props)

		struct Input {
			float placeHolder;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			const fixed4 c = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Diffuse"
}