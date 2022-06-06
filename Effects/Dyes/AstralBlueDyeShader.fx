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
    float interval = cos(uTime * 7 + framedCoords.x * 4) / 2 + 0.5;
    float4 returnColor = float4(lerp(uColor, uSecondaryColor, interval), 1); // Fade between two colors depending on the time and x position on the texture in a 0-1 cosine linear interpolation. The color can vary.
    return returnColor * sampleColor * color; // And multiply by the original color to give it a blend effect.
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}