Shader "Custom/GeometryExplosion" 
{
	SubShader
	{
		Pass
		{
			ZTest Less
			ZWrite Off
			Cull Off
			BlendOp Add

			//Blend SrcAlpha One
			Blend SrcAlpha OneMinusSrcAlpha

			Fog{ Mode off }

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag

			uniform float gOffset;
			// ---

			struct vsOutput
			{
				float4 svPosition : SV_POSITION;
				float3 normal : NORMAL;
				float3 ambient : AMBIENT;
				float2 lifetime : LIFETIME;
			};

			vsOutput vert(appdata_base v)
			{
				vsOutput output;

				output.svPosition = v.vertex;
				output.normal = v.normal;
				output.ambient = float3(1, 0, 0);// gAmbient[vID].xyz;
				output.lifetime = float2(0, 0);// gLifetime[vID].xy;

				return output;
			}

			// ---

			struct gsOutput
			{
				float4 svPosition : SV_POSITION;
				float3 ambient : AMBIENT;
				float2 uv : UV;
			};

			[maxvertexcount(3)]
			void geom(triangle vsOutput input[3], inout TriangleStream<gsOutput> TriStream)
			{
				gsOutput output;

				output.ambient = input[0].ambient;
				output.uv = float2(0,0);

				for (int i = 0; i < 3; ++i)
				{
					output.svPosition = UnityObjectToClipPos(float4(input[i].svPosition.xyz, 1.0f));// +input[0].normal * gOffset, 1.0f));

					TriStream.Append(output);
				}

				TriStream.RestartStrip();
			}

			// ---

			float4 frag(gsOutput input) : SV_TARGET0
			{
			

				return float4(gOffset, 0, 0, 1.0f);
			}

			ENDCG
		}
	}













	
	
	/*Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"*/
}
