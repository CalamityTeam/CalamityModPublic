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
    float2 swirlOffset = float2(sin(uTime * 0.26 + 1.754) * 0.31, sin(uTime * 0.26) * 0.16) * uSaturation;
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, frac(float2(framedCoords.x, framedCoords.y + uTime * 0.16) + swirlOffset - uWorldPosition * 0.0006) * 0.26);
    float4 flameColor = tex2D(uImage1, frac(float2(framedCoords.x, framedCoords.y + uTime * 0.44)) * 0.1);
    float brightnessFactor = lerp(noiseColor.r, noiseColor.g, sin(uTime * 2.46) * 0.5 + 0.5) * 2.35;
    
    float normalizedDistanceFromCenter = distance(framedCoords, float2(0.5, 0.5)) * 2;
    
    // Cause "ring" fades based on distance.
    float fadeToNormal = 1 - saturate((normalizedDistanceFromCenter - 0.67) / 0.3);
    
    // Cause noise-based stars to appear all across the sprite.
    float3 starColor = lerp(uColor * 1.1, uSecondaryColor, sin(brightnessFactor * 6.283) * 0.5 + 0.5);
    
    // Prepare a dull, bluish-greyish background across the sprite for contrast.
    color = lerp(color, float4(43 / 255.0, 58 / 255.0, 92 / 255.0, 1) * color.a, fadeToNormal * 0.45);
    
    // Fade to the secondary color based on the flame color map. This helps the overall result more vibrant.
    color = lerp(color, float4(uSecondaryColor, 1) * color.a, flameColor.r * 0.5);
    
    // And create stars based on a noise texture that rise upward.
    color = lerp(color, float4(starColor, 1) * color.a, fadeToNormal * pow(brightnessFactor, 6) * 0.15);
    return color * (1 + brightnessFactor) * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}