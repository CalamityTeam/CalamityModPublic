using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class Iceberg : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Iceberg");
		}

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 300;
		}

		public override void AI()
		{
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num468 = 0; num468 < 3; num468++)
				{
					int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 67, 0f, 0f, 100, default(Color), 1f);
					Main.dust[num469].noGravity = true;
					Main.dust[num469].velocity *= 0f;
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
			}
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			double newDamageMult = 1.0 - ((double)projectile.timeLeft / 300.0);
			damage = (int)((double)damage * newDamageMult);
			knockback = 0f;
			if (crit || target.buffImmune[mod.BuffType("GlacialState")])
				damage *= 2;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			int debuffDuration = 300 - projectile.timeLeft;
			if (projectile.timeLeft < 270)
			{
				target.AddBuff(mod.BuffType("GlacialState"), debuffDuration);
			}
		}
	}
}