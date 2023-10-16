using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MeowFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int Lifetime => 72;
        public bool PinkFire => Projectile.ai[0] == 1f;
        public ref float Time => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime; // 24 effectively
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            Time++;

            // Determines particle size as well as hitbox
            if (Time >= 6f)
                Projectile.scale = 1.5f * Utils.GetLerpValue(6f, 30f, Time, true);
            else
                return; // Helps position it at the tip
            
            // Main smokes composing of blue or pink
            float smokeRot = MathHelper.ToRadians(3f); // *Rate of rotation per frame, not a constant rotation
            Color smokeColor = Color.Lerp(PinkFire ? Color.DeepPink : Color.DodgerBlue, PinkFire ? Color.Pink : Color.PowderBlue, 0.5f + 0.3f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f));
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 12, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.25f, smokeRot, true, required: true);
            GeneralParticleHandler.SpawnParticle(smoke);

            // Overlay the glow on top, a bit brighter
            if (Main.rand.NextBool(8))
            {
                Color glowColor = Color.Lerp(smokeColor, Color.White, 0.2f);
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, glowColor, 8, Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f), 0.15f, smokeRot, true, 0.005f);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale * 0.2f);
        }

        // Circular hitbox adjusted for the size of the smoke particles (which is 52 here)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 52 * Projectile.scale * 0.5f, targetHitbox);
    }
}
