using System;
using System.Collections.Generic;
using CalamityMod.Dusts;
using CalamityMod.Effects;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBurnOrb : ModProjectile, ILocalizedModType
    {
        private static Asset<Texture2D> TrailNoiseTexture;
        private static Asset<Texture2D> TrailDistortionTexture;
        private static Asset<Texture2D> GlowOrbTexture;

        bool started = false;

        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TrailNoiseTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/HarshNoise");
                TrailDistortionTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2");
                GlowOrbTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowOrbParticle");
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.localAI[1] = Main.rand.NextFloat(30f);
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.MaxUpdates = 4;
        }

        public override void AI()
        {
            ProvUtils.ApplyGFBDamage(Projectile, 120, 50);

            Lighting.AddLight(Projectile.Center, 0.45f, 0.35f, 0f);

            if (!started)
            {
                Color cl = ProvUtils.GetProjectileColor(255);
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, cl, "CalamityMod/Particles/BlastCone", new Vector2(Main.rand.NextFloat(4f, 7f), 1.5f), Vector2.Zero.AngleTo(Projectile.velocity), 1f, 0f, 30));
                started = true;
            }

            if (Projectile.FinalExtraUpdate())
            {
                if (Projectile.ai[0] < 240f)
                {
                    Projectile.ai[0] += 1f;

                    if (Projectile.timeLeft < 160)
                        Projectile.timeLeft = 160;
                }

                if (!ProvUtils.StandardAI())
                {
                    if (Projectile.velocity.Length() < 12f && Projectile.ai[1] == 0f)
                    {
                        Projectile.velocity *= 1.02f;
                    }
                    else
                    {
                        Projectile.ai[1] += 0.075f;
                        Projectile.velocity *= MathHelper.Lerp(0.95f, 1.05f, MathF.Abs(MathF.Sin(Projectile.ai[1])));
                    }
                }
                else if (Projectile.velocity.Length() < 16f)
                    Projectile.velocity *= 1.01f;

                Projectile.localAI[1] += (Projectile.velocity.Length() / 20);
            }

            Projectile.position -= (Projectile.velocity / Projectile.MaxUpdates) * (Projectile.MaxUpdates - 1);
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Color particleColor = ProvUtils.GetProjectileColor(0);
            Color smokeColor = Color.Lerp(particleColor, Color.DarkSlateGray, 0.5f);
            Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, smokeColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0f, 0.06f, 18);
            GeneralParticleHandler.SpawnParticle(pulse);
            for (int i = 0; i < 7; i++)
            {
                Particle smoke = new HeavySmokeParticle(Projectile.Center, (Vector2.UnitX).RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(7f), smokeColor, 30, Main.rand.NextFloat(0.6f, 1f), 0.5f, Main.rand.NextFloat(-0.03f, 0.03f), true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LightDust>(), (Vector2.UnitX).RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(1.8f, 10f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(1f, 1.8f);
                dust.color = particleColor;
                dust.noLightEmittence = true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => Projectile.Kill();

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if (info.Damage <= 0 || target.creativeGodMode)
                return;

            ProvUtils.ApplyDebuffs(target, 120);
        }

        public float FireWidthFunction(float completion)
        {
            float width;
            float maxBodyWidth = 36f * Projectile.scale;
            float curveRatio = 0.2f;

            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 12f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 6f, pulseInterpolant);
            return (width + additionalPulseWidth);
        }

        public Color FireColorFunction(float completion)
        {
            Color mainColor = Color.Lerp(ProvUtils.GetProjectileColor(255, true) * 2f, ProvUtils.GetProjectileColor(255), 0.75f);
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion) * Projectile.Opacity;
        }

        public void DrawTrail()
        {
            if (Projectile.oldPos.Length <= 0)
                return;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -200f, 200f);
            Vector2 basePosition = Projectile.Center - Main.screenPosition;
            Rectangle screenBounds = new Rectangle(-40, -40, Main.screenWidth + 40, Main.screenHeight + 40);
            if (screenBounds.Contains(basePosition.ToPoint()))
            {
                List<Vector3> path = [];
                for (var i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 trailDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                    path.Add(new Vector3(trailDrawPosition.X, trailDrawPosition.Y, 0f));
                }

                if (path.Count > 2 && Projectile.ai[0] > 6)
                {
                    Effect shader = CalamityShaders.ProvidenceHolyOrbTrailShader.Value;
                    shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
                    shader.Parameters["glowPower"].SetValue(1.48f);

                    Main.graphics.GraphicsDevice.Textures[0] = TrailNoiseTexture.Value;
                    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                    Main.graphics.GraphicsDevice.Textures[1] = TrailDistortionTexture.Value;
                    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

                    using var shaderScope = SanePrimitiveRenderer.BeginShaderScope(shader, Matrix.Identity, Matrix.Identity, projection);
                    using var trailMesh = TriangleStripBuilder.BuildStripPooled(path, progress => FireWidthFunction(progress), progress => FireColorFunction(progress), PrimitiveMeshCache.Shared, textured: true, smoothingCurve: StripCurveType.Hermite);
                    shaderScope.Draw(trailMesh.View);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Trail drawn by the HolyBurnOrbDrawer system.
            DrawBurnOrb();
            DrawBurnOrbAdditive();
            return false;
        }

        private void DrawBurnOrb()
        {
            float lerpMult = MathHelper.Lerp(0.5f, 1.5f, Math.Abs(MathF.Sin(Projectile.localAI[1] / 10f)));

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color baseColor = ProvUtils.GetProjectileColor(255, true) * 4;
            Color baseColor2 = ProvUtils.GetProjectileColor(255);
            baseColor.A = 0;
            baseColor *= lerpMult;
            baseColor2 *= lerpMult;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 scale = new Vector2(0.5f, 1f) * ((lerpMult - 1) * 0.5f + 1f);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Projectile.rotation += MathHelper.ToRadians(lerpMult * 2f);

            float upRight = MathHelper.PiOver4;
            float up = MathHelper.PiOver2;
            float upLeft = 3f * MathHelper.PiOver4;
            float left = MathHelper.Pi;
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor, upLeft + Projectile.rotation, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor, upRight - Projectile.rotation, origin, scale, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, upLeft + Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, upRight - Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor, up + Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor, left - Projectile.rotation, origin, scale * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, up + Projectile.rotation, origin, scale * 0.36f, spriteEffects, 0);
            Main.EntitySpriteDraw(texture, drawPos, null, baseColor2, left - Projectile.rotation, origin, scale * 0.36f, spriteEffects, 0);
        }

        private void DrawBurnOrbAdditive()
        {
            Main.EntitySpriteDraw(GlowOrbTexture.Value, Projectile.Center - Main.screenPosition, null, ProvUtils.GetProjectileColor(255) with { A = 0 }, 0, GlowOrbTexture.Size() * 0.5f, 1f, 0, 0f);
            Main.EntitySpriteDraw(GlowOrbTexture.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, 0, GlowOrbTexture.Size() * 0.5f, 0.5f, 0, 0f);
        }
    }

    public class HolyBurnOrbDrawer : ModSystem
    {
        public static RenderTargetLease TrailPixelationLease { get; private set; }

        private List<Projectile> HolyBurnOrbs = [];
        private List<Projectile> HolyLights = [];

        public override void Load()
        {
            GeneralDrawLayerSystem.OnBeforeProjectiles += DrawOrbTrails;
            if (!Main.dedServ)
            {
                Main.QueueMainThreadAction(() => TrailPixelationLease = ScreenspaceTargetPool.Shared.Rent(Main.instance.GraphicsDevice, (w, h) => (w / 2, h / 2)));
            }
        }

        public override void PostUpdateProjectiles()
        {
            foreach (var item in Main.ActiveProjectiles)
            {
                if (item.type == ModContent.ProjectileType<HolyBurnOrb>() && !HolyBurnOrbs.Contains(item))
                    HolyBurnOrbs.Add(item);
                if (item.type == ModContent.ProjectileType<HolyLight>() && !HolyLights.Contains(item))
                    HolyLights.Add(item);
            }

            HolyBurnOrbs.RemoveAll(p => !p.active || p.timeLeft <= 1);
            HolyLights.RemoveAll(p => !p.active || p.timeLeft <= 1);
        }

        private void DrawOrbTrails()
        {
            if ((HolyBurnOrbs.Count + HolyLights.Count) > 0)
            {
                using (Main.spriteBatch.Scope())
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                    using (TrailPixelationLease.Scope(clearColor: Color.Transparent))
                    {
                         foreach (var item in HolyBurnOrbs)
                        {
                            if (item.ModProjectile is HolyBurnOrb orb)
                                orb.DrawTrail();
                        }
                         foreach (var item in HolyLights)
                        {
                            if (item.ModProjectile is HolyLight light)
                                light.DrawTrail();
                        }
                    }

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    // Redraw at double scale to pixelate the trails.
                    Main.spriteBatch.Draw(TrailPixelationLease.Target, Vector2.Zero, null, Color.White with { A = 0 }, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

                    Main.spriteBatch.End();
                }
            }
        }
    }
}
