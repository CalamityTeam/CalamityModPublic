using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
	public class RoxShockwave : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.penetrate = -1;
			projectile.width = 450;
			projectile.height = 450;
			projectile.melee = true;
			projectile.friendly = true;
			projectile.tileCollide = false;
			projectile.timeLeft = 40;
		}
	}
}
