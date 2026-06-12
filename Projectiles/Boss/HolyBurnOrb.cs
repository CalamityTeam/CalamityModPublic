using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Serialization;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBurnOrb : ModProjectile, ILocalizedModType
    {
        bool started = false;
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
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

                if (Main.getGoodWorld)
                {
                    if (Projectile.velocity.Length() < 12f && Projectile.ai[1] == 0f)
                    {
                        Projectile.velocity *= 1.02f;
                    }
                    else
                    {
                        Projectile.ai[1] += 0.05f;
                        Projectile.velocity *= MathHelper.Lerp(0.95f, 1.05f, (float)Math.Abs(Math.Sin(Projectile.ai[1])));
                    }
                }
                else if (Projectile.velocity.Length() < 16f)
                    Projectile.velocity *= 1.01f;

                Projectile.localAI[1] += (Projectile.velocity.Length() / 20);
            }

            Projectile.position -= (Projectile.velocity / Projectile.MaxUpdates) * (Projectile.MaxUpdates - 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //drawn by the HolyBurnOrbDrawer system
            return false;
        }
        public float FireWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = 32f * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = ProvUtils.GetProjectileColor(255) * 1.3f;
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion) * Projectile.Opacity;
        }

        public float FireCoreWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = Projectile.scale * 24;
            float curveRatio = 0.25f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);

            if (completion < curveRatio)
                width = MathF.Sin(completion / curveRatio * MathHelper.PiOver2) * maxBodyWidth + curveRatio;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);
            return width * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireCoreColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = ProvUtils.GetProjectileColor(255);
            Color tipColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            Color fullBodyColor = Color.Lerp(mainColor, tipColor, completion);
            return Color.Lerp(fullBodyColor, Color.White, 0.175f) * Projectile.Opacity;
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
    }


    public class HolyBurnOrbDrawer : ModSystem
    {
        public override void Load()
        {
            On_Main.DrawProjectiles += DrawAll;
        }

        private void DrawAll(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            var device = Main.instance.GraphicsDevice;

            List<Projectile> burnToDraw = [];
            List<Projectile> healToDraw = [];


            foreach (var item in Main.ActiveProjectiles)
            {
                if (item.type == ModContent.ProjectileType<HolyBurnOrb>())
                    burnToDraw.Add(item);

                if (item.type == ModContent.ProjectileType<HolyLight>())
                    healToDraw.Add(item);
            }

            if ((healToDraw.Count() + burnToDraw.Count()) < 1)
                return;

            using (Main.spriteBatch.Scope())
            {

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                //Burn trails
                using (ProvUtils.PrimaryLease.Scope(clearColor: Color.Transparent))
                {
                    foreach (var item in burnToDraw)
                        DrawBurnOrbTrail(item);

                }
                Main.spriteBatch.Draw(ProvUtils.PrimaryLease.Target, Vector2.Zero, null, Color.White * 0.75f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                //Heal trail
                using (ProvUtils.SecondaryLease.Scope(clearColor: Color.Transparent))
                {
                    foreach (var item in healToDraw)
                        DrawHealOrbTrail(item);

                }
                Main.spriteBatch.Draw(ProvUtils.SecondaryLease.Target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                //Main orb
                foreach (var item in burnToDraw)
                    DrawBurnOrb(item);
                foreach (var item in healToDraw)
                    DrawHealOrb(item);

                //Additive orb
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var item in burnToDraw)
                    DrawBurnOrbAdditive(item);
                foreach (var item in healToDraw)
                    DrawHealOrbAdditive(item);
                Main.spriteBatch.End();
            }
        }

        void DrawBurnOrbTrail(Projectile Projectile)
        {
            if (Projectile.ModProjectile is HolyBurnOrb M)
            {
                var tex = GeneralParticleHandler.GetTexture(GeneralParticleHandler.particleIDsByTypes[typeof(BloomParticle)]);
                var len = Math.Min(Projectile.oldPos.Length, 32);
                for (var i = 0; i < len; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    Color color = M.FireColorFunction(i / (float)len, Projectile.oldPos[i]);
                    var color2 = ProvUtils.GetProjectileColor(255, true);
                    float width = M.FireWidthFunction(i / (float)len, Projectile.oldPos[i]);
                    Main.spriteBatch.Draw(tex, drawPos, null, (color2 with { A = 0 }) * (1 - i / (float)len), 0f, tex.Size() * 0.5f, new Vector2(width) / tex.Width, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(tex, drawPos, null, (color with { A = 0 }) * (1 - i / (float)len), 0f, tex.Size() * 0.5f, new Vector2(width) / tex.Width * 0.25f, SpriteEffects.None, 0f);
                }
            }
        }
        void DrawBurnOrb(Projectile Projectile)
        {
                float lerpMult = MathHelper.Lerp(0.5f, 1.5f, Math.Abs(MathF.Sin(Projectile.localAI[1] / 10f)));

                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color baseColor = ProvUtils.GetProjectileColor(255, true) * 4;
                Color baseColor2 = ProvUtils.GetProjectileColor(255);
                baseColor.A = 0;
                baseColor *= lerpMult;
                baseColor2 *= lerpMult;
                Vector2 origin = texture.Size() / 2f;
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

        void DrawBurnOrbAdditive(Projectile Projectile)
        {
            var texture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowOrbParticle").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, ProvUtils.GetProjectileColor(255), 0, texture.Size() * 0.5f, 1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, texture.Size() * 0.5f, 0.5f, 0, 0f);
        }
        void DrawHealOrbTrail(Projectile Projectile)
        {
            if (Projectile.ModProjectile is HolyLight M)
            {
                var tex = GeneralParticleHandler.GetTexture(GeneralParticleHandler.particleIDsByTypes[typeof(BloomParticle)]);
                var len = Math.Min(Projectile.oldPos.Length, 32);
                for (var i = 0; i < len; i++)
                {
                    Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    Color color = M.FireCoreColorFunction(i / (float)len, Projectile.oldPos[i]);
                    var color2 = new Color(0, 25, 0);
                    float width = M.FireWidthFunction(i / (float)len, Projectile.oldPos[i]);
                    Main.spriteBatch.Draw(tex, drawPos, null, (color2 with { A = 0 }) * (1 - i / (float)len), 0f, tex.Size() * 0.5f, new Vector2(width) / tex.Width, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(tex, drawPos, null, (color with { A = 0 }) * (1 - i / (float)len), 0f, tex.Size() * 0.5f, new Vector2(width) / tex.Width * 0.25f, SpriteEffects.None, 0f);
                }
            }
        }
        void DrawHealOrb(Projectile Projectile)
        {
            float vel = Projectile.velocity.Length() / 8;
            Projectile.localAI[1] += vel;

            Texture2D drawTexture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Color brightGreen = new Color(54, 209, 54, 0);
            Vector2 projDirection = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Vector2 halfTextureSize = drawTexture.Size() / 2f;
            Color halfBrightGreen = brightGreen * 0.5f;
            float timeLeftColorScale = MathHelper.Lerp(0.5f, 1.5f, Math.Abs(MathF.Sin(Projectile.localAI[1] / 10f)));
            Projectile.rotation += MathHelper.ToRadians(timeLeftColorScale * 2f);
            Vector2 timeLeftDrawEffect = new Vector2(0.5f, 1f) * timeLeftColorScale;
            Vector2 timeLeftDrawEffect2 = new Vector2(0.5f, 1f) * timeLeftColorScale;
            brightGreen *= timeLeftColorScale;
            halfBrightGreen *= timeLeftColorScale;

            Vector2 position3 = projDirection + Projectile.velocity.SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(0.5f, 1f, Projectile.localAI[0] / 60f, clamped: true) * 0;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, MathHelper.PiOver2 - Projectile.rotation, halfTextureSize, timeLeftDrawEffect, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, 0f - Projectile.rotation, halfTextureSize, timeLeftDrawEffect2, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, MathHelper.PiOver2 - Projectile.rotation, halfTextureSize, timeLeftDrawEffect * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, 0f - Projectile.rotation, halfTextureSize, timeLeftDrawEffect2 * 0.6f, spriteEffects, 0);

            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, MathHelper.PiOver4 + Projectile.rotation, halfTextureSize, timeLeftDrawEffect * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, MathHelper.PiOver4 * 3f + Projectile.rotation, halfTextureSize, timeLeftDrawEffect2 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, MathHelper.PiOver4 + Projectile.rotation, halfTextureSize, timeLeftDrawEffect * 0.36f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, MathHelper.PiOver4 * 3f + Projectile.rotation, halfTextureSize, timeLeftDrawEffect2 * 0.36f, spriteEffects, 0);
        }

        void DrawHealOrbAdditive(Projectile Projectile)
        {
            var texture = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowOrbParticle").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(54, 209, 54), 0, texture.Size() * 0.5f, 1f, 0, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, texture.Size() * 0.5f, 0.5f, 0, 0f);
        }
    }
}
