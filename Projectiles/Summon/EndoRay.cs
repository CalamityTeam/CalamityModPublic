using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class EndoRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ray");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 10;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 265;
            projectile.coldDamage = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 100;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 5f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = projectile.position;
                    spawnPosition -= projectile.velocity * i * 0.25f;
                    int idx = Dust.NewDust(spawnPosition, 1, 1, 113, 0f, 0f, 0, default, 1.25f);
                    Main.dust[idx].position = spawnPosition;
                    Main.dust[idx].scale = Main.rand.NextFloat(0.71f, 0.93f);
                    Main.dust[idx].velocity *= 0.1f;
                    Main.dust[idx].noGravity = true;
                }
            }
        }
    }
}
