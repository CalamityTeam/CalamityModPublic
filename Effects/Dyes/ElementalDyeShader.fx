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

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w;
    
    float time = (sin(uTime + coords.x * cos(uTime + 3.141 * coords.x) + 1.5707 * frameY) * 0.5 + 0.5) * 6;
    float timeFloored = floor(time);
    
    float3 colors[6] =
    {
        float3(241, 149, 49), // Solar
        float3(88, 201, 169), // Vortex
        float3(102, 200, 249), // Stardust
        float3(226, 76, 239) * 0.7, // Nebula
        float3(102, 200, 249), // Stardust
        float3(88, 201, 169) // Vortex
    };
    
    // Use a multi-lerp to fade between colors and then base them on a trig-based time step.
    color.rgb *= lerp(colors[timeFloored] / 255.0, colors[(timeFloored + 1) % 6] / 255.0, time / 6) * 1.6;
    return color;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}