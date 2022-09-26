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
    VertexShaderOutput output = (VertexShaderOutput) 0;
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
    float bloomOpacity = pow(sin(coords.y * 3.141), 5.6);
    float noise = tex2D(uImage1, coords * 3 - float2(uTime * 2.44, 0));
    float brightnessStreak = tex2D(uImage2, coords * float2(2, 1) - float2(uTime * 1.61, 0)) + noise * bloomOpacity;
    float4 energyColor = float4(lerp(uColor, uSecondaryColor, noise), 1);
    float widthOpacity = pow(sin(coords.y * 3.141), InverseLerp(0.1, 0, coords.x) * 20);
    
    return (energyColor * bloomOpacity + brightnessStreak * bloomOpacity) * color.a * pow(1 - coords.x, 1.6) * widthOpacity;
}

technique Technique1
{
    pass PiercePass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
