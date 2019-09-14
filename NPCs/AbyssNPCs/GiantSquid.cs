using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.AbyssNPCs
{
    public class GiantSquid : ModNPC
	{
		private bool hasBeenHit = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Squid");
			Main.npcFrameCount[npc.type] = 5;
		}

		public override void SetDefaults()
		{
			npc.noGravity = true;
			npc.damage = 80;
			npc.width = 50;
			npc.height = 220;
			npc.defense = 25;
			npc.lifeMax = 400;
			npc.aiStyle = -1;
			aiType = -1;
			npc.buffImmune[mod.BuffType("CrushDepth")] = true;
			npc.value = Item.buyPrice(0, 0, 15, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("GiantSquidBanner");
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(hasBeenHit);
			writer.Write(npc.chaseable);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			hasBeenHit = reader.ReadBoolean();
			npc.chaseable = reader.ReadBoolean();
		}

		public override void AI()
		{
			if (npc.direction == 0)
			{
				npc.TargetClosest(true);
			}
			if (!npc.wet)
			{
				if (npc.velocity.Y == 0f)
				{
					npc.velocity.X = npc.velocity.X * 0.98f;
					if ((double)npc.velocity.X > -0.01 && (double)npc.velocity.X < 0.01)
					{
						npc.velocity.X = 0f;
					}
				}
				npc.velocity.Y = npc.velocity.Y + 0.2f;
				if (npc.velocity.Y > 10f)
				{
					npc.velocity.Y = 10f;
				}
				npc.ai[0] = 1f;
				return;
			}
			if (npc.collideX)
			{
				npc.velocity.X = npc.velocity.X * -1f;
				npc.direction *= -1;
			}
			if (npc.collideY)
			{
				if (npc.velocity.Y > 0f)
				{
					npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
					npc.directionY = -1;
					npc.ai[0] = -1f;
				}
				else if (npc.velocity.Y < 0f)
				{
					npc.velocity.Y = Math.Abs(npc.velocity.Y);
					npc.directionY = 1;
					npc.ai[0] = 1f;
				}
			}
			npc.TargetClosest(false);
			if ((Main.player[npc.target].wet && !Main.player[npc.target].dead &&
				Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
				(Main.player[npc.target].Center - npc.Center).Length() < ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
				Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 300f : 500f) *
				(Main.player[npc.target].GetCalamityPlayer().fishAlert ? 3f : 1f)) ||
				npc.justHit)
			{
				hasBeenHit = true;
			}
			npc.chaseable = hasBeenHit;
			npc.rotation = npc.velocity.X * 0.02f;
			if (hasBeenHit)
			{
				npc.localAI[2] = 1f;
				npc.velocity *= 0.975f;
				float num263 = 1.6f;
				if (npc.velocity.X > -num263 && npc.velocity.X < num263 && npc.velocity.Y > -num263 && npc.velocity.Y < num263)
				{
					npc.TargetClosest(true);
					float num264 = 16f;
					Vector2 vector31 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num265 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector31.X;
					float num266 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector31.Y;
					float num267 = (float)Math.Sqrt((double)(num265 * num265 + num266 * num266));
					num267 = num264 / num267;
					num265 *= num267;
					num266 *= num267;
					npc.velocity.X = num265;
					npc.velocity.Y = num266;
					return;
				}
			}
			else
			{
				npc.localAI[2] = 0f;
				npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.02f;
				if (npc.velocity.X < -1f || npc.velocity.X > 1f)
				{
					npc.velocity.X = npc.velocity.X * 0.95f;
				}
				if (npc.ai[0] == -1f)
				{
					npc.velocity.Y = npc.velocity.Y - 0.01f;
					if (npc.velocity.Y < -1f)
					{
						npc.ai[0] = 1f;
					}
				}
				else
				{
					npc.velocity.Y = npc.velocity.Y + 0.01f;
					if (npc.velocity.Y > 1f)
					{
						npc.ai[0] = -1f;
					}
				}
				int num268 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
				int num269 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
				if (Main.tile[num268, num269 - 1] == null)
				{
					Main.tile[num268, num269 - 1] = new Tile();
				}
				if (Main.tile[num268, num269 + 1] == null)
				{
					Main.tile[num268, num269 + 1] = new Tile();
				}
				if (Main.tile[num268, num269 + 2] == null)
				{
					Main.tile[num268, num269 + 2] = new Tile();
				}
				if (Main.tile[num268, num269 - 1].liquid > 128)
				{
					if (Main.tile[num268, num269 + 1].active())
					{
						npc.ai[0] = -1f;
					}
					else if (Main.tile[num268, num269 + 2].active())
					{
						npc.ai[0] = -1f;
					}
				}
				else
				{
					npc.ai[0] = 1f;
				}
				if ((double)npc.velocity.Y > 1.2 || (double)npc.velocity.Y < -1.2)
				{
					npc.velocity.Y = npc.velocity.Y * 0.99f;
					return;
				}
			}
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (projectile.minion)
			{
				return hasBeenHit;
			}
			return null;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += (hasBeenHit ? 0.15f : 0.075f);
			npc.frameCounter %= Main.npcFrameCount[npc.type];
			int frame = (int)npc.frameCounter;
			npc.frame.Y = frame * frameHeight;
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
			vector -= new Vector2((float)mod.GetTexture("NPCs/AbyssNPCs/GiantSquidGlow").Width, (float)(mod.GetTexture("NPCs/AbyssNPCs/GiantSquidGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
			vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
			Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Cyan);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/AbyssNPCs/GiantSquidGlow"), vector,
				new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetCalamityPlayer().ZoneAbyssLayer2 && spawnInfo.water)
			{
				return SpawnCondition.CaveJellyfish.Chance * 0.6f;
			}
			if (spawnInfo.player.GetCalamityPlayer().ZoneAbyssLayer3 && spawnInfo.water)
			{
				return SpawnCondition.CaveJellyfish.Chance * 1.2f;
			}
			return 0f;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("CrushDepth"), 180, true);
			player.AddBuff(BuffID.Darkness, 180, true);
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("MarkedforDeath"), 120);
				player.AddBuff(mod.BuffType("Horror"), 120, true);
			}
		}

		public override void NPCLoot()
		{
			if (Main.rand.NextBool(1000000) && CalamityWorld.revenge)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HalibutCannon"));
			}
			if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
			{
				if (Main.rand.NextBool(2))
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Lumenite"));
				}
				if (Main.rand.NextBool(2))
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthCells"), Main.rand.Next(2, 5));
				}
				if (Main.expertMode && Main.rand.NextBool(2))
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthCells"), Main.rand.Next(1, 3));
				}
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 30; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
				}
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantSquid"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantSquid2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantSquid3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/GiantSquid4"), 1f);
			}
		}
	}
}
