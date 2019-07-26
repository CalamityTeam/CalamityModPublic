using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class Piggy : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Piggy");
			Main.npcFrameCount[npc.type] = 5;
		}

		public override void SetDefaults()
		{
			npc.chaseable = false;
			npc.aiStyle = 26;
			npc.damage = 0;
			npc.width = 26;
			npc.height = 26;
			npc.defense = 0;
			npc.lifeMax = 2000;
			npc.aiStyle = 7;
			aiType = 299;
			npc.knockBackResist = 0.99f;
			npc.value = Item.buyPrice(0, 10, 0, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			/*banner = npc.type;
			bannerItem = mod.ItemType("PitbullBanner");*/
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.TownCritter.Chance * 0.01f;
		}

		public override void FindFrame(int frameHeight)
		{
			if (npc.velocity.Y == 0f)
			{
				if (npc.direction == 1)
				{
					npc.spriteDirection = -1;
				}
				if (npc.direction == -1)
				{
					npc.spriteDirection = 1;
				}
				if (npc.velocity.X == 0f)
				{
					npc.frame.Y = 0;
					npc.frameCounter = 0.0;
					return;
				}
				npc.frameCounter += (double)(Math.Abs(npc.velocity.X) * 0.25f);
				npc.frameCounter += 1.0;
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y / frameHeight >= Main.npcFrameCount[npc.type] - 1)
				{
					npc.frame.Y = frameHeight;
				}
			}
			else
			{
				npc.frameCounter = 0.0;
				npc.frame.Y = frameHeight * 2;
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Bacon);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
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
