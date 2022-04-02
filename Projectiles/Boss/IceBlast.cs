using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class IceBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.timeLeft = 300;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((projectile.position.X + (projectile.width / 2)) / 16f), (int)((projectile.position.Y + (projectile.height / 2)) / 16f), 0f, 0.25f, 0.25f);

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            for (int num322 = 0; num322 < 2; num322++)
            {
                int num323 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 92, projectile.velocity.X, projectile.velocity.Y, 50, default, 0.6f);
                Main.dust[num323].noGravity = true;
                Dust dust = Main.dust[num323];
                dust.velocity *= 0.3f;
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.01f;
                projectile.alpha -= 50;
                if (projectile.alpha <= 0)
                {
                    projectile.localAI[0] = 1f;
                    projectile.alpha = 0;
                }
            }
            else
            {
                projectile.scale -= 0.01f;
                projectile.alpha += 50;
                if (projectile.alpha >= 255)
                {
                    projectile.localAI[0] = 0f;
                    projectile.alpha = 255;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Main.dayTime ? new Color(50, 50, 255, projectile.alpha) : new Color(255, 255, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            int num497 = 5;
            Main.PlaySound(SoundID.Item27, projectile.position);
            int num3;
            for (int num498 = 0; num498 < num497; num498 = num3 + 1)
            {
                int num499 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 92, 0f, 0f, 0, default, 1f);
                if (Main.rand.Next(3) != 0)
                {
                    Dust dust = Main.dust[num499];
                    dust.velocity *= 2f;
                    Main.dust[num499].noGravity = true;
                    dust = Main.dust[num499];
                    dust.scale *= 1.75f;
                }
                else
                {
                    Dust dust = Main.dust[num499];
                    dust.scale *= 0.5f;
                }
                num3 = num498;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 90, true);
            target.AddBuff(BuffID.Chilled, 60, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
