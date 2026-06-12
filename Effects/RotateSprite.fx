sampler baseTexture : register(s0);
float rotation;                         // The rotation of the sprite.
float2 spriteDimensions;                // Dimensions of the sprite.
float4 spriteRectangle;                 // For use with animated sprites.

float realCos(float value)
{
    return sin(value + 1.57079);
}

//This only works on square sprites. 
float4 main(float4 sampleColor : COLOR0, float2 uv : TEXCOORD0) : COLOR0
{ 
    float2x2 rotate = float2x2(realCos(rotation), -sin(rotation), sin(rotation), realCos(rotation));
    float spriteDiagonal = 1 / (sqrt(2) / 2);
    float2x2 downscale = float2x2(spriteDiagonal, 0, 0, spriteDiagonal);
    
    uv += float2(-0.5, -0.5); //remap the uv to (-0.5, -0.5) - (0.5, 0.5) for trig to work.
    uv = mul(uv, rotate);
    //uv = mul(uv, downscale);
    uv += float2(0.5, 0.5); //remap the uv properly
    
    //Crop (Attempting to sample a texture with coordinates that arent between 0 to 1 wraps it around
    if (uv.x < 0 || uv.x >= 1 || uv.y < 0 || uv.y >= 1)
        return float4(0, 0, 0, 0);
    
    return tex2D(baseTexture, uv) * sampleColor;
}

technique Technique1
{
    pass RotationPass
    {
        PixelShader = compile ps_2_0 main();
    }
}
