// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/UnlitColorAlpha" {
Properties {
	_Color("Main Color", COLOR) = (1,1,1,1)
}

SubShader {
	Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
    LOD 100
	Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2_f {
                float4 vertex : SV_POSITION;
            };

			float4 _Color;

            v2_f vert (appdata_t v)
            {
                v2_f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2_f i) : SV_Target
            {
                float4 col = _Color;
                return col;
            }
        ENDCG
    }
}

}
