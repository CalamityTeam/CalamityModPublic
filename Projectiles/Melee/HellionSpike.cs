using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class HellionSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 600;
            aiType = 228;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.25f, 0f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft > 595)
				return false;

			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 8;
            target.AddBuff(BuffID.Venom, 300);
            if (crit)
            {
                float xPos = Main.rand.NextBool(2) ? projectile.position.X + 800 : projectile.position.X - 800;
                Vector2 vector2 = new Vector2(xPos, projectile.position.Y - Main.rand.Next(600, 801));
                float num80 = xPos;
                float speedX = (float)target.position.X - vector2.X;
                float speedY = (float)target.position.Y - vector2.Y;
                float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                dir = 10 / num80;
                speedX *= dir * 150;
                speedY *= dir * 150;
                if (speedX > 15f)
                {
                    speedX = 15f;
                }
                if (speedX < -15f)
                {
                    speedX = -15f;
                }
                if (speedY > 15f)
                {
                    speedY = 15f;
                }
                if (speedY < -15f)
                {
                    speedY = -15f;
                }
                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, ProjectileID.FlowerPetal, (int)((double)projectile.damage * 0.5), 2f, projectile.owner);
                }
            }
        }
    }
}
