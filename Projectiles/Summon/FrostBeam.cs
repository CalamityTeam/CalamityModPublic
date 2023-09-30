using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FrostBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.extraUpdates = 220;
            Projectile.timeLeft = 200;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 5f)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 spawnPosition = Projectile.position;
                    spawnPosition -= Projectile.velocity * i * 0.25f;
                    int idx = Dust.NewDust(spawnPosition, 1, 1, 113, 0f, 0f, 0, default, 1.25f);
                    Main.dust[idx].position = spawnPosition;
                    Main.dust[idx].scale = Main.rand.NextFloat(0.75f, 0.85f);
                    Main.dust[idx].velocity *= 0.1f;
                    Main.dust[idx].noGravity = true;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 70;
            Projectile.position -= Projectile.Size * 0.5f;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 3;
            Projectile.Damage();
            int flowerPetalCount = Main.rand.Next(3, 5 + 1);
            float thetaDelta = Projectile.velocity.ToRotation();
            float weaveDistanceMin = 2f;
            float weaveDistanceOutwardMax = 3f;
            float weaveDistanceInner = 0.5f;
            for (float theta = 0f; theta < MathHelper.TwoPi; theta += 0.05f)
            {
                Vector2 velocity = theta.ToRotationVector2() *
                    (weaveDistanceMin +
                    // The 0.5 in here is to prevent the petal from looping back into itself. With a 0.5 addition, it is perfect, coming back to (0,0)
                    // instead of weaving backwards.
                    (float)(Math.Sin(thetaDelta + theta * flowerPetalCount) + 0.5f + weaveDistanceInner) *
                    weaveDistanceOutwardMax);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 113, velocity);
                dust.noGravity = true;
                dust.scale = 1.35f;
            }
        }
    }
}
