using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class FungalHeal : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heal");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            int num487 = (int)projectile.ai[0];
            float num488 = 5f;
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
            projectile.velocity.X = (projectile.velocity.X * 9f + num489) / 16f;
            projectile.velocity.Y = (projectile.velocity.Y * 9f + num490) / 16f;
            float num494 = projectile.velocity.X * 0.334f;
            float num495 = -(projectile.velocity.Y * 0.334f);
            int num496 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 56, 0f, 0f, 100, default(Color), 0.5f);
            Main.dust[num496].noGravity = true;
            Main.dust[num496].velocity *= 0f;
            Dust expr_153E2_cp_0 = Main.dust[num496];
            expr_153E2_cp_0.position.X = expr_153E2_cp_0.position.X - num494;
            Dust expr_15401_cp_0 = Main.dust[num496];
            expr_15401_cp_0.position.Y = expr_15401_cp_0.position.Y - num495;
            float num498 = projectile.velocity.X * 0.2f;
            float num499 = -(projectile.velocity.Y * 0.2f);
            int num500 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 56, 0f, 0f, 100, default(Color), 0.7f);
            Main.dust[num500].noGravity = true;
            Main.dust[num500].velocity *= 0f;
            Dust expr_154F9_cp_0 = Main.dust[num500];
            expr_154F9_cp_0.position.X = expr_154F9_cp_0.position.X - num498;
            Dust expr_15518_cp_0 = Main.dust[num500];
            expr_15518_cp_0.position.Y = expr_15518_cp_0.position.Y - num499;
        }
    }
}