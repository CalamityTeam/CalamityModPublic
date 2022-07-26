float rotation; // The rotation of the sprite.
float2 spriteDimensions;

texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

float realCos(float value)
{
    return sin(value + 1.57079);
}

//This only works on square sprites. 
float4 main(float2 uv : TEXCOORD) : COLOR
{ 
    float2x2 rotate = float2x2(realCos(rotation), -sin(rotation), sin(rotation), realCos(rotation));
    float spriteDiagonal = 1 / (sqrt(2) / 2);
    float2x2 downscale = float2x2(spriteDiagonal, 0, 0, spriteDiagonal);
    
    uv += float2(-0.5, -0.5); //remap the uv to (-0.5, -0.5) - (0.5, 0.5) for trig to work.
    uv = mul(uv, rotate);
    uv = mul(uv, downscale);
    uv += float2(0.5, 0.5); //remap the uv properly
    
    //Crop (Attempting to sample a texture with coordinates that arent between 0 to 1 wraps it around
    if (uv.x < 0 || uv.x >= 1 || uv.y < 0 || uv.y >= 1)
        return float4(0, 0, 0, 0);
    
    return tex2D(Texture1Sampler, uv);
}

technique Technique1
{
    pass RotationPass
    {
        PixelShader = compile ps_2_0 main();
    }
}
