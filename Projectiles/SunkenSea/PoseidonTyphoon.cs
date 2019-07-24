using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.SunkenSea
{
    public class PoseidonTyphoon : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Poseidon Typhoon");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 300;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
		
		public override void AI()
        {
			projectile.rotation += projectile.velocity.X * 0.05f;
        	float centerX = projectile.Center.X;
			float centerY = projectile.Center.Y;
			float num474 = 600f;
			bool homeIn = false;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
				{
					float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
					float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						centerX = num476;
						centerY = num477;
						homeIn = true;
					}
				}
			}
			if (homeIn)
			{
				float num483 = 6f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = centerX - vector35.X;
				float num485 = centerY - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 30f + num484) / 31f;
				projectile.velocity.Y = (projectile.velocity.Y * 30f + num485) / 31f;
			}
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
        	return new Color(200, 200, 200, 0);
        }
		
		public override bool OnTileCollide(Vector2 oldVelocity)
        {
        	projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
    }
}