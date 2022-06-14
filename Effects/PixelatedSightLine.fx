float3 color;
float3 darkerColor;
float bloomSize;
float bloomMaxOpacity;
float bloomFadeStrenght;
float mainOpacity;
float laserAngle;
float laserWidth;
float laserLightStrenght;
float noiseOffset;

float2 Resolution;

texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

texture sampleTexture2;
sampler2D NoiseMap = sampler_state { texture = <sampleTexture2>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };


//I can't believe it has gotten down to this.
float realCos(float value)
{
    return sin(value + 1.57079);
}

//Hlsl's % operator applies a modulo but conserves the sign of the dividend, hence the need for a custom modulo
float mod(float a, float n)
{
    return a - floor(a / n) * n;
}

//Gets the distance of a plot from a line with a specified origin and angle
    float distanceFromLine
    (
    float2 origin, float angle, float2 plot)
{
        return abs(realCos(angle) * (origin.y - plot.y) - sin(angle) * (origin.x - plot.x));
    }

//Gets the distance of a plot from a line with a specified origin and angle, but crops the line so it only expands towards the angle
    float distanceFromLineCropped
    (
    float2 origin, float angle, float2 plot, float plotAngle)
{
    //If the angle between the line's angle and the plot's angle is less than 90° (aka , not BEHIND the half-line), return the distance from the line
    if (abs(mod(angle - plotAngle + 3.1415926, 6.2831853) - 3.1415926) < 1.5707)
            return distanceFromLine(origin, angle, plot);
    
    //If we are behind the line, just give the distance between the start point and the plot
        else
            return length(origin - plot);

    }


float4 main(float2 uv : TEXCOORD) : COLOR
{
    //Pixelate
    uv.x -= uv.x % (1 / (Resolution.x * 2));
    uv.y -= uv.y % (1 / (Resolution.y * 2));
    float2 mappedUv = float2(uv.x - 0.5, (1 - uv.y) - 0.5);
    
    float halfLaserWidth = laserWidth / 2;
    
    //get the length of the doubled distance, so that 0 = at the center of the sprite and 1 = at the very edge of the circle
    float distanceFromCenter = length(mappedUv) * 2;
    
    //Crop the sprite into a circle
    if (distanceFromCenter > 1)
        return float4(0, 0, 0, 0);
    
    //Grabs the angle (only as a positive angle, since it's a mirror image udnerneath.
    float angle = atan2(mappedUv.y, mappedUv.x);
    //Grabs the distance of the point from the edge line.
    float distanceFromLine = distanceFromLineCropped(float2(0, 0), laserAngle, mappedUv, angle);
    
    //If we are further from the line than the bloom's blending lenght, just don't.
    if (distanceFromLine > bloomSize + halfLaserWidth)
        return float4(0, 0, 0, 0);
    
    float4 noise = tex2D(NoiseMap, float2((distanceFromCenter + noiseOffset) % 1, distanceFromLine / (bloomSize + halfLaserWidth)));
    float3 laserColor = lerp(color, darkerColor, noise.r);
    float laserOpacity = (1 - pow(distanceFromCenter, laserLightStrenght)) * mainOpacity;
    
    if (distanceFromLine <= halfLaserWidth)
        return float4(laserColor * laserOpacity, laserOpacity);
    
    
    //The higher this value is, the more we blend with the edge's opacity & color.
    float bloomBlendFactor = pow(1 - (distanceFromLine - halfLaserWidth) / bloomSize, bloomFadeStrenght);
    float3 colour = lerp(float3(0, 0, 0), laserColor, bloomBlendFactor);
    float opacity = lerp(0, laserOpacity, bloomBlendFactor) * mainOpacity * bloomMaxOpacity;
    
    colour = colour * opacity;
    return float4(colour, opacity);
}

technique Technique1
{
    pass SightLinePass
    {
        PixelShader = compile ps_3_0 main();
    }
}