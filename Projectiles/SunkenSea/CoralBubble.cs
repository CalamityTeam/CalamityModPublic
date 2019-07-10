using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.SunkenSea
{
    public class CoralBubble : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bubble");
        }
    	
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.timeLeft = 360;
            projectile.penetrate = 1;
        }

		public override void AI()
		{
			if (projectile.localAI[0] > 2f)
			{
				projectile.alpha -= 5;
				if (projectile.alpha < 100)
				{
					projectile.alpha = 100;
				}
			}
			else
			{
				projectile.localAI[0] += 1f;
			}
			if (projectile.ai[1] > 30f)
			{
				if (projectile.velocity.Y > -1.5f)
				{
					projectile.velocity.Y = projectile.velocity.Y - 0.05f;
				}
			}
			else
			{
				projectile.ai[1] += 1f;
			}
			if (projectile.wet)
			{
				if (projectile.velocity.Y > 0f)
				{
					projectile.velocity.Y = projectile.velocity.Y * 0.98f;
				}
				if (projectile.velocity.Y > -1f)
				{
					projectile.velocity.Y = projectile.velocity.Y - 0.2f;
				}
			}
			int closestPlayer = (int)Player.FindClosest(projectile.Center, 1, 1);
			Vector2 distance = Main.player[closestPlayer].Center - projectile.Center;
			if (projectile.Distance(Main.player[closestPlayer].Center) < 14f)
			{
				Main.player[closestPlayer].AddBuff(BuffID.Gills, 90);
				projectile.Kill();
			}
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
            Main.PlaySound(SoundID.Item54, projectile.position);
            projectile.position = projectile.Center;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			int num190 = Main.rand.Next(5, 9);
			for (int num191 = 0; num191 < num190; num191++)
			{
				int num192 = Dust.NewDust(projectile.Center, 0, 0, 206, 0f, 0f, 100, default(Color), 1.4f);
				Main.dust[num192].velocity *= 0.8f;
				Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
				Main.dust[num192].noGravity = true;
			}
		}
    }
}
