float minRadius;
float maxRadius;
float maxOpacity;

float2 screenPosition;
float2 screenSize;
float2 anchorPoint;

float InverseLerp(float a, float b, float t)
{
    return saturate((t - a) / (b - a));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{   
    float2 worldUV = screenPosition + screenSize * uv;
    float2 deerclopsUV = anchorPoint / screenSize;
    float worldDistance = distance(worldUV, anchorPoint);
    
    // Get the distance to the pixel from Deerclops
    float distToDeerclops = distance(anchorPoint, worldUV);
    // And get the correct opacity based on it.
    float opacity = InverseLerp(0, maxRadius, distToDeerclops);
    
    // Define the border and mix the shadow and light for a smoother transition
    bool border = worldDistance < minRadius && opacity > 0;
    float colorMult = 1;

    if (worldDistance > minRadius && worldDistance < maxRadius) 
        opacity = lerp(opacity, maxOpacity, InverseLerp(minRadius, maxRadius, distToDeerclops));
    
    opacity = clamp(opacity, 0, maxOpacity); 
    
    if (border)
    {
        colorMult = InverseLerp(minRadius * 0.94, minRadius, worldDistance);
    }
    
    // If the color multi has not been changed (not border pixel) and opacity is 0 OR it's within 
    if (colorMult == 1 && (opacity == 0 || worldDistance < minRadius))
        return sampleColor;
    
    return float4(0, 0, 0, 1) * colorMult * opacity;
}

technique Technique1
{
    pass ShadowPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}