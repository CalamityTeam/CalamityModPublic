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

float4 Recolor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    
    float4 color = tex2D(uImage0, coords);
    float originalAlpha = color.a;
    color.rbg = lerp(color.rbg, uColor, uOpacity);
    return color * originalAlpha;
    
}

technique Technique1
{
    pass TintPass
    {
        PixelShader = compile ps_2_0 Recolor();
    }
}