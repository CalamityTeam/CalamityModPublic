using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
	public class ScorchedEarthClusterBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cluster Bomb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 120;
			projectile.ignoreWater = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -2;
		}

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
			projectile.velocity *= 0.95f;
			if (projectile.velocity.Length() < 0.5f && projectile.timeLeft > 10)
				projectile.timeLeft = 10;
		}

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int i = 0; i < 10; i++)
            {
				Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, default, 1f);
            }
            int projAmt = Main.rand.Next(2, 4);
            if (projectile.owner == Main.myPlayer)
            {
                for (int k = 0; k < projAmt; k++)
                {
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<RocketFire>(), projectile.damage, 0f, projectile.owner);
                }
            }
        }
    }
}
