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
    float frameY = (coords.y * uImageSize0.y - uSourceRect.y) / uSourceRect.w; // Gets a 0-1 representation of the y position on a given frame, with 0 being the top, and 1 being the bottom.
    float4 noiseColor = tex2D(uImage1, float2(frac(coords.x + uTime * 0.1), frameY));
    float4 color = tex2D(uImage0, coords);
    
    // Cause sharp brightness increases based on a perlin noise map.
    // This causes icy, crystaline structures to appear.
    float brightness = pow((noiseColor.r + noiseColor.g + noiseColor.b) / 3, 3) * 3;
    float normalizedDistanceFromCenter = distance(float2(coords.x, frameY), float2(0.5, 0.5)) * 2;
    
    // Cause "ring" fades based on distance.
    float fadeToSecondaryColor = pow(sin(uTime * -1.56 + normalizedDistanceFromCenter * 3.141), 6);
    
    // This fades out at the ends of the sprite.
    if (normalizedDistanceFromCenter > 0.7)
        fadeToSecondaryColor *= saturate(1 - (normalizedDistanceFromCenter - 0.7) / 0.2);
    float4 endFadeColor = lerp(float4(uColor, 1), float4(uSecondaryColor, 1), 0.5);
    return (lerp(color, endFadeColor, 0.3 + fadeToSecondaryColor * 0.6) * color.a) * (1 + brightness) * sampleColor.a;
}
technique Technique1
{
    pass DyePass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}