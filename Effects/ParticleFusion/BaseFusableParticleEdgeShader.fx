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
float edgeBorderSize;
float2 screenArea;
float2 renderTargetArea;
float3 borderColor;

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 backgroundColor = tex2D(uImage1, frac(coords * 35 + uWorldPosition));
    float positionNoiseInterpolant = sin(2.71828182846 * uWorldPosition.x / 25) + sin(1.57079632679 * uWorldPosition.x / 25);
    positionNoiseInterpolant = positionNoiseInterpolant * 0.5 + 0.5;
    float edgeBorderSizeAdjusted = edgeBorderSize * lerp(0.64, 1.27, positionNoiseInterpolant);
    
    // Equivalent to edgeBorderSize pixels in both the X and Y direction.
    float2 edgeOffset = float2(edgeBorderSizeAdjusted, edgeBorderSizeAdjusted) / renderTargetArea;
    
    // Check if the cardinal directions are not intersecting with any other parts of the render target.
    // If one of them isn't, that means that a border should be rendered.
    float4 aboveColor = tex2D(uImage0, coords + float2(0, edgeOffset.y));
    float4 belowColor = tex2D(uImage0, coords - float2(0, edgeOffset.y));
    float4 leftColor = tex2D(uImage0, coords + float2(edgeOffset.x, 0));
    float4 rightColor = tex2D(uImage0, coords - float2(edgeOffset.x, 0));
    if (aboveColor.r < 0.5 || belowColor.r < 0.5 || leftColor.r < 0.5 || rightColor.r < 0.5)
        return color;
    
    return backgroundColor;
}
technique Technique1
{
    pass ParticlePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}