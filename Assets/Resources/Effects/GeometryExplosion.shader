Shader "Custom/GeometryExplosion" 
{
	SubShader{
		Pass{

		Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform float gAmbientRed;
		float gAmbientGreen;
		uniform float gAmbientBlue;
		uniform float gOffset;
		
		
		struct vsOutput {
			float4 pos : SV_POSITION;
			float3 n : NORMAL;
		};

		vsOutput vert(appdata_base v)
		{
			vsOutput o;
			o.pos = float4(v.vertex.xyz, 1.0f);
			o.n = v.normal;
			
			return o;
		}

		struct gsOutput
		{
			float4 pos : SV_POSITION;
		};

		[maxvertexcount(3)]
		void geom(triangle vsOutput input[3], inout TriangleStream<gsOutput> TriStream)
		{
			gsOutput output;
			
			matrix m = 0;
			float fac = 1.0f / max(gOffset, 1.0f);
			m._11 = fac;
			m._22 = fac;
			m._33 = fac;
			m._44 = 1.0f;

			

			for (int i = 0; i < 3; ++i)
			{
				float3 p = mul(input[i].pos, m);
				output.pos = UnityObjectToClipPos(float4(p + input[0].n * gOffset, 1.0f));

				TriStream.Append(output);
			}

			TriStream.RestartStrip();
		}


		fixed4 frag() : SV_Target
		{ 
			return float4(gAmbientRed, gAmbientGreen, gAmbientBlue, 1.0f);
		}
			
			ENDCG
		}
	}
}