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

static float xAdditiveMax = 0.035;
float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    float4 color = tex2D(uImage0, coords);
    float sineTime = sin(uTime); // Saved so that I don't have the compute this multiple times. Shaders have a limited number of mathematical instructions you can use - 64.
    float2 modifiedCoords = coords;
    modifiedCoords.x += sin(uTime + framedCoords.y + 1.57) * 0.5 + 0.5; // Cause the effect to sway around a bit with time.
    
    float4 noiseColor = tex2D(uImage1, frac(modifiedCoords));
    
    float xMultiplier = sin(framedCoords.x * 2.3 + uTime * 1.15 + 1.57) + 0.5;
    float yMultiplier = (sin(framedCoords.y * 2 + uTime * 1.4) + 1) * 0.5;

    color.rgb *= lerp(uColor, uSecondaryColor, saturate(xMultiplier * yMultiplier * 1.6));
    if (noiseColor.r > 0.55 + sineTime * 0.025)
    {
        color.rgb *= 1.6; // Sometimes give light "patches" depending on the swaying noise image.
    }
    color.rgb *= 1.6; // Brighten the shader over time.
    return color * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}