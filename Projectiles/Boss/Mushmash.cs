using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class Mushmash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mushmash");
		}

		public override void SetDefaults()
		{
			projectile.width = 200;
			projectile.height = 200;
			projectile.hostile = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 10;
		}
	}
}