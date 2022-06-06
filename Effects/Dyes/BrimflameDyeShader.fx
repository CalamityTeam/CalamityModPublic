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
    float4 color = tex2D(uImage0, coords);
    
    float3 alteredColor = lerp(uColor, float3(1, 0, 0), 0.65);
    float3 alteredSecondaryColor = lerp(uSecondaryColor, float3(1, 0, 0), 0.65);
    
    // Fade out at the edges of the trail to give a blur effect.
    // This is very important.
    float flameStreakBrightness = tex2D(uImage1, float2(frac(framedCoords.y + uTime * 1.4) * 0.4, framedCoords.x * 0.65)).r;
    float fadeToRed = lerp(0.2, 1, flameStreakBrightness);
    float3 flameColor = lerp(alteredColor, alteredSecondaryColor, fadeToRed);
    flameColor.r = 0.6 + sqrt(fadeToRed) * 0.4;
    color = lerp(color, float4(flameColor, 1), 0.45) * color.a;
    return color * lerp(1.3, 5.4, pow(flameStreakBrightness, 4.44)) * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}