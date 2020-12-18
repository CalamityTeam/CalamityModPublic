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

// The X coordinate is the trail completion, the Y coordinate is the same as any other.
// This is simply how the primitive TextCoord is layed out in the C# code.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    
    // Read the fade map as a streak.
    float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.x + uTime / 4.16), coords.y));
    fadeMapColor.r *= pow(coords.x, 0.4);
    float opacity = 1;
    
    // Fading out.
    if (fadeMapColor.r < 0.31)
        opacity = fadeMapColor.r / 0.31;
    
    // Redish burn fade if close but not close enough to fading out.
    else if (fadeMapColor.r < 0.44)
        color.rgb = lerp(color.rgb, float3(1, 0.1, 0.2), 0.7 - (fadeMapColor.r - 0.31) / 0.13 * 0.7);
    
    float bloomPower = lerp(2.1, 7.4, saturate(1 - coords.x + sin(uTime * 2.3 + coords.x * 6.283) * 0.2));
    opacity *= pow(sin(coords.y * 3.141), bloomPower);
    
    color.rgb = lerp(color.rgb, float3(1, 1, 1), 0.5);
    return color * opacity;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
