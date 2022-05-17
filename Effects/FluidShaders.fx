sampler nextStates : register(s0);
sampler previousStates : register(s1);
sampler horizontalSpeeds : register(s2);
sampler verticalSpeeds : register(s3);
sampler divergencePBuffer : register(s4);
sampler colorField : register(s5);

bool horizontalCase_Divergence;
bool handlingColors;
float size;
float diffusionFactor;
float deltaTime;

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
    // Instead of doing this instead of using the current state to calculate the next one, we attempt to solve for a state that when
    // done in reverse returns the original state. Mathematically, this is written like so:
    
    // currentField[i, j] = nextField[i, j] - diffusionFactor * (averageNextCardinalState - currentField[i, j])

    // Rearranged such that nextField[i, j] is the given value, this results in the following equation:
    // nextField[i, j] = (currentField[i, j] + diffusionFactor * averageNextCardinalState) / (diffusionFactor + 1f)

    // This equation is hyperbolic instead of linear, and will converge in a way that allows for arbitrarily large viscosities.
    // However, as you may have noticed, this equation seeks averageNextCardinalState, which is not known. Fortunately, enough
    // variables are known to find this via a system of equations. In this case, the Gauss-Seidel Method is utilized as an iterative
    // solver
    float step = 1.0 / size;
    float4 currentValue = 0;
    float4 previousValue = tex2D(previousStates, coords);
    for (int i = 0; i < 4; i++)
    {
        float4 leftState = tex2D(nextStates, clamp(coords - float2(step, 0), step, 1 - step));
        float4 rightState = tex2D(nextStates, clamp(coords + float2(step, 0), step, 1 - step));
        float4 upState = tex2D(nextStates, clamp(coords - float2(0, step), step, 1 - step));
        float4 downState = tex2D(nextStates, clamp(coords + float2(0, step), step, 1 - step));
        float4 totalCardinalStateIteration = leftState + rightState + upState + downState;
        currentValue = (previousValue + diffusionFactor * totalCardinalStateIteration) / (diffusionFactor * 4 + 1);
    }
    return currentValue;
}

// TODO -- Discuss the details of this function a bit more.
float4 CalculateAdvection(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float step = 1.0 / size;
    float2 roundedCoords = clamp(round(coords, step), step, 1 - step);

    float color;
    float xSpeed = tex2D(horizontalSpeeds, roundedCoords).r;
    float ySpeed = tex2D(verticalSpeeds, roundedCoords).r;
    
    if (coords.x <= step * 4 || coords.x >= 1 - step * 4)
        xSpeed = 0;
    
    if (coords.y <= step * 4 || coords.y >= 1 - step * 4)
        ySpeed = 0;
        
    float viscosity = deltaTime * size;
    float X = clamp(roundedCoords.x - (viscosity * xSpeed) * step, step, 1 - step);
    float Y = clamp(roundedCoords.y - (viscosity * ySpeed) * step, step, 1 - step);
    
    X = clamp(X, 0, 1 - step);
    Y = clamp(Y, 0, 1 - step);
    
    float xGrid = round(X, step);
    float xGrid1 = xGrid + step;
    float YGrid = round(Y, step);
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

// This should be applied twice to each velocity axis buffer.
float4 ClearDivergence(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float step = 1.0 / size;
    float2 originalVelocity = float2(tex2D(horizontalSpeeds, coords).r, tex2D(verticalSpeeds, coords).r);
    if (horizontalCase_Divergence)
    {
        float rightPValue = tex2D(divergencePBuffer, clamp(coords + float2(step, 0), step, 1 - step));
        float leftPValue = tex2D(divergencePBuffer, clamp(coords - float2(step, 0), step, 1 - step));
        return float4(originalVelocity.x, 0, 0, 0);
    }
    float upPValue = tex2D(divergencePBuffer, clamp(coords - float2(0, step), step, 1 - step));
    float downPValue = tex2D(divergencePBuffer, clamp(coords + float2(0, step), step, 1 - step));
    return float4(originalVelocity.y, 0, 0, 0);
}

// According to the Navier-Stokes continuity equation, the divergence of the field is always zero, as anything else implies that matter is either being
// created from nothing or being destroyed. However, prior calculations may introduce divergence to the field. As such, this method exists as a means
// of clearing away the divergence in the field.
// According to Helmholtz's Theorem, any sufficiently smooth vector field can be expressed as the sum of two fields; one that is irrotational, and one that is solenoidal.
// This decomposition is used to get rid of the divergence part.
float4 PerformPoissonIteration(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float step = 1.0 / size;
    float rightHorizontalSpeed = tex2D(horizontalSpeeds, clamp(coords + float2(step, 0), step, 1 - step));
    float leftHorizontalSpeed = tex2D(horizontalSpeeds, clamp(coords - float2(step, 0), step, 1 - step));
    float upVerticalSpeed = tex2D(verticalSpeeds, clamp(coords - float2(0, step), step, 1 - step));
    float downVerticalSpeed = tex2D(verticalSpeeds, clamp(coords + float2(0, step), step, 1 - step));
    float divergence = -step * (rightHorizontalSpeed - leftHorizontalSpeed + downVerticalSpeed - upVerticalSpeed) * 0.5;
    
    float rightPValue = tex2D(previousStates, clamp(coords + float2(step, 0), step, 1 - step));
    float leftPValue = tex2D(previousStates, clamp(coords - float2(step, 0), step, 1 - step));
    float upPValue = tex2D(previousStates, clamp(coords - float2(0, step), step, 1 - step));
    float downPValue = tex2D(previousStates, clamp(coords + float2(0, step), step, 1 - step));
    return (divergence + rightPValue + leftPValue + upPValue + downPValue) / 4;
}

float4 DrawField(float4 sampleColor : TEXCOORD, float2 coords : TEXCOORD0) : COLOR0
{
    float density = tex2D(nextStates, coords).r;
    return float4(tex2D(colorField, coords).rgb, 1) * density;
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

    pass DrawFluidPass
    {
        PixelShader = compile ps_3_0 DrawField();
    }
}