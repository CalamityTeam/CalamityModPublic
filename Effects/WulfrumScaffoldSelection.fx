float Resolution; //Pixel resolution

float time;
float tileEdgeBlendStrenght; //How hard the border of a tile should get blended away.
float mainOpacity;
float blinkTime;

float4 baseTintColor; //Color of the tile's overlay
float3 tileEdgeColor; //Color of the tile's edge effects
float4 scanlineColor; //Color of the scanline effects
float4 placementGlowColor; //Color of the tile when first placed

float4 ScanLines[10];
int ScanLinesCount;
int verticalScanLinesIndex;

//Per tile stuff
float2 tilePosition; //The position of the top left of the tile
bool4 cardinalConnections; //Up, Left, Right, Down connections.


texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

bool AND(bool4 value)
{
    return value.x && value.y && value.z && value.w;
}

int COUNT(bool4 value)
{
    int count = 0;
    
    if (value.x)
        count++;
    if (value.y)
        count++;
    if (value.z)
        count++;
    if (value.w)
        count++;
    
    return count;
}

float MOD(float a, float n)
{
    return a - floor(a / n) * n;
}

float inverselerp(float x, float start, float end)
{
    return clamp((x - start) / (end - start), 0, 1);
}

//Checks if a corner should be drawn or excluded, based on how many edges the pixel is near to.
bool cornerDrawCheck(float2 position)
{
    float oneEight = 1 / Resolution;
    float sevenEights = 7 / Resolution;
    
    bool4 edgeChecks = bool4(false, false, false, false);
    
    //top
    if (position.y < oneEight && !cardinalConnections.x)
        edgeChecks.x = true;
         
    //bottom
    if (position.y >= sevenEights && !cardinalConnections.w)
        edgeChecks.w = true;
    
    //left
    if (position.x < oneEight && !cardinalConnections.y)
        edgeChecks.y = true;
    
    //right
    if (position.x >= sevenEights && !cardinalConnections.z)
        edgeChecks.z = true;
    
    //if on 2 edges at once, youre on a corner
    return COUNT(edgeChecks) < 2;
}

//Gets the gradient from the tile outline effects
float getOpacityFromEdge(float2 position)
{
    //If fully surrounded, don't draw anything
    if (AND(cardinalConnections))
        return 0;
    
    float oneFourth = 1 / Resolution;
    float twoFourths = 3 / Resolution;
    float threeFourths = 4 / Resolution;
    float one = 7 / Resolution;
    
    float baseOpacity = 0;
        
    //up
    if (!cardinalConnections.x)
        baseOpacity += pow(inverselerp(position.y, twoFourths, 0), tileEdgeBlendStrenght);
    
    //down
    if (!cardinalConnections.w)
        baseOpacity += pow(inverselerp(position.y, threeFourths, one), tileEdgeBlendStrenght);
    
    //left
    if (!cardinalConnections.y)
        baseOpacity += pow(inverselerp(position.x, twoFourths, 0), tileEdgeBlendStrenght);
    
    //right
    if (!cardinalConnections.z)
        baseOpacity += pow(inverselerp(position.x, threeFourths, one), tileEdgeBlendStrenght);
        
    return baseOpacity;
}

//Gets the opacity of the scanline on the specific pixel
float getOpacityFromScanLine(float2 position)
{
    float opacity = 0;
    float4 scanline;
    
    //x = offset from origin, y = period, z = speed, w = opacity.
    for (int i = 0; i < ScanLinesCount; i++)
    {
        scanline = ScanLines[i];
        
        float pixelPos;
        if (i >= verticalScanLinesIndex)
            pixelPos = (position.x + tilePosition.x / 16) % scanline.y;
           
        else
            pixelPos = (position.y + tilePosition.y / 16) % scanline.y;
        
        //Custom mod in case scanline.z is negative, aka the scanline moves in the opposite direction.
        float scanlinePos = MOD((scanline.x + time * scanline.z), scanline.y);
        
        scanlinePos -= scanlinePos % (1 / Resolution);
        
        pixelPos -= pixelPos % (1 / Resolution);
        
        if (pixelPos == scanlinePos)
            opacity += scanline.w;
    }
    
    
    return opacity * (scanlineColor.a - baseTintColor.a);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    //Pixelate
    uv.x -= uv.x % (1 / Resolution);
    uv.y -= uv.y % (1 / Resolution);
    
    //Don't draw the corner the case of a missing corner.
    if (!cornerDrawCheck(uv))
        return float4(0, 0, 0, 0);
        
    
    float3 ColorTotal = lerp(baseTintColor.rgb, placementGlowColor.rgb, blinkTime);
    float OpacityTotal = lerp(baseTintColor.a, placementGlowColor.a, blinkTime);
        
    //Do the outlines
    float tileEdgeBlend = getOpacityFromEdge(uv);
    OpacityTotal = lerp(OpacityTotal, 1, tileEdgeBlend);
    ColorTotal = lerp(ColorTotal, tileEdgeColor, tileEdgeBlend);
    
    float scanlineOpacity = getOpacityFromScanLine(uv);
    if (scanlineOpacity > 0)
    {
        OpacityTotal += scanlineOpacity;
        ColorTotal = lerp(ColorTotal, scanlineColor.rgb, scanlineOpacity);
    }
    
    //General fade out
    OpacityTotal *= mainOpacity;
    
    ColorTotal = ColorTotal * OpacityTotal;
    return float4(ColorTotal, OpacityTotal);
}

technique Technique1
{
    pass TileSelectPass
    {
        PixelShader = compile ps_3_0 main();
    }
}