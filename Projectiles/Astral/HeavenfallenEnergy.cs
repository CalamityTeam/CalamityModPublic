﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Astral
{
	public class HeavenfallenEnergy : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heavenfallen Energy");
		}

		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

		public override void AI()
		{
			int num154 = 14;
			int coolDust;
			projectile.ai[0] += 1;
			if (projectile.ai[0] % 2 == 0)
			{
				if (projectile.ai[0] % 4 == 0)
				{
					coolDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, mod.DustType("AstralBlue"), 0f, 0f, 100, default(Color), 1.5f);
				}
				else
				{
					coolDust = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width - num154 * 2, projectile.height - num154 * 2, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 1.5f);
				}
				Main.dust[coolDust].noGravity = true;
				Main.dust[coolDust].velocity *= 0.1f;
				Main.dust[coolDust].velocity += projectile.velocity * 0.5f;
			}

		}
	}
}