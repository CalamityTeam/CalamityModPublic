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
    float4 noiseColor = tex2D(uImage1, float2(frac(framedCoords.x + uTime * 0.1), framedCoords.y));
    float4 color = tex2D(uImage0, coords);
    
    // Cause sharp brightness increases based on a perlin noise map.
    // This causes icy, crystaline structures to appear.
    float brightness = pow((noiseColor.r + noiseColor.g + noiseColor.b) / 3, 3) * 3;
    float normalizedDistanceFromCenter = distance(framedCoords, float2(0.5, 0.5)) * 2;
    
    // Cause "ring" fades based on distance.
    float fadeToSecondaryColor = pow(sin(uTime * -1.56 + normalizedDistanceFromCenter * 3.141), 6);
    
    // This fades out at the ends of the sprite.
    if (normalizedDistanceFromCenter > 0.7)
        fadeToSecondaryColor *= saturate(1 - (normalizedDistanceFromCenter - 0.7) / 0.2);
    float4 endFadeColor = lerp(float4(uColor, 1), float4(uSecondaryColor, 1), 0.5);
    return (lerp(color, endFadeColor, 0.3 + fadeToSecondaryColor * 0.6) * color.a) * sampleColor * (1 + brightness);
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}