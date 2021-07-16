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
    float horizontalOffset = pow((sin(2.7182818 * 6.283 * coords.y + 16 * uTime) + sin(1.570796 * 16 * coords.y)) * 0.5, 3) * uOpacity * 0.46;
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