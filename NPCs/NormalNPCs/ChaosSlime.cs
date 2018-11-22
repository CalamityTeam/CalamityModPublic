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
	public class ChaosSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chaos Slime");
			Main.npcFrameCount[npc.type] = 2;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 50;
			npc.width = 40; //324
			npc.height = 30; //216
			npc.defense = 25;
			npc.lifeMax = 300;
			npc.knockBackResist = 0f;
			animationType = 81;
			npc.value = Item.buyPrice(0, 0, 40, 0);
			npc.alpha = 50;
			npc.lavaImmune = true;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedGolemBoss)
			{
				return 0f;
			}
			return SpawnCondition.Underworld.Chance * 0.06f;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("DWSparkle"), hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("DWSparkle"), hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ChaoticOre"), Main.rand.Next(10, 27));
		}
	}
}