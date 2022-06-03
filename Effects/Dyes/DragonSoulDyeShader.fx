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

static float3 colors[4] = 
{
    float3(195, 50, 37) / 255.0,
    float3(252, 155, 0) / 255.0,
    float3(255, 235, 195) / 255.0,
    float3(252, 155, 0) / 255.0
};

float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    float4 color = tex2D(uImage0, coords);
    float interval = (sin(framedCoords.x * 6.283 + uTime * 3.141) * 2 + sin(uTime * 7 + framedCoords.y * 6)) * 0.125 + 0.5;
    interval *= 2; // Exaggerate the interval.
    float4 returnColor = float4(lerp(colors[floor(interval) % 4], colors[floor(interval + 1) % 4], interval % 1), 1); // But clamp it to a 0-1 range.
    
    return returnColor * color * sampleColor * 1.95;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}