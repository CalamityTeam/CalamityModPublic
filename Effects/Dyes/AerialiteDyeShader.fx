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

float BlendFunction(float from, float to)
{
    return (to <= 0.5) ? 2.22 * from * to : 1 - 2.22 * (1 - from) * (1 - to);
}
float3 BlendColor(float3 from, float3 to)
{
    return float3(BlendFunction(from.r, to.r), BlendFunction(from.g, to.g), BlendFunction(from.b, to.b));
}
float2 InverseLerp(float2 start, float2 end, float2 x)
{
    return saturate((x - start) / (end - start));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = (coords * uImageSize0 - uSourceRect.xy) / uSourceRect.zw;
    
    // Calculate an updraft noise color. This takes world position into account, allowing movement to influence the shader.
    float updraftXCoord = sin(3.141 * frac(framedCoords.x + uWorldPosition.x * 0.0003));
    float updraftYCoord = sin(3.141 * frac(pow(framedCoords.y, 4) + uTime * 1.21 + uWorldPosition.y * 0.0003));
    float2 updraftCoords = float2(updraftXCoord, updraftYCoord);
    float4 updraftNoiseColor = tex2D(uImage1, updraftCoords * float2(0.075, 0.03));
    
    float4 color = tex2D(uImage0, coords);
    
    // Calculate a cloud color based on noise.
    float4 cloudColor = tex2D(uImage1, float2(framedCoords.x + uTime * 0.11, framedCoords.y));
    cloudColor = pow(cloudColor, 2);
    
    // Calculate the updraft color based on the noise.
    float4 updraftColor = float4(uSecondaryColor, 1) * pow(updraftNoiseColor.r, 3) * 4.7;
    
    // Along with the metal.
    float4 metalColor = lerp(color, float4(BlendColor(color.rgb * 0.6, uColor), 1), 0.7);
    
    // And mix it all together.
    return (color * 0.35 + metalColor * 0.65 + updraftColor * 0.8 + cloudColor * 0.3) * color.a * sampleColor * 0.85;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}