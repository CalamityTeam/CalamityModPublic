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

float2 RotatedBy(float theta, float x, float y)
{
    return float2(x * cos(theta) + y * sin(theta), x * sin(theta) - y * cos(theta));
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float sineTime = sin(uTime); // Saved so that I don't have the compute this multiple times. Shaders have a limited number of mathematical instructions you can use - 64.
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w; // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    
    float2 modifiedCoords = coords;
    float4 color = tex2D(uImage0, coords);
    
    modifiedCoords = RotatedBy(uTime + 0.4 * sineTime, modifiedCoords.y, modifiedCoords.x);
    float4 noiseColor = tex2D(uImage1, modifiedCoords);
    
    float xMultiplier = cos(coords.x * 2.3 + uTime) + 1;
    float yMultiplier = sin(frameY * 2 + uTime * 1.4) + 1;
    
    float interpolationValue = saturate(xMultiplier * yMultiplier) * 0.7 + 0.3; // Range of 0-1. Depends on the current coordinates and the time.
    
    color.rgb *= 0.2 + pow(noiseColor.rgb, 3); // Squash the color to the upper and lower bounds and add a bit of lightness to it.
    color.rgb *= lerp(uColor, uSecondaryColor, interpolationValue); // Blend with the secondary color.
    color.rgb *= 1.7 + (sineTime * 0.5 + 1.5) * interpolationValue; // Range of 1.7 to 3.7, can cause certain spots to become very bright.
    return color * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}