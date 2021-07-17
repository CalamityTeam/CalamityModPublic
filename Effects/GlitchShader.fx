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

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float horizontalOffset = pow((sin(coords.y * 17.07947 + uTime * 16) + sin(coords.y * 25.13274)) * 0.5, 3) * uOpacity * 0.46;
    float2 samplePosition = float2(saturate(coords.x + horizontalOffset), coords.y);
    return tex2D(uImage0, samplePosition);
}
technique Technique1
{
    pass GlitchPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}