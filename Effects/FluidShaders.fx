sampler nextStates : register(s0);
sampler previousStates : register(s1);
sampler velocityField : register(s2);
sampler divergencePBuffer : register(s3);
sampler divergenceField : register(s4);
sampler colorField : register(s5);

bool horizontalCase_Divergence;
bool handlingColors;
float size;
float diffusionFactor;
float deltaTime;
float dissipationFactor;

float round(float n, float precision)
{
    return floor(n / precision) * precision;
}

float2 round(float2 n, float precision)
{
    return float2(round(n.x, precision), round(n.y, precision));
}

float4 CalculateDiffusion(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    // A naive way of diffusing values would be to linearly interpolate between the old and new state like so:
    // nextField[i, j] = currentField[i, j] + diffusionFactor * (totalCardinalState - currentField[i, j])
    // However, this would considerably restrict the freedom of the simulation, as any visocity values above 1 would result
    // in overshooting estimations and cause unstable behavior.
    // Instead of using the current state to calculate the next one, we attempt to solve for a state that when
    // done in reverse returns the original state. Mathematically, this is written like so:
    
    // currentField[i, j] = nextField[i, j] - diffusionFactor * (averageNextCardinalState - currentField[i, j])

    // Rearranged such that nextField[i, j] is the given value, this results in the following equation:
    // nextField[i, j] = (currentField[i, j] + diffusionFactor * averageNextCardinalState) / (diffusionFactor + 1f)

    // This equation is hyperbolic instead of linear, and will converge in a way that allows for arbitrarily large viscosities.
    // However, as you may have noticed, this equation seeks averageNextCardinalState, which is not known. Fortunately, enough
    // variables are known to find this via a system of equations. In this case, the Gauss-Seidel Method is utilized as an iterative
    // solver.
    float step = 1.0 / size;
    float4 currentValue = 0;
    float4 previousValue = tex2D(previousStates, coords);
    for (int i = 0; i < 2; i++)
    {
        float4 leftState = tex2D(nextStates, clamp(coords - float2(step, 0), step, 1 - step));
        float4 rightState = tex2D(nextStates, clamp(coords + float2(step, 0), step, 1 - step));
        float4 upState = tex2D(nextStates, clamp(coords - float2(0, step), step, 1 - step));
        float4 downState = tex2D(nextStates, clamp(coords + float2(0, step), step, 1 - step));
        float4 totalCardinalStateIteration = leftState + rightState + upState + downState;
        currentValue = (previousValue + diffusionFactor * totalCardinalStateIteration) / (diffusionFactor * 4 + 1);
    }
    return currentValue * dissipationFactor;
}

float4 CalculateAdvection(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    // Similar to the diffusion calculations, this step is performed in such a way that works backwards instead of forwards, by calculating
    // the contributions from the four pixels that will end up affecting the current pixel. This is determined based on a linear interpolation
    // that gives more weight to each of the four squares the closer their centers are to the place where the backwards step landed.
    float step = 1.0 / size;
    float2 roundedCoords = clamp(coords, step, 1 - step);

    float color;
    float2 velocity = tex2D(velocityField, roundedCoords).rg;
    
    if (coords.x <= step * 4 || coords.x >= 1 - step * 4)
        velocity.x = 0;
    
    if (coords.y <= step * 4 || coords.y >= 1 - step * 4)
        velocity.y = 0;
        
    float viscosity = deltaTime * size;
    float X = clamp(roundedCoords.x - (viscosity * velocity.x) * step, step, 1 - step);
    float Y = clamp(roundedCoords.y - (viscosity * velocity.y) * step, step, 1 - step);
    
    X = clamp(X, 0, 1 - step);
    Y = clamp(Y, 0, 1 - step);
    
    float xGrid = X;
    float xGrid1 = xGrid + step;
    float YGrid = Y;
    float yGrid = YGrid + step;
    
    xGrid = clamp(xGrid, 0, 1);
    YGrid = clamp(YGrid, 0, 1);
    xGrid1 = clamp(xGrid1, 0, 1);
    yGrid = clamp(yGrid, 0, 1);

    float XRelative1 = (X - xGrid) / step;
    float XRelative0 = 1 - XRelative1;
    float YRelative1 = (Y - YGrid) / step;
    float YRelative0 = 1 - YRelative1;

    float4 c1 = tex2D(previousStates, float2(xGrid, YGrid));
    float4 c2 = tex2D(previousStates, float2(xGrid, yGrid));
    float4 c3 = tex2D(previousStates, float2(xGrid1, YGrid));
    float4 c4 = tex2D(previousStates, float2(xGrid1, yGrid));
    return XRelative0 * (YRelative0 * c1 + YRelative1 * c2) +
           XRelative1 * (YRelative0 * c3 + YRelative1 * c4);
}

// According to the Navier-Stokes continuity equation, the divergence of the field is always zero, as anything else implies that matter is either being
// created from nothing or being destroyed. However, prior calculations may introduce divergence to the field. As such, this method exists as a means
// of clearing away the divergence in the field.
// According to Helmholtz's Theorem, any sufficiently smooth vector field can be expressed as the sum of two fields; one that is irrotational, and one that is solenoidal.
// This decomposition is used to get rid of the divergence part.
float4 ClearDivergence(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float step = 1.0 / size;
    float2 originalVelocity = tex2D(velocityField, coords).rg;
    float rightPValue = tex2D(divergencePBuffer, clamp(coords + float2(step, 0), step, 1 - step));
    float leftPValue = tex2D(divergencePBuffer, clamp(coords - float2(step, 0), step, 1 - step));
    float upPValue = tex2D(divergencePBuffer, clamp(coords - float2(0, step), step, 1 - step));
    float downPValue = tex2D(divergencePBuffer, clamp(coords + float2(0, step), step, 1 - step));
    
    originalVelocity.x -= (rightPValue - leftPValue) * 0.5 / step;
    originalVelocity.y -= (downPValue - upPValue) * 0.5 / step;
    return float4(originalVelocity.x, originalVelocity.y, 0, 0);
}

float4 PerformPoissonIteration(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float step = 1.0 / size;
    
    float rightPValue = tex2D(previousStates, clamp(coords + float2(step, 0), step, 1 - step));
    float leftPValue = tex2D(previousStates, clamp(coords - float2(step, 0), step, 1 - step));
    float upPValue = tex2D(previousStates, clamp(coords - float2(0, step), step, 1 - step));
    float downPValue = tex2D(previousStates, clamp(coords + float2(0, step), step, 1 - step));
    return (tex2D(divergenceField, clamp(coords, step, 1 - step)).r + rightPValue + leftPValue + upPValue + downPValue) / 4;
}

float4 InitializeDivergence(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float step = 1.0 / size;
    float rightHorizontalSpeed = tex2D(velocityField, clamp(coords + float2(step, 0), step, 1 - step)).r;
    float leftHorizontalSpeed = tex2D(velocityField, clamp(coords - float2(step, 0), step, 1 - step)).r;
    float upVerticalSpeed = tex2D(velocityField, clamp(coords - float2(0, step), step, 1 - step)).g;
    float downVerticalSpeed = tex2D(velocityField, clamp(coords + float2(0, step), step, 1 - step)).g;
    return -step * (rightHorizontalSpeed - leftHorizontalSpeed + downVerticalSpeed - upVerticalSpeed) * 0.5;
}

float4 DrawField(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float density = pow(tex2D(nextStates, coords).r, 1.84);
    float3 color = tex2D(colorField, coords).rgb;
    return float4(color, 1) * density;
}

technique Technique1
{
    pass DiffusionPass
    {
        PixelShader = compile ps_3_0 CalculateDiffusion();
    }

    pass AdvectionPass
    {
        PixelShader = compile ps_3_0 CalculateAdvection();
    }

    pass ClearDivergencePass
    {
        PixelShader = compile ps_3_0 ClearDivergence();
    }

    pass PerformPoissonIterationPass
    {
        PixelShader = compile ps_3_0 PerformPoissonIteration();
    }

    pass InitializeDivergencePass
    {
        PixelShader = compile ps_3_0 InitializeDivergence();
    }

    pass DrawFluidPass
    {
        PixelShader = compile ps_3_0 DrawField();
    }
}