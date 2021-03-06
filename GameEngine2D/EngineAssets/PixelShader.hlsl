﻿Texture2D ShaderTexture : register(t0);
SamplerState Sampler : register(s0);

struct VSOut
{
	float4 position : SV_POSITION;
	float2 textureUV : TEXCOORD0;
};

struct PSOut
{
	float4 color: SV_TARGET;
};

PSOut main(VSOut input)
{
	PSOut output;
	output.color = ShaderTexture.Sample(Sampler, input.textureUV);
	return output;
}
