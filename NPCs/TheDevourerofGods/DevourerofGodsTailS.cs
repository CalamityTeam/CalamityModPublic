using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.TheDevourerofGods
{
    [AutoloadBossHead]
	public class DevourerofGodsTailS : ModNPC
	{
		private int invinceTime = 720;
		private bool setAlpha = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Devourer of Gods");
		}

		public override void SetDefaults()
		{
			npc.damage = 180;
			npc.npcSlots = 5f;
			npc.width = 80;
			npc.height = 140;
			npc.defense = 50;
			npc.lifeMax = CalamityWorld.revenge ? 1350000 : 1150000;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 2100000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 10000000 : 9200000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.takenDamageMultiplier = 1.25f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			npc.boss = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse");
			else
				music = MusicID.LunarBoss;
			npc.dontCountMe = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(invinceTime);
			writer.Write(setAlpha);
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			invinceTime = reader.ReadInt32();
			setAlpha = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override void BossHeadRotation(ref float rotation)
		{
			rotation = npc.rotation;
		}

		public override void AI()
		{
			if (invinceTime > 0)
			{
				invinceTime--;
				npc.dontTakeDamage = true;
			}
			else
			{
				npc.dontTakeDamage = false;
			}
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			npc.velocity.Length();
			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = -1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = 1;
			}
			bool flag = false;
			if (npc.ai[1] <= 0f)
			{
				flag = true;
			}
			else if (Main.npc[(int)npc.ai[1]].life <= 0)
			{
				flag = true;
			}
			if (flag)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.checkDead();
			}
			if (CalamityGlobalNPC.DoGHead < 0 || !Main.npc[CalamityGlobalNPC.DoGHead].active)
			{
				npc.active = false;
			}
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (Main.npc[(int)npc.ai[1]].alpha < 128 && !setAlpha)
			{
				npc.alpha -= 42;
				if (npc.alpha <= 0 && invinceTime <= 0)
				{
					setAlpha = true;
					npc.alpha = 0;
				}
			}
			else
			{
				npc.alpha = Main.npc[(int)npc.ai[2]].alpha;
			}
			if (Main.player[npc.target].dead)
			{
				npc.TargetClosest(false);
			}
			Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			num191 = (float)((int)(num191 / 16f) * 16);
			num192 = (float)((int)(num192 / 16f) * 16);
			vector18.X = (float)((int)(vector18.X / 16f) * 16);
			vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
			if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
			{
				try
				{
					vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
					num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
				}
				catch
				{
				}
				npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
				num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
				int num194 = npc.width;
				num193 = (num193 - (float)num194) / num193;
				num191 *= num193;
				num192 *= num193;
				npc.velocity = Vector2.Zero;
				npc.position.X = npc.position.X + num191;
				npc.position.Y = npc.position.Y + num192;
				if (num191 < 0f)
				{
					npc.spriteDirection = -1;
				}
				else if (num191 > 0f)
				{
					npc.spriteDirection = 1;
				}
			}
		}

		// Can only hit the target if within certain distance
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 0;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist) minDist = dist2;
			if (dist3 < minDist) minDist = dist3;
			if (dist4 < minDist) minDist = dist4;

			return minDist <= 70f && npc.alpha == 0;
		}

		// Projectiles can only hit within certain distance
		/*public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			Rectangle projectileHitbox = projectile.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, projectileHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, projectileHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, projectileHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, projectileHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist) minDist = dist2;
			if (dist3 < minDist) minDist = dist3;
			if (dist4 < minDist) minDist = dist4;

			return minDist <= 70f;
		}*/

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS5"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS6"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 10; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 20; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
            return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool PreNPCLoot()
		{
			return false;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.85f);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("GodSlayerInferno"), 180, true);
			player.AddBuff(mod.BuffType("WhisperingDeath"), 240, true);
			int num = Main.rand.Next(2);
			string key = "Mods.CalamityMod.EdgyBossText8";
			if (num == 0)
			{
				key = "Mods.CalamityMod.EdgyBossText8";
			}
			else if (num == 1)
			{
				key = "Mods.CalamityMod.EdgyBossText9";
			}
			Color messageColor = Color.Cyan;
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				Main.NewText(Language.GetTextValue(key), messageColor);
			}
			else if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
			}
			player.AddBuff(BuffID.Frostburn, 180, true);
			player.AddBuff(BuffID.Darkness, 180, true);
		}
	}
}
