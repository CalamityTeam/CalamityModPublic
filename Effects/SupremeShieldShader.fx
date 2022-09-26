sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity : register(C0);
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uShaderSpecificData;

float inverseLerp(float x, float min, float max)
{
    return saturate((x - min) / (max - min));
}
float4 PixelFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(coords.x, frac(coords.y + uTime * 0.7)));
    float start = 0.45 + sin(uTime * 3) * 0.04;
    float ringRadus = 0.1;
    float distanceRatio = distance(coords, float2(0.5, 0.5)) * 1.414;
    float3 colorToUse = lerp(uColor, uSecondaryColor, pow(distanceRatio, 0.6));
    float originalAlpha = pow(inverseLerp(distanceRatio, 0, start), 2.6);
    float ringAlpha = sin(3.141 * inverseLerp(distanceRatio, start, start + ringRadus)) * 1.96;
    if (distanceRatio < start)
        originalAlpha *= lerp(1, 2.6, noiseColor.r);
    else
        originalAlpha = lerp(originalAlpha, ringAlpha, inverseLerp(distanceRatio, start, start + ringRadus * 0.5));
    
    originalAlpha *= lerp(1, 0.56, distanceRatio) * lerp(1, 2.45, min(1, pow(sin(uTime * 3.1), 10) * 0.7 + uSaturation));
    baseColor = float4(lerp(baseColor.rgb, colorToUse, 0.5), 1.0);
    return baseColor * originalAlpha * uOpacity;
}

technique Technique1
{
    pass ShieldPass
    {
        PixelShader = compile ps_2_0 PixelFunction();
    }
}