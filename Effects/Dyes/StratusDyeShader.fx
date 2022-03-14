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

static float xAdditiveMax = 0.035;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float sineTime = sin(uTime); // Saved so that I don't have the compute this multiple times. Shaders have a limited number of mathematical instructions you can use - 64.
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w; // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    
    float2 modifiedCoords = coords;
    modifiedCoords.x += cos(uTime + frameY) * 0.5 + 0.5; // Cause the effect to sway around a bit with time.
    
    float4 noiseColor = tex2D(uImage1, frac(modifiedCoords));
    
    float xMultiplier = cos(coords.x * 2.3 + uTime * 1.15) + 0.5;
    float yMultiplier = (sin(frameY * 2 + uTime * 1.4) + 1) * 0.5;

    color.rgb *= lerp(uColor, uSecondaryColor, saturate(xMultiplier * yMultiplier * 1.6));
    if (noiseColor.r > 0.55 + sineTime * 0.025)
    {
        color.rgb *= 1.6; // Sometimes give light "patches" depending on the swaying noise image.
    }
    color.rgb *= 1.6 + (sineTime * 0.5 + 1.5) * ((frameY + coords.x) - 1) * 2; // Brighten the shader over time.
    return color * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}