using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChickenExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kentucky Fried Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 1000;
            Projectile.height = 1000;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.75f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.01f / 255f);
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                Projectile.localAI[0] += 1f;
            }

            EmitDust();
        }

        public void EmitDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 70; i++)
            {
                // The exponent being greater than 1 gives the randomness a bias towards 0. This means that more dust will spawn
                // closer to the center than the edge.
                Vector2 dustSpawnOffset = Main.rand.NextVector2Unit() * (float)Math.Pow(Main.rand.NextFloat(), 2.4D) * Projectile.Size * 0.5f;

                // Dust should fly off more quickly the farther away it is from the center.
                // At 5% out, a speed of 5 pixels/second is achieved. At 85%, a speed of 15 pixels/second is.
                // Direction is determined based on the outward direction rotated by anywhere from -90 to 90 degrees.
                Vector2 dustVelocity = dustSpawnOffset.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2 * Main.rand.NextFloatDirection());
                dustVelocity *= MathHelper.Lerp(5f, 15f, Utils.GetLerpValue(0.05f, 0.85f, (dustSpawnOffset / Projectile.Size / 0.5f).Length()));

                // Fire variants.
                int dustType = 6;
                if (Main.rand.NextBool(4))
                    dustType = 244;

                // Smoke.
                if (Main.rand.NextBool(7))
                    dustType = 31;

                Dust flame = Dust.NewDustPerfect(Projectile.Center + dustSpawnOffset, dustType, dustVelocity);
                flame.scale = Main.rand.NextFloat(0.85f, 1.3f);
                flame.noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.Size.Length() * 0.5f, targetHitbox);
    }
}
