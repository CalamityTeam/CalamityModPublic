using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Amidias
{
	public class SandDollarProj : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Dollar");
		}

		public override void SetDefaults()
		{
			projectile.width = 30;
			projectile.height = 28;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.aiStyle = 3;
			projectile.timeLeft = 300;
			aiType = 272;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.ai[0] += 0.1f;
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}
	}
}