sampler vortexNoiseTexture : register(s0);
sampler vortexNoiseTexture2 : register(s1);
sampler distortionTexture : register(s2);

float time;
float colorPaletteLimit;
float spiralArms;
float spiralAdditionalAngle;
float minPixelFadeDistance;
float maxPixelFadeDistance;

float2 pixelatonFactor;
float2 spiralTimeOffset;
float2 pixelationFactor;
float3 vortexDarkColor;
float3 vortexBrightColor;

float inverseLerp(float from, float to, float x)
{
    return saturate((x - from) / (to - from));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Pixelate.
    coords = round(coords * pixelationFactor) / pixelationFactor;
    
    float2 centeredCoords = coords - 0.5;
    float polarAngle = atan2(centeredCoords.y, centeredCoords.x) / 6.283;
    float distanceToCenter = length(centeredCoords);
    float2 polarCoords = float2(polarAngle, distanceToCenter);
   
    // Define coordinates for creating the vortex effect.
    float vortexRadius = pow(distanceToCenter, 0.6);
    float vortexAngle = spiralArms * atan2(centeredCoords.y, centeredCoords.x) / 3.141 + spiralAdditionalAngle * vortexRadius;
    float2 vortexPolarCoords = float2(vortexAngle, vortexRadius) * 0.5 + float2(time * spiralTimeOffset.x, time * spiralTimeOffset.y);
    
    // Add some distortion over it.
    float distortion1 = tex2D(distortionTexture, coords * 0.46 + float2(time * -0.02, time * 0.022)).r;
    float distortion2 = tex2D(distortionTexture, coords * 0.32 + float2(time * 0.018, time * -0.03)).r;
    float totalDistortion = (distortion1 * 0.5) + (distortion2 * 0.5);

    float4 vortexNoiseColor1 = tex2D(vortexNoiseTexture, vortexPolarCoords + totalDistortion);
    float4 vortexNoiseColor2 = tex2D(vortexNoiseTexture2, polarCoords + float2(time * -0.02, time * -0.092) + totalDistortion * 0.64);
    float4 vortexNoiseColor = lerp(vortexNoiseColor1, vortexNoiseColor2, inverseLerp(0, maxPixelFadeDistance - 0.075, distanceToCenter));
    
    // Apply coloration based on pixel brightness.
    float brightness = (vortexNoiseColor.r + vortexNoiseColor.g + vortexNoiseColor.b) / 3.0;
    vortexNoiseColor.rgb *= lerp(vortexDarkColor, vortexBrightColor, brightness);

    // Quantization.
    vortexNoiseColor = round(vortexNoiseColor * colorPaletteLimit) / colorPaletteLimit;

    // Fade out at the edge of the texture.
    float edgeFade = inverseLerp(0, minPixelFadeDistance - 0.016, distanceToCenter) * inverseLerp(maxPixelFadeDistance, minPixelFadeDistance, distanceToCenter);
    // float edgeOffset = tex2D(edgeFadeTexture, vortexPolarCoords + float2(time * spiralTimeOffset.x, time * spiralTimeOffset.y)).r * 0.13;
    // float edgeFade = smoothstep(0.5, opacityCutoffValue, polarCoords.y + edgeOffset);
       
    return (vortexNoiseColor * edgeFade) * sampleColor.a;
}

technique Technique1
{
    pass ScaryAuraPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}