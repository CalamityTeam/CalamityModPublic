using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodflareSoul : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Soul");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 1.2f;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 900;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            int num822 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 60, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            Main.dust[num822].scale = 1.3f;
            Main.dust[num822].noGravity = true;
            float num953 = 30f * projectile.ai[1]; //100
            float scaleFactor12 = 6f * projectile.ai[1]; //5
            float num954 = 600f;
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) - 1.57f;
            Lighting.AddLight(projectile.Center, 0.5f, 0.2f, 0.9f);
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
            {
                if (projectile.Distance(Main.player[projectile.owner].Center) > num954)
                {
                    Vector2 moveDirection = projectile.SafeDirectionTo(Main.player[projectile.owner].Center, Vector2.UnitY);
                    projectile.velocity = (projectile.velocity * (num953 - 1f) + moveDirection * scaleFactor12) / num953;
                    return;
                }
                CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 200f, 11f, 20f);
            }
            else
            {
                if (projectile.timeLeft > 30)
                {
                    projectile.timeLeft = 30;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
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

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 39, 1f, 0f);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 110;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 60, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 2f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            projectile.Damage();
        }
    }
}
