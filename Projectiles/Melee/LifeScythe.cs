using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class LifeScythe : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scythe");
		}

		public override void SetDefaults()
		{
			projectile.width = 38;
			projectile.height = 70;
			projectile.aiStyle = 18;
			projectile.alpha = 55;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = 3;
			projectile.timeLeft = 240;
			projectile.ignoreWater = true;
			aiType = 274;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 4;
		}

		public override void AI()
		{
			if (projectile.localAI[0] == 0f)
			{
				projectile.scale -= 0.02f;
				projectile.alpha += 30;
				if (projectile.alpha >= 250)
				{
					projectile.alpha = 255;
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale += 0.02f;
				projectile.alpha -= 30;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.localAI[0] = 0f;
				}
			}
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.1f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f);
		}
	}
}