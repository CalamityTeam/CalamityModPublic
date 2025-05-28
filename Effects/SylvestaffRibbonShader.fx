sampler noiseTextureA : register(s1);

float pixelationFactor;
float uSaturation;
float4 uShaderSpecificData;
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
    float4 color = 1;
    float2 coords = input.TextureCoordinates;
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
    
    float feelerColorStart = 0.61;
    float colorSpacingFactor = 1.9;
    float4 outlineColor = float4(91, 114, 119, 255) / 255;
    
    float erasePixelInterpolant = coords.x - 0.9 - distance(coords.y, 0.5) * 0.1;
    
    // Band 1: Outline color.
    color = lerp(color, outlineColor, coords.x >= feelerColorStart);
    
    // Band 2: Pink.
    float pinkStart = feelerColorStart + colorSpacingFactor * 0.01;
    color = lerp(color, float4(221, 150, 179, 255) / 255, coords.x >= pinkStart);
    
    // Band 3: Outline color.
    float outlineAStart = pinkStart + colorSpacingFactor * 0.04;
    color = lerp(color, outlineColor, coords.x >= outlineAStart);
    
    // Band 4: Blue.
    float blueStart = outlineAStart + colorSpacingFactor * 0.01;
    color = lerp(color, float4(48, 117, 222, 255) / 255, coords.x >= blueStart);
    
    // Band 5: Baby blue.
    float babyBlueStart = blueStart + colorSpacingFactor * 0.04;
    color = lerp(color, float4(167, 222, 255, 255) / 255, coords.x >= babyBlueStart);
    
    // Add an outline to the ends of the feeler.
    float horizontalDistanceFromCenter = distance(coords.y, 0.5);
    color = lerp(color, outlineColor, horizontalDistanceFromCenter >= 0.3);
    color = lerp(color, outlineColor, erasePixelInterpolant >= -0.02);
    
    float permittedDirection = sign(uSaturation);
    float direction = sign(dot(input.Position.xy - uShaderSpecificData.xy, uShaderSpecificData.zw));
    erasePixelInterpolant *= direction == permittedDirection || coords.x < 0.5;
    
    return color * (erasePixelInterpolant < 0) * input.Color;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
