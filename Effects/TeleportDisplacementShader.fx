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
float2 frameCount;
float4 uShaderSpecificData;

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float frameY = coords.y * frameCount.y;
    
    // 17.07947 is 2pi * e, while 25.13274 is 8pi.
    // When two sinusoids are added in this way and the periods of each sinusoid are irrational,
    // the resulting sinusoid from the addition has no period, meaning that it has pseudo-random
    // bumps and valleys. This is ideal for a glitch effect. The * 0.5 at the end is make the range
    // of the result be -1 to 1 like a typical sine.
    float offsetBase = (sin(frameY * 17.07947 + uTime * 16) + sin(coords.y * 25.13274)) * 0.5;
    
    // Here, a power is used to make the valleys and hills sharper, and bias everything else
    // towards 0, making horizontal stretches more noticable and powerful when they happen.
    // Higher values of uOpacity will result in more powerful offsets, with a uOpacity of 0
    // doing nothing at all.
    float horizontalOffset = pow(offsetBase, 3) * uOpacity / frameCount.x * 0.46;
    float2 samplePosition = float2(saturate(coords.x + horizontalOffset), coords.y);
    return tex2D(uImage0, samplePosition) * float4(uSecondaryColor, uSaturation);
}
technique Technique1
{
    pass GlitchPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}