sampler diagonalNoise : register(s1);
sampler upwardNoise : register(s2);
sampler upwardPerlinTex : register(s3);

bool day;

float colorMult;
float time;
float radius;
float maxOpacity;
float burnIntensity;

float2 screenPosition;
float2 screenSize;
float2 anchorPoint;
float2 playerPosition;

float InverseLerp(float a, float b, float t)
{
    return saturate((t - a) / (b - a));
}

float3 firePalette(float noise)
{
    // Temperature range (in Kelvin).
    float temperature = 1500. + 1500. * noise;

    float3 darkColor = float3(0.81, 0.45, 0.23) * colorMult;
    float3 midColor = float3(1., 0.75, 0.29) * colorMult;
    float3 brightColor = float3(1., 1., 0.95) * colorMult;
    if (!day) 
    {
        darkColor.rgb = darkColor.bgr;
        midColor.rgb = midColor.bgr;
        brightColor.rgb = brightColor.bgr;
    }
    float3 fireColor;
    if (noise < 0.5)
        fireColor = lerp(darkColor, midColor, noise * 2.);
    else
        fireColor = lerp(midColor, brightColor, (noise - 0.5) * 2.);
    
    fireColor = pow(fireColor, float3(5, 5, 5)) * (exp(1.43876719683e5 / (temperature * fireColor)) - 1.);

    // Exposure level. Set to "50." For "70," change the "5" to a "7," etc.
    return 1. - exp(-1.3e8 / fireColor);
}

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{   
    float2 worldUV = screenPosition + screenSize * uv;
    float2 provUV = anchorPoint / screenSize;
    float worldDistance = distance(worldUV, anchorPoint);
    float adjustedTime = time * 0.1;
    
    // Pixelate the uvs
    float2 pixelatedUV = worldUV / screenSize;
    pixelatedUV.x -= worldUV.x % (1 / screenSize.x);
    pixelatedUV.y -= worldUV.y % (1 / (screenSize.y / 2) * 2);
    
    // Sample the noise textures
    float noiseMesh1 = tex2D(upwardNoise, frac(pixelatedUV * 0.58 + float2(0, time * 0.25))).g;
    float noiseMesh2 = tex2D(upwardPerlinTex, frac(pixelatedUV * 1.57 + float2(0, time * 0.35))).g;
    float noiseMesh3 = tex2D(diagonalNoise, frac(pixelatedUV * 1.46 + float2(adjustedTime * 0.56, adjustedTime * 1.2))).g;
    float noiseMesh4 = tex2D(diagonalNoise, frac(pixelatedUV * 1.57 + float2(adjustedTime * -0.56, adjustedTime * 1.2))).g;
    float textureMesh = noiseMesh1 * 0.125 + noiseMesh2 * 0.2 + noiseMesh3 * 0.35 + noiseMesh4 * 0.35;
    
    // Get the distance to the pixel from the player.
    float distToPlayer = distance(playerPosition, worldUV);
    // And get the correct opacity based on it.
    float opacity = burnIntensity;
    // Fade in quickly as the player approaches the pixels
    opacity += InverseLerp(800, 500, distToPlayer);
    
    // Define the border and mix the inferno for a smooth transition
    bool border = worldDistance < radius && opacity > 0;
    float colorMult = 1;
    if (border) 
        colorMult = InverseLerp(radius * 0.94, radius, worldDistance);
    opacity = clamp(opacity, 0, maxOpacity); 
    // If the color multi has not been changed (not border pixel) and opacity is 0 OR it's within 
    if (colorMult == 1 && (opacity == 0 || worldDistance < radius))
        return sampleColor;
    
    return float4(firePalette(textureMesh), 1) * colorMult * opacity;
}

technique Technique1
{
    pass InfernoPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}