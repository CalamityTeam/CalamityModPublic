﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BoltArrow : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bolt Arrow");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }
        
        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 15f)
            {
                projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 vector3 = Vector2.UnitX * (float)(-(float)projectile.width) / 2f;
                    vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector3 = vector3.RotatedBy((double)(projectile.rotation - 1.57079637f), default(Vector2));
                    int num9 = Dust.NewDust(projectile.Center, 0, 0, 221, 0f, 0f, 160, default(Color), 1f);
                    Main.dust[num9].scale = 1.1f;
                    Main.dust[num9].noGravity = true;
                    Main.dust[num9].position = projectile.Center + vector3;
                    Main.dust[num9].velocity = projectile.velocity * 0.1f;
                    Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
                }
            }
        }
        
        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 92);
        	if (projectile.owner == Main.myPlayer)
        	{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("BoltExplosion"), projectile.damage, 0f, projectile.owner, 0f, 0f);
        	}
        	int num212 = Main.rand.Next(10, 20);
			for (int num213 = 0; num213 < num212; num213++)
			{
				int num214 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num214].velocity *= 2f;
				Main.dust[num214].noGravity = true;
			}
        }
    }
}