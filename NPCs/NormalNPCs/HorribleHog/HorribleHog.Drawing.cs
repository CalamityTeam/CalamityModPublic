using System;
using CalamityMod.Effects;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace CalamityMod.NPCs.NormalNPCs.HorribleHog
{
    public partial class HorribleHog
    {
        public static float MinScaryAuraOpacityDistance => 400f;
        public static float MaxScaryAuraOpacityDistance => 800f;

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                if (FrameY < IdleFrame)
                    FrameY = IdleFrame;

                NPC.frameCounter++;
                if (NPC.frameCounter >= 5)
                {
                    FrameY++;
                    if (FrameY > MaxFrame_Walking)
                        FrameY = MinFrame_Walking;
                    NPC.frameCounter = 0;
                }
            }

            NPC.frame.Y = FrameY * frameHeight;
        }

        public void Animate(int startingFrame, int endingFrame, int frameSpeed = 6, bool loop = true, int? loopStartingFrame = null, bool dynamicChanges = false)
        {
            bool jumping = dynamicChanges && NPC.velocity.Y != 0f;
            bool idling = dynamicChanges && MathF.Abs(NPC.velocity.X) < 0.06f && NPC.velocity.Y == 0f;
            if (jumping)
            {
                FrameY = JumpFrame;
            }
            else if (idling)
            {
                FrameY = IdleFrame;
            }
            else
            {
                if (FrameY < startingFrame)
                    FrameY = startingFrame;

                if (NPC.frameCounter % frameSpeed == 0)
                {
                    FrameY++;
                    if (FrameY > endingFrame)
                    {
                        if (loop)
                        {
                            if (loopStartingFrame.HasValue)
                                FrameY = loopStartingFrame.Value;
                            else
                                FrameY = startingFrame;
                        }
                        else
                        {
                            FrameY = endingFrame;
                        }
                    }
                }

                NPC.frameCounter += 1;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                Draw_ScaryEvilFuckedUpAura(spriteBatch, screenPos, true);
                return true;
            }

            Texture2D baseTexture = TextureAssets.Npc[Type].Value;
            Texture2D balledTexture = HorribleHog_BalledUp.Value;
            SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 scale = NPC.scale * SquashVector;

            float textureHeight = baseTexture.Height / Main.npcFrameCount[Type];
            float yOffset = scale.Y * textureHeight * 0.05f;
            Vector2 drawPosition = NPC.Center + new Vector2(0f, NPC.gfxOffY - yOffset) - screenPos;

            // Background effect when Horrible Hog is idling and its nearby loop is playing.
            if (DevilsTongueVolumeMultiplier > 0.05f && AIState != (int)BehaviorState.PiggyTransformation)
                Draw_ScaryEvilFuckedUpAura(spriteBatch, screenPos);

            // Horrible Hog and its afterimage trail.
            using (spriteBatch.Scope())
            {
                using var lease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight, RenderTargetDescriptor.Default);
                using (lease.Scope(clearColor: Color.Transparent))
                {
                    if (UseBalledSprite)
                    {
                        Rectangle frameRec = balledTexture.Frame();
                        Effect rotateSpriteShader = CalamityShaders.RotateSprite.Value;
                        rotateSpriteShader.Parameters["rotation"].SetValue(SpriteRotation);
                        rotateSpriteShader.Parameters["spriteDimensions"].SetValue(balledTexture.Size());
                        rotateSpriteShader.Parameters["spriteRectangle"].SetValue(new Vector4(frameRec.X, frameRec.Y, frameRec.Width, frameRec.Height));
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, rotateSpriteShader, Matrix.Identity);

                        if (CalamityClientConfig.Instance.Afterimages && AfterimageTrailOpacity > 0.05f)
                        {
                            for (int i = 0; i < NPC.oldPos.Length; i++)
                            {
                                Color afterimageColor = Color.Red * AfterimageTrailOpacity * 0.76f;
                                afterimageColor *= (float)(NPC.oldPos.Length - i) / (float)NPC.oldPos.Length;
                                Vector2 afterimageDrawPosition = NPC.oldPos[i] - Vector2.UnitY * yOffset + NPC.Size * 0.5f - screenPos;
                                spriteBatch.Draw(balledTexture, afterimageDrawPosition, null, NPC.GetAlpha(afterimageColor), NPC.rotation, balledTexture.Size() * 0.5f, scale, effects, 0f);
                            }
                        }

                        spriteBatch.Draw(balledTexture, drawPosition + Main.rand.NextVector2Circular(HorizontalShakeStrength, 0f), frameRec, NPC.GetAlpha(drawColor), NPC.rotation, balledTexture.Size() * 0.5f, scale, effects, 0f);
                        spriteBatch.End();
                    }
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);

                        if (CalamityClientConfig.Instance.Afterimages && AfterimageTrailOpacity > 0.05f)
                        {
                            for (int i = 0; i < NPC.oldPos.Length; i++)
                            {
                                Color afterimageColor = Color.Red * AfterimageTrailOpacity * 0.76f;
                                afterimageColor *= (float)(NPC.oldPos.Length - i) / (float)NPC.oldPos.Length;
                                Vector2 afterimageDrawPosition = NPC.oldPos[i] - Vector2.UnitY * yOffset + NPC.Size * 0.5f - screenPos;
                                spriteBatch.Draw(baseTexture, afterimageDrawPosition, NPC.frame, NPC.GetAlpha(afterimageColor), NPC.rotation, NPC.frame.Size() * 0.5f, scale, effects, 0f);
                            }
                        }

                        spriteBatch.Draw(baseTexture, drawPosition + Main.rand.NextVector2Circular(HorizontalShakeStrength, 0f), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, scale, effects, 0f);
                        spriteBatch.End();
                    }
                }

                Effect tintShader = CalamityShaders.BasicTintShader.Value;
                tintShader.Parameters["uColor"].SetValue(TintColor.ToVector3());
                tintShader.Parameters["uOpacity"].SetValue(TintStrength);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, tintShader, Main.GameViewMatrix.TransformationMatrix);
                spriteBatch.Draw(lease.Target, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            // Eye glint.
            if (EyeGlintScale > 0.05f)
            {
                Vector2 eyeGlintDrawPosition = drawPosition + new Vector2(18f * NPC.spriteDirection, -2f).RotatedBy(NPC.rotation) + Main.rand.NextVector2Circular(HorizontalShakeStrength, 0f);

                spriteBatch.SetBlendState(CalamityUtils.SubtractiveBlending);
                for (int i = 0; i < 2; i++)
                    spriteBatch.Draw(ShineFlare.Value, eyeGlintDrawPosition, null, NPC.GetAlpha(Color.White) * 0.7f, NPC.rotation, ShineFlare.Size() * 0.5f, EyeGlintScale, 0, 0f);
                spriteBatch.SetBlendState(BlendState.AlphaBlend);

                spriteBatch.Draw(ShineFlare.Value, eyeGlintDrawPosition, null, NPC.GetAlpha(Color.Red) with { A = 0 }, NPC.rotation, ShineFlare.Size() * 0.5f, EyeGlintScale * 0.8f, 0, 0f);
                spriteBatch.Draw(ShineFlare.Value, eyeGlintDrawPosition, null, NPC.GetAlpha(Color.White) with { A = 0 }, NPC.rotation, ShineFlare.Size() * 0.5f, EyeGlintScale * 0.4f, 0, 0f);
            }

            return false;
        }

        private void Draw_ScaryEvilFuckedUpAura(SpriteBatch spriteBatch, Vector2 screenPos, bool bestiary = false)
        {
            RasterizerState previousRasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Rectangle previousScissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;

            using (spriteBatch.Scope())
            {
                Vector2 drawPosition = NPC.Center - screenPos;
                float radiusBasedOpacity = Utils.Remap(Main.LocalPlayer.Distance(NPC.Center), MinScaryAuraOpacityDistance, MaxScaryAuraOpacityDistance, 1f, 0.15f, true);
                float generalEffectOpacity = bestiary ? MathHelper.Lerp(0.5f, 1f, MathF.Sin((float)Main.timeForVisualEffects / 75f) * 0.5f + 0.5f) : radiusBasedOpacity * DevilsTongueVolumeMultiplier;
                float generalEffectScale = bestiary ? 0.5f : 1f;
                Matrix transformatiomMatrix = bestiary ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix;

                Effect auraShader = CalamityShaders.HorribleHogAuraShader.Value;
                auraShader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                auraShader.Parameters["colorPaletteLimit"].SetValue(16f);
                auraShader.Parameters["spiralArms"].SetValue(5f);
                auraShader.Parameters["spiralAdditionalAngle"].SetValue(6f);
                auraShader.Parameters["minPixelFadeDistance"].SetValue(0.145f);
                auraShader.Parameters["maxPixelFadeDistance"].SetValue(0.485f);
                auraShader.Parameters["pixelationFactor"].SetValue(Main.ScreenSize.ToVector2() * 0.25f);
                auraShader.Parameters["spiralTimeOffset"].SetValue(new Vector2(-0.08f, -0.05f));
                auraShader.Parameters["vortexDarkColor"].SetValue(new Color(8, 8, 8).ToVector3());
                auraShader.Parameters["vortexBrightColor"].SetValue(Color.Crimson.ToVector3());

                Main.graphics.GraphicsDevice.Textures[1] = VortexTextureSecondary.Value;
                Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;

                Main.graphics.GraphicsDevice.Textures[2] = VortexDistortionTexture.Value;
                Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;

                // More accurate pixelation for the bestiary.
                if (bestiary)
                {
                    auraShader.Parameters["pixelationFactor"].SetValue(Main.ScreenSize.ToVector2());
                    Matrix pixelationMatrix = Matrix.CreateScale(0.5f, 0.5f, 1f);
                    using var pixelationLease = RenderTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, Main.screenWidth / 2, Main.screenHeight / 2, RenderTargetDescriptor.Default);
                    using (pixelationLease.Scope(clearColor: Color.Transparent))
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, auraShader, pixelationMatrix);

                        spriteBatch.Draw(VortexTexture.Value, drawPosition, null, Color.White * generalEffectOpacity, 0f, VortexTexture.Size() * 0.5f, 1f * generalEffectScale, 0, 0f);

                        spriteBatch.End();
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, pixelationMatrix);

                        float bloomCircleOpacity = MathHelper.Lerp(0.6f, 0.9f, MathF.Sin((float)Main.timeForVisualEffects / 75f + NPC.whoAmI) * 0.5f + 0.5f) * generalEffectOpacity;
                        spriteBatch.Draw(BloomCircle.Value, drawPosition, null, Color.Crimson with { A = 0 } * bloomCircleOpacity, 0f, BloomCircle.Size() * 0.5f, 1.2f * generalEffectScale, 0, 0f);

                        spriteBatch.End();
                    }

                    spriteBatch.GraphicsDevice.RasterizerState = previousRasterizerState;
                    spriteBatch.GraphicsDevice.ScissorRectangle = previousScissorRectangle;
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, previousRasterizerState, null, transformatiomMatrix);

                    spriteBatch.Draw(pixelationLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, 0, 0f);

                    spriteBatch.End();
                }
                else
                {
                    spriteBatch.GraphicsDevice.RasterizerState = previousRasterizerState;
                    spriteBatch.GraphicsDevice.ScissorRectangle = previousScissorRectangle;
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, previousRasterizerState, auraShader, transformatiomMatrix);

                    spriteBatch.Draw(VortexTexture.Value, drawPosition, null, Color.White * generalEffectOpacity, 0f, VortexTexture.Size() * 0.5f, 1f * generalEffectScale, 0, 0f);

                    spriteBatch.End();

                    spriteBatch.GraphicsDevice.RasterizerState = previousRasterizerState;
                    spriteBatch.GraphicsDevice.ScissorRectangle = previousScissorRectangle;
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, previousRasterizerState, null, transformatiomMatrix);

                    float bloomCircleOpacity = MathHelper.Lerp(0.6f, 0.9f, MathF.Sin((float)Main.timeForVisualEffects / 75f + NPC.whoAmI) * 0.5f + 0.5f) * generalEffectOpacity;
                    spriteBatch.Draw(BloomCircle.Value, drawPosition, null, Color.Crimson with { A = 0 } * bloomCircleOpacity, 0f, BloomCircle.Size() * 0.5f, 1.2f * generalEffectScale, 0, 0f);

                    spriteBatch.End();
                }
            }
        }

        private void SetSquashVectors(Vector2? squashVectorTarget = null, Vector2? squashVector = null)
        {
            SquashVectorTarget = squashVectorTarget ?? Vector2.One;
            if (squashVector.HasValue)
                SquashVector = squashVector.Value;
        }

        private void DoEyeGlintEffect(float scale)
        {
            EyeGlintScale = scale;
            float pitch = Utils.Remap(scale, 0.4f, 1f, 0.7f, 0.9f, true);
            float volume = Utils.Remap(scale, 0.4f, 1f, 1.3f, 1.6f, true);
            SoundEngine.PlaySound(SoundID.Item4 with { Volume = volume, Pitch = pitch }, NPC.Center);
        }
    }
}
