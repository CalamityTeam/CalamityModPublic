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

static float Variance = 0.4f;
static float MinBrightness = 1.5f;
static float MaxBrightness = 2.5f;

float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = InverseLerp(uLegacyArmorSourceRect.wx, uLegacyArmorSourceRect.wx + uLegacyArmorSourceRect.yz, uLegacyArmorSourceRect.wx + coords * uLegacyArmorSourceRect.yz);
    float4 color = tex2D(uImage0, coords);
    
    float4 noiseColor = tex2D(uImage1, coords);
    float interval = cos(uTime * 7 + framedCoords.y * 6) * 0.5 + 0.5;

    float brightness = pow((interval + sin(uTime + framedCoords.x * uTime * framedCoords.y * cos(uTime)) * 0.5 + 0.5) * 0.5, 2); // Force the brightness to rise to 1 more quickly
    brightness = brightness * (MaxBrightness - MinBrightness) + MinBrightness; // And bring it to a specified range

    float4 colorMap = float4(lerp(uColor, uSecondaryColor, interval) + noiseColor.rgb * 0.4, 1);
    colorMap.rgb *= brightness;
    colorMap.rgb = colorMap.rgb - colorMap.rgb % Variance; // Slice the return color map to make pixels appear less varied and more extreme
    
    return colorMap * color * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}