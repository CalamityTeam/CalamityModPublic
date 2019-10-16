using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class SilvaOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 3;
            projectile.light = 0.2f;
        }

        public override void AI()
        {
            projectile.alpha -= 2;
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
            int num487 = (int)projectile.ai[0];
            float num488 = 6f;
            Vector2 vector36 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num489 = Main.player[num487].Center.X - vector36.X;
            float num490 = Main.player[num487].Center.Y - vector36.Y;
            float num491 = (float)Math.Sqrt((double)(num489 * num489 + num490 * num490));
            if (num491 < 50f && projectile.position.X < Main.player[num487].position.X + (float)Main.player[num487].width && projectile.position.X + (float)projectile.width > Main.player[num487].position.X && projectile.position.Y < Main.player[num487].position.Y + (float)Main.player[num487].height && projectile.position.Y + (float)projectile.height > Main.player[num487].position.Y)
            {
                if (projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
                {
                    int num492 = (int)projectile.ai[1];
                    Main.player[num487].HealEffect(num492, false);
                    Main.player[num487].statLife += num492;
                    if (Main.player[num487].statLife > Main.player[num487].statLifeMax2)
                    {
                        Main.player[num487].statLife = Main.player[num487].statLifeMax2;
                    }
                    NetMessage.SendData(66, -1, -1, null, num487, (float)num492, 0f, 0f, 0, 0, 0);
                }
                projectile.Kill();
            }
            num491 = num488 / num491;
            num489 *= num491;
            num490 *= num491;
            projectile.velocity.X = (projectile.velocity.X * 15f + num489) / 16f;
            projectile.velocity.Y = (projectile.velocity.Y * 15f + num490) / 16f;
            return;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            for (int num407 = 0; num407 < 5; num407++)
            {
                int num408 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[num408].noGravity = true;
                Main.dust[num408].velocity *= 1.5f;
                Main.dust[num408].scale = 1.5f;
            }
        }
    }
}
