using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class AstrealArrow : ModProjectile
    {
        private int flameTimer = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astreal Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.arrow = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.02f;
                projectile.alpha += 30;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.02f;
                projectile.alpha -= 30;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }
            int random = Main.rand.Next(1, 4);
            flameTimer -= random;
            int choice = Main.rand.Next(2);
            projectile.velocity.X *= 1.05f;
            projectile.velocity.Y *= 1.05f;
            if (choice == 0 && (projectile.velocity.X >= 25f || projectile.velocity.Y >= 25f))
            {
                projectile.velocity.X = 0f;
                projectile.velocity.Y = 10f;
            }
            else if (choice == 1 && (projectile.velocity.X >= 25f || projectile.velocity.Y >= 25f))
            {
                projectile.velocity.X = 10f;
                projectile.velocity.Y = 0f;
            }
            else if (choice == 0 && (projectile.velocity.X <= -25f || projectile.velocity.Y <= -25f))
            {
                projectile.velocity.X = 0f;
                projectile.velocity.Y = -10f;
            }
            else if (choice == 1 && (projectile.velocity.X <= -25f || projectile.velocity.Y <= -25f))
            {
                projectile.velocity.X = -10f;
                projectile.velocity.Y = 0f;
            }
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            if (flameTimer <= 0)
            {
                float xPos = Main.rand.NextBool(2) ? projectile.position.X + 800 : projectile.position.X - 800;
                Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-800, 801));
                float num80 = xPos;
                float speedX = (float)projectile.position.X - vector2.X;
                float speedY = (float)projectile.position.Y - vector2.Y;
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
                    Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("AstrealFlame"), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                }
                flameTimer = 180;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 360);
        }
    }
}
