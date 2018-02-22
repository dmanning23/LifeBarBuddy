#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

// Effect renders portions of a normal-mapped image

float Start = 0.0;
float End = 0.0;
bool HasBorder;

sampler TextureSampler : register(s0);
sampler BorderSampler : register(s1)
{
	Texture = (BorderTexture);
};
sampler AlphaMaskSampler : register(s2)
{
	Texture = (AlphaMaskTexture);
};

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 result = { 0,0,0,0 };

	//Look up the texture value
	float4 tex = tex2D(TextureSampler, texCoord);

	if (HasBorder)
	{
		//If we are drawing the border, just do a pass-through
		result = tex2D(BorderSampler, texCoord);
		if (result.a > 0.0)
		{
			result.a *= color.a;
		}
	}
	else if (tex.a > 0.0)
	{
		//Dont do these calculations if the alpha channel is empty

		//Get the color from the palette swap texture
		float4 alphaMask = tex2D(AlphaMaskSampler, texCoord);

		//If the alpha value falls in the range we are looking for, return the specified color
		if (Start <= alphaMask.a && alphaMask.a <= End)
		{
			result = tex * color;
		}
		else if (Start <= alphaMask.a && alphaMask.a <= (End + 0.02))
		{
			result = tex * color;
			result.a *= tex.a * color.a * (1 - ((alphaMask.a - End) / ((End + 0.02) - End)));
		}
		else if ((Start - 0.02) <= alphaMask.a && alphaMask.a <= End)
		{
			result = tex * color;
			result.a *= tex.a * color.a * (((alphaMask.a - (Start - 0.02)) / (Start - (Start - 0.02))));
		}
	}

	return result;
}

technique Normalmap
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
