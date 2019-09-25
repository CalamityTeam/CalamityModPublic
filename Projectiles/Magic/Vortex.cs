using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Vortex : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vortex");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			projectile.width = 60;
			projectile.height = 60;
			projectile.alpha = 255;
			projectile.friendly = true;
			projectile.magic = true;
			projectile.penetrate = 10;
			projectile.timeLeft = 300;
			projectile.tileCollide = true;
			projectile.extraUpdates = 4;
			projectile.ignoreWater = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 5;
		}

		public override void AI()
		{
			projectile.localAI[1] += 1f;
			if (projectile.localAI[1] > 10f && Main.rand.NextBool(3))
			{
				int num713 = 5;
				for (int num714 = 0; num714 < num713; num714++)
				{
					Vector2 vector58 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width, (float)projectile.height) / 2f;
					vector58 = vector58.RotatedBy((double)(num714 - (num713 / 2 - 1)) * 3.1415926535897931 / (double)((float)num713), default) + projectile.Center;
					Vector2 value25 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
					int num715 = Dust.NewDust(vector58 + value25, 0, 0, 66, value25.X * 2f, value25.Y * 2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.7f);
					Main.dust[num715].noGravity = true;
					Main.dust[num715].noLight = true;
					Main.dust[num715].velocity /= 4f;
					Main.dust[num715].velocity -= projectile.velocity;
				}
				projectile.alpha -= 5;
				if (projectile.alpha < 50)
				{
					projectile.alpha = 50;
				}
				projectile.rotation += projectile.velocity.X * 0.1f;
				Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, ((float)Main.DiscoR / 200f), ((float)Main.DiscoG / 200f), ((float)Main.DiscoB / 200f));
			}
			int num716 = -1;
			Vector2 vector59 = projectile.Center;
			float num717 = 350f * projectile.ai[1];
			if (projectile.localAI[0] > 0f)
			{
				projectile.localAI[0] -= 1f;
			}
			if (projectile.ai[0] == 0f && projectile.localAI[0] == 0f)
			{
				for (int num718 = 0; num718 < 200; num718++)
				{
					NPC nPC6 = Main.npc[num718];
					if (nPC6.CanBeChasedBy(projectile, false) && (projectile.ai[0] == 0f || projectile.ai[0] == (float)(num718 + 1)))
					{
						Vector2 center4 = nPC6.Center;
						float num719 = Vector2.Distance(center4, vector59);
						if (num719 < num717 && Collision.CanHit(projectile.position, projectile.width, projectile.height, nPC6.position, nPC6.width, nPC6.height))
						{
							num717 = num719;
							vector59 = center4;
							num716 = num718;
						}
					}
				}
				if (num716 >= 0)
				{
					projectile.ai[0] = (float)(num716 + 1);
					projectile.netUpdate = true;
				}
			}
			if (projectile.localAI[0] == 0f && projectile.ai[0] == 0f)
			{
				projectile.localAI[0] = 30f;
			}
			bool flag32 = false;
			if (projectile.ai[0] != 0f)
			{
				int num720 = (int)(projectile.ai[0] - 1f);
				if (Main.npc[num720].active && !Main.npc[num720].dontTakeDamage && Main.npc[num720].immune[projectile.owner] == 0)
				{
					float num721 = Main.npc[num720].position.X + (float)(Main.npc[num720].width / 2);
					float num722 = Main.npc[num720].position.Y + (float)(Main.npc[num720].height / 2);
					float num723 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num721) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num722);
					if (num723 < 1000f)
					{
						flag32 = true;
						vector59 = Main.npc[num720].Center;
					}
				}
				else
				{
					projectile.ai[0] = 0f;
					flag32 = false;
					projectile.netUpdate = true;
				}
			}
			if (flag32)
			{
				Vector2 v = vector59 - projectile.Center;
				float num724 = projectile.velocity.ToRotation();
				float num725 = v.ToRotation();
				double num726 = (double)(num725 - num724);
				if (num726 > 3.1415926535897931)
				{
					num726 -= 6.2831853071795862;
				}
				if (num726 < -3.1415926535897931)
				{
					num726 += 6.2831853071795862;
				}
				projectile.velocity = projectile.velocity.RotatedBy(num726 * 0.10000000149011612, default);
			}
			float num727 = projectile.velocity.Length();
			projectile.velocity.Normalize();
			projectile.velocity *= num727 + 0.0025f;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
			return false;
		}
	}
}
