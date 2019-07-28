using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ShadeNimbusCloud : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nimbus");
			Main.projFrames[projectile.type] = 4;
		}

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.netImportant = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
			float num410 = projectile.ai[0];
			float num411 = projectile.ai[1];
			if (num410 != 0f && num411 != 0f)
			{
				bool flag12 = false;
				bool flag13 = false;
				if ((projectile.velocity.X < 0f && projectile.Center.X < num410) || (projectile.velocity.X > 0f && projectile.Center.X > num410))
				{
					flag12 = true;
				}
				if ((projectile.velocity.Y < 0f && projectile.Center.Y < num411) || (projectile.velocity.Y > 0f && projectile.Center.Y > num411))
				{
					flag13 = true;
				}
				if (flag12 && flag13)
				{
					projectile.Kill();
				}
			}
			projectile.rotation += projectile.velocity.X * 0.02f;
			projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame > 3)
				{
					projectile.frame = 0;
					return;
				}
			}
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.owner == Main.myPlayer)
			{
            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ShadeNimbus"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
			}
        }
    }
}
