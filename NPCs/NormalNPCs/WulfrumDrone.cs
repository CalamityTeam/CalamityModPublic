using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class WulfrumDrone : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wulfrum Drone");
			Main.npcFrameCount[npc.type] = 5;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 10;
			npc.aiStyle = 3;
			aiType = 73;
			npc.width = 40; //324
			npc.height = 30; //216
			npc.defense = 6;
			npc.lifeMax = 22;
			npc.knockBackResist = 0.35f;
			npc.value = Item.buyPrice(0, 0, 0, 50);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
		}
		
		public override void AI()
		{
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
		}
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || Main.hardMode || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
			{
				return 0f;
			}
			return SpawnCondition.OverworldDaySlime.Chance * 0.2f;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WulfrumShard"), Main.rand.Next(1, 4));
			if (Main.expertMode && Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WulfrumShard"));
			}
		}
	}
}