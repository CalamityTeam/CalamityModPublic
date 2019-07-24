﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class MagicNebulaShot : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shot");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.extraUpdates = 5;
            projectile.timeLeft = 200;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 2;
		}

        public override void AI()
        {
        	if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 12);
			}
        	if (projectile.alpha > 0)
			{
				projectile.alpha -= 15;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			Lighting.AddLight(projectile.Center, 0.4f, 0.2f, 0.4f);
			for (int num121 = 0; num121 < 5; num121++)
			{
				Dust dust4 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, (Main.rand.Next(3) == 0 ? 56 : 242), projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 1f)];
				dust4.velocity = Vector2.Zero;
				dust4.position -= projectile.velocity / 5f * (float)num121;
				dust4.noGravity = true;
				dust4.scale = 0.8f;
				dust4.noLight = true;
			}
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600);
			target.AddBuff(BuffID.Frostburn, 600);
		}
	}
}