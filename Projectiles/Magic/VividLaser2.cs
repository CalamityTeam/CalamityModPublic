using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VividLaser2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vivid Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
            Vector2 value7 = new Vector2(5f, 10f);
            for (int dust = 0; dust < 2; dust++)
            {
				Vector2 value8 = Vector2.UnitX * -12f;
				value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)dust * 3.14159274f), default) * value7 - projectile.rotation.ToRotationVector2() * 10f;
				int num42 = Dust.NewDust(projectile.Center, 0, 0, 66, 0f, 0f, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
				Main.dust[num42].scale = 0.75f;
				Main.dust[num42].noGravity = true;
				Main.dust[num42].position = projectile.Center + value8;
				Main.dust[num42].velocity = projectile.velocity;
            }
			if (projectile.timeLeft < 110)
				projectile.ai[0] = 1f;
            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 400f;
            bool flag17 = false;
			if (projectile.ai[0] >= 1f)
			{
				for (int num475 = 0; num475 < 200; num475++)
				{
					if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
					{
						float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
						float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
						float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
						if (num478 < num474)
						{
							num474 = num478;
							num472 = num476;
							num473 = num477;
							flag17 = true;
						}
					}
				}
				if (flag17)
				{
					float num483 = 30f;
					Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num484 = num472 - vector35.X;
					float num485 = num473 - vector35.Y;
					float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
					num486 = num483 / num486;
					num484 *= num486;
					num485 *= num486;
					projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
					projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
					return;
				}
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
					107,
					234,
					269
                });
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 30);
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 120);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
            target.AddBuff(BuffID.CursedInferno, 120);
            target.AddBuff(BuffID.Frostburn, 120);
            target.AddBuff(BuffID.OnFire, 120);
            target.AddBuff(BuffID.Ichor, 120);
		}

        // Cannot deal damage for the first several frames of existence.
        public override bool? CanHitNPC(NPC target)
		{
			if (projectile.timeLeft >= 110)
			{
				return false;
			}
			return null;
		}

        public override bool CanHitPvp(Player target)
		{
			return projectile.timeLeft < 110;
		}
    }
}
