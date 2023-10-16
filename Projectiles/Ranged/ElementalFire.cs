using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ElementalFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int Lifetime => 120;
        public ref float ColorType => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime; // 30 effectively
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Time++;
            ColorType += 0.02f;

            // Determines particle size as well as hitbox
            if (Time >= 5f)
                Projectile.scale = 1.8f * Utils.GetLerpValue(5f, 30f, Time, true);
            else
                return; // Helps position it at the tip
            
            // Rainbow smokes of increasing opacity
            float smokeRot = MathHelper.ToRadians(3f); // *Rate of rotation per frame, not a constant rotation
            float hue = 0.5f * (ColorType % 1f) + 0.5f * Utils.GetLerpValue(30f, Lifetime, Time, true) * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
            Color smokeColor = Main.hslToRgb(hue, 1f, 0.8f);
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 12, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.3f, smokeRot, true, required: true);
            GeneralParticleHandler.SpawnParticle(smoke);

            // Overlay the glow on top, which is on the brighter side
            if (Main.rand.NextBool(5))
            {
                Color glowColor = Color.Lerp(smokeColor, Color.White, 0.3f);
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, glowColor, 9, Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f), 0.2f, smokeRot, true, 0.005f);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale * 0.3f);
        }

        // Circular hitbox adjusted for the size of the smoke particles (which is 52 here)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 52 * Projectile.scale * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 540);

            // Circular spread of clouds on hit
            for (int i = 0; i < 12; i++)
            {
                Vector2 smokeVel = Main.rand.NextVector2Circular(16f, 16f);
                Color smokeColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);
                Particle smoke = new MediumMistParticle(Projectile.Center, smokeVel, smokeColor, Color.Black, Main.rand.NextFloat(0.6f, 1.6f), 200 - Main.rand.Next(60), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }
    }
}
