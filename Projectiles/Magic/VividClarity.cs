using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class VividClarity : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ray");
		}

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.penetrate = 10;
			projectile.extraUpdates = 100;
			projectile.timeLeft = 300;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 2;
		}

		public override void AI()
		{
			Vector2 value7 = new Vector2(5f, 10f);
			projectile.ai[0] += 1f;
			if (projectile.ai[0] == 48f)
			{
				projectile.ai[0] = 0f;
			}
			else
			{
				for (int num41 = 0; num41 < 2; num41++)
				{
					Vector2 value8 = Vector2.UnitX * -12f;
					value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)num41 * 3.14159274f), default(Vector2)) * value7 - projectile.rotation.ToRotationVector2() * 10f;
					int num42 = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
					Main.dust[num42].scale = 0.75f;
					Main.dust[num42].noGravity = true;
					Main.dust[num42].position = projectile.Center + value8;
					Main.dust[num42].velocity = projectile.velocity;
				}
			}
			projectile.localAI[1] += 1f;
			if (projectile.localAI[1] >= 29f && projectile.owner == Main.myPlayer)
			{
				projectile.localAI[1] = 0f;
				Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.35f, projectile.velocity.Y * 0.35f, mod.ProjectileType("VividOrb"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
			}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num447 = 0; num447 < 2; num447++)
				{
					Vector2 vector33 = projectile.position;
					vector33 -= projectile.velocity * ((float)num447 * 0.25f);
					projectile.alpha = 255;
					int num448 = Dust.NewDust(vector33, 1, 1, 66, 0f, 0f, 0, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
					Main.dust[num448].noGravity = true;
					Main.dust[num448].position = vector33;
					Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
					Main.dust[num448].velocity *= 0.2f;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.damage = (int)((double)projectile.damage * 1.1);
			projectile.penetrate--;
			if (projectile.penetrate <= 0)
			{
				projectile.Kill();
			}
			else
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
			}
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (target.type == NPCID.TargetDummy)
			{
				return;
			}
			projectile.damage = (int)((double)projectile.damage * 1.05);
			target.AddBuff(mod.BuffType("ExoFreeze"), 30);
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
			target.AddBuff(mod.BuffType("GlacialState"), 120);
			target.AddBuff(mod.BuffType("Plague"), 120);
			target.AddBuff(mod.BuffType("HolyLight"), 120);
			target.AddBuff(BuffID.CursedInferno, 120);
			target.AddBuff(BuffID.Frostburn, 120);
			target.AddBuff(BuffID.OnFire, 120);
			target.AddBuff(BuffID.Ichor, 120);
		}
	}
}
