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
    
    // Define a baseline gold color. This will be used when creating a reflective metal texture.
    float3 goldColor = float3(228 / 255.0, 175 / 255.0, 72 / 255.0);
    
    // Calculate noise with a zoomed in Y texture coordinate.
    float4 noiseColor = tex2D(uImage1, float2(framedCoords.x, sin(3.141 * frac(framedCoords.y + frac(uTime * 0.51)))) * 0.067);
    float4 color = tex2D(uImage0, coords);
    
    // Calculate a degree of shine depending on the noise. This is greatly expanded depending on the result.
    float shineFactor = pow(noiseColor.r, 0.8);
    color *= lerp(0.7, 5, shineFactor);
    float3 metalColor = lerp(uColor, uSecondaryColor, shineFactor);
    
    // Fade to the metal color with linear interpolation.
    color.rgb = lerp(color.rgb, metalColor, lerp(0.35, 0.8, shineFactor));
    
    // Use a specialized blend method to create the shine effect.
    color.rgb = (length(color.rgb) <= 0.5) ? 2 * color.rgb * goldColor : 1 - 2 * (1 - color.rgb) * (1 - goldColor);
    return (color + noiseColor * 0.45) * sampleColor * color.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}