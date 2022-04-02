using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class FrostBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.penetrate = 1;
            projectile.extraUpdates = 220;
            projectile.timeLeft = 200;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 5f)
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 spawnPosition = projectile.position;
                    spawnPosition -= projectile.velocity * i * 0.25f;
                    int idx = Dust.NewDust(spawnPosition, 1, 1, 113, 0f, 0f, 0, default, 1.25f);
                    Main.dust[idx].position = spawnPosition;
                    Main.dust[idx].scale = Main.rand.NextFloat(0.75f, 0.85f);
                    Main.dust[idx].velocity *= 0.1f;
                    Main.dust[idx].noGravity = true;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 70;
            projectile.position -= projectile.Size * 0.5f;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.damage /= 3;
            projectile.Damage();
            int flowerPetalCount = Main.rand.Next(3, 5 + 1);
            float thetaDelta = projectile.velocity.ToRotation();
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
                Dust dust = Dust.NewDustPerfect(projectile.Center, 113, velocity);
                dust.noGravity = true;
                dust.scale = 1.35f;
            }
        }
    }
}
