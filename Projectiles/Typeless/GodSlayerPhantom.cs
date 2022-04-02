using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
    public class GodSlayerPhantom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Phantom");
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }
            int num822 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 0, default, 0.5f);
            Dust dust = Main.dust[num822];
            dust.velocity *= 0.1f;
            dust.scale = 1.3f;
            dust.noGravity = true;
            float num953 = 40f * projectile.ai[1]; //100
            float scaleFactor12 = 8f * projectile.ai[1]; //5
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
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 12f, 20f);
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
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 40;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            int num226 = 36;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 vector7 = vector6 - projectile.Center;
                int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 173, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default, 0.5f);
                Main.dust[num228].noGravity = true;
                Main.dust[num228].noLight = true;
                Main.dust[num228].velocity = vector7;
            }
            projectile.Damage();
        }
    }
}
