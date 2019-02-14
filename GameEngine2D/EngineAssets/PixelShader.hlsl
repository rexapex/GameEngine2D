Texture2D ShaderTexture : register(t0);
SamplerState Sampler : register(s0);

struct VSOut
{
	float4 position : SV_POSITION;
	float2 textureUV : TEXCOORD0;
};

float4 main(VSOut input) : SV_TARGET
{
	return ShaderTexture.Sample(Sampler, input.textureUV);
}
