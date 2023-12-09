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

//
// Based on Devourer of Gods' death animation (by Dominic)
// in turn, that was based on ExampleMod's cool death animation
//


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;
    
    // Temporal drift to make the sprite slide through the noise texture
    float2 drift = float2(5 * sin(0.04165 * uTime), 1.1 * uTime);

    float2 noiseMapTexCoords = coords + drift;
    float4 noiseColor = tex2D(uImage1, noiseMapTexCoords);

    // Define thresholds for total pixel erasure and glowing lines.
    //
    // Rapidly flickering sinewave produced by Desmos, loosely based on the Weierstrass function
    // (infinitely sharp vague sinewave, periodic, continuous everywhere but differentiable nowhere)
    // https://en.wikipedia.org/wiki/Weierstrass_function
    float flickerOne = 0.05 * cos(7 * uTime);
    float flickerTwo = 0.06 * cos(31 * uTime);
    float flickerThree = 0.04 * sin(167 * uTime);
    float fullErasureThreshold = uOpacity + flickerOne + flickerTwo + flickerThree;
    float glowThreshold = fullErasureThreshold - 0.1;
    
    // If the noise over the erasure threshold, completely erase this pixel.
    if (noiseColor.r > fullErasureThreshold)
    {
        color.rgba = 0;
    }
    
    // Otherwise, if it's over the slightly lower threshold, replace it with a bright color.
    else if (noiseColor.r > glowThreshold)
    {
        // Ensure it accounts for the original alpha.
        color = float4(0.4902, 1, 0, 1) * color.a;
    }

    return color;
}

technique Technique1
{
    pass BlightPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}