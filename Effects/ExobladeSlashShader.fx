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
    
    if (flipped)
        coords.y = 1 - coords.y;
    
    // Calculate the base noise sampling coordinates. They are firstly squished, with the X coordinate factor acting as a metric of the "sameness" of the trail patterns.
    // The lower the X coordinate factor is, the more detailed the trail is, while the inverse is true for the Y coordinate. After this, a time pass is incorporated to create the effect
    // that the slash is moving against the starting point, as one might expect a slash to do.
    float2 noiseDetail = float2(2, 2);
    float2 noiseCoords = coords * float2(1.0 / noiseDetail.x, noiseDetail.y) - float2(uTime * 0.45, 0);
    
    // To prevent weird, sudden cutoffs unless the texture repeats, the X coordinate (which is in terms of the completion ratio of the trail) in transformed with a 0-1 sine instead of a
    // X Mod 1 cuttoff, which would essentially "cut off" the texture and return the start, turning a value of 1.1 to 0.1, 1.32 to 0.32, etc.
    noiseCoords.x = sin(noiseCoords.x * 5.4) * 0.5 + 0.5;
    
    // Sample noise colors based on the original base noise coordinates. These all vary somewhat to create more varied detail.
    float noise = tex2D(uImage1, noiseCoords).r;
    float noise2 = pow(tex2D(uImage1, noiseCoords * 2.2).r, 1.6);
    float noise3 = pow(tex2D(uImage1, noiseCoords * 1.1).r, 1.3);
    
    // Combine all the noise values together. The base noise serves as a universal multiplier while the others are fed into a squishing function.
    // Importantly, the squishing function ensures that its inputs range from 0-1, such that power doesn't result in values beyond 1.
    // This is also where the "white hot edge" and trail fade effect comes from, due to the incorporation of the X and Y coordinate. In this context, the X coordinate exists to
    // create lower opacities (which then are squished to make the trail look like it's vanishing) the further along the trail the point is. The Y coordinate, meanwhile, is multiplied
    // with the noise and subtracted such that values close to 1 (the bottom of the trail) are given less weight and recieve less brightness. The inverse is true, with values closer to 0
    // recieving more brightness.
    float opacity = noise * pow(saturate((1 - coords.x) - noise * coords.y * 0.54), 3);
    
    // Fade to the second primary color based on one of the noise values.
    color = lerp(color, float4(uColor, 1), noise2);
    
    // Create dark colors. Points that are further along the trail are incentized to fade to the dark color more strongly. This also holds true to points that are closer to the bottom of
    // the trail. To create some variance on top of this, the primary noise value is added as well.
    float darkColorWeight = saturate(coords.y * 1.8 + coords.x * 0.45 + noise * 0.1);
    color = lerp(color, float4(uSecondaryColor, 1), darkColorWeight);

    // Create a fire streak. This only applies to points in the top 30% of the trail, and the effect is stronger the closer a point is to the top. After this, a multiplier is incorporated
    // to make the effect dissipate the further along the trail a point is.
    float fireColorWeight = InverseLerp(0.3, 0, coords.y) * pow(1 - coords.x, 1.56);
    color = lerp(color, float4(fireColor, 1), fireColorWeight);
    
    // Calculate the final color.
    float4 noiseColor = color * opacity * (noise3 * 2.4 + 2.4);
    noiseColor.a = lerp(noiseColor.a, 0, 1 - fireColorWeight);
    return noiseColor * input.Color.a;
}

technique Technique1
{
    pass TrailPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
