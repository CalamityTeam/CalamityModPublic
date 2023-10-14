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

        public static int Lifetime => 120;
        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime; // 30 effectively
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
        }

        public override void AI()
        {
            Time++;

            // Determines particle size as well as hitbox
            if (Time >= 6f)
                Projectile.scale = 1.8f * Utils.GetLerpValue(6f, 30f, Time, true);
            else
                return; // Helps position it at the tip
            
            // Main smokes shifting between green to green-blue and back
            float smokeRot = MathHelper.ToRadians(3f); // *Rate of rotation per frame, not a constant rotation
            Color smokeColor = Color.Lerp(Color.SpringGreen, Color.Aquamarine, CalamityUtils.Convert01To010(Utils.GetLerpValue(30f, Lifetime, Time, true)));
            Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 12, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.8f, smokeRot, required: true);
            GeneralParticleHandler.SpawnParticle(smoke);

            // Overlay the glow on top, shifted toward yellow
            if (Main.rand.NextBool(5))
            {
                Color glowColor = Color.Lerp(smokeColor, Color.GreenYellow, 0.75f);
                Particle smokeGlow = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, glowColor, 9, Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f), 0.8f, smokeRot, true, 0.005f, true);
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale * 0.5f);
        }

        // Circular hitbox adjusted for the size of the smoke particles (which is 52 here)
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 52 * Projectile.scale * 0.5f, targetHitbox);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Circular spread of clouds on hit
            for (int i = 0; i < 12; i++)
            {
                Vector2 smokeVel = Main.rand.NextVector2Circular(16f, 16f);
                Color smokeColor = Color.Lerp(Color.SpringGreen, Color.Aquamarine, CalamityUtils.Convert01To010(Utils.GetLerpValue(30f, Lifetime, Time, true))) * 0.8f;
                Particle smoke = new MediumMistParticle(Projectile.Center, smokeVel, smokeColor, Color.Black, Main.rand.NextFloat(0.6f, 1.6f), 200 - Main.rand.Next(60), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }
    }
}
