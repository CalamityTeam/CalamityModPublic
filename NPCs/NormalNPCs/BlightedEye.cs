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
	public class BlightedEye : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blighted Eye");
			Main.npcFrameCount[npc.type] = 2;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 2;
			npc.damage = 32;
			npc.width = 30; //324
			npc.height = 32; //216
			npc.defense = 18;
			npc.lifeMax = 120;
			npc.knockBackResist = 0.6f;
			animationType = 2;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
			{
				return 0f;
			}
			return SpawnCondition.OverworldNightMonster.Chance * 0.075f;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.Weak, 100, true);
			}
			else
			{
				player.AddBuff(BuffID.Weak, 60, true);
			}
		}
		
		public override void NPCLoot()
		{
			if (Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BlightedLens"));
			}
		}
	}
}