sampler noiseTexture : register(s1);
sampler noiseTexture2 : register(s2);

float time;
float glowPower;
float overallColorStrength;
float2 noiseScale;
float2 edgeFadeoutThreshold;
float3 innerColor;
float3 outerColor;
float3 overallColor;
float3 tipColor;
matrix uWorldViewProjection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
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
    
    // Account for texture distortion artifacts.
    coords.y = (coords.y - 0.5) / input.TextureCoordinates.z + 0.5;
       
    // Fade out at the horizontal and vertical ends of the streak.
    float horizonatalDistanceFromCenter = distance(0.5, coords.y);
    float opacity = smoothstep(0.5, 0.5 - edgeFadeoutThreshold, horizonatalDistanceFromCenter);

    // Calculate the main nosie color and distortion.
    float4 noiseDistortion = tex2D(noiseTexture, coords * float2(4, 2) + float2(time * -0.72, time * 0.51));
    float4 noiseColor = tex2D(noiseTexture, coords * noiseScale + float2(time * -2.42 - noiseDistortion.r * 0.8, time * -1.33 + noiseDistortion.r * 0.8));
    
    // Calculate how much a pixel should glow from the center outwards.
    float glow = pow(0.1 / horizonatalDistanceFromCenter, glowPower);
    
    // Render as white near the tip of the flame jet, then ease into the inner and outer colors of the jet.
    float3 flameBodyColor = lerp(tipColor, lerp(outerColor, innerColor, opacity), InverseLerp(0, 0.092, coords.x));
    float3 finalFlameColor = lerp(flameBodyColor, overallColor, overallColorStrength);
    
    float4 returnColor = (color * float4(finalFlameColor, 1) + noiseColor * glow) * opacity;
    return returnColor;
}

technique Technique1
{
    pass LaserPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
