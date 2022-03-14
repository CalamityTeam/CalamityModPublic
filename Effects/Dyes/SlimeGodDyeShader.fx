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
    // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(coords.x, frac(frameY - uTime * 0.27)) * 0.5) * 1.5;
    float verticalFlowMovement = (sin(coords.x * 14.71 + frameY * 13.5 - uTime * 1.6) * 0.5 + 0.5) * noiseColor.r;
    
    // Adjust coords to have an offset to the sprite.
    if (color.a == 0 && frameY < 0.82)
        coords.y = saturate(coords.y + verticalFlowMovement / uSourceRect.w / 17);

    // Prepare the dark outlines.
    float outlineFade = pow(1 - (color.r + color.g + color.b) / 3, 2);
    color = lerp(color, float4(uColor * 0.8, 1), outlineFade * 0.7) * (color.a + outlineFade * color.a * 0.15);
    
    // Prepare the bright in-lines.
    color = lerp(color, float4(uSecondaryColor * 0.85, 1), saturate(1 - outlineFade) * 0.4) * color.a;
    
    // Handle sludge fades based on noise.
    color = lerp(color, float4(uSecondaryColor * 0.375, 1), pow(noiseColor.r, 3) * 0.3 + verticalFlowMovement * 0.3 + 0.13) * color.a;
    
    return color * (1 + (1 - outlineFade) * 0.1) * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}