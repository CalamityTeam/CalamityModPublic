using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BlackAnurian : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bubble");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] > 2f)
            {
                projectile.alpha -= 20;
                if (projectile.alpha < 100)
                {
                    projectile.alpha = 100;
                }
            }
            else
            {
                projectile.localAI[0] += 1f;
            }
            if (projectile.ai[0] > 30f)
            {
                if (projectile.velocity.Y > -8f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.05f;
                }
                projectile.velocity.X = projectile.velocity.X * 0.98f;
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation = projectile.velocity.X * 0.1f;
            if (projectile.wet)
            {
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                }
                if (projectile.velocity.Y > -8f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.2f;
                }
                projectile.velocity.X = projectile.velocity.X * 0.94f;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item54, projectile.position);
            int num3;
            for (int num246 = 0; num246 < 25; num246 = num3 + 1)
            {
                int num247 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 14, 0f, 0f, 0, default(Color), 1f);
                Main.dust[num247].position = (Main.dust[num247].position + projectile.position) / 2f;
                Main.dust[num247].velocity = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                Main.dust[num247].velocity.Normalize();
                Dust dust = Main.dust[num247];
                dust.velocity *= (float)Main.rand.Next(1, 30) * 0.1f;
                Main.dust[num247].alpha = projectile.alpha;
                num3 = num246;
            }
        }
    }
}
