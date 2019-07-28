using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniSandShark : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mini Sand Shark");
		}

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.minionSlots = 0f;
        }

        public override void AI()
        {
            projectile.alpha -= 40;
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 19, 1f, 0f);
            }
            if (projectile.ai[0] >= 45f)
            {
                projectile.ai[0] = 45f;
                projectile.velocity.Y = projectile.velocity.Y + 0.025f;
            }
            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (float)Math.Atan2((double)(-(double)projectile.velocity.Y), (double)(-(double)projectile.velocity.X));
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = (projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
			return false;
		}

		public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position = projectile.Center;
            projectile.width = (projectile.height = 32);
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 85, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            projectile.Damage();
        }
    }
}
