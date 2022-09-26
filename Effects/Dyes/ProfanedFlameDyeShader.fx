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
    
    // Read the fade map as a streak.
    float4 fadeMapColor = tex2D(uImage1, float2(frac(framedCoords.x * 0.18 + sin(uTime * 0.9) * 0.021), frac(framedCoords.y * 0.32 + uTime * 0.2)));
    fadeMapColor.r *= pow(framedCoords.x, 0.2);
    
    float opacity = lerp(1.05, 1.95, fadeMapColor.r);
    opacity *= fadeMapColor.r + 1;
    opacity *= lerp(0.4, 1.1, fadeMapColor.r);
    
    float3 transformColor = lerp(uColor, uSecondaryColor, fadeMapColor.r);
    float orangeFade = 1 - saturate((framedCoords.y - 0.08) / 0.74);
    opacity += (1 - orangeFade) * 0.6;
    
    transformColor = lerp(transformColor, float3(206 / 255, 116 / 255, 59 / 255), orangeFade);
    color.rgb = lerp(color.rgb, transformColor, fadeMapColor.r);
    opacity *= color.a;
    return color * sampleColor * opacity;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}