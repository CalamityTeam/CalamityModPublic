sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float3 uColor;
float3 uSecondaryColor;
float uOpacity;
float uSaturation;
float uRotation;
float uTime;
float4 uSourceRect;
float2 uWorldPosition;
float uDirection;
float3 uLightSource;
float2 uImageSize0;
float2 uImageSize1;
float4 uShaderSpecificData;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate the swirl coordinates.
    float2 centeredCoords = coords - 0.5;
    float swirlRotation = length(centeredCoords) * 17.2 - uTime * 6;
    float swirlColorFade = 1 - saturate(length(centeredCoords) * 1.8);
    float swirlSine = sin(swirlRotation);
    float swirlCosine = sin(swirlRotation + 1.5707);
    float2x2 swirlRotationMatrix = float2x2(swirlCosine, -swirlSine, swirlSine, swirlCosine);
    float2 swirlCoordinates = mul(centeredCoords, swirlRotationMatrix) + 0.5;
    return tex2D(uImage0, swirlCoordinates) * sampleColor * swirlColorFade;
}
technique Technique1
{
    pass VortexPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}