Shader "Mobile/MobileColorDiffuse" {
		
		Properties{
			_MainTex("Main Texture", 2D) = "white" {}
			_Color("Main Color", COLOR) = (1,1,1,1)
		}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 150

			CGPROGRAM
			#pragma surface surf Lambert noforwardadd

			sampler2D _MainTex;
			float4 _Color;
			struct Input {
				float placeHolder;
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) 
			{
				fixed4 pic = tex2D(_MainTex, IN.uv_MainTex);

				o.Albedo = _Color.rgb * pic;
				o.Alpha = _Color.a;
			}
			ENDCG
		}

			Fallback "Mobile/VertexLit"
}