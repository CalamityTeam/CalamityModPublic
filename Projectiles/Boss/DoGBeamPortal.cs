using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGBeamPortal : ModProjectile
    {
    	public int beamTimer = 180;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Portal");
			Main.projFrames[projectile.type] = 6;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.penetrate = 1;
            cooldownSlot = 1;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.95f) / 255f, ((255 - projectile.alpha) * 1.15f) / 255f);
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
            beamTimer--;
        	if (beamTimer <= 0)
        	{
        		Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 33);
	        	float spread = 30f * 0.0174f;
				double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
				double deltaAngle = spread / 8f;
				double offsetAngle;
				int i;
				if (projectile.owner == Main.myPlayer)
				{
					for (i = 0; i < 4; i++)
					{
                        float speed = 4f;
                        int projectileDamage = Main.expertMode ? 64 : 75;
                        if (CalamityWorld.death)
                        {
                            speed = 7f;
                        }
                        else if (CalamityWorld.revenge)
                        {
                            speed = 6f;
                        }
                        else if (Main.expertMode)
                        {
                            speed = 5f;
                        }
                        offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * speed ), (float)( Math.Cos(offsetAngle) * speed ), mod.ProjectileType("DoGBeam"), projectileDamage, projectile.knockBack, projectile.owner, 0f, 0f);
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * speed ), (float)( -Math.Cos(offsetAngle) * speed ), mod.ProjectileType("DoGBeam"), projectileDamage, projectile.knockBack, projectile.owner, 0f, 0f);
					}
				}
				beamTimer = 180;
        	}
            int num103 = (int)Player.FindClosest(projectile.Center, 1, 1);
            float scaleFactor2 = projectile.velocity.Length();
            Vector2 vector11 = Main.player[num103].Center - projectile.Center;
            if (Vector2.Distance(Main.player[num103].Center, projectile.Center) > 2000f)
            {
                projectile.position.X = (float)(Main.player[num103].Center.X / 16) * 16f - (float)(projectile.width / 2);
                projectile.position.Y = ((float)(Main.player[num103].Center.Y / 16) * 16f - (float)(projectile.height / 2)) - 250f;
                projectile.ai[1] = 0f;
                beamTimer = 90;
            }
            vector11.Normalize();
            vector11 *= scaleFactor2;
            projectile.velocity = (projectile.velocity * 24f + vector11) / 25f;
            projectile.velocity.Normalize();
            projectile.velocity *= scaleFactor2;
            if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 92);
                for (int num621 = 0; num621 < 30; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1.2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1.7f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 173, 0f, 0f, 100, default(Color), 1f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }
    }
}