Shader "Custom/BufferShader"
{
    SubShader
    {
        Pass
        {
            ZTest Less
			Cull Back
			ZWrite Off

            Fog{ Mode off }

            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag

            struct Particle
            {
                float3 position;
                float pad;
            };

            uniform StructuredBuffer<Particle> gParticleBuffer;

            struct VSoutput
            {
                float4 position : SV_POSITION;
                float3 color : COLOR;
            };

            VSoutput vert(uint vID : SV_VertexID, uint iID : SV_InstanceID)
            {
                float3 lensRight = UNITY_MATRIX_IT_MV[0].xyz;
                float3 lensUp = UNITY_MATRIX_IT_MV[1].xyz;
                float3 lensForward = UNITY_MATRIX_IT_MV[1].xyz;

                Particle particle = gParticleBuffer[iID];
                float3 pPosition = particle.position;

                float3 pForward = normalize(_WorldSpaceCameraPos - pPosition);
                float3 pRight = cross(pForward, lensUp);
                float3 pUp = cross(pRight, pForward);
                float2 scale = float2(1, 1);

                float x = vID == 0 || vID == 1 || vID == 5;
                float y = vID == 0 || vID == 2 || vID == 4;

                float2 uv = float2(x, 1.0f - y);

                float3 vPosition = pPosition + pRight * ((x * 2.f - 1.f) * scale.x) + pUp * ((y * 2.f - 1.f) * scale.y);

                VSoutput output;
                output.position = UnityObjectToClipPos(float4(vPosition, 1));
                output.color = float3(uv, 0);

                return output;
            }

            float4 frag(VSoutput input) : SV_TARGET0
            {
                return float4(input.color, 1);
            }

            ENDCG
        }
    }
}
