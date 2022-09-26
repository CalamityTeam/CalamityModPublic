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
    float fadeInterpolant = cos(coords.y * coords.x * 6.283 - uTime * 13.1) * 0.5 + 0.5;
    
    float4 fadeColor = float4(lerp(uColor, uSecondaryColor, fadeInterpolant), 1);
    float4 color = tex2D(uImage0, coords) * fadeColor;
    color *= 1 + fadeInterpolant * 0.5;
    return color;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}