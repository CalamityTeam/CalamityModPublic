using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class HolySpear : ModProjectile
    {
		Vector2 velocity = Vector2.Zero;
		Vector2 providenceCenter = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Spear");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 200;
            cooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
			writer.WriteVector2(velocity);
			writer.WriteVector2(providenceCenter);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
			velocity = reader.ReadVector2();
			providenceCenter = reader.ReadVector2();
		}

        public override void AI()
        {
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;

				providenceCenter = Main.npc[CalamityGlobalNPC.holyBoss].Center;

				if (projectile.ai[0] == 1f)
					velocity = projectile.velocity;
			}

			float timeGateValue = 510f;
			float timeGateValue2 = 540f;
			int playerIndex = Player.FindClosest(projectile.Center, 1, 1);
			bool farFromProvidence = Vector2.Distance(Main.player[playerIndex].Center, providenceCenter) > 1920f;
			Vector2 velocity2 = Main.player[playerIndex].Center - projectile.Center;
			float scaleFactor = projectile.velocity.Length();
			velocity2.Normalize();
			velocity2 *= scaleFactor;

			if (projectile.ai[0] == 0f)
			{
				projectile.ai[1] += 1f;

				float slowGateValue = 90f;
				float fastGateValue = 30f;
				float minVelocity = 3f;
				float maxVelocity = 12f;
				float extremeVelocity = 24f;
				float deceleration = 0.95f;
				float acceleration = 1.2f;

				if (projectile.localAI[1] > timeGateValue)
				{
					if (projectile.velocity.Length() < extremeVelocity)
						projectile.velocity *= acceleration;
					else
					{
						projectile.velocity = (projectile.velocity * 10f + velocity2) / 11f;
						projectile.velocity.Normalize();
						projectile.velocity *= scaleFactor;
					}
				}
				else
				{
					if (projectile.ai[1] <= slowGateValue)
					{
						if (projectile.velocity.Length() > minVelocity)
							projectile.velocity *= deceleration;
					}
					else if (projectile.ai[1] < slowGateValue + fastGateValue)
					{
						if (projectile.velocity.Length() < maxVelocity)
							projectile.velocity *= acceleration;
					}
					else
						projectile.ai[1] = 0f;
				}
			}
			else
			{
				if (projectile.localAI[1] > timeGateValue)
				{
					projectile.velocity = (projectile.velocity * 10f + velocity2) / 11f;
					projectile.velocity.Normalize();
					projectile.velocity *= scaleFactor;
				}
				else
				{
					float frequency = 0.1f;
					float amplitude = 2f;

					projectile.ai[1] += frequency;

					float wavyVelocity = (float)Math.Sin(projectile.ai[1]);

					projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity).RotatedBy(MathHelper.ToRadians(velocity.ToRotation())) * amplitude;
				}
			}

			if (projectile.localAI[1] < timeGateValue2)
			{
				projectile.localAI[1] += 1f;

				if (projectile.timeLeft < 160)
					projectile.timeLeft = 160;
			}

			projectile.Opacity = MathHelper.Lerp(240f, 220f, projectile.timeLeft);

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			Texture2D value = Main.projectileTexture[projectile.type];
			int green = projectile.ai[0] != 0f ? 255 : 125;
			int blue = projectile.ai[0] != 0f ? 0 : 125;
			Color baseColor = new Color(255, green, blue, 255);

			if (!Main.dayTime)
			{
				int red = projectile.ai[0] != 0f ? 100 : 175;
				green = projectile.ai[0] != 0f ? 255 : 175;
				baseColor = new Color(red, green, 255, 255);
			}

			Color color33 = baseColor * 0.5f;
			color33.A = 0;
			Vector2 vector28 = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
			Color color34 = color33;
			Vector2 origin5 = value.Size() / 2f;
			Color color35 = color33 * 0.5f;
			float num162 = CalamityUtils.GetLerpValue(15f, 30f, projectile.timeLeft, clamped: true) * CalamityUtils.GetLerpValue(240f, 200f, projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
			Vector2 vector29 = new Vector2(1f, 1.5f) * num162;
			Vector2 vector30 = new Vector2(0.5f, 1f) * num162;
			color34 *= num162;
			color35 *= num162;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int i = 0; i < projectile.oldPos.Length; i++)
				{
					Vector2 drawPos = projectile.oldPos[i] + vector28;
					Color color = projectile.GetAlpha(color34) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);
					Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector29, spriteEffects, 0f);
					Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector30, spriteEffects, 0f);

					color = projectile.GetAlpha(color35) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);
					Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector29 * 0.6f, spriteEffects, 0f);
					Main.spriteBatch.Draw(value, drawPos, null, color, projectile.rotation, origin5, vector30 * 0.6f, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(value, vector28, null, color34, projectile.rotation, origin5, vector29, spriteEffects, 0);
			spriteBatch.Draw(value, vector28, null, color34, projectile.rotation, origin5, vector30, spriteEffects, 0);
			spriteBatch.Draw(value, vector28, null, color35, projectile.rotation, origin5, vector29 * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(value, vector28, null, color35, projectile.rotation, origin5, vector30 * 0.6f, spriteEffects, 0);

			return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			int buffType = Main.dayTime ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>();
			target.AddBuff(buffType, 120);
		}

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
