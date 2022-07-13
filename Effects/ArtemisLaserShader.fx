sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
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
float2 uImageSize2;
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

float InverseLerp(float from, float to, float x)
{
    return saturate((x - from) / (to - from));
}

// The X coordinate is the trail completion, the Y coordinate is the same as any other.
// This is simply how the primitive TextCoord is layed out in the C# code.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    
    // Read the fade map as a streak.
    float bloomFadeout = pow(sin(coords.y * 3.141), 4);
    float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.x * 20 - uTime * 5.5), coords.y));
    float opacity = (0.5 + fadeMapColor.g) * bloomFadeout;
    
    // Calculate electricity colors.
    float electricityFade = tex2D(uImage2, float2(frac(coords.x * 6 - uTime * 2.5), coords.y)).r;
    float4 electricityColor = InverseLerp(0.4, 0.5, electricityFade * bloomFadeout) * float4(uColor, 1);
    
    // Fade out at the ends of the streak.
    if (coords.x < 0.018)
        opacity *= pow(coords.x / 0.018, 6);
    if (coords.x > 0.95)
        opacity *= pow(1 - (coords.x - 0.95) / 0.05, 6);
    return color * opacity * 2 + electricityColor * opacity;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
