sampler baseTexture : register(s0);

float time;
float waveAmplitude;
float waveFrequency;
bool distortHorizontally;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 horizontalDistortion = sin(coords.x * waveFrequency + time) * waveAmplitude;
    float2 verticalDistortion = sin(coords.y * waveFrequency + time) * waveAmplitude;
    
    if (distortHorizontally)
        coords.y += horizontalDistortion;
    else
        coords.x += verticalDistortion;
    
    return tex2D(baseTexture, coords) * sampleColor;
}

technique Technique1
{
    pass SinePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}