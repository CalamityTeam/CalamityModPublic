using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class TheDeadlyMicrobeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Microbe");
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 14;
			projectile.friendly = true;
			projectile.alpha = 255;
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.timeLeft = 3600;
		}

		public override void AI()
		{
			projectile.SporeSacAI();
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.CursedInferno, 90);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.CursedInferno, 90);
		}

		public override void Kill(int timeLeft)
		{
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 56);
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			int dustAmt = 36;
			for (int d = 0; d < dustAmt; d++)
			{
				Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
				source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
				Vector2 dustVel = source - projectile.Center;
				int index = Dust.NewDust(source + dustVel, 0, 0, 44, dustVel.X, dustVel.Y, 100, default, 0.5f);
				Main.dust[index].noGravity = true;
				Main.dust[index].noLight = true;
				Main.dust[index].velocity = dustVel;
			}
			projectile.Damage();
		}
	}
}
