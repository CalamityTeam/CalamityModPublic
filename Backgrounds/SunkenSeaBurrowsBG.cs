using CalamityMod.Graphics;
using CalamityMod.Systems.Graphic;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class SunkenSeaBurrowsBG : ModSystem
    {
        private static RenderTargetLease WaterDistortionTarget;

        private static bool CurrentlyRendering { get; set; }

        public override void Load()
        {
            if (Main.dedServ)
            {
                return;
            }

            GeneralDrawLayerSystem.OnPrepareDraw += DrawToTarget;
            On_Main.DrawBackgroundBlackFill += On_Main_DrawBackgroundBlackFill;

            Main.QueueMainThreadAction(() => WaterDistortionTarget = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice));
        }

        public override void Unload()
        {
            GeneralDrawLayerSystem.OnPrepareDraw -= DrawToTarget;
        }

        /// <summary>
        /// This code runs because there is a system in vanilla that causes backgrounds to not draw if lighting is too low.
        /// That would be fine, the problem arises because sloped tiles still block light the same way normal tiles do.
        /// Which means that the background will occasionally be blocked by slopes as though said slopes were full squares.
        /// This is a hacky method that sets the light level of every sloped tile *AT LEAST* high enough to render the background while in the Burrows.
        /// 
        /// ENNWAY's note: 
        /// if this doesn't get fixed in 1.4.5 i am throwing really big rocks at unsuspecting re logic employees
        /// </summary>
        public override void PreUpdatePlayers()
        {
            if (!Main.dedServ && Main.LocalPlayer.InModBiome<BiomeManagers.GleamingBurrowsBiome>())
            {
                int drawLimitX = Main.screenWidth / 16;
                int drawLimitY = Main.screenHeight / 16;
                Point drawPoint = (Main.screenPosition / 16).ToPoint();
                for (int i = 0; i < drawLimitX; i++)
                {
                    for (int j = 0; j < drawLimitY; j++)
                    {
                        Point pos = drawPoint + new Point(i, j);
                        if (!Main.tile[pos.X, pos.Y].HasTile &&
                            Main.tile[pos.X, pos.Y].WallType == WallID.None)
                            Lighting.AddLight(pos.X, pos.Y, TorchID.White, 0.1f);
                    }
                }
            }
            if (!Main.dedServ && (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.TimelessShoresBiome>())))
            {
                int drawLimitX = Main.screenWidth / 16;
                int drawLimitY = Main.screenHeight / 16;
                Point drawPoint = (Main.screenPosition / 16).ToPoint();
                for (int i = 0; i < drawLimitX; i++)
                {
                    for (int j = 0; j < drawLimitY; j++)
                    {
                        Point pos = drawPoint + new Point(i, j);
                        if (!Main.tile[pos.X, pos.Y].HasTile &&
                            Main.tile[pos.X, pos.Y].WallType == WallID.None &&
                            Main.tile[pos.X, pos.Y].LiquidAmount == 0)
                            Lighting.AddLight(pos.X, pos.Y, TorchID.White, 0.2f);
                    }
                }
            }
            if (!Main.dedServ && (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.PolypForestBiome>())))
            {
                int drawLimitX = Main.screenWidth / 16;
                int drawLimitY = Main.screenHeight / 16;
                Point drawPoint = (Main.screenPosition / 16).ToPoint();
                for (int i = 0; i < drawLimitX; i++)
                {
                    for (int j = 0; j < drawLimitY; j++)
                    {
                        Point pos = drawPoint + new Point(i, j);
                        if (!Main.tile[pos.X, pos.Y].HasTile &&
                        Main.tile[pos.X, pos.Y].WallType == WallID.None)
                            Lighting.AddLight(pos.X, pos.Y, TorchID.White, 0.2f);
                    }
                }
            }
            if (!Main.dedServ && (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.RadiantReefsBiome>())))
            {
                int drawLimitX = Main.screenWidth / 16;
                int drawLimitY = Main.screenHeight / 16;
                Point drawPoint = (Main.screenPosition / 16).ToPoint();
                for (int i = 0; i < drawLimitX; i++)
                {
                    for (int j = 0; j < drawLimitY; j++)
                    {
                        Point pos = drawPoint + new Point(i, j);
                        if (!Main.tile[pos.X, pos.Y].HasTile &&
                            Main.tile[pos.X, pos.Y].WallType == WallID.None)
                            Lighting.AddLight(pos.X, pos.Y, TorchID.White, 0.2f);
                    }
                }
            }
            if (!Main.dedServ && (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.BasaltGullyBiome>())))
            {
                int drawLimitX = Main.screenWidth / 16;
                int drawLimitY = Main.screenHeight / 16;
                Point drawPoint = (Main.screenPosition / 16).ToPoint();
                for (int i = 0; i < drawLimitX; i++)
                {
                    for (int j = 0; j < drawLimitY; j++)
                    {
                        Point pos = drawPoint + new Point(i, j);
                        if (pos.Y >= Main.maxTilesY - 450)
                        {
                            if (!Main.tile[pos.X, pos.Y].HasTile &&
                                Main.tile[pos.X, pos.Y].WallType == WallID.None)
                                Lighting.AddLight(pos.X, pos.Y, TorchID.Red, 0.6f);
                        }
                    }
                }
            }
        }

        private void DrawToTarget()
        {
            if (Main.gameMenu)
            {
                return;
            }

            CurrentlyRendering = true;

            using (WaterDistortionTarget.Scope(clearColor: Color.Transparent))
            {
                // 13MAY2025: fryzahh: Note that when other Sunken Sea backgrounds are implemented they should use this same system.
                // Leaving this here for other programmers, in case I don't get to doing this myself.
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, CalamityUtils.BackgroundMatrix);

                DrawShoresBG();
                DrawBurrowsBG();

                Main.spriteBatch.End();
            }

            CurrentlyRendering = false;
        }

        private void On_Main_DrawBackgroundBlackFill(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            if (Main.gameMenu || Main.screenPosition.Y + Main.screenHeight < ((int)Main.worldSurface) * 16f)
            {
                orig(self);
                return;
            }

            orig(self);

            // This won't render if the Wave Quality setting isn't turned off unfortunately, so don't bother running any of the shader
            // rendering code if that setting is on.
            if (Main.WaveQuality > 0 || !CalamityClientConfig.Instance.SunkenSeaBackgroundDistortion)
            {
                DrawShoresBG();
                DrawBurrowsBG();
            }
            else
            {
                MiscShaderData distortionShader = GameShaders.Misc["CalamityMod:BasicTextureDistortion"];
                Asset<Texture2D> distortionTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Swirls");

                // Apply a distortion shader to the Sunken Sea backgrounds to give them an underwater ripple-like effect.
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, distortionShader.Shader, CalamityUtils.BackgroundMatrix);

                float distortionXSpeed = -0.012f;
                float distortionYSpeed = 0.02f;
                float noiseScale = 0.075f;
                float noiseStrength = 0.035f;

                distortionShader.Shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                distortionShader.Shader.Parameters["noiseScale"].SetValue(noiseScale);
                distortionShader.Shader.Parameters["distortionStrength"].SetValue(noiseStrength);
                distortionShader.Shader.Parameters["timeOffset"].SetValue(new Vector2(distortionXSpeed, distortionYSpeed));
                Main.graphics.GraphicsDevice.Textures[1] = distortionTexture.Value;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

                Main.spriteBatch.Draw(WaterDistortionTarget.Target, new Rectangle(16, 16, Main.screenWidth, Main.screenHeight), Color.White);
            }
        }

        public static float Transparency;
        public static float TransitionSpeed => 0.02f;

        private static void DrawShoresBG()
        {
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.TimelessShoresBiome>()))
            {
                Transparency += 1f;

                if (Transparency > 1f)
                    Transparency = 1f;
            }
            else
            {
                // Make transparency immediately go down so it doesnt look weird, since vanilla underground backgrounds dont have the fade-in effect this background has.
                Transparency -= 1f;

                if (Transparency < 0f)
                    Transparency = 0f;
            }

            // Don't bother running any of the background drawing if the transparency is zero (meaning the background isnt actually active).
            // Also do not run any of the background drawing if you have the vanilla background config option turned off.
            if (Transparency > 0f && Main.BackgroundEnabled)
            {
                Vector2 vector = Main.screenPosition + new Vector2((Main.screenWidth >> 1), (Main.screenHeight >> 1));
                float num = (Main.GameViewMatrix.Zoom.Y - 1f) * 0.5f * 200f;
                float Scale = 1.5f;
                float playerDrawPosition = ((Main.LocalPlayer.Center.Y / 16f) - 90) * 16f;

                for (int Layers = 4; Layers >= 0; Layers--)
                {
                    // Get each background texture.
                    Texture2D BGTexture = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/SunkenSeaShoresBG" + Layers).Value;

                    Vector2 vector2 = new Vector2(BGTexture.Width, BGTexture.Height) * 0.5f;
                    float num2 = (Layers * 2 + 3f);
                    Vector2 vector3 = new Vector2(1f / num2);
                    Rectangle rectangle = new Rectangle(0, 0, BGTexture.Width, BGTexture.Height);
                    Vector2 zero = Vector2.Zero;

                    switch (Layers)
                    {
                        case 0:
                            {
                                zero.Y += 200f;
                                break;
                            }
                        case 1:
                            {
                                zero.Y += 0f;
                                break;
                            }
                        case 2:
                            {
                                zero.Y += 0f;
                                break;
                            }
                        case 3:
                            {
                                zero.Y += 0f;
                                break;
                            }
                        case 4:
                            {
                                zero.Y += 0f;
                                break;
                            }
                    }

                    vector2 *= Scale;

                    zero.Y -= num;
                    float LoopWidth = Scale * rectangle.Width;
                    float LoopHeight = Scale * rectangle.Height;
                    int LoopX = (int)((vector.X * vector3.X - vector2.X + zero.X - (Main.screenWidth >> 1)) / LoopWidth);
                    int LoopY = (int)((vector.Y * vector3.Y - vector2.Y + zero.Y - (Main.screenWidth >> 1)) / LoopWidth);

                    for (int i = LoopY - 2; i < LoopY + 4 + (int)(Main.screenWidth / LoopHeight); i++)
                    {
                        for (int j = LoopX - 2; j < LoopX + 4 + (int)(Main.screenWidth / LoopWidth); j++)
                        {
                            Vector2 drawPosition = (new Vector2(j * Scale * (rectangle.Width / vector3.X), playerDrawPosition) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2 + zero;

                            var frame = rectangle;
                            var color = Color.White * Transparency;

                            Main.spriteBatch.Draw(BGTexture, drawPosition, frame, color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }
        private static void DrawBurrowsBG()
        {
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.GleamingBurrowsBiome>()))
            {
                Transparency += 1f;

                if (Transparency > 1f)
                    Transparency = 1f;
            }
            else
            {
                // Make transparency immediately go down so it doesnt look weird, since vanilla underground backgrounds dont have the fade-in effect this background has.
                Transparency -= 1f;

                if (Transparency < 0f)
                    Transparency = 0f;
            }

            // Don't bother running any of the background drawing if the transparency is zero (meaning the background isnt actually active).
            // Also do not run any of the background drawing if you have the vanilla background config option turned off.
            if (Transparency > 0f && Main.BackgroundEnabled)
            {
                Vector2 vector = Main.screenPosition + new Vector2((Main.screenWidth >> 1), (Main.screenHeight >> 1));
                float num = (Main.GameViewMatrix.Zoom.Y - 1f) * 0.5f * 200f;
                float Scale = 1.5f;
                float playerDrawPosition = ((Main.LocalPlayer.Center.Y / 16f) - 90) * 16f;

                for (int Layers = 4; Layers >= 0; Layers--)
                {
                    // Get each background texture.
                    Texture2D BGTexture = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/SunkenSeaBurrowsBG" + Layers).Value;

                    Vector2 vector2 = new Vector2(BGTexture.Width, BGTexture.Height) * 0.5f;
                    float num2 = (Layers * 2 + 3f);
                    Vector2 vector3 = new Vector2(1f / num2);
                    Rectangle rectangle = new Rectangle(0, 0, BGTexture.Width, BGTexture.Height);
                    Vector2 zero = Vector2.Zero;

                    switch (Layers)
                    {
                        case 0:
                            {
                                zero.Y += 800f;
                                break;
                            }
                        case 1:
                            {
                                zero.Y += 200f;
                                break;
                            }
                        case 2:
                            {
                                zero.Y += 450f;
                                break;
                            }
                        case 3:
                            {
                                zero.Y += 45f;
                                break;
                            }
                        case 4:
                            {
                                zero.Y += 45f;
                                break;
                            }
                    }

                    vector2 *= Scale;

                    zero.Y -= num;
                    float LoopWidth = Scale * rectangle.Width;
                    float LoopHeight = Scale * rectangle.Height;
                    int LoopX = (int)((vector.X * vector3.X - vector2.X + zero.X - (Main.screenWidth >> 1)) / LoopWidth);
                    int LoopY = (int)((vector.Y * vector3.Y - vector2.Y + zero.Y - (Main.screenWidth >> 1)) / LoopWidth);

                    for (int i = LoopY - 2; i < LoopY + 4 + (int)(Main.screenWidth / LoopHeight); i++)
                    {
                        for (int j = LoopX - 2; j < LoopX + 4 + (int)(Main.screenWidth / LoopWidth); j++)
                        {
                            Vector2 drawPosition = (new Vector2(j * Scale * (rectangle.Width / vector3.X), playerDrawPosition) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2 + zero;

                            var frame = rectangle;
                            var color = Color.White * Transparency;

                            Main.spriteBatch.Draw(BGTexture, drawPosition, frame, color, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }
    }
}
