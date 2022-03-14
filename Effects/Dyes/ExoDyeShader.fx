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
float3 specialPalette[9] =
{
    float3(250, 255, 112) / 255.0,
    float3(211, 235, 108) / 255.0,
    float3(166, 240, 105) / 255.0,
    float3(105, 240, 220) / 255.0,
    float3(64, 130, 145) / 255.0,
    float3(145, 96, 145) / 255.0,
    float3(242, 112, 73) / 255.0,
    float3(199, 62, 62) / 255.0,
    float3(250, 255, 112) / 255.0,
};

float InverseLerp(float from, float to, float x)
{
    return saturate((x - from) / (to - from));
}

// Multi-color lerp which uses the palette.
float3 PaletteLerp(float interpolant)
{
    float startingIndex = floor(interpolant * 8);
    float squashedInterpolant = frac(interpolant * 8);
    return lerp(specialPalette[startingIndex], specialPalette[startingIndex + 1], squashedInterpolant);
}

// (Co)sines use a lot of instructions. As such, a more linear wave constructed by moduli is used instead.
// Is this kind of ridiculous? Probably. But there's basically nothing I can do until 1.4 rolls around
// and the pathetically low instruction count cap is increased significantly. -Dominic
float TriangleWave(float x)
{
    if (x % 2 < 1)
        return x % 2;
    return -(x % 2) + 2;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w; // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float4 noiseColor = tex2D(uImage1, float2(coords.x, frameY - uTime * 0.45) * 0.1);
    float4 color = tex2D(uImage0, coords);
    float3 blendColor = PaletteLerp(frac(frameY + uTime * 1.3));
    float blendFactor = (TriangleWave(uTime * 9 + coords.x * 2 - frameY * 2.1) * 0.5 + 0.5) * 0.5;
    float brightness = (blendFactor * 0.4 + noiseColor.r * 0.9) * 1.2;
    
    float4 colorBlendMultiplier = lerp(float4(blendColor, 1), float4(1, 1, 1, 1), saturate(blendFactor * 1.5));
    return (lerp(color, float4(blendColor, 1), blendFactor * 0.5 + 0.2) * color.a) * colorBlendMultiplier * (1 + brightness) * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}