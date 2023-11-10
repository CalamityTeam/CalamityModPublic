using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int Lifetime => 96;
        public bool BlueFire => Projectile.ai[0] == 1f;
        public ref float Time => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = Lifetime; // 24 effectively
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Time++;

            if (Time == 1)
            {
                MediumMistParticle smoke2 = new MediumMistParticle(Projectile.Center + Main.rand.NextVector2Circular(5, 5), (Projectile.velocity * 1f).RotatedByRandom(0.15f) * Main.rand.NextFloat(0.2f, 2.1f), Color.Lime, Color.Turquoise, Main.rand.NextFloat(0.8f, 1.9f), 180, Main.rand.NextFloat(-3f, 3f));
                GeneralParticleHandler.SpawnParticle(smoke2);
                MediumMistParticle smoke3 = new MediumMistParticle(Projectile.Center + Main.rand.NextVector2Circular(5, 5), (Projectile.velocity * 2).RotatedByRandom(0.05f) * Main.rand.NextFloat(0.2f, 2.1f), Color.Lime, Color.Turquoise, Main.rand.NextFloat(0.4f, 1.1f), 180, Main.rand.NextFloat(-3f, 3f));
                GeneralParticleHandler.SpawnParticle(smoke3);
                for (int i = 0; i < 9; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(5) ? 135 : 107);
                    dust2.noGravity = true;
                    dust2.velocity = Projectile.velocity.RotatedByRandom(0.7f) * Main.rand.NextFloat(0.3f, 2.9f);
                    dust2.scale = Main.rand.NextFloat(1.3f, 2.1f);
                }
            }
            // Determines particle size as well as hitbox
            if (Time >= 1f)
                Projectile.scale = 1.8f * Utils.GetLerpValue(6f, 30f, Time, true);
            else
                return; // Helps position it at the tip
            
            // Main smokes shifting between green to green-blue and back
            float smokeRot = MathHelper.ToRadians(3f); // *Rate of rotation per frame, not a constant rotation
            float colorValue = CalamityUtils.Convert01To010(Utils.GetLerpValue(30f, Lifetime, Time, true));
            Color smokeColor = Color.Lerp(Color.Lime, Color.Turquoise, colorValue);
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 18, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.4f, smokeRot, true, required: true);
            GeneralParticleHandler.SpawnParticle(smoke);
            
            if (Time > 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    float dustArea = Main.rand.NextFloat(0.1f, 1.7f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9, 9) + Projectile.velocity * Main.rand.NextFloat(-1.8f, 1.8f), Main.rand.NextBool(5) ? 135 : 107);
                    dust.noGravity = true;
                    dust.velocity = new Vector2(6, 6).RotatedByRandom(100) * dustArea;
                    dust.scale = (1.8f - dustArea) * 0.65f;
                }
            }

            // Overlay a gold glow on top
            if (Main.rand.NextBool(5))
            {
                Color glowColor = Color.Gold;
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, glowColor, 9, Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f), 0.2f, smokeRot, true, 0.005f);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale * 0.3f);
        }

        // Circular hitbox adjusted for the size of the smoke particles (which is 52 here)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 52 * Projectile.scale * 0.5f, targetHitbox);
    }
}
