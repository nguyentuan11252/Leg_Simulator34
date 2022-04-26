Shader "Mobile/MobileColorTransparent" {
        Properties
        {
            _Color("Color", Color) = (1,1,1,1)
            _MainTex("Albedo (RGB)", 2D) = "white" {}
        }

        SubShader
        {
            LOD 200
             Pass {
                 ColorMask 0
             }
            // Render normally

                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha
                ColorMask RGB

           CGPROGRAM

           #pragma surface surf Lambert noforwardadd alpha:fade
           #pragma target 3.0

           sampler2D _MainTex;
           fixed4 _Color;

           struct Input {
               float2 uv_MainTex;
           };


           void surf(Input IN, inout SurfaceOutput o)
           {
               fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
               o.Albedo = c.rgb;
               o.Alpha = c.a;
           }
           ENDCG
        }
            FallBack "Standard"
}