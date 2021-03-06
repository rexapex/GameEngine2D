﻿cbuffer PerObject: register(b0)
{
	float4x4 WorldViewProj;
};

struct VSOut
{
	float4 position : SV_POSITION;
	float2 textureUV : TEXCOORD0;
};

VSOut main(float4 position : POSITION, float2 textureUV : TEXTUREUV)
{
	VSOut output;
	output.position = mul(WorldViewProj, position);
	output.textureUV = textureUV;
	return output;
}
