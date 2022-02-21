using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;
using System.Collections.Generic;

namespace CalamityMod.Projectiles.Melee
{
    public class ArkoftheCosmosConstellation : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public float Timer => projectile.ai[0] - projectile.timeLeft;
        const float ConstellationSwapTime = 15;

        public List<Particle> Particles = new List<Particle>();

        Vector2 AnchorStart => Owner.Center;
        Vector2 AnchorEnd => Owner.Calamity().mouseWorld;
        public Vector2 SizeVector => Utils.SafeNormalize(AnchorEnd - AnchorStart, Vector2.Zero) * MathHelper.Clamp((AnchorEnd - AnchorStart).Length(), 0, ArkoftheCosmos.MaxThrowReach);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Constellation");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.width = projectile.height = 8;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = (int)ConstellationSwapTime;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + SizeVector, 30f, ref collisionPoint);
        }

        public void BootlegSpawnParticle(Particle particle)
        {
            Particles.Add(particle);
            particle.Type = GeneralParticleHandler.particleTypes[particle.GetType()];
        }

        public override void AI()
        {
            projectile.Center = Owner.Center;

            if (!Owner.channel && projectile.timeLeft > 20)
                projectile.timeLeft = 20;

            if (!Owner.active)
            {
                projectile.Kill();
                return;
            }
                
            if (Timer % ConstellationSwapTime == 0 && projectile.timeLeft >= 20)
            {
                Particles.Clear();

                float constellationColorHue = Main.rand.NextFloat();
                Color constellationColor = Main.hslToRgb(constellationColorHue, 1, 0.8f);
                Vector2 previousStar = AnchorStart;
                Vector2 offset;
                Particle Line;
                Particle Star = new GenericSparkle(previousStar, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(Star);

                for (float i = 0 + Main.rand.NextFloat(0.2f, 0.5f); i < 1; i += Main.rand.NextFloat(0.2f, 0.5f))
                {
                    constellationColorHue = (constellationColorHue + 0.16f) % 1;
                    constellationColor = Main.hslToRgb(constellationColorHue, 1, 0.8f);

                    offset = Main.rand.NextFloat(-50f, 50f) * Utils.SafeNormalize(SizeVector.RotatedBy(MathHelper.PiOver2), Vector2.Zero);
                    Star = new GenericSparkle(AnchorStart + SizeVector * i + offset, Vector2.Zero, Color.White, constellationColor, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    BootlegSpawnParticle(Star);

                    Line = new BloomLineVFX(previousStar, AnchorStart + SizeVector * i + offset - previousStar, 0.8f, constellationColor * 0.75f, 20, true, true);
                    BootlegSpawnParticle(Line);

                    if (Main.rand.Next(3) == 0)
                    {
                        constellationColorHue = (constellationColorHue + 0.16f) % 1;
                        constellationColor = Main.hslToRgb(constellationColorHue, 1, 0.8f);

                        offset = Main.rand.NextFloat(-50f, 50f) * Utils.SafeNormalize(SizeVector.RotatedBy(MathHelper.PiOver2), Vector2.Zero);
                        Star = new GenericSparkle(AnchorStart + SizeVector * i + offset, Vector2.Zero, Color.White, constellationColor, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(Star);

                        Line = new BloomLineVFX(previousStar, AnchorStart + SizeVector * i + offset - previousStar, 0.8f, constellationColor, 20, true, true);
                        BootlegSpawnParticle(Line);
                    }

                    previousStar = AnchorStart + SizeVector * i + offset;
                }

                constellationColorHue = (constellationColorHue + 0.16f) % 1;
                constellationColor = Main.hslToRgb(constellationColorHue, 1, 0.8f);

                Star = new GenericSparkle(AnchorStart + SizeVector, Vector2.Zero, Color.White, constellationColor, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(Star);

                Line = new BloomLineVFX(previousStar, AnchorStart + SizeVector - previousStar, 0.8f, constellationColor * 0.75f, 20, true);
                BootlegSpawnParticle(Line);
            }

            //Run the particles manually to be sure it doesnt get fucked over by the particle cap
            Vector2 moveDirection = Vector2.Zero;
            if (Timer > projectile.oldPos.Length)
                moveDirection = projectile.position - projectile.oldPos[0];

            foreach (Particle particle in Particles)
            {
                if (particle == null)
                    continue;
                particle.Position += particle.Velocity + moveDirection;
                particle.Time++;
                particle.Update();

                particle.Color = Main.hslToRgb(Main.rgbToHsl(particle.Color).X + 0.02f, Main.rgbToHsl(particle.Color).Y, Main.rgbToHsl(particle.Color).Z);
            }

            Particles.RemoveAll(particle => (particle.Time >= particle.Lifetime && particle.SetLifetime));
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, null, null, Main.GameViewMatrix.ZoomMatrix);

            foreach (Particle particle in Particles)
            {
                particle.CustomDraw(spriteBatch);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}