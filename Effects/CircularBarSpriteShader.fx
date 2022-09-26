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
float4 uShaderSpecificData;

float4 MainPS(float2 coords : TEXCOORD0) : COLOR
{
    //this is the center of the sprite, coords work from 0 - 1
    float2 center = float2(0.5, 0.5);
    //get the vector between them
    float2 between = coords - center;
    //get the length
    float newLength = length(between);
    
    
    float angle = atan2(between.x, between.y) + 3.1415926;
    //get the progress of the angle
    float anglePercent = angle / 6.28318531;

    //if we are below the progress point, return the pixel at the coords specified
    if (anglePercent < uSaturation)
    {
        float4 barColor = tex2D(uImage0, coords);
        float opacity = uOpacity * barColor.a;
        return float4(barColor.rbg, opacity);
    }
    
    float4 subBarColor = tex2D(uImage1, coords);
    float subOpacity = uOpacity * subBarColor.a;
    return float4(subBarColor.rbg, subOpacity);
}


technique Technique1
{
    pass Pass0
    {
        PixelShader = compile ps_2_0 MainPS();
    }
}