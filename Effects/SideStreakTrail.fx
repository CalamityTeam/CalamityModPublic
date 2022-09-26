sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
matrix uWorldViewProjection;
float4 uShaderSpecificData;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float innerBrightnessIntensity = 2.7;
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    float bloomOpacity = pow(cos(coords.y * 4.8 - 0.8), 55 + pow(coords.x, 4) * 700);
    
    // Create some noisy opaque blotches in the inner part of the trail.
    if (coords.y > 0.15 && coords.y < 0.85)
    {
        float minOpacity = pow(1 - sin(coords.y * 3.141) + tex2D(uImage1, coords * 1.1 + float2(uTime * -0.6, 0)).r * 2.2, 0.2);
        bloomOpacity += pow(coords.x, 0.3) * lerp(0.04, 0.4, minOpacity);
    }
    
    // Make the front half of the trail have a strong bloom effect.
    if (coords.x < 0.5)
        bloomOpacity *= (1 - coords.x * 2) * innerBrightnessIntensity + 1;
    
    return color * lerp(0, 3.6, bloomOpacity * pow(coords.x, 0.1)) * pow(1 - coords.x, 1.1);
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
