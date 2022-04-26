// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/UnlitTextureColorAdditive" {
Properties {
    _MainTex ("Base (RGBA)", 2D) = "white" {}
	_Color("Main Color", COLOR) = (1,1,1,1)
}

SubShader {
	Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
    LOD 100
    Blend SrcAlpha One

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2_f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
			float4 _Color;

            v2_f vert (appdata_t v)
            {
                v2_f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2_f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.texcoord) * _Color;
                return col;
            }
        ENDCG
    }
}

}
