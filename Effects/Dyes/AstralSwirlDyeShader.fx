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
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float2 RotatedBy(float2 xy, float theta)
{
    return float2(xy.x * sin(theta + 1.57) + xy.y * sin(theta), xy.x * sin(theta) - xy.y * sin(theta + 1.57));
}

float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    float4 color = tex2D(uImage0, coords);
    float interval = (sin(uTime * 2.2 + framedCoords.x * framedCoords.y * 1.55 + 1.57) * 0.5 + 0.5) * 0.6 + 0.2; // A complex function that works based on the X and Y coords with a 0.2-0.8 range.
    float brightness = 1.35;
    float3 returnColor = uColor;
    // Rotate the colors around and adjust the brightness.
    for (int i = 0; i < 4; i++)
    {
        float2 positionAt = float2(interval, 1 - interval);
        positionAt = (RotatedBy(float2(sin(uTime * 1.7 + i / 4 * 6.283), sin(uTime * 1.9 + i / 4 * 6.283)), uTime + i / 4 * 6.283) * 0.5 + 0.5) - positionAt;
        float dist = distance(coords, positionAt);
        if (dist < 0.35)
        {
            float inverseDistanceRatio = 1 - dist / 0.35;
            returnColor = lerp(uColor, uSecondaryColor, inverseDistanceRatio);
            brightness = lerp(1.35, 1.7, inverseDistanceRatio);
        }
    }
    
    if (framedCoords.y > 0.5)
    {
        returnColor = lerp(returnColor, uSecondaryColor, (framedCoords.y - 0.5) * 3);
    }
    
    return float4(returnColor, 1) * color * sampleColor * brightness;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}