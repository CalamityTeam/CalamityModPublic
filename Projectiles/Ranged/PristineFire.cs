using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PristineFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Particles/MediumMist";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public float smokeOpa = 0.25f;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 118)
            {
                if (smokeOpa > 0)
                    smokeOpa -= 0.02f;

                Lighting.AddLight(Projectile.Center, 1f, 1f, 0.25f);

                // Light smoke
                Color smokeColor = new Color(255, Main.rand.Next(160, 230 + 1), 100, 100);
                Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(smokeOpa * 10) * (Main.rand.NextFloat(0.4f, 0.65f) - smokeOpa), smokeColor, 16, Main.rand.NextFloat(0.6f, 1.2f), 0.2f + smokeOpa, glowing: true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(50);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            LineParticle spark = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.23f, 0.56f)) * Main.rand.NextFloat(0.4f, 2.5f), false, 8, 1.2f, Main.rand.NextBool() ? Color.Orange : Color.DarkOrange);
            GeneralParticleHandler.SpawnParticle(spark);
            LineParticle spark2 = new LineParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.23f, -0.56f)) * Main.rand.NextFloat(0.4f, 2.5f), false, 8, 1.2f, Main.rand.NextBool() ? Color.Orange : Color.DarkOrange);
            GeneralParticleHandler.SpawnParticle(spark2);
            
            for (int i = 0; i <= 3; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? 169 : 158, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.5f), 0, default, Main.rand.NextFloat(1.6f, 2.2f));
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
            // Big poofy column
            for (int i = 0; i < 7; i++)
            {
                float velMulti = Main.rand.NextFloat(0.1f, 1.8f);
                Vector2 smokePos = Projectile.Center + Main.rand.NextVector2Circular(32f, 32f);
                Vector2 smokeVel = Vector2.UnitY * Main.rand.NextFloat(-12f, -8f) * velMulti;
                Particle smoke = new MediumMistParticle(smokePos, smokeVel, Main.rand.NextBool() ? Color.Orange : Color.DarkOrange, Color.Black, Main.rand.NextFloat(0.7f, 1.9f) - velMulti, 225 - Main.rand.Next(60), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public float HelixTrailWidthFunction(float completionRatio) => Utils.GetLerpValue(1f, 0.4f, completionRatio, true) * 1.5f;
        public Color HelixTrailColorFunction(float completionRatio) => Projectile.timeLeft < 105 ? Projectile.ai[0] == 1 ? Color.DarkOrange * (1f - completionRatio) : Color.Orange * (1f - completionRatio) : Color.Transparent;

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw a double helix trail
            for (int direction = -1; direction <= 1; direction += 2)
            {
                List<Vector2> trailPositions = new List<Vector2>();
                int trailPoints = ProjectileID.Sets.TrailCacheLength[Type];
                // Add points adjusted for sine
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                        break;

                    Vector2 sinOffset = (Vector2.UnitY * direction * MathF.Sin(i * MathHelper.Pi * 0.125f) * 24f).RotatedBy(Projectile.oldRot[i]);
                    trailPositions.Add(Projectile.oldPos[i] + sinOffset);
                }
                PrimitiveSet.Prepare(trailPositions, new(HelixTrailWidthFunction, HelixTrailColorFunction, (_) => Projectile.Size * 0.5f), 60);
            }
            return false;
        }
    }
}
