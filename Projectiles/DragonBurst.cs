using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class DragonBurst : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burst");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.extraUpdates = 10;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.65f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        	for (int num92 = 0; num92 < 2; num92++)
			{
				float num93 = projectile.velocity.X / 3f * (float)num92;
				float num94 = projectile.velocity.Y / 3f * (float)num92;
				int num95 = 4;
				int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 127, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num96].noGravity = true;
				Main.dust[num96].velocity *= 0.1f;
				Main.dust[num96].velocity += projectile.velocity * 0.1f;
				Dust expr_47FA_cp_0 = Main.dust[num96];
				expr_47FA_cp_0.position.X = expr_47FA_cp_0.position.X - num93;
				Dust expr_4815_cp_0 = Main.dust[num96];
				expr_4815_cp_0.position.Y = expr_4815_cp_0.position.Y - num94;
			}
			if (Main.rand.Next(10) == 0)
			{
				int num97 = 4;
				int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, 127, 0f, 0f, 100, default(Color), 0.6f);
				Main.dust[num98].velocity *= 0.25f;
				Main.dust[num98].velocity += projectile.velocity * 0.5f;
			}
			if (projectile.velocity.Y > 20f)
			{
				projectile.velocity.Y = 20f;
				return;
			}
        }

        public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
        		int boom = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, 612, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                Main.projectile[boom].melee = false;
                Main.projectile[boom].ranged = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.OnFire, 500);
        }
    }
}