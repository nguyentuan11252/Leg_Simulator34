Shader "Mobile/MobileDiffultCullOff"
{
		Properties{
			_MainTex("Main Texture", 2D) = "white" {}
		}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 150
			Cull Off

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

				o.Albedo =  pic;
				o.Alpha = pic.a;
			}
			ENDCG
		}

			Fallback "Mobile/VertexLit"
}
