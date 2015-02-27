Shader "Unlit/VertexColor" {

Category {
	
	Lighting Off
	ZWrite Off
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	SubShader {
		ZTest Always
		Tags { "Queue"="Overlay"}
		Pass {
			SetTexture [_MainTex] {
				combine primary
			}
		}
	}
}
}