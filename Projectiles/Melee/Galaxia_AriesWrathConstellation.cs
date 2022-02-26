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
    public class AriesWrathConstellation : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[projectile.owner];
        public ref float Timer => ref projectile.ai[0];

        public List<Particle> Particles = new List<Particle>();

        const float ConstellationSwapTime = 15;

        Vector2 PreviousEnd = Vector2.Zero;

        Vector2 AnchorStart => Owner.Center;
        Vector2 AnchorEnd => Owner.Calamity().mouseWorld;
        public Vector2 SizeVector => Utils.SafeNormalize(AnchorEnd - AnchorStart, Vector2.Zero) * MathHelper.Clamp((AnchorEnd - AnchorStart).Length(), 0, FourSeasonsGalaxia.AriesAttunement_Reach);

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
            projectile.localNPCHitCooldown = 30;
            projectile.timeLeft = 20;
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

            if (Owner.channel)
                projectile.timeLeft = 20;

            if (!Owner.active)
            {
                projectile.Kill();
                return;
            }

            if (Timer % ConstellationSwapTime == 0 && projectile.timeLeft >= 20)
            {
                Particles.Clear();

                PreviousEnd = Owner.Calamity().mouseWorld - Owner.Center;
                projectile.ai[0] = 1;
                Vector2 previousStar = projectile.Center;
                Vector2 offset;
                Particle Line;
                Particle Star = new GenericSparkle(previousStar, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(Star);

                for (float i = 0 + Main.rand.NextFloat(0.2f, 0.5f); i < 1; i += Main.rand.NextFloat(0.2f, 0.5f))
                {
                    offset = Main.rand.NextFloat(-50f, 50f) * Utils.SafeNormalize(SizeVector.RotatedBy(MathHelper.PiOver2), Vector2.Zero);
                    Star = new GenericSparkle(projectile.Center + SizeVector * i + offset, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                    BootlegSpawnParticle(Star);

                    Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector * i + offset - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true, true);
                    BootlegSpawnParticle(Line);

                    if (Main.rand.Next(3) == 0)
                    {
                        offset = Main.rand.NextFloat(-50f, 50f) * Utils.SafeNormalize(SizeVector.RotatedBy(MathHelper.PiOver2), Vector2.Zero);
                        Star = new GenericSparkle(projectile.Center + SizeVector * i + offset, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                        BootlegSpawnParticle(Star);

                        Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector * i + offset - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true, true);
                        BootlegSpawnParticle(Line);
                    }

                    previousStar = projectile.Center + SizeVector * i + offset;
                }

                Star = new GenericSparkle(projectile.Center + SizeVector, Vector2.Zero, Color.White, Color.Plum, Main.rand.NextFloat(1f, 1.5f), 20, 0f, 3f);
                BootlegSpawnParticle(Star);

                Line = new BloomLineVFX(previousStar, projectile.Center + SizeVector - previousStar, 0.8f, Color.MediumVioletRed * 0.75f, 20, true);
                BootlegSpawnParticle(Line);

            }

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
            }

            Particles.RemoveAll(particle => (particle.Time >= particle.Lifetime && particle.SetLifetime));
            Timer++;

            //Reset the constellation if the mouse goes too far
            if ((Owner.Calamity().mouseWorld - Owner.Center - PreviousEnd).Length() > 120)
                Timer = ConstellationSwapTime;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.EnterShaderRegion(BlendState.Additive);

            foreach (Particle particle in Particles)
            {
                particle.CustomDraw(spriteBatch);
            }

            spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}