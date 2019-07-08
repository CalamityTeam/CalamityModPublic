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

namespace CalamityMod.NPCs.AbyssNPCs
{
	public class MirageJelly : ModNPC
	{
		private bool teleporting = false;
		private bool rephasing = false;
		private bool hasBeenHit = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mirage Jelly");
			Main.npcFrameCount[npc.type] = 7;
		}

		public override void SetDefaults()
		{
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.damage = 100;
			npc.width = 70;
			npc.height = 162;
			npc.defense = 10;
			npc.lifeMax = 6000;
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.buffImmune[mod.BuffType("CrushDepth")] = true;
			npc.value = Item.buyPrice(0, 0, 25, 0);
			npc.HitSound = SoundID.NPCHit25;
			npc.DeathSound = SoundID.NPCDeath28;
			banner = npc.type;
			bannerItem = mod.ItemType("MirageJellyBanner");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(hasBeenHit);
			writer.Write(teleporting);
			writer.Write(rephasing);
			writer.Write(npc.chaseable);
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			hasBeenHit = reader.ReadBoolean();
			teleporting = reader.ReadBoolean();
			rephasing = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
		}

		public override void AI()
		{
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			npc.velocity *= 0.985f;
			if (npc.velocity.Y > -0.3f)
			{
				npc.velocity.Y = -3f;
			}
			if (npc.justHit)
			{
				if (Main.rand.Next(10) == 0)
				{
					teleporting = true;
				}
				hasBeenHit = true;
			}
			if (npc.ai[0] == 0f)
			{
				npc.chaseable = true;
				npc.dontTakeDamage = false;
				if (Main.netMode != 1)
				{
					if (teleporting)
					{
						teleporting = false;
						npc.TargetClosest(true);
						int num1249 = 0;
						int num1250;
						int num1251;
						while (true)
						{
							num1249++;
							num1250 = (int)player.Center.X / 16;
							num1251 = (int)player.Center.Y / 16;
							num1250 += Main.rand.Next(-50, 51);
							num1251 += Main.rand.Next(-50, 51);
							if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, player.position, player.width, player.height) &&
								Main.tile[num1250, num1251].liquid > 204)
							{
								break;
							}
							if (num1249 > 100)
							{
								goto Block;
							}
						}
						npc.ai[0] = 1f;
						npc.ai[1] = (float)num1250;
						npc.ai[2] = (float)num1251;
						npc.netUpdate = true;
					Block:;
					}
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.damage = 0;
				npc.chaseable = false;
				npc.dontTakeDamage = true;
				npc.alpha += 5;
				if (npc.alpha >= 255)
				{
					npc.alpha = 255;
					npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
					npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
					npc.ai[0] = 2f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.alpha -= 5;
				if (npc.alpha <= 0)
				{
					npc.damage = Main.expertMode ? 160 : 80;
					npc.chaseable = true;
					npc.dontTakeDamage = false;
					npc.alpha = 0;
					npc.ai[0] = 0f;
					npc.netUpdate = true;
				}
			}
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
			Vector2 vector = center - Main.screenPosition;
			vector -= new Vector2((float)mod.GetTexture("NPCs/AbyssNPCs/MirageJellyGlow").Width, (float)(mod.GetTexture("NPCs/AbyssNPCs/MirageJellyGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Purple);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/AbyssNPCs/MirageJellyGlow"), vector,
				new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += (hasBeenHit ? 0.15f : 0.1f);
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyssLayer3 && spawnInfo.water)
			{
				return SpawnCondition.CaveJellyfish.Chance * 0.6f;
			}
			return 0f;
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(1000000) == 0 && CalamityWorld.revenge)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HalibutCannon"));
			}
			if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
			{
				if (Main.rand.Next(2) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthCells"), Main.rand.Next(5, 8));
				}
				if (Main.expertMode && Main.rand.Next(2) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthCells"), Main.rand.Next(5, 8));
				}
			}
			if (Main.expertMode)
			{
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VitalJelly"));
				}
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LifeJelly"));
				}
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ManaJelly"));
				}
			}
			else
			{
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VitalJelly"));
				}
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LifeJelly"));
				}
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ManaJelly"));
				}
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Venom, 240, true);
			player.AddBuff(BuffID.Electrified, 60, true);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}