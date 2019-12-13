using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TalonSmallProj : ModProjectile
    {
        private int x;
        private double speed = 10;
        private float startSpeedY = 0f;
		private bool sign = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Talon");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
			if (projectile.ai[0] == 0f) //only happens once
			{
				if (Main.rand.NextBool(2))
					sign = false;
			}
            if (Main.rand.Next(5) == 0)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 2f)
            {
                projectile.tileCollide = true;
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.velocity.X = projectile.velocity.X + (Main.player[projectile.owner].velocity.X * 0.5f);
                startSpeedY = projectile.velocity.Y + (Main.player[projectile.owner].velocity.Y * 0.5f);
                projectile.velocity.Y = startSpeedY;
            }
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 5f)
            {
				if (sign == true)
				{
					x++;
					speed += 0.01;
					projectile.velocity.Y = startSpeedY + (float)(speed * Math.Sin(x / 4));
				}
				else
				{
					x++;
					speed += 0.01;
					projectile.velocity.Y = startSpeedY + (float)(speed * -Math.Sin(x / 4));
				}
            }
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 85, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
