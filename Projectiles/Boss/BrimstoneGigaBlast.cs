using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneGigaBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fireblast");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.alpha = 50;
            projectile.timeLeft = 150;
            cooldownSlot = 1;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 5)
            {
                projectile.frame = 0;
            }
            bool revenge = CalamityWorld.revenge;
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.9f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0f / 255f);
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
            }
            float num953 = revenge ? 100f : 80f; //100
            float scaleFactor12 = revenge ? 20f : 15f; //5
            float num954 = 40f;
            int num959 = (int)projectile.ai[0];
            if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead)
            {
                if (projectile.Distance(Main.player[num959].Center) > num954)
                {
                    Vector2 vector102 = projectile.DirectionTo(Main.player[num959].Center);
                    if (vector102.HasNaNs())
                    {
                        vector102 = Vector2.UnitY;
                    }
                    projectile.velocity = (projectile.velocity * (num953 - 1f) + vector102 * scaleFactor12) / num953;
                }
            }
            else
            {
                if (projectile.ai[0] != -1f)
                {
                    projectile.ai[0] = -1f;
                    projectile.netUpdate = true;
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
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 240);
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 180, true);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            int i;
            if (projectile.owner == Main.myPlayer)
            {
                for (i = 0; i < 8; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 7f), (float)(Math.Cos(offsetAngle) * 7f), ModContent.ProjectileType<BrimstoneBarrage>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 7f), (float)(-Math.Cos(offsetAngle) * 7f), ModContent.ProjectileType<BrimstoneBarrage>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
                }
            }
            Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 50, default, 1f);
            for (int num194 = 0; num194 < 10; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 0, default, 1.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 50, default, 1f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
        }
    }
}
