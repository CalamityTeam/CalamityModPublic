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
    float2 coords = input.Coordinates - 0.5;
    float rotationSine = sin(uSaturation);
    float rotationCosine = sin(uSaturation + 1.57);
    float rotationOriginSine = sin(uCircularRotation);
    float rotationOriginCosine = sin(uCircularRotation + 1.57);
    float2x2 rotationMatrix = float2x2(rotationCosine, -rotationSine, rotationSine, rotationCosine);
    float2x2 circularRotationMatrix = float2x2(rotationOriginCosine, -rotationOriginSine, rotationOriginSine, rotationOriginCosine);
    float2x2 scalingMatrix = float2x2(2, 0, 0, 1);
    
    output.Color = input.Color;
    
    // Rotate based on direction, squash the result, and then rotate the squashed result by the circular rotation.
    output.Coordinates = mul(input.Coordinates - 0.5, rotationMatrix) + 0.5;
    output.Coordinates = mul(output.Coordinates - 0.5, scalingMatrix) + 0.5;
    output.Coordinates = mul(output.Coordinates - 0.5, circularRotationMatrix) + 0.5;
    output.Position = mul(input.Position, uWorldViewProjection);

    return output;
}

float4 PixelFunction(VertexShaderOutput input) : COLOR0
{
    float2 updatedCoords = input.Coordinates;
    
    // Adjust for horizontal rotation.
    if (uDirection == -1)
        updatedCoords.y = 1 - updatedCoords.y;
    
    float overallAdjustedYCoord = 0.5 + lerp(-uImageSize0.y / overallImageSize.y, uImageSize0.y / overallImageSize.y, updatedCoords.y) * 0.5;
    float4 baseColor = tex2D(uImage0, updatedCoords);
    float colorFade = abs(sin(overallAdjustedYCoord + uTime * 0.5));
    float luminosity = (baseColor.r + baseColor.g + baseColor.b) / 3;
    float4 endColor = baseColor * float4(lerp(uColor, uSecondaryColor, colorFade), 1);
    endColor *= 1 + luminosity * 0.5;
    return (endColor * 0.7 + baseColor * 0.5) * baseColor.a * uOpacity;
}

technique Technique1
{
    pass ShieldPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelFunction();
    }
}