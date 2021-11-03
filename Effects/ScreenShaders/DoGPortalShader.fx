sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

float4 PixelShaderFunction(float4 position : SV_POSITION, float2 coords : TEXCOORD0) : COLOR0
{
    // Calculate relative screen values.
    float2 targetUVCoords = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 centerOffset = (coords - targetUVCoords) * (uScreenResolution / uScreenResolution.y);
    float distanceFromTargetPosition = distance(coords, targetUVCoords) / uZoom;
    
    // Calculate the swirl coordinates.
    float2 centeredCoords = (targetUVCoords - coords) * (uScreenResolution / uScreenResolution.y);
    float swirlRotation = length(centeredCoords) * 21.2 - uTime * 6;
    float swirlSine = sin(swirlRotation);
    float swirlCosine = cos(swirlRotation);
    float2x2 swirlRotationMatrix = float2x2(swirlCosine, -swirlSine, swirlSine, swirlCosine);
    float2 swirlCoordinates = mul(centeredCoords, swirlRotationMatrix) + 0.5;
    
    // Calculate fade, swirl arm colors, and draw the portal to the screen.
    float4 color = tex2D(uImage0, coords);
    float swirlColorFade = saturate(distanceFromTargetPosition * 3) / (uProgress + 0.0001);
    float3 swirlBaseColor = lerp(uColor, uSecondaryColor, pow(swirlColorFade, 0.33));
    float4 swirlNoiseColor = tex2D(uImage1, swirlCoordinates) * (1 - swirlColorFade);
    float4 endColor = lerp(float4(swirlBaseColor, 0.1), color, swirlColorFade);
    return lerp(color, endColor * (1 + (1 - swirlColorFade) * 2), saturate(swirlNoiseColor.r));
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}