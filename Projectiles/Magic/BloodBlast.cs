﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BloodBlast : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blast");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 2;
            projectile.magic = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        	for (int num92 = 0; num92 < 3; num92++)
			{
				float num93 = projectile.velocity.X / 3f * (float)num92;
				float num94 = projectile.velocity.Y / 3f * (float)num92;
				int num95 = 4;
				int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 5, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num96].noGravity = true;
				Main.dust[num96].velocity *= 0.1f;
				Main.dust[num96].velocity += projectile.velocity * 0.1f;
				Dust expr_47FA_cp_0 = Main.dust[num96];
				expr_47FA_cp_0.position.X = expr_47FA_cp_0.position.X - num93;
				Dust expr_4815_cp_0 = Main.dust[num96];
				expr_4815_cp_0.position.Y = expr_4815_cp_0.position.Y - num94;
			}
			if (Main.rand.Next(5) == 0)
			{
				int num97 = 4;
				int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, 5, 0f, 0f, 100, default(Color), 0.6f);
				Main.dust[num98].velocity *= 0.25f;
				Main.dust[num98].velocity += projectile.velocity * 0.5f;
			}
			if (projectile.ai[1] >= 20f)
			{
				projectile.velocity.Y = projectile.velocity.Y + 0.2f;
			}
			else
			{
				projectile.rotation += 0.3f * (float)projectile.direction;
			}
        }
    }
}