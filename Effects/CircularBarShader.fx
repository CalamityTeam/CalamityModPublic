sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation; //Saturation is being used as completion here
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;

float4 MainPS(float2 coords : TEXCOORD0) : COLOR
{
    //this is the center of the sprite, coords work from 0 - 1
    float2 center = float2(0.5, 0.5);
    //get the vector between them
    float2 between = coords - center;
    //get the length
    float newLength = length(between);
    
    //Crop the sprite into a circle
    if (newLength > 0.5)
        return float4(0, 0, 0, 0);
    
    //Make the brightness increase towards the edge
    float brightness = 0.7 + 0.3 * (newLength / 0.5);
    
    //Brighten the very edge
    if (newLength > 0.4)
        brightness = 1;
    //Give it a darker outline
    if (newLength > 0.46)
        brightness = 0.3;
    
    //get the new angle that we want it to be at, as otherwise it works from the left
    float angle = atan2(between.y, between.x) - 1.57079633;
    //shift angle to between 0 and 2 pi
    angle += 3.14159265;
    //get the progress of the angle
    float anglePercent = angle / 6.28318531;

    //if we are below the progress point, return the pixel at the coords specified
    if (anglePercent < uSaturation)
    {
        float3 barColor = uColor * (1 - anglePercent) + uSecondaryColor * anglePercent;
        barColor.rbg *= brightness;
        return float4(barColor, uOpacity);
    }

    //If we are on the empty part of the cooldown bar, crop the circle down
    if (newLength > 0.35)
        return float4(0, 0, 0, 0);
    
    //Darken the bar color as well
    brightness = 0.4;
    float3 barColor = uColor * brightness;
    return float4(barColor, uOpacity * 0.8);
}


technique Technique1
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}