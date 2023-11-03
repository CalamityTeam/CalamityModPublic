sampler metaballContents : register(s0);
sampler overlayTexture : register(s1);

float2 screenSize;
float2 layerSize;
float2 layerOffset;
float4 edgeColor;
float2 singleFrameScreenOffset;

// The usage of these two methods seemingly prevents imprecision problems for some reason.
float2 convertToScreenCoords(float2 coords)
{
    return coords * screenSize;
}

float2 convertFromScreenCoords(float2 coords)
{
    return coords / screenSize;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate the base color. This is the calculated from the raw objects in the metaball render target.
    float4 baseColor = tex2D(metaballContents, coords);
    
    // Used to negate the need for an inverted if (baseColor.a > 0) by ensuring all of the edge checks fail.
    float alphaOffset = (1 - any(baseColor.a));
    
    // Check if there are any empty pixels nearby. If there are, that means this pixel is at an edge, and should be colored accordingly.
    float left = tex2D(metaballContents, convertFromScreenCoords(convertToScreenCoords(coords) + float2(-2, 0))).a + alphaOffset;
    float right = tex2D(metaballContents, convertFromScreenCoords(convertToScreenCoords(coords) + float2(2, 0))).a + alphaOffset;
    float top = tex2D(metaballContents, convertFromScreenCoords(convertToScreenCoords(coords) + float2(0, -2))).a + alphaOffset;
    float bottom = tex2D(metaballContents, convertFromScreenCoords(convertToScreenCoords(coords) + float2(0, 2))).a + alphaOffset;
    
    // Use step instead of branching in order to determine whether neighboring pixels are invisible.
    float leftHasNoAlpha = step(left, 0);
    float rightHasNoAlpha = step(right, 0);
    float topHasNoAlpha = step(top, 0);
    float bottomHasNoAlpha = step(bottom, 0);
    
    // Use addition instead of the OR boolean operator to get a 0-1 value for whether an edge is invisible.
    // The equivalent for AND would be multiplication.
    float conditionOpacityFactor = 1 - saturate(leftHasNoAlpha + rightHasNoAlpha + topHasNoAlpha + bottomHasNoAlpha);
    
    // Calculate layer colors.
    float4 layerColor = tex2D(overlayTexture, (coords + layerOffset + singleFrameScreenOffset) * screenSize / layerSize);
    float4 defaultColor = layerColor * tex2D(metaballContents, coords) * sampleColor;
    
    // Lastly, use the aforementioned condition to switch between the colors. If conditionOpacityFactor is 1, the default color is
    // zeroed out and the edge is toggled, and vice versa.
    return (defaultColor * conditionOpacityFactor) + (edgeColor * sampleColor * (1 - conditionOpacityFactor));
}
technique Technique1
{
    pass ParticlePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}