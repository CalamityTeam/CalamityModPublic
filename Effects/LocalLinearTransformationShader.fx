sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity : register(C0);
float uSaturation;
float uCircularRotation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float2 overallImageSize;
matrix uWorldViewProjection;
float2x2 localMatrix;
float4 uShaderSpecificData;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 Coordinates : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 Coordinates : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Color = input.Color;
    output.Coordinates = mul(input.Coordinates - 0.5, localMatrix) + 0.5;
    output.Position = mul(input.Position, uWorldViewProjection);

    return output;
}

float4 PixelFunction(VertexShaderOutput input) : COLOR0
{
    if (input.Coordinates.x < 0 || input.Coordinates.x > 1 || input.Coordinates.y < 0 || input.Coordinates.y > 1)
        return 0;

    float4 color = tex2D(uImage0, input.Coordinates) * input.Color;
    float originalAlpha = color.a;
    color.rbg = lerp(color.rbg, uColor, uOpacity);
    return color * originalAlpha;
}

technique Technique1
{
    pass TransformationPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelFunction();
    }
}