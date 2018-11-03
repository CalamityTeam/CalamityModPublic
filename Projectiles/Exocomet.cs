using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Exocomet : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Comet");
			Main.projFrames[projectile.type] = 5;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.alpha = 50;
        }

        public override void AI()
        {
        	projectile.frameCounter++;
			if (projectile.frameCounter > 5)
			{
			    projectile.frame++;
			    projectile.frameCounter = 0;
			}
			if (projectile.frame > 4)
			{
			   projectile.frame = 0;
			}
        	float num953 = 100f * projectile.ai[1]; //100
        	float scaleFactor12 = 20f * projectile.ai[1]; //5
			float num954 = 40f;
			if (projectile.timeLeft > 30 && projectile.alpha > 0) 
			{
				projectile.alpha -= 25;
			}
			if (projectile.timeLeft > 30 && projectile.alpha < 128 && Collision.SolidCollision(projectile.position, projectile.width, projectile.height)) 
			{
				projectile.alpha = 128;
			}
			if (projectile.alpha < 0) 
			{
				projectile.alpha = 0;
			}
			if (projectile.alpha < 40) 
			{
				int num309 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X * 4f + 2f, projectile.position.Y + 2f - projectile.velocity.Y * 4f), 8, 8, 107, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
				Main.dust[num309].velocity *= -0.25f;
				num309 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X * 4f + 2f, projectile.position.Y + 2f - projectile.velocity.Y * 4f), 8, 8, 107, projectile.oldVelocity.X, projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
				Main.dust[num309].velocity *= -0.25f;
				Main.dust[num309].position -= projectile.velocity * 0.5f;
			}
			projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
			Lighting.AddLight(projectile.Center, 0f, 0.5f, 0.5f);
			int num959 = (int)projectile.ai[0];
			if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead) 
			{
				if (projectile.Distance(Main.player[num959].Center) > num954) 
				{
					Vector2 vector102 = projectile.DirectionTo(Main.player[num959].Center);
					if (vector102.HasNaNs()) 
					{
						vector102 = Vector2.UnitY;
					}
					projectile.velocity = (projectile.velocity * (num953 - 1f) + vector102 * scaleFactor12) / num953;
					return;
				}
			} 
			else 
			{
				if (projectile.timeLeft > 30) 
				{
					projectile.timeLeft = 30;
				}
				if (projectile.ai[0] != -1f) 
				{
					projectile.ai[0] = -1f;
					projectile.netUpdate = true;
					return;
				}
			}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	if (Main.rand.Next(30) == 0)
	    	{
	    		target.AddBuff(mod.BuffType("ExoFreeze"), 240);
	    	}
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 100);
        	target.AddBuff(mod.BuffType("GlacialState"), 100);
        	target.AddBuff(mod.BuffType("Plague"), 100);
        	target.AddBuff(mod.BuffType("HolyLight"), 100);
        	target.AddBuff(BuffID.CursedInferno, 100);
			target.AddBuff(BuffID.Frostburn, 100);
			target.AddBuff(BuffID.OnFire, 100);
			target.AddBuff(BuffID.Ichor, 100);
			if (target.type == NPCID.TargetDummy)
			{
				return;
			}
			float num = (float)damage * 0.01f;
			if ((int)num == 0)
			{
				return;
			}
			if (Main.player[Main.myPlayer].lifeSteal <= 0f)
			{
				return;
			}
			Main.player[Main.myPlayer].lifeSteal -= num;
			int num2 = projectile.owner;
			Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, mod.ProjectileType("Exoheal"), 0, 0f, projectile.owner, (float)num2, num);
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
        	return new Color(0, 255, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 103, 1f, 0f);
			projectile.position = projectile.Center;
			projectile.width = (projectile.height = 80);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num193 = 0; num193 < 2; num193++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
			}
			for (int num194 = 0; num194 < 20; num194++)
			{
				int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
				Main.dust[num195].noGravity = true;
				Main.dust[num195].velocity *= 3f;
				num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
				Main.dust[num195].velocity *= 2f;
				Main.dust[num195].noGravity = true;
			}
        }
    }
}