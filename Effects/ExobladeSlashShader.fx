sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
float3 uColor;
float3 uSecondaryColor;
float3 fireColor;
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
float2 uImageSize2;
matrix uWorldViewProjection;
float4 uShaderSpecificData;
bool flipped;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    float4 pos = mul(input.Position, uWorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float InverseLerp(float from, float to, float x)
{
    return saturate((x - from) / (to - from));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    float timeDirection = -1;
    if (flipped)
    {
        timeDirection = 1;
        coords.y = 1 - coords.y;
    }
    
    float2 noiseCoords = coords * float2(0.5, 3) - float2(uTime * timeDirection * 0.45, 0);
    noiseCoords.x = sin(noiseCoords.x * 5.4) * 0.5 + 0.5;
    float noise = tex2D(uImage1, noiseCoords).r;
    float noise2 = pow(tex2D(uImage1, noiseCoords * 2.2).r, 1.6);
    float noise3 = pow(tex2D(uImage1, noiseCoords * 1.1).r, 1.3);
    float opacity = noise * pow(saturate((1 - coords.x) - noise * coords.y * 0.54), 3);
    
    // Fade to the second primary color.
    color = lerp(color, float4(uColor, 1), noise2);
    
    // Create dark colors.
    float darkColorWeight = saturate(coords.y * 1.8 + coords.x * 0.45 + noise * 0.1);
    color = lerp(color, float4(uSecondaryColor, 1), darkColorWeight);

    // Create a fire streak.
    float fireColorWeight = InverseLerp(0.3, 0, coords.y) * pow(1 - coords.x, 0.5);
    color = lerp(color, float4(fireColor, 1), fireColorWeight);
    
    float4 noiseColor = color * opacity * (noise3 * 2.4 + 2.4);
    noiseColor.a = lerp(noiseColor.a, 0, 1 - fireColorWeight);
    return noiseColor;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
