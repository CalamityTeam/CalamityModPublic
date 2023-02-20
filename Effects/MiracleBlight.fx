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

// Cosine does not work in shaders in Terraria 1.4. It returns incredibly corrupted values and nobody is sure why.
// This reimplements cosine using trigonometric identities.
//
// Discovery of malfunctioning cosine attributed to ScalarVector from SLR:
// https://discord.com/channels/103110554649894912/445276626352209920/979928448300634175
float realCos(float value)
{
    return sin(value + 1.57079);
}

//
// Based on Devourer of Gods' death animation (by Dominic)
// in turn, that was based on ExampleMod's cool death animation
//


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
        return color;

    // Framed coordinates -- a UV of the NPC's current framed sprite, instead of the whole sheet
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;

    // Temporal drift to make the sprite slide through the noise texture
    float2 drift = float2(5 * sin(8.33 * uTime), 220 * uTime);

    float2 noiseMapTexCoords = framedCoords * uSourceRect.zw + drift;
    float4 noiseColor = tex2D(uImage1, noiseMapTexCoords / uImageSize1);

    // Define thresholds for total pixel erasure and glowing lines.
    //
    // Rapidly flickering sinewave produced by Desmos, loosely based on the Weierstrass function
    // (infinitely sharp vague sinewave, periodic, continuous everywhere but differentiable nowhere)
    // https://en.wikipedia.org/wiki/Weierstrass_function
    float flickerOne = 0.05 * realCos(7 * uTime);
    float flickerTwo = 0.06 * realCos(31 * uTime);
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
        color = float4(0.4902, 1, 0, 1);
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