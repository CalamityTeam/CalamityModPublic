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

float randomness(float2 uv)
{
    float noise = (frac(sin(dot(uv, float2(12.9898, 78.233) * 2.0)) * 43758.5453));
    return noise;
}

float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    coords.x += randomness(uTime + framedCoords.y).x * (1 - framedCoords.y) * 6 / uImageSize0.x; // Add an X offset based on a noise function, giving a moon lord void-esque shader effect.
    float4 color = tex2D(uImage0, coords);
    float4 returnColor = color * 0.1;
    returnColor.a = color.a;
    return returnColor * sampleColor;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}