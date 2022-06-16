float Resolution; //Pixel resolution

float time;
float2 pingCenter; //The origin of the ping wave.
float pingRadius; //The max radius of the ping.
float pingWaveThickness; //The thickness of the wave before it entirely fades out.
float pingProgress; //How far along the ping progressed
float pingTravelTime; //Percent of the ping's duration during which the ping expands to reach its full radius.
float pingFadePoint; //Percent of the ping's duration at which to start fading away.
float edgeBlendStrength; //How starkly should the ping's edge fade off.
float edgeBlendOutLenght; //Small lenght at the edge of the wave to blend away smoothly.

float4 baseTintColor; //Color of the tile's overlay
float4 tileEdgeColor; //Color of the tile's edge effects
float4 scanlineColor; //Color of the scanline effects

float4 waveColor; //Color of the ping wave

//Per tile stuff
float2 tilePosition; //The position of the top left of the tile
float4 cardinalConnections; //Up, Left, Right, Down connections.
float4 ordinalConnections; //Top left, Top Right, Bottom left, bottom right connections.


texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };


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

float4 main(float2 uv : TEXCOORD) : COLOR
{
    //Pixelate
    uv.x -= uv.x % (1 / Resolution);
    uv.y -= uv.y % (1 / Resolution);
    
    float distanceFromPingOrigin = length(pingCenter - (tilePosition + uv * 16));
    float waveExpansionPercent = pingProgress / pingTravelTime;
    float realPingRadius = pingRadius * waveExpansionPercent;
    
    //Crop the ping wave into a circle
    if (distanceFromPingOrigin > realPingRadius)
        return float4(0, 0, 0, 0);
    
    float waveEdgeDistanceFromCenter = max(realPingRadius - pingWaveThickness, 0);
    float edgeBlendFactor = pow(max((distanceFromPingOrigin - waveEdgeDistanceFromCenter), 0) / pingWaveThickness, edgeBlendStrength);
    
    float3 ColorTotal = lerp(waveColor.rbg, baseTintColor.rbg, edgeBlendFactor);
    float OpacityTotal = lerp(waveColor.a, baseTintColor.a, edgeBlendFactor);
        
    //Fade away the border
    if (realPingRadius - distanceFromPingOrigin < edgeBlendOutLenght)
    {
       ColorTotal = waveColor.rbg;
       OpacityTotal = waveColor.a * (realPingRadius - distanceFromPingOrigin) / edgeBlendOutLenght;
    }
    
    
    //General fade out at the end.
    OpacityTotal *= max(pingProgress - pingFadePoint, 0) / (1 - pingFadePoint);
    
    ColorTotal = ColorTotal * OpacityTotal;
    return float4(ColorTotal, OpacityTotal);
}

technique Technique1
{
    pass TilePingPass
    {
        PixelShader = compile ps_3_0 main();
    }
}