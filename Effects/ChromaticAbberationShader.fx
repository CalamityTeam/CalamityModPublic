sampler baseTexture : register(s0);

bool useCustomColors;
float abberationStrength;
float2 impactPosition;
float3 primaryAbberationColor;
float3 secondaryAbberationColor;
float3 tertiaryAbberationColor;

float3 ChromaticAbberation(sampler2D samplerTexture, float2 coords)
{
    // Calculate the distortion level of each color.
    float separation = abberationStrength / distance(coords, impactPosition);
    float rColor = tex2D(samplerTexture, coords + float2(-1, -1) * separation).r;
    float gColor = tex2D(samplerTexture, coords + float2(1, -1) * separation).g;
    float bColor = tex2D(samplerTexture, coords + float2(0, 1) * separation).b;
    
    // Return the three calculations as one singular color.
    float3 returnColor = float3(rColor, gColor, bColor);
    return returnColor;
}

float3 ChromaticAbberation_CustomColors(sampler2D samplerTexture, float2 coords)
{
    // Calculate the distortion level of each color.
    float separation = abberationStrength / distance(coords, impactPosition);
    float4 pColor = tex2D(samplerTexture, coords + float2(-1, -1) * separation) * float4(primaryAbberationColor, 1);
    float4 sColor = tex2D(samplerTexture, coords + float2(1, -1) * separation) * float4(primaryAbberationColor, 1);
    float4 tColor = tex2D(samplerTexture, coords + float2(0, 1) * separation) * float4(primaryAbberationColor, 1);
    
    // Return the three calculations as one singular color.
    float3 returnColor = saturate(pColor + sColor + tColor);
    return returnColor;
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float3 color = useCustomColors ? ChromaticAbberation_CustomColors(baseTexture, coords) : ChromaticAbberation(baseTexture, coords);
    return sampleColor * float4(color, 1.0);
}

technique Technique1
{
    pass ChormaAbberationPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}