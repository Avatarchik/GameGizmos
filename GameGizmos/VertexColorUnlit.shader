Shader "Unlit/VertexColor" {

Category {
	Tags { "Queue"="Overlay" }
	Lighting Off
	ZWrite Off
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	}

	SubShader {
		Tags { "Queue"="Overlay" }
		Pass {
			SetTexture [_MainTex] {
				combine primary
				//return primary
			}
		}
	}
}
}
// source: http://wiki.unity3d.com/index.php/VertexColorUnlit