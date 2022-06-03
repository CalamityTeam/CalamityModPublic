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
float2 uTargetPosition;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    
    // Calculate noise for red flames.
    float flameXCoord = sin(3.141 * frac(framedCoords.x + 0.56));
    float flameYCoord = sin(3.141 * frac(framedCoords.y + uTime * 0.21));
    float2 flameCoords = float2(flameXCoord, flameYCoord);
    float4 noiseColor = tex2D(uImage1, float2(framedCoords.x, framedCoords.y) * 0.4);
    
    float4 color = tex2D(uImage0, coords);
    
    // Calculate the flame noise based on zoomed in noise coordinates.
    float4 flameNoiseColor = tex2D(uImage1, flameCoords * float2(0.05, 0.06));
    float4 flameColor = float4(uSecondaryColor, 1) * pow(flameNoiseColor.r, 4) * 4.7;
    
    // Calculate the astal stone color by approaching the first inputted color from the pixel color with linear interpolation.
    float4 stoneColor = lerp(color, float4(uColor, 1), 0.55) * color.a;
    
    // Become notably darker to reinforce the stone aesthetic.
    stoneColor.rgb *= 0.2 + noiseColor.r * 0.65;
    return (stoneColor + flameColor * 0.4) * sampleColor * color.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
