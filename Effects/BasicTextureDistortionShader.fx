sampler baseTexture : register(s0);
sampler distortionTexture : register(s1);

float time;
float noiseScale;
float distortionStrength;
float2 timeOffset;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR
{
    float2 distortedCoords = coords + float2(time * timeOffset.x, time * timeOffset.y);
    float distortion = tex2D(distortionTexture, distortedCoords * noiseScale).r;
    return tex2D(baseTexture, coords + distortion * distortionStrength);
}

technique Technique1
{
    pass DistortionPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
