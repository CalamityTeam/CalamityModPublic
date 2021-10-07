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

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    float updraftXCoord = sin(3.141 * frac(coords.x + 0.56));
    float updraftYCoord = sin(3.141 * frac(frameY + uTime * 0.21));
    float2 updraftCoords = float2(updraftXCoord, updraftYCoord);
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(coords.x, frameY) * 0.4);
    
    float4 updraftNoiseColor = tex2D(uImage1, updraftCoords * float2(0.05, 0.06));
    float4 updraftColor = float4(uSecondaryColor, 1) * pow(updraftNoiseColor.r, 4) * 4.7;
    float4 stoneColor = lerp(color, float4(uColor, 1), 0.55) * color.a;
    stoneColor.rgb *= 0.2 + noiseColor.r * 0.65;
    return (stoneColor + updraftColor * 0.4) * color.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
