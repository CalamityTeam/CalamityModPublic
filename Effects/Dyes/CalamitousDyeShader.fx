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

float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    float4 noiseColor = tex2D(uImage1, framedCoords);
    float4 color = tex2D(uImage0, coords);
    float3 blendColor = lerp(uColor, uSecondaryColor, sqrt(framedCoords.y));
    float blendFactor = (cos(uTime * 9 + framedCoords.x * 18) * 0.5 + 0.5) * 0.5;
    
    // Allow the fade to pulse upward based on how far up the pixel is.
    blendFactor += (cos(uTime * -13 - framedCoords.y * 7.1)) * 0.5;
    float brightness = blendFactor * 0.35 + noiseColor.r * 0.35;
    
    // Cause the effects to taper off at the bottom of the sprite.
    if (framedCoords.y < 0.2)
    {
        brightness *= framedCoords.y / 0.2;
        blendFactor *= framedCoords.y / 0.2;
    }
    float4 colorBlendMultiplier = lerp(float4(blendColor, 1), float4(1, 1, 1, 1), saturate(pow(blendFactor * 1.5, 2)));
    return (lerp(color, float4(blendColor, 1), blendFactor * 0.5 + 0.2) * color.a) * colorBlendMultiplier * (1 + brightness) * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}