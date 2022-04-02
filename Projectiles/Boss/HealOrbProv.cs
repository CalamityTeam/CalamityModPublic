using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class HealOrbProv : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heal");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            int num487 = (int)projectile.ai[0];
            float num488 = 2.5f;
            Vector2 vector36 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num489 = Main.player[num487].Center.X - vector36.X;
            float num490 = Main.player[num487].Center.Y - vector36.Y;
            float num491 = (float)Math.Sqrt((double)(num489 * num489 + num490 * num490));
            if (num491 < 50f && projectile.position.X < Main.player[num487].position.X + (float)Main.player[num487].width && projectile.position.X + (float)projectile.width > Main.player[num487].position.X && projectile.position.Y < Main.player[num487].position.Y + (float)Main.player[num487].height && projectile.position.Y + (float)projectile.height > Main.player[num487].position.Y)
            {
                if (projectile.owner == Main.myPlayer)
                {
                    int num492 = (int)projectile.ai[1];
                    Main.player[num487].HealEffect(num492, false);
                    Main.player[num487].statLife += num492;
                    if (Main.player[num487].statLife > Main.player[num487].statLifeMax2)
                    {
                        Main.player[num487].statLife = Main.player[num487].statLifeMax2;
                    }
                    if (Main.player[num487].statLife < 0)
                    {
                        Main.player[num487].KillMe(PlayerDeathReason.ByCustomReason(Main.player[Main.myPlayer].name + " burst into sinless ash."), 1000.0, 0, false);
                    }
                    NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, num487, (float)num492, 0f, 0f, 0, 0, 0);
                }
                projectile.Kill();
            }
            num491 = num488 / num491;
            num489 *= num491;
            num490 *= num491;
            projectile.velocity.X = (projectile.velocity.X * 15f + num489) / 16f;
            projectile.velocity.Y = (projectile.velocity.Y * 15f + num490) / 16f;
            for (int num497 = 0; num497 < 1; num497++)
            {
                float num498 = projectile.velocity.X * 0.2f * (float)num497;
                float num499 = -(projectile.velocity.Y * 0.2f) * (float)num497;
                int num500 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 127, 0f, 0f, 100, default, 1f);
                Main.dust[num500].noGravity = true;
                Main.dust[num500].velocity *= 0f;
                Dust expr_154F9_cp_0 = Main.dust[num500];
                expr_154F9_cp_0.position.X -= num498;
                Dust expr_15518_cp_0 = Main.dust[num500];
                expr_15518_cp_0.position.Y -= num499;
            }
            return;
        }
    }
}
