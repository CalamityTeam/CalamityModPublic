float time;
float blowUpPower; //The power the blow up effect is set as. Ideally betwen 0 and 1
float blowUpSize; //The strenght of the expansion caused by the blow up effect
float3 shieldColor;
float shieldOpacity;
float3 shieldEdgeColor;
float shieldEdgeBlendStrenght;
float noiseScale;
float resolution;

texture sampleTexture;
sampler2D Texture1Sampler = sampler_state { texture = <sampleTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter = LINEAR; AddressU = wrap; AddressV = wrap; };

float4 main(float2 uv : TEXCOORD) : COLOR
{
    //Pixelate?
    //uv.x -= uv.x % (1 / resolution);
    //uv.y -= uv.y % (1 / resolution);
    
    //Crop in a circle
    float distanceFromCenter = length(uv - float2(0.5, 0.5)) * 2;
    if (distanceFromCenter > 1)
        return float4(0, 0, 0, 0);
    
    //"Blow up" the noise map so it looks circular.
    float blownUpUVX = pow((abs(uv.x - 0.5)) * 2, blowUpPower);
    float blownUpUVY = pow((abs(uv.y - 0.5)) * 2, blowUpPower);
    float2 blownUpUV = float2(-blownUpUVY * blowUpSize * 0.5 + uv.x * (1 + blownUpUVY * blowUpSize), -blownUpUVX * blowUpSize * 0.5 + uv.y * (1 + blownUpUVX * blowUpSize));
    
    //Rescale
    blownUpUV *= noiseScale;
    //Scroll effect
    blownUpUV.x = (blownUpUV.x + time) % 1;
    
    //Get the noise color
    float4 noiseColor = tex2D(Texture1Sampler, blownUpUV);

    //Apply a layers of fake fresnel
    noiseColor += pow(distanceFromCenter, 6);  // + pow(distanceFromCenter, 3) * 0.6; <- Brings us over into ps 3.0 territory if we have pixelation on.
    //Fade the edges
    if (distanceFromCenter > 0.95)
        noiseColor *= (1 - ((distanceFromCenter - 0.95) / 0.05));
    
    return noiseColor * float4(lerp(shieldColor, shieldEdgeColor, pow(distanceFromCenter, shieldEdgeBlendStrenght)), shieldOpacity);
}

technique Technique1
{
    pass ShieldPass
    {
        PixelShader = compile ps_2_0 main();
    }
}