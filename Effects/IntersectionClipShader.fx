sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity : register(C0);
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float uIntersectionCutoffDirection;
float2 uIntersectionPosition;
float2 uIntersectionNormal;
float2 uSize;
float4 uShaderSpecificData;

float2 RotatedBy(float2 xy, float theta)
{
    return float2(xy.x * sin(theta + 1.57) - xy.y * sin(theta), xy.x * sin(theta) + xy.y * sin(theta + 1.57));
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 worldPos = uWorldPosition + RotatedBy(coords - 0.5, uRotation) * uSize;
    if (sign(dot(uIntersectionNormal, normalize(worldPos - uIntersectionPosition))) == uIntersectionCutoffDirection)
        return 0;
    
    return tex2D(uImage0, coords) * sampleColor;
}

technique Technique1
{
    pass ClipPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}