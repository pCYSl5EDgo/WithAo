Shader "Unlit/StructuredBuffer"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_instancing

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	StructuredBuffer<float3> _PositionBuffer;

	v2f vert(appdata v, uint instanceID : SV_InstanceID)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		float4x4 matrix_ = UNITY_MATRIX_I_V;
		matrix_._14_24_34 = _PositionBuffer[instanceID];
		o.vertex = UnityObjectToClipPos(mul(matrix_, v.vertex));
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return tex2D(_MainTex, i.uv);
	}
		ENDCG
	}
	}
}