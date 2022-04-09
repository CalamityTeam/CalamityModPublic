sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor; //Color of the telegraph zone
float3 uSecondaryColor; //Color of the outline
float uOpacity;
float uSaturation; //Saturation is being used as completion here. 0 = Telegraph start, 1 = Telegraph about to end
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uShaderSpecificData;

float4 Recolor(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float3 color = uColor;
    //this is the center of the sprite, coords work from 0 - 1
    float2 center = float2(0.5, 0.5);
    //get the length of the doubled distance, so that 0 = at the center of the sprite and 1 = at the very edge of the circle
    float distanceFromCenter = length(coords - center) * 2;
    
    //Crop the sprite into a circle
    if (distanceFromCenter > 1)
        return float4(0, 0, 0, 0);
    
    //Make the brightness increase towards the edge
    float opacity = 0.1 + (0.2 + 0.35 * pow(uSaturation, 2)) * distanceFromCenter;
    
    //Brighten the very edge
    if (distanceFromCenter > 0.995)
    {
        //Make the color get closer to the outline color. At halfway completion the transition is complete (so proud of them)
        color = lerp(color, uSecondaryColor, min(uSaturation * 2, 1));
        
        //Do the same for the opacity
        opacity = lerp(opacity, 1, pow(uSaturation, 2)) * lerp(uOpacity, 1, min(uSaturation * 1.25, min(uOpacity + 0.1, 1)));
    }
    
    else
        opacity = opacity * uOpacity;
    
    color = color * opacity;
    
    return float4(color, opacity);
}


technique Technique1
{
    pass TelegraphPass
    {
        PixelShader = compile ps_2_0 Recolor();
    }
}