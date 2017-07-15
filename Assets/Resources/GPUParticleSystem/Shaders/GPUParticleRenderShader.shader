Shader "Custom/BufferShader"
{
    SubShader
    {
        Pass
        {
            ZTest Less
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Fog{ Mode off }

            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag

            StructuredBuffer<float4> gPosition;
            StructuredBuffer<float4> gVelocity;
            StructuredBuffer<float4> gScale;
            StructuredBuffer<float4> gColor;

            struct VSoutput
            {
                float4 position : SV_POSITION;
                float3 color : COLOR;
                float2 uv : UV;
            };

            VSoutput vert(uint vID : SV_VertexID, uint iID : SV_InstanceID)
            {
                float3 lensRight = UNITY_MATRIX_IT_MV[0].xyz;
                float3 lensUp = UNITY_MATRIX_IT_MV[1].xyz;
                float3 lensForward = UNITY_MATRIX_IT_MV[2].xyz;

                float3 pPosition = gPosition[iID].xyz;
                float2 pScale = gScale[iID].xy;
                float3 pColor = gColor[iID].xyz;

                float3 pForward = normalize(_WorldSpaceCameraPos - pPosition);
                float3 pRight = cross(pForward, lensUp);
                float3 pUp = cross(pRight, pForward);

                float x = vID == 0 || vID == 1 || vID == 5;
                float y = vID == 0 || vID == 2 || vID == 4;

                float3 vPosition = pPosition + pRight * ((x * 2.f - 1.f) * pScale.x) + pUp * ((y * 2.f - 1.f) * pScale.y);

                VSoutput output;
                output.position = UnityObjectToClipPos(float4(vPosition, 1));
                output.color = pColor;
                output.uv = float2(x, 1.0f - y);

                return output;
            }

            float4 frag(VSoutput input) : SV_TARGET0
            {
                float x = input.uv.x - 0.5f;
                float y = input.uv.y - 0.5f;
                float r = sqrt(x * x + y * y);
                float factor = max(1.f - r * 2.f, 0.f); //[1,0]
                float sinFactor = 1.f - sin(3.14159265f / 2.f * (factor + 1.f));

                return float4(input.color, sinFactor);
            }

            ENDCG
        }
    }
}
