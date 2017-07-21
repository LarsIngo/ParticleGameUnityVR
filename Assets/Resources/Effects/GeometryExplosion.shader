Shader "Custom/GeometryExplosion" 
{
	Properties
	{
		_MainTex("Texture", 2D) = "red" {}
	}

	SubShader{
		Pass{

		Cull Off

		CGPROGRAM
		#pragma vertex vert
		#pragma geometry geom
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform float gAmbientRed;
		uniform float gAmbientGreen;
		uniform float gAmbientBlue;
		uniform float gOffset;
		

		struct vsOutput {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			uint id : VERTID;
		};

		sampler2D _MainTex;

		float4 _MainTex_ST;

		vsOutput vert(appdata_base v, uint vid : SV_VertexID)
		{
			vsOutput o;
			o.pos = float4(v.vertex.xyz, 1.0f);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.id = vid;
			
			return o;
		}

		struct gsOutput
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		[maxvertexcount(3)]
		void geom(triangle vsOutput input[3], inout TriangleStream<gsOutput> TriStream)
		{
			gsOutput output;
			
			int rID = (int)(input[0].id) % 16;

			matrix m = 0;
			//Scaling with z rotaation

			float fac = 1.0f / max(gOffset * rID, 1.3f) + exp(gOffset / 3) - 0.7f;
			
			float rot = gOffset / rID;
			
			float s = sin(rot);
			float c = cos(rot);
			
			if (gOffset > 3.0f)
			{
				m = 0;
			}
			else if (gOffset > 2.0f)
			{
				float test = gOffset * gOffset * gOffset;
				fac = 10.0f / (test);
				m._11 = fac;
				m._22 = fac;
				m._33 = fac;
				//m = 0;
			}
			else
			{
				m._11 = fac * c;	m._12 = -s;
				m._21 = s; m._22 = fac * c;
				m._33 = fac;
				m._44 = 1.0f;
			}


			float3 p[3];
			p[0] = mul(input[0].pos, m);
			p[1] = mul(input[1].pos, m);
			p[2] = mul(input[2].pos, m);

			float3 v1 = normalize(p[0] - p[1]);
			float3 v2 = normalize(p[0] - p[2]);

			float3 n = cross(v1, v2);

			for (int i = 0; i < 3; ++i)
			{ 
				output.pos = UnityObjectToClipPos(float4(p[i] + n * gOffset * rID / 10.0f, 1.0f));
				output.uv = input[i].uv;

				TriStream.Append(output);
			}

			TriStream.RestartStrip();
		}

		fixed4 frag(gsOutput i) : SV_Target
		{
			fixed4 texcol = tex2D(_MainTex, i.uv);
			return texcol;// float4(gAmbientRed, gAmbientGreen, gAmbientBlue, 1.0f);
		}
			
			ENDCG
		}
	}
}