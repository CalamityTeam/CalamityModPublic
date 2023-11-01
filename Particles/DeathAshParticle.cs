using CalamityMod.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Particles
{
    public class DeathAshParticle
    {
        internal static Dictionary<NPC, ManagedRenderTarget> PendingNPCsToDraw = new();
        internal static BasicEffect basicShader = null;
        internal static VertexPositionColorTexture[] VertexCache = new VertexPositionColorTexture[PrimitiveBatchSize * 4];
        internal static short[] IndexCache = new short[PrimitiveBatchSize * 6];

        public int Time;
        public int Lifetime;
        public int ID;
        public float Scale = 1f;
        public Color AshColor;
        public Vector2 Center;
        public Vector2 Velocity;
        public Vector2 TopLeft => Center - Vector2.One * Scale * 3.5f;
        public Vector2 TopRight => Center + new Vector2(1f, -1f) * Scale * 3.5f;
        public Vector2 BottomRight => Center + Vector2.One * Scale * 3.5f;
        public Vector2 BottomLeft => Center + new Vector2(-1f, 1f) * Scale * 3.5f;

        public static HashSet<DeathAshParticle> Ashes = new HashSet<DeathAshParticle>();
        public const int PrimitiveBatchSize = 256;
        public const int AshCountLimit = 45000;
        public static BasicEffect BasicShader
        {
            get
            {
                if (Main.netMode != NetmodeID.Server && basicShader is null)
                {
                    basicShader = new BasicEffect(Main.instance.GraphicsDevice)
                    {
                        VertexColorEnabled = true,
                        TextureEnabled = false
                    };
                }
                return basicShader;
            }
        }

        public DeathAshParticle(int lifetime, float brightness, Vector2 spawnPosition)
        {
            Time = 0;
            Lifetime = lifetime;
            ID = Ashes.Count;
            AshColor = Main.hslToRgb(0f, 0f, brightness * 0.67f);
            Scale = Main.rand.NextFloat(0.7f, 1.3f);
            Center = spawnPosition;
        }

        public static void PrepareRenderTargets()
        {
            foreach (NPC npc in PendingNPCsToDraw.Keys)
            {
                // Prepare the sprite batch for specialized drawing in prepration that the graphics device will draw to new render targets.
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

                // Draw the NPC to the temporary render target so that all of its drawcode is localized, and then get the colors from the results.
                Main.instance.GraphicsDevice.SetRenderTarget(PendingNPCsToDraw[npc]);
                Main.instance.GraphicsDevice.Clear(Color.Transparent);

                Vector2 oldPosition = npc.position;
                npc.oldPos = new Vector2[npc.oldPos.Length];
                npc.position = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                try
                {
                    Main.instance.DrawNPC(npc.whoAmI, true);
                }
                catch { }
                npc.position = oldPosition;
                npc.Opacity = 0f;

                Main.spriteBatch.End();
            }
            Main.instance.GraphicsDevice.SetRenderTarget(null);
        }

        public static void CreateAshesFromNPC(NPC npc)
        {
            // Don't create ashes serverside.
            if (Main.netMode == NetmodeID.Server)
                return;

            PendingNPCsToDraw[npc] = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
        }

        public static Dictionary<Vector2, Color> GetColorCacheFromTexture(Texture2D texture, Rectangle? frame = null, bool pruneForEfficency = false)
        {
            Dictionary<Vector2, Color> colorCache = new Dictionary<Vector2, Color>();
            Color[] uncleanedCache = new Color[texture.Width * texture.Height];
            texture.GetData(uncleanedCache);

            // Initialize the frame to its default of the texture size if nothing else is specified.
            int width = texture.Width;
            int height = texture.Height;
            if (frame is null)
                frame = new Rectangle(0, 0, width, height);

            int stride = 1;
            int totalFilledPixels = uncleanedCache.Count(c => c.R != 0 || c.G != 0 || c.B != 0 || c.A != 0);
            if (pruneForEfficency && totalFilledPixels > 16000)
                stride = 2;
            if (pruneForEfficency && totalFilledPixels > 38000)
                stride = 3;

            // Store all non-empty pixels.
            for (int i = 0; i < width; i += stride)
            {
                for (int j = 0; j < height; j += stride)
                {
                    Color color = uncleanedCache[i + j * width];

                    // Ignore textures outside the bounds of the frame.
                    if (i < frame.Value.Left || i >= frame.Value.Right || j < frame.Value.Top || j >= frame.Value.Bottom)
                        continue;

                    if (color.R != 0 || color.G != 0 || color.B != 0 || color.A != 0)
                        colorCache[new Vector2(i - frame.Value.X, j - frame.Value.Y)] = color;
                }
            }
            return colorCache;
        }

        public static void FlushContentsAndCreateAshes()
        {
            // Don't do anything if no NPCs have to be drawn.
            if (PendingNPCsToDraw.Count == 0)
                return;

            foreach (NPC npc in PendingNPCsToDraw.Keys)
            {
                RenderTarget2D temporaryTextureDrawTarget = PendingNPCsToDraw[npc].Target;
                Rectangle frame = new Rectangle(0, 0, temporaryTextureDrawTarget.Width, temporaryTextureDrawTarget.Height);
                Dictionary<Vector2, Color> colorsOnNPC = GetColorCacheFromTexture(temporaryTextureDrawTarget, frame, true);

                // Enforce a hard limit on the amount of ashes that can exist.
                if (Ashes.Count + colorsOnNPC.Count > AshCountLimit)
                    break;

                // Create ashes.
                foreach (Vector2 drawOffset in colorsOnNPC.Keys)
                {
                    int ashLifetime = Main.rand.Next(105, 145);

                    // Adjust the offset of the ashes from pixel space by accounting for direction, rotation, and scale.
                    Vector2 ashSpawnPosition = npc.position + drawOffset - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
                    Color color = colorsOnNPC[drawOffset];
                    float brightness = (color.R + color.G + color.B) / 765f;
                    DeathAshParticle ash = new DeathAshParticle(ashLifetime, brightness, ashSpawnPosition)
                    {
                        Velocity = (Main.npc.IndexInRange(npc.realLife) ? Main.npc[npc.realLife].velocity : npc.velocity) * 0.7f
                    };

                    if (brightness > 0.05f)
                        Ashes.Add(ash);
                }

                // And release the render target.
                PendingNPCsToDraw[npc].Dispose();
            }

            // Clear the pending NPCs once done.
            PendingNPCsToDraw.Clear();
        }

        public static void DrawAll()
        {
            FlushContentsAndCreateAshes();

            // Redefine the perspective matrices of the shader.
            CalamityUtils.CalculatePerspectiveMatricies(out Matrix effectView, out Matrix effectProjection);
            BasicShader.View = effectView;
            BasicShader.Projection = effectProjection;

            int batchIndex = 0;

            // Reset the cache before drawing.
            Array.Clear(VertexCache, 0, VertexCache.Length);
            Array.Clear(IndexCache, 0, IndexCache.Length);

            // Go through all particles and draw them directly with primitives.
            // Particles are drawn in batches, to prevent unnecessary overhead in the form of primitive draw preparations.
            BasicShader.CurrentTechnique.Passes[0].Apply();
            foreach (DeathAshParticle ash in Ashes)
            {
                Color fadedAshColor = ash.AshColor * ash.Scale;

                VertexCache[batchIndex * 4] = new VertexPositionColorTexture(new Vector3(ash.TopLeft - Main.screenPosition, 0f), fadedAshColor, new Vector2(0f, 0f));
                VertexCache[batchIndex * 4 + 1] = new VertexPositionColorTexture(new Vector3(ash.TopRight - Main.screenPosition, 0f), fadedAshColor, new Vector2(1f, 0f));
                VertexCache[batchIndex * 4 + 2] = new VertexPositionColorTexture(new Vector3(ash.BottomRight - Main.screenPosition, 0f), fadedAshColor, new Vector2(1f, 1f));
                VertexCache[batchIndex * 4 + 3] = new VertexPositionColorTexture(new Vector3(ash.BottomLeft - Main.screenPosition, 0f), fadedAshColor, new Vector2(0f, 1f));

                // Construct independent primitives by creating a square from two triangles defined by the edges of the particle.
                IndexCache[batchIndex * 6] = (short)(batchIndex * 4);
                IndexCache[batchIndex * 6 + 1] = (short)(batchIndex * 4 + 1);
                IndexCache[batchIndex * 6 + 2] = (short)(batchIndex * 4 + 2);
                IndexCache[batchIndex * 6 + 3] = (short)(batchIndex * 4);
                IndexCache[batchIndex * 6 + 4] = (short)(batchIndex * 4 + 2);
                IndexCache[batchIndex * 6 + 5] = (short)(batchIndex * 4 + 3);

                batchIndex++;

                // Draw the batch once it is at full capacity.
                if (batchIndex >= PrimitiveBatchSize)
                {
                    Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexCache, 0, PrimitiveBatchSize * 4, IndexCache, 0, PrimitiveBatchSize * 2);
                    batchIndex = 0;
                }
            }

            // Draw any remaining primitives which did not fit neatly into individual batches in the above loop.
            // All undeclared vertices in the cache are ignored.
            if (batchIndex > 0)
                Main.instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, VertexCache, 0, batchIndex * 4, IndexCache, 0, batchIndex * 2);
        }

        public void Update()
        {
            float brightness = (AshColor.R + AshColor.G + AshColor.B) / 765f;

            Time++;
            Scale = MathHelper.Clamp(Scale - (brightness < 0.1f ? 0.08f : 0.008f), 0f, 1f);

            float dissipationFactor = Utils.GetLerpValue(6f, 16f, Velocity.Length(), true);
            float velocityInterpolant = Utils.GetLerpValue(25f - dissipationFactor * 10f, 80f - dissipationFactor * 45f, Time, true);
            Vector2 idealVelocity = new Vector2(Main.windSpeedCurrent * MathHelper.Lerp(0.8f, 1.2f, (float)Math.Sin(Center.Y / 50f + ID)) * 20f, (float)Math.Sin(Main.time / 20f + ID * 0.01f) * 3f - 1f);
            Velocity = Vector2.Lerp(Velocity, idealVelocity, velocityInterpolant * 0.16f);
            Center += Velocity;
        }

        public static void UpdateAll()
        {
            // Don't draw ashes serverside.
            if (Main.netMode == NetmodeID.Server)
                return;

            foreach (DeathAshParticle ash in Ashes)
                ash.Update();

            Ashes.RemoveWhere(a => a.Time >= a.Lifetime);
        }
    }
}
