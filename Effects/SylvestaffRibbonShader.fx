sampler noiseTextureA : register(s1);

float pixelationFactor;
float feelerColorStart;
float colorSpacingFactor;
float4 outlineColor;
matrix uWorldViewProjection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
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

float QuadraticBump(float x)
{
    return x * (4 - x * 4);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    // Pixelate coords.
    coords = round(coords * pixelationFactor) / pixelationFactor;
    
    float erasePixelInterpolant = coords.x - 0.9 - distance(coords.y, 0.5) * 0.1;
    
    // Band 1: Outline color.
    color = lerp(color, outlineColor, coords.x >= feelerColorStart);
    
    // Band 2: Pink.
    float pinkStart = feelerColorStart + colorSpacingFactor * 0.01;
    color = lerp(color, float4(237, 145, 222, 255) / 255, coords.x >= pinkStart);
    
    // Band 3: Outline color.
    float outlineAStart = pinkStart + colorSpacingFactor * 0.04;
    color = lerp(color, outlineColor, coords.x >= outlineAStart);
    
    // Band 4: Blue.
    float blueStart = outlineAStart + colorSpacingFactor * 0.01;
    color = lerp(color, float4(29, 145, 248, 255) / 255, coords.x >= blueStart);
    
    // Band 5: Baby blue.
    float babyBlueStart = blueStart + colorSpacingFactor * 0.04;
    color = lerp(color, float4(159, 222, 255, 255) / 255, coords.x >= babyBlueStart);
    
    // Add an outline to the ends of the feeler.
    float horizontalDistanceFromCenter = distance(coords.y, 0.5);
    color = lerp(color, outlineColor, horizontalDistanceFromCenter >= 0.3);
    color = lerp(color, outlineColor, erasePixelInterpolant >= -0.02);
    
    return color * (erasePixelInterpolant < 0);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
