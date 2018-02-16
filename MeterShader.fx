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

sampler TextureSampler : register(s0);
sampler AlphaMaskSampler : register(s1)
{
	Texture = (AlphaMaskTexture);
};

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	//Look up the texture value
	float4 tex = tex2D(TextureSampler, texCoord);
	
	//Dont do these calculations if the alpha channel is empty
	if (tex.a > 0.0)
	{
		//Get the color from the palette swap texture
		float4 alphaMask = tex2D(AlphaMaskSampler, texCoord);

		//If the alpha value falls in the range we are looking for, return the specified color
		if (Start <+ alphaMask.a && alphaMask.a <= End)
		{
			return tex * color;
		}
	}

	return {0, 0, 0, 0};
}

technique Normalmap
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}
