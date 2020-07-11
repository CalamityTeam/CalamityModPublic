using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
	public class MadAlchemistsCocktailAlt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mad Alchemist's Cocktail");
		}

		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 24;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.ignoreWater = true;
			projectile.penetrate = 1;
		}

		public override void AI()
		{
			MadAlchemistsCocktailBlue.FlaskAI();
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
			target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
			target.AddBuff(BuffID.Poisoned, 300);
			target.AddBuff(BuffID.OnFire, 300);
			target.AddBuff(BuffID.CursedInferno, 180);
			target.AddBuff(BuffID.Frostburn, 300);
			target.AddBuff(BuffID.Venom, 300);
			target.AddBuff(BuffID.ShadowFlame, 300);
		}

		public override void OnHitPvp(Player target, int damage, bool crit)
		{
			target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
			target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300);
			target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
			target.AddBuff(BuffID.Poisoned, 300);
			target.AddBuff(BuffID.OnFire, 300);
			target.AddBuff(BuffID.CursedInferno, 180);
			target.AddBuff(BuffID.Frostburn, 300);
			target.AddBuff(BuffID.Venom, 300);
			target.AddBuff(BuffID.ShadowFlame, 300);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item107, projectile.position);
			Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704, 1f);
			Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705, 1f);
			int height = 120;
			float dustScaleA = 2.5f;
			float dustScaleB = 1.8f;
			Vector2 dustDirection = (-MathHelper.PiOver2).ToRotationVector2();
			Vector2 dustVel = dustDirection * projectile.velocity.Length() * projectile.MaxUpdates;
			Main.PlaySound(SoundID.Item20, projectile.Center);
			projectile.position = projectile.Center;
			projectile.width = projectile.height = height;
			projectile.Center = projectile.position;
			projectile.maxPenetrate = -1;
			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 10;
			projectile.damage *= 2;
			projectile.Damage();
			for (int i = 0; i < 40; i++)
			{
				int prettyEffects = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, 0f, 0f, 200, default, dustScaleA);
				Dust dust = Main.dust[prettyEffects];
				dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * projectile.width / 2f;
				dust.noGravity = true;
				dust.velocity *= 4f;
				dust.velocity += dustVel * Main.rand.NextFloat();
				prettyEffects = Dust.NewDust(projectile.position, projectile.width, projectile.height, 174, 0f, 0f, 100, default, dustScaleB);
				dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * projectile.width / 2f;
				dust.velocity *= 3f;
				dust.noGravity = true;
				prettyEffects = Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, 0f, 0f, 100, default, dustScaleB);
				dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(Math.PI) * Main.rand.NextFloat() * projectile.width / 2f;
				dust.velocity *= 2f;
				dust.noGravity = true;
				dust.fadeIn = 1f;
				dust.color = Color.Green * 0.5f;
				dust.velocity += dustVel * Main.rand.NextFloat();
			}
			for (int j = 0; j < 20; j++)
			{
				int dusty = Dust.NewDust(projectile.position, projectile.width, projectile.height, 206, 0f, 0f, 0, default, num52);
				Dust dust = Main.dust[dusty];
				dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(Math.PI).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 3f;
				dust.noGravity = true;
				dust.velocity *= 0.5f;
				dust.velocity += dustVel * (0.6f + 0.6f * Main.rand.NextFloat());
			}
		}
	}
}
