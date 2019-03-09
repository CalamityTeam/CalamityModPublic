using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Astral
{
	public class Hive : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hive");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			projectile.width = 38;
			projectile.height = 60;
			projectile.ignoreWater = true;
			projectile.tileCollide = true;
			projectile.sentry = true;
			projectile.timeLeft = Projectile.SentryLifeTime;
			projectile.penetrate = -1;
		}

		public override void AI()
		{
			if (projectile.localAI[0] == 0f)
			{
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				projectile.localAI[0] += 1f;
			}
			if (Main.player[projectile.owner].minionDamage != projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue)
			{
				int damage2 = (int)(((float)projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue /
					projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue) *
					Main.player[projectile.owner].minionDamage);
				projectile.damage = damage2;
			}

			projectile.frameCounter++;
			if (projectile.frameCounter > 5)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 5)
			{
				projectile.frame = 0;
			}
			projectile.velocity.Y += 0.5f;

			if (projectile.velocity.Y > 10f)
			{
				projectile.velocity.Y = 10f;
			}

			if (projectile.owner == Main.myPlayer)
			{
				if (projectile.ai[0] != 0f)
				{
					projectile.ai[0] -= 1f;
					return;
				}
				projectile.ai[1] += 1f;
				if ((projectile.ai[1] % 15f) == 0f)
				{
					float velocityX = Main.rand.NextFloat(-0.4f, 0.4f);
					float velocityY = Main.rand.NextFloat(-0.3f, -0.5f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, velocityX, velocityY, mod.ProjectileType("Hiveling"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (projectile.penetrate == 0)
			{
				projectile.Kill();
			}
			return false;
		}

		public override bool CanDamage()
		{
			return false;
		}
	}
}
