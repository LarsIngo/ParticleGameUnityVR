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
			StructuredBuffer<float4> gAmbient;
			StructuredBuffer<float4> gLifetime;

			uniform int gColorLifetimeCount;
			StructuredBuffer<float4> gColorLifetimeBuffer;

			uniform int gHaloLifetimeCount;
			StructuredBuffer<float4> gHaloLifetimeBuffer;

			uniform int gScaleLifetimeCount;
			StructuredBuffer<float4> gScaleLifetimeBuffer;

			uniform int gTransparencyLifetimeCount;
			StructuredBuffer<float4> gTransparencyLifetimeBuffer;

			// ---

			struct vsOutput
			{
				float4 svPosition : SV_POSITION;
				float3 velocity : VELOCITY;
				float3 ambient : AMBIENT;
				float2 lifetime : LIFETIME;
			};

			vsOutput vert(uint vID : SV_VertexID) 
			{
				vsOutput output;

				output.svPosition = float4(gPosition[vID].xyz, 1);
				output.velocity = gVelocity[vID].xyz;
				output.ambient = gAmbient[vID].xyz;
				output.lifetime = gLifetime[vID].xy;

				return output;
			}

			// ---

			struct gsOutput 
			{
				float4 svPosition : SV_POSITION;
				float3 color : COLOR;
				float  transparency : TRANSPARENCY;
				float3 ambient : AMBIENT;
				float3 halo : HALO;
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
				float3 pAmbient = input[0].ambient;

				float3 pColor;
				float3 pHalo;
				float2 pScale;
				float  pTransparency;

				//update lifetime dependent variables
				float currentLifetimeFactor = 1 - pLifetime.x / pLifetime.y;//since lifetime starts from zero to one instead of one to zero
				


				for (int i = 1; i < gColorLifetimeCount; ++i)
				{
					if (gColorLifetimeBuffer[i].w > currentLifetimeFactor)
					{
						float4 c0 = gColorLifetimeBuffer[i - 1];
						float4 c1 = gColorLifetimeBuffer[i];

						float lerpFactor = (currentLifetimeFactor - c0.w) / (c1.w - c0.w);

						pColor = c0.xyz * (1 - lerpFactor) + c1.xyz * lerpFactor;

						//exit
						break;
					}
				}

				for (int i = 1; i < gHaloLifetimeCount; ++i)
				{
					if (gHaloLifetimeBuffer[i].w > currentLifetimeFactor)
					{
						float4 c0 = gHaloLifetimeBuffer[i - 1];
						float4 c1 = gHaloLifetimeBuffer[i];

						float lerpFactor = (currentLifetimeFactor - c0.w) / (c1.w - c0.w);

						pHalo = c0.xyz * (1 - lerpFactor) + c1.xyz * lerpFactor;

						//exit
						break;
					}
				}

				for (int i = 1; i < gScaleLifetimeCount; ++i)
				{
					if (gScaleLifetimeBuffer[i].w > currentLifetimeFactor)
					{
						float4 c0 = gScaleLifetimeBuffer[i - 1];
						float4 c1 = gScaleLifetimeBuffer[i];

						float lerpFactor = (currentLifetimeFactor - c0.w) / (c1.w - c0.w);

						pScale = c0.xy * (1 - lerpFactor) + c1.xy * lerpFactor;

						//exit
						break;
					}
				}

				for (int i = 1; i < gTransparencyLifetimeCount; ++i)
				{
					if (gTransparencyLifetimeBuffer[i].w > currentLifetimeFactor)
					{
						float4 c0 = gTransparencyLifetimeBuffer[i - 1];
						float4 c1 = gTransparencyLifetimeBuffer[i];

						float lerpFactor = (currentLifetimeFactor - c0.w) / (c1.w - c0.w);

						pTransparency = c0.x * (1 - lerpFactor) + c1.x * lerpFactor;

						//exit
						break;
					}
				}
				

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
					output.color = pColor;
					output.transparency = pTransparency;
					output.ambient = pAmbient;
					output.halo = pHalo;
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


				return float4(factor * input.color * input.ambient + (1 - factor) * input.halo, cosFactor) * float4(1, 1, 1, input.transparency);
			}

			ENDCG
		}
	}
}
