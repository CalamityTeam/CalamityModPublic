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
float coordinateZoomFactor;
bool useStaticLine;
bool useTrueNoise;

float Random(float2 coords)
{
    return abs(frac(sin(dot(coords, float2(17.8342, 74.8819))) * 53648));
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate a noise value that very, very rapidly descends downward.
    // So fast that it gives the appearance of TV static. This as an overall effect on the entire texture.
    float noise = Random(coords * coordinateZoomFactor * 0.3 + float2(0, frac(uTime * -97.3)));
        
    // Calculate a base noise value that makes a bright line scroll down the screen. This is achieved by using a strongly squished sinusoid that scrolls downward and
    // taking its resulting value as a 0-1 interpolant.
    float brightLineInterpolant = useStaticLine ? pow(sin(uTime * 3.1 - coords.y * 5.4) * 0.5 + 0.5, 4.96) : 0;
    
    // After the initial bright line value is calculated, incorporate the X axis to weaker extent. This allows the line to have bends and look less unnaturally simple.
    brightLineInterpolant += pow(sin(uTime * 1.9 - coords.x * 2.4) * 0.5 + 0.5, 2.51) * 0.55;
    
    // Lastly, apply the noise from above, and clamp the entire interpolant between 0-1 before passing it in as a color multiplier.
    brightLineInterpolant += noise * (useStaticLine ? 0.16 : 0.91);
    
    return float4(uColor, 1) * lerp(1, 6, saturate(brightLineInterpolant)) * sampleColor * tex2D(uImage0, coords).a * 0.2;
}
technique Technique1
{
    pass GlitchPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}