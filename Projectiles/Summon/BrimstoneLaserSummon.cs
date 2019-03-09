using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class BrimstoneLaserSummon : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Laser");
		}

		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 2;
			projectile.aiStyle = 1;
			aiType = 100;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.minion = true;
			projectile.minionSlots = 0;
			projectile.penetrate = 1;
			projectile.alpha = 120;
			projectile.timeLeft = 300;
		}

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f);
			projectile.velocity.X *= 1.05f;
			projectile.velocity.Y *= 1.05f;
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			projectile.velocity.Y += projectile.ai[0];
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 60);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(250, 50, 50, projectile.alpha);
		}
	}
}