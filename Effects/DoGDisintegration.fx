sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity : register(C0);
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

// This shader is largely based on code used for ExampleMod's cool death animation thing; it is suprisingly useful for this use case.
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    
    float fade = pow(saturate(1 - uOpacity), 0.7);
    float4 color1 = tex2D(uImage1, float2(coords.x, frac(coords.y + uSaturation * 0.11)) * 0.3);

    float readRed = fade * 1.05;

    if (color1.r > readRed)
    {
        color.rgba = 0;
    }
    else if (color1.b > fade)
    {
        color = float4(1, 105.0 / 255, 180.0 / 255, 1);
    }
    return color;
}

technique Technique1
{
    pass DisintegrationPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}