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
    float luminosity = (color.r + color.g + color.b) / 3;
    float yMultiplier = sqrt(framedCoords.y) * (sin(framedCoords.y * 2 + uTime * 1.4) + 1) * 0.5;

    color.rgb *= ((framedCoords.x * uColor * (1 - yMultiplier) * 1.4) + ((1 - framedCoords.x) * uSecondaryColor * yMultiplier * 1.7)) * luminosity * 2.1;
    color.rgb *= clamp(1.6 * saturate(distance((0.5, 0.5), framedCoords)), 0.8, 1.6); // Intensify the colors based on how far they are from the center of the sprite.
    return color * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}