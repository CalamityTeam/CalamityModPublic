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
    float4 pixelBelow = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(framedCoords.x, frac(framedCoords.y - uTime * 0.67)) * 0.5) * 1.5;
    float verticalFlowMovement = (sin(framedCoords.x * 14.71 + framedCoords.y * 13.5 - uTime * 5.6) * 0.5 + 0.5) * noiseColor.r;
    if (pixelBelow.a == 0 && framedCoords.y < 0.96)
        coords.y = saturate(coords.y + verticalFlowMovement / uSourceRect.w / 17);
    float4 color = tex2D(uImage0, coords);

    // Prepare the dark outlines.
    float outlineFade = pow(1 - (color.r + color.g + color.b) / 3, 2);
    color = lerp(color, float4(uColor, 1), outlineFade) * color.a;
    
    // Prepare the bright in-lines.
    color = lerp(color, float4(uSecondaryColor, 1), saturate(1 - outlineFade) * 0.8) * color.a;
    
    // Handle red fades based on noise.
    color = lerp(color, float4(1, 0, 0, 1), pow(noiseColor.r, 3) * 0.3 + verticalFlowMovement * 0.3 + 0.16) * color.a;
    
    return color * (1 + (1 - outlineFade) * 0.25) * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}