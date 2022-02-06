using CalamityMod.Buffs.StatDebuffs;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Events;
using CalamityMod.World;

namespace CalamityMod.NPCs.OldDuke
{
	public class OldDukeSharkron : ModNPC
	{
		bool spawnedProjectiles = false;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sulphurous Sharkron");
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}
		
		public override void SetDefaults()
		{
			npc.Calamity().canBreakPlayerDefense = true;
			npc.aiStyle = -1;
			aiType = -1;
			npc.width = 44;
			npc.height = 44;
			npc.GetNPCDamage();
			npc.defense = 100;
			npc.lifeMax = 6000;
			if (BossRushEvent.BossRushActive)
			{
				npc.lifeMax = 10000;
			}
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.knockBackResist = 0f;
			npc.alpha = 255;
			npc.noGravity = true;
			npc.dontTakeDamage = true;
			npc.noTileCollide = true;
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.noGravity);
			writer.Write(spawnedProjectiles);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.noGravity = reader.ReadBoolean();
			spawnedProjectiles = reader.ReadBoolean();
		}

		public override void AI()
		{
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(false);
				npc.netUpdate = true;
			}

			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2(-npc.velocity.Y, -npc.velocity.X);
			}
			else
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);
			}

			npc.alpha -= 6;
			if (npc.alpha < 0)
				npc.alpha = 0;

			// Fly towards Old Duke
			bool normalAI = npc.ai[3] == 0f;

			// Fly up
			bool upwardAI = npc.ai[3] < 0f;

			// Fly down
			bool downwardAI = npc.ai[3] > 0f;

			float flyTowardTargetGateValue = malice ? 60f : death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
			float extraTime = malice ? 90f : death ? 100f : revenge ? 105f : expertMode ? 110f : 120f;
			float aiGateValue = flyTowardTargetGateValue + extraTime;
			if (!normalAI)
				aiGateValue -= extraTime * 0.3f;
			float explodeIntoGoreGateValue = aiGateValue + extraTime;
			float fallDownGateValue = aiGateValue + extraTime * 0.5f;
			float maxVelocity = malice ? 24f : death ? 22f : revenge ? 21f : expertMode ? 20f : 18f;

			if (npc.ai[0] == 0f)
			{
				// Set velocity to fly towards a specified location on the first frame
				if (npc.ai[1] == 0f)
				{
					if (normalAI)
						npc.velocity = Vector2.Normalize(Main.npc[(int)npc.ai[2]].Center - npc.Center) * (maxVelocity - 6f);
					else
						npc.velocity = new Vector2(npc.ai[2], npc.ai[3]);

					Main.PlaySound(SoundID.NPCDeath19, npc.position);
				}

				// Fly towards a target after a certain time has passed
				npc.ai[1] += 1f;
				if (npc.ai[1] >= flyTowardTargetGateValue)
				{
					// Start second part of AI if not inside tiles and a certain time has passed
					if (!Collision.SolidCollision(npc.position, npc.width, npc.height) && npc.ai[1] >= aiGateValue)
						npc.ai[0] = 1f;

					// If not set to fly towards Old Duke from the start, accelerate
					if (!normalAI)
					{
						if (npc.velocity.Length() < maxVelocity)
							npc.velocity *= 1.01f;
					}

					// Fly towards the target
					float scaleFactor2 = npc.velocity.Length();
					Vector2 vector17 = Main.player[npc.target].Center - npc.Center;
					vector17.Normalize();
					vector17 *= scaleFactor2;
					float inertia = malice ? 16f : death ? 18f : revenge ? 20f : expertMode ? 22f : 25f;
					npc.velocity = (npc.velocity * (inertia - 1f) + vector17) / inertia;
					npc.velocity.Normalize();
					npc.velocity *= scaleFactor2;
				}
			}
			else if (npc.ai[0] == 1f)
			{
				// Move slower if set to fly upward from the start
				if (upwardAI)
					maxVelocity -= 6f;

				// Decrease velocity if moving faster than max
				if (npc.velocity.Length() > maxVelocity)
					npc.velocity *= 0.99f;

				npc.dontTakeDamage = false;

				// Explode into gores if colliding with tiles or after a certain time has passed
				npc.ai[1] += 1f;
				if (Collision.SolidCollision(npc.position, npc.width, npc.height) || npc.ai[1] >= explodeIntoGoreGateValue)
				{
					if (npc.DeathSound != null)
						Main.PlaySound(npc.DeathSound, npc.position);

					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					return;
				}

				// Fall down after a certain time has passed
				if (npc.ai[1] >= fallDownGateValue)
				{
					npc.noGravity = false;
					npc.velocity.Y += 0.3f;
				}
			}
		}

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(8) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			return false;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
			if (npc.spriteDirection == -1)
				spriteEffects = SpriteEffects.None;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.Lime;
			float amount9 = 0.5f;
			int num153 = 10;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return npc.alpha == 0;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<Irradiated>(), 240);
		}

        public override bool CheckDead()
		{
			Main.PlaySound(SoundID.NPCDeath12, npc.position);

			npc.position.X = npc.position.X + (npc.width / 2);
            npc.position.Y = npc.position.Y + (npc.height / 2);
            npc.width = npc.height = 96;
            npc.position.X = npc.position.X - (npc.width / 2);
            npc.position.Y = npc.position.Y - (npc.height / 2);

			for (int num621 = 0; num621 < 15; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2f);
				Main.dust[num622].velocity.Y *= 6f;
				Main.dust[num622].velocity.X *= 3f;
				if (Main.rand.NextBool(2))
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}

			for (int num623 = 0; num623 < 30; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 3f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity.Y *= 10f;
				num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 2f);
				Main.dust[num624].velocity.X *= 2f;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient && !spawnedProjectiles)
			{
				spawnedProjectiles = true;
				int spawnX = npc.width / 2;
				int type = ModContent.ProjectileType<OldDukeGore>();
				int damage = npc.GetProjectileDamage(type);
				for (int i = 0; i < 2; i++)
					Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-spawnX, spawnX), npc.Center.Y,
						Main.rand.Next(-3, 4), Main.rand.Next(-12, -6), type, damage, 0f, Main.myPlayer);
			}

			return true;
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}