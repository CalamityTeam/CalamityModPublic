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
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

// The X coordinate is the trail completion, the Y coordinate is the same as any other.
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    
    // Read the fade map as a streak.
    float fadeMapY = frac(coords.x - uTime * 1.4 * uSaturation);
    float4 fadeMapColor = tex2D(uImage1, float2(coords.y, fadeMapY));
    fadeMapColor.r *= pow(coords.x, 0.2);
    
    float opacity = lerp(1.45, 1.95, fadeMapColor.r) * color.a;
    opacity *= pow(sin(coords.y * 3.141), lerp(1, 4, pow(coords.x, 2)));
    opacity *= fadeMapColor.r * 1.5 + 1;
    opacity *= lerp(0.4, 0.9, fadeMapColor.r);
    
    float3 transformColor = lerp(float3(1, 205 / 255.0, 119 / 255.0), float3(1, 76 / 255.0, 79 / 255.0), fadeMapColor.r);
    color.rgb = lerp(color.rgb, transformColor, fadeMapColor.r);
    return color * opacity * 1.6;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
