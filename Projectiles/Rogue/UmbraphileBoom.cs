using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.Projectiles.Rogue
{
	public class UmbraphileBoom : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explosion");
		}

		public override void SetDefaults()
		{
			projectile.width = 60;
			projectile.height = 60;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.timeLeft = 5;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = -1;
			projectile.Calamity().rogue = true;
		}

		public override void Kill(int timeLeft)
		{
			int dustType = Main.rand.NextBool(2) ? 246 : 176;
			bool flag15 = false;
			bool flag16 = false;
			if (projectile.velocity.X < 0f && projectile.position.X < projectile.ai[0])
			{
				flag15 = true;
			}
			if (projectile.velocity.X > 0f && projectile.position.X > projectile.ai[0])
			{
				flag15 = true;
			}
			if (projectile.velocity.Y < 0f && projectile.position.Y < projectile.ai[1])
			{
				flag16 = true;
			}
			if (projectile.velocity.Y > 0f && projectile.position.Y > projectile.ai[1])
			{
				flag16 = true;
			}
			if (flag15 && flag16)
			{
				projectile.Kill();
			}
			float num461 = 25f;
			if (projectile.ai[0] > 180f)
			{
				num461 -= (projectile.ai[0] - 180f) / 2f;
			}
			if (num461 <= 0f)
			{
				num461 = 0f;
				projectile.Kill();
			}
			num461 *= 0.7f;
			projectile.ai[0] += 4f;
			int num462 = 0;
			while ((float)num462 < num461)
			{
				float num463 = (float)Main.rand.Next(-10, 11);
				float num464 = (float)Main.rand.Next(-10, 11);
				float num465 = (float)Main.rand.Next(3, 9);
				float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
				num466 = num465 / num466;
				num463 *= num466;
				num464 *= num466;
				int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1f);
				Dust dust = Main.dust[num467];
				dust.noGravity = true;
				dust.position.X = projectile.Center.X;
				dust.position.Y = projectile.Center.Y;
				dust.position.X += (float)Main.rand.Next(-10, 11);
				dust.position.Y += (float)Main.rand.Next(-10, 11);
				dust.velocity.X = num463;
				dust.velocity.Y = num464;
				num462++;
			}
			return;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (Main.dayTime || Main.rand.NextBool(3)) //100% during day, 33.33% chance at night
				target.AddBuff(BuffID.Daybreak, 60);

			if (!Main.dayTime || Main.rand.NextBool(3)) //100% at night, 33.33% chance during day
				target.AddBuff(ModContent.BuffType<Nightwither>(), 60);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			if (!Main.dayTime || Main.rand.NextBool(3)) //100% at night, 33.33% chance during day
				target.AddBuff(ModContent.BuffType<Nightwither>(), 60);
		}
	}
}
