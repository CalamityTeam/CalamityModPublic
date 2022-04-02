using CalamityMod.Items.Weapons.Typeless;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class AeroExplosive : ModProjectile
	{
        public override string Texture => "CalamityMod/Items/Weapons/Typeless/AeroDynamite";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aeroboom");
		}

		public override void SetDefaults()
		{
			projectile.width = 15;
			projectile.height = 15;
			projectile.friendly = true;
			projectile.tileCollide = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 300;

			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
		}

		public override void AI()
		{
			if (projectile.timeLeft < 2)
			{
				projectile.damage = AeroDynamite.Damage; // Like most explosives, not boosted by damage boosts
				projectile.knockBack = AeroDynamite.Knockback;
			}

			if (projectile.timeLeft % 4 == 0 && projectile.timeLeft < 270)
				projectile.ai[0]++;
			projectile.velocity *= 0.999f - projectile.ai[0] * Main.rand.NextFloat(0.00075f, 0.00125f);
			projectile.rotation += projectile.velocity.Length() * 0.09f * projectile.direction;

			if (Main.rand.NextBool(5))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100, new Color(53, Main.DiscoG, 255));
			}
			if (Main.rand.NextBool(5))
			{
				Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 16, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
			}

			if (Main.rand.NextBool())
			{
				int smoke = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1f);
				Main.dust[smoke].scale = 0.1f + Main.rand.NextFloat(0f, 0.5f);
				Main.dust[smoke].fadeIn = 1.5f + Main.rand.NextFloat(0f, 0.5f);
				Main.dust[smoke].noGravity = true;
				Main.dust[smoke].position = projectile.Center + new Vector2(0f, -projectile.height / 2f).RotatedBy(projectile.rotation, default) * 1.1f;
				int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 1f);
				Main.dust[fire].scale = 1f + Main.rand.NextFloat(0f, 0.5f);
				Main.dust[fire].noGravity = true;
				Main.dust[fire].position = projectile.Center + new Vector2(0f, -projectile.height / 2f).RotatedBy(projectile.rotation, default) * 1.1f;
			}
		}

		// Makes the projectile bounce infinitely, but lose a ton of speed on bounce.
		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X * 0.1f;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y * 0.1f;
			}
			return false;
		}

		public override void Kill(int timeLeft)
		{
			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 200);
			projectile.maxPenetrate = projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.Damage();
			Main.PlaySound(SoundID.Item14, projectile.Center);

			for (int d = 0; d < 40; d++)
			{
				int smoke = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
				Main.dust[smoke].velocity *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[smoke].scale = 0.5f;
					Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int d = 0; d < 70; d++)
			{
				int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
				Main.dust[fire].noGravity = true;
				Main.dust[fire].velocity *= 5f;
				fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
				Main.dust[fire].velocity *= 2f;
			}
			CalamityUtils.ExplosionGores(projectile.Center, 3);

			CalamityGlobalProjectile.ExpandHitboxBy(projectile, 15);

			if (projectile.owner == Main.myPlayer)
			{
				CalamityUtils.ExplodeandDestroyTiles(projectile, 7, true, new List<int>() { }, new List<int>() { });
			}
		}
	}
}
