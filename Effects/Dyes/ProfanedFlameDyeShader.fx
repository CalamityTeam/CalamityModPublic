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
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w; // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float4 color = tex2D(uImage0, coords);
    
    // Read the fade map as a streak.
    float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.x * 0.18 + cos(uTime * 0.9) * 0.021), frac(frameY * 0.32 + uTime * 0.2)));
    fadeMapColor.r *= pow(coords.x, 0.2);
    
    float opacity = lerp(1.05, 1.95, fadeMapColor.r);
    opacity *= fadeMapColor.r + 1;
    opacity *= lerp(0.4, 1.1, fadeMapColor.r);
    
    float3 transformColor = lerp(uColor, uSecondaryColor, fadeMapColor.r);
    float orangeFade = 1 - saturate((frameY - 0.08) / 0.74);
    opacity += (1 - orangeFade) * 0.6;
    
    transformColor = lerp(transformColor, float3(206 / 255, 116 / 255, 59 / 255), orangeFade);
    color.rgb = lerp(color.rgb, transformColor, fadeMapColor.r);
    opacity *= color.a;
    return color * opacity * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}