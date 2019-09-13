using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.Utilities;

namespace CalamityMod.NPCs.CeaselessVoid
{
    [AutoloadBossHead]
	public class CeaselessVoid : ModNPC
	{
		private float bossLife;
		private int beamPortal = 0;
		private int shootBoost = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ceaseless Void");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.damage = 150;
			npc.npcSlots = 36f;
			npc.width = 100; //324
			npc.height = 100; //216
			npc.defense = 0;
			npc.lifeMax = 200;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
			else
				music = MusicID.Boss3;
			if (CalamityWorld.DoGSecondStageCountdown <= 0)
			{
				npc.value = Item.buyPrice(0, 35, 0, 0);
				if (calamityModMusic != null)
					music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Void");
				else
					music = MusicID.Boss3;
			}
			npc.aiStyle = -1; //new
			aiType = -1; //new
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.boss = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(beamPortal);
			writer.Write(shootBoost);
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			beamPortal = reader.ReadInt32();
			shootBoost = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 0.15f;
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Texture2D texture = Main.npcTexture[npc.type];
			CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor, true);
			return false;
		}

		public override void AI()
		{
			double lifeRatio = (double)npc.life / (double)npc.lifeMax;
			int lifePercentage = (int)(100.0 * lifeRatio);
			Player player = Main.player[npc.target];
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			CalamityGlobalNPC.voidBoss = npc.whoAmI;
			Vector2 vector = npc.Center;
			npc.TargetClosest(true);
			if (NPC.CountNPCS(mod.NPCType("DarkEnergy")) > 0 ||
				NPC.CountNPCS(mod.NPCType("DarkEnergy2")) > 0 ||
				NPC.CountNPCS(mod.NPCType("DarkEnergy3")) > 0)
			{
				npc.dontTakeDamage = true;
			}
			else
			{
				npc.dontTakeDamage = false;
			}
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					npc.velocity = new Vector2(0f, -10f);
					CalamityWorld.DoGSecondStageCountdown = 0;
					if (Main.netMode == 2)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
						netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
						netMessage.Send();
					}
					if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else if (npc.timeLeft < 2400)
			{
				npc.timeLeft = 2400;
			}
			if (lifePercentage < 90)
			{
				float num472 = npc.Center.X;
				float num473 = npc.Center.Y;
				float num474 = (float)(500.0 * (1.0 - lifeRatio));
				if (!player.ZoneDungeon)
				{
					num474 *= 1.25f;
				}
				npc.ai[0] += 1f;
				if (npc.ai[0] == 60f)
				{
					npc.ai[0] = 0f;
					int numDust = (int)(0.2f * MathHelper.TwoPi * num474);
					float angleIncrement = MathHelper.TwoPi / (float)numDust;
					Vector2 dustOffset = new Vector2(num474, 0f);
					dustOffset = dustOffset.RotatedByRandom(MathHelper.TwoPi);
					for (int i = 0; i < numDust; i++)
					{
						dustOffset = dustOffset.RotatedBy(angleIncrement);
						int dust = Dust.NewDust(npc.Center, 1, 1, 173);
						Main.dust[dust].position = npc.Center + dustOffset;
						Main.dust[dust].noGravity = true;
						Main.dust[dust].fadeIn = 1f;
						Main.dust[dust].velocity *= 0f;
						Main.dust[dust].scale = 0.5f;
					}
					for (int num475 = 0; num475 < 255; num475++)
					{
						if (Collision.CanHit(npc.Center, 1, 1, Main.player[num475].Center, 1, 1))
						{
							float num476 = Main.player[num475].position.X + (float)(Main.player[num475].width / 2);
							float num477 = Main.player[num475].position.Y + (float)(Main.player[num475].height / 2);
							float num478 = Math.Abs(npc.position.X + (float)(npc.width / 2) - num476) + Math.Abs(npc.position.Y + (float)(npc.height / 2) - num477);
							if (num478 < num474)
							{
								if (Main.player[num475].position.X < num472)
								{
									Main.player[num475].velocity.X += 15f;
								}
								else
								{
									Main.player[num475].velocity.X -= 15f;
								}
								if (Main.player[num475].position.Y < num473)
								{
									Main.player[num475].velocity.Y += 15f;
								}
								else
								{
									Main.player[num475].velocity.Y -= 15f;
								}
							}
						}
					}
				}
			}
			if (Main.netMode != 1)
			{
				beamPortal += expertMode ? 2 : 1;
				beamPortal += shootBoost;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					beamPortal += 4;
				}
				if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
				{
					beamPortal += 2;
				}
				if (beamPortal >= 1200)
				{
					beamPortal = 0;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						float num941 = 3f; //speed
						Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
						float num942 = player.position.X + (float)player.width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
						float num943 = player.position.Y + (float)player.height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
						float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
						num944 = num941 / num944;
						num942 *= num944;
						num943 *= num944;
						int num945 = expertMode ? 42 : 58;
						int num946 = mod.ProjectileType("DoGBeamPortal");
						vector104.X += num942 * 5f;
						vector104.Y += num943 * 5f;
						int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num947].timeLeft = 300;
						npc.netUpdate = true;
					}
					if (lifePercentage < 50 && revenge)
					{
						float spread = 45f * 0.0174f;
						double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
						double deltaAngle = spread / 8f;
						double offsetAngle;
						int damage = expertMode ? 42 : 58;
						int i;
						float passedVar = 1f;
						for (i = 0; i < 4; i++)
						{
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)(Math.Sin(offsetAngle) * 3f), (float)(Math.Cos(offsetAngle) * 3f), mod.ProjectileType("DarkEnergyBall"), damage, 0f, Main.myPlayer, passedVar, 0f);
							passedVar += 1f;
						}
					}
				}
			}
			float num823 = 7.5f;
			float num824 = 0.08f;
			if (!player.ZoneDungeon)
			{
				num823 = 25f;
			}
			Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
			float num826 = player.position.Y + (float)(player.height / 2) - vector82.Y;
			float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
			num827 = num823 / num827;
			num825 *= num827;
			num826 *= num827;
			if (npc.velocity.X < num825)
			{
				npc.velocity.X = npc.velocity.X + num824;
				if (npc.velocity.X < 0f && num825 > 0f)
				{
					npc.velocity.X = npc.velocity.X + num824;
				}
			}
			else if (npc.velocity.X > num825)
			{
				npc.velocity.X = npc.velocity.X - num824;
				if (npc.velocity.X > 0f && num825 < 0f)
				{
					npc.velocity.X = npc.velocity.X - num824;
				}
			}
			if (npc.velocity.Y < num826)
			{
				npc.velocity.Y = npc.velocity.Y + num824;
				if (npc.velocity.Y < 0f && num826 > 0f)
				{
					npc.velocity.Y = npc.velocity.Y + num824;
				}
			}
			else if (npc.velocity.Y > num826)
			{
				npc.velocity.Y = npc.velocity.Y - num824;
				if (npc.velocity.Y > 0f && num826 < 0f)
				{
					npc.velocity.Y = npc.velocity.Y - num824;
				}
			}
			if (bossLife == 0f && npc.life > 0)
			{
				bossLife = (float)npc.lifeMax;
			}
			if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.26);
					if ((float)(npc.life + num660) < bossLife)
					{
						bossLife = (float)npc.life;
						shootBoost += 1;
						int glob = revenge ? 8 : 4;
						if (bossLife <= 0.5f)
						{
							glob = revenge ? 16 : 8;
						}
						for (int num662 = 0; num662 < glob; num662++)
						{
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType("DarkEnergySpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			damage = (crit ? 2 : 1);
			return false;
		}

		public override void NPCLoot()
		{
            // Only drop items if fought alone
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
			{
                // Materials
                DropHelper.DropItem(npc, mod.ItemType("DarkPlasma"), true, 2, 3);

                // Weapons
                DropHelper.DropItemChance(npc, mod.ItemType("MirrorBlade"), 3);

                // Equipment
                float f = Main.rand.NextFloat();
                bool replaceWithRare = f <= DropHelper.RareVariantDropRateFloat; // 1/40 chance overall of getting The Evolution
                if (f < 0.2f) // 1/5 chance of getting Arcanum of the Void OR The Evolution replacing it
                {
                    DropHelper.DropItemCondition(npc, mod.ItemType("ArcanumoftheVoid"), !replaceWithRare);
                    DropHelper.DropItemCondition(npc, mod.ItemType("TheEvolution"), replaceWithRare);
                }

                // Vanity
                DropHelper.DropItemChance(npc, mod.ItemType("CeaselessVoidTrophy"), 10);

                // Other
                bool lastSentinelKilled = !CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3;
                DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeSentinels"), true, lastSentinelKilled);
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSentinel1, 5, 2, 1);
            }

            // If DoG's fight is active, set the timer for the remaining two sentinels
            else if (CalamityWorld.DoGSecondStageCountdown > 14460)
            {
                CalamityWorld.DoGSecondStageCountdown = 14460;
                if (Main.netMode == 2)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Ceaseless Void as dead
            CalamityWorld.downedSentinel1 = true;
            CalamityMod.UpdateServerBoolean();
        }

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 100;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
				float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid2"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid3"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid4"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CeaselessVoid5"), 1f);
			}
		}
	}
}
