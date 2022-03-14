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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float4 color = tex2D(uImage0, coords);
    
    float3 cosmiliteMetalColor = float3(122 / 255.0, 127 / 255.0, 155 / 255.0);
    float4 noiseColor = tex2D(uImage1, float2(coords.x, frac(frameY + uTime * 1.51)) * 0.12);
    float4 noiseColor2 = tex2D(uImage1, float2(coords.x, frac(frameY + uTime * 1.51 - 0.458)) * 0.12);
    float opacity = color.a * sampleColor.a;
    float3 baseColor = lerp(uColor, cosmiliteMetalColor, 0.3) * (1 - pow(noiseColor.r, 3) * 0.5);
    baseColor = lerp(color.rgb, baseColor, 0.67);
    baseColor *= 1 + pow(baseColor, 5).r * 8 - noiseColor2.r * 1.25;
    baseColor = lerp(baseColor, uSecondaryColor * 2, pow(noiseColor.r, 2));
    
    return float4(baseColor, 1) * opacity;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}