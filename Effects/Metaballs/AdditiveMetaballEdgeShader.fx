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
float4 uShaderSpecificData;

float2 screenArea;
float2 layerOffset;
float2 singleFrameScreenOffset;

float4 PixelShaderFunction(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float2 originalCoords = coords;
    coords += layerOffset + singleFrameScreenOffset;
    
    float2 offset = 3 / screenArea;
    float4 color = tex2D(uImage0, coords);
    float4 leftColor = tex2D(uImage0, coords + float2(-offset.x, 0));
    float4 rightColor = tex2D(uImage0, coords + float2(-offset.x, 0));
    float4 topColor = tex2D(uImage0, coords + float2(0, -offset.y));
    float4 bottomColor = tex2D(uImage0, coords + float2(0, offset.y));
    float4 avergeColor = (color + leftColor + rightColor + topColor + bottomColor) / 4.7;
    float lowestColorValue = min(avergeColor.r, avergeColor.g);
    lowestColorValue = min(lowestColorValue, avergeColor.b);
    avergeColor = lerp(avergeColor, float4(1, 1, 1, 1), pow(lowestColorValue, 0.5)) * avergeColor.a;
    return avergeColor;
}
technique Technique1
{
    pass ParticlePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}