#include <Packages/com.blendernodesgraph.core/Editor/Includes/Importers.hlsl>

void Hvac_Pipes_float(float3 _POS, float3 _PVS, float3 _PWS, float3 _NOS, float3 _NVS, float3 _NWS, float3 _NTS, float3 _TWS, float3 _BTWS, float3 _UV, float3 _SP, float3 _VVS, float3 _VWS, float Speed, float2 VectorIn, Texture2D gradient_63262, Texture2D gradient_63280, Texture2D gradient_63264, out float2 ColorOut, out float3 NormalOut)
{
	
	float _SimpleNoiseTexture_63260_fac; float4 _SimpleNoiseTexture_63260_col; node_simple_noise_texture_full(VectorIn, 0, 60, 16, 0.6, 0, 1, _SimpleNoiseTexture_63260_fac, _SimpleNoiseTexture_63260_col);
	float4 _ColorRamp_63262 = color_ramp(gradient_63262, _SimpleNoiseTexture_63260_fac);
	float4 _ColorRamp_63264 = color_ramp(gradient_63264, _ColorRamp_63262);
	float _SimpleNoiseTexture_63268_fac; float4 _SimpleNoiseTexture_63268_col; node_simple_noise_texture_full(VectorIn, 0, 6, 2, 0.5, 0, 1, _SimpleNoiseTexture_63268_fac, _SimpleNoiseTexture_63268_col);
	float4 _Bump_63270; node_bump(_POS, 1, 0.02, 1, _SimpleNoiseTexture_63268_fac, _NTS, _Bump_63270);

	ColorOut = _ColorRamp_63264;
	NormalOut = _Bump_63270;
}