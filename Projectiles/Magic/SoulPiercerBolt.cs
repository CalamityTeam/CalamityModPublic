﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class SoulPiercerBolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Piercer");
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.extraUpdates = 100;
			projectile.friendly = true;
			projectile.timeLeft = 180;
			projectile.penetrate = -1;
			projectile.magic = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			for (int num447 = 0; num447 < 4; num447++)
			{
				Vector2 vector33 = projectile.position;
				vector33 -= projectile.velocity * ((float)num447 * 0.25f);
				projectile.alpha = 255;
				int num448 = Dust.NewDust(vector33, 1, 1, 173, 0f, 0f, 0, default(Color), 0.5f);
				Main.dust[num448].position = vector33;
				Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.007f;
				Main.dust[num448].velocity *= 0.2f;
			}
		}
	}
}