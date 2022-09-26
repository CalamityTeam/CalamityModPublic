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
bool borderShouldBeSolid;
float3 edgeBorderColor;
float2 screenArea;
float2 renderTargetArea;
float2 screenMoveOffset;
float2 generalBackgroundOffset;
bool invertedScreen;
float2 upscaleFactor;
float4 uShaderSpecificData;

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float2 originalCoords = coords;
    if (invertedScreen)
        coords.y = 1 - coords.y;
    
    // Account for screen movements. Not doing this causes the scene to move based on that.
    coords += screenMoveOffset / renderTargetArea;
    
    float4 color = tex2D(uImage0, coords);
    
    // Determine how much downscaling is required to reasonably zoom in on things.
    float downscaleFactor = float2(5, 5) / upscaleFactor / max(renderTargetArea.x, renderTargetArea.y) / 2;
    float2 backgroundCoords = frac((renderTargetArea * (originalCoords + generalBackgroundOffset * upscaleFactor) + uWorldPosition) * downscaleFactor);
    float4 backgroundColor = tex2D(uImage1, backgroundCoords);
    
    // Equivalent to edgeBorderSize pixels in both the X and Y direction.
    float2 edgeOffset = float2(edgeBorderSize, edgeBorderSize) / renderTargetArea;
    
    // Check if the cardinal directions are not intersecting with any other active parts of the render target.
    // If one of them isn't, that means that a border should be rendered.
    float4 aboveColor = tex2D(uImage0, coords + float2(0, edgeOffset.y));
    float4 belowColor = tex2D(uImage0, coords - float2(0, edgeOffset.y));
    float4 leftColor = tex2D(uImage0, coords + float2(edgeOffset.x, 0));
    float4 rightColor = tex2D(uImage0, coords - float2(edgeOffset.x, 0));
    
    // Invert color check thresholds based on whether the border should be solid when drawing.
    float checkThreshold = 0.02;
    if (!borderShouldBeSolid)
        checkThreshold = 0.95;
    
    // If any cardinal color is sufficiently close to transparency choose an edge border color.
    // Opacity multipliers ensure that this does not affect completely invisible areas of the render target.
    if (aboveColor.r < checkThreshold || belowColor.r < checkThreshold || leftColor.r < checkThreshold || rightColor.r < checkThreshold)
    {
        float borderOpacity = color.a;
        
        // Use binary opacity if the border should be solid when drawing.
        if (borderOpacity > 0 && borderShouldBeSolid)
            borderOpacity = 1;
        
        return float4(edgeBorderColor, 1) * borderOpacity;
    }
    
    return backgroundColor;
}
technique Technique1
{
    pass ParticlePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}