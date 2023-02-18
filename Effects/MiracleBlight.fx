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

    // Framed coordinates -- a UV of the NPC's current framed sprite, instead of the whole sheet
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;

    // Temporal drift to make the sprite slide through the noise texture
    float2 drift = float2(5 * sin(8.33 * uTime), 40 * uTime);

    float2 noiseMapTexCoords = framedCoords * uSourceRect.zw + drift;

    float4 noiseColor = tex2D(uImage1, noiseMapTexCoords / uImageSize1);
    return noiseColor;

    /*
    float noiseRedErasureThreshold = fadeThreshold + 0.08;

    // If the noise over the slightly higher threshold, completely erase this pixel.
    if (perlin.r > noiseRedErasureThreshold)
    {
        color.rgba = 0;
    }
    // Otherwise, if the noise is over the original threshold, replace this pixel with glowing energy lines.
    else if (perlin.r > fadeThreshold)
    {
        color = float4(125.0 / 255, 1, 0, 1);
    }
    // Otherwise do nothing.
    return color;
    */
}

technique Technique1
{
    pass BlightPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}