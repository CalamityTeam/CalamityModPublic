using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.FluidSimulation
{
    public class FluidField
    {
        // TODO -- Try to make all of these specialized data structures.
        internal RenderTarget2D TemporaryAuxilaryTarget;

        internal RenderTarget2D HorizontalFieldSpeed;

        internal RenderTarget2D VerticalFieldSpeed;

        internal RenderTarget2D DensityField;

        internal RenderTarget2D ColorField;

        internal RenderTarget2D PreviousHorizontalFieldSpeed;

        internal RenderTarget2D PreviousVerticalFieldSpeed;

        internal RenderTarget2D PreviousDensityField;

        internal RenderTarget2D PreviousColorField;

        internal Queue<PixelQueueValue> DensityQueue = new();

        internal Queue<PixelQueueValue> HorizontalSpeedQueue = new();

        internal Queue<PixelQueueValue> VerticalSpeedQueue = new();

        internal Queue<PixelQueueValue> ColorFieldQueue = new();

        public int Size;

        public float Viscosity;

        public float DiffusionFactor;

        public const float DeltaTime = 0.016666f;

        public const int GaussSeidelIterations = 8;

        internal static BasicEffect basicShader = null;

        public static BasicEffect BasicShader
        {
            get
            {
                if (Main.netMode != NetmodeID.Server && basicShader is null)
                {
                    basicShader = new BasicEffect(Main.instance.GraphicsDevice)
                    {
                        VertexColorEnabled = true,
                        TextureEnabled = true
                    };
                }
                return basicShader;
            }
        }

        public FluidField(int size, float viscosity, float diffusionFactor)
        {
            Size = size;
            Viscosity = viscosity;
            DiffusionFactor = diffusionFactor;

            HorizontalFieldSpeed = new(Main.instance.GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            VerticalFieldSpeed = new(Main.instance.GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            DensityField = new(Main.instance.GraphicsDevice, Size, Size, false, 0, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            ColorField = new(Main.instance.GraphicsDevice, Size, Size, false, 0, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            PreviousHorizontalFieldSpeed = new(Main.instance.GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            PreviousVerticalFieldSpeed = new(Main.instance.GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            PreviousDensityField = new(Main.instance.GraphicsDevice, Size, Size, false, 0, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            PreviousColorField = new(Main.instance.GraphicsDevice, Size, Size, false, 0, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            // A surface format of Vector4 is used here to allow for both 0-1 ranged colors and other things at the same time.
            TemporaryAuxilaryTarget = new(Main.instance.GraphicsDevice, Size, Size, false, SurfaceFormat.Vector4, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
        }

        public void ApplyThingToTarget(RenderTarget2D currentField, Action shaderPreparationsAction)
        {
            Main.instance.GraphicsDevice.SetRenderTarget(TemporaryAuxilaryTarget);
            Main.instance.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

            shaderPreparationsAction();
            Main.spriteBatch.Draw(currentField, currentField.Bounds, Color.White);
            Main.spriteBatch.End();

            currentField.CopyContentsFrom(TemporaryAuxilaryTarget);
            Main.instance.GraphicsDevice.SetRenderTarget(null);
        }

        public void FlushQueueToTarget(RenderTarget2D currentField, Queue<PixelQueueValue> queue)
        {
            ApplyThingToTarget(currentField, () =>
            {
                int batchIndex = 0;
                int pixelCount = queue.Count;

                // Get the FUCK out of here if the queue is empty. If this check isn't here the primitive drawing method will attempt to access
                // memory that it shouldn't and the OS will tell the program to go fuck itself, resulting in a crash.
                Texture2D pixel = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Pixel").Value;
                if (pixelCount <= 0)
                    return;

                CalamityUtils.CalculatePerspectiveMatricies(out Matrix viewMatrix, out Matrix projectionMatrix);
                BasicShader.View = viewMatrix;
                BasicShader.Projection = projectionMatrix;

                // Go through all particles and draw them directly with primitives.
                BasicShader.CurrentTechnique.Passes[0].Apply();

                // Decide the vertices and indices.
                FieldVertex2D[] vertices = new FieldVertex2D[pixelCount * 4];
                short[] indices = new short[pixelCount * 6];

                // Go through the queue and prepare the vertices/indices.
                while (queue.TryDequeue(out PixelQueueValue v))
                {
                    Color value = new(v.Value.X, v.Value.Y, v.Value.Z, v.Value.W);
                    Vector2 topLeft = v.Position;
                    Vector2 topRight = v.Position + Vector2.UnitX;
                    Vector2 bottomLeft = v.Position + Vector2.UnitY;
                    Vector2 bottomRight = v.Position + Vector2.One;

                    vertices[batchIndex * 4] = new FieldVertex2D(topLeft, v.Value, new Vector2(0f, 0f));
                    vertices[batchIndex * 4 + 1] = new FieldVertex2D(topRight, v.Value, new Vector2(1f, 0f));
                    vertices[batchIndex * 4 + 2] = new FieldVertex2D(bottomRight, v.Value, new Vector2(1f, 1f));
                    vertices[batchIndex * 4 + 3] = new FieldVertex2D(bottomLeft, v.Value, new Vector2(0f, 1f));

                    // Construct independent primitives by creating a square from two triangles defined by the edges of the particle.
                    indices[batchIndex * 6] = (short)(batchIndex * 4);
                    indices[batchIndex * 6 + 1] = (short)(batchIndex * 4 + 1);
                    indices[batchIndex * 6 + 2] = (short)(batchIndex * 4 + 2);
                    indices[batchIndex * 6 + 3] = (short)(batchIndex * 4);
                    indices[batchIndex * 6 + 4] = (short)(batchIndex * 4 + 2);
                    indices[batchIndex * 6 + 5] = (short)(batchIndex * 4 + 3);
                    batchIndex++;

                    // I don't know why this is necessary to make the primitives render, but it is.
                    Main.spriteBatch.Draw(pixel, Vector2.Zero, null, Color.Transparent);
                }

                // Flush the vertices and indices and draw them to the render target.
                Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, pixelCount * 4, indices, 0, pixelCount * 2);
            });
        }

        public void CalculateDiffusion(float diffusionFactor, RenderTarget2D currentField, RenderTarget2D nextField, bool colors = false)
        {
            diffusionFactor *= DeltaTime * Size;
            ApplyThingToTarget(nextField, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = currentField;
                CalamityShaders.FluidShaders.Parameters["size"].SetValue(Size);
                CalamityShaders.FluidShaders.Parameters["diffusionFactor"].SetValue(diffusionFactor);
                CalamityShaders.FluidShaders.Parameters["handlingColors"].SetValue(colors);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["DiffusionPass"].Apply();
            });
        }

        public void CalculateAdvection(RenderTarget2D currentField, RenderTarget2D nextField, RenderTarget2D horizontalVelocities, RenderTarget2D verticalVelocities, bool colors = false)
        {
            ApplyThingToTarget(nextField, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = currentField;
                Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                CalamityShaders.FluidShaders.Parameters["size"].SetValue(Size);
                CalamityShaders.FluidShaders.Parameters["deltaTime"].SetValue(DeltaTime);
                CalamityShaders.FluidShaders.Parameters["handlingColors"].SetValue(colors);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["AdvectionPass"].Apply();
            });
        }

        public void ClearDivergence(RenderTarget2D horizontalVelocities, RenderTarget2D verticalVelocities, RenderTarget2D p)
        {
            for (int i = 0; i < GaussSeidelIterations; i++)
            {
                ApplyThingToTarget(p, () =>
                {
                    Main.instance.GraphicsDevice.Textures[1] = p;
                    Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                    Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                    CalamityShaders.FluidShaders.Parameters["size"].SetValue(Size);
                    CalamityShaders.FluidShaders.CurrentTechnique.Passes["PerformPoissonIterationPass"].Apply();
                });
            }

            ApplyThingToTarget(horizontalVelocities, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                Main.instance.GraphicsDevice.Textures[4] = p;
                CalamityShaders.FluidShaders.Parameters["horizontalCase_Divergence"].SetValue(true);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["ClearDivergencePass"].Apply();
            });

            ApplyThingToTarget(verticalVelocities, () =>
            {
                Main.instance.GraphicsDevice.Textures[1] = verticalVelocities;
                Main.instance.GraphicsDevice.Textures[2] = horizontalVelocities;
                Main.instance.GraphicsDevice.Textures[3] = verticalVelocities;
                Main.instance.GraphicsDevice.Textures[4] = p;
                CalamityShaders.FluidShaders.Parameters["horizontalCase_Divergence"].SetValue(false);
                CalamityShaders.FluidShaders.CurrentTechnique.Passes["ClearDivergencePass"].Apply();
            });
        }

        public void CreateSource(int x, int y, float density, Color color, Vector2 velocity)
        {
            Vector2 pos = new(x, y);

            if (x < 0 || y < 0 || x >= Size || y >= Size)
                return;

            ColorFieldQueue.Enqueue(new PixelQueueValue(pos, color));

            HorizontalSpeedQueue.Enqueue(new(pos, new Vector4(velocity.X, 0f, 0f, 0f)));
            VerticalSpeedQueue.Enqueue(new(pos, new Vector4(velocity.Y, 0f, 0f, 0f)));

            DensityQueue.Enqueue(new PixelQueueValue(pos, new Color(density, 0f, 0f)));
        }

        public void Update()
        {
            // Everything here involves heavy manipulation of render targets to work. Doing any of that on the server
            // would certainly result in a crash due to a lack of a graphics device.
            if (Main.netMode == NetmodeID.Server)
                return;

            // Create a constant stream of fluid.
            Color sprayColor = Color.Lerp(Color.Red, Color.Yellow, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.5f + 0.5f);
            Vector2 sprayVelocity = (Main.GlobalTimeWrappedHourly * 1.6f).ToRotationVector2() * 0.2f;

            int x = (int)((Main.MouseScreen.X - Main.screenWidth * 0.2f) / 2f);
            int y = (int)((Main.MouseScreen.Y - Main.screenHeight * 0.2f) / 2f);
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dy = -3; dy <= 3; dy++)
                    CreateSource(x + dx, y + dy, 1f, sprayColor, sprayVelocity);
            }

            // Clear queues.
            FlushQueueToTarget(HorizontalFieldSpeed, HorizontalSpeedQueue);
            FlushQueueToTarget(VerticalFieldSpeed, VerticalSpeedQueue);
            FlushQueueToTarget(ColorField, ColorFieldQueue);
            FlushQueueToTarget(DensityField, DensityQueue);

            UpdateVelocityFields();
            UpdateDensityFields();
        }

        public void UpdateVelocityFields()
        {
            Utils.Swap(ref PreviousHorizontalFieldSpeed, ref HorizontalFieldSpeed);
            CalculateDiffusion(Viscosity, HorizontalFieldSpeed, PreviousHorizontalFieldSpeed);
            Utils.Swap(ref PreviousVerticalFieldSpeed, ref VerticalFieldSpeed);
            CalculateDiffusion(Viscosity, VerticalFieldSpeed, PreviousVerticalFieldSpeed);

            ClearDivergence(HorizontalFieldSpeed, VerticalFieldSpeed, PreviousHorizontalFieldSpeed);

            Utils.Swap(ref PreviousHorizontalFieldSpeed, ref HorizontalFieldSpeed);
            Utils.Swap(ref PreviousVerticalFieldSpeed, ref VerticalFieldSpeed);

            CalculateAdvection(HorizontalFieldSpeed, PreviousHorizontalFieldSpeed, PreviousHorizontalFieldSpeed, PreviousVerticalFieldSpeed);
            CalculateAdvection(VerticalFieldSpeed, PreviousVerticalFieldSpeed, PreviousHorizontalFieldSpeed, PreviousVerticalFieldSpeed);

            ClearDivergence(HorizontalFieldSpeed, VerticalFieldSpeed, PreviousHorizontalFieldSpeed);
        }

        public void UpdateDensityFields()
        {
            Utils.Swap(ref PreviousDensityField, ref DensityField);
            CalculateDiffusion(DiffusionFactor, DensityField, PreviousDensityField);
            CalculateAdvection(PreviousDensityField, DensityField, HorizontalFieldSpeed, VerticalFieldSpeed);

            Utils.Swap(ref PreviousColorField, ref ColorField);
            CalculateDiffusion(DiffusionFactor, ColorField, PreviousColorField, true);
            CalculateAdvection(PreviousColorField, ColorField, HorizontalFieldSpeed, VerticalFieldSpeed, true);
        }

        public void Draw()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
            Main.instance.GraphicsDevice.Textures[5] = ColorField;
            CalamityShaders.FluidShaders.CurrentTechnique.Passes["DrawFluidPass"].Apply();
            Main.spriteBatch.Draw(DensityField, new Vector2(Main.screenWidth, Main.screenHeight) * 0.21f, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);
            Main.spriteBatch.End();
        }
    }
}
