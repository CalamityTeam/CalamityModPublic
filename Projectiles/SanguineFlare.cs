using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class SanguineFlare : ModProjectile
    {
    	public int x;
    	public double speed = 10;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flare");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.alpha = 255;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.localAI[0] += 1f;
        	if (projectile.localAI[0] >= 180f)
        	{
	        	x++;
	        	speed += 0.1;
	        	projectile.velocity.Y = (float)(speed * Math.Sin(x/4));
        	}
        	projectile.rotation += projectile.velocity.Y * 0.02f;
        	projectile.alpha -= 5;
        	if (projectile.alpha < 30)
        	{
        		projectile.alpha = 30;
        	}
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
        	projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = 10;
			projectile.height = 10;
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 3; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 6; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 100, default(Color), 1.7f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num624].velocity *= 2f;
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (target.type == NPCID.TargetDummy)
			{
				return;
			}
        	Player player = Main.player[projectile.owner];
			player.statLife += 1;
    		player.HealEffect(1);
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        }
    }
}