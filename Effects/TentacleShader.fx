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
float4 uShaderSpecificData;

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float trailLengthRatio = uSaturation + (coords.x + coords.y) / 2 * uOpacity;
    
    float r = (sin(uTime + trailLengthRatio * 12.566) + 1) * 0.5;
    float g = (-cos(uTime + trailLengthRatio * 12.566) + 1) * 0.5;
    float b = (-sin(uTime + trailLengthRatio * 12.566) + 1) * 0.5;
    
    float a = (trailLengthRatio * 0.5 + 0.5) * (abs(coords.x - 0.5)) / 0.5;
    a *= tex2D(uImage1, coords).r;

    float4 returnColor = float4(r, g, b, a);
    
    float whiteness = 0;
    
    if (trailLengthRatio > 0.7)
        whiteness = (trailLengthRatio - 0.7) / 0.3;
    
    returnColor.rgb = lerp(returnColor.rgb, float3(1, 1, 1), whiteness) * trailLengthRatio * 0.6;
    
    return returnColor * tex2D(uImage0, coords);
}
technique Technique1
{
    pass BurstPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}