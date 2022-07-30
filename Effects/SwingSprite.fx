float rotation; // The rotation of the sword sprite.
//float2 spriteDimensions; //The size of the sprite (necessary as the shader only gives us a map between 0 and 1).
float pommelToOriginPercent; //I have no idea for a better variable name. This is the perccentage of the full lenght between the rotation origin and the sword's tip that is an empty gap between it and the bottom left of the sword.
float4 color;

texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

float realCos(float value)
{
    return sin(value + 1.57079);
}

//This only works on square sprites. Commented out code can be found for attempts to make it work on non square sprites but my god is it a headache to work with transform matrices
//In a context like that. Gee.
//At this point i barely understand why some of it works but yea it just happens.
float4 main(float2 uv : TEXCOORD) : COLOR
{ 
    //Transformation matrices
    //float2x2 squareify = float2x2(1, 0, 0, 1);
    //float2x2 unsquareify = float2x2(1, 0, 0, 1);
    
    float2x2 rotate = float2x2(realCos(rotation), -sin(rotation), sin(rotation), realCos(rotation));
    float spriteDiagonal = 1 / (sqrt(2) / 4);
    float spriteDiagonalReal = spriteDiagonal * (1 / (1 - pommelToOriginPercent));
    
    float2x2 downscale = float2x2(spriteDiagonalReal, 0, 0, spriteDiagonalReal);
    float displaceFromOrigin = (1 / (1 - pommelToOriginPercent)) * (1 - 0.5 * (1 - pommelToOriginPercent)); //It's complicated as fuck to explain, but this gets the distance of the displacement.
    
    /*
    //Since the rotation effect stretches and squishes sprites that arent square, we have to turn it into a square before the rotation (and then unsquareify it after its done)
    if (spriteDimensions.x != spriteDimensions.y)
    {
        float aspectRatio = max(spriteDimensions.x, spriteDimensions.y) / min(spriteDimensions.x, spriteDimensions.y);
        //Rescale matrix
        squareify = float2x2(1, 0, 0, aspectRatio);
        unsquareify = float2x2(1 / aspectRatio, 0, 0, 1 / aspectRatio);
        
        if (spriteDimensions.x > spriteDimensions.y)
        {
            squareify = float2x2(aspectRatio, 0, 0, 1);
            unsquareify = float2x2(1 / aspectRatio, 0, 0, 1 / aspectRatio);
        }
    }
    */
    
    uv += float2(-0.5, -0.5); //remap the uv to (-0.5, -0.5) - (0.5, 0.5) for trig to work.
    //uv = mul(uv, squareify);
    uv = mul(uv, rotate);
    uv = mul(uv, downscale);
    //uv = mul(uv, unsquareify);
    uv += float2(-displaceFromOrigin, displaceFromOrigin);
    uv += float2(0.5, 0.5); //remap the uv properly
    
    //Crop (Attempting to sample a texture with coordinates that arent between 0 to 1 wraps it around
    if (uv.x < 0 || uv.x >= 1 || uv.y < 0 || uv.y >= 1)
        return float4(0, 0, 0, 0);
    
    return tex2D(Texture1Sampler, uv) * color;
}

technique Technique1
{
    pass SwingPass
    {
        PixelShader = compile ps_2_0 main();
    }
}
