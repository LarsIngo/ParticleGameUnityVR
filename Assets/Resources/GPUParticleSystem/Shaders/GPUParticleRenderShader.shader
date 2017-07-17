Shader "Custom/BufferShader"
{
	SubShader
	{
		Pass
		{

		ZTest Less
		Cull Back
		ZWrite Off
		BlendOp Add
		//Blend SrcAlpha One
		Blend SrcAlpha OneMinusSrcAlpha

		Fog	{ Mode off }

		CGPROGRAM

		#include "UnityCG.cginc"
		#pragma target 5.0
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag

		StructuredBuffer<float4> gPosition;
		StructuredBuffer<float4> gVelocity;
		StructuredBuffer<float4> gScale;
		StructuredBuffer<float4> gColor;
		StructuredBuffer<float4> gHaloColor;
		StructuredBuffer<float4> gLifetime;

		// ---

		struct vsOutput
		{
			float4 svPosition : SV_POSITION;
			float3 velocity : VELOCITY;
			float2 scale : SCALE;
			float3 color : COLOR;
			float3 haloColor : HALO;
			float2 lifetime : LIFETIME;
		};

		vsOutput vert(uint vID : SV_VertexID) 
		{
			vsOutput output;

			output.svPosition = float4(gPosition[vID].xyz, 1);
			output.velocity = gVelocity[vID].xyz;
			output.scale = gScale[vID].xy;
			output.color = gColor[vID].xyz;
			output.haloColor = gHaloColor[vID].xyz;
			output.lifetime = gLifetime[vID].xy;

			return output;
		}

		// ---

		struct gsOutput 
		{
			float4 svPosition : SV_POSITION;
			float3 worldPosition : WORLDPOSITION;
			float3 velocity : VELOCITY;
			float2 scale : SCALE;
			float3 color : COLOR;
			float3 haloColor : HALO;
			float2 lifetime : LIFETIME;
			float2 uv : UV;
		};

		[maxvertexcount(4)]
		void geom(point vsOutput input[1], inout TriangleStream<gsOutput> TriStream) 
		{
			float2 pLifetime = input[0].lifetime;

			if (pLifetime.x < 0.01f) return;

			float3 lensRight = UNITY_MATRIX_IT_MV[0].xyz;
			float3 lensUp = UNITY_MATRIX_IT_MV[1].xyz;
			float3 lensForward = UNITY_MATRIX_IT_MV[2].xyz;

			float3 pPosition = input[0].svPosition.xyz;
			float3 pVelocity = input[0].velocity;
			float2 pScale = input[0].scale;
			float3 pColor = input[0].color;
			float3 pHaloColor = input[0].haloColor;

			float3 pForward = normalize(_WorldSpaceCameraPos - pPosition);
			float3 pRight = cross(pForward, lensUp);
			float3 pUp = cross(pRight, pForward);

			gsOutput output;
			for (int vID = 0; vID < 4; ++vID)
			{
				float x = vID == 0 || vID == 1;
				float y = vID == 0 || vID == 2;

				float3 vPosition = pPosition + pRight * ((x * 2.f - 1.f) * pScale.x) + pUp * ((y * 2.f - 1.f) * pScale.y);

				output.svPosition = UnityObjectToClipPos(float4(vPosition, 1));
				output.worldPosition = vPosition;
				output.velocity = pVelocity;
				output.scale = pScale;
				output.color = pColor;
				output.haloColor = pHaloColor;
				output.lifetime = pLifetime;
				output.uv = float2(x, 1.0f - y);

				TriStream.Append(output);
			}

			TriStream.RestartStrip();
		}

		// ---

		float4 frag(gsOutput input) : SV_TARGET0
		{
			float x = input.uv.x - 0.5f;
			float y = input.uv.y - 0.5f;
			float r = sqrt(x * x + y * y);
			float factor = max(1.f - r * 2.f, 0.f); //[1,0]

			float cosFactor = -cos(3.14159265f / 2.f * (factor + 1.f));

			float lifeFactor = input.lifetime.x / input.lifetime.y;

			return float4(input.haloColor, 1);// float4(factor * input.color + (1 - factor) * input.haloColor, cosFactor);
		}
			//return float4(factor * input.color + (1 - factor) * input.haloColor, cosFactor);// +float4(input.haloColor, cosRevFactor);

		ENDCG
		}
	}
}
