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
	public class EbonianBlightSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ebonian Blight Slime");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 30;
			npc.width = 60; //324
			npc.height = 42; //216
			npc.defense = 12;
			npc.lifeMax = 130;
			npc.knockBackResist = 0.3f;
			animationType = 244;
			npc.value = Item.buyPrice(0, 0, 5, 0);
			npc.alpha = 105;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 40; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss)
			{
				return 0f;
			}
			return SpawnCondition.Corruption.Chance * 0.15f;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.ManaSickness, 100, true);
				player.AddBuff(BuffID.Weak, 260, true);
			}
			else
			{
				player.AddBuff(BuffID.ManaSickness, 80, true);
				player.AddBuff(BuffID.Weak, 200, true);
			}
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EbonianGel"), Main.rand.Next(15, 17));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Gel, Main.rand.Next(10, 15));
		}
	}
}