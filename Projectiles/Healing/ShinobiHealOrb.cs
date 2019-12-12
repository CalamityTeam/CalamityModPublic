using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class ShinobiHealOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Healing Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            aiType = 348;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            int num487 = projectile.owner;

            float num488 = 6f;
            if (Main.player[num487].lifeMagnet)
                num488 *= 1.5f;

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

            for (int num468 = 0; num468 < 3; num468++)
            {
                int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 15, 0f, 0f, 100, default, 1.3f);
                Main.dust[num469].noGravity = true;
                Main.dust[num469].velocity *= 0f;
            }
        }
    }
}
