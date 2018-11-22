using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DarkEnergyBall : ModProjectile
    {
        private double timeElapsed = 0.0;
        private double circleSize = 1.0;
        private double circleGrowth = 0.02;

    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Energy");
            Main.projFrames[projectile.type] = 6;
        }
    	
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            timeElapsed += 0.02;
            projectile.velocity.X = (float)(Math.Sin(timeElapsed * (double)(0.5f * projectile.ai[0])) * circleSize);
            projectile.velocity.Y = (float)(Math.Cos(timeElapsed * (double)(0.5f * projectile.ai[0])) * circleSize);
            circleSize += circleGrowth;
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 5)
            {
                projectile.frame = 0;
            }
            int num1009 = 1;
            int num1010 = 60;
            for (int num1011 = 0; num1011 < 2; num1011++)
            {
                if (Main.rand.Next(3) < num1009)
                {
                    int num1012 = Dust.NewDust(projectile.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                    Main.dust[num1012].noGravity = true;
                    Main.dust[num1012].velocity *= 0.2f;
                    Main.dust[num1012].fadeIn = 1f;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 90, 0f, 0f);
            }
        }
    }
}