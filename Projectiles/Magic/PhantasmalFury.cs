using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class PhantasmalFury : ModProjectile
    {
    	int lightTimer = 10;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fury");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.magic = true;
        }

        public override void AI()
        {
        	lightTimer--;
        	if (lightTimer == 0)
        	{
        		if (projectile.owner == Main.myPlayer)
        		{
        			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("Phantom"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        		}
				lightTimer = 10;
        	}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f);
			for (int num457 = 0; num457 < 5; num457++)
			{
				int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num458].noGravity = true;
				Main.dust[num458].velocity *= 0.5f;
				Main.dust[num458].velocity += projectile.velocity * 0.1f;
			}
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 43);
        	for (int j = 0; j <= 10; j++)
        	{
        		Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 175, 0f, 0f, 100, default(Color), 1f);
        	}
        	float spread = 30f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y)- spread/2;
			double deltaAngle = spread/8f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer)
			{
				for (i = 0; i < 4; i++ )
				{
					offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("Phantom"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("Phantom"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
        }
    }
}
