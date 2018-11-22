using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class IonBlast : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blast");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 2;
            projectile.ignoreWater = false;
            projectile.timeLeft = 120;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	projectile.velocity.X *= 1.015f;
        	projectile.velocity.Y *= 1.015f;
        	if (projectile.alpha > 0)
			{
				projectile.alpha -= 3;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f, 0f, 0.2f);
			float num55 = 100f;
			float num56 = 3f;
			if (projectile.ai[1] == 0f)
			{
				projectile.localAI[0] += num56;
				if (projectile.localAI[0] > num55)
				{
					projectile.localAI[0] = num55;
				}
			}
			else
			{
				projectile.localAI[0] -= num56;
				if (projectile.localAI[0] <= 0f)
				{
					projectile.Kill();
					return;
				}
			}
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 92);
        	if (projectile.owner == Main.myPlayer)
        	{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("IonExplosion"), projectile.damage, 0f, projectile.owner, 0f, 0f);
        	}
            int num212 = Main.rand.Next(15, 30);
			for (int num213 = 0; num213 < num212; num213++)
			{
				int num214 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 130, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num214].velocity *= 2f;
				Main.dust[num214].noGravity = true;
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        }
    }
}