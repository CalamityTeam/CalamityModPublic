using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using CalamityMod.World;

namespace CalamityMod.NPCs.Perforator
{
	[AutoloadBossHead]
	public class PerforatorHive : ModNPC
	{
		private bool small = false;
		private bool medium = false;
		private bool large = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Perforator Hive");
			Main.npcFrameCount[npc.type] = 10;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 18f;
			npc.damage = 35;
			npc.width = 110; //324
			npc.height = 100; //216
			npc.defense = 0;
			npc.lifeMax = CalamityWorld.revenge ? 5400 : 3750;
			if (CalamityWorld.death)
			{
				npc.lifeMax = 7600;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 3000000 : 2700000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.aiStyle = -1; //new
			aiType = -1; //new
			npc.buffImmune[mod.BuffType("GlacialState")] = true;
			npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 6, 0, 0);
			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit13;
			npc.DeathSound = SoundID.NPCDeath19;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/BloodCoagulant");
			else
				music = MusicID.Boss2;
			bossBag = mod.ItemType("PerforatorBag");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 0.15f;
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

		public override void AI()
		{
			CalamityGlobalNPC.perfHive = npc.whoAmI;
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 1.5f, 0f, 0f);
			Player player = Main.player[npc.target];
			bool isCrimson = player.ZoneCrimson || CalamityWorld.bossRushActive;
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			if (Vector2.Distance(player.Center, npc.Center) > 5600f)
			{
				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					npc.velocity = new Vector2(0f, 10f);
					if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
			{
				npc.timeLeft = 1800;
			}
			npc.TargetClosest(true);
			bool wormAlive = false;
			if (NPC.AnyNPCs(mod.NPCType("PerforatorHeadLarge")))
			{
				wormAlive = true;
			}
			if (wormAlive)
			{
				npc.dontTakeDamage = true;
			}
			else
			{
				npc.dontTakeDamage = isCrimson ? false : true;
			}
			if (Main.netMode != 1)
			{
				int shoot = revenge ? 6 : 4;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					shoot += 3;
				}
				npc.localAI[0] += (float)Main.rand.Next(shoot);
				if (npc.localAI[0] >= (float)Main.rand.Next(300, 900))
				{
					npc.localAI[0] = 0f;
					Main.PlaySound(3, (int)npc.position.X, (int)npc.position.Y, 20);
					for (int num621 = 0; num621 < 8; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 170, 0f, 0f, 100, default(Color), 1f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num622].scale = 0.25f;
							Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
						}
					}
					for (int num623 = 0; num623 < 16; num623++)
					{
						int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 1.5f);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 5f;
						num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 1f);
						Main.dust[num624].velocity *= 2f;
					}
					npc.TargetClosest(true);
					float num179 = 8f;
					Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
					float num181 = Math.Abs(num180) * 0.1f;
					float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
					float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
					npc.netUpdate = true;
					num183 = num179 / num183;
					num180 *= num183;
					num182 *= num183;
					int num184 = expertMode ? 14 : 18;
					int num185 = (Main.rand.Next(2) == 0 ? mod.ProjectileType("IchorShot") : mod.ProjectileType("BloodGeyser"));
					value9.X += num180;
					value9.Y += num182;
					for (int num186 = 0; num186 < 20; num186++)
					{
						num180 = player.position.X + (float)player.width * 0.5f - value9.X;
						num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
						num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
						num183 = num179 / num183;
						num180 += (float)Main.rand.Next(-180, 181);
						num180 *= num183;
						Projectile.NewProjectile(value9.X, value9.Y, num180, -5f, num185, num184, 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
			npc.rotation = npc.velocity.X * 0.04f;
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			if (npc.position.Y > player.position.Y - 160f) //200
			{
				if (npc.velocity.Y > 0f)
				{
					npc.velocity.Y = npc.velocity.Y * 0.98f;
				}
				npc.velocity.Y = npc.velocity.Y - 0.1f;
				if (npc.velocity.Y > 2f)
				{
					npc.velocity.Y = 2f;
				}
			}
			else if (npc.position.Y < player.position.Y - 400f) //500
			{
				if (npc.velocity.Y < 0f)
				{
					npc.velocity.Y = npc.velocity.Y * 0.98f;
				}
				npc.velocity.Y = npc.velocity.Y + 0.1f;
				if (npc.velocity.Y < -2f)
				{
					npc.velocity.Y = -2f;
				}
			}
			if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 80f)
			{
				if (npc.velocity.X > 0f)
				{
					npc.velocity.X = npc.velocity.X * 0.98f;
				}
				npc.velocity.X = npc.velocity.X - 0.1f;
				if (npc.velocity.X > 8f)
				{
					npc.velocity.X = 8f;
				}
			}
			if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 80f)
			{
				if (npc.velocity.X < 0f)
				{
					npc.velocity.X = npc.velocity.X * 0.98f;
				}
				npc.velocity.X = npc.velocity.X + 0.1f;
				if (npc.velocity.X < -8f)
				{
					npc.velocity.X = -8f;
				}
			}
			if (npc.ai[3] == 0f && npc.life > 0)
			{
				npc.ai[3] = (float)npc.lifeMax;
			}
			if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.3);
					if ((float)(npc.life + num660) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						int wormType = mod.NPCType("PerforatorHeadSmall");
						if (!small)
						{
							small = true;
						}
						else if (!medium)
						{
							medium = true;
							wormType = mod.NPCType("PerforatorHeadMedium");
						}
						else if (!large)
						{
							large = true;
							wormType = mod.NPCType("PerforatorHeadLarge");
						}
						NPC.SpawnOnPlayer(npc.FindClosestPlayer(), wormType);
						return;
					}
				}
			}
		}

		public override bool CheckDead()
		{
			if (NPC.AnyNPCs(mod.NPCType("PerforatorHeadLarge")))
			{
				return false;
			}
			return true;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.HealingPotion;
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PerforatorTrophy"));
			}
			if (CalamityWorld.armageddon)
			{
				for (int i = 0; i < 5; i++)
				{
					npc.DropBossBags();
				}
			}
			if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			else
			{
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PerforatorMask"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Aorta"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SausageMaker"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodyRupture"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodBath"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VeinBurster"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Eviscerator"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ToothBall"), Main.rand.Next(25, 51));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodClotStaff"));
				}
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodSample"), Main.rand.Next(7, 15));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Vertebrae, Main.rand.Next(3, 10));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CrimtaneBar, Main.rand.Next(2, 6));
				if (Main.hardMode)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Ichor, Main.rand.Next(10, 21));
				}
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive4"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 100;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}