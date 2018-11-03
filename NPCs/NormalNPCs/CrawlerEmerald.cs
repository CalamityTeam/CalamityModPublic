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
	public class CrawlerEmerald : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Emerald Crawler");
			Main.npcFrameCount[npc.type] = 5;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 0.3f;
			npc.aiStyle = 3;
			npc.damage = 20;
			npc.width = 44; //324
			npc.height = 34; //216
			npc.defense = 12;
			npc.lifeMax = 70;
			npc.knockBackResist = 0.55f;
			animationType = 257;
			aiType = NPCID.AnomuraFungus;
			npc.value = Item.buyPrice(0, 0, 0, 50);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath36;
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss)
			{
				return 0f;
			}
			return SpawnCondition.Cavern.Chance * 0.0275f;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 89, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 89, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Emerald, Main.rand.Next(2, 5));
		}
	}
}