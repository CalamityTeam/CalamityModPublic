using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.AstrumDeus
{
    public class AstrumDeusBody : ModNPC
	{
		private int spawn = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
		}

		public override void SetDefaults()
		{
			npc.damage = 90; //70
			npc.npcSlots = 5f;
			npc.width = 38; //324
			npc.height = 44; //216
			npc.defense = 45;
            npc.LifeMaxNERD(12000, 18000, 29100, 360000, 420000);
			double HPBoost = Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = 6; //new
			aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.2f;
			if (Main.expertMode)
			{
				npc.scale = 1.35f;
			}
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.dontCountMe = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(spawn);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.chaseable);
			writer.Write(npc.localAI[3]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			spawn = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
			npc.localAI[3] = reader.ReadSingle();
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override void AI()
		{
			if (CalamityGlobalNPC.astrumDeusHeadMain != -1)
			{
				if (Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].active)
				{
					npc.dontTakeDamage = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].dontTakeDamage;
					npc.chaseable = !Main.npc[CalamityGlobalNPC.astrumDeusHeadMain].chaseable;
				}
			}
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (!Main.npc[(int)npc.ai[1]].active)
			{
				npc.life = 0;
				npc.HitEffect(0, 10.0);
				npc.active = false;
			}
			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				if (npc.alpha != 0)
				{
					for (int num934 = 0; num934 < 2; num934++)
					{
						int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
						Main.dust[num935].noGravity = true;
						Main.dust[num935].noLight = true;
					}
				}
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int shootTime = 4;
				if ((double)npc.life <= (double)npc.lifeMax * 0.3 || CalamityWorld.bossRushActive)
				{
					shootTime += 2;
				}
				npc.localAI[0] += (float)Main.rand.Next(shootTime);
				if (npc.localAI[0] >= (float)Main.rand.Next(1400, 26000))
				{
					npc.localAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num941 = revenge ? 13f : 12f; //speed
						if (CalamityWorld.death || CalamityWorld.bossRushActive)
						{
							num941 = 18f;
						}
						Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
						float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
						float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
						float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
						num944 = num941 / num944;
						num942 *= num944;
						num943 *= num944;
						int num945 = expertMode ? 35 : 45;
						int num946 = mod.ProjectileType("AstralShot2");
						vector104.X += num942 * 5f;
						vector104.Y += num943 * 5f;
						int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
						npc.netUpdate = true;
					}
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return !npc.dontTakeDamage;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Color lightColor = new Color(125, 75, Main.DiscoB, npc.alpha); //250 150 Disco
			Color newColor = (npc.dontTakeDamage ? lightColor : drawColor);
			Texture2D texture = mod.GetTexture("NPCs/AstrumDeus/AstrumDeusBodyAlt");
			CalamityMod.DrawTexture(spriteBatch, (npc.localAI[3] == 1f ? texture : Main.npcTexture[npc.type]), 0, npc, newColor);
			return false;
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.type == mod.ProjectileType("RainbowBoom") || ProjectileID.Sets.StardustDragon[projectile.type])
			{
				damage = (int)((double)damage * 0.1);
			}
			else if (projectile.type == mod.ProjectileType("RainBolt") || projectile.type == mod.ProjectileType("AtlantisSpear2"))
			{
				damage = (int)((double)damage * 0.2);
			}
			else if (projectile.type == ProjectileID.DD2BetsyArrow || projectile.type == mod.ProjectileType("CraniumSmasherExplosive") || projectile.type == mod.ProjectileType("BigNukeExplosion"))
			{
				damage = (int)((double)damage * 0.3);
			}
			else if (projectile.type == mod.ProjectileType("SpikecragSpike"))
			{
				damage = (int)((double)damage * 0.5);
			}
			else if (projectile.type == mod.ProjectileType("GoliathExplosion"))
			{
				damage = (int)((double)damage * 0.6);
			}
			if (projectile.penetrate == -1 && !projectile.minion)
			{
				if (projectile.type == mod.ProjectileType("CosmicFire"))
					damage = (int)((double)damage * 0.3);
				else
					damage = (int)((double)damage * 0.2);
			}
			else if (projectile.penetrate > 1)
			{
				damage /= projectile.penetrate;
			}
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool PreNPCLoot()
		{
			return false;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.CountNPCS(mod.NPCType("AstrumDeusProbe3")) < 3 && CalamityWorld.revenge)
			{
				if (npc.life > 0 && Main.netMode != NetmodeID.MultiplayerClient && spawn == 0 && Main.rand.NextBool(25))
				{
					spawn = 1;
					int num660 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), mod.NPCType("AstrumDeusProbe3"), 0, 0f, 0f, 0f, 0f, 255);
					if (Main.netMode == NetmodeID.Server && num660 < 200)
					{
						NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
					}
					npc.netUpdate = true;
				}
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("AstralInfectionDebuff"), 180, true);
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}
