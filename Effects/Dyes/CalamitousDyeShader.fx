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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w; // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float4 noiseColor = tex2D(uImage1, float2(coords.x, frameY));
    float4 color = tex2D(uImage0, coords);
    float3 blendColor = lerp(uColor, uSecondaryColor, sqrt(frameY));
    float blendFactor = (cos(uTime * 9 + coords.x * 18) * 0.5 + 0.5) * 0.5;
    
    // Allow the fade to pulse upward based on how far up the pixel is.
    blendFactor += (cos(uTime * -13 - frameY * 7.1)) * 0.5;
    float brightness = blendFactor * 0.35 + noiseColor.r * 0.35;
    
    // Cause the effects to taper off at the bottom of the sprite.
    if (frameY < 0.2)
    {
        brightness *= frameY / 0.2;
        blendFactor *= frameY / 0.2;
    }
    float4 colorBlendMultiplier = lerp(float4(blendColor, 1), float4(1, 1, 1, 1), saturate(pow(blendFactor * 1.5, 2)));
    return (lerp(color, float4(blendColor, 1), blendFactor * 0.5 + 0.2) * color.a) * colorBlendMultiplier * (1 + brightness) * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}