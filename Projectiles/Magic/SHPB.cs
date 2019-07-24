using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SHPB : ModProjectile
    {
    	public int explosionTimer = 120;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("SHPB");
			Main.projFrames[projectile.type] = 4;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
        	float num = (float)Main.rand.Next(90, 111) * 0.01f;
        	num *= Main.essScale;
			Lighting.AddLight(projectile.Center, 1f * num, 0.2f * num, 0.75f * num);
        	projectile.alpha -= 2;
        	projectile.frameCounter++;
			if (projectile.frameCounter > 4)
			{
			    projectile.frame++;
			    projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
			   projectile.frame = 0;
			}
        	projectile.ai[0] = (float)Main.rand.Next(-100, 101) * 0.0025f;
			projectile.ai[1] = (float)Main.rand.Next(-100, 101) * 0.0025f;
			if (projectile.localAI[0] == 0f)
			{
				projectile.scale += 0.05f;
				if ((double)projectile.scale > 1.2) 
				{
					projectile.localAI[0] = 1f;
				}
			}
			else
			{
				projectile.scale -= 0.05f;
				if ((double)projectile.scale < 0.8) 
				{
					projectile.localAI[0] = 0f;
				}
			}
        	projectile.velocity.X *= 0.985f;
        	projectile.velocity.Y *= 0.985f;
            float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 250f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (flag17)
			{
				explosionTimer--;
				if (explosionTimer <= 0)
				{
					projectile.Kill();
				}
			}
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
        	return new Color(255, Main.DiscoG, 155, projectile.alpha);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        	Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 105);
            if (projectile.owner == Main.myPlayer)
			{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("SHPExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}