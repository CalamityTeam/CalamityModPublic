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
    float4 noiseColor = tex2D(uImage1, float2(framedCoords.x, frac(framedCoords.y + uTime * 0.7)));
    float4 color = tex2D(uImage0, coords);
    float normalizedDistanceFromCenter = distance(framedCoords, float2(0.5, 0.5)) * 2;
    
    // Cause "ring" pulses based on distance.
    float fadeToSecondaryColor = pow(sin(uTime * -2.56 + normalizedDistanceFromCenter * 2), 4);
        
    // This fades out at the ends of the sprite.
    if (normalizedDistanceFromCenter > 0.7)
        fadeToSecondaryColor *= saturate(1 - (normalizedDistanceFromCenter - 0.7) / 0.37 + sin(framedCoords.x * framedCoords.y * 12) * 0.1);
    float4 endFadeColor = float4(lerp(uColor, uSecondaryColor, fadeToSecondaryColor * 0.74 + noiseColor.g * 0.13), 1);
    
    float4 blendedColor = (color + endFadeColor) * 0.5 - 0.05;
    blendedColor *= 1.4;
    blendedColor.a = 1;
    return blendedColor * color.a * (1 + lerp(0.07, 0.3, fadeToSecondaryColor) + noiseColor.g * 0.1) * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}