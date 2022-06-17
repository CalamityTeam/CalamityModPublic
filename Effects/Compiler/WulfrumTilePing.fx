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
float tileEdgeBlendStrenght; //How hard the border of a tile should get blended away.

float4 baseTintColor; //Color of the tile's overlay
float3 tileEdgeColor; //Color of the tile's edge effects
float4 scanlineColor; //Color of the scanline effects

float4 waveColor; //Color of the ping wave

//Per tile stuff
float2 tilePosition; //The position of the top left of the tile
bool4 cardinalConnections; //Up, Left, Right, Down connections.
bool4 ordinalConnections; //Top left, Top Right, Bottom left, bottom right connections.


texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

bool AND(bool4 value)
{
    return value.x && value.y && value.z && value.w;
}

float inverselerp(float x, float start, float end)
{
    return (x - start) / (end - start);
}

float inverselerp(float2 x, float2 start, float2 end)
{
    return inverselerp(length(x - start), 0, length(end - start));
}

//Checks if a corner should be drawn or excluded.
bool cornerDrawCheck(float2 position)
{
    float oneEight = 1 / Resolution;
    float sevenEights = 7 / Resolution;
    
    //Top left & top right checks.
    if (!(ordinalConnections.x && ordinalConnections.y) && position.y < oneEight && !cardinalConnections.x)
    {
        if ((!ordinalConnections.x && position.x < oneEight && !cardinalConnections.y) || (!ordinalConnections.y && position.x >= sevenEights && !cardinalConnections.z))
            return false;
    }
    
    //Bottom left & bottom right checks.
    if (!(ordinalConnections.z && ordinalConnections.w) && position.y >= sevenEights && !cardinalConnections.w)
    {
        if ((!ordinalConnections.z && position.x < oneEight && !cardinalConnections.y) || (!ordinalConnections.w && position.x >= sevenEights && !cardinalConnections.z))
            return false;
    }
        
    return true;
}

float getOpacityFromEdge(float2 position)
{
    //If fully surrounded, don't draw anything
    if (AND(cardinalConnections) && AND(ordinalConnections))
        return 0;
    
    float oneFourth = 1 / Resolution;
    float twoFourths = 3 / Resolution;
    float threeFourths = 5 / Resolution;
    float one = 7 / Resolution;
    
    //Do the corners
    //if (!AND(ordinalConnections) && false)
    //{
        
    //}
    
    if (!AND(cardinalConnections))
    {
        float baseOpacity = 0;
        
        //up
        if (!cardinalConnections.x && position.y < twoFourths)
            baseOpacity += pow(inverselerp(position.y, twoFourths, 0), tileEdgeBlendStrenght);
        
        //down
        if (!cardinalConnections.w && position.y >= threeFourths)
            baseOpacity += pow(inverselerp(position.y, threeFourths, one), tileEdgeBlendStrenght);
        
        //left
        if (!cardinalConnections.y && position.x < twoFourths)
            baseOpacity += pow(inverselerp(position.x, twoFourths, 0), tileEdgeBlendStrenght);
        
        //right
        if (!cardinalConnections.z && position.x >= threeFourths)
            baseOpacity += pow(inverselerp(position.x, threeFourths, one), tileEdgeBlendStrenght);
        
        return baseOpacity;
    }
    
    return 0;
    
}


float4 main(float2 uv : TEXCOORD) : COLOR
{
    //Pixelate
    uv.x -= uv.x % (1 / Resolution);
    uv.y -= uv.y % (1 / Resolution);
    
    //Don't draw the corner the case of a missing corner.
    if (!cornerDrawCheck(uv))
        return float4(0, 0, 0, 0);
        
    float distanceFromPingOrigin = length(pingCenter - (tilePosition + uv * 16));
    float waveExpansionPercent = pingProgress / pingTravelTime;
    float currentPingRadius = pingRadius * waveExpansionPercent;
    float realWaveExpansionPercent = min(waveExpansionPercent, 1);
    float realPingRadius = pingRadius * realWaveExpansionPercent;
    
    
    //Crop the ping wave into a circle
    if (distanceFromPingOrigin > realPingRadius)
        return float4(0, 0, 0, 0);
    
    float waveEdgeDistanceFromCenter = max(currentPingRadius - pingWaveThickness, 0);
    float edgeBlendFactor = pow(min(max((distanceFromPingOrigin - (waveEdgeDistanceFromCenter)), 0) / (pingWaveThickness - edgeBlendOutLenght), 1), edgeBlendStrength);
    
    float3 ColorTotal = lerp(baseTintColor.rgb, waveColor.rgb, edgeBlendFactor);
    float OpacityTotal = lerp(baseTintColor.a, waveColor.a, edgeBlendFactor);
        
    //Do the outlines
    float tileEdgeBlend = getOpacityFromEdge(uv);
    OpacityTotal = lerp(OpacityTotal, tileEdgeBlend, tileEdgeBlend);
    ColorTotal = lerp(ColorTotal, tileEdgeColor, tileEdgeBlend);
    
    
    //Fade away the border
    if (realPingRadius - distanceFromPingOrigin < edgeBlendOutLenght)
        OpacityTotal *= (realPingRadius - distanceFromPingOrigin) / edgeBlendOutLenght;
    
    //General fade out at the end.
    OpacityTotal *= 1 - max(pingProgress - pingFadePoint, 0) / (1 - pingFadePoint);
    
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