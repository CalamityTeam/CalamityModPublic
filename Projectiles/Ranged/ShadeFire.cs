using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ShadeFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int Lifetime => 60;
        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = Lifetime; // 20 effectively
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Time++;

            // Determines particle size as well as hitbox
            if (Time >= 6f)
                Projectile.scale = 1.5f * Utils.GetLerpValue(6f, 36f, Time, true);
            else
                return; // Helps position it at the tip

            // Draw shadow smokes over the whole thing
            float smokeRot = MathHelper.ToRadians(3f); // *Rate of rotation per frame, not a constant rotation
            Color smokeColor = Color.Lerp(Color.BlueViolet, Color.Black, 0.7f + 0.2f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f));
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 20, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.8f, smokeRot, required: true);
            GeneralParticleHandler.SpawnParticle(smoke);

            // Overlay the glow on top, which is on the brighter side
            if (Main.rand.NextBool(5))
            {
                Color glowColor = Color.Lerp(smokeColor, Color.MidnightBlue, 0.25f);
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, glowColor, 15, Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f), 0.8f, smokeRot, true, 0.005f);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }
        }

        // Circular hitbox adjusted for the size of the smoke particles (which is 52 here)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 52 * Projectile.scale * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrainRot>(), 300);
    }
}
