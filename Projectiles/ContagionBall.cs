using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ContagionBall : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Contagion Ball");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 60;
            projectile.ranged = true;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("HiveBombExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 60;
			projectile.height = 60;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num621 = 0; num621 < 2; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 44, 0f, 0f, 100, default(Color), 0.15f);
				Main.dust[num622].velocity *= 1.2f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 2; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 44, 0f, 0f, 100, default(Color), 0.35f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 1.5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 44, 0f, 0f, 100, default(Color), 0.15f);
				Main.dust[num624].velocity *= 1.1f;
			}
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 60;
			projectile.height = 60;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("Plague"), 360);
        }
    }
}